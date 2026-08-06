namespace WinFormsApp1
{
    partial class frmKanbanBoard
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            panelHeader = new Panel();
            lblTitle = new Label();
            lblUser = new Label();
            lblCount = new Label();
            flpBoard = new FlowLayoutPanel();
            btnRefresh = new Button();
            btnClose = new Button();
            panelHeader.SuspendLayout();
            SuspendLayout();

            panelHeader.BackColor = Color.FromArgb(20, 55, 90);
            panelHeader.Controls.Add(lblTitle);
            panelHeader.Controls.Add(lblUser);
            panelHeader.Controls.Add(lblCount);
            panelHeader.Dock = DockStyle.Top;
            panelHeader.Height = 78;

            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI Semibold", 18F, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(18, 12);
            lblTitle.Text = "Procurement Kanban Board";

            lblUser.AutoSize = true;
            lblUser.ForeColor = Color.FromArgb(190, 210, 230);
            lblUser.Location = new Point(20, 48);
            lblUser.Text = "User";

            lblCount.AutoSize = true;
            lblCount.ForeColor = Color.White;
            lblCount.Location = new Point(900, 28);
            lblCount.Text = "Cards: 0";

            flpBoard.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            flpBoard.AutoScroll = true;
            flpBoard.Location = new Point(12, 90);
            flpBoard.Name = "flpBoard";
            flpBoard.Size = new Size(1160, 540);
            flpBoard.WrapContents = false;

            btnRefresh.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnRefresh.Location = new Point(12, 645);
            btnRefresh.Size = new Size(120, 34);
            btnRefresh.Text = "Refresh";
            btnRefresh.Click += btnRefresh_Click;

            btnClose.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnClose.Location = new Point(1072, 645);
            btnClose.Size = new Size(100, 34);
            btnClose.Text = "Close";
            btnClose.Click += btnClose_Click;

            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(240, 244, 248);
            ClientSize = new Size(1184, 690);
            Controls.Add(btnClose);
            Controls.Add(btnRefresh);
            Controls.Add(flpBoard);
            Controls.Add(panelHeader);
            Name = "frmKanbanBoard";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Kanban Board";
            Load += frmKanbanBoard_Load;
            panelHeader.ResumeLayout(false);
            panelHeader.PerformLayout();
            ResumeLayout(false);
        }

        private Panel panelHeader;
        private Label lblTitle;
        private Label lblUser;
        private Label lblCount;
        private FlowLayoutPanel flpBoard;
        private Button btnRefresh;
        private Button btnClose;
    }
}
