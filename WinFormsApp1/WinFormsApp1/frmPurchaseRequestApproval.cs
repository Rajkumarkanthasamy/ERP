using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WinFormsApp1;

namespace WinFormsApp1
{
    public partial class frmPurchaseRequestApproval : Form
    {
        private PurchaseRequestApprovalDAL _dal;
        private string _currentUser;
        private string _userRole;
        private string _selectedPRID = "";

        public frmPurchaseRequestApproval()
        {
            InitializeComponent();
            _dal = new PurchaseRequestApprovalDAL();
            _currentUser = AppSession.IsAuthenticated ? AppSession.UserName : Environment.UserName;
            _userRole = AppSession.IsAuthenticated ? AppSession.Role : "Material Manager";
        }

        private void frmPurchaseRequestApproval_Load(object sender, EventArgs e)
        {
            // Allow bulk checkbox selection
            dgvPRList.ReadOnly = false;
            if (dgvPRList.Columns.Contains("Select"))
                dgvPRList.Columns["Select"].ReadOnly = false;
            foreach (DataGridViewColumn col in dgvPRList.Columns)
            {
                if (col.Name != "Select")
                    col.ReadOnly = true;
            }

            LoadFilters();
            LoadStatistics();
            LoadPRGrid();
            lblCurrentUser.Text = "User: " + _currentUser + " | Role: " + _userRole;
            EnsureConvertToPOButton();
        }

