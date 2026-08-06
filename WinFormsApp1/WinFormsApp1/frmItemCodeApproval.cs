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
    public partial class frmItemCodeApproval : Form
    {
        private ItemCodeCreationDAL _dal;
        private List<ItemCodeRequest> _allRequests;
        private List<ItemCodeRequest> _filteredRequests;
        private ItemCodeRequest _selectedRequest;

        private string _currentUserRole = AppSession.IsAuthenticated ? AppSession.Role : "Material Manager";
        private string _currentUserName = AppSession.IsAuthenticated ? AppSession.UserName : Environment.UserName;

        private readonly List<string> _categories = new List<string>
        {
            "All", "Capex / Fixed Asset", "Maintenance", "Consumables", "Services",
            "Packing Items", "Kanban", "Electrical & Electronics", "Hydraulics",
            "Mechanical", "Test Lab", "Consumables Non-Inventory",
            "Project-Specific (Design)", "Machining Items", "Mechanical Brought-Out Parts", "R & D"
        };

        private readonly List<string> _departments = new List<string>
        {
            "All", "Purchase", "Maintenance", "Production", "R & D", "Test Lab",
            "Design", "Machining", "Hydraulics", "Electrical", "Stores",
            "Quality", "Project", "Packing"
        };

        private readonly List<string> _statusOptions = new List<string>
        {
            "All", "Pending", "Approved", "Rejected", "On Hold"
        };

        private readonly List<string> _actions = new List<string>
        {
            "Approve", "Reject", "On Hold"
        };

        public frmItemCodeApproval()
        {
            InitializeComponent();
            _dal = new ItemCodeCreationDAL();
        }

        private void frmItemCodeApproval_Load(object sender, EventArgs e)
        {
            txtUserRole.Text = _currentUserRole;
            txtApproverName.Text = _currentUserName;
            dtpApprovalDate.Value = DateTime.Now;

            LoadFilterDropdowns();
            LoadActionDropdown();
            LoadDataFromDatabase();
            RefreshGrid();
            UpdateStatistics();
        }

        private void LoadFilterDropdowns()
        {
            cmbFilterCategory.Items.Clear();
            foreach (var cat in _categories) cmbFilterCategory.Items.Add(cat);
            cmbFilterCategory.SelectedIndex = 0;

            cmbFilterDepartment.Items.Clear();
            foreach (var dept in _departments) cmbFilterDepartment.Items.Add(dept);
            cmbFilterDepartment.SelectedIndex = 0;

            cmbFilterStatus.Items.Clear();
            foreach (var status in _statusOptions) cmbFilterStatus.Items.Add(status);
            cmbFilterStatus.SelectedIndex = 0;
        }

        private void LoadActionDropdown()
        {
            cmbAction.Items.Clear();
            foreach (var action in _actions) cmbAction.Items.Add(action);
            cmbAction.SelectedIndex = 0;
        }

        private void LoadDataFromDatabase()
        {
            try
            {
                _allRequests = _dal.GetPendingRequests();
                if (_allRequests == null || _allRequests.Count == 0)
                    LoadSampleData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database connection failed. Loading sample data.\n" + ex.Message,
                    "Database Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                LoadSampleData();
            }
            _filteredRequests = new List<ItemCodeRequest>(_allRequests);
        }

        private void LoadSampleData()
        {
            _allRequests = new List<ItemCodeRequest>
            {
                new ItemCodeRequest { RequestID = "ICCRF-2026-001", RequestDate = DateTime.Now.AddDays(-2), Requestor = "Theertha Rao", Department = "Purchase", ItemCategory = "Capex / Fixed Asset", ItemDescription = "CNC MACHINE VMC 850", TechnicalSpec = "Vertical Machining Center", UnitOfMeasure = "Nos", DrawingReference = "DRW-CNC-001", CriticalityLevel = "A - High Critical", HSNCode = "8457.10.00", ItemCode = "FA-XX-1001", AuthorizedCreator = "Purchase Department", ApprovalAuthority = "Material Manager", ApprovalStatus = "Pending" },
                new ItemCodeRequest { RequestID = "ICCRF-2026-002", RequestDate = DateTime.Now.AddDays(-1), Requestor = "Kumar Ravi", Department = "Machining", ItemCategory = "Machining Items", ItemDescription = "CARBIDE END MILL 12MM", TechnicalSpec = "Solid Carbide, 4 Flute", UnitOfMeasure = "Nos", DrawingReference = "DRW-TOOL-045", CriticalityLevel = "B - Medium Critical", HSNCode = "8207.70.90", ItemCode = "MAC-XX-1002", AuthorizedCreator = "Mr. Santosh", ApprovalAuthority = "Operations Head", ApprovalStatus = "Pending" },
                new ItemCodeRequest { RequestID = "ICCRF-2026-003", RequestDate = DateTime.Now.AddDays(-3), Requestor = "Ravi Naiker", Department = "Electrical", ItemCategory = "Electrical & Electronics", ItemDescription = "PLC MODULE S7-1200", TechnicalSpec = "Siemens S7-1200", UnitOfMeasure = "Nos", DrawingReference = "DRW-ELE-112", CriticalityLevel = "A - High Critical", HSNCode = "8537.10.90", ItemCode = "ELE-XX-1003", AuthorizedCreator = "Mr. Ravi Naiker", ApprovalAuthority = "Operations Head", ApprovalStatus = "Pending" },
                new ItemCodeRequest { RequestID = "ICCRF-2026-004", RequestDate = DateTime.Now.AddDays(-5), Requestor = "Sanjay PB", Department = "Hydraulics", ItemCategory = "Hydraulics", ItemDescription = "HYDRAULIC CYLINDER 100X200", TechnicalSpec = "Bore 100mm, Stroke 200mm", UnitOfMeasure = "Nos", DrawingReference = "DRW-HYD-078", CriticalityLevel = "A - High Critical", HSNCode = "8412.21.00", ItemCode = "HYD-XX-1004", AuthorizedCreator = "Mr. Santosh", ApprovalAuthority = "Operations Head", ApprovalStatus = "Approved", StoreLocation = "Store-A", BinNumber = "H-12-05" },
                new ItemCodeRequest { RequestID = "ICCRF-2026-005", RequestDate = DateTime.Now.AddDays(-1), Requestor = "Veena", Department = "Purchase", ItemCategory = "Consumables", ItemDescription = "CUTTING OIL SYNTHETIC 20L", TechnicalSpec = "Synthetic cutting fluid", UnitOfMeasure = "Drum", DrawingReference = "", CriticalityLevel = "C - Low Critical", HSNCode = "3403.19.00", ItemCode = "CON-XX-1005", AuthorizedCreator = "Purchase Department", ApprovalAuthority = "Material Manager", ApprovalStatus = "Pending" },
                new ItemCodeRequest { RequestID = "ICCRF-2026-006", RequestDate = DateTime.Now.AddDays(-4), Requestor = "Manoj", Department = "Test Lab", ItemCategory = "Test Lab", ItemDescription = "DIGITAL MICROMETER 0-25MM", TechnicalSpec = "0-25mm range", UnitOfMeasure = "Nos", DrawingReference = "DRW-TEL-023", CriticalityLevel = "B - Medium Critical", HSNCode = "9017.30.00", ItemCode = "TEL-XX-1006", AuthorizedCreator = "Mr. Manoj", ApprovalAuthority = "Operations Head", ApprovalStatus = "Rejected", ApprovalRemarks = "Duplicate item already exists." },
                new ItemCodeRequest { RequestID = "ICCRF-2026-007", RequestDate = DateTime.Now, Requestor = "Sachin Ravipati", Department = "Design", ItemCategory = "Project-Specific (Design)", ItemDescription = "CUSTOM BRACKET ASSEMBLY", TechnicalSpec = "SS304, Custom fabricated", UnitOfMeasure = "Nos", DrawingReference = "DRW-PRJ-089", CriticalityLevel = "A - High Critical", HSNCode = "7326.90.90", ItemCode = "PRJ-XX-1007", AuthorizedCreator = "Mr. Kumar Ravi", ApprovalAuthority = "Operations Head", ApprovalStatus = "On Hold", ApprovalRemarks = "Awaiting design finalization." }
            };
        }

        private void RefreshGrid()
        {
            dgvPendingRequests.Rows.Clear();
            foreach (var request in _filteredRequests)
            {
                int idx = dgvPendingRequests.Rows.Add(false, request.RequestID, request.RequestDate.ToShortDateString(),
                    request.Requestor, request.Department, request.ItemCategory, request.ItemDescription,
                    request.ItemCode, request.AuthorizedCreator, request.ApprovalStatus);

                Color c = Color.White;
                if (request.ApprovalStatus == "Approved") c = Color.LightGreen;
                else if (request.ApprovalStatus == "Rejected") c = Color.LightCoral;
                else if (request.ApprovalStatus == "On Hold") c = Color.LightYellow;
                dgvPendingRequests.Rows[idx].DefaultCellStyle.BackColor = c;
            }
        }

        private void UpdateStatistics()
        {
            txtTotalPending.Text = _allRequests.Count(r => r.ApprovalStatus == "Pending").ToString();
            txtTotalApproved.Text = _allRequests.Count(r => r.ApprovalStatus == "Approved").ToString();
            txtTotalRejected.Text = _allRequests.Count(r => r.ApprovalStatus == "Rejected").ToString();
        }

        private void dgvPendingRequests_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == 0)
            {
                DataGridViewCheckBoxCell cell = (DataGridViewCheckBoxCell)dgvPendingRequests.Rows[e.RowIndex].Cells[0];
                cell.Value = !(cell.Value == null ? false : (bool)cell.Value);
            }
        }

        private void dgvPendingRequests_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvPendingRequests.SelectedRows.Count > 0)
            {
                int idx = dgvPendingRequests.SelectedRows[0].Index;
                if (idx >= 0 && idx < _filteredRequests.Count)
                {
                    _selectedRequest = _filteredRequests[idx];
                    DisplaySelectedRequest();
                }
            }
        }

        private void DisplaySelectedRequest()
        {
            if (_selectedRequest == null) return;
            txtSelectedRequestID.Text = _selectedRequest.RequestID;
            txtSelectedItemCode.Text = _selectedRequest.ItemCode;
            txtSelectedDescription.Text = _selectedRequest.ItemDescription;
            txtSelectedCategory.Text = _selectedRequest.ItemCategory;
            txtSelectedRequestor.Text = _selectedRequest.Requestor;
            txtSelectedDepartment.Text = _selectedRequest.Department;
            txtSelectedTechnicalSpec.Text = _selectedRequest.TechnicalSpec;
            txtSelectedUOM.Text = _selectedRequest.UnitOfMeasure;
            txtSelectedCriticality.Text = _selectedRequest.CriticalityLevel;
            txtSelectedHSN.Text = _selectedRequest.HSNCode;
            txtApprovalRemarks.Text = _selectedRequest.ApprovalRemarks ?? "";
        }

        private void btnFilter_Click(object sender, EventArgs e) { ApplyFilters(); }
        private void btnClearFilter_Click(object sender, EventArgs e)
        {
            cmbFilterCategory.SelectedIndex = 0;
            cmbFilterDepartment.SelectedIndex = 0;
            cmbFilterStatus.SelectedIndex = 0;
            ApplyFilters();
        }

        private void ApplyFilters()
        {
            string cat = cmbFilterCategory.SelectedItem?.ToString();
            string dept = cmbFilterDepartment.SelectedItem?.ToString();
            string stat = cmbFilterStatus.SelectedItem?.ToString();

            try
            {
                _filteredRequests = _dal.GetPendingRequests(null,
                    cat == "All" ? null : cat,
                    dept == "All" ? null : dept,
                    stat == "All" ? null : stat);
            }
            catch
            {
                _filteredRequests = _allRequests.Where(r =>
                    (cat == "All" || r.ItemCategory == cat) &&
                    (dept == "All" || r.Department == dept) &&
                    (stat == "All" || r.ApprovalStatus == stat)).ToList();
            }
            RefreshGrid();
        }

        private void btnApprove_Click(object sender, EventArgs e) { ProcessApproval("Approved"); }
        private void btnReject_Click(object sender, EventArgs e) { ProcessApproval("Rejected"); }
        private void btnHold_Click(object sender, EventArgs e) { ProcessApproval("On Hold"); }

        private void ProcessApproval(string status)
        {
            if (_selectedRequest == null)
            {
                MessageBox.Show("Please select a request from the grid.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!ValidateApproverAuthority(_selectedRequest))
            {
                MessageBox.Show("You do not have authority to approve " + _selectedRequest.ItemCategory + " items.\nRequired: " + _selectedRequest.ApprovalAuthority,
                    "Authorization Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (MessageBox.Show("Confirm " + status + "?\n\nRequest: " + _selectedRequest.RequestID + "\nItem: " + _selectedRequest.ItemCode,
                "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            try
            {
                bool ok = _dal.UpdateApprovalStatus(_selectedRequest.RequestID, status,
                    txtApprovalRemarks.Text, txtApproverName.Text, txtStoreLocation.Text, txtBinNumber.Text);

                if (ok)
                {
                    _selectedRequest.ApprovalStatus = status;
                    _selectedRequest.ApprovedBy = txtApproverName.Text;
                    _selectedRequest.ApprovalDate = dtpApprovalDate.Value;

                    if (status == "Approved")
                    {
                        _selectedRequest.IsActive = true;
                        string pushResult = _dal.PushToItemMaster(_selectedRequest.RequestID, txtApproverName.Text);
                        MessageBox.Show("APPROVED and pushed to ItemMaster.\n" + pushResult, "Success",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Request " + status + ".", "Done",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                    RefreshGrid();
                    UpdateStatistics();
                    ClearSelectedRequest();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "Database Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool ValidateApproverAuthority(ItemCodeRequest request)
        {
            if (_currentUserRole == "Operations Head")
                return request.ApprovalAuthority == "Operations Head";
            else if (_currentUserRole == "Material Manager")
                return request.ApprovalAuthority == "Material Manager";
            return false;
        }

        private void btnBulkApprove_Click(object sender, EventArgs e) { ProcessBulkAction("Approved"); }
        private void btnBulkReject_Click(object sender, EventArgs e) { ProcessBulkAction("Rejected"); }
        private void btnBulkHold_Click(object sender, EventArgs e) { ProcessBulkAction("On Hold"); }

        private void ProcessBulkAction(string status)
        {
            List<ItemCodeRequest> selected = new List<ItemCodeRequest>();
            for (int i = 0; i < dgvPendingRequests.Rows.Count; i++)
            {
                DataGridViewCheckBoxCell cell = (DataGridViewCheckBoxCell)dgvPendingRequests.Rows[i].Cells[0];
                if (cell.Value != null && (bool)cell.Value)
                    selected.Add(_filteredRequests[i]);
            }

            if (selected.Count == 0)
            {
                MessageBox.Show("Select at least one request using checkboxes.", "No Selection",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            List<ItemCodeRequest> valid = new List<ItemCodeRequest>();
            foreach (var req in selected)
                if (ValidateApproverAuthority(req)) valid.Add(req);

            if (valid.Count == 0)
            {
                MessageBox.Show("No authorization for selected items.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (MessageBox.Show("Process " + valid.Count + " request(s) as " + status + "?",
                "Confirm", MessageBoxButtons.YesNo) != DialogResult.Yes)
                return;

            int success = 0;
            foreach (var req in valid)
            {
                try
                {
                    if (_dal.UpdateApprovalStatus(req.RequestID, status, txtApprovalRemarks.Text, txtApproverName.Text))
                    {
                        req.ApprovalStatus = status;
                        req.ApprovedBy = txtApproverName.Text;
                        if (status == "Approved")
                            _dal.PushToItemMaster(req.RequestID, txtApproverName.Text);
                        success++;
                    }
                }
                catch { }
            }

            MessageBox.Show(success + " processed.", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
            RefreshGrid();
            UpdateStatistics();
            ClearSelectedRequest();
        }

        private void ClearSelectedRequest()
        {
            _selectedRequest = null;
            txtSelectedRequestID.Clear();
            txtSelectedItemCode.Clear();
            txtSelectedDescription.Clear();
            txtSelectedCategory.Clear();
            txtSelectedRequestor.Clear();
            txtSelectedDepartment.Clear();
            txtSelectedTechnicalSpec.Clear();
            txtSelectedUOM.Clear();
            txtSelectedCriticality.Clear();
            txtSelectedHSN.Clear();
            txtApprovalRemarks.Clear();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadDataFromDatabase();
            ApplyFilters();
            MessageBox.Show("Refreshed.", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "CSV files (*.csv)|*.csv";
            dlg.FileName = "ItemCodeApproval_" + DateTime.Now.ToString("yyyyMMdd") + ".csv";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    using (StreamWriter w = new StreamWriter(dlg.FileName))
                    {
                        w.WriteLine("RequestID,RequestDate,Requestor,Department,ItemCategory,ItemDescription,ItemCode,AuthorizedCreator,ApprovalAuthority,ApprovalStatus,ApprovedBy,ApprovalDate,ApprovalRemarks");
                        //foreach (var r in _filteredRequests)
                        //    w.WriteLine(r.RequestID + "," + r.RequestDate.ToString("yyyy-MM-dd") + "," + r.Requestor + "," + r.Department + ","" + r.ItemCategory + "","" + r.ItemDescription + ""," + r.ItemCode + "," + r.AuthorizedCreator + "," + r.ApprovalAuthority + "," + r.ApprovalStatus + "," + r.ApprovedBy + "," + (r.ApprovalDate?.ToString("yyyy-MM-dd") ?? "") + ","" + r.ApprovalRemarks + """);
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

            g.DrawString("ITEM CODE APPROVAL REPORT", t, Brushes.Black, 40, y);
            y += lh * 2;
            g.DrawString("By: " + txtApproverName.Text + "    Role: " + _currentUserRole + "    Records: " + _filteredRequests.Count, f, Brushes.Black, 40, y);
            y += lh * 2;

            g.DrawLine(Pens.Black, 40, y, 700, y);
            y += 3;
            g.DrawString("Req.ID", b, Brushes.Black, 40, y);
            g.DrawString("Date", b, Brushes.Black, 120, y);
            g.DrawString("Requestor", b, Brushes.Black, 190, y);
            g.DrawString("Category", b, Brushes.Black, 290, y);
            g.DrawString("Item Code", b, Brushes.Black, 420, y);
            g.DrawString("Status", b, Brushes.Black, 520, y);
            y += lh;
            g.DrawLine(Pens.Black, 40, y, 700, y);
            y += 3;

            foreach (var r in _filteredRequests)
            {
                if (y > e.PageBounds.Height - 60) { e.HasMorePages = true; return; }
                g.DrawString(r.RequestID, f, Brushes.Black, 40, y);
                g.DrawString(r.RequestDate.ToString("dd-MMM"), f, Brushes.Black, 120, y);
                g.DrawString(r.Requestor, f, Brushes.Black, 190, y);
                g.DrawString(r.ItemCategory.Length > 15 ? r.ItemCategory.Substring(0, 15) + "..." : r.ItemCategory, f, Brushes.Black, 290, y);
                g.DrawString(r.ItemCode, f, Brushes.Black, 420, y);
                Brush br = Brushes.Black;
                if (r.ApprovalStatus == "Approved") br = Brushes.Green;
                else if (r.ApprovalStatus == "Rejected") br = Brushes.Red;
                else if (r.ApprovalStatus == "On Hold") br = Brushes.Orange;
                g.DrawString(r.ApprovalStatus, f, br, 520, y);
                y += lh;
            }
            y += 5;
            g.DrawLine(Pens.Black, 40, y, 700, y);
            y += lh;
            g.DrawString("Pending: " + txtTotalPending.Text + "    Approved: " + txtTotalApproved.Text + "    Rejected: " + txtTotalRejected.Text, b, Brushes.Black, 40, y);
            e.HasMorePages = false;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}