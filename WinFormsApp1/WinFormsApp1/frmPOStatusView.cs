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
        private bool _fittingToScreen;
        private Size _lastLayoutSize = Size.Empty;
        private double _mainSplitRatio = 0.48;
        private double _detailSplitRatio = 0.55;

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
            FitFormToScreen();
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

        private void frmPOStatusView_Shown(object sender, EventArgs e)
        {
            // Re-fit after the form is actually visible (parent/DPI applied).
            FitFormToScreen();
            _splitsInitialized = false;
            LayoutDetailPanels();
        }

        private void frmPOStatusView_Resize(object sender, EventArgs e)
        {
            if (_fittingToScreen)
                return;
            LayoutDetailPanels();
        }

        /// <summary>
        /// Size the form to the current monitor working area so it fits
        /// laptops, desktops, and scaled DPI displays without clipping.
        /// </summary>
        private void FitFormToScreen()
        {
            if (_fittingToScreen)
                return;

            _fittingToScreen = true;
            try
            {
                Screen screen = Screen.FromControl(this);
                if (screen == null)
                    screen = Screen.PrimaryScreen;
                if (screen == null)
                    return;

                Rectangle work = screen.WorkingArea;

                // Preferred size: ~92% of working area, but never smaller than MinimumSize
                // and never larger than the working area.
                int targetW = Math.Max(MinimumSize.Width, (int)(work.Width * 0.92));
                int targetH = Math.Max(MinimumSize.Height, (int)(work.Height * 0.90));
                targetW = Math.Min(targetW, work.Width - 16);
                targetH = Math.Min(targetH, work.Height - 16);

                // Very small screens: maximize so nothing is cut off.
                if (work.Width < 1100 || work.Height < 700)
                {
                    WindowState = FormWindowState.Maximized;
                }
                else
                {
                    if (WindowState == FormWindowState.Maximized)
                        WindowState = FormWindowState.Normal;

                    Size = new Size(targetW, targetH);

                    // Center on the same monitor
                    Left = work.Left + Math.Max(0, (work.Width - Width) / 2);
                    Top = work.Top + Math.Max(0, (work.Height - Height) / 2);
                }

                AdaptChromeForWidth();
            }
            finally
            {
                _fittingToScreen = false;
            }
        }

        /// <summary>Hide/show header subtitle and enable filter scroll on narrow screens.</summary>
        private void AdaptChromeForWidth()
        {
            int w = ClientSize.Width;
            if (lblSubtitle != null)
                lblSubtitle.Visible = w >= 980;

            if (panelFilters != null)
                panelFilters.AutoScroll = w < 1050;

            if (panelPipeline != null)
            {
                // Slightly shorter pipeline strip on short screens to leave room for grids
                panelPipeline.Height = ClientSize.Height < 700 ? 90 : 108;
            }

            if (panelInfo != null)
                panelInfo.Height = ClientSize.Height < 700 ? 88 : 100;
        }

        /// <summary>Keep Close button / splits / chrome aligned as the form resizes.</summary>
        private void LayoutDetailPanels()
        {
            AdaptChromeForWidth();

            if (btnClose != null && panelInfo != null && panelInfo.ClientSize.Width > 0)
                btnClose.Left = Math.Max(220, panelInfo.ClientSize.Width - btnClose.Width - 12);

            if (splitMain == null || splitDetail == null)
                return;

            bool sizeChangedSignificantly =
                _lastLayoutSize.IsEmpty
                || Math.Abs(ClientSize.Width - _lastLayoutSize.Width) > 40
                || Math.Abs(ClientSize.Height - _lastLayoutSize.Height) > 40;

            if (!_splitsInitialized
                || (sizeChangedSignificantly
                    && splitMain.ClientSize.Height > 200
                    && splitDetail.ClientSize.Width > 200))
            {
                SafeSetSplitterDistance(splitMain, _mainSplitRatio);
                SafeSetSplitterDistance(splitDetail, _detailSplitRatio);
                _splitsInitialized = true;
                _lastLayoutSize = ClientSize;
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

        private void btnSearch_Click(object sender, EventArgs e)
        {
            LoadStatusList();
        }

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
                string status = "All";
                if (cmbStatus.SelectedItem != null)
                    status = cmbStatus.SelectedItem.ToString();
                DateTime? from = chkDateFilter.Checked ? (DateTime?)dtpFrom.Value.Date : null;
                DateTime? to = chkDateFilter.Checked ? (DateTime?)dtpTo.Value.Date : null;

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
                object cellVal = row.Cells["CurrentStatus"].Value;
                string status = cellVal == null ? "" : cellVal.ToString();
                Color back = Color.White;

                if (status == "Cancelled" || status == "Deleted")
                    back = Color.FromArgb(255, 230, 230);
                else if (status == "Closed")
                    back = Color.FromArgb(230, 230, 230);
                else if (status == "Under Review")
                    back = Color.FromArgb(255, 245, 200);
                else if (status == "Fully Received")
                    back = Color.FromArgb(220, 245, 220);
                else if (status == "Partially Received")
                    back = Color.FromArgb(230, 245, 255);
                else if (status == "Ready to Generate")
                    back = Color.FromArgb(225, 240, 255);
                else if (status == "PO Generated" || status == "Sent to Vendor")
                    back = Color.FromArgb(235, 250, 235);
                else if (status == "Pending GM Approval")
                    back = Color.FromArgb(255, 235, 210);
                else if (status.StartsWith("Pending", StringComparison.OrdinalIgnoreCase))
                    back = Color.FromArgb(255, 248, 230);

                row.DefaultCellStyle.BackColor = back;
            }
        }

        private void UpdateSummaryChips(DataTable dt)
        {
            flpSummary.Controls.Clear();
            System.Collections.Generic.SortedDictionary<string, int> counts =
                new System.Collections.Generic.SortedDictionary<string, int>();
            foreach (DataRow row in dt.Rows)
            {
                object stObj = row["CurrentStatus"];
                string st = stObj == null || stObj == DBNull.Value ? "Unknown" : stObj.ToString();
                if (!counts.ContainsKey(st)) counts[st] = 0;
                counts[st]++;
            }

            foreach (System.Collections.Generic.KeyValuePair<string, int> kv in counts)
            {
                Label chip = new Label();
                chip.AutoSize = true;
                chip.Margin = new Padding(4);
                chip.Padding = new Padding(8, 4, 8, 4);
                chip.BackColor = Color.FromArgb(20, 55, 90);
                chip.ForeColor = Color.White;
                chip.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
                chip.Text = kv.Key + ": " + kv.Value;
                flpSummary.Controls.Add(chip);
            }
        }

        private void dgvPOList_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvPOList.CurrentRow == null)
            {
                ClearDetail();
                return;
            }

            DataRowView view = dgvPOList.CurrentRow.DataBoundItem as DataRowView;
            if (view == null)
            {
                ClearDetail();
                return;
            }

            DataRow row = view.Row;
            object poObj = row["PONumber"];
            _selectedPO = poObj == null || poObj == DBNull.Value ? "" : poObj.ToString();
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

            // Approval route from Amount + GST (.NET 4.5 compatible — no ValueTuple)
            decimal routeAmt = amount;
            POApprovalRoute route = DataAccessLayer.GetPOApprovalRoute(routeAmt);
            lblFinal.Text = lblFinal.Text + "  |  Route: " + route.NextApprover
                + " (<=50k PC final / >50k-2.5L Vivek G-OM / >2.5L GM)";

            BuildPipeline(row);
            LayoutDetailPanels();
        }

        private static string Col(DataRow row, string name, string fallback)
        {
            if (row == null || !row.Table.Columns.Contains(name) || row[name] == DBNull.Value)
                return fallback;
            object v = row[name];
            return v == null ? fallback : v.ToString();
        }

        private static string Col(DataRow row, string name)
        {
            return Col(row, name, "");
        }

        private static Color StatusColor(string status)
        {
            if (status == "Cancelled" || status == "Deleted")
                return Color.Firebrick;
            if (status == "Closed")
                return Color.DimGray;
            if (status == "Under Review")
                return Color.DarkOrange;
            if (status == "Fully Received")
                return Color.ForestGreen;
            if (status == "Ready to Generate" || status == "PO Generated" || status == "Sent to Vendor")
                return Color.DarkGreen;
            if (!string.IsNullOrEmpty(status) && status.StartsWith("Pending", StringComparison.OrdinalIgnoreCase))
                return Color.Chocolate;
            return Color.FromArgb(20, 55, 90);
        }

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
            string s = value.ToString();
            if (s == null) return false;
            s = s.Trim();
            return s.Length > 0 && s != "--";
        }

        private static bool ToBool(object value)
        {
            if (value == null || value == DBNull.Value) return false;
            if (value is bool) return (bool)value;
            int i;
            if (int.TryParse(value.ToString(), out i)) return i != 0;
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
            object userObj = row[userCol];
            string user = userObj == null || userObj == DBNull.Value ? "" : userObj.ToString();
            if (!IsFilled(user) || user == "Deleted") return;
            string date = "";
            if (row.Table.Columns.Contains(dateCol))
            {
                object dateObj = row[dateCol];
                date = dateObj == null || dateObj == DBNull.Value ? "" : dateObj.ToString();
            }
            string remarks = "";
            if (remarksCol != null && row.Table.Columns.Contains(remarksCol))
            {
                object remObj = row[remarksCol];
                remarks = remObj == null || remObj == DBNull.Value ? "" : remObj.ToString();
            }
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

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
