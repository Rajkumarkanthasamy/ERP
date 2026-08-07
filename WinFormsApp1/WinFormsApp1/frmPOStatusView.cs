using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class frmPOStatusView : Form
    {
        private readonly DataAccessLayer _dal = new DataAccessLayer();
        private string _selectedPO = "";
        private bool _splitsInitialized;

        private static readonly string[] StatusOptions =
        {
            "All",
            "Created",
            "Pending PM / Dept Approval",
            "Pending MH Approval",
            "Pending Purchase Committee",
            "Pending OM Approval",
            "Pending GM Approval",
            "Ready to Generate",
            "PO Generated",
            "Sent to Vendor",
            "Partially Received",
            "Fully Received",
            "Under Review",
            "Closed",
            "Cancelled",
            "Deleted"
        };

        public frmPOStatusView()
        {
            InitializeComponent();
            _dal.fnGetConnectionString();
        }

        private void frmPOStatusView_Load(object sender, EventArgs e)
        {
            lblUser.Text = AppSession.IsAuthenticated ? AppSession.DisplayLabel : Environment.UserName;
            cmbStatus.Items.Clear();
            cmbStatus.Items.AddRange(StatusOptions);
            cmbStatus.SelectedIndex = 0;
            dtpFrom.Value = DateTime.Today.AddMonths(-6);
            dtpTo.Value = DateTime.Today;
            chkDateFilter.Checked = false;
            dtpFrom.Enabled = false;
            dtpTo.Enabled = false;
            LayoutDetailPanels();
            LoadStatusList();
        }

        private void frmPOStatusView_Resize(object sender, EventArgs e) => LayoutDetailPanels();

        /// <summary>Keep Close button aligned; set split distances once after real size is known.</summary>
        private void LayoutDetailPanels()
        {
            if (btnClose != null && panelInfo != null && panelInfo.ClientSize.Width > 0)
                btnClose.Left = Math.Max(220, panelInfo.ClientSize.Width - btnClose.Width - 12);

            if (_splitsInitialized)
                return;

            if (splitMain != null && splitMain.ClientSize.Height > 200
                && splitDetail != null && splitDetail.ClientSize.Width > 200)
            {
                SafeSetSplitterDistance(splitMain, preferredRatio: 0.48);
                SafeSetSplitterDistance(splitDetail, preferredRatio: 0.55);
                _splitsInitialized = true;
            }
        }

        private static void SafeSetSplitterDistance(SplitContainer split, double preferredRatio)
        {
            if (split == null || split.IsDisposed)
                return;

            int total = split.Orientation == Orientation.Horizontal
                ? split.ClientSize.Height
                : split.ClientSize.Width;

            int available = total - split.SplitterWidth;
            if (available <= split.Panel1MinSize + split.Panel2MinSize)
                return;

            int desired = (int)(available * preferredRatio);
            int min = split.Panel1MinSize;
            int max = available - split.Panel2MinSize;
            if (max < min)
                return;

            int value = Math.Min(max, Math.Max(min, desired));
            if (split.SplitterDistance != value)
            {
                try { split.SplitterDistance = value; }
                catch (InvalidOperationException) { /* ignore during early layout */ }
            }
        }

        private void chkDateFilter_CheckedChanged(object sender, EventArgs e)
        {
            dtpFrom.Enabled = chkDateFilter.Checked;
            dtpTo.Enabled = chkDateFilter.Checked;
        }

        private void btnSearch_Click(object sender, EventArgs e) => LoadStatusList();

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            txtPONumber.Clear();
            txtVendor.Clear();
            txtProject.Clear();
            cmbStatus.SelectedIndex = 0;
            chkDateFilter.Checked = false;
            LoadStatusList();
        }

        private void LoadStatusList()
        {
            try
            {
                string status = cmbStatus.SelectedItem?.ToString() ?? "All";
                DateTime? from = chkDateFilter.Checked ? dtpFrom.Value.Date : null;
                DateTime? to = chkDateFilter.Checked ? dtpTo.Value.Date : null;

                DataSet ds = _dal.fnPOStatusList(status, txtPONumber.Text, txtVendor.Text, txtProject.Text, from, to);
                DataTable dt = (ds != null && ds.Tables.Count > 0) ? ds.Tables[0] : new DataTable();

                dgvPOList.SelectionChanged -= dgvPOList_SelectionChanged;
                dgvPOList.DataSource = dt;
                FormatListGrid();
                ColorStatusRows();
                lblCount.Text = "POs: " + dt.Rows.Count;
                UpdateSummaryChips(dt);
                ClearDetail();
                dgvPOList.SelectionChanged += dgvPOList_SelectionChanged;
            }
            catch (Exception ex)
            {
                dgvPOList.SelectionChanged -= dgvPOList_SelectionChanged;
                dgvPOList.SelectionChanged += dgvPOList_SelectionChanged;
                MessageBox.Show("LoadStatusList: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FormatListGrid()
        {
            dgvPOList.RowHeadersVisible = false;
            dgvPOList.AllowUserToAddRows = false;
            dgvPOList.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPOList.MultiSelect = false;
            dgvPOList.ReadOnly = true;

            string[] hide =
            {
                "PMApproved", "MHApproved", "OMApproved", "POApproved",
                "PMApprovedDate", "MHApprovedDate", "FinalizedDate", "PCAuthoriseDate",
                "OMApprovedDate", "GMApprovedDate", "POSentVendorDate", "AuthoriedBy",
                "RejectReason", "FinalComment", "Remarks", "PMRemarks", "PMMHRemarks",
                "FinalizedRemarks", "PCRemarks", "OMRemarks", "BOMProjects", "TotalQty"
            };
            foreach (string col in hide)
            {
                if (dgvPOList.Columns.Contains(col))
                    dgvPOList.Columns[col].Visible = false;
            }

            SetHeader("PONumber", "PO Number");
            SetHeader("ProjectCode", "Project");
            SetHeader("VendorCode", "Vendor");
            SetHeader("PreparedBy", "Prepared By");
            SetHeader("PreparedDate", "Prepared");
            SetHeader("DeliveryDate", "Delivery");
            SetHeader("TotalAmount", "Amount (incl GST)");
            SetHeader("BaseAmount", "Base Amt");
            SetHeader("GSTAmount", "GST");
            SetHeader("RemainingQty", "Rem Qty");
            SetHeader("CurrentStatus", "Status");
            SetHeader("POGeneratedBy", "Generated By");
            SetHeader("POGeneratedDate", "Generated");
            SetHeader("PurchaseCommitee", "PC");
            SetHeader("FinalizedBy", "Finalized By");
            SetHeader("GMName", "GM");
            SetHeader("OMName", "OM");
            SetHeader("PMName", "PM");
            SetHeader("MHName", "MH");

            // Prefer readable widths instead of equal Fill squeezing every column
            dgvPOList.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            if (dgvPOList.Columns.Contains("CurrentStatus"))
                dgvPOList.Columns["CurrentStatus"].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            if (dgvPOList.Columns.Contains("PONumber"))
                dgvPOList.Columns["PONumber"].MinimumWidth = 110;
        }

        private void SetHeader(string col, string text)
        {
            if (dgvPOList.Columns.Contains(col))
                dgvPOList.Columns[col].HeaderText = text;
        }

        private void ColorStatusRows()
        {
            if (!dgvPOList.Columns.Contains("CurrentStatus"))
                return;

            foreach (DataGridViewRow row in dgvPOList.Rows)
            {
                string status = row.Cells["CurrentStatus"].Value?.ToString() ?? "";
                Color back = status switch
                {
                    "Cancelled" or "Deleted" => Color.FromArgb(255, 230, 230),
                    "Closed" => Color.FromArgb(230, 230, 230),
                    "Under Review" => Color.FromArgb(255, 245, 200),
                    "Fully Received" => Color.FromArgb(220, 245, 220),
                    "Partially Received" => Color.FromArgb(230, 245, 255),
                    "Ready to Generate" => Color.FromArgb(225, 240, 255),
                    "PO Generated" or "Sent to Vendor" => Color.FromArgb(235, 250, 235),
                    "Pending GM Approval" => Color.FromArgb(255, 235, 210),
                    _ when status.StartsWith("Pending", StringComparison.OrdinalIgnoreCase)
                        => Color.FromArgb(255, 248, 230),
                    _ => Color.White
                };
                row.DefaultCellStyle.BackColor = back;
            }
        }

        private void UpdateSummaryChips(DataTable dt)
        {
            flpSummary.Controls.Clear();
            var counts = new System.Collections.Generic.SortedDictionary<string, int>();
            foreach (DataRow row in dt.Rows)
            {
                string st = row["CurrentStatus"]?.ToString() ?? "Unknown";
                if (!counts.ContainsKey(st)) counts[st] = 0;
                counts[st]++;
            }

            foreach (var kv in counts)
            {
                var chip = new Label
                {
                    AutoSize = true,
                    Margin = new Padding(4),
                    Padding = new Padding(8, 4, 8, 4),
                    BackColor = Color.FromArgb(20, 55, 90),
                    ForeColor = Color.White,
                    Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                    Text = kv.Key + ": " + kv.Value
                };
                flpSummary.Controls.Add(chip);
            }
        }

        private void dgvPOList_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvPOList.CurrentRow == null || dgvPOList.CurrentRow.DataBoundItem is not DataRowView view)
            {
                ClearDetail();
                return;
            }

            DataRow row = view.Row;
            _selectedPO = row["PONumber"]?.ToString() ?? "";
            ShowDetail(row);
            LoadLines(_selectedPO);
            BuildTimelineFromPORow(row);
        }

        private void ShowDetail(DataRow row)
        {
            string status = Col(row, "CurrentStatus", "-");
            lblPO.Text = "PO: " + Col(row, "PONumber");
            lblStatus.Text = status;
            lblStatus.ForeColor = StatusColor(status);
            lblProject.Text = "Project: " + Col(row, "ProjectCode");
            lblVendor.Text = "Vendor: " + Col(row, "VendorCode");
            lblPrepared.Text = "Prepared: " + Col(row, "PreparedBy") + " / " + Col(row, "PreparedDate");

            decimal amount = 0, baseAmt = 0, gstAmt = 0;
            if (row.Table.Columns.Contains("TotalAmount") && row["TotalAmount"] != DBNull.Value)
                amount = Convert.ToDecimal(row["TotalAmount"]);
            if (row.Table.Columns.Contains("BaseAmount") && row["BaseAmount"] != DBNull.Value)
                baseAmt = Convert.ToDecimal(row["BaseAmount"]);
            if (row.Table.Columns.Contains("GSTAmount") && row["GSTAmount"] != DBNull.Value)
                gstAmt = Convert.ToDecimal(row["GSTAmount"]);
            lblAmount.Text = "Amount (incl GST): " + amount.ToString("N2") + " " + Col(row, "Currency")
                + "  [Base " + baseAmt.ToString("N2") + " + GST " + gstAmt.ToString("N2") + "]";
            lblDelivery.Text = "Delivery: " + Col(row, "DeliveryDate");

            string finalStatus = Col(row, "FinalStatus");
            lblFinal.Text = string.IsNullOrWhiteSpace(finalStatus)
                ? "Lifecycle: Active"
                : "Lifecycle: " + finalStatus + " — " + Col(row, "FinalComment");

            BuildPipeline(row);
            LayoutDetailPanels();
        }

        private static string Col(DataRow row, string name, string fallback = "")
        {
            if (row == null || !row.Table.Columns.Contains(name) || row[name] == DBNull.Value)
                return fallback;
            return row[name]?.ToString() ?? fallback;
        }

        private static Color StatusColor(string status) => status switch
        {
            "Cancelled" or "Deleted" => Color.Firebrick,
            "Closed" => Color.DimGray,
            "Under Review" => Color.DarkOrange,
            "Fully Received" => Color.ForestGreen,
            "Ready to Generate" or "PO Generated" or "Sent to Vendor" => Color.DarkGreen,
            _ when status.StartsWith("Pending", StringComparison.OrdinalIgnoreCase) => Color.Chocolate,
            _ => Color.FromArgb(20, 55, 90)
        };

        private void BuildPipeline(DataRow row)
        {
            flpPipeline.Controls.Clear();
            AddStep("Created", true, Col(row, "PreparedBy"), Col(row, "PreparedDate"));
            AddStep("PM", IsFilled(Col(row, "PMName")) || ToBool(Col(row, "PMApproved")), Col(row, "PMName"), Col(row, "PMApprovedDate"));
            AddStep("MH", IsFilled(Col(row, "MHName")) || ToBool(Col(row, "MHApproved")), Col(row, "MHName"), Col(row, "MHApprovedDate"));
            AddStep("PC", IsFilled(Col(row, "PurchaseCommitee")), Col(row, "PurchaseCommitee"), Col(row, "PCAuthoriseDate"));
            AddStep("OM", IsFilled(Col(row, "OMName")) || ToBool(Col(row, "OMApproved")), Col(row, "OMName"), Col(row, "OMApprovedDate"));
            AddStep("GM", IsFilled(Col(row, "GMName")), Col(row, "GMName"), Col(row, "GMApprovedDate"));
            AddStep("Approved", ToBool(Col(row, "POApproved")), ToBool(Col(row, "POApproved")) ? "Yes" : "No", null);
            string genBy = Col(row, "POGeneratedBy");
            AddStep("Generated", IsFilled(genBy) && genBy != "Deleted", genBy, Col(row, "POGeneratedDate"));
            AddStep("Vendor", IsFilled(Col(row, "POSenttoVendorBy")), Col(row, "POSenttoVendorBy"), Col(row, "POSentVendorDate"));

            decimal rem = 0, tot = 0;
            if (row.Table.Columns.Contains("RemainingQty") && row["RemainingQty"] != DBNull.Value)
                rem = Convert.ToDecimal(row["RemainingQty"]);
            if (row.Table.Columns.Contains("TotalQty") && row["TotalQty"] != DBNull.Value)
                tot = Convert.ToDecimal(row["TotalQty"]);
            bool received = IsFilled(genBy) && rem < tot;
            AddStep("Receipt", received, rem <= 0 && tot > 0 ? "Full" : (received ? "Partial" : "Pending"), null);
        }

        private void AddStep(string title, bool done, string who, string when)
        {
            var panel = new Panel
            {
                Width = 100,
                Height = 74,
                Margin = new Padding(3, 2, 3, 2),
                Padding = new Padding(6, 4, 6, 4),
                BackColor = done ? Color.FromArgb(40, 120, 90) : Color.FromArgb(210, 215, 220)
            };

            string whoText = string.IsNullOrWhiteSpace(who) || who == "--" ? (done ? "Done" : "—") : who;
            if (whoText.Length > 14) whoText = whoText.Substring(0, 14);

            string whenText = "";
            if (!string.IsNullOrWhiteSpace(when) && when != "--")
                whenText = when.Length > 10 ? when.Substring(0, 10) : when;

            panel.Controls.Add(new Label
            {
                Text = title,
                ForeColor = done ? Color.White : Color.FromArgb(70, 70, 70),
                Font = new Font("Segoe UI Semibold", 8.5F, FontStyle.Bold),
                Location = new Point(6, 4),
                AutoSize = true
            });
            panel.Controls.Add(new Label
            {
                Text = whoText,
                ForeColor = done ? Color.FromArgb(220, 240, 230) : Color.FromArgb(100, 100, 100),
                Font = new Font("Segoe UI", 7.5F),
                Location = new Point(6, 26),
                Size = new Size(88, 18),
                AutoEllipsis = true
            });
            panel.Controls.Add(new Label
            {
                Text = whenText,
                ForeColor = done ? Color.FromArgb(200, 230, 210) : Color.Gray,
                Font = new Font("Segoe UI", 7F),
                Location = new Point(6, 48),
                Size = new Size(88, 16),
                AutoEllipsis = true
            });
            flpPipeline.Controls.Add(panel);
        }

        private static bool IsFilled(object value)
        {
            if (value == null || value == DBNull.Value) return false;
            string s = value.ToString()?.Trim() ?? "";
            return s.Length > 0 && s != "--";
        }

        private static bool ToBool(object value)
        {
            if (value == null || value == DBNull.Value) return false;
            if (value is bool b) return b;
            if (int.TryParse(value.ToString(), out int i)) return i != 0;
            return false;
        }

        private void LoadLines(string poNumber)
        {
            try
            {
                DataSet lines = _dal.fnPOStatusLines(poNumber);
                dgvLines.DataSource = (lines != null && lines.Tables.Count > 0) ? lines.Tables[0] : null;
                dgvLines.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
                dgvLines.RowHeadersVisible = false;
                dgvLines.ReadOnly = true;
                dgvLines.AllowUserToAddRows = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("LoadLines: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>Timeline from PurchaseOrder approval columns on the selected row only.</summary>
        private void BuildTimelineFromPORow(DataRow row)
        {
            DataTable t = new DataTable();
            t.Columns.Add("Stage", typeof(string));
            t.Columns.Add("User", typeof(string));
            t.Columns.Add("Date", typeof(string));
            t.Columns.Add("Remarks", typeof(string));

            AddTimelineRow(t, "Prepared", row, "PreparedBy", "PreparedDate", "Remarks");
            AddTimelineRow(t, "PM Approval", row, "PMName", "PMApprovedDate", "PMRemarks");
            AddTimelineRow(t, "MH Approval", row, "MHName", "MHApprovedDate", "PMMHRemarks");
            AddTimelineRow(t, "Finalized", row, "FinalizedBy", "FinalizedDate", "FinalizedRemarks");
            AddTimelineRow(t, "Purchase Committee", row, "PurchaseCommitee", "PCAuthoriseDate", "PCRemarks");
            AddTimelineRow(t, "OM Approval", row, "OMName", "OMApprovedDate", "OMRemarks");
            AddTimelineRow(t, "GM Approval", row, "GMName", "GMApprovedDate", "RejectReason");
            AddTimelineRow(t, "PO Generated", row, "POGeneratedBy", "POGeneratedDate", null);
            AddTimelineRow(t, "Sent to Vendor", row, "POSenttoVendorBy", "POSentVendorDate", null);

            if (IsFilled(Col(row, "FinalStatus")))
                t.Rows.Add(Col(row, "FinalStatus"), "", "", Col(row, "FinalComment"));

            dgvTimeline.DataSource = t;
            dgvTimeline.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvTimeline.RowHeadersVisible = false;
            dgvTimeline.ReadOnly = true;
            dgvTimeline.AllowUserToAddRows = false;
        }

        private void AddTimelineRow(DataTable t, string stage, DataRow row, string userCol, string dateCol, string remarksCol)
        {
            if (!row.Table.Columns.Contains(userCol)) return;
            string user = row[userCol]?.ToString() ?? "";
            if (!IsFilled(user) || user == "Deleted") return;
            string date = row.Table.Columns.Contains(dateCol) ? (row[dateCol]?.ToString() ?? "") : "";
            string remarks = remarksCol != null && row.Table.Columns.Contains(remarksCol)
                ? (row[remarksCol]?.ToString() ?? "")
                : "";
            t.Rows.Add(stage, user, date, remarks);
        }

        private void ClearDetail()
        {
            _selectedPO = "";
            lblPO.Text = "PO: —";
            lblStatus.Text = "Select a PO";
            lblStatus.ForeColor = Color.FromArgb(20, 55, 90);
            lblProject.Text = "Project: —";
            lblVendor.Text = "Vendor: —";
            lblPrepared.Text = "Prepared: —";
            lblAmount.Text = "Amount: —";
            lblDelivery.Text = "Delivery: —";
            lblFinal.Text = "Lifecycle: —";
            flpPipeline.Controls.Clear();
            dgvLines.DataSource = null;
            dgvTimeline.DataSource = null;
        }

        private void btnClose_Click(object sender, EventArgs e) => Close();
    }
}
