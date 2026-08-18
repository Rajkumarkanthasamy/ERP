using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class frmPRtoPOConversion_new : Form
    {
        private PRtoPODAL _dal;
        private string _currentUser;
        private int _selectedPRID = 0;
        private string _selectedPRNumber = "";
        private DataTable _prDetailDt;
        private bool _isSelectAllChanging = false;

        public frmPRtoPOConversion_new()
        {
            InitializeComponent();
            _dal = new PRtoPODAL();
            _currentUser = AppSession.IsAuthenticated ? AppSession.UserName : Environment.UserName;
        }

        private void frmPRtoPOConversion_Load(object sender, EventArgs e)
        {
            LoadApprovedPRs();
            lblCurrentUser.Text = "User: " + _currentUser;
            if (string.IsNullOrWhiteSpace(txtPreparedBy.Text))
                txtPreparedBy.Text = _currentUser;
            dtpPODeliveryDate.Value = DateTime.Now.AddDays(30);
            cmbCurrency.SelectedIndex = 0; // INR
        }

        // ============================================
        // APPROVED PRs GRID
        // ============================================
        private void LoadApprovedPRs()
        {
            DataTable dt = _dal.GetApprovedPRsForPO();
            dgvApprovedPRs.DataSource = dt;
            FormatApprovedPRGrid();
            lblPRCount.Text = "Approved PRs Ready: " + dt.Rows.Count.ToString();
        }

        private void FormatApprovedPRGrid()
        {
            if (dgvApprovedPRs.Columns.Contains("PRID"))
                dgvApprovedPRs.Columns["PRID"].Visible = false;
            if (dgvApprovedPRs.Columns.Contains("PRNumber"))
            {
                dgvApprovedPRs.Columns["PRNumber"].HeaderText = "PR Number";
                dgvApprovedPRs.Columns["PRNumber"].Width = 110;
            }
            if (dgvApprovedPRs.Columns.Contains("ProjectCode"))
            {
                dgvApprovedPRs.Columns["ProjectCode"].HeaderText = "Project";
                dgvApprovedPRs.Columns["ProjectCode"].Width = 90;
            }
            if (dgvApprovedPRs.Columns.Contains("VendorName"))
            {
                dgvApprovedPRs.Columns["VendorName"].HeaderText = "Vendor";
                dgvApprovedPRs.Columns["VendorName"].Width = 160;
            }
            if (dgvApprovedPRs.Columns.Contains("VendorCode"))
                dgvApprovedPRs.Columns["VendorCode"].Visible = false;
            if (dgvApprovedPRs.Columns.Contains("TotalAmount"))
            {
                dgvApprovedPRs.Columns["TotalAmount"].HeaderText = "Amount";
                dgvApprovedPRs.Columns["TotalAmount"].DefaultCellStyle.Format = "N2";
                dgvApprovedPRs.Columns["TotalAmount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvApprovedPRs.Columns["TotalAmount"].Width = 100;
            }
            if (dgvApprovedPRs.Columns.Contains("Status"))
            {
                dgvApprovedPRs.Columns["Status"].HeaderText = "Status";
                dgvApprovedPRs.Columns["Status"].Width = 80;
            }
            if (dgvApprovedPRs.Columns.Contains("RequestedBy"))
            {
                dgvApprovedPRs.Columns["RequestedBy"].HeaderText = "Requested By";
                dgvApprovedPRs.Columns["RequestedBy"].Width = 120;
            }
            if (dgvApprovedPRs.Columns.Contains("RequestDate"))
            {
                dgvApprovedPRs.Columns["RequestDate"].HeaderText = "Req. Date";
                dgvApprovedPRs.Columns["RequestDate"].DefaultCellStyle.Format = "dd-MMM-yyyy";
                dgvApprovedPRs.Columns["RequestDate"].Width = 95;
            }
            if (dgvApprovedPRs.Columns.Contains("ApprovedBy"))
            {
                dgvApprovedPRs.Columns["ApprovedBy"].HeaderText = "Approved By";
                dgvApprovedPRs.Columns["ApprovedBy"].Width = 120;
            }
            if (dgvApprovedPRs.Columns.Contains("ApprovedDate"))
            {
                dgvApprovedPRs.Columns["ApprovedDate"].HeaderText = "Appr. Date";
                dgvApprovedPRs.Columns["ApprovedDate"].DefaultCellStyle.Format = "dd-MMM-yyyy";
                dgvApprovedPRs.Columns["ApprovedDate"].Width = 95;
            }
            if (dgvApprovedPRs.Columns.Contains("Remarks"))
            {
                dgvApprovedPRs.Columns["Remarks"].HeaderText = "Remarks";
                dgvApprovedPRs.Columns["Remarks"].Width = 180;
            }
            if (dgvApprovedPRs.Columns.Contains("IsClubbed"))
                dgvApprovedPRs.Columns["IsClubbed"].Visible = false;
            if (dgvApprovedPRs.Columns.Contains("ClubbedFromPRIDs"))
                dgvApprovedPRs.Columns["ClubbedFromPRIDs"].Visible = false;

            dgvApprovedPRs.ClearSelection();
        }

        private void dgvApprovedPRs_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            _selectedPRID = Convert.ToInt32(dgvApprovedPRs.Rows[e.RowIndex].Cells["PRID"].Value);
            _selectedPRNumber = dgvApprovedPRs.Rows[e.RowIndex].Cells["PRNumber"].Value.ToString();

            lblSelectedPR.Text = "Step 2: Select Line Items to Convert  |  PR: " + _selectedPRNumber;
            LoadPRDetailForPO(_selectedPRNumber);
        }

        // ============================================
        // PR LINES GRID (with checkbox selection)
        // ============================================
        private void LoadPRDetailForPO(string prID)
        {
            _prDetailDt = _dal.GetPRDetailForPO(prID);
            dgvPRLines.DataSource = _prDetailDt;
            AddSelectCheckboxColumn();
            FormatPRLinesGrid();
            UpdateSelectionStats();
            pnlPODetails.Enabled = _prDetailDt.Rows.Count > 0;
            chkSelectAllLines.Checked = false;
        }

        private void AddSelectCheckboxColumn()
        {
            // Remove existing select column if any
            if (dgvPRLines.Columns.Contains("Select"))
                dgvPRLines.Columns.Remove("Select");

            DataGridViewCheckBoxColumn chkCol = new DataGridViewCheckBoxColumn();
            chkCol.Name = "Select";
            chkCol.HeaderText = "Select";
            chkCol.Width = 50;
            chkCol.FalseValue = false;
            chkCol.TrueValue = true;
            chkCol.IndeterminateValue = false;
            chkCol.DataPropertyName = null; // Not bound to DataTable
            dgvPRLines.Columns.Insert(0, chkCol);
        }

        private void FormatPRLinesGrid()
        {
            
            if (dgvPRLines.Columns.Contains("Select"))
            {
                dgvPRLines.Columns["Select"].HeaderText = "✓";
                dgvPRLines.Columns["Select"].Width = 45;
                dgvPRLines.Columns["Select"].ReadOnly = false;
            }
            if (dgvPRLines.Columns.Contains("DetailID"))
                dgvPRLines.Columns["DetailID"].Visible = false;
            if (dgvPRLines.Columns.Contains("PRID"))
                dgvPRLines.Columns["PRID"].Visible = false;
            if (dgvPRLines.Columns.Contains("PRNumber"))
                dgvPRLines.Columns["PRNumber"].Visible = false;
            if (dgvPRLines.Columns.Contains("ProjectCode"))
            {
                dgvPRLines.Columns["ProjectCode"].HeaderText = "Project";
                dgvPRLines.Columns["ProjectCode"].Width = 80;
            }
            if (dgvPRLines.Columns.Contains("ProductNo"))
            {
                dgvPRLines.Columns["ProductNo"].HeaderText = "Product";
                dgvPRLines.Columns["ProductNo"].Width = 80;
            }
            if (dgvPRLines.Columns.Contains("ItemCode"))
            {
                dgvPRLines.Columns["ItemCode"].HeaderText = "Item Code";
                dgvPRLines.Columns["ItemCode"].Width = 90;
            }
            if (dgvPRLines.Columns.Contains("ItemName"))
            {
                dgvPRLines.Columns["ItemName"].HeaderText = "Item Name";
                dgvPRLines.Columns["ItemName"].Width = 150;
            }
            if (dgvPRLines.Columns.Contains("ItemDescription"))
            {
                dgvPRLines.Columns["ItemDescription"].HeaderText = "Description";
                dgvPRLines.Columns["ItemDescription"].Width = 160;
            }
            if (dgvPRLines.Columns.Contains("Specification"))
            {
                dgvPRLines.Columns["Specification"].HeaderText = "Specification";
                dgvPRLines.Columns["Specification"].Width = 130;
            }
            if (dgvPRLines.Columns.Contains("Make"))
            {
                dgvPRLines.Columns["Make"].HeaderText = "Make";
                dgvPRLines.Columns["Make"].Width = 90;
            }
            if (dgvPRLines.Columns.Contains("MfgPartNo"))
            {
                dgvPRLines.Columns["MfgPartNo"].HeaderText = "Mfg Part No";
                dgvPRLines.Columns["MfgPartNo"].Width = 110;
            }
            if (dgvPRLines.Columns.Contains("RequariedQty"))
            {
                dgvPRLines.Columns["RequariedQty"].HeaderText = "Qty";
                dgvPRLines.Columns["RequariedQty"].DefaultCellStyle.Format = "N2";
                dgvPRLines.Columns["RequariedQty"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvPRLines.Columns["RequariedQty"].Width = 60;
            }
            if (dgvPRLines.Columns.Contains("UOM"))
            {
                dgvPRLines.Columns["UOM"].HeaderText = "UOM";
                dgvPRLines.Columns["UOM"].Width = 50;
            }
            if (dgvPRLines.Columns.Contains("UnitPrice"))
            {
                dgvPRLines.Columns["UnitPrice"].HeaderText = "Unit Price";
                dgvPRLines.Columns["UnitPrice"].DefaultCellStyle.Format = "N2";
                dgvPRLines.Columns["UnitPrice"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvPRLines.Columns["UnitPrice"].Width = 90;
            }
            if (dgvPRLines.Columns.Contains("Amount"))
            {
                dgvPRLines.Columns["Amount"].HeaderText = "Amount";
                dgvPRLines.Columns["Amount"].DefaultCellStyle.Format = "N2";
                dgvPRLines.Columns["Amount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvPRLines.Columns["Amount"].Width = 100;
            }
            if (dgvPRLines.Columns.Contains("RemainingQty"))
                dgvPRLines.Columns["RemainingQty"].Visible = false;
            if (dgvPRLines.Columns.Contains("VendorCode"))
            {
                dgvPRLines.Columns["VendorCode"].HeaderText = "Vendor";
                dgvPRLines.Columns["VendorCode"].Width = 80;
            }
            if (dgvPRLines.Columns.Contains("VendorName"))
                dgvPRLines.Columns["VendorName"].Visible = false;
            if (dgvPRLines.Columns.Contains("DrawingNo"))
            {
                dgvPRLines.Columns["DrawingNo"].HeaderText = "Drawing No";
                dgvPRLines.Columns["DrawingNo"].Width = 100;
            }
            if (dgvPRLines.Columns.Contains("Location"))
            {
                dgvPRLines.Columns["Location"].HeaderText = "Location";
                dgvPRLines.Columns["Location"].Width = 80;
            }
            if (dgvPRLines.Columns.Contains("HSNCode"))
            {
                dgvPRLines.Columns["HSNCode"].HeaderText = "HSN";
                dgvPRLines.Columns["HSNCode"].Width = 80;
            }
            if (dgvPRLines.Columns.Contains("BOMCode"))
            {
                dgvPRLines.Columns["BOMCode"].HeaderText = "BOM Code";
                dgvPRLines.Columns["BOMCode"].Width = 90;
            }
            if (dgvPRLines.Columns.Contains("BOMQty"))
            {
                dgvPRLines.Columns["BOMQty"].HeaderText = "BOM Qty";
                dgvPRLines.Columns["BOMQty"].DefaultCellStyle.Format = "N2";
                dgvPRLines.Columns["BOMQty"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvPRLines.Columns["BOMQty"].Width = 70;
            }
            if (dgvPRLines.Columns.Contains("Remarks"))
            {
                dgvPRLines.Columns["Remarks"].HeaderText = "Remarks";
                dgvPRLines.Columns["Remarks"].Width = 120;
            }
            if (dgvPRLines.Columns.Contains("CreatedBy"))
                dgvPRLines.Columns["CreatedBy"].Visible = false;
            if (dgvPRLines.Columns.Contains("CreatedDate"))
                dgvPRLines.Columns["CreatedDate"].Visible = false;

            dgvPRLines.ClearSelection();
        }

        // ============================================
        // CHECKBOX HANDLING
        // ============================================
        private void dgvPRLines_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgvPRLines.CurrentCell is DataGridViewCheckBoxCell)
            {
                dgvPRLines.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        //private void dgvPRLines_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        //{
        //    if (e.RowIndex < 0) return;
        //    if (dgvPRLines.Columns[e.ColumnIndex].Name == "Select")
        //    {
        //        UpdateSelectionStats();
        //    }
        //}

        //private void chkSelectAllLines_CheckedChanged(object sender, EventArgs e)
        //{
        //    if (_isSelectAllChanging) return;
        //    _isSelectAllChanging = true;

        //    bool check = chkSelectAllLines.Checked;
        //    foreach (DataGridViewRow row in dgvPRLines.Rows)
        //    {
        //        if (row.Cells["Select"] is DataGridViewCheckBoxCell chkCell)
        //        {
        //            chkCell.Value = check;
        //        }
        //    }
        //    UpdateSelectionStats();
        //    _isSelectAllChanging = false;
        //}

        //private void UpdateSelectionStats()
        //{
        //    MessageBox.Show("Calling UpdateSelectionStats..!");
        //    int selectedCount = 0;
        //    decimal selectedTotal = 0;
        //    int totalRows = dgvPRLines.Rows.Count;

        //    foreach (DataGridViewRow row in dgvPRLines.Rows)
        //    {
        //        if (row.Cells["Select"] is DataGridViewCheckBoxCell chkCell &&
        //            chkCell.Value != null && (bool)chkCell.Value == true)
        //        {
        //            selectedCount++;
        //            if (row.Cells["Amount"].Value != null && row.Cells["Amount"].Value != DBNull.Value)
        //                selectedTotal += Convert.ToDecimal(row.Cells["Amount"].Value);
        //        }
        //    }

        //    lblLineCount.Text = "Selected: " + selectedCount + " / " + totalRows;
        //    lblLineTotal.Text = "Total: Rs. " + selectedTotal.ToString("N2");

        //    // Update Select All checkbox state
        //    if (!_isSelectAllChanging)
        //    {
        //        if (selectedCount == 0)
        //            chkSelectAllLines.Checked = false;
        //        else if (selectedCount == totalRows)
        //            chkSelectAllLines.Checked = true;
        //        else
        //            chkSelectAllLines.CheckState = CheckState.Indeterminate;
        //    }
        //}

        //private bool _isSelectAllChanging = false;

        // ========== SELECT ALL CHECKBOX ==========
        private void chkSelectAllLines_CheckedChanged(object sender, EventArgs e)
        {
            if (_isSelectAllChanging) return;
            _isSelectAllChanging = true;

            bool check = chkSelectAllLines.Checked;
            foreach (DataGridViewRow row in dgvPRLines.Rows)
            {
                if (row.Cells["Select"] is DataGridViewCheckBoxCell chkCell)
                {
                    chkCell.Value = check;
                }
            }

            // Commit immediately so stats read correct values
            dgvPRLines.EndEdit();
           
            UpdateSelectionStats();
            _isSelectAllChanging = false;
        }

        // ========== WHEN USER CLICKS A ROW CHECKBOX ==========
        

        private void dgvPRLines_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            // Only react to the "Select" column
            
            if (e.ColumnIndex >= 0 && dgvPRLines.Columns[e.ColumnIndex].Name == "Select")
            {
               // MessageBox.Show("First - Calling UpdateSelectionStats..!", e.ColumnIndex.ToString());

                UpdateSelectionStats();

            }
        }

        // ========== STATS UPDATE ==========
        private void UpdateSelectionStats_old()
        {
            // REMOVE THIS BEFORE DEPLOYING:
             MessageBox.Show("Calling UpdateSelectionStats..!");

            int selectedCount = 0;
            decimal selectedTotal = 0;
            int totalRows = dgvPRLines.Rows.Count;

            foreach (DataGridViewRow row in dgvPRLines.Rows)
            {
                if (row.Cells["Select"] is DataGridViewCheckBoxCell chkCell &&
                    chkCell.Value != null && Convert.ToBoolean(chkCell.Value))
                {
                    selectedCount++;

                    if (row.Cells["Amount"].Value != null && row.Cells["Amount"].Value != DBNull.Value)
                        selectedTotal += Convert.ToDecimal(row.Cells["Amount"].Value);
                }
            }

            lblLineCount.Text = $"Selected: {selectedCount} / {totalRows}";
            lblLineTotal.Text = $"Total: Rs. {selectedTotal:N2}";

            // Update Select All checkbox state (Indeterminate when partially selected)
            if (!_isSelectAllChanging)
            {
                if (selectedCount == 0)
                    chkSelectAllLines.Checked = false;
                else if (selectedCount == totalRows)
                    chkSelectAllLines.Checked = true;
                else
                    chkSelectAllLines.CheckState = CheckState.Indeterminate;
            }
        }

        private void UpdateSelectionStats()
        {
            int selectedCount = 0;
            decimal selectedTotal = 0;
            int totalRows = dgvPRLines.Rows.Count;

            foreach (DataGridViewRow row in dgvPRLines.Rows)
            {
                if (row.Cells["Select"] is DataGridViewCheckBoxCell chkCell &&
                    chkCell.Value != null && Convert.ToBoolean(chkCell.Value))
                {
                    selectedCount++;

                    if (row.Cells["Amount"].Value != null && row.Cells["Amount"].Value != DBNull.Value)
                        selectedTotal += Convert.ToDecimal(row.Cells["Amount"].Value);
                }
            }

            lblLineCount.Text = $"Selected: {selectedCount} / {totalRows}";
            lblLineTotal.Text = $"Total: Rs. {selectedTotal:N2}";

            // Update Select All checkbox state
            if (!_isSelectAllChanging)
            {
                _isSelectAllChanging = true;  // <-- ADD THIS

                if (selectedCount == 0)
                    chkSelectAllLines.Checked = false;
                else if (selectedCount == totalRows)
                    chkSelectAllLines.Checked = true;
                else
                    chkSelectAllLines.CheckState = CheckState.Indeterminate;

                _isSelectAllChanging = false; // <-- ADD THIS
            }
        }

        // ============================================
        // GENERATE PO (only selected lines)
        // ============================================
        private void btnGeneratePO_Click(object sender, EventArgs e)
        {
            if (_selectedPRNumber == "")
            {
                MessageBox.Show("Please select a PR first.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Collect only selected rows
            List<DataGridViewRow> selectedRows = new List<DataGridViewRow>();
            foreach (DataGridViewRow row in dgvPRLines.Rows)
            {
                if (row.Cells["Select"] is DataGridViewCheckBoxCell chkCell &&
                    chkCell.Value != null && (bool)chkCell.Value == true)
                {
                    selectedRows.Add(row);
                }
            }

            if (selectedRows.Count == 0)
            {
                MessageBox.Show("Please select at least one line item to convert.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(txtPreparedBy.Text.Trim()))
            {
                MessageBox.Show("Please enter Prepared By.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPreparedBy.Focus();
                return;
            }

            if (MessageBox.Show("Generate PO from " + selectedRows.Count + " selected line(s) of PR " + _selectedPRNumber + "?",
                "Confirm PO Generation", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                return;

            List<PRtoPOModel> poList = new List<PRtoPOModel>();
            string poDate = DateTime.Now.ToString("dd-MMM-yyyy");
            string deliveryDate = dtpPODeliveryDate.Value.ToString("dd-MMM-yyyy");
            string currency = cmbCurrency.SelectedItem != null ? cmbCurrency.SelectedItem.ToString() : "INR";

            foreach (DataGridViewRow row in selectedRows)
            {
                PRtoPOModel po = new PRtoPOModel
                {
                    DBOMNo = _selectedPRNumber,
                    PRID = _selectedPRID,
                    DetailID = Convert.ToInt32(row.Cells["DetailID"].Value),
                    ProjectCode = row.Cells["ProjectCode"].Value.ToString(),
                    ProductNo = dgvPRLines.Columns.Contains("ProductNo") && row.Cells["ProductNo"].Value != null
                        ? row.Cells["ProductNo"].Value.ToString() : "",
                    VendorCode = row.Cells["VendorCode"].Value.ToString(),
                    ItemCode = row.Cells["ItemCode"].Value.ToString(),
                    //ItemName = row.Cells["ItemName"].Value != DBNull.Value ? row.Cells["ItemName"].Value.ToString() : "",
                    ItemDescription = row.Cells["ItemDescription"].Value != DBNull.Value ? row.Cells["ItemDescription"].Value.ToString() : "",
                    Specification = row.Cells["Specification"].Value != DBNull.Value ? row.Cells["Specification"].Value.ToString() : "",
                    Make = row.Cells["Make"].Value != DBNull.Value ? row.Cells["Make"].Value.ToString() : "",
                    MfgPartNo = row.Cells["MfgPartNo"].Value != DBNull.Value ? row.Cells["MfgPartNo"].Value.ToString() : "",
                    RequariedQty = row.Cells["RequariedQty"].Value != DBNull.Value ? Convert.ToSingle(row.Cells["RequariedQty"].Value) : 0,
                    UOM = row.Cells["UOM"].Value != DBNull.Value ? row.Cells["UOM"].Value.ToString() : "",
                    UnitPrice = row.Cells["UnitPrice"].Value != DBNull.Value ? Convert.ToSingle(row.Cells["UnitPrice"].Value) : 0,
                    Amount = row.Cells["Amount"].Value != DBNull.Value ? Convert.ToSingle(row.Cells["Amount"].Value) : 0,
                    RemainingQty = row.Cells["RemainingQty"].Value != DBNull.Value ? Convert.ToSingle(row.Cells["RemainingQty"].Value) : 0,
                    BOMQty = row.Cells["BOMQty"].Value != DBNull.Value ? Convert.ToSingle(row.Cells["BOMQty"].Value) : 0,
                    PreparedBy = txtPreparedBy.Text.Trim(),
                    AuthoriedBy = txtAuthoriedBy.Text.Trim(),
                    POGeneratedBy = _currentUser,
                    POGeneratedDate = poDate,
                    POPreparedDate = poDate,
                    PODeliveryDate = deliveryDate,
                    ProjectStatus = "PO Generated",
                    Remarks = txtPORemarks.Text.Trim(),
                    POApproved = false,
                    WithoutBOM = false,
                    Currency = currency,
                    FXRate = currency == "INR" ? 1 : 0
                };

                poList.Add(po);
            }

            int count = _dal.BulkConvertPRtoPO(poList);

            if (count > 0)
            {
                foreach (var po in poList)
                {
                    _dal.UpdatePRLineStatusToConverted(po.DetailID);
                }

                string headerStatus = _dal.RefreshPRConversionStatus(_selectedPRNumber);
                MessageBox.Show(
                    count + " PO line(s) generated successfully!\n\nPO Reference: " + _selectedPRNumber +
                    "\nPR Status: " + headerStatus,
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                ClearForm();
                LoadApprovedPRs();
            }
            else
            {
                MessageBox.Show("Failed to generate PO.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ============================================
        // EXISTING POs
        // ============================================
        private void btnViewExistingPO_Click(object sender, EventArgs e)
        {
            if (_selectedPRID == 0 || string.IsNullOrEmpty(_selectedPRNumber))
            {
                MessageBox.Show("Please select a PR first.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataTable dt = _dal.GetPOListFromPR(_selectedPRNumber);
            dgvExistingPOs.DataSource = dt;
            FormatExistingPOGrid();
            lblExistingPOCount.Text = "Existing POs: " + dt.Rows.Count.ToString();
        }

        private void FormatExistingPOGrid()
        {
            if (dgvExistingPOs.Columns.Contains("ID"))
            {
                dgvExistingPOs.Columns["ID"].HeaderText = "PO ID";
                dgvExistingPOs.Columns["ID"].Width = 60;
            }
            if (dgvExistingPOs.Columns.Contains("DBOMNo"))
            {
                dgvExistingPOs.Columns["DBOMNo"].HeaderText = "PR Ref";
                dgvExistingPOs.Columns["DBOMNo"].Width = 100;
            }
            if (dgvExistingPOs.Columns.Contains("ProjectCode"))
            {
                dgvExistingPOs.Columns["ProjectCode"].HeaderText = "Project";
                dgvExistingPOs.Columns["ProjectCode"].Width = 80;
            }
            if (dgvExistingPOs.Columns.Contains("VendorCode"))
            {
                dgvExistingPOs.Columns["VendorCode"].HeaderText = "Vendor";
                dgvExistingPOs.Columns["VendorCode"].Width = 80;
            }
            if (dgvExistingPOs.Columns.Contains("ItemCode"))
            {
                dgvExistingPOs.Columns["ItemCode"].HeaderText = "Item Code";
                dgvExistingPOs.Columns["ItemCode"].Width = 90;
            }
            if (dgvExistingPOs.Columns.Contains("UOM"))
            {
                dgvExistingPOs.Columns["UOM"].Width = 50;
            }
            if (dgvExistingPOs.Columns.Contains("RequariedQty"))
            {
                dgvExistingPOs.Columns["RequariedQty"].HeaderText = "Qty";
                dgvExistingPOs.Columns["RequariedQty"].DefaultCellStyle.Format = "N2";
                dgvExistingPOs.Columns["RequariedQty"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvExistingPOs.Columns["RequariedQty"].Width = 60;
            }
            if (dgvExistingPOs.Columns.Contains("UnitPrice"))
            {
                dgvExistingPOs.Columns["UnitPrice"].HeaderText = "Unit Price";
                dgvExistingPOs.Columns["UnitPrice"].DefaultCellStyle.Format = "N2";
                dgvExistingPOs.Columns["UnitPrice"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvExistingPOs.Columns["UnitPrice"].Width = 90;
            }
            if (dgvExistingPOs.Columns.Contains("Amount"))
            {
                dgvExistingPOs.Columns["Amount"].DefaultCellStyle.Format = "N2";
                dgvExistingPOs.Columns["Amount"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                dgvExistingPOs.Columns["Amount"].Width = 90;
            }
            if (dgvExistingPOs.Columns.Contains("RemainingQty"))
                dgvExistingPOs.Columns["RemainingQty"].Visible = false;
            if (dgvExistingPOs.Columns.Contains("POGeneratedBy"))
            {
                dgvExistingPOs.Columns["POGeneratedBy"].HeaderText = "Generated By";
                dgvExistingPOs.Columns["POGeneratedBy"].Width = 110;
            }
            if (dgvExistingPOs.Columns.Contains("POGeneratedDate"))
            {
                dgvExistingPOs.Columns["POGeneratedDate"].HeaderText = "Gen. Date";
                dgvExistingPOs.Columns["POGeneratedDate"].Width = 90;
            }
            if (dgvExistingPOs.Columns.Contains("POApproved"))
            {
                dgvExistingPOs.Columns["POApproved"].HeaderText = "Approved";
                dgvExistingPOs.Columns["POApproved"].Width = 65;
            }
            if (dgvExistingPOs.Columns.Contains("POPreparedDate"))
                dgvExistingPOs.Columns["POPreparedDate"].Visible = false;
            if (dgvExistingPOs.Columns.Contains("PODeliveryDate"))
            {
                dgvExistingPOs.Columns["PODeliveryDate"].HeaderText = "Del. Date";
                dgvExistingPOs.Columns["PODeliveryDate"].Width = 90;
            }
            if (dgvExistingPOs.Columns.Contains("Remarks"))
            {
                dgvExistingPOs.Columns["Remarks"].Width = 130;
            }

            dgvExistingPOs.ClearSelection();
        }

        // ============================================
        // UTILITY
        // ============================================
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadApprovedPRs();
            ClearForm();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ClearForm()
        {
            _selectedPRID = 0;
            _selectedPRNumber = "";
            _prDetailDt = null;
            lblSelectedPR.Text = "Step 2: Select Line Items to Convert";
            lblLineTotal.Text = "Total: Rs. 0.00";
            lblLineCount.Text = "Selected: 0 / 0";
            txtPreparedBy.Clear();
            txtAuthoriedBy.Clear();
            txtPORemarks.Clear();
            dgvPRLines.DataSource = null;
            dgvExistingPOs.DataSource = null;
            lblExistingPOCount.Text = "Existing POs: 0";
            pnlPODetails.Enabled = false;
            chkSelectAllLines.Checked = false;
            dgvApprovedPRs.ClearSelection();
        }

        private void dgvPRLines_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        private void dgvApprovedPRs_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        private void frmPRtoPOConversion_new_Load(object sender, EventArgs e)
        {

        }
    }
}