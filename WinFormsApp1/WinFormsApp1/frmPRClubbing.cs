//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Drawing;
//using System.Text;
//using System.Windows.Forms;

//namespace WinFormsApp1
//{
//    public partial class frmPRClubbing : Form
//    {
//        public frmPRClubbing()
//        {
//            InitializeComponent();
//        }
//    }
//}


using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using WinFormsApp1;

namespace WinFormsApp1
{
    public partial class frmPRClubbing : Form
    {
        private PurchaseRequestDAL _dal;
        private DataTable _availablePRs;
        private DataTable _availablePRsDetails;
        private const decimal PR_LIMIT = 1200000; // 12 Lakh

        public frmPRClubbing()
        {
            InitializeComponent();
            _dal = new PurchaseRequestDAL();
        }

        private void frmPRClubbing_Load(object sender, EventArgs e)
        {
            LoadAvailablePRs();
            LoadVendorFilter();
            txtLimitRemaining.Text = PR_LIMIT.ToString("N2");
        }

        #region === Load Data ===

        private void LoadAvailablePRs()
        {
            try
            {
                _availablePRs = _dal.GetPRsForClubbing();
                RefreshPRGrid();
            }
            catch (Exception error)
            {
                MessageBox.Show("get error" + error.ToString(), error.ToString());
                //LoadSamplePRs();
            }
        }

        private void LoadSamplePRs()
        {
            dgvAvailablePRs.Rows.Clear();
            string[,] sample = {
                {"PR-2026-0001", "PRJ-2026-001", "PROD-001", "Vendor A", "450000", "5", "INR"},
                {"PR-2026-0002", "PRJ-2026-002", "PROD-003", "Vendor A", "350000", "3", "INR"},
                {"PR-2026-0003", "PRJ-2026-003", "PROD-005", "Vendor B", "280000", "4", "INR"},
                {"PR-2026-0004", "PRJ-2026-001", "PROD-002", "Vendor C", "150000", "2", "INR"},
                {"PR-2026-0005", "PRJ-2026-004", "PROD-007", "Vendor A", "200000", "3", "INR"}
            };

            for (int i = 0; i < sample.GetLength(0); i++)
            {
                dgvAvailablePRs.Rows.Add(false, sample[i, 0], sample[i, 1], sample[i, 2],
                    sample[i, 3], sample[i, 4], sample[i, 5], sample[i, 6]);
            }
            txtTotalPRs.Text = sample.GetLength(0).ToString();
        }

        private void RefreshPRGrid()
        {
            dgvAvailablePRs.Rows.Clear();
            if (_availablePRs == null) return;

            foreach (DataRow row in _availablePRs.Rows)
            {
                dgvAvailablePRs.Rows.Add(false,
                    row["PRNumber"].ToString(),
                    row["ProjectCode"].ToString(),
                    //row["ProductNo"].ToString(),
                    row["VendorCode"].ToString(),
                    Convert.ToDecimal(row["TotalAmount"]).ToString("N2"),
                    row["ApprovedBy"].ToString(),
                    "INR");
            }
            txtTotalPRs.Text = _availablePRs.Rows.Count.ToString();
        }

        private void LoadVendorFilter()
        {
            cmbFilterVendor.Items.Clear();
            cmbFilterVendor.Items.Add("All");

            HashSet<string> vendors = new HashSet<string>();
            foreach (DataGridViewRow row in dgvAvailablePRs.Rows)
            {
                string vendor = row.Cells["colPRVendor"].Value?.ToString();
                if (!string.IsNullOrEmpty(vendor))
                    vendors.Add(vendor);
            }

            foreach (var v in vendors.OrderBy(x => x))
                cmbFilterVendor.Items.Add(v);

            cmbFilterVendor.SelectedIndex = 0;
        }

        #endregion

        #region === Filter & Selection ===

        private void btnFilter_Click(object sender, EventArgs e)
        {
            string vendor = cmbFilterVendor.SelectedItem?.ToString();
            //MessageBox.Show(vendor, "Selected Vendor...");
            if (vendor == "All") vendor = null;

            try
            {
                _availablePRs = _dal.GetPRsForClubbing(vendor);
                RefreshPRGrid();
            }
            catch
            {
                // Manual filter
                foreach (DataGridViewRow row in dgvAvailablePRs.Rows)
                {
                    bool visible = vendor == null || row.Cells["colPRVendor"].Value?.ToString() == vendor;
                    row.Visible = visible;
                }
            }
        }

