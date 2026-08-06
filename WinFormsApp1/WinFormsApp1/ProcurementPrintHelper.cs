using System;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public static class ProcurementPrintHelper
    {
        public static void PrintEntityPack(string entityType, string entityRef, CommentAttachmentDAL? commentDal = null)
        {
            commentDal ??= new CommentAttachmentDAL();
            DataTable comments = commentDal.GetComments(entityType, entityRef);
            DataTable files = commentDal.GetAttachments(entityType, entityRef);

            PrintDocument pd = new PrintDocument();
            pd.PrintPage += (s, e) =>
            {
                if (e.Graphics == null) return;
                Graphics g = e.Graphics;
                Font title = new Font("Segoe UI", 16, FontStyle.Bold);
                Font head = new Font("Segoe UI", 10, FontStyle.Bold);
                Font body = new Font("Segoe UI", 9);
                float y = 40;
                float left = 40;
                float lh = body.GetHeight(g) + 4;

                g.DrawString("ERP Procurement Pack", title, Brushes.Black, left, y);
                y += lh * 2;
                g.DrawString($"{entityType}: {entityRef}", head, Brushes.Black, left, y);
                y += lh;
                g.DrawString($"Printed: {DateTime.Now:g}   By: {(AppSession.IsAuthenticated ? AppSession.UserName : Environment.UserName)}", body, Brushes.Gray, left, y);
                y += lh * 2;

                g.DrawString("Comments", head, Brushes.Black, left, y);
                y += lh;
                if (comments.Rows.Count == 0)
                {
                    g.DrawString("(none)", body, Brushes.Gray, left, y);
                    y += lh;
                }
                else
                {
                    foreach (DataRow row in comments.Rows)
                    {
                        string line = $"{row["CreatedDate"]:g}  {row["CreatedBy"]}: {row["CommentText"]}";
                        g.DrawString(Truncate(line, 110), body, Brushes.Black, left, y);
                        y += lh;
                        if (y > e.MarginBounds.Bottom - 80) break;
                    }
                }

                y += lh;
                g.DrawString("Attachments", head, Brushes.Black, left, y);
                y += lh;
                if (files.Rows.Count == 0)
                {
                    g.DrawString("(none)", body, Brushes.Gray, left, y);
                }
                else
                {
                    foreach (DataRow row in files.Rows)
                    {
                        g.DrawString($"{row["FileName"]}  ({row["FileSizeKB"]} KB)  by {row["UploadedBy"]}", body, Brushes.Black, left, y);
                        y += lh;
                        if (y > e.MarginBounds.Bottom - 40) break;
                    }
                }

                e.HasMorePages = false;
            };

            using PrintPreviewDialog preview = new PrintPreviewDialog
            {
                Document = pd,
                Width = 900,
                Height = 700
            };
            preview.ShowDialog();
        }

        public static void PrintPriceVariance(string prNumber, DataTable variance)
        {
            PrintDocument pd = new PrintDocument();
            pd.PrintPage += (s, e) =>
            {
                if (e.Graphics == null) return;
                Graphics g = e.Graphics;
                Font title = new Font("Segoe UI", 14, FontStyle.Bold);
                Font head = new Font("Segoe UI", 9, FontStyle.Bold);
                Font body = new Font("Consolas", 8);
                float y = 40;
                float left = 30;
                float lh = body.GetHeight(g) + 3;

                g.DrawString($"Price Variance Report — PR {prNumber}", title, Brushes.Black, left, y);
                y += lh * 2;
                g.DrawString(
                    $"{"Item",-16} {"Current",10} {"Last",10} {"Var%",8} {"Flag",12}",
                    head, Brushes.Black, left, y);
                y += lh;

                foreach (DataRow row in variance.Rows)
                {
                    string line =
                        $"{Truncate(row["ItemCode"]?.ToString() ?? "", 16),-16} " +
                        $"{Convert.ToDecimal(row["CurrentPrice"]),10:N2} " +
                        $"{Convert.ToDecimal(row["LastPrice"]),10:N2} " +
                        $"{Convert.ToDecimal(row["VariancePct"]),7:N1}% " +
                        $"{row["Flag"]}";
                    Brush brush = (row["Flag"]?.ToString() ?? "").StartsWith("ALERT")
                        ? Brushes.Firebrick
                        : Brushes.Black;
                    g.DrawString(line, body, brush, left, y);
                    y += lh;
                    if (y > e.MarginBounds.Bottom - 40) break;
                }
                e.HasMorePages = false;
            };

            using PrintPreviewDialog preview = new PrintPreviewDialog
            {
                Document = pd,
                Width = 900,
                Height = 700
            };
            preview.ShowDialog();
        }

        private static string Truncate(string value, int max)
            => value.Length <= max ? value : value.Substring(0, max - 1) + "…";
    }
}
