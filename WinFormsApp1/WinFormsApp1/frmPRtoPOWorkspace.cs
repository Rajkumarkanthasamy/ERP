using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace WinFormsApp1
{
    /// <summary>
    /// Phase 2: Drag approved PRs onto the PO canvas to build purchase orders.
    /// </summary>
    public partial class frmPRtoPOWorkspace : Form
    {
        private readonly PRtoPODAL _dal = new PRtoPODAL();
        private readonly string _currentUser;
        private readonly List<WorkspacePR> _droppedPRs = new List<WorkspacePR>();
        private Point _dragStart;
        private bool _dragging;

        private class WorkspacePR
        {
            public int PRID { get; set; }
            public string PRNumber { get; set; } = "";
            public string VendorCode { get; set; } = "";
            public string VendorName { get; set; } = "";
            public string ProjectCode { get; set; } = "";
            public decimal TotalAmount { get; set; }
            public DataTable Lines { get; set; } = new DataTable();
        }

        public frmPRtoPOWorkspace()
        {
            InitializeComponent();
            _currentUser = AppSession.IsAuthenticated ? AppSession.UserName : Environment.UserName;
        }

        private void frmPRtoPOWorkspace_Load(object sender, EventArgs e)
        {
            lblUser.Text = AppSession.DisplayLabel;
            txtPreparedBy.Text = _currentUser;
            dtpDelivery.Value = DateTime.Now.AddDays(30);
            cmbCurrency.Items.Clear();
            cmbCurrency.Items.AddRange(new object[] { "INR", "USD", "EUR" });
            cmbCurrency.SelectedIndex = 0;
            LoadSourcePRs();
            UpdateDropSummary();
        }

        private void LoadSourcePRs()
        {
            DataTable dt = _dal.GetApprovedPRsForPO();
            dgvSourcePRs.DataSource = dt;
            dgvSourcePRs.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvSourcePRs.RowHeadersVisible = false;
            dgvSourcePRs.AllowUserToAddRows = false;
            dgvSourcePRs.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvSourcePRs.ReadOnly = true;
            dgvSourcePRs.MultiSelect = true;

            if (dgvSourcePRs.Columns.Contains("PRID"))
                dgvSourcePRs.Columns["PRID"].Visible = false;
            if (dgvSourcePRs.Columns.Contains("IsClubbed"))
                dgvSourcePRs.Columns["IsClubbed"].Visible = false;
            if (dgvSourcePRs.Columns.Contains("ClubbedFromPRIDs"))
                dgvSourcePRs.Columns["ClubbedFromPRIDs"].Visible = false;

            lblSourceCount.Text = "Approved PRs ready: " + dt.Rows.Count;
        }

        private void dgvSourcePRs_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            var hit = dgvSourcePRs.HitTest(e.X, e.Y);
            if (hit.RowIndex < 0) return;

            if (!dgvSourcePRs.Rows[hit.RowIndex].Selected)
            {
                dgvSourcePRs.ClearSelection();
                dgvSourcePRs.Rows[hit.RowIndex].Selected = true;
            }

            _dragStart = e.Location;
            _dragging = true;
        }

        private void dgvSourcePRs_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_dragging || e.Button != MouseButtons.Left) return;
            if (Math.Abs(e.X - _dragStart.X) < 5 && Math.Abs(e.Y - _dragStart.Y) < 5)
                return;

            List<string> prNumbers = new List<string>();
            foreach (DataGridViewRow row in dgvSourcePRs.SelectedRows)
            {
                if (row.DataBoundItem is DataRowView view)
                    prNumbers.Add(view.Row["PRNumber"].ToString() ?? "");
            }

            if (prNumbers.Count == 0) return;
            _dragging = false;
            dgvSourcePRs.DoDragDrop(string.Join("|", prNumbers), DragDropEffects.Copy);
        }

        private void dgvSourcePRs_MouseUp(object sender, MouseEventArgs e) => _dragging = false;

        private void panelDropZone_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data != null && e.Data.GetDataPresent(DataFormats.StringFormat))
            {
                e.Effect = DragDropEffects.Copy;
                panelDropZone.BackColor = Color.FromArgb(220, 235, 250);
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void panelDropZone_DragLeave(object sender, EventArgs e)
        {
            panelDropZone.BackColor = Color.FromArgb(248, 250, 252);
        }

        private void panelDropZone_DragDrop(object sender, DragEventArgs e)
        {
            panelDropZone.BackColor = Color.FromArgb(248, 250, 252);
            string payload = e.Data?.GetData(DataFormats.StringFormat)?.ToString() ?? "";
            if (string.IsNullOrWhiteSpace(payload)) return;

            foreach (string prNumber in payload.Split('|', StringSplitOptions.RemoveEmptyEntries))
                AddPRToCanvas(prNumber.Trim());

            RefreshDroppedGrid();
            UpdateDropSummary();
        }

        private void AddPRToCanvas(string prNumber)
        {
            if (_droppedPRs.Any(p => p.PRNumber == prNumber))
                return;

            DataTable source = dgvSourcePRs.DataSource as DataTable;
            if (source == null) return;

            DataRow? header = null;
            foreach (DataRow r in source.Rows)
            {
                if (string.Equals(r["PRNumber"]?.ToString(), prNumber, StringComparison.OrdinalIgnoreCase))
                {
                    header = r;
                    break;
                }
            }
            if (header == null) return;

            DataTable lines = _dal.GetPRDetailForPO(prNumber);
            _droppedPRs.Add(new WorkspacePR
            {
                PRID = Convert.ToInt32(header["PRID"]),
                PRNumber = prNumber,
                VendorCode = header["VendorCode"]?.ToString() ?? "",
                VendorName = header["VendorName"]?.ToString() ?? "",
                ProjectCode = header["ProjectCode"]?.ToString() ?? "",
                TotalAmount = Convert.ToDecimal(header["TotalAmount"]),
                Lines = lines
            });
        }

        private void RefreshDroppedGrid()
        {
            dgvDropped.Rows.Clear();
            foreach (var pr in _droppedPRs)
            {
                int lineCount = pr.Lines?.Rows.Count ?? 0;
                dgvDropped.Rows.Add(pr.PRNumber, pr.VendorName, pr.ProjectCode,
                    pr.TotalAmount.ToString("N2"), lineCount.ToString());
            }

            // Preview all lines
            dgvLines.Rows.Clear();
            foreach (var pr in _droppedPRs)
            {
                foreach (DataRow line in pr.Lines.Rows)
                {
                    dgvLines.Rows.Add(
                        pr.PRNumber,
                        line["ItemCode"]?.ToString(),
                        line["ItemDescription"]?.ToString(),
                        line["RequariedQty"]?.ToString(),
                        Convert.ToDecimal(line["UnitPrice"]).ToString("N2"),
                        Convert.ToDecimal(line["Amount"]).ToString("N2"),
                        line["VendorCode"]?.ToString());
                }
            }
        }

        private void UpdateDropSummary()
        {
            decimal total = _droppedPRs.Sum(p => p.TotalAmount);
            int lines = _droppedPRs.Sum(p => p.Lines?.Rows.Count ?? 0);
            int vendors = _droppedPRs.Select(p => p.VendorCode).Distinct(StringComparer.OrdinalIgnoreCase).Count();

            lblDropHint.Text = _droppedPRs.Count == 0
                ? "Drop approved PRs here to build PO(s)"
                : $"{_droppedPRs.Count} PR(s) · {lines} line(s) · {vendors} vendor(s)";

            lblDropTotal.Text = "Canvas total: ₹ " + total.ToString("N2");
            lblDropTotal.ForeColor = total > AppConfig.PR_AMOUNT_LIMIT
                ? Color.Firebrick
                : Color.FromArgb(40, 120, 90);

            btnGeneratePO.Enabled = _droppedPRs.Count > 0;
            btnClearCanvas.Enabled = _droppedPRs.Count > 0;
        }

        private void btnAddSelected_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dgvSourcePRs.SelectedRows)
            {
                if (row.DataBoundItem is DataRowView view)
                    AddPRToCanvas(view.Row["PRNumber"].ToString() ?? "");
            }
            RefreshDroppedGrid();
            UpdateDropSummary();
        }

        private void btnRemoveSelected_Click(object sender, EventArgs e)
        {
            HashSet<string> remove = new HashSet<string>();
            foreach (DataGridViewRow row in dgvDropped.SelectedRows)
            {
                string? pr = row.Cells[0].Value?.ToString();
                if (!string.IsNullOrEmpty(pr))
                    remove.Add(pr);
            }

            _droppedPRs.RemoveAll(p => remove.Contains(p.PRNumber));
            RefreshDroppedGrid();
            UpdateDropSummary();
        }

        private void btnClearCanvas_Click(object sender, EventArgs e)
        {
            _droppedPRs.Clear();
            RefreshDroppedGrid();
            UpdateDropSummary();
        }

        private void btnGeneratePO_Click(object sender, EventArgs e)
        {
            if (_droppedPRs.Count == 0)
            {
                MessageBox.Show("Drop at least one PR onto the canvas.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPreparedBy.Text))
            {
                MessageBox.Show("Enter Prepared By.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Group by vendor — one PO batch per vendor
            var byVendor = _droppedPRs.GroupBy(p => p.VendorCode, StringComparer.OrdinalIgnoreCase);
            int totalLines = 0;
            List<string> results = new List<string>();

            foreach (var vendorGroup in byVendor)
            {
                List<PRtoPOModel> poList = new List<PRtoPOModel>();
                foreach (var pr in vendorGroup)
                {
                    foreach (DataRow line in pr.Lines.Rows)
                    {
                        poList.Add(new PRtoPOModel
                        {
                            DetailID = Convert.ToInt32(line["DetailID"]),
                            PRID = pr.PRID,
                            DBOMNo = pr.PRNumber,
                            ProjectCode = line["ProjectCode"]?.ToString() ?? pr.ProjectCode,
                            VendorCode = pr.VendorCode,
                            ItemCode = line["ItemCode"]?.ToString() ?? "",
                            UOM = line["UOM"]?.ToString() ?? "Nos",
                            RequariedQty = Convert.ToSingle(line["RequariedQty"]),
                            UnitPrice = Convert.ToSingle(line["UnitPrice"]),
                            Amount = Convert.ToSingle(line["Amount"]),
                            RemainingQty = Convert.ToSingle(line["RequariedQty"]),
                            BOMQty = line["BOMQty"] != DBNull.Value ? Convert.ToSingle(line["BOMQty"]) : 0,
                            PreparedBy = txtPreparedBy.Text.Trim(),
                            AuthoriedBy = txtAuthorisedBy.Text.Trim(),
                            POGeneratedBy = _currentUser,
                            POGeneratedDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            POPreparedDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                            PODeliveryDate = dtpDelivery.Value.ToString("yyyy-MM-dd"),
                            Currency = cmbCurrency.SelectedItem?.ToString() ?? "INR",
                            FXRate = 1,
                            Remarks = txtRemarks.Text.Trim(),
                            POApproved = false,
                            WithoutBOM = false,
                            ProjectStatus = "WIP",
                            BOMProjectList = pr.ProjectCode
                        });
                    }
                }

                int count = _dal.BulkConvertPRtoPO(poList);
                if (count > 0)
                {
                    foreach (var po in poList)
                        _dal.UpdatePRLineStatusToConverted(po.DetailID);

                    foreach (var pr in vendorGroup.Select(v => v.PRNumber).Distinct())
                        _dal.RefreshPRConversionStatus(pr);

                    totalLines += count;
                    results.Add($"{vendorGroup.Key}: {count} PO line(s) from {vendorGroup.Count()} PR(s)");
                }
            }

            if (totalLines > 0)
            {
                MessageBox.Show(
                    "PO generation complete.\n\n" + string.Join("\n", results) +
                    "\n\nNext step: open PO Approval.",
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                _droppedPRs.Clear();
                RefreshDroppedGrid();
                UpdateDropSummary();
                LoadSourcePRs();
            }
            else
            {
                MessageBox.Show("No PO lines were created.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnOpenPOApproval_Click(object sender, EventArgs e)
        {
            new frmPOApproval().Show(this);
        }

        private void btnRefresh_Click(object sender, EventArgs e) => LoadSourcePRs();
        private void btnClose_Click(object sender, EventArgs e) => Close();
    }
}
