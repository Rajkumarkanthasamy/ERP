//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace WinFormsApp1
//{
//    internal class PurchaseRequestDAL
//    {
//    }
//}


using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public class PurchaseRequestDAL
    {
        public string cs = AppConfig.ConnectionString;
        public SqlConnection SQLCon;
        public SqlCommand SQLCmd;
        public SqlDataAdapter SQLDadpr;
        public DataSet SQLDataset;

        public PurchaseRequestDAL()
        {
            fnGetConnectionString();
        }

        public void fnGetConnectionString()
        {
            try
            {
                cs = AppConfig.ConnectionString;
                SQLCon = new SqlConnection(cs);
                SQLCmd = new SqlCommand();
                SQLDadpr = new SqlDataAdapter();
                SQLDataset = new DataSet();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Connection error: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public bool fnCheckConnection()
        {
            try
            {
                SQLCon.Open();
                SQLCon.Close();
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Server not found. " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                SQLCon.Close();
                return false;
            }
        }

        #region === BOM to PR Conversion ===


        public DataTable GetBOMProjects(string type, string code)
        {
            DataTable dt = new DataTable();
            try
            {
                SQLCon.Open();
                if (type == "projectcode")
                {
                    SQLCmd = new SqlCommand(@"
                        SELECT DISTINCT ProjectBOM.ProjectCode
                        FROM ProjectBOM
                        JOIN ProjectMaster ON ProjectMaster.ProjectCode = ProjectBOM.ProjectCode
                        WHERE ProjectMaster.Status = 'WIP'
                        ORDER BY ProjectBOM.ProjectCode", SQLCon);
                }
                else if (type == "product")
                {
                    SQLCmd = new SqlCommand(@"
                        SELECT DISTINCT ProductCode
                        FROM ProjectBOM
                        WHERE ProjectCode = @ProjectCode
                          AND ISNULL(ProductCode, '') <> ''
                        ORDER BY ProductCode", SQLCon);
                    SQLCmd.Parameters.AddWithValue("@ProjectCode", code ?? "");
                }
                else
                {
                    return dt;
                }

                SQLDadpr = new SqlDataAdapter(SQLCmd);
                SQLDadpr.Fill(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Get BOM Items", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SQLCon.Close();
            }
            return dt;
        }

        public DataTable GetBOMItemsByProject(string projectCode, string productNo = null)
        {
            DataTable dt = new DataTable();
            try
            {
                SQLCon.Open();
                SQLCmd = new SqlCommand(@$"SELECT
                                            MachineBOM.ProjectBOMCode,
                                            MachineBOM.ProductNo,
                                            PB.ProductCode,
                                            MachineBOM.ItemName,
                                            ItemMaster.ItemDescription,
                                            MachineBOM.Quantity AS BOMQty,

                                           ISNULL((SELECT SUM(IssuedQuantity) FROM Issued where Project_Code=MachineBOM.ProjectCode and ProductNo=MachineBOM.ProductNo and Item_Code=MachineBOM.ItemName), 0) as IssuedQty,
                                           MachineBOM.Quantity - ISNULL((SELECT SUM(IssuedQuantity) FROM Issued where Project_Code=MachineBOM.ProjectCode and ProductNo=MachineBOM.ProductNo and Item_Code=MachineBOM.ItemName), 0) AS BOMBalanceIssueQty,

                                            -- Converted Qty
                                            ISNULL((
                                                SELECT SUM(ConvertedQty)
                                                FROM BOMtoPRConversion
                                                WHERE BOMProjectCode = MachineBOM.ProjectCode
                                                 -- AND ProductNo = MachineBOM.ProductNo
                                                  AND ItemCode = MachineBOM.ItemName
                                            ), 0) AS ConvertedQty,

                                            -- BOM Balance Issue Qty
   

                                            -- Pending PO QTY (Fixed: Added ORDER BY for deterministic TOP 1)
                                            ISNULL((
                                                SELECT TOP 1
                                                    pob.POQuantity - ISNULL(SUM(r.Quantity), 0) AS RemainingQty
                                                FROM PurchaseOrderBOM pob
                                                LEFT JOIN Receipt r 
                                                    ON r.RefNumber = pob.PONumber
                                                    AND r.ItemCode = pob.ItemCode
                                                    AND r.ProductNo = pob.ProductNo
                                                    AND r.BOMProjectCode = pob.ProjectCode
                                                WHERE pob.ProjectCode = MachineBOM.ProjectCode
                                                  AND pob.ItemCode = MachineBOM.ItemName
                                                  AND pob.ProductNo = MachineBOM.ProductNo
                                                GROUP BY 
                                                    pob.PONumber,
                                                    pob.ProjectCode,
                                                    pob.ProductNo,
                                                    pob.ItemCode,
                                                    pob.POQuantity
                                                ORDER BY RemainingQty DESC  -- Deterministic: shows highest remaining first
                                            ), 0) AS [Pending PO QTY],

                                            -- Vendor Code (Fixed: Added ORDER BY for deterministic TOP 1)
                                            ISNULL((
                                                SELECT TOP 1
                                                    MAX(PO.VendorCode) AS VendorCode
                                                FROM PurchaseOrderBOM pob
                                                LEFT JOIN Receipt r 
                                                    ON r.RefNumber = pob.PONumber
                                                    AND r.ItemCode = pob.ItemCode
                                                    AND r.ProductNo = pob.ProductNo
                                                    AND r.BOMProjectCode = pob.ProjectCode
                                                JOIN PurchaseOrder PO 
                                                    ON PO.DBOMNo = pob.PONumber
                                                WHERE pob.ProjectCode = MachineBOM.ProjectCode
                                                  AND pob.ItemCode = MachineBOM.ItemName
                                                  AND pob.ProductNo = MachineBOM.ProductNo
                                                GROUP BY 
                                                    pob.PONumber,
                                                    pob.ProjectCode,
                                                    pob.ProductNo,
                                                    pob.ItemCode
                                                ORDER BY pob.PONumber DESC  -- Deterministic: latest PO first
                                            ), '-') AS VendorCode,

                                            ItemMaster.AvailableQty,
                                            ItemMaster.UnitCost,
                                            MachineBOM.ProjectCode,
                                            ItemMaster.Location,
                                            ItemMaster.HSNSACCode,
                                            MachineBOM.Vendor,
                                            MachineBOM.DrawingNo,
                                            MachineBOM.ExtraItem,
                                            MachineBOM.ProjectStatus,
                                            MachineBOM.AddedBy,
                                            MachineBOM.AddedDate,
                                            MachineBOM.ReturnQty,
                                            MachineBOM.POQty,
                                            MachineBOM.ProductType,
                                            MachineBOM.FixedCost

                                        FROM [ERP_Database].[dbo].[MachineBOM] AS MachineBOM

                                        -- FIX: Changed from LEFT JOIN to OUTER APPLY (SELECT TOP 1)
                                        -- This prevents duplicate rows when ProjectBOM has multiple entries
                                        -- for the same ProjectCode + ProductNo combination.
                                        OUTER APPLY (
                                            SELECT TOP 1 
                                                ProductCode
                                            FROM ProjectBOM AS PB
                                            WHERE PB.ProjectCode = MachineBOM.ProjectCode
                                              AND PB.ProductNo = MachineBOM.ProductNo
                                        ) AS PB

                                        INNER JOIN ItemMaster 
                                            ON ItemMaster.ItemCode = MachineBOM.ItemName

                                            WHERE MachineBOM.ProjectCode = @ProjectCode
                                              AND (
                                                    @ProductFilter IS NULL
                                                    OR PB.ProductCode = @ProductFilter
                                                    OR MachineBOM.ProductNo = @ProductFilter
                                                  )
                                            ORDER BY MachineBOM.ItemName;", SQLCon);
                SQLCmd.Parameters.AddWithValue("@ProjectCode", projectCode ?? "");
                SQLCmd.Parameters.AddWithValue("@ProductFilter",
                    string.IsNullOrWhiteSpace(productNo) ? (object)DBNull.Value : productNo);
                SQLDadpr = new SqlDataAdapter(SQLCmd);
                SQLDadpr.Fill(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Get BOM Items", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SQLCon.Close();
            }
            return dt;
        }

        public DataTable GetVendorsForItem(string itemCode)
        {
            DataTable dt = new DataTable();
            try
            {
                SQLCon.Open();
                SQLCmd = new SqlCommand("SELECT ItemCode,VendorCode FROM Receipt where ItemCode='"+ itemCode + "'", SQLCon);
               // SQLCmd.CommandType = CommandType.StoredProcedure;
               // SQLCmd.Parameters.AddWithValue("@ItemCode", itemCode);
                SQLDadpr = new SqlDataAdapter(SQLCmd);
                SQLDadpr.Fill(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Get Vendors", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SQLCon.Close();
            }
            return dt;
        }

        public DataTable CheckItemInPR(string projectCode, string productNo, string itemCode)
        {
            DataTable dt = new DataTable();
            try
            {
                SQLCon.Open();
                SQLCmd = new SqlCommand("sp_PR_CheckItemInPR", SQLCon);
                SQLCmd.CommandType = CommandType.StoredProcedure;
                SQLCmd.Parameters.AddWithValue("@ProjectCode", projectCode);
                SQLCmd.Parameters.AddWithValue("@ProductNo", productNo);
                SQLCmd.Parameters.AddWithValue("@ItemCode", itemCode);
                SQLDadpr = new SqlDataAdapter(SQLCmd);
                SQLDadpr.Fill(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Check PR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SQLCon.Close();
            }
            return dt;
        }

        /// <summary>
        /// Returns the next PR number in format PR-YYYY-MM-#######
        /// </summary>
        public string GetNextPRNumber()
        {
            string prefix = $"PR-{DateTime.Now.Year}-{DateTime.Now.Month:D2}-";
            int nextSeq = 1;

            try
            {
                SQLCon.Open();
                SQLCmd = new SqlCommand(@"
                    SELECT MAX(PRNumber)
                    FROM PurchaseRequest
                    WHERE PRNumber LIKE @Prefix + '%'", SQLCon);
                SQLCmd.Parameters.AddWithValue("@Prefix", prefix);

                object result = SQLCmd.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    string last = result.ToString() ?? "";
                    string[] parts = last.Split('-');
                    if (parts.Length >= 4 && int.TryParse(parts[3], out int current))
                        nextSeq = current + 1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Get PR Number", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SQLCon.Close();
            }

            return prefix + nextSeq.ToString("D7");
        }

        public string GetVendorName(string vendorCode)
        {
            string VendorName = "";
            //try
            //{
            //    SQLCon.Open();
            //    SQLCmd = new SqlCommand("SELECT TOP (1) PRNumber FROM PurchaseRequest order by PRNumber desc", SQLCon);
            //    SQLCmd.CommandType = CommandType.StoredProcedure;
            //    object result = SQLCmd.ExecuteScalar();
            //    if (result != null)
            //        prNumber = result.ToString();
            //}


            try
            {
                SQLCon.Open();

                SQLCmd = new SqlCommand(
                    "SELECT VendorName FROM Vendors WHERE VendorCode = @VendorCode",
                    SQLCon);
                SQLCmd.Parameters.AddWithValue("@VendorCode", vendorCode ?? "");

                object result = SQLCmd.ExecuteScalar();

                if (result != null)
                    VendorName = result.ToString();
            }

            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Get VendorName", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SQLCon.Close();
            }
            return VendorName;
        }

        public int InsertPRHeader(string prNumber, string projectCode,
            string vendorCode, string vendorName, decimal totalAmount, string currency, float fxRate,
            string remarks, string createdBy)
        {
            int newId = 0;
            try
            {
                SQLCon.Open();
                SQLCmd = new SqlCommand(@"INSERT INTO PurchaseRequest
                                            (
                                                PRNumber,
                                                ProjectCode,
                                                VendorCode,
                                                VendorName,
                                                TotalAmount,
                                                Status,
                                                Remarks,
                                                CreatedBy,
                                                RequestedBy,
                                                RequestDate,
                                                ModifiedBy,
                                                ModifiedDate,
                                                IsClubbed
                                            )
                                            VALUES
                                            (
                                                @PRNumber,
                                                @ProjectCode,
                                                @VendorCode,
                                                @VendorName,
                                                @TotalAmount,
                                                @Status,
                                                @Remarks,
                                                @CreatedBy,
                                                @CreatedBy,
                                                GETDATE(),
                                                @ModifiedBy,
                                                GETDATE(),
                                                0
                                            );
                                            SELECT CAST(SCOPE_IDENTITY() AS INT);", SQLCon);
                SQLCmd.Parameters.AddWithValue("@PRNumber", prNumber);
                SQLCmd.Parameters.AddWithValue("@ProjectCode", projectCode);
                SQLCmd.Parameters.AddWithValue("@VendorCode", string.IsNullOrEmpty(vendorCode) ? (object)DBNull.Value : vendorCode);
                SQLCmd.Parameters.AddWithValue("@VendorName", string.IsNullOrEmpty(vendorName) ? (object)DBNull.Value : vendorName);
                SQLCmd.Parameters.AddWithValue("@TotalAmount", totalAmount);
                SQLCmd.Parameters.AddWithValue("@Status", PRStatus.Pending);
                SQLCmd.Parameters.AddWithValue("@Remarks", string.IsNullOrEmpty(remarks) ? (object)DBNull.Value : remarks);
                SQLCmd.Parameters.AddWithValue("@CreatedBy", createdBy);
                SQLCmd.Parameters.AddWithValue("@ModifiedBy", createdBy);
                object result = SQLCmd.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                    newId = Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Insert PR Header", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
            finally
            {
                SQLCon.Close();
            }
            return newId;
        }

        public int InsertPRDetail(PurchaseRequestDetail detail)
        {
            int newId = 0;
            try
            {
                SQLCon.Open();
                SQLCmd = new SqlCommand(@"INSERT INTO PurchaseRequestDetailNew
                                        (
                                            PRID,
                                            PRNumber,
                                            ProjectBOMCode,
                                            ProjectCode,
                                            ProductCode,
                                            ProductNo,
                                            ItemCode,
                                            ItemDescription,
                                            Specification,
                                            Make,
                                            MfgPartNo,
                                            Quantity,
                                            UOM,
                                            UnitCost,
                                            TotalCost,
                                            VendorCode,
                                            VendorName,
                                            DrawingNo,
                                            Location,
                                            HSNCode,
                                            BOMQuantity,
                                            AlreadyPurchasedQty,
                                            BalanceQty,
                                            LineStatus,
                                            CreatedBy,
                                            ModifiedBy,
                                            CreatedDate
                                        )
                                        VALUES
                                        (
                                            @PRID,
                                            @PRNumber,
                                            @ProjectBOMCode,
                                            @ProjectCode,
                                            @ProductCode,
                                            @ProductNo,
                                            @ItemCode,
                                            @ItemDescription,
                                            @Specification,
                                            @Make,
                                            @MfgPartNo,
                                            @Quantity,
                                            @UOM,
                                            @UnitCost,
                                            @TotalCost,
                                            @VendorCode,
                                            @VendorName,
                                            @DrawingNo,
                                            @Location,
                                            @HSNCode,
                                            @BOMQuantity,
                                            @AlreadyPurchasedQty,
                                            @BalanceQty,
                                            @LineStatus,
                                            @CreatedBy,
                                            @ModifiedBy,
                                            GETDATE()
                                        );
                                        SELECT CAST(SCOPE_IDENTITY() AS INT);", SQLCon);
                SQLCmd.Parameters.AddWithValue("@PRID", detail.PRID ?? "");
                SQLCmd.Parameters.AddWithValue("@PRNumber", detail.PRNumber);
                SQLCmd.Parameters.AddWithValue("@ProjectBOMCode", string.IsNullOrEmpty(detail.ProjectBOMCode) ? (object)DBNull.Value : detail.ProjectBOMCode);
                SQLCmd.Parameters.AddWithValue("@ProjectCode", detail.ProjectCode ?? "");
                SQLCmd.Parameters.AddWithValue("@ProductNo", string.IsNullOrEmpty(detail.ProductNo) ? (object)DBNull.Value : detail.ProductNo);
                SQLCmd.Parameters.AddWithValue("@ProductCode", string.IsNullOrEmpty(detail.ProductCode) ? (object)DBNull.Value : detail.ProductCode);
                SQLCmd.Parameters.AddWithValue("@ItemCode", detail.ItemCode);
                SQLCmd.Parameters.AddWithValue("@ItemDescription", string.IsNullOrEmpty(detail.ItemDescription) ? (object)DBNull.Value : detail.ItemDescription);
                SQLCmd.Parameters.AddWithValue("@Specification", string.IsNullOrEmpty(detail.Specification) ? (object)DBNull.Value : detail.Specification);
                SQLCmd.Parameters.AddWithValue("@Make", string.IsNullOrEmpty(detail.Make) ? (object)DBNull.Value : detail.Make);
                SQLCmd.Parameters.AddWithValue("@MfgPartNo", string.IsNullOrEmpty(detail.MfgPartNo) ? (object)DBNull.Value : detail.MfgPartNo);
                SQLCmd.Parameters.AddWithValue("@Quantity", detail.Quantity);
                SQLCmd.Parameters.AddWithValue("@UOM", string.IsNullOrEmpty(detail.UOM) ? (object)DBNull.Value : detail.UOM);
                SQLCmd.Parameters.AddWithValue("@UnitCost", detail.UnitCost);
                SQLCmd.Parameters.AddWithValue("@TotalCost", detail.TotalCost);
                SQLCmd.Parameters.AddWithValue("@VendorCode", string.IsNullOrEmpty(detail.VendorCode) ? (object)DBNull.Value : detail.VendorCode);
                SQLCmd.Parameters.AddWithValue("@VendorName", string.IsNullOrEmpty(detail.VendorName) ? (object)DBNull.Value : detail.VendorName);
                SQLCmd.Parameters.AddWithValue("@DrawingNo", string.IsNullOrEmpty(detail.DrawingNo) ? (object)DBNull.Value : detail.DrawingNo);
                SQLCmd.Parameters.AddWithValue("@Location", string.IsNullOrEmpty(detail.Location) ? (object)DBNull.Value : detail.Location);
                SQLCmd.Parameters.AddWithValue("@HSNCode", string.IsNullOrEmpty(detail.HSNCode) ? (object)DBNull.Value : detail.HSNCode);
                SQLCmd.Parameters.AddWithValue("@BOMQuantity", (object?)detail.BOMQuantity ?? DBNull.Value);
                SQLCmd.Parameters.AddWithValue("@AlreadyPurchasedQty", (object?)detail.AlreadyPurchasedQty ?? DBNull.Value);
                SQLCmd.Parameters.AddWithValue("@BalanceQty", (object?)detail.BalanceQty ?? DBNull.Value);
                SQLCmd.Parameters.AddWithValue("@LineStatus", PRLineStatus.Pending);
                SQLCmd.Parameters.AddWithValue("@CreatedBy", detail.CreatedBy ?? "");
                SQLCmd.Parameters.AddWithValue("@ModifiedBy", detail.CreatedBy ?? "");
                object result = SQLCmd.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                    newId = Convert.ToInt32(result);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Insert PR Detail", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }
            finally
            {
                SQLCon.Close();
            }
            return newId;
        }

        public bool TrackBOMConversion(string projectCode, string productNo, string itemCode,
            string projectBOMCode, string prNumber, float convertedQty, string convertedBy, int prId = 0)
        {
            bool success = false;
            try
            {
                SQLCon.Open();
                SQLCmd = new SqlCommand(@"INSERT INTO BOMtoPRConversion
                                        (
                                            BOMProjectCode,
                                            ProductNo,
                                            ItemCode,
                                            PRID,
                                            BOMCode,
                                            PRNumber,
                                            ConvertedQty,
                                            ConvertedBy
                                        )
                                        VALUES
                                        (
                                            @ProjectCode,
                                            @ProductNo,
                                            @ItemCode,
                                            @PRID,
                                            @ProjectBOMCode,
                                            @PRNumber,
                                            @ConvertedQty,
                                            @ConvertedBy
                                        )", SQLCon);
                SQLCmd.Parameters.AddWithValue("@ProjectCode", projectCode ?? "");
                SQLCmd.Parameters.AddWithValue("@ProductNo", productNo ?? "");
                SQLCmd.Parameters.AddWithValue("@ItemCode", itemCode ?? "");
                SQLCmd.Parameters.AddWithValue("@PRID", prId > 0 ? prId : 0);
                SQLCmd.Parameters.AddWithValue("@ProjectBOMCode", projectBOMCode ?? "");
                SQLCmd.Parameters.AddWithValue("@PRNumber", prNumber ?? "");
                SQLCmd.Parameters.AddWithValue("@ConvertedQty", convertedQty);
                SQLCmd.Parameters.AddWithValue("@ConvertedBy", convertedBy ?? "");
                SQLCmd.ExecuteNonQuery();
                success = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Track Conversion", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SQLCon.Close();
            }
            return success;
        }

        #endregion

        #region === PR Clubbing ===

        public DataTable GetPRsForClubbing(string vendorCode = null)
        {
            DataTable dt = new DataTable();
            try
            {
                SQLCon.Open();
                SQLCmd = new SqlCommand(@"
                    SELECT *
                    FROM PurchaseRequest
                    WHERE Status = @Status
                      AND ISNULL(IsClubbed, 0) = 0
                      AND (@VendorCode IS NULL OR VendorCode = @VendorCode)
                    ORDER BY RequestDate DESC", SQLCon);
                SQLCmd.Parameters.AddWithValue("@Status", PRStatus.Approved);
                SQLCmd.Parameters.AddWithValue("@VendorCode",
                    string.IsNullOrWhiteSpace(vendorCode) ? (object)DBNull.Value : vendorCode);
                SQLDadpr = new SqlDataAdapter(SQLCmd);
                SQLDadpr.Fill(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Get PRs", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SQLCon.Close();
            }
            return dt;
        }

        public DataTable GetPRsDetails(string PrNumber)
        {
            DataTable dt = new DataTable();
            try
            {
                SQLCon.Open();
                if (!string.IsNullOrWhiteSpace(PrNumber))
                {
                    SQLCmd = new SqlCommand(
                        "SELECT * FROM PurchaseRequestDetailNew WHERE PRNumber = @PRNumber ORDER BY DetailID",
                        SQLCon);
                    SQLCmd.Parameters.AddWithValue("@PRNumber", PrNumber);
                }
                else
                {
                    SQLCmd = new SqlCommand("SELECT * FROM PurchaseRequestDetailNew ORDER BY DetailID", SQLCon);
                }
                SQLDadpr = new SqlDataAdapter(SQLCmd);
                SQLDadpr.Fill(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Get PRs", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SQLCon.Close();
            }
            return dt;
        }


        #endregion

        public bool UpdatePRClubbedFromPRIDs(string prNumber, string ClubbedFromPRIDs)
        {
            bool success = false;
            try
            {
                SQLCon.Open();
                string query = @"
                    UPDATE PurchaseRequest
                    SET ClubbedFromPRIDs = @ClubbedFromPRIDs,
                        ModifiedDate = GETDATE(),
                        IsClubbed = 1,
                        Status = @Status
                    WHERE PRNumber = @PRNumber";
                SQLCmd = new SqlCommand(query, SQLCon);
                SQLCmd.Parameters.AddWithValue("@PRNumber", prNumber);
                SQLCmd.Parameters.AddWithValue("@ClubbedFromPRIDs", ClubbedFromPRIDs);
                SQLCmd.Parameters.AddWithValue("@Status", PRStatus.Clubbed);
                success = SQLCmd.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating PR PRNumber: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (SQLCon.State == ConnectionState.Open)
                    SQLCon.Close();
            }
            return success;
        }

        /// <summary>
        /// Creates a real clubbed PR: new header + copied lines, marks source PRs as Clubbed.
        /// Sources must share the same vendor and stay under the 12L limit.
        /// </summary>
        public string CreateClubbedPR(List<string> sourcePRNumbers, string createdBy, out string errorMessage)
        {
            errorMessage = "";
            if (sourcePRNumbers == null || sourcePRNumbers.Count < 2)
            {
                errorMessage = "Select at least two approved PRs to club.";
                return null;
            }

            try
            {
                SQLCon.Open();
                using (SqlTransaction tx = SQLCon.BeginTransaction())
                {
                    // Load source headers
                    DataTable headers = new DataTable();
                    using (SqlCommand cmd = new SqlCommand(@"
                        SELECT PRID, PRNumber, ProjectCode, VendorCode, VendorName, TotalAmount, Status, IsClubbed
                        FROM PurchaseRequest
                        WHERE PRNumber IN (" + string.Join(",", sourcePRNumbers.Select((_, i) => "@p" + i)) + @")", SQLCon, tx))
                    {
                        for (int i = 0; i < sourcePRNumbers.Count; i++)
                            cmd.Parameters.AddWithValue("@p" + i, sourcePRNumbers[i]);
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                            da.Fill(headers);
                    }

                    if (headers.Rows.Count != sourcePRNumbers.Count)
                    {
                        errorMessage = "One or more selected PRs were not found.";
                        tx.Rollback();
                        return null;
                    }

                    string vendorCode = headers.Rows[0]["VendorCode"]?.ToString() ?? "";
                    string vendorName = headers.Rows[0]["VendorName"]?.ToString();
                    if (string.IsNullOrWhiteSpace(vendorName))
                        vendorName = vendorCode;
                    decimal totalAmount = 0;
                    List<string> sourceIds = new List<string>();

                    foreach (DataRow row in headers.Rows)
                    {
                        if (!string.Equals(row["VendorCode"]?.ToString(), vendorCode, StringComparison.OrdinalIgnoreCase))
                        {
                            errorMessage = "All selected PRs must belong to the same vendor.";
                            tx.Rollback();
                            return null;
                        }
                        if (!string.Equals(row["Status"]?.ToString(), PRStatus.Approved, StringComparison.OrdinalIgnoreCase))
                        {
                            errorMessage = "Only Approved PRs can be clubbed.";
                            tx.Rollback();
                            return null;
                        }
                        if (row["IsClubbed"] != DBNull.Value && Convert.ToBoolean(row["IsClubbed"]))
                        {
                            errorMessage = "One or more selected PRs are already clubbed.";
                            tx.Rollback();
                            return null;
                        }
                        totalAmount += Convert.ToDecimal(row["TotalAmount"]);
                        sourceIds.Add(row["PRID"].ToString());
                    }

                    if (totalAmount > AppConfig.PR_AMOUNT_LIMIT)
                    {
                        errorMessage = "Clubbed amount exceeds the 12 Lakh limit.";
                        tx.Rollback();
                        return null;
                    }

                    string newPRNumber = GetNextPRNumberInTransaction(tx);
                    string projectCode = headers.Rows[0]["ProjectCode"]?.ToString() ?? "";
                    string clubbedFrom = string.Join(",", sourceIds);

                    int newPrId;
                    using (SqlCommand cmd = new SqlCommand(@"
                        INSERT INTO PurchaseRequest
                        (
                            PRNumber, ProjectCode, VendorCode, VendorName, TotalAmount,
                            Status, Remarks, CreatedBy, RequestedBy, RequestDate,
                            ApprovedBy, ApprovedDate, ModifiedBy, ModifiedDate,
                            IsClubbed, ClubbedFromPRIDs
                        )
                        VALUES
                        (
                            @PRNumber, @ProjectCode, @VendorCode, @VendorName, @TotalAmount,
                            @Status, @Remarks, @CreatedBy, @CreatedBy, GETDATE(),
                            @ApprovedBy, GETDATE(), @CreatedBy, GETDATE(),
                            0, @ClubbedFromPRIDs
                        );
                        SELECT CAST(SCOPE_IDENTITY() AS INT);", SQLCon, tx))
                    {
                        cmd.Parameters.AddWithValue("@PRNumber", newPRNumber);
                        cmd.Parameters.AddWithValue("@ProjectCode", projectCode);
                        cmd.Parameters.AddWithValue("@VendorCode", vendorCode);
                        cmd.Parameters.AddWithValue("@VendorName", (object?)vendorName ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@TotalAmount", totalAmount);
                        cmd.Parameters.AddWithValue("@Status", PRStatus.Approved);
                        cmd.Parameters.AddWithValue("@Remarks", "Clubbed from: " + string.Join(", ", sourcePRNumbers));
                        cmd.Parameters.AddWithValue("@CreatedBy", createdBy ?? "");
                        cmd.Parameters.AddWithValue("@ApprovedBy", createdBy ?? "");
                        cmd.Parameters.AddWithValue("@ClubbedFromPRIDs", clubbedFrom);
                        object idObj = cmd.ExecuteScalar();
                        newPrId = idObj != null && idObj != DBNull.Value ? Convert.ToInt32(idObj) : 0;
                    }

                    if (newPrId <= 0)
                    {
                        errorMessage = "Failed to create clubbed PR header.";
                        tx.Rollback();
                        return null;
                    }

                    // Copy all detail lines from source PRs
                    using (SqlCommand cmd = new SqlCommand(@"
                        INSERT INTO PurchaseRequestDetailNew
                        (
                            PRID, PRNumber, ProjectBOMCode, ProjectCode, ProductCode, ProductNo,
                            ItemCode, ItemDescription, Specification, Make, MfgPartNo,
                            Quantity, UOM, UnitCost, TotalCost, VendorCode, VendorName,
                            DrawingNo, Location, HSNCode, BOMQuantity, AlreadyPurchasedQty,
                            BalanceQty, LineStatus, Remarks, CreatedBy, ModifiedBy, CreatedDate
                        )
                        SELECT
                            @NewPRID, @NewPRNumber, ProjectBOMCode, ProjectCode, ProductCode, ProductNo,
                            ItemCode, ItemDescription, Specification, Make, MfgPartNo,
                            Quantity, UOM, UnitCost, TotalCost, VendorCode, VendorName,
                            DrawingNo, Location, HSNCode, BOMQuantity, AlreadyPurchasedQty,
                            BalanceQty, @LineStatus,
                            'Clubbed from ' + PRNumber, @CreatedBy, @CreatedBy, GETDATE()
                        FROM PurchaseRequestDetailNew
                        WHERE PRNumber IN (" + string.Join(",", sourcePRNumbers.Select((_, i) => "@sp" + i)) + @")", SQLCon, tx))
                    {
                        cmd.Parameters.AddWithValue("@NewPRID", newPrId.ToString());
                        cmd.Parameters.AddWithValue("@NewPRNumber", newPRNumber);
                        cmd.Parameters.AddWithValue("@LineStatus", PRLineStatus.Pending);
                        cmd.Parameters.AddWithValue("@CreatedBy", createdBy ?? "");
                        for (int i = 0; i < sourcePRNumbers.Count; i++)
                            cmd.Parameters.AddWithValue("@sp" + i, sourcePRNumbers[i]);
                        cmd.ExecuteNonQuery();
                    }

                    // Mark source PRs as clubbed / retired
                    using (SqlCommand cmd = new SqlCommand(@"
                        UPDATE PurchaseRequest
                        SET IsClubbed = 1,
                            Status = @ClubbedStatus,
                            ClubbedFromPRIDs = @NewPRNumber,
                            ModifiedDate = GETDATE(),
                            ModifiedBy = @CreatedBy
                        WHERE PRNumber IN (" + string.Join(",", sourcePRNumbers.Select((_, i) => "@mp" + i)) + @")", SQLCon, tx))
                    {
                        cmd.Parameters.AddWithValue("@ClubbedStatus", PRStatus.Clubbed);
                        cmd.Parameters.AddWithValue("@NewPRNumber", newPRNumber);
                        cmd.Parameters.AddWithValue("@CreatedBy", createdBy ?? "");
                        for (int i = 0; i < sourcePRNumbers.Count; i++)
                            cmd.Parameters.AddWithValue("@mp" + i, sourcePRNumbers[i]);
                        cmd.ExecuteNonQuery();
                    }

                    tx.Commit();
                    return newPRNumber;
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return null;
            }
            finally
            {
                if (SQLCon.State == ConnectionState.Open)
                    SQLCon.Close();
            }
        }

        private string GetNextPRNumberInTransaction(SqlTransaction tx)
        {
            string prefix = $"PR-{DateTime.Now.Year}-{DateTime.Now.Month:D2}-";
            int nextSeq = 1;
            using (SqlCommand cmd = new SqlCommand(@"
                SELECT MAX(PRNumber)
                FROM PurchaseRequest
                WHERE PRNumber LIKE @Prefix + '%'", SQLCon, tx))
            {
                cmd.Parameters.AddWithValue("@Prefix", prefix);
                object result = cmd.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    string last = result.ToString() ?? "";
                    string[] parts = last.Split('-');
                    if (parts.Length >= 4 && int.TryParse(parts[3], out int current))
                        nextSeq = current + 1;
                }
            }
            return prefix + nextSeq.ToString("D7");
        }
    }

    #region === Data Models ===

    public class PurchaseRequestDetail
    {
        public int ID { get; set; }
        public string PRID { get; set; }
        public string PRNumber { get; set; }
        public string ProjectBOMCode { get; set; }
        public string ProjectCode { get; set; }
        public string ProductNo { get; set; }
        public string ProductCode { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public string Specification { get; set; }
        public string Make { get; set; }
        public string MfgPartNo { get; set; }
        public float Quantity { get; set; }
        public string UOM { get; set; }
        public decimal UnitCost { get; set; }
        public decimal TotalCost { get; set; }
        public string VendorCode { get; set; }
        public string VendorName { get; set; }
        public string DrawingNo { get; set; }
        public string Location { get; set; }
        public string HSNCode { get; set; }
        public float? IGSTRate { get; set; }
        public float? SGSTRate { get; set; }
        public float? CGSTRate { get; set; }
        public decimal? GSTAmount { get; set; }
        public decimal? LandingCost { get; set; }
        public bool IsFromBOM { get; set; }
        public float? BOMQuantity { get; set; }
        public float? AlreadyPurchasedQty { get; set; }
        public float? BalanceQty { get; set; }
        public string Status { get; set; }
        public string Remarks { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class BOMItem
    {
        public int ID { get; set; }
        public string ProjectBOMCode { get; set; }
        public string ProjectCode { get; set; }
        public string ProductNo { get; set; }
        public string ItemName { get; set; }
        public float BOMQuantity { get; set; }
        public string UOM { get; set; }
        public string MfgPartNo { get; set; }
        public string Make { get; set; }
        public string Specification { get; set; }
        public string Location { get; set; }
        public string Vendor { get; set; }
        public string DrawingNo { get; set; }
        public string RefItemCode { get; set; }
        public decimal FixedCost { get; set; }
        public float AlreadyPurchasedQty { get; set; }
        public float BalanceQty { get; set; }
        public bool AlreadyInPR { get; set; }
    }

    #endregion
}
