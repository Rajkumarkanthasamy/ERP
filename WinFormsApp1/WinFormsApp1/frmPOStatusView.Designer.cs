using System.Drawing;
using System.Windows.Forms;

namespace WinFormsApp1
{
    partial class frmPOStatusView
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            panelHeader = new Panel();
            lblTitle = new Label();
            lblUser = new Label();
            lblSubtitle = new Label();
            panelFilters = new Panel();
            tblFilters = new TableLayoutPanel();
            lblFilterPO = new Label();
            txtPONumber = new TextBox();
            lblFilterVendor = new Label();
            txtVendor = new TextBox();
            lblFilterProject = new Label();
            txtProject = new TextBox();
            lblFilterStatus = new Label();
            cmbStatus = new ComboBox();
            chkDateFilter = new CheckBox();
            dtpFrom = new DateTimePicker();
            dtpTo = new DateTimePicker();
            btnSearch = new Button();
            btnRefresh = new Button();
            lblCount = new Label();
            panelSummary = new Panel();
            flpSummary = new FlowLayoutPanel();
            splitMain = new SplitContainer();
            dgvPOList = new DataGridView();
            panelDetail = new Panel();
            panelInfo = new Panel();
            tblInfo = new TableLayoutPanel();
            lblPO = new Label();
            lblStatus = new Label();
            btnClose = new Button();
            lblProject = new Label();
            lblVendor = new Label();
            lblPrepared = new Label();
            lblAmount = new Label();
            lblDelivery = new Label();
            lblFinal = new Label();
            panelPipeline = new Panel();
            lblPipeline = new Label();
            flpPipeline = new FlowLayoutPanel();
            splitDetail = new SplitContainer();
            lblLines = new Label();
            dgvLines = new DataGridView();
            lblTimeline = new Label();
            dgvTimeline = new DataGridView();

