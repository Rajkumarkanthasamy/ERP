namespace WinFormsApp1
{
    partial class frmPriceVariance
    {
        private System.ComponentModel.IContainer components = null;
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            lblTitle = new Label();
            lblPR = new Label();
            txtPRNumber = new TextBox();
            btnLoad = new Button();
            lblSummary = new Label();
            dgvVariance = new DataGridView();
            btnPrint = new Button();
            btnClose = new Button();
            ((System.ComponentModel.ISupportInitialize)dgvVariance).BeginInit();
            SuspendLayout();

            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI Semibold", 16F, FontStyle.Bold);
            lblTitle.Location = new Point(16, 14);
            lblTitle.Text = "Price Variance / Last-PO Compare";

            lblPR.AutoSize = true;
            lblPR.Location = new Point(18, 58);
            lblPR.Text = "PR Number";

            txtPRNumber.Location = new Point(100, 54);
            txtPRNumber.Size = new Size(220, 23);

            btnLoad.Location = new Point(330, 52);
            btnLoad.Size = new Size(100, 28);
            btnLoad.Text = "Analyze";
            btnLoad.Click += btnLoad_Click;

            lblSummary.AutoSize = true;
            lblSummary.Location = new Point(450, 58);
            lblSummary.Text = "0 item(s)";

            dgvVariance.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvVariance.BackgroundColor = Color.White;
            dgvVariance.Location = new Point(16, 96);
            dgvVariance.Size = new Size(850, 400);

            btnPrint.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            btnPrint.BackColor = Color.FromArgb(20, 90, 140);
            btnPrint.FlatStyle = FlatStyle.Flat;
            btnPrint.ForeColor = Color.White;
            btnPrint.Location = new Point(16, 512);
            btnPrint.Size = new Size(120, 34);
            btnPrint.Text = "Print Report";
            btnPrint.Click += btnPrint_Click;

            btnClose.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnClose.Location = new Point(766, 512);
            btnClose.Size = new Size(100, 34);
            btnClose.Text = "Close";
            btnClose.Click += btnClose_Click;

            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(240, 244, 248);
            ClientSize = new Size(884, 560);
            Controls.AddRange(new Control[] { lblTitle, lblPR, txtPRNumber, btnLoad, lblSummary, dgvVariance, btnPrint, btnClose });
            Name = "frmPriceVariance";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Price Variance";
            Load += frmPriceVariance_Load;
            ((System.ComponentModel.ISupportInitialize)dgvVariance).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        private Label lblTitle;
        private Label lblPR;
        private TextBox txtPRNumber;
        private Button btnLoad;
        private Label lblSummary;
        private DataGridView dgvVariance;
        private Button btnPrint;
        private Button btnClose;
    }
}
