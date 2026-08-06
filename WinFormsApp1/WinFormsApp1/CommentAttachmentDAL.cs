using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public class CommentAttachmentDAL
    {
        public DataTable GetComments(string entityType, string entityRef)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(AppConfig.ConnectionString))
                using (SqlCommand cmd = new SqlCommand(@"
                    SELECT CommentID, CreatedBy, CreatedDate, CommentText
                    FROM ProcurementComment
                    WHERE EntityType = @Type AND EntityRef = @Ref
                    ORDER BY CreatedDate DESC", con))
                {
                    cmd.Parameters.AddWithValue("@Type", entityType);
                    cmd.Parameters.AddWithValue("@Ref", entityRef);
                    con.Open();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        da.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Comments load error (run Phase3_Schema_Alignment.sql):\n" + ex.Message,
                    "Comments", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            return dt;
        }

        public bool AddComment(string entityType, string entityRef, string text, string createdBy)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(AppConfig.ConnectionString))
                using (SqlCommand cmd = new SqlCommand(@"
                    INSERT INTO ProcurementComment (EntityType, EntityRef, CommentText, CreatedBy)
                    VALUES (@Type, @Ref, @Text, @By)", con))
                {
                    cmd.Parameters.AddWithValue("@Type", entityType);
                    cmd.Parameters.AddWithValue("@Ref", entityRef);
                    cmd.Parameters.AddWithValue("@Text", text);
                    cmd.Parameters.AddWithValue("@By", createdBy ?? "");
                    con.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Add comment failed:\n" + ex.Message, "Comments", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        public DataTable GetAttachments(string entityType, string entityRef)
        {
            DataTable dt = new DataTable();
            try
            {
                using (SqlConnection con = new SqlConnection(AppConfig.ConnectionString))
                using (SqlCommand cmd = new SqlCommand(@"
                    SELECT AttachmentID, FileName, FilePath, FileSizeKB, UploadedBy, UploadedDate, Remarks
                    FROM ProcurementAttachment
                    WHERE EntityType = @Type AND EntityRef = @Ref
                    ORDER BY UploadedDate DESC", con))
                {
                    cmd.Parameters.AddWithValue("@Type", entityType);
                    cmd.Parameters.AddWithValue("@Ref", entityRef);
                    con.Open();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        da.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Attachments load error:\n" + ex.Message, "Attachments", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            return dt;
        }

        public bool AddAttachment(string entityType, string entityRef, string fileName, string filePath, decimal sizeKb, string uploadedBy, string remarks)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(AppConfig.ConnectionString))
                using (SqlCommand cmd = new SqlCommand(@"
                    INSERT INTO ProcurementAttachment
                        (EntityType, EntityRef, FileName, FilePath, FileSizeKB, UploadedBy, Remarks)
                    VALUES (@Type, @Ref, @Name, @Path, @Size, @By, @Remarks)", con))
                {
                    cmd.Parameters.AddWithValue("@Type", entityType);
                    cmd.Parameters.AddWithValue("@Ref", entityRef);
                    cmd.Parameters.AddWithValue("@Name", fileName);
                    cmd.Parameters.AddWithValue("@Path", filePath);
                    cmd.Parameters.AddWithValue("@Size", sizeKb);
                    cmd.Parameters.AddWithValue("@By", uploadedBy ?? "");
                    cmd.Parameters.AddWithValue("@Remarks", (object?)remarks ?? DBNull.Value);
                    con.Open();
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Add attachment failed:\n" + ex.Message, "Attachments", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
    }

    public class PriceIntelligenceDAL
    {
        public decimal? GetLastPurchasePrice(string itemCode)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(AppConfig.ConnectionString))
                {
                    con.Open();
                    // Prefer latest approved PO unit price, then Receipt
                    using (SqlCommand cmd = new SqlCommand(@"
                        SELECT TOP 1 UnitPrice
                        FROM PurchaseOrder
                        WHERE ItemCode = @ItemCode AND UnitPrice IS NOT NULL AND UnitPrice > 0
                        ORDER BY ID DESC", con))
                    {
                        cmd.Parameters.AddWithValue("@ItemCode", itemCode ?? "");
                        object result = cmd.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                            return Convert.ToDecimal(result);
                    }

                    using (SqlCommand cmd = new SqlCommand(@"
                        SELECT TOP 1 UnitCost
                        FROM Receipt
                        WHERE ItemCode = @ItemCode AND UnitCost IS NOT NULL AND UnitCost > 0
                        ORDER BY ID DESC", con))
                    {
                        cmd.Parameters.AddWithValue("@ItemCode", itemCode ?? "");
                        object result = cmd.ExecuteScalar();
                        if (result != null && result != DBNull.Value)
                            return Convert.ToDecimal(result);
                    }
                }
            }
            catch
            {
                // Non-fatal — variance is advisory
            }
            return null;
        }

        public DataTable GetPriceVarianceForPR(string prNumber)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ItemCode");
            dt.Columns.Add("ItemDescription");
            dt.Columns.Add("CurrentPrice", typeof(decimal));
            dt.Columns.Add("LastPrice", typeof(decimal));
            dt.Columns.Add("VariancePct", typeof(decimal));
            dt.Columns.Add("VarianceAmt", typeof(decimal));
            dt.Columns.Add("Flag");

            try
            {
                using (SqlConnection con = new SqlConnection(AppConfig.ConnectionString))
                using (SqlCommand cmd = new SqlCommand(@"
                    SELECT ItemCode, ItemDescription, UnitCost
                    FROM PurchaseRequestDetailNew
                    WHERE PRNumber = @PRNumber", con))
                {
                    cmd.Parameters.AddWithValue("@PRNumber", prNumber);
                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string item = reader["ItemCode"]?.ToString() ?? "";
                            string desc = reader["ItemDescription"]?.ToString() ?? "";
                            decimal current = reader["UnitCost"] != DBNull.Value ? Convert.ToDecimal(reader["UnitCost"]) : 0m;
                            decimal? last = GetLastPurchasePrice(item);
                            decimal lastVal = last ?? 0m;
                            decimal varAmt = current - lastVal;
                            decimal varPct = lastVal > 0 ? (varAmt / lastVal) * 100m : 0m;
                            string flag = !last.HasValue ? "No history"
                                : Math.Abs(varPct) >= 10m ? "ALERT ≥10%"
                                : Math.Abs(varPct) >= 5m ? "Watch ≥5%"
                                : "OK";

                            dt.Rows.Add(item, desc, current, lastVal, Math.Round(varPct, 2), Math.Round(varAmt, 2), flag);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Price variance error:\n" + ex.Message, "Price Intelligence", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            return dt;
        }
    }
}
