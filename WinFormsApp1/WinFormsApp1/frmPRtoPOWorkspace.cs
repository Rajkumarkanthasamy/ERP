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
        private FormWindowState _lastWindowState = FormWindowState.Normal;

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
            FitToWorkingArea();
            lblUser.Text = AppSession.DisplayLabel;
            txtPreparedBy.Text = _currentUser;
            dtpDelivery.Value = DateTime.Now.AddDays(30);
            cmbCurrency.Items.Clear();
            cmbCurrency.Items.AddRange(new object[] { "INR", "USD", "EUR" });
            cmbCurrency.SelectedIndex = 0;
            LoadSourcePRs();
            UpdateDropSummary();
        }

        private void frmPRtoPOWorkspace_Shown(object sender, EventArgs e)
        {
            // Set splitter ratios after the form has a real size (avoids crash / bad layout)
            ApplyResponsiveSplitters();
            ConfigureGridFill();
        }

        private void frmPRtoPOWorkspace_ResizeEnd(object sender, EventArgs e)
        {
            ApplyResponsiveSplitters();
        }

        private void frmPRtoPOWorkspace_SizeChanged(object sender, EventArgs e)
        {
            // Rebalance when maximizing / restoring (ResizeEnd does not fire for that)
            if (WindowState == FormWindowState.Maximized || WindowState == FormWindowState.Normal)
                ApplyResponsiveSplitters();
        }

        /// <summary>
        /// Fit form to the working area so it works on laptop / 1080p / large monitors.
        /// </summary>
        private void FitToWorkingArea()
        {
            Rectangle wa = Screen.FromControl(this).WorkingArea;
            int margin = 24;
            int targetW = Math.Max(MinimumSize.Width, Math.Min(wa.Width - margin, Math.Max(1000, (int)(wa.Width * 0.92))));
            int targetH = Math.Max(MinimumSize.Height, Math.Min(wa.Height - margin, Math.Max(640, (int)(wa.Height * 0.90))));

            // Very small screens: use almost full working area
            if (wa.Width < 1100 || wa.Height < 700)
            {
                targetW = Math.Max(MinimumSize.Width, wa.Width - 16);
                targetH = Math.Max(MinimumSize.Height, wa.Height - 16);
            }

            Width = targetW;
            Height = targetH;
            Left = wa.Left + Math.Max(0, (wa.Width - Width) / 2);
            Top = wa.Top + Math.Max(0, (wa.Height - Height) / 2);
            WindowState = FormWindowState.Normal;
        }

        private void ApplyResponsiveSplitters()
        {
            try
            {
                if (splitMain.Height > 100)
                    splitMain.SplitterDistance = Math.Max(splitMain.Panel1MinSize,
                        Math.Min(splitMain.Height - splitMain.Panel2MinSize - splitMain.SplitterWidth,
                            (int)(splitMain.Height * 0.58)));

                if (splitTop.Width > 100)
                    splitTop.SplitterDistance = Math.Max(splitTop.Panel1MinSize,
                        Math.Min(splitTop.Width - splitTop.Panel2MinSize - splitTop.SplitterWidth,
                            (int)(splitTop.Width * 0.48)));

                if (splitBottom.Width > 100)
                    splitBottom.SplitterDistance = Math.Max(splitBottom.Panel1MinSize,
                        Math.Min(splitBottom.Width - splitBottom.Panel2MinSize - splitBottom.SplitterWidth,
                            (int)(splitBottom.Width * 0.68)));
            }
            catch
            {
                // Ignore if control not ready yet
            }
        }

        private void ConfigureGridFill()
        {
            dgvSourcePRs.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvDropped.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvLines.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
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

            // Phase 3: price variance advisory before convert
            PriceIntelligenceDAL priceDal = new PriceIntelligenceDAL();
            int alertCount = 0;
            foreach (var pr in _droppedPRs)
            {
                DataTable variance = priceDal.GetPriceVarianceForPR(pr.PRNumber);
                alertCount += CountAlerts(variance);
            }

            if (alertCount > 0)
            {
                var answer = MessageBox.Show(
                    $"{alertCount} price variance alert/watch flag(s) found on dropped PRs.\n\nOpen Price Variance report before generating PO?",
                    "Price Variance", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (answer == DialogResult.Cancel) return;
                if (answer == DialogResult.Yes)
                {
                    new frmPriceVariance(_droppedPRs[0].PRNumber).ShowDialog(this);
                    if (MessageBox.Show("Continue generating PO?", "Confirm",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                        return;
                }
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
                            ProductNo = line.Table.Columns.Contains("ProductNo")
                                ? (line["ProductNo"]?.ToString() ?? "")
                                : "",
                            VendorCode = pr.VendorCode,
                            ItemCode = line["ItemCode"]?.ToString() ?? "",
                            UOM = line["UOM"]?.ToString() ?? "Nos",
                            RequariedQty = Convert.ToSingle(line["RequariedQty"]),
                            UnitPrice = Convert.ToSingle(line["UnitPrice"]),
                            Amount = Convert.ToSingle(line["Amount"]),
                            RemainingQty = Convert.ToSingle(line["RequariedQty"]),
                            BOMQty = line.Table.Columns.Contains("BOMQty") && line["BOMQty"] != DBNull.Value
                                ? Convert.ToSingle(line["BOMQty"]) : 0,
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

                // BulkConvert aggregates duplicate ItemCodes into one PurchaseOrder line
                // and writes project/product splits into PurchaseOrderBOM.
                int count = _dal.BulkConvertPRtoPO(poList);
                if (count > 0)
                {
                    foreach (var po in poList)
                        _dal.UpdatePRLineStatusToConverted(po.DetailID);

                    foreach (var pr in vendorGroup.Select(v => v.PRNumber).Distinct())
                        _dal.RefreshPRConversionStatus(pr);

                    totalLines += count;
                    results.Add($"{vendorGroup.Key}: {count} unique PO item(s) from {poList.Count} PR line(s) / {vendorGroup.Count()} PR(s)");
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

        private static int CountAlerts(DataTable variance)
        {
            int n = 0;
            foreach (DataRow row in variance.Rows)
            {
                string flag = row["Flag"]?.ToString() ?? "";
                if (flag.StartsWith("ALERT") || flag.StartsWith("Watch"))
                    n++;
            }
            return n;
        }
    }
}
