//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Text;
//using WinFormsApp1;

//namespace WinFormsApp1
//{
//    internal class PurchaseRequestApprovalDAL
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
    public class PurchaseRequestApprovalDAL
    {
        private DataAccessLayer _dal;

        public PurchaseRequestApprovalDAL()
        {
            _dal = new DataAccessLayer();
            _dal.fnGetConnectionString();
        }

        public DataTable GetPurchaseRequests(string statusFilter, string projectFilter, string vendorFilter, string searchPR)
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
                        V.VendorName,
                        (SELECT COUNT(*) FROM PurchaseRequestDetailNew WHERE PRID = PR.PRID) AS ItemCount,
                        PR.TotalAmount,
                        PR.Status,
                        PR.RequestedBy,
                        PR.RequestDate,
                        PR.ApprovedBy,
                        PR.ApprovedDate,
                        PR.RejectionReason,
                        PR.HoldReason,
                        PR.Remarks
                    FROM PurchaseRequest PR
                    LEFT JOIN Vendors V ON PR.VendorCode = V.VendorCode
                    WHERE 1=1";

                if (statusFilter != "All")
                    query += " AND PR.Status = @Status";
                if (projectFilter != "All")
                    query += " AND PR.ProjectCode = @ProjectCode";
                if (vendorFilter != "All")
                    query += " AND V.VendorName = @VendorName";
                if (!string.IsNullOrEmpty(searchPR))
                    query += " AND PR.PRNumber LIKE @SearchPR";
               // query += " AND PR.Status != 'Approved'";
                query += " ORDER BY PR.RequestDate DESC";

                _dal.SQLCmd = new SqlCommand(query, _dal.SQLCon);

                if (statusFilter != "All")
                    _dal.SQLCmd.Parameters.AddWithValue("@Status", statusFilter);
                if (projectFilter != "All")
                    _dal.SQLCmd.Parameters.AddWithValue("@ProjectCode", projectFilter);
                if (vendorFilter != "All")
                    _dal.SQLCmd.Parameters.AddWithValue("@VendorName", vendorFilter);
                if (!string.IsNullOrEmpty(searchPR))
                    _dal.SQLCmd.Parameters.AddWithValue("@SearchPR", "%" + searchPR + "%");

                _dal.SQLDadpr = new SqlDataAdapter(_dal.SQLCmd);
                _dal.SQLDadpr.Fill(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading PRs: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (_dal.SQLCon.State == ConnectionState.Open)
                    _dal.SQLCon.Close();
            }
            return dt;
        }

        public PurchaseRequestModel GetPRByID(string prID)
        {
            PurchaseRequestModel pr = null;
            try
            {
                _dal.SQLCon.Open();
                string headerQuery = @"
                    SELECT PR.*, V.VendorName 
                    FROM PurchaseRequest PR
                    LEFT JOIN Vendors V ON PR.VendorCode = V.VendorCode
                    WHERE PR.PRNumber = @PRNumber";

                _dal.SQLCmd = new SqlCommand(headerQuery, _dal.SQLCon);
                _dal.SQLCmd.Parameters.AddWithValue("@PRNumber", prID);

                SqlDataReader reader = _dal.SQLCmd.ExecuteReader();
                if (reader.Read())
                {
                    pr = new PurchaseRequestModel
                    {
                        PRID = Convert.ToInt32(reader["PRID"]),
                        PRNumber = reader["PRNumber"].ToString(),
                        ProjectCode = reader["ProjectCode"].ToString(),
                        VendorCode = reader["VendorCode"].ToString(),
                        VendorName = reader["VendorName"] != DBNull.Value && !string.IsNullOrWhiteSpace(reader["VendorName"].ToString())
                            ? reader["VendorName"].ToString()
                            : reader["VendorCode"].ToString(),
                        TotalAmount = Convert.ToDecimal(reader["TotalAmount"]),
                        Status = reader["Status"].ToString(),
                        RequestedBy = reader["RequestedBy"].ToString(),
                        RequestDate = Convert.ToDateTime(reader["RequestDate"]),
                        ApprovedBy = reader["ApprovedBy"] != DBNull.Value ? reader["ApprovedBy"].ToString() : "",
                        ApprovedDate = reader["ApprovedDate"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(reader["ApprovedDate"]) : null,
                        RejectionReason = reader["RejectionReason"] != DBNull.Value ? reader["RejectionReason"].ToString() : "",
                        HoldReason = reader["HoldReason"] != DBNull.Value ? reader["HoldReason"].ToString() : "",
                        Remarks = reader["Remarks"] != DBNull.Value ? reader["Remarks"].ToString() : ""
                    };
                }
                reader.Close();

                if (pr != null)
                {
                    string detailQuery = @"
                        SELECT PRD.*, I.ItemDescription, I.Units 
                        FROM PurchaseRequestDetailNew PRD
                        LEFT JOIN ItemMaster I ON PRD.ItemCode = I.ItemCode
                        WHERE PRD.PRNumber = @PRNumber";

                    _dal.SQLCmd = new SqlCommand(detailQuery, _dal.SQLCon);
                    _dal.SQLCmd.Parameters.AddWithValue("@PRNumber", prID);

                    SqlDataReader detailReader = _dal.SQLCmd.ExecuteReader();
                    while (detailReader.Read())
                    {
                        pr.LineItems.Add(new PurchaseRequestDetailModel
                        {
                            DetailID = Convert.ToInt32(detailReader["DetailID"]),
                            PRID = prID,
                            ItemCode = detailReader["ItemCode"].ToString(),
                            ItemDescription = detailReader["ItemDescription"] != DBNull.Value ? detailReader["ItemDescription"].ToString() : "",
                            Quantity = Convert.ToDecimal(detailReader["Quantity"]),
                            UnitCost = Convert.ToDecimal(detailReader["UnitCost"]),
                            UOM = detailReader["Units"] != DBNull.Value ? detailReader["Units"].ToString() : "",
                            BOMProjectCode = detailReader["ProjectBOMCode"].ToString()
                        });
                    }
                    detailReader.Close();
                    pr.ItemCount = pr.LineItems.Count;
                }
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
            return pr;
        }

        public bool ApprovePR(string prNumber, string approvedBy, string remarks)
        {
            bool success = false;
            try
            {
                _dal.SQLCon.Open();
                string query = @"
                    UPDATE PurchaseRequest 
                    SET Status = 'Approved', 
                        ApprovedBy = @ApprovedBy, 
                        ApprovedDate = GETDATE(), 
                        Remarks = @Remarks 
                    WHERE PRNumber = @PRNumber AND Status = 'Pending'";

                _dal.SQLCmd = new SqlCommand(query, _dal.SQLCon);
                _dal.SQLCmd.Parameters.AddWithValue("@PRNumber", prNumber);
                _dal.SQLCmd.Parameters.AddWithValue("@ApprovedBy", approvedBy);
                _dal.SQLCmd.Parameters.AddWithValue("@Remarks", remarks);

                int rows = _dal.SQLCmd.ExecuteNonQuery();
                success = rows > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error approving PR: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (_dal.SQLCon.State == ConnectionState.Open)
                    _dal.SQLCon.Close();
            }
            return success;
        }

        public bool RejectPR(string prID, string rejectedBy, string reason)
        {
            bool success = false;
            try
            {
                _dal.SQLCon.Open();
                string query = @"
                    UPDATE PurchaseRequest 
                    SET Status = 'Rejected', 
                        ApprovedBy = @RejectedBy, 
                        ApprovedDate = GETDATE(), 
                        RejectionReason = @Reason 
                    WHERE PRNumber = @PRNumber AND Status = 'Pending'";

                _dal.SQLCmd = new SqlCommand(query, _dal.SQLCon);
                _dal.SQLCmd.Parameters.AddWithValue("@PRNumber", prID);
                _dal.SQLCmd.Parameters.AddWithValue("@RejectedBy", rejectedBy);
                _dal.SQLCmd.Parameters.AddWithValue("@Reason", reason);

                int rows = _dal.SQLCmd.ExecuteNonQuery();
                success = rows > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error rejecting PR: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (_dal.SQLCon.State == ConnectionState.Open)
                    _dal.SQLCon.Close();
            }
            return success;
        }

        public bool HoldPR(string prID, string heldBy, string reason)
        {
            bool success = false;
            try
            {
                _dal.SQLCon.Open();
                string query = @"
                    UPDATE PurchaseRequest 
                    SET Status = @OnHold, 
                        ApprovedBy = @HeldBy, 
                        ApprovedDate = GETDATE(), 
                        HoldReason = @Reason 
                    WHERE PRNumber = @PRNumber AND Status = @Pending";

                _dal.SQLCmd = new SqlCommand(query, _dal.SQLCon);
                _dal.SQLCmd.Parameters.AddWithValue("@PRNumber", prID);
                _dal.SQLCmd.Parameters.AddWithValue("@HeldBy", heldBy);
                _dal.SQLCmd.Parameters.AddWithValue("@Reason", reason);
                _dal.SQLCmd.Parameters.AddWithValue("@OnHold", PRStatus.OnHold);
                _dal.SQLCmd.Parameters.AddWithValue("@Pending", PRStatus.Pending);

                int rows = _dal.SQLCmd.ExecuteNonQuery();
                success = rows > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error holding PR: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (_dal.SQLCon.State == ConnectionState.Open)
                    _dal.SQLCon.Close();
            }
            return success;
        }

        public bool ReleaseHoldPR(string prNumber, string releasedBy, string remarks)
        {
            bool success = false;
            try
            {
                _dal.SQLCon.Open();
                string query = @"
                    UPDATE PurchaseRequest
                    SET Status = @Pending,
                        HoldReason = NULL,
                        Remarks = @Remarks,
                        ModifiedDate = GETDATE(),
                        ModifiedBy = @ReleasedBy
                    WHERE PRNumber = @PRNumber AND Status = @OnHold";

                _dal.SQLCmd = new SqlCommand(query, _dal.SQLCon);
                _dal.SQLCmd.Parameters.AddWithValue("@PRNumber", prNumber);
                _dal.SQLCmd.Parameters.AddWithValue("@ReleasedBy", releasedBy);
                _dal.SQLCmd.Parameters.AddWithValue("@Remarks",
                    string.IsNullOrWhiteSpace(remarks) ? (object)DBNull.Value : remarks);
                _dal.SQLCmd.Parameters.AddWithValue("@Pending", PRStatus.Pending);
                _dal.SQLCmd.Parameters.AddWithValue("@OnHold", PRStatus.OnHold);

                success = _dal.SQLCmd.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error releasing hold: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (_dal.SQLCon.State == ConnectionState.Open)
                    _dal.SQLCon.Close();
            }
            return success;
        }

        public int BulkApprove(List<int> prIDs, string approvedBy, string remarks)
        {
            int approvedCount = 0;
            try
            {
                _dal.SQLCon.Open();
                foreach (int prID in prIDs)
                {
                    string query = @"
                        UPDATE PurchaseRequest 
                        SET Status = 'Approved', 
                            ApprovedBy = @ApprovedBy, 
                            ApprovedDate = GETDATE(), 
                            Remarks = @Remarks 
                        WHERE PRID = @PRID AND Status = 'Pending'";

                    _dal.SQLCmd = new SqlCommand(query, _dal.SQLCon);
                    _dal.SQLCmd.Parameters.AddWithValue("@PRID", prID);
                    _dal.SQLCmd.Parameters.AddWithValue("@ApprovedBy", approvedBy);
                    _dal.SQLCmd.Parameters.AddWithValue("@Remarks", remarks);

                    approvedCount += _dal.SQLCmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in bulk approve: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (_dal.SQLCon.State == ConnectionState.Open)
                    _dal.SQLCon.Close();
            }
            return approvedCount;
        }

        public int BulkReject(List<int> prIDs, string rejectedBy, string reason)
        {
            int rejectedCount = 0;
            try
            {
                _dal.SQLCon.Open();
                foreach (int prID in prIDs)
                {
                    string query = @"
                        UPDATE PurchaseRequest 
                        SET Status = 'Rejected', 
                            ApprovedBy = @RejectedBy, 
                            ApprovedDate = GETDATE(), 
                            RejectionReason = @Reason 
                        WHERE PRID = @PRID AND Status = 'Pending'";

                    _dal.SQLCmd = new SqlCommand(query, _dal.SQLCon);
                    _dal.SQLCmd.Parameters.AddWithValue("@PRID", prID);
                    _dal.SQLCmd.Parameters.AddWithValue("@RejectedBy", rejectedBy);
                    _dal.SQLCmd.Parameters.AddWithValue("@Reason", reason);

                    rejectedCount += _dal.SQLCmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error in bulk reject: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (_dal.SQLCon.State == ConnectionState.Open)
                    _dal.SQLCon.Close();
            }
            return rejectedCount;
        }

        public DataTable GetStatistics()
        {
            DataTable dt = new DataTable();
            try
            {
                _dal.SQLCon.Open();
                string query = @"
                    SELECT 
                        SUM(CASE WHEN Status = 'Pending' THEN 1 ELSE 0 END) AS PendingCount,
                        SUM(CASE WHEN Status = 'Approved' THEN 1 ELSE 0 END) AS ApprovedCount,
                        SUM(CASE WHEN Status = 'Rejected' THEN 1 ELSE 0 END) AS RejectedCount,
                        SUM(CASE WHEN Status = 'On Hold' THEN 1 ELSE 0 END) AS OnHoldCount,
                        SUM(CASE WHEN Status = 'Pending' THEN TotalAmount ELSE 0 END) AS PendingAmount,
                        SUM(TotalAmount) AS TotalAmount
                    FROM PurchaseRequest";

                _dal.SQLCmd = new SqlCommand(query, _dal.SQLCon);
                _dal.SQLDadpr = new SqlDataAdapter(_dal.SQLCmd);
                _dal.SQLDadpr.Fill(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading statistics: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (_dal.SQLCon.State == ConnectionState.Open)
                    _dal.SQLCon.Close();
            }
            return dt;
        }

        public DataTable GetProjects()
        {
            DataTable dt = new DataTable();
            try
            {
                _dal.SQLCon.Open();
                _dal.SQLCmd = new SqlCommand("SELECT DISTINCT ProjectCode FROM PurchaseRequest ORDER BY ProjectCode", _dal.SQLCon);
                _dal.SQLDadpr = new SqlDataAdapter(_dal.SQLCmd);
                _dal.SQLDadpr.Fill(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading projects: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (_dal.SQLCon.State == ConnectionState.Open)
                    _dal.SQLCon.Close();
            }
            return dt;
        }

        public DataTable GetVendors()
        {
            DataTable dt = new DataTable();
            try
            {
                _dal.SQLCon.Open();
                _dal.SQLCmd = new SqlCommand("SELECT DISTINCT VendorCode, VendorName FROM Vendors WHERE Status = 'Active' ORDER BY VendorName", _dal.SQLCon);
                _dal.SQLDadpr = new SqlDataAdapter(_dal.SQLCmd);
                _dal.SQLDadpr.Fill(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading vendors: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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