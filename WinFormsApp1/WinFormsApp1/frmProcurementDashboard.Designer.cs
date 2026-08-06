namespace WinFormsApp1
{
    partial class frmProcurementDashboard
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
            flpMetrics = new FlowLayoutPanel();
            lblAging = new Label();
            dgvAging = new DataGridView();
            lblActivity = new Label();
            dgvActivity = new DataGridView();
            btnRefresh = new Button();
            btnOpenPRApproval = new Button();
            btnOpenPOApproval = new Button();
            btnOpenWorkspace = new Button();
            btnOpenKanban = new Button();
            btnOpenGRN = new Button();
            btnClose = new Button();
            panelHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvAging).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvActivity).BeginInit();
            SuspendLayout();
            //
            // panelHeader
            //
            panelHeader.BackColor = Color.FromArgb(20, 55, 90);
            panelHeader.Controls.Add(lblTitle);
            panelHeader.Controls.Add(lblUser);
            panelHeader.Dock = DockStyle.Top;
            panelHeader.Height = 80;
            panelHeader.Name = "panelHeader";
            //
            // lblTitle
            //
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI Semibold", 18F, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(20, 14);
            lblTitle.Name = "lblTitle";
            lblTitle.Text = "Procurement Inbox / Dashboard";
            //
            // lblUser
            //
            lblUser.AutoSize = true;
            lblUser.Font = new Font("Segoe UI", 9.5F);
            lblUser.ForeColor = Color.FromArgb(190, 210, 230);
            lblUser.Location = new Point(22, 50);
            lblUser.Name = "lblUser";
            lblUser.Text = "User";
            //
            // flpMetrics
            //
            flpMetrics.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            flpMetrics.Location = new Point(20, 100);
            flpMetrics.Name = "flpMetrics";
            flpMetrics.Size = new Size(1040, 130);
            flpMetrics.WrapContents = false;
            //
            // lblAging
            //
            lblAging.AutoSize = true;
            lblAging.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
            lblAging.Location = new Point(20, 240);
            lblAging.Name = "lblAging";
            lblAging.Text = "Aging PRs (Pending / On Hold)";
            //
            // dgvAging
            //
            dgvAging.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            dgvAging.BackgroundColor = Color.White;
            dgvAging.Location = new Point(20, 265);
            dgvAging.Name = "dgvAging";
            dgvAging.Size = new Size(1040, 180);
            //
            // lblActivity
            //
            lblActivity.AutoSize = true;
            lblActivity.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
            lblActivity.Location = new Point(20, 460);
            lblActivity.Name = "lblActivity";
            lblActivity.Text = "Recent PR Activity";
            //
            // dgvActivity
            //
            dgvActivity.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvActivity.BackgroundColor = Color.White;
            dgvActivity.Location = new Point(20, 485);
            dgvActivity.Name = "dgvActivity";
            dgvActivity.Size = new Size(1040, 160);
            //
            // buttons
            //
            btnRefresh.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnRefresh.Location = new Point(20, 660);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(120, 34);
            btnRefresh.Text = "Refresh";
            btnRefresh.Click += btnRefresh_Click;

            btnOpenPRApproval.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnOpenPRApproval.Location = new Point(150, 660);
            btnOpenPRApproval.Name = "btnOpenPRApproval";
            btnOpenPRApproval.Size = new Size(140, 34);
            btnOpenPRApproval.Text = "PR Approval";
            btnOpenPRApproval.Click += btnOpenPRApproval_Click;

            btnOpenPOApproval.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnOpenPOApproval.Location = new Point(300, 660);
            btnOpenPOApproval.Name = "btnOpenPOApproval";
            btnOpenPOApproval.Size = new Size(140, 34);
            btnOpenPOApproval.Text = "PO Approval";
            btnOpenPOApproval.Click += btnOpenPOApproval_Click;

            btnOpenWorkspace.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnOpenWorkspace.BackColor = Color.FromArgb(20, 90, 140);
            btnOpenWorkspace.FlatStyle = FlatStyle.Flat;
            btnOpenWorkspace.ForeColor = Color.White;
            btnOpenWorkspace.Location = new Point(450, 660);
            btnOpenWorkspace.Name = "btnOpenWorkspace";
            btnOpenWorkspace.Size = new Size(150, 34);
            btnOpenWorkspace.Text = "Drag-Drop PO";
            btnOpenWorkspace.Click += btnOpenWorkspace_Click;

            btnOpenKanban = new Button();
            btnOpenKanban.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnOpenKanban.Location = new Point(610, 660);
            btnOpenKanban.Name = "btnOpenKanban";
            btnOpenKanban.Size = new Size(110, 34);
            btnOpenKanban.Text = "Kanban";
            btnOpenKanban.Click += btnOpenKanban_Click;

            btnOpenGRN = new Button();
            btnOpenGRN.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnOpenGRN.Location = new Point(730, 660);
            btnOpenGRN.Name = "btnOpenGRN";
            btnOpenGRN.Size = new Size(100, 34);
            btnOpenGRN.Text = "GRN";
            btnOpenGRN.Click += btnOpenGRN_Click;

            btnClose.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnClose.Location = new Point(960, 660);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(100, 34);
            btnClose.Text = "Close";
            btnClose.Click += btnClose_Click;
            //
            // frmProcurementDashboard
            //
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(240, 244, 248);
            ClientSize = new Size(1080, 710);
            Controls.Add(btnClose);
            Controls.Add(btnOpenGRN);
            Controls.Add(btnOpenKanban);
            Controls.Add(btnOpenWorkspace);
            Controls.Add(btnOpenPOApproval);
            Controls.Add(btnOpenPRApproval);
            Controls.Add(btnRefresh);
            Controls.Add(dgvActivity);
            Controls.Add(lblActivity);
            Controls.Add(dgvAging);
            Controls.Add(lblAging);
            Controls.Add(flpMetrics);
            Controls.Add(panelHeader);
            Name = "frmProcurementDashboard";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Procurement Dashboard";
            Load += frmProcurementDashboard_Load;
            panelHeader.ResumeLayout(false);
            panelHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvAging).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvActivity).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel panelHeader;
        private Label lblTitle;
        private Label lblUser;
        private FlowLayoutPanel flpMetrics;
        private Label lblAging;
        private DataGridView dgvAging;
        private Label lblActivity;
        private DataGridView dgvActivity;
        private Button btnRefresh;
        private Button btnOpenPRApproval;
        private Button btnOpenPOApproval;
        private Button btnOpenWorkspace;
        private Button btnOpenKanban;
        private Button btnOpenGRN;
        private Button btnClose;
    }
}
