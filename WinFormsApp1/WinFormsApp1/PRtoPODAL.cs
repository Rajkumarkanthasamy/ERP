//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Text;
//using WinFormsApp1;

//namespace WinFormsApp1
//{
//    internal class PRtoPODAL
//    {
//    }
//}


using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public class PRtoPODAL
    {
        private DataAccessLayer _dal;

        public PRtoPODAL()
        {
            _dal = new DataAccessLayer();
            _dal.fnGetConnectionString();
        }

        /// <summary>
        /// Get Approved PRs that are ready to convert to PO
        /// </summary>
        public DataTable GetApprovedPRsForPO()
        {
            DataTable dt = new DataTable();
            try
            {
                _dal.SQLCon.Open();
                string query = @"
                    SELECT 
                        PR.PRID,
                        PR.PRNumber,
                        PR.ProjectCode,
                        PR.VendorCode,
                        ISNULL(V.VendorName, PR.VendorCode) AS VendorName,
                        PR.TotalAmount,
                        PR.Status,
                        PR.RequestedBy,
                        PR.RequestDate,
                        PR.ApprovedBy,
                        PR.ApprovedDate,
                        PR.Remarks,
                        PR.IsClubbed,
                        PR.ClubbedFromPRIDs
                    FROM PurchaseRequest PR
                    LEFT JOIN Vendors V ON PR.VendorCode = V.VendorCode
                    WHERE PR.Status IN (@Approved, @Partial)
                      AND EXISTS (
                          SELECT 1 FROM PurchaseRequestDetailNew D
                          WHERE D.PRNumber = PR.PRNumber
                            AND ISNULL(D.LineStatus, @PendingLine) = @PendingLine
                      )
                    ORDER BY PR.ApprovedDate DESC";

                _dal.SQLCmd = new SqlCommand(query, _dal.SQLCon);
                _dal.SQLCmd.Parameters.AddWithValue("@Approved", PRStatus.Approved);
                _dal.SQLCmd.Parameters.AddWithValue("@Partial", PRStatus.PartiallyConverted);
                _dal.SQLCmd.Parameters.AddWithValue("@PendingLine", PRLineStatus.Pending);
                _dal.SQLDadpr = new SqlDataAdapter(_dal.SQLCmd);
                _dal.SQLDadpr.Fill(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading approved PRs: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (_dal.SQLCon.State == ConnectionState.Open)
                    _dal.SQLCon.Close();
            }
            return dt;
        }

        /// <summary>
        /// Get PR Detail lines for a specific PR (ready for PO conversion)
        /// </summary>
        public DataTable GetPRDetailForPO(string selectedPRNumber)
        {
            DataTable dt = new DataTable();
            try
            {
                _dal.SQLCon.Open();
                string query = @"
                    SELECT 
                        PRD.DetailID,
                        PRD.PRID,
                        PRD.PRNumber,
                        PRD.ProjectCode,
                        PRD.ProductCode,
                        PRD.ProductNo,
                        PRD.ItemCode,
                        
                        PRD.ItemDescription,
                        PRD.Specification,
                        PRD.Make,
                        PRD.MfgPartNo,
                        PRD.Quantity AS RequariedQty,
                        PRD.UOM,
                        PRD.UnitCost AS UnitPrice,
                        PRD.TotalCost AS Amount,
                        PRD.Quantity AS RemainingQty,
                        PRD.VendorCode,
                        PRD.VendorName,
                        PRD.DrawingNo,
                        PRD.Location,
                        PRD.HSNCode,
                        PRD.BOMCode,
                        PRD.BOMQuantity AS BOMQty,
                        PRD.Remarks,
                        PRD.CreatedBy,
                        PRD.CreatedDate
                    FROM PurchaseRequestDetailNew PRD
                    WHERE PRD.PRNumber = @PRNumber
                      AND PRD.LineStatus = 'Pending'
                    ORDER BY PRD.DetailID";

                _dal.SQLCmd = new SqlCommand(query, _dal.SQLCon);
                _dal.SQLCmd.Parameters.AddWithValue("@PRNumber", selectedPRNumber);
                _dal.SQLDadpr = new SqlDataAdapter(_dal.SQLCmd);
                _dal.SQLDadpr.Fill(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading PR details: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (_dal.SQLCon.State == ConnectionState.Open)
                    _dal.SQLCon.Close();
            }
            return dt;
        }

        /// <summary>
        /// Convert PR to PO - Insert into existing PurchaseOrder table
        /// Maps all PR fields to your existing PO table columns
        /// </summary>
        public bool ConvertPRtoPO(PRtoPOModel poModel)
        {
            bool success = false;
            try
            {
                _dal.SQLCon.Open();

                string query = @"
                    INSERT INTO PurchaseOrder 
                    (
                        DBOMNo, ProjectCode, VendorCode, PreparedBy, AuthoriedBy,
                        ItemCode, UOM, RequariedQty, UnitPrice, Amount, RemainingQty,
                        POGeneratedBy, POGeneratedDate, Remarks, POPreparedDate,
                        PODeliveryDate, ProjectStatus, BOMQty, BOMProjectList,
                        POApproved, WithoutBOM, Currency, FXRate,
                        CreatedDate
                    )
                    VALUES 
                    (
                        @DBOMNo, @ProjectCode, @VendorCode, @PreparedBy, @AuthoriedBy,
                        @ItemCode, @UOM, @RequariedQty, @UnitPrice, @Amount, @RemainingQty,
                        @POGeneratedBy, @POGeneratedDate, @Remarks, @POPreparedDate,
                        @PODeliveryDate, @ProjectStatus, @BOMQty, @BOMProjectList,
                        @POApproved, @WithoutBOM, @Currency, @FXRate,
                        GETDATE()
                    );
                    SELECT SCOPE_IDENTITY();";

                _dal.SQLCmd = new SqlCommand(query, _dal.SQLCon);

                _dal.SQLCmd.Parameters.AddWithValue("@DBOMNo", poModel.DBOMNo);
                _dal.SQLCmd.Parameters.AddWithValue("@ProjectCode", poModel.ProjectCode);
                _dal.SQLCmd.Parameters.AddWithValue("@VendorCode", poModel.VendorCode);
                _dal.SQLCmd.Parameters.AddWithValue("@PreparedBy", string.IsNullOrEmpty(poModel.PreparedBy) ? (object)DBNull.Value : poModel.PreparedBy);
                _dal.SQLCmd.Parameters.AddWithValue("@AuthoriedBy", string.IsNullOrEmpty(poModel.AuthoriedBy) ? (object)DBNull.Value : poModel.AuthoriedBy);
                _dal.SQLCmd.Parameters.AddWithValue("@ItemCode", poModel.ItemCode);
                _dal.SQLCmd.Parameters.AddWithValue("@UOM", string.IsNullOrEmpty(poModel.UOM) ? (object)DBNull.Value : poModel.UOM);
                _dal.SQLCmd.Parameters.AddWithValue("@RequariedQty", poModel.RequariedQty);
                _dal.SQLCmd.Parameters.AddWithValue("@UnitPrice", poModel.UnitPrice);
                _dal.SQLCmd.Parameters.AddWithValue("@Amount", poModel.Amount);
                _dal.SQLCmd.Parameters.AddWithValue("@RemainingQty", poModel.RemainingQty);
                _dal.SQLCmd.Parameters.AddWithValue("@POGeneratedBy", string.IsNullOrEmpty(poModel.POGeneratedBy) ? (object)DBNull.Value : poModel.POGeneratedBy);
                _dal.SQLCmd.Parameters.AddWithValue("@POGeneratedDate", poModel.POGeneratedDate);
                _dal.SQLCmd.Parameters.AddWithValue("@Remarks", string.IsNullOrEmpty(poModel.Remarks) ? (object)DBNull.Value : poModel.Remarks);
                _dal.SQLCmd.Parameters.AddWithValue("@POPreparedDate", poModel.POPreparedDate);
                _dal.SQLCmd.Parameters.AddWithValue("@PODeliveryDate", string.IsNullOrEmpty(poModel.PODeliveryDate) ? (object)DBNull.Value : poModel.PODeliveryDate);
                _dal.SQLCmd.Parameters.AddWithValue("@ProjectStatus", string.IsNullOrEmpty(poModel.ProjectStatus) ? (object)DBNull.Value : poModel.ProjectStatus);
                _dal.SQLCmd.Parameters.AddWithValue("@BOMQty", poModel.BOMQty);
                _dal.SQLCmd.Parameters.AddWithValue("@BOMProjectList", string.IsNullOrEmpty(poModel.BOMProjectList) ? (object)DBNull.Value : poModel.BOMProjectList);
                _dal.SQLCmd.Parameters.AddWithValue("@POApproved", poModel.POApproved);
                _dal.SQLCmd.Parameters.AddWithValue("@WithoutBOM", poModel.WithoutBOM);
                _dal.SQLCmd.Parameters.AddWithValue("@Currency", string.IsNullOrEmpty(poModel.Currency) ? (object)DBNull.Value : poModel.Currency);
                _dal.SQLCmd.Parameters.AddWithValue("@FXRate", poModel.FXRate);

                object result = _dal.SQLCmd.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    poModel.ID = Convert.ToInt32(result);
                    success = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error converting PR to PO: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (_dal.SQLCon.State == ConnectionState.Open)
                    _dal.SQLCon.Close();
            }
            return success;
        }

        /// <summary>
        /// Bulk convert multiple PR lines to PO
        /// </summary>
        public int BulkConvertPRtoPO(List<PRtoPOModel> poList)
        {
            int insertedCount = 0;
            try
            {
                _dal.SQLCon.Open();
                foreach (var po in poList)
                {
                    string query = @"
                        INSERT INTO PurchaseOrder 
                        (
                            DBOMNo, ProjectCode, VendorCode, PreparedBy, AuthoriedBy,
                            ItemCode, UOM, RequariedQty, UnitPrice, Amount, RemainingQty,
                            POGeneratedBy, POGeneratedDate, Remarks, POPreparedDate,
                            PODeliveryDate, ProjectStatus, BOMQty, BOMProjectList,
                            POApproved, WithoutBOM, Currency, FXRate
                           
                        )
                        VALUES 
                        (
                            @DBOMNo, @ProjectCode, @VendorCode, @PreparedBy, @AuthoriedBy,
                            @ItemCode, @UOM, @RequariedQty, @UnitPrice, @Amount, @RemainingQty,
                            @POGeneratedBy, @POGeneratedDate, @Remarks, @POPreparedDate,
                            @PODeliveryDate, @ProjectStatus, @BOMQty, @BOMProjectList,
                            @POApproved, @WithoutBOM, @Currency, @FXRate
                        )";

                    _dal.SQLCmd = new SqlCommand(query, _dal.SQLCon);
                    _dal.SQLCmd.Parameters.AddWithValue("@DBOMNo", po.DBOMNo);
                    _dal.SQLCmd.Parameters.AddWithValue("@ProjectCode", po.ProjectCode);
                    _dal.SQLCmd.Parameters.AddWithValue("@VendorCode", po.VendorCode);
                    _dal.SQLCmd.Parameters.AddWithValue("@PreparedBy", string.IsNullOrEmpty(po.PreparedBy) ? (object)DBNull.Value : po.PreparedBy);
                    _dal.SQLCmd.Parameters.AddWithValue("@AuthoriedBy", string.IsNullOrEmpty(po.AuthoriedBy) ? (object)DBNull.Value : po.AuthoriedBy);
                    _dal.SQLCmd.Parameters.AddWithValue("@ItemCode", po.ItemCode);
                    _dal.SQLCmd.Parameters.AddWithValue("@UOM",
                        string.IsNullOrWhiteSpace(po.UOM) ? "Nos" : po.UOM);
                    _dal.SQLCmd.Parameters.AddWithValue("@RequariedQty", po.RequariedQty);
                    _dal.SQLCmd.Parameters.AddWithValue("@UnitPrice", po.UnitPrice);
                    _dal.SQLCmd.Parameters.AddWithValue("@Amount", po.Amount);
                    _dal.SQLCmd.Parameters.AddWithValue("@RemainingQty", po.RemainingQty);
                    _dal.SQLCmd.Parameters.AddWithValue("@POGeneratedBy", string.IsNullOrEmpty(po.POGeneratedBy) ? (object)DBNull.Value : po.POGeneratedBy);
                    _dal.SQLCmd.Parameters.AddWithValue("@POGeneratedDate", po.POGeneratedDate);
                    _dal.SQLCmd.Parameters.AddWithValue("@Remarks", string.IsNullOrEmpty(po.Remarks) ? (object)DBNull.Value : po.Remarks);
                    _dal.SQLCmd.Parameters.AddWithValue("@POPreparedDate", po.POPreparedDate);
                    _dal.SQLCmd.Parameters.AddWithValue("@PODeliveryDate", string.IsNullOrEmpty(po.PODeliveryDate) ? (object)DBNull.Value : po.PODeliveryDate);
                    _dal.SQLCmd.Parameters.AddWithValue("@ProjectStatus", string.IsNullOrEmpty(po.ProjectStatus) ? (object)DBNull.Value : po.ProjectStatus);
                    _dal.SQLCmd.Parameters.AddWithValue("@BOMQty", po.BOMQty);
                    _dal.SQLCmd.Parameters.AddWithValue("@BOMProjectList", string.IsNullOrEmpty(po.BOMProjectList) ? (object)DBNull.Value : po.BOMProjectList);
                    _dal.SQLCmd.Parameters.AddWithValue("@POApproved", po.POApproved);
                    _dal.SQLCmd.Parameters.AddWithValue("@WithoutBOM", po.WithoutBOM);
                    _dal.SQLCmd.Parameters.AddWithValue("@Currency", string.IsNullOrEmpty(po.Currency) ? (object)DBNull.Value : po.Currency);
                    _dal.SQLCmd.Parameters.AddWithValue("@FXRate", po.FXRate);
                   

                    insertedCount += _dal.SQLCmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in bulk PO conversion: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (_dal.SQLCon.State == ConnectionState.Open)
                    _dal.SQLCon.Close();
            }
            return insertedCount;
        }

        /// <summary>
        /// Update PR line status to 'Converted to PO' after successful conversion
        /// </summary>
        public bool UpdatePRLineStatusToConverted(int detailID)
        {
            bool success = false;
            try
            {
                _dal.SQLCon.Open();
                string query = @"UPDATE PurchaseRequestDetailNew SET LineStatus = @LineStatus, ModifiedDate = GETDATE() WHERE DetailID = @DetailID";
                _dal.SQLCmd = new SqlCommand(query, _dal.SQLCon);
                _dal.SQLCmd.Parameters.AddWithValue("@DetailID", detailID);
                _dal.SQLCmd.Parameters.AddWithValue("@LineStatus", PRLineStatus.ConvertedToPO);
                success = _dal.SQLCmd.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating PR line status: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (_dal.SQLCon.State == ConnectionState.Open)
                    _dal.SQLCon.Close();
            }
            return success;
        }

        /// <summary>
        /// Sets PR header to Partially Converted or Fully Converted based on remaining Pending lines.
        /// </summary>
        public string RefreshPRConversionStatus(string prNumber)
        {
            string status = PRStatus.Approved;
            try
            {
                _dal.SQLCon.Open();
                _dal.SQLCmd = new SqlCommand(@"
                    SELECT
                        SUM(CASE WHEN ISNULL(LineStatus, 'Pending') = @Pending THEN 1 ELSE 0 END) AS PendingLines,
                        COUNT(*) AS TotalLines
                    FROM PurchaseRequestDetailNew
                    WHERE PRNumber = @PRNumber", _dal.SQLCon);
                _dal.SQLCmd.Parameters.AddWithValue("@PRNumber", prNumber);
                _dal.SQLCmd.Parameters.AddWithValue("@Pending", PRLineStatus.Pending);

                int pending = 0;
                int total = 0;
                using (SqlDataReader reader = _dal.SQLCmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        pending = reader["PendingLines"] != DBNull.Value ? Convert.ToInt32(reader["PendingLines"]) : 0;
                        total = reader["TotalLines"] != DBNull.Value ? Convert.ToInt32(reader["TotalLines"]) : 0;
                    }
                }

                if (total == 0)
                    status = PRStatus.Approved;
                else if (pending == 0)
                    status = PRStatus.FullyConverted;
                else if (pending < total)
                    status = PRStatus.PartiallyConverted;
                else
                    status = PRStatus.Approved;

                _dal.SQLCmd = new SqlCommand(@"
                    UPDATE PurchaseRequest
                    SET Status = @Status, ModifiedDate = GETDATE()
                    WHERE PRNumber = @PRNumber", _dal.SQLCon);
                _dal.SQLCmd.Parameters.AddWithValue("@Status", status);
                _dal.SQLCmd.Parameters.AddWithValue("@PRNumber", prNumber);
                _dal.SQLCmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error refreshing PR conversion status: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (_dal.SQLCon.State == ConnectionState.Open)
                    _dal.SQLCon.Close();
            }
            return status;
        }

        /// <summary>
        /// Get PO list generated from PRs
        /// </summary>
        public DataTable GetPOListFromPR(string prNumber)
        {
            DataTable dt = new DataTable();
            try
            {
                _dal.SQLCon.Open();
                string query = @"
                    SELECT 
                        ID, DBOMNo, ProjectCode, VendorCode, ItemCode,
                        UOM, RequariedQty, UnitPrice, Amount, RemainingQty,
                        POGeneratedBy, POGeneratedDate, POApproved,
                        POPreparedDate, PODeliveryDate, Remarks
                    FROM PurchaseOrder
                    WHERE DBOMNo = @DBOMNo
                    ORDER BY ID";

                _dal.SQLCmd = new SqlCommand(query, _dal.SQLCon);
                _dal.SQLCmd.Parameters.AddWithValue("@DBOMNo", prNumber);
                _dal.SQLDadpr = new SqlDataAdapter(_dal.SQLCmd);
                _dal.SQLDadpr.Fill(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading PO list: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (_dal.SQLCon.State == ConnectionState.Open)
                    _dal.SQLCon.Close();
            }
            return dt;
        }
    }
}