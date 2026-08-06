namespace WinFormsApp1
{
    partial class frmGRN
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
            lblOpen = new Label();
            lblOpenCount = new Label();
            dgvOpenPO = new DataGridView();
            btnAddSelected = new Button();
            btnRefresh = new Button();
            lblReceive = new Label();
            lblGRN = new Label();
            txtGRNNumber = new TextBox();
            lblReceivedBy = new Label();
            txtReceivedBy = new TextBox();
            lblRemarks = new Label();
            txtRemarks = new TextBox();
            dgvReceive = new DataGridView();
            colSelect = new DataGridViewCheckBoxColumn();
            colPOID = new DataGridViewTextBoxColumn();
            colPORef = new DataGridViewTextBoxColumn();
            colItem = new DataGridViewTextBoxColumn();
            colOrdered = new DataGridViewTextBoxColumn();
            colRemaining = new DataGridViewTextBoxColumn();
            colRecvQty = new DataGridViewTextBoxColumn();
            colUnitPrice = new DataGridViewTextBoxColumn();
            colAmount = new DataGridViewTextBoxColumn();
            colUOM = new DataGridViewTextBoxColumn();
            colVendor = new DataGridViewTextBoxColumn();
            colProject = new DataGridViewTextBoxColumn();
            colRemarks = new DataGridViewTextBoxColumn();
            lblReceiveTotal = new Label();
            btnPostGRN = new Button();
            btnClear = new Button();
            lblHistory = new Label();
            dgvHistory = new DataGridView();
            btnCollaborate = new Button();
            btnClose = new Button();
            panelHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvOpenPO).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvReceive).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvHistory).BeginInit();
            SuspendLayout();

            panelHeader.BackColor = Color.FromArgb(20, 55, 90);
            panelHeader.Controls.Add(lblTitle);
            panelHeader.Controls.Add(lblUser);
            panelHeader.Dock = DockStyle.Top;
            panelHeader.Height = 72;

            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI Semibold", 18F, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(16, 10);
            lblTitle.Text = "Goods Receipt (GRN) against PO";

            lblUser.AutoSize = true;
            lblUser.ForeColor = Color.FromArgb(190, 210, 230);
            lblUser.Location = new Point(18, 44);
            lblUser.Text = "User";

            lblOpen.AutoSize = true;
            lblOpen.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            lblOpen.Location = new Point(16, 84);
            lblOpen.Text = "1. Open approved PO lines";

            lblOpenCount.AutoSize = true;
            lblOpenCount.Location = new Point(250, 86);
            lblOpenCount.Text = "Open: 0";

            dgvOpenPO.Location = new Point(16, 108);
            dgvOpenPO.Size = new Size(1050, 160);
            dgvOpenPO.BackgroundColor = Color.White;

            btnAddSelected.Location = new Point(16, 276);
            btnAddSelected.Size = new Size(150, 30);
            btnAddSelected.Text = "Add Selected ↓";
            btnAddSelected.Click += btnAddSelected_Click;

            btnRefresh.Location = new Point(176, 276);
            btnRefresh.Size = new Size(100, 30);
            btnRefresh.Text = "Refresh";
            btnRefresh.Click += btnRefresh_Click;

            lblReceive.AutoSize = true;
            lblReceive.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            lblReceive.Location = new Point(16, 316);
            lblReceive.Text = "2. Receive quantities (partial OK)";

            lblGRN.AutoSize = true;
            lblGRN.Location = new Point(420, 318);
            lblGRN.Text = "GRN #";
            txtGRNNumber.Location = new Point(470, 314);
            txtGRNNumber.ReadOnly = true;
            txtGRNNumber.Size = new Size(160, 23);

            lblReceivedBy.AutoSize = true;
            lblReceivedBy.Location = new Point(650, 318);
            lblReceivedBy.Text = "Received By";
            txtReceivedBy.Location = new Point(730, 314);
            txtReceivedBy.Size = new Size(140, 23);

            lblRemarks.AutoSize = true;
            lblRemarks.Location = new Point(880, 318);
            lblRemarks.Text = "Remarks";
            txtRemarks.Location = new Point(940, 314);
            txtRemarks.Size = new Size(126, 23);

            dgvReceive.AllowUserToAddRows = false;
            dgvReceive.BackgroundColor = Color.White;
            dgvReceive.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvReceive.Columns.AddRange(new DataGridViewColumn[] {
                colSelect, colPOID, colPORef, colItem, colOrdered, colRemaining, colRecvQty,
                colUnitPrice, colAmount, colUOM, colVendor, colProject, colRemarks });
            dgvReceive.Location = new Point(16, 344);
            dgvReceive.Size = new Size(1050, 160);
            dgvReceive.CellEndEdit += dgvReceive_CellEndEdit;

            colSelect.HeaderText = "";
            colSelect.Width = 30;
            colPOID.HeaderText = "POID";
            colPOID.Visible = false;
            colPORef.HeaderText = "PO/PR Ref";
            colPORef.ReadOnly = true;
            colPORef.Width = 110;
            colItem.HeaderText = "Item";
            colItem.ReadOnly = true;
            colItem.Width = 100;
            colOrdered.HeaderText = "Ordered";
            colOrdered.ReadOnly = true;
            colOrdered.Width = 70;
            colRemaining.HeaderText = "Remaining";
            colRemaining.ReadOnly = true;
            colRemaining.Width = 80;
            colRecvQty.HeaderText = "Receive Qty";
            colRecvQty.Width = 80;
            colUnitPrice.HeaderText = "Unit Price";
            colUnitPrice.ReadOnly = true;
            colUnitPrice.Width = 80;
            colAmount.HeaderText = "Amount";
            colAmount.ReadOnly = true;
            colAmount.Width = 80;
            colUOM.HeaderText = "UOM";
            colUOM.ReadOnly = true;
            colUOM.Width = 50;
            colVendor.HeaderText = "Vendor";
            colVendor.ReadOnly = true;
            colVendor.Width = 80;
            colProject.HeaderText = "Project";
            colProject.ReadOnly = true;
            colProject.Width = 90;
            colRemarks.HeaderText = "Remarks";
            colRemarks.Width = 100;

            lblReceiveTotal.AutoSize = true;
            lblReceiveTotal.Location = new Point(16, 514);
            lblReceiveTotal.Text = "Receive lines: 0 · Total: ₹ 0.00";

            btnPostGRN.BackColor = Color.FromArgb(40, 120, 90);
            btnPostGRN.FlatStyle = FlatStyle.Flat;
            btnPostGRN.ForeColor = Color.White;
            btnPostGRN.Location = new Point(800, 508);
            btnPostGRN.Size = new Size(160, 34);
            btnPostGRN.Text = "Post GRN";
            btnPostGRN.Click += btnPostGRN_Click;

            btnClear.Location = new Point(680, 508);
            btnClear.Size = new Size(100, 34);
            btnClear.Text = "Clear";
            btnClear.Click += btnClear_Click;

            lblHistory.AutoSize = true;
            lblHistory.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            lblHistory.Location = new Point(16, 552);
            lblHistory.Text = "3. Recent GRNs";

            dgvHistory.BackgroundColor = Color.White;
            dgvHistory.Location = new Point(16, 576);
            dgvHistory.Size = new Size(1050, 120);

            btnCollaborate.Location = new Point(16, 708);
            btnCollaborate.Size = new Size(160, 32);
            btnCollaborate.Text = "Comments / Files";
            btnCollaborate.Click += btnCollaborate_Click;

            btnClose.Location = new Point(966, 708);
            btnClose.Size = new Size(100, 32);
            btnClose.Text = "Close";
            btnClose.Click += btnClose_Click;

            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(240, 244, 248);
            ClientSize = new Size(1084, 752);
            Controls.AddRange(new Control[] {
                panelHeader, lblOpen, lblOpenCount, dgvOpenPO, btnAddSelected, btnRefresh,
                lblReceive, lblGRN, txtGRNNumber, lblReceivedBy, txtReceivedBy, lblRemarks, txtRemarks,
                dgvReceive, lblReceiveTotal, btnPostGRN, btnClear, lblHistory, dgvHistory,
                btnCollaborate, btnClose });
            Name = "frmGRN";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Goods Receipt Note";
            Load += frmGRN_Load;
            panelHeader.ResumeLayout(false);
            panelHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvOpenPO).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvReceive).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvHistory).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        private Panel panelHeader;
        private Label lblTitle;
        private Label lblUser;
        private Label lblOpen;
        private Label lblOpenCount;
        private DataGridView dgvOpenPO;
        private Button btnAddSelected;
        private Button btnRefresh;
        private Label lblReceive;
        private Label lblGRN;
        private TextBox txtGRNNumber;
        private Label lblReceivedBy;
        private TextBox txtReceivedBy;
        private Label lblRemarks;
        private TextBox txtRemarks;
        private DataGridView dgvReceive;
        private DataGridViewCheckBoxColumn colSelect;
        private DataGridViewTextBoxColumn colPOID;
        private DataGridViewTextBoxColumn colPORef;
        private DataGridViewTextBoxColumn colItem;
        private DataGridViewTextBoxColumn colOrdered;
        private DataGridViewTextBoxColumn colRemaining;
        private DataGridViewTextBoxColumn colRecvQty;
        private DataGridViewTextBoxColumn colUnitPrice;
        private DataGridViewTextBoxColumn colAmount;
        private DataGridViewTextBoxColumn colUOM;
        private DataGridViewTextBoxColumn colVendor;
        private DataGridViewTextBoxColumn colProject;
        private DataGridViewTextBoxColumn colRemarks;
        private Label lblReceiveTotal;
        private Button btnPostGRN;
        private Button btnClear;
        private Label lblHistory;
        private DataGridView dgvHistory;
        private Button btnCollaborate;
        private Button btnClose;
    }
}
