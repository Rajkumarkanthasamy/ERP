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
            flpSummary = new FlowLayoutPanel();
            splitMain = new SplitContainer();
            dgvPOList = new DataGridView();
            panelDetail = new Panel();
            lblPO = new Label();
            lblStatus = new Label();
            lblProject = new Label();
            lblVendor = new Label();
            lblPrepared = new Label();
            lblAmount = new Label();
            lblDelivery = new Label();
            lblFinal = new Label();
            lblPipeline = new Label();
            flpPipeline = new FlowLayoutPanel();
            splitDetail = new SplitContainer();
            lblLines = new Label();
            dgvLines = new DataGridView();
            lblTimeline = new Label();
            dgvTimeline = new DataGridView();
            btnClose = new Button();
            panelHeader.SuspendLayout();
            panelFilters.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitMain).BeginInit();
            splitMain.Panel1.SuspendLayout();
            splitMain.Panel2.SuspendLayout();
            splitMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvPOList).BeginInit();
            panelDetail.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitDetail).BeginInit();
            splitDetail.Panel1.SuspendLayout();
            splitDetail.Panel2.SuspendLayout();
            splitDetail.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvLines).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvTimeline).BeginInit();
            SuspendLayout();

            // Header
            panelHeader.BackColor = Color.FromArgb(20, 55, 90);
            panelHeader.Controls.Add(lblTitle);
            panelHeader.Controls.Add(lblUser);
            panelHeader.Controls.Add(lblSubtitle);
            panelHeader.Dock = DockStyle.Top;
            panelHeader.Height = 78;

            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI Semibold", 16F, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(16, 10);
            lblTitle.Text = "Purchase Order Status View";

            lblUser.AutoSize = true;
            lblUser.Font = new Font("Segoe UI", 9F);
            lblUser.ForeColor = Color.FromArgb(200, 220, 235);
            lblUser.Location = new Point(18, 42);
            lblUser.Text = "User: -";

            lblSubtitle.AutoSize = true;
            lblSubtitle.Font = new Font("Segoe UI", 8.5F);
            lblSubtitle.ForeColor = Color.FromArgb(170, 195, 215);
            lblSubtitle.Location = new Point(280, 44);
            lblSubtitle.Text = "Track every PO from create → approve → generate → receive / close";

            // Filters
            panelFilters.BackColor = Color.White;
            panelFilters.Dock = DockStyle.Top;
            panelFilters.Height = 96;
            panelFilters.Padding = new Padding(12);
            panelFilters.Controls.Add(lblFilterPO);
            panelFilters.Controls.Add(txtPONumber);
            panelFilters.Controls.Add(lblFilterVendor);
            panelFilters.Controls.Add(txtVendor);
            panelFilters.Controls.Add(lblFilterProject);
            panelFilters.Controls.Add(txtProject);
            panelFilters.Controls.Add(lblFilterStatus);
            panelFilters.Controls.Add(cmbStatus);
            panelFilters.Controls.Add(chkDateFilter);
            panelFilters.Controls.Add(dtpFrom);
            panelFilters.Controls.Add(dtpTo);
            panelFilters.Controls.Add(btnSearch);
            panelFilters.Controls.Add(btnRefresh);
            panelFilters.Controls.Add(lblCount);
            panelFilters.Controls.Add(flpSummary);

            lblFilterPO.Text = "PO No";
            lblFilterPO.Location = new Point(12, 10);
            lblFilterPO.AutoSize = true;
            txtPONumber.Location = new Point(60, 6);
            txtPONumber.Size = new Size(120, 23);

            lblFilterVendor.Text = "Vendor";
            lblFilterVendor.Location = new Point(190, 10);
            lblFilterVendor.AutoSize = true;
            txtVendor.Location = new Point(242, 6);
            txtVendor.Size = new Size(140, 23);

            lblFilterProject.Text = "Project";
            lblFilterProject.Location = new Point(392, 10);
            lblFilterProject.AutoSize = true;
            txtProject.Location = new Point(444, 6);
            txtProject.Size = new Size(140, 23);

            lblFilterStatus.Text = "Status";
            lblFilterStatus.Location = new Point(596, 10);
            lblFilterStatus.AutoSize = true;
            cmbStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbStatus.Location = new Point(644, 6);
            cmbStatus.Size = new Size(200, 23);

            chkDateFilter.Text = "Prepared date";
            chkDateFilter.Location = new Point(12, 40);
            chkDateFilter.AutoSize = true;
            chkDateFilter.CheckedChanged += chkDateFilter_CheckedChanged;

            dtpFrom.Format = DateTimePickerFormat.Custom;
            dtpFrom.CustomFormat = "dd-MM-yyyy";
            dtpFrom.Location = new Point(120, 38);
            dtpFrom.Size = new Size(110, 23);

            dtpTo.Format = DateTimePickerFormat.Custom;
            dtpTo.CustomFormat = "dd-MM-yyyy";
            dtpTo.Location = new Point(236, 38);
            dtpTo.Size = new Size(110, 23);

            btnSearch.Text = "Search";
            btnSearch.BackColor = Color.FromArgb(20, 90, 140);
            btnSearch.ForeColor = Color.White;
            btnSearch.FlatStyle = FlatStyle.Flat;
            btnSearch.Location = new Point(360, 36);
            btnSearch.Size = new Size(90, 28);
            btnSearch.Click += btnSearch_Click;

            btnRefresh.Text = "Reset";
            btnRefresh.FlatStyle = FlatStyle.Flat;
            btnRefresh.Location = new Point(456, 36);
            btnRefresh.Size = new Size(80, 28);
            btnRefresh.Click += btnRefresh_Click;

            lblCount.AutoSize = true;
            lblCount.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            lblCount.Location = new Point(550, 42);
            lblCount.Text = "POs: 0";

            flpSummary.Location = new Point(12, 68);
            flpSummary.Size = new Size(1080, 26);
            flpSummary.WrapContents = false;
            flpSummary.AutoScroll = true;

            // Split
            splitMain.Dock = DockStyle.Fill;
            splitMain.Orientation = Orientation.Horizontal;
            splitMain.SplitterDistance = 280;
            splitMain.Panel1.Controls.Add(dgvPOList);
            splitMain.Panel2.Controls.Add(panelDetail);

            dgvPOList.Dock = DockStyle.Fill;
            dgvPOList.BackgroundColor = Color.White;
            dgvPOList.SelectionChanged += dgvPOList_SelectionChanged;

            panelDetail.Dock = DockStyle.Fill;
            panelDetail.Padding = new Padding(10);
            panelDetail.Controls.Add(splitDetail);
            panelDetail.Controls.Add(flpPipeline);
            panelDetail.Controls.Add(lblPipeline);
            panelDetail.Controls.Add(lblFinal);
            panelDetail.Controls.Add(lblDelivery);
            panelDetail.Controls.Add(lblAmount);
            panelDetail.Controls.Add(lblPrepared);
            panelDetail.Controls.Add(lblVendor);
            panelDetail.Controls.Add(lblProject);
            panelDetail.Controls.Add(lblStatus);
            panelDetail.Controls.Add(lblPO);
            panelDetail.Controls.Add(btnClose);

            lblPO.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
            lblPO.Location = new Point(12, 8);
            lblPO.AutoSize = true;
            lblPO.Text = "PO: —";

            lblStatus.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
            lblStatus.Location = new Point(220, 8);
            lblStatus.AutoSize = true;
            lblStatus.Text = "Select a PO";

            lblProject.Location = new Point(12, 36);
            lblProject.AutoSize = true;
            lblProject.Text = "Project: —";

            lblVendor.Location = new Point(420, 36);
            lblVendor.AutoSize = true;
            lblVendor.Text = "Vendor: —";

            lblPrepared.Location = new Point(12, 58);
            lblPrepared.AutoSize = true;
            lblPrepared.Text = "Prepared: —";

            lblAmount.Location = new Point(420, 58);
            lblAmount.AutoSize = true;
            lblAmount.Text = "Amount: —";

            lblDelivery.Location = new Point(700, 58);
            lblDelivery.AutoSize = true;
            lblDelivery.Text = "Delivery: —";

            lblFinal.Location = new Point(12, 80);
            lblFinal.AutoSize = true;
            lblFinal.Text = "Lifecycle: —";

            lblPipeline.Text = "Approval pipeline";
            lblPipeline.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            lblPipeline.Location = new Point(12, 104);
            lblPipeline.AutoSize = true;

            flpPipeline.Location = new Point(12, 124);
            flpPipeline.Size = new Size(1080, 80);
            flpPipeline.WrapContents = false;
            flpPipeline.AutoScroll = true;

            splitDetail.Location = new Point(12, 210);
            splitDetail.Size = new Size(1080, 200);
            splitDetail.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            splitDetail.SplitterDistance = 560;

            splitDetail.Panel1.Controls.Add(dgvLines);
            splitDetail.Panel1.Controls.Add(lblLines);
            splitDetail.Panel2.Controls.Add(dgvTimeline);
            splitDetail.Panel2.Controls.Add(lblTimeline);

            lblLines.Dock = DockStyle.Top;
            lblLines.Height = 22;
            lblLines.Text = "  PO line items";
            lblLines.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            dgvLines.Dock = DockStyle.Fill;
            dgvLines.BackgroundColor = Color.White;

            lblTimeline.Dock = DockStyle.Top;
            lblTimeline.Height = 22;
            lblTimeline.Text = "  Remarks / approval timeline";
            lblTimeline.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            dgvTimeline.Dock = DockStyle.Fill;
            dgvTimeline.BackgroundColor = Color.White;

            btnClose.Text = "Close";
            btnClose.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnClose.Location = new Point(1000, 6);
            btnClose.Size = new Size(90, 28);
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.Click += btnClose_Click;

            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(240, 244, 248);
            ClientSize = new Size(1120, 720);
            Controls.Add(splitMain);
            Controls.Add(panelFilters);
            Controls.Add(panelHeader);
            MinimumSize = new Size(1000, 640);
            Name = "frmPOStatusView";
            StartPosition = FormStartPosition.CenterParent;
            Text = "PO Status View";
            Load += frmPOStatusView_Load;

            panelHeader.ResumeLayout(false);
            panelHeader.PerformLayout();
            panelFilters.ResumeLayout(false);
            panelFilters.PerformLayout();
            splitMain.Panel1.ResumeLayout(false);
            splitMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitMain).EndInit();
            splitMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvPOList).EndInit();
            panelDetail.ResumeLayout(false);
            panelDetail.PerformLayout();
            splitDetail.Panel1.ResumeLayout(false);
            splitDetail.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitDetail).EndInit();
            splitDetail.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvLines).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvTimeline).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel panelHeader;
        private Label lblTitle;
        private Label lblUser;
        private Label lblSubtitle;
        private Panel panelFilters;
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
        private FlowLayoutPanel flpSummary;
        private SplitContainer splitMain;
        private DataGridView dgvPOList;
        private Panel panelDetail;
        private Label lblPO;
        private Label lblStatus;
        private Label lblProject;
        private Label lblVendor;
        private Label lblPrepared;
        private Label lblAmount;
        private Label lblDelivery;
        private Label lblFinal;
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
