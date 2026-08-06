using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public class GRNDAL
    {
        public DataTable GetApprovedPOsForReceipt()
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(AppConfig.ConnectionString))
                using (SqlCommand cmd = new SqlCommand(@"
                    SELECT
                        PO.ID,
                        PO.DBOMNo AS PORef,
                        PO.ProjectCode,
                        PO.VendorCode,
                        PO.ItemCode,
                        PO.UOM,
                        PO.RequariedQty AS OrderedQty,
                        ISNULL(PO.RemainingQty, PO.RequariedQty) AS RemainingQty,
                        PO.UnitPrice,
                        PO.Amount,
                        PO.POGeneratedBy,
                        PO.POGeneratedDate
                    FROM PurchaseOrder PO
                    WHERE PO.POApproved = 1
                      AND ISNULL(PO.RemainingQty, PO.RequariedQty) > 0
                    ORDER BY PO.ID DESC", con))
                {
                    con.Open();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        da.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading open POs:\n" + ex.Message, "GRN", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return dt;
        }

        public string GetNextGRNNumber()
        {
            string prefix = $"GRN-{DateTime.Now:yyyy-MM}-";
            int next = 1;
            try
            {
                using (SqlConnection con = new SqlConnection(AppConfig.ConnectionString))
                using (SqlCommand cmd = new SqlCommand(@"
                    SELECT MAX(GRNNumber) FROM ProcurementGRN WHERE GRNNumber LIKE @Prefix + '%'", con))
                {
                    cmd.Parameters.AddWithValue("@Prefix", prefix);
                    con.Open();
                    object result = cmd.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        string last = result.ToString() ?? "";
                        string[] parts = last.Split('-');
                        if (parts.Length >= 3 && int.TryParse(parts[^1], out int n))
                            next = n + 1;
                    }
                }
            }
            catch { /* table may not exist yet */ }
            return prefix + next.ToString("D5");
        }

        public string CreateGRN(
            string poRef,
            string vendorCode,
            string projectCode,
            string receivedBy,
            string remarks,
            DataTable lines,
            out string error)
        {
            error = "";
            if (lines == null || lines.Rows.Count == 0)
            {
                error = "No lines to receive.";
                return null;
            }

            string grnNumber = GetNextGRNNumber();
            try
            {
                using (SqlConnection con = new SqlConnection(AppConfig.ConnectionString))
                {
                    con.Open();
                    using (SqlTransaction tx = con.BeginTransaction())
                    {
                        int grnId;
                        using (SqlCommand cmd = new SqlCommand(@"
                            INSERT INTO ProcurementGRN
                                (GRNNumber, PORef, VendorCode, ProjectCode, ReceivedBy, Remarks, CreatedBy, Status)
                            VALUES
                                (@GRN, @PORef, @Vendor, @Project, @By, @Remarks, @By, 'Received');
                            SELECT CAST(SCOPE_IDENTITY() AS INT);", con, tx))
                        {
                            cmd.Parameters.AddWithValue("@GRN", grnNumber);
                            cmd.Parameters.AddWithValue("@PORef", (object?)poRef ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@Vendor", (object?)vendorCode ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@Project", (object?)projectCode ?? DBNull.Value);
                            cmd.Parameters.AddWithValue("@By", receivedBy ?? "");
                            cmd.Parameters.AddWithValue("@Remarks", (object?)remarks ?? DBNull.Value);
                            grnId = Convert.ToInt32(cmd.ExecuteScalar());
                        }

                        foreach (DataRow row in lines.Rows)
                        {
                            int poId = Convert.ToInt32(row["POID"]);
                            string itemCode = row["ItemCode"].ToString() ?? "";
                            decimal recvQty = Convert.ToDecimal(row["ReceivedQty"]);
                            if (recvQty <= 0) continue;

                            using (SqlCommand cmd = new SqlCommand(@"
                                INSERT INTO ProcurementGRNDetail
                                    (GRNID, GRNNumber, POID, ItemCode, OrderedQty, ReceivedQty, UnitPrice, Amount, UOM, Remarks)
                                VALUES
                                    (@GRNID, @GRN, @POID, @Item, @Ordered, @Recv, @Price, @Amount, @UOM, @Remarks)", con, tx))
                            {
                                cmd.Parameters.AddWithValue("@GRNID", grnId);
                                cmd.Parameters.AddWithValue("@GRN", grnNumber);
                                cmd.Parameters.AddWithValue("@POID", poId);
                                cmd.Parameters.AddWithValue("@Item", itemCode);
                                cmd.Parameters.AddWithValue("@Ordered", Convert.ToDecimal(row["OrderedQty"]));
                                cmd.Parameters.AddWithValue("@Recv", recvQty);
                                cmd.Parameters.AddWithValue("@Price", Convert.ToDecimal(row["UnitPrice"]));
                                cmd.Parameters.AddWithValue("@Amount", Convert.ToDecimal(row["Amount"]));
                                cmd.Parameters.AddWithValue("@UOM", (object?)row["UOM"] ?? DBNull.Value);
                                cmd.Parameters.AddWithValue("@Remarks", (object?)row["Remarks"] ?? DBNull.Value);
                                cmd.ExecuteNonQuery();
                            }

                            // Reduce remaining qty on PO line
                            using (SqlCommand cmd = new SqlCommand(@"
                                UPDATE PurchaseOrder
                                SET RemainingQty = CASE
                                        WHEN ISNULL(RemainingQty, RequariedQty) - @Recv < 0 THEN 0
                                        ELSE ISNULL(RemainingQty, RequariedQty) - @Recv
                                    END
                                WHERE ID = @POID", con, tx))
                            {
                                cmd.Parameters.AddWithValue("@Recv", recvQty);
                                cmd.Parameters.AddWithValue("@POID", poId);
                                cmd.ExecuteNonQuery();
                            }

                            // Best-effort write into legacy Receipt table (columns vary by DB)
                            TryInsertReceipt(con, tx, poRef, itemCode, recvQty,
                                Convert.ToDecimal(row["UnitPrice"]),
                                row["UOM"]?.ToString(),
                                projectCode,
                                vendorCode,
                                receivedBy,
                                grnNumber);
                        }

                        tx.Commit();
                        return grnNumber;
                    }
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return null;
            }
        }

        private static void TryInsertReceipt(
            SqlConnection con, SqlTransaction tx,
            string poRef, string itemCode, decimal qty, decimal unitCost,
            string uom, string projectCode, string vendorCode, string receivedBy, string ginNumber)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand(@"
                    INSERT INTO Receipt (RefNumber, ItemCode, Quantity, UnitCost, Units, VendorCode, GINNumber, Type, CreatedBy, CreatedDate)
                    VALUES (@Ref, @Item, @Qty, @Cost, @UOM, @Vendor, @GIN, 'Boughtout', @By, GETDATE())", con, tx))
                {
                    cmd.Parameters.AddWithValue("@Ref", poRef ?? "");
                    cmd.Parameters.AddWithValue("@Item", itemCode ?? "");
                    cmd.Parameters.AddWithValue("@Qty", qty);
                    cmd.Parameters.AddWithValue("@Cost", unitCost);
                    cmd.Parameters.AddWithValue("@UOM", (object?)uom ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Vendor", (object?)vendorCode ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@GIN", ginNumber ?? "");
                    cmd.Parameters.AddWithValue("@By", receivedBy ?? "");
                    cmd.ExecuteNonQuery();
                }
            }
            catch
            {
                // Receipt schema differs across environments — GRN tables remain source of truth
            }
        }

        public DataTable GetRecentGRNs(int topN = 50)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(AppConfig.ConnectionString))
                using (SqlCommand cmd = new SqlCommand(@"
                    SELECT TOP (@Top)
                        GRNNumber, PORef, VendorCode, ProjectCode, ReceivedBy, ReceivedDate, Status, Remarks
                    FROM ProcurementGRN
                    ORDER BY ReceivedDate DESC", con))
                {
                    cmd.Parameters.AddWithValue("@Top", topN);
                    con.Open();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        da.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("GRN history error (run Phase3_Schema_Alignment.sql):\n" + ex.Message,
                    "GRN", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            return dt;
        }
    }
}
