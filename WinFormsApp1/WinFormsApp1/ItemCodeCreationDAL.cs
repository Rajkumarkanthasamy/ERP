using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using WinFormsApp1;

namespace WinFormsApp1
{
    /// <summary>
    /// Data Access Layer for ItemCodeCreation - matches existing project pattern
    /// </summary>
    public class ItemCodeCreationDAL
    {
        // Connection string - uses same pattern as existing DataAccessLayer
        public string cs = "Data Source=GTKA064W111\\SQLEXPRESS01;Initial Catalog=ERP_Database; User ID=sa;Password=Bangalore@560058";
        public SqlConnection SQLCon;
        public SqlCommand SQLCmd;
        public SqlDataAdapter SQLDadpr;
        public DataSet SQLDataset;

        public ItemCodeCreationDAL()
        {
            fnGetConnectionString();
        }

        public void fnGetConnectionString()
        {
            try
            {
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
                MessageBox.Show("Server was not found. " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                SQLCon.Close();
                return false;
            }
        }

        #region === INSERT OPERATIONS ===

        /// <summary>
        /// Insert a new item code creation request into the database
        /// </summary>
        public int InsertItemCodeRequest(ItemCodeRequest request)
        {
            int newId = 0;

            try
            {
                SQLCon.Open();
                SQLCmd = new SqlCommand("sp_ItemCodeCreation_Insert", SQLCon);
                SQLCmd.CommandType = CommandType.StoredProcedure;

                SQLCmd.Parameters.AddWithValue("@RequestID", request.RequestID);
                SQLCmd.Parameters.AddWithValue("@Requestor", request.Requestor);
                SQLCmd.Parameters.AddWithValue("@Department", request.Department);
                SQLCmd.Parameters.AddWithValue("@ItemCategory", request.ItemCategory);
                SQLCmd.Parameters.AddWithValue("@ItemDescription", request.ItemDescription);
                SQLCmd.Parameters.AddWithValue("@TechnicalSpecification",
                    string.IsNullOrEmpty(request.TechnicalSpec) ? (object)DBNull.Value : request.TechnicalSpec);
                SQLCmd.Parameters.AddWithValue("@UnitOfMeasure", request.UnitOfMeasure);
                SQLCmd.Parameters.AddWithValue("@DrawingReference",
                    string.IsNullOrEmpty(request.DrawingReference) ? (object)DBNull.Value : request.DrawingReference);
                SQLCmd.Parameters.AddWithValue("@CriticalityLevel",
                    string.IsNullOrEmpty(request.CriticalityLevel) ? (object)DBNull.Value : request.CriticalityLevel);
                SQLCmd.Parameters.AddWithValue("@HSNCode",
                    string.IsNullOrEmpty(request.HSNCode) ? (object)DBNull.Value : request.HSNCode);
                SQLCmd.Parameters.AddWithValue("@ItemCode", request.ItemCode);
                SQLCmd.Parameters.AddWithValue("@Prefix", request.ItemCode.Split('-')[0]);
                SQLCmd.Parameters.AddWithValue("@CategoryCode", "XX");
                SQLCmd.Parameters.AddWithValue("@SequentialNumber", request.ItemCode.Split('-')[2]);
                SQLCmd.Parameters.AddWithValue("@AuthorizedCreator", request.AuthorizedCreator);
                SQLCmd.Parameters.AddWithValue("@ApprovalAuthority", request.ApprovalAuthority);
                SQLCmd.Parameters.AddWithValue("@IsEmergency", request.IsEmergency);
                SQLCmd.Parameters.AddWithValue("@EmergencyApprovedBy",
                    string.IsNullOrEmpty(request.EmergencyApprovedBy) ? (object)DBNull.Value : request.EmergencyApprovedBy);
                SQLCmd.Parameters.AddWithValue("@CreatedBy", request.Requestor);

                object result = SQLCmd.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    newId = Convert.ToInt32(result);
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Database Error: " + ex.Message, "Insert Failed",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SQLCon.Close();
            }

            return newId;
        }

        #endregion

        #region === UPDATE OPERATIONS ===

        /// <summary>
        /// Update approval status of a request
        /// </summary>
        public bool UpdateApprovalStatus(string requestId, string status, string remarks,
            string approvedBy, string storeLocation = null, string binNumber = null)
        {
            bool success = false;

            try
            {
                SQLCon.Open();
                SQLCmd = new SqlCommand("sp_ItemCodeCreation_UpdateApproval", SQLCon);
                SQLCmd.CommandType = CommandType.StoredProcedure;

                SQLCmd.Parameters.AddWithValue("@RequestID", requestId);
                SQLCmd.Parameters.AddWithValue("@ApprovalStatus", status);
                SQLCmd.Parameters.AddWithValue("@ApprovalRemarks",
                    string.IsNullOrEmpty(remarks) ? (object)DBNull.Value : remarks);
                SQLCmd.Parameters.AddWithValue("@ApprovedBy", approvedBy);
                SQLCmd.Parameters.AddWithValue("@StoreLocation",
                    string.IsNullOrEmpty(storeLocation) ? (object)DBNull.Value : storeLocation);
                SQLCmd.Parameters.AddWithValue("@BinNumber",
                    string.IsNullOrEmpty(binNumber) ? (object)DBNull.Value : binNumber);

                int rowsAffected = SQLCmd.ExecuteNonQuery();
                success = rowsAffected > 0;
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Database Error: " + ex.Message, "Update Failed",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SQLCon.Close();
            }

            return success;
        }

        #endregion

        #region === SELECT OPERATIONS ===

        /// <summary>
        /// Get all pending requests for approval dashboard
        /// </summary>
        public List<ItemCodeRequest> GetPendingRequests(string approvalAuthority = null,
            string itemCategory = null, string department = null, string status = null)
        {
            List<ItemCodeRequest> requests = new List<ItemCodeRequest>();
            MessageBox.Show(approvalAuthority, "Status Filter..!");
            try
            {
                SQLCon.Open();
                SQLCmd = new SqlCommand("sp_ItemCodeCreation_GetPending", SQLCon);
                SQLCmd.CommandType = CommandType.StoredProcedure;

                SQLCmd.Parameters.AddWithValue("@ApprovalAuthority",
                    string.IsNullOrEmpty(approvalAuthority) ? (object)DBNull.Value : approvalAuthority);
                SQLCmd.Parameters.AddWithValue("@ItemCategory",
                    string.IsNullOrEmpty(itemCategory) || itemCategory == "All" ? (object)DBNull.Value : itemCategory);
                SQLCmd.Parameters.AddWithValue("@Department",
                    string.IsNullOrEmpty(department) || department == "All" ? (object)DBNull.Value : department);
                SQLCmd.Parameters.AddWithValue("@ApprovalStatus",
                    string.IsNullOrEmpty(status) || status == "All" ? (object)DBNull.Value : status);

                SQLDadpr = new SqlDataAdapter(SQLCmd);
                DataTable dt = new DataTable();
                SQLDadpr.Fill(dt);

                foreach (DataRow row in dt.Rows)
                {
                    requests.Add(MapDataRowToRequest(row));
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Database Error: " + ex.Message, "Query Failed",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SQLCon.Close();
            }

            return requests;
        }

        /// <summary>
        /// Get a single request by RequestID
        /// </summary>
        public ItemCodeRequest GetRequestById(string requestId)
        {
            ItemCodeRequest request = null;

            try
            {
                SQLCon.Open();
                SQLCmd = new SqlCommand("sp_ItemCodeCreation_GetByID", SQLCon);
                SQLCmd.CommandType = CommandType.StoredProcedure;
                SQLCmd.Parameters.AddWithValue("@RequestID", requestId);

                SQLDadpr = new SqlDataAdapter(SQLCmd);
                DataTable dt = new DataTable();
                SQLDadpr.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    request = MapDataRowToRequest(dt.Rows[0]);
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Database Error: " + ex.Message, "Query Failed",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SQLCon.Close();
            }

            return request;
        }

        /// <summary>
        /// Get dashboard statistics
        /// </summary>
        public DataTable GetStatistics()
        {
            DataTable dt = new DataTable();

            try
            {
                SQLCon.Open();
                SQLCmd = new SqlCommand("sp_ItemCodeCreation_GetStatistics", SQLCon);
                SQLCmd.CommandType = CommandType.StoredProcedure;

                SQLDadpr = new SqlDataAdapter(SQLCmd);
                SQLDadpr.Fill(dt);
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Database Error: " + ex.Message, "Query Failed",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SQLCon.Close();
            }

            return dt;
        }

        /// <summary>
        /// Get next sequential number for a given prefix
        /// </summary>
        public int GetNextSequence(string prefix)
        {
            int nextSeq = 1000;

            try
            {
                SQLCon.Open();
                SQLCmd = new SqlCommand("sp_ItemCodeCreation_GetNextSequence", SQLCon);
                SQLCmd.CommandType = CommandType.StoredProcedure;
                SQLCmd.Parameters.AddWithValue("@Prefix", prefix);

                object result = SQLCmd.ExecuteScalar();
                if (result != null && result != DBNull.Value)
                {
                    nextSeq = Convert.ToInt32(result);
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Database Error: " + ex.Message, "Sequence Failed",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SQLCon.Close();
            }

            return nextSeq;
        }

        #endregion

        #region === PUSH TO ITEM MASTER ===

        /// <summary>
        /// Push approved item code to ItemMaster table
        /// </summary>
        public string PushToItemMaster(string requestId, string pushedBy)
        {
            string result = "";

            try
            {
                SQLCon.Open();
                SQLCmd = new SqlCommand("sp_ItemCodeCreation_PushToItemMaster", SQLCon);
                SQLCmd.CommandType = CommandType.StoredProcedure;
                SQLCmd.Parameters.AddWithValue("@RequestID", requestId);
                SQLCmd.Parameters.AddWithValue("@PushedBy", pushedBy);

                SQLDadpr = new SqlDataAdapter(SQLCmd);
                DataTable dt = new DataTable();
                SQLDadpr.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    string status = dt.Rows[0]["Result"].ToString();
                    string message = dt.Rows[0]["Message"].ToString();
                    result = status + ": " + message;
                }
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Database Error: " + ex.Message, "Push Failed",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SQLCon.Close();
            }

            return result;
        }

        #endregion

        #region === HELPER METHODS ===

        /// <summary>
        /// Map DataRow to ItemCodeRequest object
        /// </summary>
        private ItemCodeRequest MapDataRowToRequest(DataRow row)
        {
            return new ItemCodeRequest
            {
                ID = row["ID"] != DBNull.Value ? Convert.ToInt32(row["ID"]) : 0,
                RequestID = row["RequestID"] != DBNull.Value ? row["RequestID"].ToString() : "",
                RequestDate = row["RequestDate"] != DBNull.Value ? Convert.ToDateTime(row["RequestDate"]) : DateTime.MinValue,
                Requestor = row["Requestor"] != DBNull.Value ? row["Requestor"].ToString() : "",
                Department = row["Department"] != DBNull.Value ? row["Department"].ToString() : "",
                ItemCategory = row["ItemCategory"] != DBNull.Value ? row["ItemCategory"].ToString() : "",
                ItemDescription = row["ItemDescription"] != DBNull.Value ? row["ItemDescription"].ToString() : "",
                TechnicalSpec = row["TechnicalSpecification"] != DBNull.Value ? row["TechnicalSpecification"].ToString() : "",
                UnitOfMeasure = row["UnitOfMeasure"] != DBNull.Value ? row["UnitOfMeasure"].ToString() : "",
                DrawingReference = row["DrawingReference"] != DBNull.Value ? row["DrawingReference"].ToString() : "",
                CriticalityLevel = row["CriticalityLevel"] != DBNull.Value ? row["CriticalityLevel"].ToString() : "",
                HSNCode = row["HSNCode"] != DBNull.Value ? row["HSNCode"].ToString() : "",
                ItemCode = row["ItemCode"] != DBNull.Value ? row["ItemCode"].ToString() : "",
                AuthorizedCreator = row["AuthorizedCreator"] != DBNull.Value ? row["AuthorizedCreator"].ToString() : "",
                ApprovalAuthority = row["ApprovalAuthority"] != DBNull.Value ? row["ApprovalAuthority"].ToString() : "",
                ApprovalStatus = row["ApprovalStatus"] != DBNull.Value ? row["ApprovalStatus"].ToString() : "Pending",
                ApprovalRemarks = row["ApprovalRemarks"] != DBNull.Value ? row["ApprovalRemarks"].ToString() : "",
                ApprovedBy = row["ApprovedBy"] != DBNull.Value ? row["ApprovedBy"].ToString() : "",
                ApprovalDate = row["ApprovalDate"] != DBNull.Value ? (DateTime?)Convert.ToDateTime(row["ApprovalDate"]) : null,
                StoreLocation = row["StoreLocation"] != DBNull.Value ? row["StoreLocation"].ToString() : "",
                BinNumber = row["BinNumber"] != DBNull.Value ? row["BinNumber"].ToString() : "",
                IsEmergency = row["IsEmergency"] != DBNull.Value ? Convert.ToBoolean(row["IsEmergency"]) : false,
                EmergencyApprovedBy = row["EmergencyApprovedBy"] != DBNull.Value ? row["EmergencyApprovedBy"].ToString() : "",
                IsActive = row["IsActive"] != DBNull.Value ? Convert.ToBoolean(row["IsActive"]) : false,
                IsPushedToItemMaster = row["IsPushedToItemMaster"] != DBNull.Value ? Convert.ToBoolean(row["IsPushedToItemMaster"]) : false
            };
        }

        #endregion
    }
}