using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class frmPRtoPOConversion : Form
    {
        private PRtoPODAL _dal;
        private string _currentUser = "Mr. Santosh";
        private int _selectedPRID = 0;
        private string _selectedPRNumber = "";
        private DataTable _prDetailDt;

        public frmPRtoPOConversion()
        {
            InitializeComponent();
            _dal = new PRtoPODAL();
        }

        private void frmPRtoPOConversion_Load(object sender, EventArgs e)
        {
            LoadApprovedPRs();
            lblCurrentUser.Text = "User: " + _currentUser;
            dtpPODeliveryDate.Value = DateTime.Now.AddDays(30);
            cmbCurrency.SelectedIndex = 0; // INR
        }

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
                dgvApprovedPRs.Columns["VendorName"].Width = 140;
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
                dgvApprovedPRs.Columns["RequestedBy"].Width = 110;
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
                dgvApprovedPRs.Columns["ApprovedBy"].Width = 110;
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
                dgvApprovedPRs.Columns["Remarks"].Width = 150;
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

            //_selectedPRNumber = dgvApprovedPRs.Rows[e.RowIndex].Cells["PRNumber"].Value.ToString;
            _selectedPRNumber = dgvApprovedPRs.Rows[e.RowIndex].Cells["PRNumber"].Value.ToString();
            //MessageBox.Show(_selectedPRNumber.ToString());
            lblSelectedPR.Text = "Selected PR: " + _selectedPRNumber;
            LoadPRDetailForPO(_selectedPRNumber);
        }

        private void LoadPRDetailForPO(string selectedPRNumber)
        {
            _prDetailDt = _dal.GetPRDetailForPO(selectedPRNumber);
            dgvPRLines.DataSource = _prDetailDt;
            FormatPRLinesGrid();

            // Calculate totals
            decimal totalAmount = 0;
            foreach (DataRow row in _prDetailDt.Rows)
            {
                totalAmount += row["Amount"] != DBNull.Value ? Convert.ToDecimal(row["Amount"]) : 0;
            }
            lblLineTotal.Text = "Line Total: Rs. " + totalAmount.ToString("N2");
            lblLineCount.Text = "Lines: " + _prDetailDt.Rows.Count.ToString();

            pnlPODetails.Enabled = _prDetailDt.Rows.Count > 0;
        }

        private void FormatPRLinesGrid()
        {
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
                dgvPRLines.Columns["ItemName"].Width = 140;
            }
            if (dgvPRLines.Columns.Contains("ItemDescription"))
            {
                dgvPRLines.Columns["ItemDescription"].HeaderText = "Description";
                dgvPRLines.Columns["ItemDescription"].Width = 160;
            }
            if (dgvPRLines.Columns.Contains("Specification"))
            {
                dgvPRLines.Columns["Specification"].HeaderText = "Specification";
                dgvPRLines.Columns["Specification"].Width = 120;
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

        private void btnGeneratePO_Click(object sender, EventArgs e)
        {
            if (_selectedPRNumber == "")
            {
                MessageBox.Show("Please select a PR first.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_prDetailDt == null || _prDetailDt.Rows.Count == 0)
            {
                MessageBox.Show("No line items available for conversion.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(txtPreparedBy.Text.Trim()))
            {
                MessageBox.Show("Please enter Prepared By.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPreparedBy.Focus();
                return;
            }

            List<PRtoPOModel> poList = new List<PRtoPOModel>();
            string poDate = DateTime.Now.ToString("dd-MMM-yyyy");
            string deliveryDate = dtpPODeliveryDate.Value.ToString("dd-MMM-yyyy");
            string currency = cmbCurrency.SelectedItem != null ? cmbCurrency.SelectedItem.ToString() : "INR";

            foreach (DataRow row in _prDetailDt.Rows)
            {
                PRtoPOModel po = new PRtoPOModel
                {
                    DBOMNo = _selectedPRNumber,
                    PRID = _selectedPRID,
                    DetailID = Convert.ToInt32(row["DetailID"]),
                    ProjectCode = row["ProjectCode"].ToString(),
                    ProductNo = row.Table.Columns.Contains("ProductNo") && row["ProductNo"] != DBNull.Value
                        ? row["ProductNo"].ToString() : "",
                    VendorCode = row["VendorCode"].ToString(),
                    ItemCode = row["ItemCode"].ToString(),
                   // ItemName = row["ItemName"] != DBNull.Value ? row["ItemName"].ToString() : "",
                    ItemDescription = row["ItemDescription"] != DBNull.Value ? row["ItemDescription"].ToString() : "",
                    Specification = row["Specification"] != DBNull.Value ? row["Specification"].ToString() : "",
                    Make = row["Make"] != DBNull.Value ? row["Make"].ToString() : "",
                    MfgPartNo = row["MfgPartNo"] != DBNull.Value ? row["MfgPartNo"].ToString() : "",
                    RequariedQty = row["RequariedQty"] != DBNull.Value ? Convert.ToSingle(row["RequariedQty"]) : 0,
                    UOM = row["UOM"] != DBNull.Value ? row["UOM"].ToString() : "",
                    UnitPrice = row["UnitPrice"] != DBNull.Value ? Convert.ToSingle(row["UnitPrice"]) : 0,
                    Amount = row["Amount"] != DBNull.Value ? Convert.ToSingle(row["Amount"]) : 0,
                    RemainingQty = row["RemainingQty"] != DBNull.Value ? Convert.ToSingle(row["RemainingQty"]) : 0,
                    BOMQty = row["BOMQty"] != DBNull.Value ? Convert.ToSingle(row["BOMQty"]) : 0,
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

                MessageBox.Show(count + " PO line(s) generated successfully!\n\nPO Reference: " + _selectedPRNumber,
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                ClearForm();
                LoadApprovedPRs();
            }
            else
            {
                MessageBox.Show("Failed to generate PO.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadApprovedPRs();
            ClearForm();
        }

        private void btnViewExistingPO_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_selectedPRNumber))
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
                dgvExistingPOs.Columns["POGeneratedBy"].Width = 100;
            }
            if (dgvExistingPOs.Columns.Contains("POGeneratedDate"))
            {
                dgvExistingPOs.Columns["POGeneratedDate"].HeaderText = "Gen. Date";
                dgvExistingPOs.Columns["POGeneratedDate"].Width = 90;
            }
            if (dgvExistingPOs.Columns.Contains("POApproved"))
            {
                dgvExistingPOs.Columns["POApproved"].HeaderText = "Approved";
                dgvExistingPOs.Columns["POApproved"].Width = 60;
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
                dgvExistingPOs.Columns["Remarks"].Width = 120;
            }

            dgvExistingPOs.ClearSelection();
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
            lblSelectedPR.Text = "Selected PR: None";
            lblLineTotal.Text = "Line Total: Rs. 0.00";
            lblLineCount.Text = "Lines: 0";
            txtPreparedBy.Clear();
            txtAuthoriedBy.Clear();
            txtPORemarks.Clear();
            dgvPRLines.DataSource = null;
            dgvExistingPOs.DataSource = null;
            lblExistingPOCount.Text = "Existing POs: 0";
            pnlPODetails.Enabled = false;
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
    }
}