//using System;
//using System.Collections.Generic;
//using System.ComponentModel;
//using System.Data;
//using System.Drawing;
//using System.Text;
//using System.Windows.Forms;

//namespace WinFormsApp1
//{
//    public partial class frmPurchaseRequestGeneration : Form
//    {
//        public frmPurchaseRequestGeneration()
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
    public partial class frmPurchaseRequestGeneration : Form
    {
        private PurchaseRequestDAL _dal;
        private DataTable _bomItems;
        private const decimal PR_LIMIT = 1200000; // 12 Lakh limit
        private bool _isSelectAllChanging = false;


        public frmPurchaseRequestGeneration()
        {
            InitializeComponent();
            _dal = new PurchaseRequestDAL();
        }

        private void frmPurchaseRequestGeneration_Load(object sender, EventArgs e)
        {
            txtCreatedBy.Text = AppSession.IsAuthenticated ? AppSession.UserName : Environment.UserName;
            dtpCreatedDate.Value = DateTime.Now;
            txtLimitRemaining.Text = PR_LIMIT.ToString("N2");
            LoadProjectCodes();
        }

        #region === Load Data ===

        private void LoadProjectCodes()
        {
            // In real implementation, load from ProjectBOM header table
            // For now, sample data
            cmbProjectCode.Items.Clear();

            DataTable projectList = _dal.GetBOMProjects("projectcode", null);

            foreach (DataRow dr in projectList.Rows)
            {
                if (!cmbProjectCode.Items.Contains(dr["ProjectCode"].ToString()))
                    cmbProjectCode.Items.Add(dr["ProjectCode"].ToString());
            }

            cmbProjectCode.SelectedIndex = -1;

        }

        private void cmbProjectCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbProjectCode.SelectedItem != null)
            {
                string projectCode = cmbProjectCode.SelectedItem.ToString();
                txtProjectName.Text = "Project: " + projectCode;
                LoadProductNumbers(projectCode);
            }
        }

        private void LoadProductNumbers(string projectCode)
        {
            cmbProductNo.Items.Clear();
            // Load product numbers from ProjectBOM for this project
            DataTable productList = _dal.GetBOMProjects("product", projectCode);

            foreach (DataRow dr in productList.Rows)
            {
                if (!cmbProductNo.Items.Contains(dr["ProductCode"].ToString()) && dr["ProductCode"].ToString() != "")
                    cmbProductNo.Items.Add(dr["ProductCode"].ToString());
            }

            //cmbProductNo.SelectedIndex = 1;
        }

        private void btnLoadBOM_Click(object sender, EventArgs e)
        {
            if (cmbProjectCode.SelectedItem == null)
            {
                MessageBox.Show("Please select a Project Code.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string projectCode = cmbProjectCode.SelectedItem.ToString();
            string productNo = cmbProductNo.SelectedItem?.ToString();

            try
            {
                _bomItems = _dal.GetBOMItemsByProject(projectCode, productNo);
                LoadBOMGrid();
                txtTotalItems.Text = _bomItems.Rows.Count.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading BOM: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                //LoadSampleBOMData(); // Fallback
            }
        }

        private void LoadBOMGrid()
        {
            dgvBOMItems.Rows.Clear();

            // MessageBox.Show(_bomItems.Rows.Count.ToString(), "Count of the BOM list..!");//total 637 lines but only 273 lines only in UI how what when wrong
            foreach (DataRow row in _bomItems.Rows)
            {
                bool alreadyInPR = row["ConvertedQty"] != DBNull.Value && int.Parse(row["ConvertedQty"].ToString()) == int.Parse(row["BOMQty"].ToString()) ? true : false;
                //MessageBox.Show(row["ConvertedQty"].ToString());
                float balanceQty = row["BOMBalanceIssueQty"] != DBNull.Value ? Convert.ToSingle(row["BOMBalanceIssueQty"]) : 0;

                if (balanceQty <= 0 || int.Parse(row["Pending PO QTY"].ToString()) == int.Parse(row["BOMQty"].ToString()))//alreadyInPR
                {
                    //MessageBox.Show($"BOMBalanceIssueQty :{balanceQty}\n\n Pending PO QTY:{row["Pending PO QTY"].ToString()}\n\n BOMQty : {row["BOMQty"].ToString()}", "Skipping Row:");

                    // Skip items that are fully purchased or already in PR
                    continue;
                }

                string fixedCost = row["FixedCost"] != DBNull.Value
                    ? Convert.ToDecimal(row["FixedCost"]).ToString("N2")
                    : "0.00";


                decimal balanceQty_ = Convert.ToDecimal(row["BOMBalanceIssueQty"]);
                decimal unitCost_ = Convert.ToDecimal(row["UnitCost"]);

                string totalCost = (balanceQty_ * unitCost_).ToString();//"0.00"

                //MessageBox.Show(row["HSNSACCode"].ToString());

                int idx = dgvBOMItems.Rows.Add(
                    false, // Select checkbox
                    row["ProjectBOMCode"].ToString(),
                    row["ItemName"].ToString(),
                    row["ItemDescription"].ToString(),
                    row["ProductCode"].ToString(),
                    row["ProductNo"].ToString(),
                    row["IssuedQty"].ToString(),
                    row["BOMQty"].ToString(),//ConvertedQty
                    row["ConvertedQty"].ToString(),//
                    row["Pending PO QTY"].ToString(),
                    row["POQty"].ToString(),//
                    row["AvailableQty"].ToString(),//
                    row["BOMBalanceIssueQty"].ToString(), // UnitCost
                    row["UnitCost"].ToString(), // UnitCost
                    totalCost,
                   "", // Total cost - calculated
                       // "", // Vendor - dropdown
                    row["HSNSACCode"].ToString(),
                    row["Location"].ToString(),//Location
                                               //
                    alreadyInPR ? "Already in PR'" + row["Pending PO QTY"].ToString() + "'" : "NO Pending PO"
                );

                // Load vendor dropdown for this item
                LoadVendorDropdown(idx, row["ItemName"].ToString());

                float existPOQty = row["POQty"] != DBNull.Value ? Convert.ToSingle(row["POQty"]) : 0;


                // Color code rows
                if (alreadyInPR  || existPOQty == Convert.ToDouble(row["BOMQty"].ToString()))
                {
                    dgvBOMItems.Rows[idx].DefaultCellStyle.BackColor = Color.LightGreen;
                    DataGridViewCheckBoxCell chk = (DataGridViewCheckBoxCell)dgvBOMItems.Rows[idx].Cells["colSelect"];
                    chk.ReadOnly = true;
                }
                else if (existPOQty > 0)
                {
                    dgvBOMItems.Rows[idx].DefaultCellStyle.BackColor = Color.LightYellow;
                }
            }
        }

        private void LoadVendorDropdown(int rowIndex, string itemCode)
        {
            try
            {
                DataTable vendors = _dal.GetVendorsForItem(itemCode);
                DataGridViewComboBoxCell vendorCell = (DataGridViewComboBoxCell)dgvBOMItems.Rows[rowIndex].Cells["colVendor"];
                vendorCell.Items.Clear();

                // foreach (DataRow v in vendors.Rows)
                // {
                // string vendorText = v["VendorCode"].ToString();
                // if (v["IsPreviousVendor"] != DBNull.Value && Convert.ToBoolean(v["IsPreviousVendor"]))
                //  vendorText += " (Previous)";
                foreach (DataRow dr in vendors.Rows)
                {
                    if (!vendorCell.Items.Contains(dr["VendorCode"].ToString()))
                        vendorCell.Items.Add(dr["VendorCode"].ToString());
                }
                //vendorCell.Items.Add(vendorText);
                //}

                if (vendorCell.Items.Count > 0)
                    vendorCell.Value = vendorCell.Items[0];
            }
            catch
            {
                // Fallback vendors
                DataGridViewComboBoxCell vendorCell = (DataGridViewComboBoxCell)dgvBOMItems.Rows[rowIndex].Cells["colVendor"];
                vendorCell.Items.Add("Vendor A");
                vendorCell.Items.Add("Vendor B");
                vendorCell.Items.Add("Vendor C");
                vendorCell.Value = "Vendor A";
            }
        }

        private void LoadSampleBOMData()
        {
            // Fallback sample data when database is not available
            dgvBOMItems.Rows.Clear();

            string[,] sampleData = {
                {"BOM-001", "MEC-XX-1001", "MOTOR 5HP", "5HP, 3Phase", "Siemens", "10", "Nos", "5", "5", "25000", "Vendor A"},
                {"BOM-002", "ELE-XX-1002", "CABLE 4SQMM", "Copper, 4mm2", "Polycab", "100", "Meter", "0", "100", "150", "Vendor B"},
                {"BOM-003", "HYD-XX-1003", "CYLINDER 100X200", "Bore 100mm", "HydroTech", "5", "Nos", "2", "3", "8500", "Vendor A"},
                {"BOM-004", "MEC-XX-1004", "BEARING 6205", "Deep Groove", "SKF", "50", "Nos", "20", "30", "450", "Vendor C"},
                {"BOM-005", "ELE-XX-1005", "PLC S7-1200", "Siemens PLC", "Siemens", "3", "Nos", "1", "2", "45000", "Vendor B"}
            };

            for (int i = 0; i < sampleData.GetLength(0); i++)
            {
                int idx = dgvBOMItems.Rows.Add(
                    false,
                    sampleData[i, 0], sampleData[i, 1], sampleData[i, 2], sampleData[i, 3],
                    sampleData[i, 4], sampleData[i, 5], sampleData[i, 6], sampleData[i, 7],
                    sampleData[i, 8], sampleData[i, 9], "0.00", sampleData[i, 10], sampleData[i, 8], "Available"
                );

                DataGridViewComboBoxCell vendorCell = (DataGridViewComboBoxCell)dgvBOMItems.Rows[idx].Cells["colVendor"];
                vendorCell.Items.Add("Vendor A (Previous)");
                vendorCell.Items.Add("Vendor B");
                vendorCell.Items.Add("Vendor C");
                vendorCell.Value = sampleData[i, 10];
            }

            txtTotalItems.Text = sampleData.GetLength(0).ToString();
        }

        #endregion

        #region === Grid Events ===

        private void dgvBOMItems_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgvBOMItems.IsCurrentCellDirty)
                dgvBOMItems.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        private void dgvBOMItems_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            // Recalculate totals when checkbox, qty, or vendor changes
            if (dgvBOMItems.Columns[e.ColumnIndex].Name == "colSelect" ||
                dgvBOMItems.Columns[e.ColumnIndex].Name == "colAlreadyPurchased" ||
                dgvBOMItems.Columns[e.ColumnIndex].Name == "colUnitCost")
            {
                CalculateTotals();
            }
        }

        private void CalculateTotals()
        {
            int selectedCount = 0;
            decimal totalAmount = 0;

            foreach (DataGridViewRow row in dgvBOMItems.Rows)
            {
                DataGridViewCheckBoxCell chk = row.Cells["colSelect"] as DataGridViewCheckBoxCell;

                if (chk != null && chk.Value != null && (bool)chk.Value)
                {


                    selectedCount++;

                    decimal unitCost = 0;
                    decimal qty = 0;

                    if (row.Cells["colUnitCost"].Value != null)
                        decimal.TryParse(row.Cells["colUnitCost"].Value.ToString(), out unitCost);
                    if (row.Cells["colBalanceQty"].Value != null)
                        decimal.TryParse(row.Cells["colBalanceQty"].Value.ToString(), out qty);

                    decimal lineTotal = unitCost * qty;
                    //MessageBox.Show(lineTotal.ToString(), "getting update - colTotalCost");
                    row.Cells["colTotalCost"].Value = lineTotal.ToString("N2");
                    totalAmount += lineTotal;
                }
                else
                {
                    //row.Cells["colTotalCost"].Value = "0.00";
                    //MessageBox.Show("This stage colTotalCost is getting ZERO...?");
                }
            }

            txtSelectedItems.Text = selectedCount.ToString();
            txtTotalAmount.Text = totalAmount.ToString("N2");

            decimal remaining = PR_LIMIT - totalAmount;
            txtLimitRemaining.Text = remaining.ToString("N2");

            if (totalAmount > PR_LIMIT)
            {
                lblLimitStatus.Text = "EXCEEDED!";
                lblLimitStatus.ForeColor = Color.Red;
                btnGeneratePR.Enabled = false;
            }
            else if (totalAmount > PR_LIMIT * 0.9m)
            {
                lblLimitStatus.Text = "Warning: Near Limit";
                lblLimitStatus.ForeColor = Color.Orange;
                btnGeneratePR.Enabled = true;
            }
            else
            {
                lblLimitStatus.Text = "OK";
                lblLimitStatus.ForeColor = Color.Green;
                btnGeneratePR.Enabled = true;
            }
        }

        #endregion

        #region === Auto Group by Vendor ===

        private void chkGroupByVendor_CheckedChanged(object sender, EventArgs e)
        {
            if (chkGroupByVendor.Checked)
                AutoGroupByVendor();
            else
                dgvVendorGroups.Rows.Clear();
        }

        private void btnAutoGroup_Click(object sender, EventArgs e)
        {
            AutoGroupByVendor();
        }

        private void AutoGroupByVendor()
        {
            dgvVendorGroups.Rows.Clear();

            Dictionary<string, List<DataGridViewRow>> vendorGroups = new Dictionary<string, List<DataGridViewRow>>();

            foreach (DataGridViewRow row in dgvBOMItems.Rows)
            {
                DataGridViewCheckBoxCell chk = row.Cells["colSelect"] as DataGridViewCheckBoxCell;
                if (chk != null && chk.Value != null && (bool)chk.Value)
                {
                    string vendor = row.Cells["colVendor"].Value?.ToString() ?? "Unknown";
                    if (!vendorGroups.ContainsKey(vendor))
                        vendorGroups[vendor] = new List<DataGridViewRow>();
                    vendorGroups[vendor].Add(row);
                }
            }

            foreach (var group in vendorGroups)
            {
                decimal groupTotal = 0;
                foreach (var row in group.Value)
                {
                    decimal total = 0;
                    decimal.TryParse(row.Cells["colTotalCost"].Value?.ToString(), out total);
                    groupTotal += total;
                }

                // Generate PR number for this group
                string prNumber = "";
                try { prNumber = _dal.GetNextPRNumber(); }
                catch { prNumber = "PR-" + DateTime.Now.Year + "-" + DateTime.Now.Month.ToString("D2") + "-XXXXXXX"; }

                dgvVendorGroups.Rows.Add(
                    group.Key,
                    group.Value.Count.ToString(),
                    groupTotal.ToString("N2"),
                    prNumber + " (preview)"
                );
            }
        }

        #endregion

        #region === Generate PR ===

        private void btnGeneratePR_Click(object sender, EventArgs e)
        {
            if (dgvBOMItems.Rows.Count == 0)
            {
                MessageBox.Show("No BOM items loaded. Please select a project and load BOM.",
                    "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Validate 12L limit
            decimal totalAmount = 0;
            decimal.TryParse(txtTotalAmount.Text, out totalAmount);
            if (totalAmount > PR_LIMIT)
            {
                MessageBox.Show("Total amount exceeds 12 Lakh limit. Please reduce items.",
                    "Limit Exceeded", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Get selected items
            List<DataGridViewRow> selectedRows = new List<DataGridViewRow>();
            foreach (DataGridViewRow row in dgvBOMItems.Rows)
            {
                DataGridViewCheckBoxCell chk = row.Cells["colSelect"] as DataGridViewCheckBoxCell;
                if (chk != null && chk.Value != null && (bool)chk.Value)
                    selectedRows.Add(row);
            }

            if (selectedRows.Count == 0)
            {
                MessageBox.Show("Please select at least one item.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Group by vendor
            Dictionary<string, List<DataGridViewRow>> vendorGroups = new Dictionary<string, List<DataGridViewRow>>();
            foreach (var row in selectedRows)
            {
                string vendor = row.Cells["colVendor"].Value?.ToString() ?? "Unknown";
                if (!vendorGroups.ContainsKey(vendor))
                    vendorGroups[vendor] = new List<DataGridViewRow>();
                vendorGroups[vendor].Add(row);
            }

            // Generate PR for each vendor group
            int prCount = 0;
            List<string> generatedPRs = new List<string>();

            foreach (var group in vendorGroups)
            {
                // Check if group total is within limit
                decimal groupTotal = 0;
                foreach (var row in group.Value)
                {
                    decimal total = 0;
                    decimal.TryParse(row.Cells["colTotalCost"].Value?.ToString(), out total);
                    //MessageBox.Show(row.Cells["colTotalCost"].Value?.ToString());

                    groupTotal += total;
                }
                //MessageBox.Show(groupTotal.ToString());

                if (groupTotal > PR_LIMIT)
                {
                    MessageBox.Show("Vendor group '" + group.Key + "' exceeds 12L limit. Splitting not implemented.",
                        "Limit Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    continue;
                }

                string prNumber = _dal.GetNextPRNumber();
                if (string.IsNullOrWhiteSpace(prNumber))
                {
                    MessageBox.Show("Could not generate next PR number.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    continue;
                }

                string projectCode = cmbProjectCode.SelectedItem?.ToString() ?? "";
                string createdBy = txtCreatedBy.Text;

                // Insert PR Header — returns real identity
                int headerId = _dal.InsertPRHeader(
                    prNumber, projectCode, group.Key, _dal.GetVendorName(group.Key),
                    groupTotal, "INR", 1, "Generated from BOM", createdBy);

                if (headerId <= 0)
                {
                    MessageBox.Show("Failed to create PR header for vendor " + group.Key,
                        "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    continue;
                }

                bool detailsOk = true;
                foreach (var row in group.Value)
                {
                    float bomQty = GetFloatValue(row.Cells["colBOMQty"].Value);
                    float convertedQty = GetFloatValue(row.Cells["colConvertedQty"].Value);
                    float requestQty = bomQty - convertedQty;
                    if (requestQty <= 0)
                        requestQty = GetFloatValue(row.Cells["colBalanceQty"].Value);

                    PurchaseRequestDetail detail = new PurchaseRequestDetail
                    {
                        PRID = headerId.ToString(),
                        PRNumber = prNumber,
                        ProjectBOMCode = row.Cells["colProjectBOMCode"].Value?.ToString(),
                        ProjectCode = projectCode,
                        ProductCode = row.Cells["colProductCode"].Value?.ToString(),
                        ProductNo = row.Cells["colProductNo"].Value?.ToString(),
                        ItemCode = row.Cells["colItemCode"].Value?.ToString(),
                        ItemDescription = row.Cells["colItemName"].Value?.ToString(),
                        Quantity = requestQty,
                        BOMQuantity = bomQty,
                        AlreadyPurchasedQty = GetFloatValue(row.Cells["colAlreadyPurchased"].Value),
                        BalanceQty = GetFloatValue(row.Cells["colBalanceQty"].Value),
                        UnitCost = GetDecimalValue(row.Cells["colUnitCost"].Value),
                        TotalCost = GetDecimalValue(row.Cells["colTotalCost"].Value),
                        VendorCode = row.Cells["colVendor"].Value?.ToString(),
                        VendorName = _dal.GetVendorName(group.Key),
                        HSNCode = row.Cells["colHSNCode"].Value?.ToString(),
                        Location = row.Cells["collocation"].Value?.ToString(),
                        CreatedBy = createdBy
                    };

                    int detailId = _dal.InsertPRDetail(detail);
                    if (detailId <= 0)
                    {
                        detailsOk = false;
                        MessageBox.Show("Failed to add detail for item " + detail.ItemCode,
                            "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        continue;
                    }

                    bool tracked = _dal.TrackBOMConversion(
                        projectCode,
                        row.Cells["colProductNo"].Value?.ToString(),
                        detail.ItemCode,
                        detail.ProjectBOMCode,
                        prNumber,
                        detail.Quantity,
                        createdBy,
                        headerId);

                    if (!tracked)
                    {
                        MessageBox.Show("BOM conversion tracking failed for " + detail.ItemCode,
                            "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }

                if (detailsOk)
                {
                    prCount++;
                    generatedPRs.Add(prNumber + " (" + group.Key + ") [PRID=" + headerId + "]");
                }
            }

            if (prCount > 0)
            {
                string msg = prCount + " Purchase Request(s) generated successfully:\n\n" +
                    string.Join("\n", generatedPRs) + "\n\n" +
                    "Total Amount: " + totalAmount.ToString("N2") + " INR\n" +
                    "Items: " + selectedRows.Count;

                MessageBox.Show(msg, "PR Generated", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Refresh to show updated status
                btnLoadBOM_Click(sender, e);
            }
        }

        private float GetFloatValue(object value)
        {
            float result = 0;
            if (value != null) float.TryParse(value.ToString(), out result);
            return result;
        }

        private decimal GetDecimalValue(object value)
        {
            decimal result = 0;
            if (value != null) decimal.TryParse(value.ToString(), out result);
            return result;
        }

        #endregion

        #region === Other Buttons ===

        private void btnRefreshProjects_Click(object sender, EventArgs e)
        {
            LoadProjectCodes();
            MessageBox.Show("Project list refreshed.", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnClearSelection_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dgvBOMItems.Rows)
            {
                DataGridViewCheckBoxCell chk = row.Cells["colSelect"] as DataGridViewCheckBoxCell;
                if (chk != null) chk.Value = false;
            }
            CalculateTotals();
            dgvVendorGroups.Rows.Clear();
        }

        private void btnViewPRHistory_Click(object sender, EventArgs e)
        {
            // Phase 1: open approval list as the PR history/status view
            frmPurchaseRequestApproval historyForm = new frmPurchaseRequestApproval();
            historyForm.ShowDialog(this);
        }

        private void btnClubPR_Click(object sender, EventArgs e)
        {
            frmPRClubbing clubForm = new frmPRClubbing();
            clubForm.ShowDialog(this);
        }

        private void btnApprovalPR_Click(object sender, EventArgs e)
        {
            frmPurchaseRequestApproval approvalForm = new frmPurchaseRequestApproval();
            approvalForm.ShowDialog(this);
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "CSV files (*.csv)|*.csv";
            dlg.FileName = "BOM_PR_" + DateTime.Now.ToString("yyyyMMdd") + ".csv";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (StreamWriter w = new StreamWriter(dlg.FileName))
                    {
                        w.WriteLine("ProjectBOMCode,ItemCode,ItemName,Specification,Make,BOMQty,UOM,AlreadyPurchased,BalanceQty,UnitCost,TotalCost,Vendor,PRQty,Status");
                        foreach (DataGridViewRow row in dgvBOMItems.Rows)
                        {
                            w.WriteLine(string.Join(",",
                                Csv(row.Cells["colProjectBOMCode"].Value),
                                Csv(row.Cells["colItemCode"].Value),
                                Csv(row.Cells["colItemName"].Value),
                                "",
                                "",
                                Csv(row.Cells["colBOMQty"].Value),
                                "",
                                Csv(row.Cells["colAlreadyPurchased"].Value),
                                Csv(row.Cells["colBalanceQty"].Value),
                                Csv(row.Cells["colUnitCost"].Value),
                                Csv(row.Cells["colTotalCost"].Value),
                                Csv(row.Cells["colVendor"].Value),
                                "",
                                Csv(row.Cells["colStatus"].Value)
                            ));
                        }
                    }
                    MessageBox.Show("Exported to:\n" + dlg.FileName, "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            pd.PrintPage += new PrintPageEventHandler(PrintDocument_PrintPage);
            PrintPreviewDialog pv = new PrintPreviewDialog();
            pv.Document = pd;
            pv.ShowDialog();
        }

        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            Font f = new Font("Arial", 9);
            Font b = new Font("Arial", 9, FontStyle.Bold);
            Font t = new Font("Arial", 14, FontStyle.Bold);
            float y = 40;
            float lh = f.GetHeight() + 3;

            g.DrawString("BOM TO PURCHASE REQUEST", t, Brushes.Black, 40, y);
            y += lh * 2;
            g.DrawString("Project: " + cmbProjectCode.SelectedItem + "    Product: " + cmbProductNo.SelectedItem, f, Brushes.Black, 40, y);
            y += lh * 2;

            g.DrawLine(Pens.Black, 40, y, 700, y);
            y += 3;
            string[] headers = { "ItemCode", "ItemName", "BOMQty", "Balance", "UnitCost", "Total", "Vendor" };
            int[] xPos = { 40, 120, 280, 340, 400, 480, 560 };
            for (int i = 0; i < headers.Length; i++)
                g.DrawString(headers[i], b, Brushes.Black, xPos[i], y);
            y += lh;
            g.DrawLine(Pens.Black, 40, y, 700, y);
            y += 3;

            foreach (DataGridViewRow row in dgvBOMItems.Rows)
            {
                if (y > e.PageBounds.Height - 60) { e.HasMorePages = true; return; }
                g.DrawString(row.Cells["colItemCode"].Value?.ToString(), f, Brushes.Black, xPos[0], y);
                g.DrawString((row.Cells["colItemName"].Value?.ToString() ?? "").Length > 15 ? (row.Cells["colItemName"].Value?.ToString() ?? "").Substring(0, 15) + "..." : row.Cells["colItemName"].Value?.ToString(), f, Brushes.Black, xPos[1], y);
                g.DrawString(row.Cells["colBOMQty"].Value?.ToString(), f, Brushes.Black, xPos[2], y);
                g.DrawString(row.Cells["colBalanceQty"].Value?.ToString(), f, Brushes.Black, xPos[3], y);
                g.DrawString(row.Cells["colUnitCost"].Value?.ToString(), f, Brushes.Black, xPos[4], y);
                g.DrawString(row.Cells["colTotalCost"].Value?.ToString(), f, Brushes.Black, xPos[5], y);
                g.DrawString(row.Cells["colVendor"].Value?.ToString(), f, Brushes.Black, xPos[6], y);
                y += lh;
            }

            y += 5;
            g.DrawLine(Pens.Black, 40, y, 700, y);
            y += lh;
            g.DrawString("Total: " + txtTotalAmount.Text + "    Selected: " + txtSelectedItems.Text + "/" + txtTotalItems.Text, b, Brushes.Black, 40, y);
            e.HasMorePages = false;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion
        // colConvertedQty -- colBOMQty
        private void checkAll_CheckedChanged(object sender, EventArgs e)
        {
            if (_isSelectAllChanging) return;
            _isSelectAllChanging = true;

            bool check = checkAll.Checked;
            foreach (DataGridViewRow row in dgvBOMItems.Rows)
            {
                if (row.Cells["colSelect"] is DataGridViewCheckBoxCell chkCell && Convert.ToDouble(row.Cells["colConvertedQty"].Value.ToString()) != Convert.ToDouble(row.Cells["colBOMQty"].Value.ToString()))
                {
                    chkCell.Value = check;
                }
            }

            // Commit immediately so stats / amounts refresh correctly
            dgvBOMItems.EndEdit();
            CalculateTotals();
            UpdateSelectionStats();
            _isSelectAllChanging = false;
        }

        private void UpdateSelectionStats()
        {
            int selectedCount = 0;
            decimal selectedTotal = 0;
            int totalRows = dgvBOMItems.Rows.Count;

            foreach (DataGridViewRow row in dgvBOMItems.Rows)
            {
                if (row.Cells["colSelect"] is DataGridViewCheckBoxCell chkCell &&
                    chkCell.Value != null && Convert.ToBoolean(chkCell.Value))
                {
                    selectedCount++;
                    selectedTotal += GetDecimalValue(row.Cells["colTotalCost"].Value);
                }
            }

            // Update Select All checkbox state
            if (!_isSelectAllChanging)
            {
                _isSelectAllChanging = true;

                if (selectedCount == 0)
                    checkAll.Checked = false;
                else if (selectedCount == totalRows)
                    checkAll.Checked = true;
                else
                    checkAll.CheckState = CheckState.Indeterminate;

                _isSelectAllChanging = false;
            }
        }

        private static string Csv(object value)
        {
            string text = value?.ToString() ?? "";
            if (text.Contains(',') || text.Contains('"') || text.Contains('\n'))
                return "\"" + text.Replace("\"", "\"\"") + "\"";
            return text;
        }
    }
}