        private void btnClearFilter_Click(object sender, EventArgs e)
        {
            cmbFilterVendor.SelectedIndex = 0;
            foreach (DataGridViewRow row in dgvAvailablePRs.Rows)
                row.Visible = true;
        }

        private void dgvAvailablePRs_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgvAvailablePRs.IsCurrentCellDirty)
                dgvAvailablePRs.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void dgvAvailablePRs_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (dgvAvailablePRs.Columns[e.ColumnIndex].Name == "colPRSelect")
            {
                UpdateSelectionSummary();
                PreviewClubbedPR();
            }
        }

        private void UpdateSelectionSummary()
        {
            int selected = 0;
            decimal total = 0;

            foreach (DataGridViewRow row in dgvAvailablePRs.Rows)
            {
                DataGridViewCheckBoxCell chk = row.Cells["colPRSelect"] as DataGridViewCheckBoxCell;
                if (chk != null && chk.Value != null && (bool)chk.Value)
                {
                    selected++;
                    decimal amt = 0;
                    decimal.TryParse(row.Cells["colPRAmount"].Value?.ToString(), out amt);
                    total += amt;
                }
            }

            txtSelectedPRs.Text = selected.ToString();
            txtClubbedAmount.Text = total.ToString("N2");

            decimal remaining = PR_LIMIT - total;
            txtLimitRemaining.Text = remaining.ToString("N2");

            if (total > PR_LIMIT)
            {
                txtLimitRemaining.ForeColor = Color.Red;
                btnGenerateClubbedPR.Enabled = false;
            }
            else
            {
                txtLimitRemaining.ForeColor = Color.Green;
                btnGenerateClubbedPR.Enabled = true;
            }
        }

        #endregion

        #region === Preview Clubbed PR ===

        private void PreviewClubbedPR()
        {
            dgvClubbedPR.Rows.Clear();

            // Get selected PRs
            List<string> selectedPRs = new List<string>();
            foreach (DataGridViewRow row in dgvAvailablePRs.Rows)
            {
                DataGridViewCheckBoxCell chk = row.Cells["colPRSelect"] as DataGridViewCheckBoxCell;
                if (chk != null && chk.Value != null && (bool)chk.Value)
                {
                    selectedPRs.Add(row.Cells["colPRNumber"].Value?.ToString());
                }
            }

            if (selectedPRs.Count == 0) return;

            // In real implementation, fetch details from PurchaseRequestDetail
            // For now, show summary
            foreach (string pr in selectedPRs)
            {
                _availablePRsDetails = _dal.GetPRsDetails(pr);

                foreach (DataRow row in _availablePRsDetails.Rows)
                {
                    dgvClubbedPR.Rows.Add(row["ItemCode"].ToString(), row["ItemDescription"].ToString(), row["Quantity"].ToString(), row["UnitCost"].ToString(),row["TotalCost"].ToString(), row["VendorCode"].ToString(), row["PRNumber"].ToString());

                }// row["ItemCode"].ToString()
            }

            // Generate new PR number
            try { txtNewPRNumber.Text = _dal.GetNextPRNumber(); }
            catch { txtNewPRNumber.Text = "PR-" + DateTime.Now.Year + "-CLUB-" + DateTime.Now.ToString("MMddHHmmss"); }
        }

        #endregion

        #region === Generate Clubbed PR ===

        private void btnGenerateClubbedPR_Click(object sender, EventArgs e)
        {
            decimal total = 0;
            decimal.TryParse(txtClubbedAmount.Text, out total);

            if (total > PR_LIMIT)
            {
                MessageBox.Show("Clubbed amount exceeds 12 Lakh limit.", "Limit Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            List<string> selectedPRs = new List<string>();
            HashSet<string> vendors = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (DataGridViewRow row in dgvAvailablePRs.Rows)
            {
                DataGridViewCheckBoxCell chk = row.Cells["colPRSelect"] as DataGridViewCheckBoxCell;
                if (chk != null && chk.Value != null && (bool)chk.Value)
                {
                    selectedPRs.Add(row.Cells["colPRNumber"].Value?.ToString());
                    vendors.Add(row.Cells["colPRVendor"].Value?.ToString() ?? "");
                }
            }

            if (selectedPRs.Count < 2)
            {
                MessageBox.Show("Select at least two PRs to club.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (vendors.Count > 1)
            {
                MessageBox.Show("All selected PRs must belong to the same vendor.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string createdBy = AppSession.IsAuthenticated ? AppSession.UserName : Environment.UserName;
            string newPR = _dal.CreateClubbedPR(selectedPRs, createdBy, out string error);

            if (string.IsNullOrEmpty(newPR))
            {
                MessageBox.Show("Clubbing failed:\n" + error, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            txtNewPRNumber.Text = newPR;
            MessageBox.Show(
                "Clubbed PR Generated:\n" + newPR +
                "\n\nFrom " + selectedPRs.Count + " PRs\nTotal: " + total.ToString("N2") + " INR" +
                "\n\nSource PRs marked as Clubbed. New PR is Approved and ready for PO.",
                "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

            LoadAvailablePRs();
            dgvClubbedPR.Rows.Clear();
        }

        #endregion

        #region === Other Buttons ===

        private void btnPreview_Click(object sender, EventArgs e)
        {
            PreviewClubbedPR();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "CSV files (*.csv)|*.csv";
            dlg.FileName = "ClubbedPR_" + DateTime.Now.ToString("yyyyMMdd") + ".csv";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (StreamWriter w = new StreamWriter(dlg.FileName))
                    {
                        w.WriteLine("PRNumber,ProjectCode,ProductNo,Vendor,Amount,Items,Currency");
                        foreach (DataGridViewRow row in dgvAvailablePRs.Rows)
                        {
                            DataGridViewCheckBoxCell chk = row.Cells["colPRSelect"] as DataGridViewCheckBoxCell;
                            if (chk != null && chk.Value != null && (bool)chk.Value)
                            {
                                w.WriteLine(row.Cells["colPRNumber"].Value + "," + row.Cells["colPRProjectCode"].Value + "," +
                                    row.Cells["colPRProductNo"].Value + "," + row.Cells["colPRVendor"].Value + "," +
                                    row.Cells["colPRAmount"].Value + "," + row.Cells["colPRItemCount"].Value + "," +
                                    row.Cells["colPRCurrency"].Value);
                            }
                        }
                    }
                    MessageBox.Show("Exported.", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Export failed:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            PrintDocument pd = new PrintDocument();
            pd.PrintPage += new PrintPageEventHandler(PrintPage);
            PrintPreviewDialog pv = new PrintPreviewDialog();
            pv.Document = pd;
            pv.ShowDialog();
        }

        private void PrintPage(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            Font f = new Font("Arial", 9);
            Font b = new Font("Arial", 9, FontStyle.Bold);
            Font t = new Font("Arial", 14, FontStyle.Bold);
            float y = 40;
            float lh = f.GetHeight() + 3;

            g.DrawString("PR CLUBBING REPORT", t, Brushes.Black, 40, y);
            y += lh * 2;
            g.DrawString("Selected PRs to be clubbed:", b, Brushes.Black, 40, y);
            y += lh;

            foreach (DataGridViewRow row in dgvAvailablePRs.Rows)
            {
                DataGridViewCheckBoxCell chk = row.Cells["colPRSelect"] as DataGridViewCheckBoxCell;
                if (chk != null && chk.Value != null && (bool)chk.Value)
                {
                    g.DrawString(row.Cells["colPRNumber"].Value + " - " + row.Cells["colPRVendor"].Value +
                        " - " + row.Cells["colPRAmount"].Value + " " + row.Cells["colPRCurrency"].Value,
                        f, Brushes.Black, 40, y);
                    y += lh;
                }
            }

            y += lh;
            g.DrawString("New Clubbed PR: " + txtNewPRNumber.Text, b, Brushes.Black, 40, y);
            y += lh;
            g.DrawString("Total Amount: " + txtClubbedAmount.Text, b, Brushes.Black, 40, y);
            e.HasMorePages = false;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        private void grpAvailablePRs_Enter(object sender, EventArgs e)
        {

        }
    }
}