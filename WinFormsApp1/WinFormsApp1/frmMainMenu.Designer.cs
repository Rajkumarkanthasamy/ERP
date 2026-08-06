namespace WinFormsApp1
{
    partial class frmMainMenu
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
            lblTitle = new Label();
            lblUser = new Label();
            lblFlow = new Label();
            btnDashboard = new Button();
            btnPRGeneration = new Button();
            btnPRApproval = new Button();
            btnPRClubbing = new Button();
            btnPRtoPO = new Button();
            btnWorkspace = new Button();
            btnPOApproval = new Button();
            btnItemCodeCreation = new Button();
            btnItemCodeApproval = new Button();
            btnLogout = new Button();
            btnExit = new Button();
            panelHeader = new Panel();
            panelHeader.SuspendLayout();
            SuspendLayout();

            panelHeader.BackColor = Color.FromArgb(20, 55, 90);
            panelHeader.Controls.Add(lblTitle);
            panelHeader.Controls.Add(lblUser);
            panelHeader.Controls.Add(lblFlow);
            panelHeader.Dock = DockStyle.Top;
            panelHeader.Size = new Size(900, 110);

            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI Semibold", 20F, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(24, 18);
            lblTitle.Text = "ERP Procurement Hub";

            lblUser.AutoSize = true;
            lblUser.Font = new Font("Segoe UI", 10F);
            lblUser.ForeColor = Color.FromArgb(200, 220, 235);
            lblUser.Location = new Point(28, 62);
            lblUser.Text = "User: -";

            lblFlow.AutoSize = true;
            lblFlow.Font = new Font("Segoe UI", 9F);
            lblFlow.ForeColor = Color.FromArgb(170, 195, 215);
            lblFlow.Location = new Point(28, 84);
            lblFlow.Text = "Flow";

            void StyleTile(Button b, string text, int x, int y, EventHandler click, bool primary = false)
            {
                b.BackColor = primary ? Color.FromArgb(20, 90, 140) : Color.White;
                b.ForeColor = primary ? Color.White : Color.Black;
                b.FlatAppearance.BorderColor = Color.FromArgb(200, 210, 220);
                b.FlatStyle = FlatStyle.Flat;
                b.Font = new Font("Segoe UI Semibold", 10.5F, FontStyle.Bold);
                b.Location = new Point(x, y);
                b.Size = new Size(400, 64);
                b.Text = text;
                b.TextAlign = ContentAlignment.MiddleLeft;
                b.UseVisualStyleBackColor = false;
                b.Click += click;
            }

            StyleTile(btnDashboard, "  0. Procurement Dashboard / Inbox", 40, 130, btnDashboard_Click, true);
            StyleTile(btnPRGeneration, "  1. Purchase Request Generation  (BOM → PR, auto-split 12L)", 40, 205, btnPRGeneration_Click);
            StyleTile(btnPRApproval, "  2. Purchase Request Approval", 460, 205, btnPRApproval_Click);
            StyleTile(btnPRClubbing, "  3. PR Clubbing", 40, 280, btnPRClubbing_Click);
            StyleTile(btnPRtoPO, "  4. PR → PO Conversion (classic)", 460, 280, btnPRtoPO_Click);
            StyleTile(btnWorkspace, "  5. Drag & Drop PR → PO Workspace", 40, 355, btnWorkspace_Click, true);
            StyleTile(btnPOApproval, "  6. PO Approval (PM / MH / Final)", 460, 355, btnPOApproval_Click, true);
            StyleTile(btnItemCodeCreation, "  7. Item Code Creation", 40, 430, btnItemCodeCreation_Click);
            StyleTile(btnItemCodeApproval, "  8. Item Code Approval", 460, 430, btnItemCodeApproval_Click);

            btnLogout.FlatStyle = FlatStyle.Flat;
            btnLogout.Font = new Font("Segoe UI", 10F);
            btnLogout.Location = new Point(40, 520);
            btnLogout.Size = new Size(170, 40);
            btnLogout.Text = "Logout";
            btnLogout.Click += btnLogout_Click;

            btnExit.BackColor = Color.FromArgb(20, 90, 140);
            btnExit.FlatAppearance.BorderSize = 0;
            btnExit.FlatStyle = FlatStyle.Flat;
            btnExit.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            btnExit.ForeColor = Color.White;
            btnExit.Location = new Point(690, 520);
            btnExit.Size = new Size(170, 40);
            btnExit.Text = "Exit";
            btnExit.Click += btnExit_Click;

            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(240, 244, 248);
            ClientSize = new Size(900, 590);
            Controls.Add(btnExit);
            Controls.Add(btnLogout);
            Controls.Add(btnItemCodeApproval);
            Controls.Add(btnItemCodeCreation);
            Controls.Add(btnPOApproval);
            Controls.Add(btnWorkspace);
            Controls.Add(btnPRtoPO);
            Controls.Add(btnPRClubbing);
            Controls.Add(btnPRApproval);
            Controls.Add(btnPRGeneration);
            Controls.Add(btnDashboard);
            Controls.Add(panelHeader);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "frmMainMenu";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "ERP Main Menu";
            Load += frmMainMenu_Load;
            panelHeader.ResumeLayout(false);
            panelHeader.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel panelHeader;
        private Label lblTitle;
        private Label lblUser;
        private Label lblFlow;
        private Button btnDashboard;
        private Button btnPRGeneration;
        private Button btnPRApproval;
        private Button btnPRClubbing;
        private Button btnPRtoPO;
        private Button btnWorkspace;
        private Button btnPOApproval;
        private Button btnItemCodeCreation;
        private Button btnItemCodeApproval;
        private Button btnLogout;
        private Button btnExit;
    }
}
