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
            panelHeader = new Panel();
            panelBody = new Panel();
            btnDashboard = new Button();
            btnKanban = new Button();
            btnPRGeneration = new Button();
            btnPRApproval = new Button();
            btnPRClubbing = new Button();
            btnPRtoPO = new Button();
            btnWorkspace = new Button();
            btnPOApproval = new Button();
            btnGRN = new Button();
            btnPriceVariance = new Button();
            btnItemCodeCreation = new Button();
            btnItemCodeApproval = new Button();
            btnLogout = new Button();
            btnExit = new Button();
            panelHeader.SuspendLayout();
            panelBody.SuspendLayout();
            SuspendLayout();

            panelHeader.BackColor = Color.FromArgb(20, 55, 90);
            panelHeader.Controls.Add(lblTitle);
            panelHeader.Controls.Add(lblUser);
            panelHeader.Controls.Add(lblFlow);
            panelHeader.Dock = DockStyle.Top;
            panelHeader.Size = new Size(920, 100);

            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI Semibold", 20F, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(24, 14);
            lblTitle.Text = "ERP Procurement Hub";

            lblUser.AutoSize = true;
            lblUser.Font = new Font("Segoe UI", 10F);
            lblUser.ForeColor = Color.FromArgb(200, 220, 235);
            lblUser.Location = new Point(28, 54);
            lblUser.Text = "User: -";

            lblFlow.AutoSize = true;
            lblFlow.Font = new Font("Segoe UI", 8.5F);
            lblFlow.ForeColor = Color.FromArgb(170, 195, 215);
            lblFlow.Location = new Point(28, 76);
            lblFlow.Text = "Flow";

            panelBody.AutoScroll = true;
            panelBody.Dock = DockStyle.Fill;
            panelBody.Padding = new Padding(20);

            void StyleTile(Button b, string text, int x, int y, EventHandler click, bool primary = false)
            {
                b.BackColor = primary ? Color.FromArgb(20, 90, 140) : Color.White;
                b.ForeColor = primary ? Color.White : Color.Black;
                b.FlatAppearance.BorderColor = Color.FromArgb(200, 210, 220);
                b.FlatStyle = FlatStyle.Flat;
                b.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
                b.Location = new Point(x, y);
                b.Size = new Size(420, 58);
                b.Text = text;
                b.TextAlign = ContentAlignment.MiddleLeft;
                b.UseVisualStyleBackColor = false;
                b.Click += click;
                panelBody.Controls.Add(b);
            }

            StyleTile(btnDashboard, "  Dashboard / Inbox", 20, 16, btnDashboard_Click, true);
            StyleTile(btnKanban, "  Kanban Pipeline Board (drag stages)", 460, 16, btnKanban_Click, true);
            StyleTile(btnPRGeneration, "  1. PR Generation (BOM → PR, 12L auto-split)", 20, 86, btnPRGeneration_Click);
            StyleTile(btnPRApproval, "  2. PR Approval", 460, 86, btnPRApproval_Click);
            StyleTile(btnPRClubbing, "  3. PR Clubbing", 20, 156, btnPRClubbing_Click);
            StyleTile(btnWorkspace, "  4. Drag & Drop PR → PO Workspace", 460, 156, btnWorkspace_Click, true);
            StyleTile(btnPRtoPO, "  5. Classic PR → PO Conversion", 20, 226, btnPRtoPO_Click);
            StyleTile(btnPOApproval, "  6. PO Approval (PM / MH / Final)", 460, 226, btnPOApproval_Click, true);
            StyleTile(btnGRN, "  7. Goods Receipt (GRN)", 20, 296, btnGRN_Click, true);
            StyleTile(btnPriceVariance, "  8. Price Variance / Last-PO Compare", 460, 296, btnPriceVariance_Click, true);
            StyleTile(btnItemCodeCreation, "  9. Item Code Creation", 20, 366, btnItemCodeCreation_Click);
            StyleTile(btnItemCodeApproval, "  10. Item Code Approval", 460, 366, btnItemCodeApproval_Click);

            btnLogout.FlatStyle = FlatStyle.Flat;
            btnLogout.Location = new Point(20, 450);
            btnLogout.Size = new Size(160, 38);
            btnLogout.Text = "Logout";
            btnLogout.Click += btnLogout_Click;
            panelBody.Controls.Add(btnLogout);

            btnExit.BackColor = Color.FromArgb(20, 90, 140);
            btnExit.FlatStyle = FlatStyle.Flat;
            btnExit.ForeColor = Color.White;
            btnExit.Location = new Point(720, 450);
            btnExit.Size = new Size(160, 38);
            btnExit.Text = "Exit";
            btnExit.Click += btnExit_Click;
            panelBody.Controls.Add(btnExit);

            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(240, 244, 248);
            ClientSize = new Size(920, 640);
            Controls.Add(panelBody);
            Controls.Add(panelHeader);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "frmMainMenu";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "ERP Main Menu";
            Load += frmMainMenu_Load;
            panelHeader.ResumeLayout(false);
            panelHeader.PerformLayout();
            panelBody.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel panelHeader;
        private Panel panelBody;
        private Label lblTitle;
        private Label lblUser;
        private Label lblFlow;
        private Button btnDashboard;
        private Button btnKanban;
        private Button btnPRGeneration;
        private Button btnPRApproval;
        private Button btnPRClubbing;
        private Button btnPRtoPO;
        private Button btnWorkspace;
        private Button btnPOApproval;
        private Button btnGRN;
        private Button btnPriceVariance;
        private Button btnItemCodeCreation;
        private Button btnItemCodeApproval;
        private Button btnLogout;
        private Button btnExit;
    }
}
