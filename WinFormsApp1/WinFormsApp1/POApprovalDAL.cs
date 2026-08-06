


using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public class POApprovalDAL
    {
        private DataAccessLayer _dal;

        public POApprovalDAL()
        {
            _dal = new DataAccessLayer();
            _dal.fnGetConnectionString();
        }

        public DataTable GetPOListForApproval()
        {
            DataTable dt = new DataTable();
            try
            {
                _dal.SQLCon.Open();
                string query = @"
                    SELECT 
                        ID, DBOMNo, ProjectCode, VendorCode, ItemCode,
                        UOM, RequariedQty, UnitPrice, Amount,
                        PreparedBy, POPreparedDate, POApproved,
                        PMName, PMApproved, MHName, MHApproved,
                        POGeneratedBy, POGeneratedDate
                    FROM PurchaseOrder
                    WHERE POApproved = 0 OR POApproved IS NULL
                    ORDER BY POGeneratedDate DESC";

                _dal.SQLCmd = new SqlCommand(query, _dal.SQLCon);
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

        public DataTable GetPOByID(int poID)
        {
            DataTable dt = new DataTable();
            try
            {
                _dal.SQLCon.Open();
                string query = @"SELECT * FROM PurchaseOrder WHERE ID = @ID";
                _dal.SQLCmd = new SqlCommand(query, _dal.SQLCon);
                _dal.SQLCmd.Parameters.AddWithValue("@ID", poID);
                _dal.SQLDadpr = new SqlDataAdapter(_dal.SQLCmd);
                _dal.SQLDadpr.Fill(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading PO: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (_dal.SQLCon.State == ConnectionState.Open)
                    _dal.SQLCon.Close();
            }
            return dt;
        }

        public bool ApprovePO(int poID, string approvedBy, string remarks)
        {
            bool success = false;
            try
            {
                _dal.SQLCon.Open();
                string query = @"
                    UPDATE PurchaseOrder 
                    SET POApproved = 1,
                        AuthoriedBy = @AuthoriedBy,
                        POAuthoriseDate = CONVERT(NVARCHAR(64), GETDATE(), 120),
                        Remarks = ISNULL(Remarks, '') + ' | PO Approved by ' + @AuthoriedBy + ': ' + @Remarks
                    WHERE ID = @ID AND (POApproved = 0 OR POApproved IS NULL)";

                _dal.SQLCmd = new SqlCommand(query, _dal.SQLCon);
                _dal.SQLCmd.Parameters.AddWithValue("@ID", poID);
                _dal.SQLCmd.Parameters.AddWithValue("@AuthoriedBy", approvedBy);
                _dal.SQLCmd.Parameters.AddWithValue("@Remarks", string.IsNullOrEmpty(remarks) ? "" : remarks);

                success = _dal.SQLCmd.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error approving PO: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (_dal.SQLCon.State == ConnectionState.Open)
                    _dal.SQLCon.Close();
            }
            return success;
        }

        public bool RejectPO(int poID, string rejectedBy, string reason)
        {
            bool success = false;
            try
            {
                _dal.SQLCon.Open();
                string query = @"
                    UPDATE PurchaseOrder 
                    SET POApproved = 0,
                        RejectReason = @RejectReason,
                        Remarks = ISNULL(Remarks, '') + ' | PO Rejected by ' + @RejectedBy + ': ' + @RejectReason
                    WHERE ID = @ID";

                _dal.SQLCmd = new SqlCommand(query, _dal.SQLCon);
                _dal.SQLCmd.Parameters.AddWithValue("@ID", poID);
                _dal.SQLCmd.Parameters.AddWithValue("@RejectedBy", rejectedBy);
                _dal.SQLCmd.Parameters.AddWithValue("@RejectReason", reason);

                success = _dal.SQLCmd.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error rejecting PO: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (_dal.SQLCon.State == ConnectionState.Open)
                    _dal.SQLCon.Close();
            }
            return success;
        }

        public bool PMApprovePO(int poID, string pmName, string remarks)
        {
            bool success = false;
            try
            {
                _dal.SQLCon.Open();
                string query = @"
                    UPDATE PurchaseOrder 
                    SET PMName = @PMName,
                        PMApproved = 1,
                        PMApprovedDate = CONVERT(NVARCHAR(20), GETDATE(), 103),
                        PMRemarks = @PMRemarks
                    WHERE ID = @ID";

                _dal.SQLCmd = new SqlCommand(query, _dal.SQLCon);
                _dal.SQLCmd.Parameters.AddWithValue("@ID", poID);
                _dal.SQLCmd.Parameters.AddWithValue("@PMName", pmName);
                _dal.SQLCmd.Parameters.AddWithValue("@PMRemarks", string.IsNullOrEmpty(remarks) ? "" : remarks);

                success = _dal.SQLCmd.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error PM approving PO: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (_dal.SQLCon.State == ConnectionState.Open)
                    _dal.SQLCon.Close();
            }
            return success;
        }

        public bool MHApprovePO(int poID, string mhName, string remarks)
        {
            bool success = false;
            try
            {
                _dal.SQLCon.Open();
                string query = @"
                    UPDATE PurchaseOrder 
                    SET MHName = @MHName,
                        MHApproved = 1,
                        MHApprovedDate = CONVERT(NVARCHAR(20), GETDATE(), 103),
                        PMMHRemarks = @PMMHRemarks
                    WHERE ID = @ID";

                _dal.SQLCmd = new SqlCommand(query, _dal.SQLCon);
                _dal.SQLCmd.Parameters.AddWithValue("@ID", poID);
                _dal.SQLCmd.Parameters.AddWithValue("@MHName", mhName);
                _dal.SQLCmd.Parameters.AddWithValue("@PMMHRemarks", string.IsNullOrEmpty(remarks) ? "" : remarks);

                success = _dal.SQLCmd.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error MH approving PO: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (_dal.SQLCon.State == ConnectionState.Open)
                    _dal.SQLCon.Close();
            }
            return success;
        }
    }
}