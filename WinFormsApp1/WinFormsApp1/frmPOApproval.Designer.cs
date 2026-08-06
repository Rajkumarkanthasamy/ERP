namespace WinFormsApp1
{
    partial class frmPOApproval
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
            lblCount = new Label();
            lblTotal = new Label();
            dgvPOs = new DataGridView();
            grpDetail = new GroupBox();
            lblPOId = new Label();
            lblPRRef = new Label();
            lblProject = new Label();
            lblVendor = new Label();
            lblItem = new Label();
            lblAmount = new Label();
            lblPMStatus = new Label();
            lblMHStatus = new Label();
            lblRemarks = new Label();
            txtRemarks = new TextBox();
            btnPMApprove = new Button();
            btnMHApprove = new Button();
            btnFinalApprove = new Button();
            btnReject = new Button();
            btnRefresh = new Button();
            btnClose = new Button();
            panelHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvPOs).BeginInit();
            grpDetail.SuspendLayout();
            SuspendLayout();

            panelHeader.BackColor = Color.FromArgb(20, 55, 90);
            panelHeader.Controls.Add(lblTitle);
            panelHeader.Controls.Add(lblUser);
            panelHeader.Controls.Add(lblCount);
            panelHeader.Controls.Add(lblTotal);
            panelHeader.Dock = DockStyle.Top;
            panelHeader.Height = 90;

            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI Semibold", 18F, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(18, 12);
            lblTitle.Text = "PO Approval";

            lblUser.AutoSize = true;
            lblUser.ForeColor = Color.FromArgb(190, 210, 230);
            lblUser.Location = new Point(20, 50);
            lblUser.Text = "User";

            lblCount.AutoSize = true;
            lblCount.ForeColor = Color.White;
            lblCount.Location = new Point(520, 20);
            lblCount.Text = "Pending PO lines: 0";

            lblTotal.AutoSize = true;
            lblTotal.ForeColor = Color.FromArgb(200, 230, 200);
            lblTotal.Location = new Point(520, 48);
            lblTotal.Text = "Pending value: ₹ 0.00";

            dgvPOs.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvPOs.BackgroundColor = Color.White;
            dgvPOs.Location = new Point(16, 106);
            dgvPOs.Name = "dgvPOs";
            dgvPOs.Size = new Size(700, 430);
            dgvPOs.SelectionChanged += dgvPOs_SelectionChanged;

            grpDetail.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            grpDetail.Controls.Add(lblPOId);
            grpDetail.Controls.Add(lblPRRef);
            grpDetail.Controls.Add(lblProject);
            grpDetail.Controls.Add(lblVendor);
            grpDetail.Controls.Add(lblItem);
            grpDetail.Controls.Add(lblAmount);
            grpDetail.Controls.Add(lblPMStatus);
            grpDetail.Controls.Add(lblMHStatus);
            grpDetail.Controls.Add(lblRemarks);
            grpDetail.Controls.Add(txtRemarks);
            grpDetail.Controls.Add(btnPMApprove);
            grpDetail.Controls.Add(btnMHApprove);
            grpDetail.Controls.Add(btnFinalApprove);
            grpDetail.Controls.Add(btnReject);
            grpDetail.Location = new Point(732, 106);
            grpDetail.Name = "grpDetail";
            grpDetail.Size = new Size(320, 430);
            grpDetail.Text = "Approval Actions";

            int y = 28;
            foreach (var lbl in new[] { lblPOId, lblPRRef, lblProject, lblVendor, lblItem, lblAmount, lblPMStatus, lblMHStatus })
            {
                lbl.AutoSize = true;
                lbl.Location = new Point(14, y);
                y += 24;
            }
            lblPOId.Text = "PO ID: -";
            lblPRRef.Text = "PR Ref: -";
            lblProject.Text = "Project: -";
            lblVendor.Text = "Vendor: -";
            lblItem.Text = "Item: -";
            lblAmount.Text = "Amount: -";
            lblPMStatus.Text = "PM: -";
            lblMHStatus.Text = "MH: -";

            lblRemarks.AutoSize = true;
            lblRemarks.Location = new Point(14, 230);
            lblRemarks.Text = "Remarks";

            txtRemarks.Location = new Point(14, 250);
            txtRemarks.Multiline = true;
            txtRemarks.Size = new Size(290, 60);

            btnPMApprove.Location = new Point(14, 325);
            btnPMApprove.Size = new Size(140, 32);
            btnPMApprove.Text = "PM Approve";
            btnPMApprove.Click += btnPMApprove_Click;

            btnMHApprove.Location = new Point(164, 325);
            btnMHApprove.Size = new Size(140, 32);
            btnMHApprove.Text = "MH Approve";
            btnMHApprove.Click += btnMHApprove_Click;

            btnFinalApprove.BackColor = Color.FromArgb(40, 120, 90);
            btnFinalApprove.FlatStyle = FlatStyle.Flat;
            btnFinalApprove.ForeColor = Color.White;
            btnFinalApprove.Location = new Point(14, 368);
            btnFinalApprove.Size = new Size(140, 36);
            btnFinalApprove.Text = "Final Approve";
            btnFinalApprove.Click += btnFinalApprove_Click;

            btnReject.BackColor = Color.FromArgb(160, 60, 60);
            btnReject.FlatStyle = FlatStyle.Flat;
            btnReject.ForeColor = Color.White;
            btnReject.Location = new Point(164, 368);
            btnReject.Size = new Size(140, 36);
            btnReject.Text = "Reject";
            btnReject.Click += btnReject_Click;

            btnRefresh.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnRefresh.Location = new Point(16, 550);
            btnRefresh.Size = new Size(110, 34);
            btnRefresh.Text = "Refresh";
            btnRefresh.Click += btnRefresh_Click;

            btnClose.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnClose.Location = new Point(952, 550);
            btnClose.Size = new Size(100, 34);
            btnClose.Text = "Close";
            btnClose.Click += btnClose_Click;

            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(240, 244, 248);
            ClientSize = new Size(1070, 600);
            Controls.Add(btnClose);
            Controls.Add(btnRefresh);
            Controls.Add(grpDetail);
            Controls.Add(dgvPOs);
            Controls.Add(panelHeader);
            Name = "frmPOApproval";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "PO Approval";
            Load += frmPOApproval_Load;

            panelHeader.ResumeLayout(false);
            panelHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvPOs).EndInit();
            grpDetail.ResumeLayout(false);
            grpDetail.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel panelHeader;
        private Label lblTitle;
        private Label lblUser;
        private Label lblCount;
        private Label lblTotal;
        private DataGridView dgvPOs;
        private GroupBox grpDetail;
        private Label lblPOId;
        private Label lblPRRef;
        private Label lblProject;
        private Label lblVendor;
        private Label lblItem;
        private Label lblAmount;
        private Label lblPMStatus;
        private Label lblMHStatus;
        private Label lblRemarks;
        private TextBox txtRemarks;
        private Button btnPMApprove;
        private Button btnMHApprove;
        private Button btnFinalApprove;
        private Button btnReject;
        private Button btnRefresh;
        private Button btnClose;
    }
}
