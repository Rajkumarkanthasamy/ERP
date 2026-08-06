using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public class ProcurementDashboardDAL
    {
        public DataTable GetInboxSummary()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Metric");
            dt.Columns.Add("Count", typeof(int));
            dt.Columns.Add("Amount", typeof(decimal));

            try
            {
                using (SqlConnection con = new SqlConnection(AppConfig.ConnectionString))
                {
                    con.Open();

                    AddMetric(dt, con,
                        "Pending PR Approvals",
                        $"SELECT COUNT(*), ISNULL(SUM(TotalAmount),0) FROM PurchaseRequest WHERE Status = '{PRStatus.Pending}'");

                    AddMetric(dt, con,
                        "Approved PRs Ready for PO",
                        $@"SELECT COUNT(*), ISNULL(SUM(TotalAmount),0) FROM PurchaseRequest PR
                           WHERE PR.Status IN ('{PRStatus.Approved}', '{PRStatus.PartiallyConverted}')
                             AND ISNULL(PR.IsClubbed,0) = 0
                             AND EXISTS (
                                 SELECT 1 FROM PurchaseRequestDetailNew D
                                 WHERE D.PRNumber = PR.PRNumber
                                   AND ISNULL(D.LineStatus,'{PRLineStatus.Pending}') = '{PRLineStatus.Pending}')");

                    AddMetric(dt, con,
                        "POs Awaiting Approval",
                        "SELECT COUNT(*), ISNULL(SUM(Amount),0) FROM PurchaseOrder WHERE POApproved = 0 OR POApproved IS NULL");

                    AddMetric(dt, con,
                        "Item Codes Pending",
                        @"SELECT COUNT(*), 0 FROM ItemCodeCreation
                          WHERE ApprovalStatus = 'Pending'");
                }
            }
            catch (Exception ex)
            {
                // ItemCodeCreation table may not exist — still return PR/PO metrics gathered so far
                if (dt.Rows.Count == 0)
                    MessageBox.Show("Dashboard load error: " + ex.Message, "Dashboard", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            return dt;
        }

        private static void AddMetric(DataTable dt, SqlConnection con, string name, string sql)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand(sql, con))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        dt.Rows.Add(
                            name,
                            reader.IsDBNull(0) ? 0 : Convert.ToInt32(reader.GetValue(0)),
                            reader.IsDBNull(1) ? 0m : Convert.ToDecimal(reader.GetValue(1)));
                    }
                }
            }
            catch
            {
                dt.Rows.Add(name, 0, 0m);
            }
        }

        public DataTable GetAgingPRs(int topN = 20)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(AppConfig.ConnectionString))
                using (SqlCommand cmd = new SqlCommand($@"
                    SELECT TOP (@TopN)
                        PRNumber, ProjectCode, VendorCode, TotalAmount, Status,
                        RequestedBy, RequestDate,
                        DATEDIFF(DAY, RequestDate, GETDATE()) AS AgeDays
                    FROM PurchaseRequest
                    WHERE Status IN (@Pending, @OnHold)
                    ORDER BY RequestDate ASC", con))
                {
                    cmd.Parameters.AddWithValue("@TopN", topN);
                    cmd.Parameters.AddWithValue("@Pending", PRStatus.Pending);
                    cmd.Parameters.AddWithValue("@OnHold", PRStatus.OnHold);
                    con.Open();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        da.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading aging PRs: " + ex.Message, "Dashboard", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            return dt;
        }

        public DataTable GetRecentActivity(int topN = 15)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(AppConfig.ConnectionString))
                using (SqlCommand cmd = new SqlCommand(@"
                    SELECT TOP (@TopN)
                        PRNumber AS RefNo,
                        'PR' AS Type,
                        Status,
                        TotalAmount AS Amount,
                        ISNULL(ModifiedDate, RequestDate) AS ActivityDate,
                        ISNULL(ModifiedBy, RequestedBy) AS ByUser
                    FROM PurchaseRequest
                    ORDER BY ISNULL(ModifiedDate, RequestDate) DESC", con))
                {
                    cmd.Parameters.AddWithValue("@TopN", topN);
                    con.Open();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        da.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading activity: " + ex.Message, "Dashboard", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            return dt;
        }
    }
}
