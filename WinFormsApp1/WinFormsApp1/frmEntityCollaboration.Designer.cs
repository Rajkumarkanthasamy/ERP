namespace WinFormsApp1
{
    partial class frmEntityCollaboration
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            lblHeader = new Label();
            lblComments = new Label();
            dgvComments = new DataGridView();
            txtComment = new TextBox();
            btnAddComment = new Button();
            lblFiles = new Label();
            dgvFiles = new DataGridView();
            btnAttach = new Button();
            btnOpenFile = new Button();
            btnPrint = new Button();
            btnClose = new Button();
            ((System.ComponentModel.ISupportInitialize)dgvComments).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvFiles).BeginInit();
            SuspendLayout();

            lblHeader.AutoSize = true;
            lblHeader.Font = new Font("Segoe UI Semibold", 14F, FontStyle.Bold);
            lblHeader.Location = new Point(16, 14);
            lblHeader.Text = "Entity";

            lblComments.AutoSize = true;
            lblComments.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            lblComments.Location = new Point(16, 50);
            lblComments.Text = "Comments / @mentions thread";

            dgvComments.Location = new Point(16, 74);
            dgvComments.Size = new Size(650, 180);
            dgvComments.BackgroundColor = Color.White;

            txtComment.Location = new Point(16, 264);
            txtComment.Multiline = true;
            txtComment.Size = new Size(520, 50);

            btnAddComment.Location = new Point(546, 264);
            btnAddComment.Size = new Size(120, 50);
            btnAddComment.Text = "Add Comment";
            btnAddComment.Click += btnAddComment_Click;

            lblFiles.AutoSize = true;
            lblFiles.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            lblFiles.Location = new Point(16, 330);
            lblFiles.Text = "Attachments (drawings, quotes, emails)";

            dgvFiles.Location = new Point(16, 354);
            dgvFiles.Size = new Size(650, 150);
            dgvFiles.BackgroundColor = Color.White;

            btnAttach.Location = new Point(16, 518);
            btnAttach.Size = new Size(120, 32);
            btnAttach.Text = "Attach File";
            btnAttach.Click += btnAttach_Click;

            btnOpenFile.Location = new Point(146, 518);
            btnOpenFile.Size = new Size(120, 32);
            btnOpenFile.Text = "Open File";
            btnOpenFile.Click += btnOpenFile_Click;

            btnPrint.BackColor = Color.FromArgb(20, 90, 140);
            btnPrint.FlatStyle = FlatStyle.Flat;
            btnPrint.ForeColor = Color.White;
            btnPrint.Location = new Point(426, 518);
            btnPrint.Size = new Size(120, 32);
            btnPrint.Text = "Print Pack";
            btnPrint.Click += btnPrint_Click;

            btnClose.Location = new Point(556, 518);
            btnClose.Size = new Size(110, 32);
            btnClose.Text = "Close";
            btnClose.Click += btnClose_Click;

            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(240, 244, 248);
            ClientSize = new Size(690, 565);
            Controls.AddRange(new Control[] {
                lblHeader, lblComments, dgvComments, txtComment, btnAddComment,
                lblFiles, dgvFiles, btnAttach, btnOpenFile, btnPrint, btnClose });
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            Name = "frmEntityCollaboration";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Collaboration";
            Load += frmEntityCollaboration_Load;
            ((System.ComponentModel.ISupportInitialize)dgvComments).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvFiles).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        private Label lblHeader;
        private Label lblComments;
        private DataGridView dgvComments;
        private TextBox txtComment;
        private Button btnAddComment;
        private Label lblFiles;
        private DataGridView dgvFiles;
        private Button btnAttach;
        private Button btnOpenFile;
        private Button btnPrint;
        private Button btnClose;
    }
}
