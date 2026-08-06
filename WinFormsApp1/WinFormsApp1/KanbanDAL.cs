using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public class KanbanDAL
    {
        public DataTable GetPipelineCards()
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(AppConfig.ConnectionString))
                using (SqlCommand cmd = new SqlCommand(@"
                    SELECT * FROM (
                        SELECT
                            'PR' AS CardType,
                            PR.PRNumber AS RefNo,
                            CAST(PR.PRID AS NVARCHAR(20)) AS RefId,
                            PR.ProjectCode,
                            ISNULL(PR.VendorName, PR.VendorCode) AS Vendor,
                            PR.TotalAmount AS Amount,
                            PR.Status AS Stage,
                            PR.RequestedBy AS Owner,
                            CAST(PR.RequestDate AS DATETIME) AS StageDate,
                            DATEDIFF(DAY, PR.RequestDate, GETDATE()) AS AgeDays
                        FROM PurchaseRequest PR
                        WHERE PR.Status NOT IN (@Clubbed)
                          AND ISNULL(PR.IsClubbed, 0) = 0

                        UNION ALL

                        SELECT
                            'PO' AS CardType,
                            ISNULL(PO.DBOMNo, CAST(MIN(PO.ID) AS NVARCHAR(40))) AS RefNo,
                            ISNULL(PO.DBOMNo, CAST(MIN(PO.ID) AS NVARCHAR(40))) AS RefId,
                            MAX(PO.ProjectCode) AS ProjectCode,
                            MAX(PO.VendorCode) AS Vendor,
                            SUM(ISNULL(PO.Amount, 0)) AS Amount,
                            CASE
                                WHEN MAX(CASE WHEN PO.POApproved = 1 THEN 1 ELSE 0 END) = 1 THEN 'PO Approved'
                                ELSE 'PO Pending Approval'
                            END AS Stage,
                            MAX(PO.POGeneratedBy) AS Owner,
                            MAX(TRY_CONVERT(DATETIME, PO.POGeneratedDate)) AS StageDate,
                            DATEDIFF(DAY, MAX(TRY_CONVERT(DATETIME, PO.POGeneratedDate)), GETDATE()) AS AgeDays
                        FROM PurchaseOrder PO
                        WHERE PO.DBOMNo IS NOT NULL AND PO.DBOMNo <> ''
                        GROUP BY PO.DBOMNo
                    ) X
                    ORDER BY StageDate DESC", con))
                {
                    cmd.Parameters.AddWithValue("@Clubbed", PRStatus.Clubbed);
                    con.Open();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        da.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kanban load error:\n" + ex.Message, "Kanban", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            return dt;
        }

        public bool MovePRStage(string prNumber, string newStatus, string user, out string error)
        {
            error = "";
            try
            {
                using (SqlConnection con = new SqlConnection(AppConfig.ConnectionString))
                {
                    con.Open();
                    string sql;
                    if (newStatus == PRStatus.Approved)
                    {
                        sql = @"UPDATE PurchaseRequest SET Status=@Status, ApprovedBy=@User, ApprovedDate=GETDATE(), ModifiedDate=GETDATE(), ModifiedBy=@User
                                WHERE PRNumber=@PR AND Status IN (@Pending, @OnHold)";
                    }
                    else if (newStatus == PRStatus.OnHold)
                    {
                        sql = @"UPDATE PurchaseRequest SET Status=@Status, HoldReason='Moved via Kanban', ModifiedDate=GETDATE(), ModifiedBy=@User
                                WHERE PRNumber=@PR AND Status=@Pending";
                    }
                    else if (newStatus == PRStatus.Pending)
                    {
                        sql = @"UPDATE PurchaseRequest SET Status=@Status, HoldReason=NULL, ModifiedDate=GETDATE(), ModifiedBy=@User
                                WHERE PRNumber=@PR AND Status=@OnHold";
                    }
                    else if (newStatus == PRStatus.Rejected)
                    {
                        sql = @"UPDATE PurchaseRequest SET Status=@Status, RejectionReason='Rejected via Kanban', ApprovedBy=@User, ApprovedDate=GETDATE()
                                WHERE PRNumber=@PR AND Status=@Pending";
                    }
                    else
                    {
                        error = "This stage change is not allowed from the board. Use the dedicated screens.";
                        return false;
                    }

                    using (SqlCommand cmd = new SqlCommand(sql, con))
                    {
                        cmd.Parameters.AddWithValue("@Status", newStatus);
                        cmd.Parameters.AddWithValue("@User", user ?? "");
                        cmd.Parameters.AddWithValue("@PR", prNumber);
                        cmd.Parameters.AddWithValue("@Pending", PRStatus.Pending);
                        cmd.Parameters.AddWithValue("@OnHold", PRStatus.OnHold);
                        int rows = cmd.ExecuteNonQuery();
                        if (rows == 0)
                        {
                            error = "No PR updated. Check current status / permissions.";
                            return false;
                        }
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }
    }
}