            panelHeader.SuspendLayout();
            panelFilters.SuspendLayout();
            tblFilters.SuspendLayout();
            panelSummary.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitMain).BeginInit();
            splitMain.Panel1.SuspendLayout();
            splitMain.Panel2.SuspendLayout();
            splitMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvPOList).BeginInit();
            panelDetail.SuspendLayout();
            panelInfo.SuspendLayout();
            tblInfo.SuspendLayout();
            panelPipeline.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitDetail).BeginInit();
            splitDetail.Panel1.SuspendLayout();
            splitDetail.Panel2.SuspendLayout();
            splitDetail.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvLines).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvTimeline).BeginInit();
            SuspendLayout();

            // ===== Header =====
            panelHeader.BackColor = Color.FromArgb(20, 55, 90);
            panelHeader.Dock = DockStyle.Top;
            panelHeader.Height = 72;
            panelHeader.Padding = new Padding(16, 10, 16, 8);
            panelHeader.Controls.Add(lblSubtitle);
            panelHeader.Controls.Add(lblUser);
            panelHeader.Controls.Add(lblTitle);

            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI Semibold", 15F, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(16, 8);
            lblTitle.Text = "Purchase Order Status View";

            lblUser.AutoSize = true;
            lblUser.Font = new Font("Segoe UI", 9F);
            lblUser.ForeColor = Color.FromArgb(200, 220, 235);
            lblUser.Location = new Point(18, 40);
            lblUser.Text = "User: -";

            lblSubtitle.AutoSize = true;
            lblSubtitle.Font = new Font("Segoe UI", 8.5F);
            lblSubtitle.ForeColor = Color.FromArgb(170, 195, 215);
            lblSubtitle.Location = new Point(320, 42);
            lblSubtitle.Text = "Track every PO from create → approve → generate → receive / close";

            // ===== Filters (table aligned) =====
            panelFilters.BackColor = Color.White;
            panelFilters.Dock = DockStyle.Top;
            panelFilters.Height = 78;
            panelFilters.Padding = new Padding(8, 6, 8, 4);
            panelFilters.Controls.Add(tblFilters);

            tblFilters.ColumnCount = 12;
            tblFilters.RowCount = 2;
            tblFilters.Dock = DockStyle.Fill;
            tblFilters.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 52F));   // PO label
            tblFilters.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));  // PO box
            tblFilters.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 55F));   // Vendor label
            tblFilters.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130F));  // Vendor box
            tblFilters.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 55F));   // Project label
            tblFilters.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130F));  // Project box
            tblFilters.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 50F));   // Status label
            tblFilters.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 200F));  // Status combo
            tblFilters.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90F));   // Search
            tblFilters.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 80F));   // Reset
            tblFilters.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90F));   // Count
            tblFilters.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));   // spacer
            tblFilters.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            tblFilters.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            tblFilters.Controls.Add(lblFilterPO, 0, 0);
            tblFilters.Controls.Add(txtPONumber, 1, 0);
            tblFilters.Controls.Add(lblFilterVendor, 2, 0);
            tblFilters.Controls.Add(txtVendor, 3, 0);
            tblFilters.Controls.Add(lblFilterProject, 4, 0);
            tblFilters.Controls.Add(txtProject, 5, 0);
            tblFilters.Controls.Add(lblFilterStatus, 6, 0);
            tblFilters.Controls.Add(cmbStatus, 7, 0);
            tblFilters.Controls.Add(btnSearch, 8, 0);
            tblFilters.Controls.Add(btnRefresh, 9, 0);
            tblFilters.Controls.Add(lblCount, 10, 0);
            tblFilters.Controls.Add(chkDateFilter, 0, 1);
            tblFilters.SetColumnSpan(chkDateFilter, 2);
            tblFilters.Controls.Add(dtpFrom, 2, 1);
            tblFilters.SetColumnSpan(dtpFrom, 2);
            tblFilters.Controls.Add(dtpTo, 4, 1);
            tblFilters.SetColumnSpan(dtpTo, 2);

            StyleFilterLabel(lblFilterPO, "PO No");
            StyleFilterLabel(lblFilterVendor, "Vendor");
            StyleFilterLabel(lblFilterProject, "Project");
            StyleFilterLabel(lblFilterStatus, "Status");

            txtPONumber.Dock = DockStyle.Fill;
            txtPONumber.Margin = new Padding(2, 4, 8, 4);
            txtVendor.Dock = DockStyle.Fill;
            txtVendor.Margin = new Padding(2, 4, 8, 4);
            txtProject.Dock = DockStyle.Fill;
            txtProject.Margin = new Padding(2, 4, 8, 4);

            cmbStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbStatus.Dock = DockStyle.Fill;
            cmbStatus.Margin = new Padding(2, 4, 8, 4);

            chkDateFilter.Text = "Prepared date";
            chkDateFilter.Dock = DockStyle.Fill;
            chkDateFilter.Margin = new Padding(4, 6, 4, 2);
            chkDateFilter.CheckedChanged += chkDateFilter_CheckedChanged;

            dtpFrom.Format = DateTimePickerFormat.Custom;
            dtpFrom.CustomFormat = "dd-MM-yyyy";
            dtpFrom.Dock = DockStyle.Fill;
            dtpFrom.Margin = new Padding(2, 4, 8, 4);

            dtpTo.Format = DateTimePickerFormat.Custom;
            dtpTo.CustomFormat = "dd-MM-yyyy";
            dtpTo.Dock = DockStyle.Fill;
            dtpTo.Margin = new Padding(2, 4, 8, 4);

            btnSearch.Text = "Search";
            btnSearch.BackColor = Color.FromArgb(20, 90, 140);
            btnSearch.ForeColor = Color.White;
            btnSearch.FlatStyle = FlatStyle.Flat;
            btnSearch.Dock = DockStyle.Fill;
            btnSearch.Margin = new Padding(4, 2, 4, 2);
            btnSearch.Click += btnSearch_Click;

            btnRefresh.Text = "Reset";
            btnRefresh.FlatStyle = FlatStyle.Flat;
            btnRefresh.Dock = DockStyle.Fill;
            btnRefresh.Margin = new Padding(4, 2, 4, 2);
            btnRefresh.Click += btnRefresh_Click;

            lblCount.AutoSize = false;
            lblCount.Dock = DockStyle.Fill;
            lblCount.TextAlign = ContentAlignment.MiddleLeft;
            lblCount.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            lblCount.Margin = new Padding(8, 4, 4, 4);
            lblCount.Text = "POs: 0";

            // ===== Summary chips strip =====
            panelSummary.BackColor = Color.FromArgb(245, 248, 252);
            panelSummary.Dock = DockStyle.Top;
            panelSummary.Height = 36;
            panelSummary.Padding = new Padding(8, 4, 8, 4);
            panelSummary.Controls.Add(flpSummary);

            flpSummary.Dock = DockStyle.Fill;
            flpSummary.WrapContents = false;
            flpSummary.AutoScroll = true;
            flpSummary.FlowDirection = FlowDirection.LeftToRight;

            // ===== Main split: list | detail =====
            splitMain.Dock = DockStyle.Fill;
            splitMain.Orientation = Orientation.Horizontal;
            splitMain.SplitterDistance = 300;
            splitMain.SplitterWidth = 6;
            splitMain.Panel1MinSize = 120;
            splitMain.Panel2MinSize = 220;
            splitMain.Panel1.Controls.Add(dgvPOList);
            splitMain.Panel2.Controls.Add(panelDetail);
            splitMain.Panel2.Padding = new Padding(0);

            dgvPOList.Dock = DockStyle.Fill;
            dgvPOList.BackgroundColor = Color.White;
            dgvPOList.BorderStyle = BorderStyle.None;
            dgvPOList.SelectionChanged += dgvPOList_SelectionChanged;

            // ===== Detail (docked sections) =====
            panelDetail.Dock = DockStyle.Fill;
            panelDetail.BackColor = Color.White;
            panelDetail.Padding = new Padding(0);
            // Add fill first, then top panels (WinForms docks top last = higher)
            panelDetail.Controls.Add(splitDetail);
            panelDetail.Controls.Add(panelPipeline);
            panelDetail.Controls.Add(panelInfo);

            // --- Info header ---
            panelInfo.Dock = DockStyle.Top;
            panelInfo.Height = 100;
            panelInfo.Padding = new Padding(8, 6, 8, 4);
            panelInfo.BackColor = Color.FromArgb(250, 252, 255);
            panelInfo.Controls.Add(tblInfo);
            panelInfo.Controls.Add(btnClose);

            btnClose.Text = "Close";
            btnClose.Size = new Size(88, 28);
            btnClose.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnClose.Location = new Point(1010, 8);
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.Click += btnClose_Click;

            tblInfo.Dock = DockStyle.Fill;
            tblInfo.ColumnCount = 3;
            tblInfo.RowCount = 4;
            tblInfo.Padding = new Padding(0, 0, 100, 0); // leave room for Close
            tblInfo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            tblInfo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F));
            tblInfo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tblInfo.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
            tblInfo.RowStyles.Add(new RowStyle(SizeType.Absolute, 22F));
            tblInfo.RowStyles.Add(new RowStyle(SizeType.Absolute, 22F));
            tblInfo.RowStyles.Add(new RowStyle(SizeType.Absolute, 22F));
            tblInfo.Controls.Add(lblPO, 0, 0);
            tblInfo.Controls.Add(lblStatus, 1, 0);
            tblInfo.SetColumnSpan(lblStatus, 2);
            tblInfo.Controls.Add(lblProject, 0, 1);
            tblInfo.Controls.Add(lblVendor, 1, 1);
            tblInfo.SetColumnSpan(lblVendor, 2);
            tblInfo.Controls.Add(lblPrepared, 0, 2);
            tblInfo.Controls.Add(lblAmount, 1, 2);
            tblInfo.Controls.Add(lblDelivery, 2, 2);
            tblInfo.Controls.Add(lblFinal, 0, 3);
            tblInfo.SetColumnSpan(lblFinal, 3);

            StyleInfoLabel(lblPO, "PO: —", true);
            StyleInfoLabel(lblStatus, "Select a PO", true);
            StyleInfoLabel(lblProject, "Project: —", false);
            StyleInfoLabel(lblVendor, "Vendor: —", false);
            StyleInfoLabel(lblPrepared, "Prepared: —", false);
            StyleInfoLabel(lblAmount, "Amount: —", false);
            StyleInfoLabel(lblDelivery, "Delivery: —", false);
            StyleInfoLabel(lblFinal, "Lifecycle: —", false);

            // --- Pipeline ---
            panelPipeline.Dock = DockStyle.Top;
            panelPipeline.Height = 108;
            panelPipeline.Padding = new Padding(12, 4, 12, 4);
            panelPipeline.BackColor = Color.White;
            panelPipeline.Controls.Add(flpPipeline);
            panelPipeline.Controls.Add(lblPipeline);

            lblPipeline.Dock = DockStyle.Top;
            lblPipeline.Height = 22;
            lblPipeline.TextAlign = ContentAlignment.MiddleLeft;
            lblPipeline.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            lblPipeline.Text = "Approval pipeline";

            flpPipeline.Dock = DockStyle.Fill;
            flpPipeline.WrapContents = false;
            flpPipeline.AutoScroll = true;
            flpPipeline.FlowDirection = FlowDirection.LeftToRight;
            flpPipeline.Padding = new Padding(0, 2, 0, 2);

            // --- Lines | Timeline ---
            splitDetail.Dock = DockStyle.Fill;
            splitDetail.Orientation = Orientation.Vertical;
            splitDetail.SplitterWidth = 6;
            splitDetail.SplitterDistance = 560;
            splitDetail.Panel1MinSize = 200;
            splitDetail.Panel2MinSize = 200;
            splitDetail.Margin = new Padding(0);
            splitDetail.Panel1.Padding = new Padding(12, 4, 4, 8);
            splitDetail.Panel2.Padding = new Padding(4, 4, 12, 8);

            splitDetail.Panel1.Controls.Add(dgvLines);
            splitDetail.Panel1.Controls.Add(lblLines);
            splitDetail.Panel2.Controls.Add(dgvTimeline);
            splitDetail.Panel2.Controls.Add(lblTimeline);

            lblLines.Dock = DockStyle.Top;
            lblLines.Height = 24;
            lblLines.TextAlign = ContentAlignment.MiddleLeft;
            lblLines.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            lblLines.Text = "PO line items";

            dgvLines.Dock = DockStyle.Fill;
            dgvLines.BackgroundColor = Color.White;
            dgvLines.BorderStyle = BorderStyle.FixedSingle;

            lblTimeline.Dock = DockStyle.Top;
            lblTimeline.Height = 24;
            lblTimeline.TextAlign = ContentAlignment.MiddleLeft;
            lblTimeline.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            lblTimeline.Text = "Approval timeline (from PurchaseOrder)";

            dgvTimeline.Dock = DockStyle.Fill;
            dgvTimeline.BackgroundColor = Color.White;
            dgvTimeline.BorderStyle = BorderStyle.FixedSingle;

            // ===== Form =====
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(240, 244, 248);
            ClientSize = new Size(1120, 720);
            // Dock order: Fill first, then Top panels (last Top = highest)
            Controls.Add(splitMain);
            Controls.Add(panelSummary);
            Controls.Add(panelFilters);
            Controls.Add(panelHeader);
            MinimumSize = new Size(1000, 640);
            Name = "frmPOStatusView";
            StartPosition = FormStartPosition.CenterParent;
            Text = "PO Status View";
            Load += frmPOStatusView_Load;
            Resize += frmPOStatusView_Resize;

            panelHeader.ResumeLayout(false);
            panelHeader.PerformLayout();
            tblFilters.ResumeLayout(false);
            tblFilters.PerformLayout();
            panelFilters.ResumeLayout(false);
            panelSummary.ResumeLayout(false);
            splitMain.Panel1.ResumeLayout(false);
            splitMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitMain).EndInit();
            splitMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvPOList).EndInit();
            panelInfo.ResumeLayout(false);
            tblInfo.ResumeLayout(false);
            tblInfo.PerformLayout();
            panelPipeline.ResumeLayout(false);
            panelDetail.ResumeLayout(false);
            splitDetail.Panel1.ResumeLayout(false);
            splitDetail.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitDetail).EndInit();
            splitDetail.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvLines).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvTimeline).EndInit();
            ResumeLayout(false);
        }

        private static void StyleFilterLabel(Label lbl, string text)
        {
            lbl.Text = text;
            lbl.Dock = DockStyle.Fill;
            lbl.TextAlign = ContentAlignment.MiddleLeft;
            lbl.Margin = new Padding(4, 4, 2, 4);
        }

        private static void StyleInfoLabel(Label lbl, string text, bool bold)
        {
            lbl.Text = text;
            lbl.Dock = DockStyle.Fill;
            lbl.TextAlign = ContentAlignment.MiddleLeft;
            lbl.Margin = new Padding(4, 0, 8, 0);
            lbl.AutoEllipsis = true;
            lbl.Font = bold
                ? new Font("Segoe UI Semibold", 11F, FontStyle.Bold)
                : new Font("Segoe UI", 9F);
        }

        #endregion

        private Panel panelHeader;
        private Label lblTitle;
        private Label lblUser;
        private Label lblSubtitle;
        private Panel panelFilters;
        private TableLayoutPanel tblFilters;
        private Label lblFilterPO;
        private TextBox txtPONumber;
        private Label lblFilterVendor;
        private TextBox txtVendor;
        private Label lblFilterProject;
        private TextBox txtProject;
        private Label lblFilterStatus;
        private ComboBox cmbStatus;
        private CheckBox chkDateFilter;
        private DateTimePicker dtpFrom;
        private DateTimePicker dtpTo;
        private Button btnSearch;
        private Button btnRefresh;
        private Label lblCount;
        private Panel panelSummary;
        private FlowLayoutPanel flpSummary;
        private SplitContainer splitMain;
        private DataGridView dgvPOList;
        private Panel panelDetail;
        private Panel panelInfo;
        private TableLayoutPanel tblInfo;
        private Label lblPO;
        private Label lblStatus;
        private Label lblProject;
        private Label lblVendor;
        private Label lblPrepared;
        private Label lblAmount;
        private Label lblDelivery;
        private Label lblFinal;
        private Panel panelPipeline;
        private Label lblPipeline;
        private FlowLayoutPanel flpPipeline;
        private SplitContainer splitDetail;
        private Label lblLines;
        private DataGridView dgvLines;
        private Label lblTimeline;
        private DataGridView dgvTimeline;
        private Button btnClose;
    }
}
