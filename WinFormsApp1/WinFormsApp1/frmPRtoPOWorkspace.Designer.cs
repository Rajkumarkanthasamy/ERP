namespace WinFormsApp1
{
    partial class frmPRtoPOWorkspace
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
            lblSource = new Label();
            lblSourceCount = new Label();
            dgvSourcePRs = new DataGridView();
            btnAddSelected = new Button();
            btnRefresh = new Button();
            panelDropZone = new Panel();
            lblDropHint = new Label();
            lblDropTotal = new Label();
            dgvDropped = new DataGridView();
            colDropPR = new DataGridViewTextBoxColumn();
            colDropVendor = new DataGridViewTextBoxColumn();
            colDropProject = new DataGridViewTextBoxColumn();
            colDropAmount = new DataGridViewTextBoxColumn();
            colDropLines = new DataGridViewTextBoxColumn();
            btnRemoveSelected = new Button();
            btnClearCanvas = new Button();
            lblLines = new Label();
            dgvLines = new DataGridView();
            colLinePR = new DataGridViewTextBoxColumn();
            colLineItem = new DataGridViewTextBoxColumn();
            colLineDesc = new DataGridViewTextBoxColumn();
            colLineQty = new DataGridViewTextBoxColumn();
            colLinePrice = new DataGridViewTextBoxColumn();
            colLineAmount = new DataGridViewTextBoxColumn();
            colLineVendor = new DataGridViewTextBoxColumn();
            grpPO = new GroupBox();
            lblPreparedBy = new Label();
            txtPreparedBy = new TextBox();
            lblAuthorisedBy = new Label();
            txtAuthorisedBy = new TextBox();
            lblDelivery = new Label();
            dtpDelivery = new DateTimePicker();
            lblCurrency = new Label();
            cmbCurrency = new ComboBox();
            lblRemarks = new Label();
            txtRemarks = new TextBox();
            btnGeneratePO = new Button();
            btnOpenPOApproval = new Button();
            btnClose = new Button();
            panelHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvSourcePRs).BeginInit();
            panelDropZone.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvDropped).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvLines).BeginInit();
            grpPO.SuspendLayout();
            SuspendLayout();

            panelHeader.BackColor = Color.FromArgb(20, 55, 90);
            panelHeader.Controls.Add(lblTitle);
            panelHeader.Controls.Add(lblUser);
            panelHeader.Dock = DockStyle.Top;
            panelHeader.Height = 78;

            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI Semibold", 18F, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(18, 12);
            lblTitle.Text = "PR → PO Workspace (Drag & Drop)";

            lblUser.AutoSize = true;
            lblUser.ForeColor = Color.FromArgb(190, 210, 230);
            lblUser.Location = new Point(20, 48);
            lblUser.Text = "User";

            lblSource.AutoSize = true;
            lblSource.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
            lblSource.Location = new Point(16, 92);
            lblSource.Text = "1. Approved PRs — drag onto the canvas →";

            lblSourceCount.AutoSize = true;
            lblSourceCount.Location = new Point(420, 96);
            lblSourceCount.Text = "Approved PRs ready: 0";

            dgvSourcePRs.AllowDrop = false;
            dgvSourcePRs.BackgroundColor = Color.White;
            dgvSourcePRs.Location = new Point(16, 118);
            dgvSourcePRs.Name = "dgvSourcePRs";
            dgvSourcePRs.Size = new Size(520, 280);
            dgvSourcePRs.MouseDown += dgvSourcePRs_MouseDown;
            dgvSourcePRs.MouseMove += dgvSourcePRs_MouseMove;
            dgvSourcePRs.MouseUp += dgvSourcePRs_MouseUp;

            btnAddSelected.Location = new Point(16, 408);
            btnAddSelected.Size = new Size(140, 30);
            btnAddSelected.Text = "Add Selected →";
            btnAddSelected.Click += btnAddSelected_Click;

            btnRefresh.Location = new Point(166, 408);
            btnRefresh.Size = new Size(100, 30);
            btnRefresh.Text = "Refresh";
            btnRefresh.Click += btnRefresh_Click;

            panelDropZone.AllowDrop = true;
            panelDropZone.BackColor = Color.FromArgb(248, 250, 252);
            panelDropZone.BorderStyle = BorderStyle.FixedSingle;
            panelDropZone.Controls.Add(lblDropHint);
            panelDropZone.Controls.Add(lblDropTotal);
            panelDropZone.Controls.Add(dgvDropped);
            panelDropZone.Controls.Add(btnRemoveSelected);
            panelDropZone.Controls.Add(btnClearCanvas);
            panelDropZone.Location = new Point(552, 118);
            panelDropZone.Name = "panelDropZone";
            panelDropZone.Size = new Size(520, 320);
            panelDropZone.DragEnter += panelDropZone_DragEnter;
            panelDropZone.DragLeave += panelDropZone_DragLeave;
            panelDropZone.DragDrop += panelDropZone_DragDrop;

            lblDropHint.AutoSize = true;
            lblDropHint.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
            lblDropHint.ForeColor = Color.FromArgb(20, 90, 140);
            lblDropHint.Location = new Point(12, 10);
            lblDropHint.Text = "Drop approved PRs here to build PO(s)";

            lblDropTotal.AutoSize = true;
            lblDropTotal.Location = new Point(12, 36);
            lblDropTotal.Text = "Canvas total: ₹ 0.00";

            dgvDropped.AllowUserToAddRows = false;
            dgvDropped.BackgroundColor = Color.White;
            dgvDropped.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvDropped.Columns.AddRange(new DataGridViewColumn[] {
                colDropPR, colDropVendor, colDropProject, colDropAmount, colDropLines });
            dgvDropped.Location = new Point(12, 60);
            dgvDropped.Name = "dgvDropped";
            dgvDropped.ReadOnly = true;
            dgvDropped.RowHeadersVisible = false;
            dgvDropped.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvDropped.Size = new Size(490, 210);

            colDropPR.HeaderText = "PR Number";
            colDropPR.Name = "colDropPR";
            colDropPR.Width = 120;
            colDropVendor.HeaderText = "Vendor";
            colDropVendor.Name = "colDropVendor";
            colDropVendor.Width = 110;
            colDropProject.HeaderText = "Project";
            colDropProject.Name = "colDropProject";
            colDropProject.Width = 100;
            colDropAmount.HeaderText = "Amount";
            colDropAmount.Name = "colDropAmount";
            colDropAmount.Width = 90;
            colDropLines.HeaderText = "Lines";
            colDropLines.Name = "colDropLines";
            colDropLines.Width = 50;

            btnRemoveSelected.Location = new Point(12, 280);
            btnRemoveSelected.Size = new Size(140, 28);
            btnRemoveSelected.Text = "Remove Selected";
            btnRemoveSelected.Click += btnRemoveSelected_Click;

            btnClearCanvas.Location = new Point(162, 280);
            btnClearCanvas.Size = new Size(120, 28);
            btnClearCanvas.Text = "Clear Canvas";
            btnClearCanvas.Enabled = false;
            btnClearCanvas.Click += btnClearCanvas_Click;

            lblLines.AutoSize = true;
            lblLines.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
            lblLines.Location = new Point(16, 452);
            lblLines.Text = "2. Line items on canvas";

            dgvLines.AllowUserToAddRows = false;
            dgvLines.BackgroundColor = Color.White;
            dgvLines.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvLines.Columns.AddRange(new DataGridViewColumn[] {
                colLinePR, colLineItem, colLineDesc, colLineQty, colLinePrice, colLineAmount, colLineVendor });
            dgvLines.Location = new Point(16, 478);
            dgvLines.Name = "dgvLines";
            dgvLines.ReadOnly = true;
            dgvLines.RowHeadersVisible = false;
            dgvLines.Size = new Size(700, 160);

            colLinePR.HeaderText = "PR";
            colLinePR.Width = 110;
            colLineItem.HeaderText = "Item";
            colLineItem.Width = 100;
            colLineDesc.HeaderText = "Description";
            colLineDesc.Width = 180;
            colLineQty.HeaderText = "Qty";
            colLineQty.Width = 60;
            colLinePrice.HeaderText = "Unit Price";
            colLinePrice.Width = 80;
            colLineAmount.HeaderText = "Amount";
            colLineAmount.Width = 80;
            colLineVendor.HeaderText = "Vendor";
            colLineVendor.Width = 80;

            grpPO.Controls.Add(lblPreparedBy);
            grpPO.Controls.Add(txtPreparedBy);
            grpPO.Controls.Add(lblAuthorisedBy);
            grpPO.Controls.Add(txtAuthorisedBy);
            grpPO.Controls.Add(lblDelivery);
            grpPO.Controls.Add(dtpDelivery);
            grpPO.Controls.Add(lblCurrency);
            grpPO.Controls.Add(cmbCurrency);
            grpPO.Controls.Add(lblRemarks);
            grpPO.Controls.Add(txtRemarks);
            grpPO.Controls.Add(btnGeneratePO);
            grpPO.Controls.Add(btnOpenPOApproval);
            grpPO.Location = new Point(732, 452);
            grpPO.Name = "grpPO";
            grpPO.Size = new Size(340, 186);
            grpPO.Text = "3. PO Details";

            lblPreparedBy.AutoSize = true;
            lblPreparedBy.Location = new Point(12, 28);
            lblPreparedBy.Text = "Prepared By*";
            txtPreparedBy.Location = new Point(110, 24);
            txtPreparedBy.Size = new Size(210, 23);

            lblAuthorisedBy.AutoSize = true;
            lblAuthorisedBy.Location = new Point(12, 58);
            lblAuthorisedBy.Text = "Authorised By";
            txtAuthorisedBy.Location = new Point(110, 54);
            txtAuthorisedBy.Size = new Size(210, 23);

            lblDelivery.AutoSize = true;
            lblDelivery.Location = new Point(12, 88);
            lblDelivery.Text = "Delivery";
            dtpDelivery.Location = new Point(110, 84);
            dtpDelivery.Size = new Size(210, 23);
            dtpDelivery.Format = DateTimePickerFormat.Short;

            lblCurrency.AutoSize = true;
            lblCurrency.Location = new Point(12, 118);
            lblCurrency.Text = "Currency";
            cmbCurrency.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbCurrency.Location = new Point(110, 114);
            cmbCurrency.Size = new Size(80, 23);

            lblRemarks.AutoSize = true;
            lblRemarks.Location = new Point(200, 118);
            lblRemarks.Text = "Remarks";
            txtRemarks.Location = new Point(12, 142);
            txtRemarks.Size = new Size(150, 23);

            btnGeneratePO.BackColor = Color.FromArgb(20, 90, 140);
            btnGeneratePO.Enabled = false;
            btnGeneratePO.FlatStyle = FlatStyle.Flat;
            btnGeneratePO.ForeColor = Color.White;
            btnGeneratePO.Location = new Point(170, 140);
            btnGeneratePO.Size = new Size(150, 30);
            btnGeneratePO.Text = "Generate PO";
            btnGeneratePO.Click += btnGeneratePO_Click;

            btnOpenPOApproval.Location = new Point(16, 652);
            btnOpenPOApproval.Size = new Size(140, 32);
            btnOpenPOApproval.Text = "Open PO Approval";
            btnOpenPOApproval.Click += btnOpenPOApproval_Click;

            btnClose.Location = new Point(972, 652);
            btnClose.Size = new Size(100, 32);
            btnClose.Text = "Close";
            btnClose.Click += btnClose_Click;

            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(240, 244, 248);
            ClientSize = new Size(1090, 700);
            Controls.Add(btnClose);
            Controls.Add(btnOpenPOApproval);
            Controls.Add(grpPO);
            Controls.Add(dgvLines);
            Controls.Add(lblLines);
            Controls.Add(panelDropZone);
            Controls.Add(btnRefresh);
            Controls.Add(btnAddSelected);
            Controls.Add(dgvSourcePRs);
            Controls.Add(lblSourceCount);
            Controls.Add(lblSource);
            Controls.Add(panelHeader);
            Name = "frmPRtoPOWorkspace";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "PR to PO Drag && Drop Workspace";
            Load += frmPRtoPOWorkspace_Load;

            panelHeader.ResumeLayout(false);
            panelHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvSourcePRs).EndInit();
            panelDropZone.ResumeLayout(false);
            panelDropZone.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvDropped).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvLines).EndInit();
            grpPO.ResumeLayout(false);
            grpPO.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel panelHeader;
        private Label lblTitle;
        private Label lblUser;
        private Label lblSource;
        private Label lblSourceCount;
        private DataGridView dgvSourcePRs;
        private Button btnAddSelected;
        private Button btnRefresh;
        private Panel panelDropZone;
        private Label lblDropHint;
        private Label lblDropTotal;
        private DataGridView dgvDropped;
        private DataGridViewTextBoxColumn colDropPR;
        private DataGridViewTextBoxColumn colDropVendor;
        private DataGridViewTextBoxColumn colDropProject;
        private DataGridViewTextBoxColumn colDropAmount;
        private DataGridViewTextBoxColumn colDropLines;
        private Button btnRemoveSelected;
        private Button btnClearCanvas;
        private Label lblLines;
        private DataGridView dgvLines;
        private DataGridViewTextBoxColumn colLinePR;
        private DataGridViewTextBoxColumn colLineItem;
        private DataGridViewTextBoxColumn colLineDesc;
        private DataGridViewTextBoxColumn colLineQty;
        private DataGridViewTextBoxColumn colLinePrice;
        private DataGridViewTextBoxColumn colLineAmount;
        private DataGridViewTextBoxColumn colLineVendor;
        private GroupBox grpPO;
        private Label lblPreparedBy;
        private TextBox txtPreparedBy;
        private Label lblAuthorisedBy;
        private TextBox txtAuthorisedBy;
        private Label lblDelivery;
        private DateTimePicker dtpDelivery;
        private Label lblCurrency;
        private ComboBox cmbCurrency;
        private Label lblRemarks;
        private TextBox txtRemarks;
        private Button btnGeneratePO;
        private Button btnOpenPOApproval;
        private Button btnClose;
    }
}