        private void EnsureConvertToPOButton()
        {
            if (Controls.Find("btnConvertToPO", true).Length > 0)
                return;

            Button btn = new Button
            {
                Name = "btnConvertToPO",
                Text = "Convert to PO",
                Size = new Size(130, 32),
                BackColor = Color.FromArgb(20, 90, 140),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.Location = new Point(Math.Max(20, ClientSize.Width - 160), 12);
            btn.Click += (s, e) =>
            {
                frmPRtoPOConversion_new poForm = new frmPRtoPOConversion_new();
                poForm.ShowDialog(this);
                LoadPRGrid();
                LoadStatistics();
            };
            Controls.Add(btn);
            btn.BringToFront();

            if (Controls.Find("btnCollaborate", true).Length == 0)
            {
                Button btnCollab = new Button
                {
                    Name = "btnCollaborate",
                    Text = "Comments / Files",
                    Size = new Size(130, 32),
                    Location = new Point(Math.Max(20, ClientSize.Width - 300), 12),
                    Anchor = AnchorStyles.Top | AnchorStyles.Right
                };
                btnCollab.Click += (s, e) =>
                {
                    if (string.IsNullOrEmpty(_selectedPRID))
                    {
                        MessageBox.Show("Select a PR first.", "Collaboration", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    new frmEntityCollaboration("PR", _selectedPRID).ShowDialog(this);
                };
                Controls.Add(btnCollab);
                btnCollab.BringToFront();
            }

            if (Controls.Find("btnPriceVariance", true).Length == 0)
            {
                Button btnVar = new Button
                {
                    Name = "btnPriceVariance",
                    Text = "Price Variance",
                    Size = new Size(120, 32),
                    Location = new Point(Math.Max(20, ClientSize.Width - 430), 12),
                    Anchor = AnchorStyles.Top | AnchorStyles.Right
                };
                btnVar.Click += (s, e) =>
                {
                    if (string.IsNullOrEmpty(_selectedPRID))
                    {
                        MessageBox.Show("Select a PR first.", "Price Variance", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return;
                    }
                    new frmPriceVariance(_selectedPRID).ShowDialog(this);
                };
                Controls.Add(btnVar);
                btnVar.BringToFront();
            }
        }

        private void LoadFilters()
        {
            DataTable dtProjects = _dal.GetProjects();
            cmbProject.Items.Clear();
            cmbProject.Items.Add("All");
            foreach (DataRow row in dtProjects.Rows)
            {
                cmbProject.Items.Add(row["ProjectCode"].ToString());
            }
            cmbProject.SelectedIndex = 0;

            DataTable dtVendors = _dal.GetVendors();
            cmbVendor.Items.Clear();
            cmbVendor.Items.Add("All");
            foreach (DataRow row in dtVendors.Rows)
            {
                cmbVendor.Items.Add(row["VendorName"].ToString());
            }
            cmbVendor.SelectedIndex = 0;
        }

        private void LoadStatistics()
        {
            DataTable dtStats = _dal.GetStatistics();
            if (dtStats.Rows.Count > 0)
            {
                DataRow row = dtStats.Rows[0];
                lblPendingCount.Text = row["PendingCount"] != DBNull.Value ? row["PendingCount"].ToString() : "0";
                lblApprovedCount.Text = row["ApprovedCount"] != DBNull.Value ? row["ApprovedCount"].ToString() : "0";
                lblRejectedCount.Text = row["RejectedCount"] != DBNull.Value ? row["RejectedCount"].ToString() : "0";
                lblOnHoldCount.Text = row["OnHoldCount"] != DBNull.Value ? row["OnHoldCount"].ToString() : "0";
                lblTotalAmount.Text = "Rs. " + (row["TotalAmount"] != DBNull.Value ? Convert.ToDecimal(row["TotalAmount"]).ToString("N0") : "0");
            }
        }

        private void LoadPRGrid()
        {
            string status = cmbStatus.SelectedItem != null ? cmbStatus.SelectedItem.ToString() : "All";
            string project = cmbProject.SelectedItem != null ? cmbProject.SelectedItem.ToString() : "All";
            string vendor = cmbVendor.SelectedItem != null ? cmbVendor.SelectedItem.ToString() : "All";
            string search = txtSearchPR.Text.Trim();
           // MessageBox.Show(status, "Status");
            DataTable dt = _dal.GetPurchaseRequests(status, project, vendor, search);
            dgvPRList.DataSource = dt;

            if (dgvPRList.Columns.Contains("TotalAmount"))
                dgvPRList.Columns["TotalAmount"].DefaultCellStyle.Format = "N2";
            if (dgvPRList.Columns.Contains("RequestDate"))
                dgvPRList.Columns["RequestDate"].DefaultCellStyle.Format = "dd-MMM-yyyy";

            foreach (DataGridViewRow row in dgvPRList.Rows)
            {
                string statusVal = row.Cells["Status"].Value != null ? row.Cells["Status"].Value.ToString() : "";
                switch (statusVal)
                {
                    case "Approved":
                        row.DefaultCellStyle.BackColor = Color.FromArgb(232, 245, 233);
                        break;
                    case "Rejected":
                        row.DefaultCellStyle.BackColor = Color.FromArgb(255, 235, 238);
                        break;
                    case "On Hold":
                        row.DefaultCellStyle.BackColor = Color.FromArgb(255, 243, 224);
                        break;
                    default:
                        row.DefaultCellStyle.BackColor = Color.White;
                        break;
                }
            }
        }

        private void dgvPRList_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            //_selectedPRID = Convert.ToInt32(dgvPRList.Rows[e.RowIndex].Cells["PRNumber"].Value);
            _selectedPRID = dgvPRList.Rows[e.RowIndex].Cells["PRNumber"].Value.ToString();
           string _selectedPRNumber = dgvPRList.Rows[e.RowIndex].Cells["PRNumber"].Value.ToString();
            dgvPRList.Rows[e.RowIndex].Cells["Select"].Value = true; //Select
            LoadPRDetails(_selectedPRNumber);
        }

        private void LoadPRDetails(string prID)
        {
            PurchaseRequestModel pr = _dal.GetPRByID(prID);
            if (pr == null) return;

            pnlDetails.Visible = true;
            lblDetailPRNumber.Text = pr.PRNumber;
            lblDetailProject.Text = pr.ProjectCode;
            lblDetailVendor.Text = pr.VendorName;
            lblDetailRequestor.Text = pr.RequestedBy;
            lblDetailAmount.Text = "Rs. " + pr.TotalAmount.ToString("N2");
            lblDetailStatus.Text = pr.Status;

            switch (pr.Status)
            {
                case "Approved":
                    lblDetailStatus.BackColor = Color.FromArgb(76, 175, 80);
                    lblDetailStatus.ForeColor = Color.White;
                    break;
                case "Rejected":
                    lblDetailStatus.BackColor = Color.FromArgb(244, 67, 54);
                    lblDetailStatus.ForeColor = Color.White;
                    break;
                case "On Hold":
                    lblDetailStatus.BackColor = Color.FromArgb(255, 152, 0);
                    lblDetailStatus.ForeColor = Color.White;
                    break;
                default:
                    lblDetailStatus.BackColor = Color.FromArgb(255, 243, 224);
                    lblDetailStatus.ForeColor = Color.FromArgb(230, 81, 0);
                    break;
            }

            dgvLineItems.Rows.Clear();
            foreach (var item in pr.LineItems)
            {
                dgvLineItems.Rows.Add(item.ItemCode, item.ItemDescription, item.Quantity, item.UOM, item.UnitCost, item.TotalCost);
            }

            bool isPending = pr.Status == "Pending";
            btnApprove.Enabled = isPending;
            btnReject.Enabled = isPending;
            btnHold.Enabled = isPending;
            cmbApprovalAction.Enabled = isPending;
            txtApprovalRemarks.Enabled = isPending;
            btnSubmitApproval.Enabled = isPending;
        }

        private void btnApprove_Click(object sender, EventArgs e)
        {
            if (_selectedPRID == "")
            {
                MessageBox.Show("Please select a PR to approve.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (MessageBox.Show("Are you sure you want to APPROVE this PR?", "Confirm Approval",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                string remarks = txtApprovalRemarks.Text.Trim();
                if (_dal.ApprovePR(_selectedPRID, _currentUser, remarks))
                {
                    MessageBox.Show("PR approved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    RefreshAll();
                }
                else
                {
                    MessageBox.Show("Failed to approve PR. It may have already been processed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnReject_Click(object sender, EventArgs e)
        {
            if (_selectedPRID == "")
            {
                MessageBox.Show("Please select a PR to reject.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string reason = txtApprovalRemarks.Text.Trim();
            if (string.IsNullOrEmpty(reason))
            {
                MessageBox.Show("Please enter a rejection reason in the Remarks field.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (MessageBox.Show("Are you sure you want to REJECT this PR?", "Confirm Rejection",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                if (_dal.RejectPR(_selectedPRID, _currentUser, reason))
                {
                    MessageBox.Show("PR rejected successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    RefreshAll();
                }
                else
                {
                    MessageBox.Show("Failed to reject PR.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnHold_Click(object sender, EventArgs e)
        {
            if (_selectedPRID == "")
            {
                MessageBox.Show("Please select a PR to put on hold.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // If already on hold, release it back to Pending
            if (string.Equals(lblDetailStatus.Text, PRStatus.OnHold, StringComparison.OrdinalIgnoreCase))
            {
                if (MessageBox.Show("Release this PR from Hold back to Pending?", "Confirm Release",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    if (_dal.ReleaseHoldPR(_selectedPRID, _currentUser, txtApprovalRemarks.Text.Trim()))
                    {
                        MessageBox.Show("PR released from hold.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        RefreshAll();
                    }
                    else
                    {
                        MessageBox.Show("Failed to release hold.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                return;
            }

            string reason = txtApprovalRemarks.Text.Trim();
            if (string.IsNullOrEmpty(reason))
            {
                MessageBox.Show("Please enter a hold reason in the Remarks field.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (MessageBox.Show("Are you sure you want to put this PR ON HOLD?", "Confirm Hold",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                if (_dal.HoldPR(_selectedPRID, _currentUser, reason))
                {
                    MessageBox.Show("PR put on hold successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    RefreshAll();
                }
                else
                {
                    MessageBox.Show("Failed to hold PR.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnSubmitApproval_Click(object sender, EventArgs e)
        {
            if (_selectedPRID == "")
            {
                MessageBox.Show("Please select a PR first.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string action = cmbApprovalAction.SelectedItem != null ? cmbApprovalAction.SelectedItem.ToString() : "Approve";
            string remarks = txtApprovalRemarks.Text.Trim();
            switch (action)
            {
                case "Approve":
                    btnApprove_Click(sender, e);
                    break;
                case "Reject":
                    if (string.IsNullOrEmpty(remarks))
                    {
                        MessageBox.Show("Remarks are required for rejection.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    btnReject_Click(sender, e);
                    break;
                case "On Hold":
                    if (string.IsNullOrEmpty(remarks))
                    {
                        MessageBox.Show("Remarks are required for On Hold.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    btnHold_Click(sender, e);
                    break;
            }
        }

        private void btnBulkApprove_Click(object sender, EventArgs e)
        {
            List<int> selectedIDs = GetSelectedPRIDs();
            if (selectedIDs.Count == 0)
            {
                MessageBox.Show("Please select at least one PR using the checkboxes.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (MessageBox.Show("Approve " + selectedIDs.Count + " selected PR(s)?", "Confirm Bulk Approval",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                string remarks = txtApprovalRemarks.Text.Trim();
                int count = _dal.BulkApprove(selectedIDs, _currentUser, remarks);
                MessageBox.Show(count + " PR(s) approved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                RefreshAll();
            }
        }

        private void btnBulkReject_Click(object sender, EventArgs e)
        {
            List<int> selectedIDs = GetSelectedPRIDs();
            if (selectedIDs.Count == 0)
            {
                MessageBox.Show("Please select at least one PR using the checkboxes.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string reason = txtApprovalRemarks.Text.Trim();
            if (string.IsNullOrEmpty(reason))
            {
                MessageBox.Show("Please enter a rejection reason in the Remarks field.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (MessageBox.Show("Reject " + selectedIDs.Count + " selected PR(s)?", "Confirm Bulk Rejection",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                int count = _dal.BulkReject(selectedIDs, _currentUser, reason);
                MessageBox.Show(count + " PR(s) rejected successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                RefreshAll();
            }
        }

        private List<int> GetSelectedPRIDs()
        {
            List<int> ids = new List<int>();
            foreach (DataGridViewRow row in dgvPRList.Rows)
            {
                DataGridViewCheckBoxCell chk = row.Cells["Select"] as DataGridViewCheckBoxCell;
                if (chk != null && Convert.ToBoolean(chk.Value))
                {
                    ids.Add(Convert.ToInt32(row.Cells["PRID"].Value));
                }
            }
            return ids;
        }

        private void btnApplyFilter_Click(object sender, EventArgs e)
        {
            LoadPRGrid();
        }

        private void btnClearFilter_Click(object sender, EventArgs e)
        {
            cmbStatus.SelectedIndex = 0;
            cmbProject.SelectedIndex = 0;
            cmbVendor.SelectedIndex = 0;
            txtSearchPR.Clear();
            LoadPRGrid();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            RefreshAll();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            ExportToExcel();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (_selectedPRID == "")
            {
                MessageBox.Show("Please select a PR to print.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            MessageBox.Show("Print preview for PR " + lblDetailPRNumber.Text, "Print", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void RefreshAll()
        {
            LoadStatistics();
            LoadPRGrid();
            pnlDetails.Visible = false;
            _selectedPRID = "";
            txtApprovalRemarks.Clear();
        }

        private void ExportToExcel()
        {
            try
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";
                sfd.FileName = "PR_Approval_List_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".csv";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    // Header
                    foreach (DataGridViewColumn col in dgvPRList.Columns)
                    {
                        if (col.Name != "Select")
                        {
                            sb.Append(col.HeaderText + ",");
                        }
                    }
                    sb.AppendLine();
                    // Data rows
                    foreach (DataGridViewRow row in dgvPRList.Rows)
                    {
                        foreach (DataGridViewColumn col in dgvPRList.Columns)
                        {
                            if (col.Name != "Select")
                            {
                                string val = row.Cells[col.Name].Value != null ? row.Cells[col.Name].Value.ToString() : "";
                                val = val.Replace(",", ";");
                                sb.Append(""" + val + """);
                            }
                        }
                        sb.AppendLine();
                    }
                    System.IO.File.WriteAllText(sfd.FileName, sb.ToString());
                    MessageBox.Show("Exported successfully to: " + sfd.FileName, "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error exporting: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}