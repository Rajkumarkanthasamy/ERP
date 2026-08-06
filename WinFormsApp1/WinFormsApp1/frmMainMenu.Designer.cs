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
            btnPRGeneration = new Button();
            btnPRApproval = new Button();
            btnPRClubbing = new Button();
            btnPRtoPO = new Button();
            btnItemCodeCreation = new Button();
            btnItemCodeApproval = new Button();
            btnLogout = new Button();
            btnExit = new Button();
            panelHeader = new Panel();
            panelHeader.SuspendLayout();
            SuspendLayout();
            //
            // panelHeader
            //
            panelHeader.BackColor = Color.FromArgb(20, 55, 90);
            panelHeader.Controls.Add(lblTitle);
            panelHeader.Controls.Add(lblUser);
            panelHeader.Controls.Add(lblFlow);
            panelHeader.Dock = DockStyle.Top;
            panelHeader.Location = new Point(0, 0);
            panelHeader.Name = "panelHeader";
            panelHeader.Size = new Size(860, 110);
            panelHeader.TabIndex = 0;
            //
            // lblTitle
            //
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI Semibold", 20F, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(24, 18);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(320, 37);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "ERP Procurement Hub";
            //
            // lblUser
            //
            lblUser.AutoSize = true;
            lblUser.Font = new Font("Segoe UI", 10F);
            lblUser.ForeColor = Color.FromArgb(200, 220, 235);
            lblUser.Location = new Point(28, 62);
            lblUser.Name = "lblUser";
            lblUser.Size = new Size(80, 19);
            lblUser.TabIndex = 1;
            lblUser.Text = "User: -";
            //
            // lblFlow
            //
            lblFlow.AutoSize = true;
            lblFlow.Font = new Font("Segoe UI", 9F);
            lblFlow.ForeColor = Color.FromArgb(170, 195, 215);
            lblFlow.Location = new Point(28, 84);
            lblFlow.Name = "lblFlow";
            lblFlow.Size = new Size(300, 15);
            lblFlow.TabIndex = 2;
            lblFlow.Text = "Flow";
            //
            // btnPRGeneration
            //
            btnPRGeneration.BackColor = Color.White;
            btnPRGeneration.FlatAppearance.BorderColor = Color.FromArgb(200, 210, 220);
            btnPRGeneration.FlatStyle = FlatStyle.Flat;
            btnPRGeneration.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
            btnPRGeneration.Location = new Point(40, 140);
            btnPRGeneration.Name = "btnPRGeneration";
            btnPRGeneration.Size = new Size(370, 70);
            btnPRGeneration.TabIndex = 1;
            btnPRGeneration.Text = "1. Purchase Request Generation\n(BOM → PR)";
            btnPRGeneration.TextAlign = ContentAlignment.MiddleLeft;
            btnPRGeneration.UseVisualStyleBackColor = false;
            btnPRGeneration.Click += btnPRGeneration_Click;
            //
            // btnPRApproval
            //
            btnPRApproval.BackColor = Color.White;
            btnPRApproval.FlatAppearance.BorderColor = Color.FromArgb(200, 210, 220);
            btnPRApproval.FlatStyle = FlatStyle.Flat;
            btnPRApproval.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
            btnPRApproval.Location = new Point(450, 140);
            btnPRApproval.Name = "btnPRApproval";
            btnPRApproval.Size = new Size(370, 70);
            btnPRApproval.TabIndex = 2;
            btnPRApproval.Text = "2. Purchase Request Approval\n(Approve / Reject / Hold)";
            btnPRApproval.TextAlign = ContentAlignment.MiddleLeft;
            btnPRApproval.UseVisualStyleBackColor = false;
            btnPRApproval.Click += btnPRApproval_Click;
            //
            // btnPRClubbing
            //
            btnPRClubbing.BackColor = Color.White;
            btnPRClubbing.FlatAppearance.BorderColor = Color.FromArgb(200, 210, 220);
            btnPRClubbing.FlatStyle = FlatStyle.Flat;
            btnPRClubbing.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
            btnPRClubbing.Location = new Point(40, 230);
            btnPRClubbing.Name = "btnPRClubbing";
            btnPRClubbing.Size = new Size(370, 70);
            btnPRClubbing.TabIndex = 3;
            btnPRClubbing.Text = "3. PR Clubbing\n(Merge approved PRs under 12L)";
            btnPRClubbing.TextAlign = ContentAlignment.MiddleLeft;
            btnPRClubbing.UseVisualStyleBackColor = false;
            btnPRClubbing.Click += btnPRClubbing_Click;
            //
            // btnPRtoPO
            //
            btnPRtoPO.BackColor = Color.White;
            btnPRtoPO.FlatAppearance.BorderColor = Color.FromArgb(200, 210, 220);
            btnPRtoPO.FlatStyle = FlatStyle.Flat;
            btnPRtoPO.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
            btnPRtoPO.Location = new Point(450, 230);
            btnPRtoPO.Name = "btnPRtoPO";
            btnPRtoPO.Size = new Size(370, 70);
            btnPRtoPO.TabIndex = 4;
            btnPRtoPO.Text = "4. PR → PO Conversion\n(Convert approved lines to PO)";
            btnPRtoPO.TextAlign = ContentAlignment.MiddleLeft;
            btnPRtoPO.UseVisualStyleBackColor = false;
            btnPRtoPO.Click += btnPRtoPO_Click;
            //
            // btnItemCodeCreation
            //
            btnItemCodeCreation.BackColor = Color.White;
            btnItemCodeCreation.FlatAppearance.BorderColor = Color.FromArgb(200, 210, 220);
            btnItemCodeCreation.FlatStyle = FlatStyle.Flat;
            btnItemCodeCreation.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
            btnItemCodeCreation.Location = new Point(40, 320);
            btnItemCodeCreation.Name = "btnItemCodeCreation";
            btnItemCodeCreation.Size = new Size(370, 70);
            btnItemCodeCreation.TabIndex = 5;
            btnItemCodeCreation.Text = "5. Item Code Creation\n(ICCRF request)";
            btnItemCodeCreation.TextAlign = ContentAlignment.MiddleLeft;
            btnItemCodeCreation.UseVisualStyleBackColor = false;
            btnItemCodeCreation.Click += btnItemCodeCreation_Click;
            //
            // btnItemCodeApproval
            //
            btnItemCodeApproval.BackColor = Color.White;
            btnItemCodeApproval.FlatAppearance.BorderColor = Color.FromArgb(200, 210, 220);
            btnItemCodeApproval.FlatStyle = FlatStyle.Flat;
            btnItemCodeApproval.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
            btnItemCodeApproval.Location = new Point(450, 320);
            btnItemCodeApproval.Name = "btnItemCodeApproval";
            btnItemCodeApproval.Size = new Size(370, 70);
            btnItemCodeApproval.TabIndex = 6;
            btnItemCodeApproval.Text = "6. Item Code Approval\n(Approve new item masters)";
            btnItemCodeApproval.TextAlign = ContentAlignment.MiddleLeft;
            btnItemCodeApproval.UseVisualStyleBackColor = false;
            btnItemCodeApproval.Click += btnItemCodeApproval_Click;
            //
            // btnLogout
            //
            btnLogout.FlatStyle = FlatStyle.Flat;
            btnLogout.Font = new Font("Segoe UI", 10F);
            btnLogout.Location = new Point(40, 420);
            btnLogout.Name = "btnLogout";
            btnLogout.Size = new Size(170, 40);
            btnLogout.TabIndex = 7;
            btnLogout.Text = "Logout";
            btnLogout.UseVisualStyleBackColor = true;
            btnLogout.Click += btnLogout_Click;
            //
            // btnExit
            //
            btnExit.BackColor = Color.FromArgb(20, 90, 140);
            btnExit.FlatAppearance.BorderSize = 0;
            btnExit.FlatStyle = FlatStyle.Flat;
            btnExit.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            btnExit.ForeColor = Color.White;
            btnExit.Location = new Point(650, 420);
            btnExit.Name = "btnExit";
            btnExit.Size = new Size(170, 40);
            btnExit.TabIndex = 8;
            btnExit.Text = "Exit";
            btnExit.UseVisualStyleBackColor = false;
            btnExit.Click += btnExit_Click;
            //
            // frmMainMenu
            //
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(240, 244, 248);
            ClientSize = new Size(860, 490);
            Controls.Add(btnExit);
            Controls.Add(btnLogout);
            Controls.Add(btnItemCodeApproval);
            Controls.Add(btnItemCodeCreation);
            Controls.Add(btnPRtoPO);
            Controls.Add(btnPRClubbing);
            Controls.Add(btnPRApproval);
            Controls.Add(btnPRGeneration);
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
        private Button btnPRGeneration;
        private Button btnPRApproval;
        private Button btnPRClubbing;
        private Button btnPRtoPO;
        private Button btnItemCodeCreation;
        private Button btnItemCodeApproval;
        private Button btnLogout;
        private Button btnExit;
    }
}
