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
            panelFooter = new Panel();
            btnOpenPOApproval = new Button();
            btnClose = new Button();
            splitMain = new SplitContainer();
            splitTop = new SplitContainer();
            panelSource = new Panel();
            lblSource = new Label();
            lblSourceCount = new Label();
            dgvSourcePRs = new DataGridView();
            panelSourceButtons = new Panel();
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
            panelDropButtons = new Panel();
            btnRemoveSelected = new Button();
            btnClearCanvas = new Button();
            splitBottom = new SplitContainer();
            panelLines = new Panel();
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
            tblPoFields = new TableLayoutPanel();
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
            panelHeader.SuspendLayout();
            panelFooter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitMain).BeginInit();
            splitMain.Panel1.SuspendLayout();
            splitMain.Panel2.SuspendLayout();
            splitMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitTop).BeginInit();
            splitTop.Panel1.SuspendLayout();
            splitTop.Panel2.SuspendLayout();
            splitTop.SuspendLayout();
            panelSource.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvSourcePRs).BeginInit();
            panelSourceButtons.SuspendLayout();
            panelDropZone.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvDropped).BeginInit();
            panelDropButtons.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitBottom).BeginInit();
            splitBottom.Panel1.SuspendLayout();
            splitBottom.Panel2.SuspendLayout();
            splitBottom.SuspendLayout();
            panelLines.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvLines).BeginInit();
            grpPO.SuspendLayout();
            tblPoFields.SuspendLayout();
            SuspendLayout();
            // 
            // panelHeader
            // 
            panelHeader.BackColor = Color.FromArgb(20, 55, 90);
            panelHeader.Controls.Add(lblTitle);
            panelHeader.Controls.Add(lblUser);
            panelHeader.Dock = DockStyle.Top;
            panelHeader.Location = new Point(0, 0);
            panelHeader.Name = "panelHeader";
            panelHeader.Padding = new Padding(12, 8, 12, 8);
            panelHeader.Size = new Size(1100, 70);
            panelHeader.TabIndex = 0;
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI Semibold", 16F, FontStyle.Bold);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(16, 10);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(340, 30);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "PR → PO Workspace (Drag & Drop)";
            // 
            // lblUser
            // 
            lblUser.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            lblUser.AutoEllipsis = true;
            lblUser.ForeColor = Color.FromArgb(190, 210, 230);
            lblUser.Location = new Point(18, 42);
            lblUser.Name = "lblUser";
            lblUser.Size = new Size(1060, 18);
            lblUser.TabIndex = 1;
            lblUser.Text = "User";
            // 
            // panelFooter
            // 
            panelFooter.Controls.Add(btnOpenPOApproval);
            panelFooter.Controls.Add(btnClose);
            panelFooter.Dock = DockStyle.Bottom;
            panelFooter.Location = new Point(0, 652);
            panelFooter.Name = "panelFooter";
            panelFooter.Padding = new Padding(12, 8, 12, 8);
            panelFooter.Size = new Size(1100, 48);
            panelFooter.TabIndex = 2;
            // 
            // btnOpenPOApproval
            // 
            btnOpenPOApproval.Location = new Point(12, 8);
            btnOpenPOApproval.Name = "btnOpenPOApproval";
            btnOpenPOApproval.Size = new Size(150, 32);
            btnOpenPOApproval.TabIndex = 0;
            btnOpenPOApproval.Text = "Open PO Approval";
            btnOpenPOApproval.Click += btnOpenPOApproval_Click;
            // 
            // btnClose
            // 
            btnClose.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnClose.Location = new Point(988, 8);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(100, 32);
            btnClose.TabIndex = 1;
            btnClose.Text = "Close";
            btnClose.Click += btnClose_Click;
            // 
            // splitMain
            // 
            splitMain.Dock = DockStyle.Fill;
            splitMain.Location = new Point(0, 70);
            splitMain.Name = "splitMain";
            splitMain.Orientation = Orientation.Horizontal;
            // 
            // splitMain.Panel1
            // 
            splitMain.Panel1.Controls.Add(splitTop);
            splitMain.Panel1MinSize = 180;
            // 
            // splitMain.Panel2
            // 
            splitMain.Panel2.Controls.Add(splitBottom);
            splitMain.Panel2MinSize = 160;
            splitMain.Size = new Size(1100, 582);
            splitMain.SplitterDistance = 340;
            splitMain.TabIndex = 1;
            // 
            // splitTop
            // 
            splitTop.Dock = DockStyle.Fill;
            splitTop.Location = new Point(0, 0);
            splitTop.Name = "splitTop";
            // 
            // splitTop.Panel1
            // 
            splitTop.Panel1.Controls.Add(panelSource);
            splitTop.Panel1MinSize = 220;
            // 
            // splitTop.Panel2
            // 
            splitTop.Panel2.Controls.Add(panelDropZone);
            splitTop.Panel2MinSize = 220;
            splitTop.Size = new Size(1100, 340);
            splitTop.SplitterDistance = 540;
            splitTop.TabIndex = 0;
            // 
            // panelSource
            // 
            panelSource.Controls.Add(dgvSourcePRs);
            panelSource.Controls.Add(panelSourceButtons);
            panelSource.Controls.Add(lblSourceCount);
            panelSource.Controls.Add(lblSource);
            panelSource.Dock = DockStyle.Fill;
            panelSource.Location = new Point(0, 0);
            panelSource.Name = "panelSource";
            panelSource.Padding = new Padding(10, 8, 8, 8);
            panelSource.Size = new Size(540, 340);
            panelSource.TabIndex = 0;
            // 
            // lblSource
            // 
            lblSource.AutoSize = true;
            lblSource.Dock = DockStyle.Top;
            lblSource.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
            lblSource.Location = new Point(10, 8);
            lblSource.Name = "lblSource";
            lblSource.Padding = new Padding(0, 0, 0, 4);
            lblSource.Size = new Size(303, 24);
            lblSource.TabIndex = 0;
            lblSource.Text = "1. Approved PRs — drag onto the canvas →";
            // 
            // lblSourceCount
            // 
            lblSourceCount.AutoSize = true;
            lblSourceCount.Dock = DockStyle.Top;
            lblSourceCount.Location = new Point(10, 32);
            lblSourceCount.Name = "lblSourceCount";
            lblSourceCount.Padding = new Padding(0, 0, 0, 4);
            lblSourceCount.Size = new Size(125, 19);
            lblSourceCount.TabIndex = 1;
            lblSourceCount.Text = "Approved PRs ready: 0";
            // 
            // dgvSourcePRs
            // 
            dgvSourcePRs.AllowUserToAddRows = false;
            dgvSourcePRs.BackgroundColor = Color.White;
            dgvSourcePRs.Dock = DockStyle.Fill;
            dgvSourcePRs.Location = new Point(10, 51);
            dgvSourcePRs.Name = "dgvSourcePRs";
            dgvSourcePRs.ReadOnly = true;
            dgvSourcePRs.RowHeadersVisible = false;
            dgvSourcePRs.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvSourcePRs.Size = new Size(522, 241);
            dgvSourcePRs.TabIndex = 2;
            dgvSourcePRs.MouseDown += dgvSourcePRs_MouseDown;
            dgvSourcePRs.MouseMove += dgvSourcePRs_MouseMove;
            dgvSourcePRs.MouseUp += dgvSourcePRs_MouseUp;
            // 
            // panelSourceButtons
            // 
            panelSourceButtons.Controls.Add(btnAddSelected);
            panelSourceButtons.Controls.Add(btnRefresh);
            panelSourceButtons.Dock = DockStyle.Bottom;
            panelSourceButtons.Location = new Point(10, 292);
            panelSourceButtons.Name = "panelSourceButtons";
            panelSourceButtons.Size = new Size(522, 40);
            panelSourceButtons.TabIndex = 3;
            // 
            // btnAddSelected
            // 
            btnAddSelected.Location = new Point(0, 6);
            btnAddSelected.Name = "btnAddSelected";
            btnAddSelected.Size = new Size(140, 28);
            btnAddSelected.TabIndex = 0;
            btnAddSelected.Text = "Add Selected →";
            btnAddSelected.Click += btnAddSelected_Click;
            // 
            // btnRefresh
            // 
            btnRefresh.Location = new Point(150, 6);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(100, 28);
            btnRefresh.TabIndex = 1;
            btnRefresh.Text = "Refresh";
            btnRefresh.Click += btnRefresh_Click;
            // 
            // panelDropZone
            // 
            panelDropZone.AllowDrop = true;
            panelDropZone.BackColor = Color.FromArgb(248, 250, 252);
            panelDropZone.BorderStyle = BorderStyle.FixedSingle;
            panelDropZone.Controls.Add(dgvDropped);
            panelDropZone.Controls.Add(panelDropButtons);
            panelDropZone.Controls.Add(lblDropTotal);
            panelDropZone.Controls.Add(lblDropHint);
            panelDropZone.Dock = DockStyle.Fill;
            panelDropZone.Location = new Point(0, 0);
            panelDropZone.Name = "panelDropZone";
            panelDropZone.Padding = new Padding(10, 8, 10, 8);
            panelDropZone.Size = new Size(556, 340);
            panelDropZone.TabIndex = 0;
            panelDropZone.DragDrop += panelDropZone_DragDrop;
            panelDropZone.DragEnter += panelDropZone_DragEnter;
            panelDropZone.DragLeave += panelDropZone_DragLeave;
            // 
            // lblDropHint
            // 
            lblDropHint.AutoSize = true;
            lblDropHint.Dock = DockStyle.Top;
            lblDropHint.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
            lblDropHint.ForeColor = Color.FromArgb(20, 90, 140);
            lblDropHint.Location = new Point(10, 8);
            lblDropHint.Name = "lblDropHint";
            lblDropHint.Padding = new Padding(0, 0, 0, 2);
            lblDropHint.Size = new Size(274, 22);
            lblDropHint.TabIndex = 0;
            lblDropHint.Text = "Drop approved PRs here to build PO(s)";
            // 
            // lblDropTotal
            // 
            lblDropTotal.AutoSize = true;
            lblDropTotal.Dock = DockStyle.Top;
            lblDropTotal.Location = new Point(10, 30);
            lblDropTotal.Name = "lblDropTotal";
            lblDropTotal.Padding = new Padding(0, 0, 0, 4);
            lblDropTotal.Size = new Size(108, 19);
            lblDropTotal.TabIndex = 1;
            lblDropTotal.Text = "Canvas total: ₹ 0.00";
            // 
            // dgvDropped
            // 
            dgvDropped.AllowUserToAddRows = false;
            dgvDropped.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvDropped.BackgroundColor = Color.White;
            dgvDropped.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvDropped.Columns.AddRange(new DataGridViewColumn[] { colDropPR, colDropVendor, colDropProject, colDropAmount, colDropLines });
            dgvDropped.Dock = DockStyle.Fill;
            dgvDropped.Location = new Point(10, 49);
            dgvDropped.Name = "dgvDropped";
            dgvDropped.ReadOnly = true;
            dgvDropped.RowHeadersVisible = false;
            dgvDropped.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvDropped.Size = new Size(534, 243);
            dgvDropped.TabIndex = 2;
            // 
            // colDropPR
            // 
            colDropPR.FillWeight = 120F;
            colDropPR.HeaderText = "PR Number";
            colDropPR.Name = "colDropPR";
            colDropPR.ReadOnly = true;
            // 
            // colDropVendor
            // 
            colDropVendor.FillWeight = 110F;
            colDropVendor.HeaderText = "Vendor";
            colDropVendor.Name = "colDropVendor";
            colDropVendor.ReadOnly = true;
            // 
            // colDropProject
            // 
            colDropProject.FillWeight = 100F;
            colDropProject.HeaderText = "Project";
            colDropProject.Name = "colDropProject";
            colDropProject.ReadOnly = true;
            // 
            // colDropAmount
            // 
            colDropAmount.FillWeight = 80F;
            colDropAmount.HeaderText = "Amount";
            colDropAmount.Name = "colDropAmount";
            colDropAmount.ReadOnly = true;
            // 
            // colDropLines
            // 
            colDropLines.FillWeight = 50F;
            colDropLines.HeaderText = "Lines";
            colDropLines.Name = "colDropLines";
            colDropLines.ReadOnly = true;
            // 
            // panelDropButtons
            // 
            panelDropButtons.Controls.Add(btnRemoveSelected);
            panelDropButtons.Controls.Add(btnClearCanvas);
            panelDropButtons.Dock = DockStyle.Bottom;
            panelDropButtons.Location = new Point(10, 292);
            panelDropButtons.Name = "panelDropButtons";
            panelDropButtons.Size = new Size(534, 40);
            panelDropButtons.TabIndex = 3;
            // 
            // btnRemoveSelected
            // 
            btnRemoveSelected.Location = new Point(0, 6);
            btnRemoveSelected.Name = "btnRemoveSelected";
            btnRemoveSelected.Size = new Size(140, 28);
            btnRemoveSelected.TabIndex = 0;
            btnRemoveSelected.Text = "Remove Selected";
            btnRemoveSelected.Click += btnRemoveSelected_Click;
            // 
            // btnClearCanvas
            // 
            btnClearCanvas.Enabled = false;
            btnClearCanvas.Location = new Point(150, 6);
            btnClearCanvas.Name = "btnClearCanvas";
            btnClearCanvas.Size = new Size(120, 28);
            btnClearCanvas.TabIndex = 1;
            btnClearCanvas.Text = "Clear Canvas";
            btnClearCanvas.Click += btnClearCanvas_Click;
            // 
            // splitBottom
            // 
            splitBottom.Dock = DockStyle.Fill;
            splitBottom.Location = new Point(0, 0);
            splitBottom.Name = "splitBottom";
            // 
            // splitBottom.Panel1
            // 
            splitBottom.Panel1.Controls.Add(panelLines);
            splitBottom.Panel1MinSize = 280;
            // 
            // splitBottom.Panel2
            // 
            splitBottom.Panel2.Controls.Add(grpPO);
            splitBottom.Panel2MinSize = 260;
            splitBottom.Size = new Size(1100, 238);
            splitBottom.SplitterDistance = 740;
            splitBottom.TabIndex = 0;
            // 
            // panelLines
            // 
            panelLines.Controls.Add(dgvLines);
            panelLines.Controls.Add(lblLines);
            panelLines.Dock = DockStyle.Fill;
            panelLines.Location = new Point(0, 0);
            panelLines.Name = "panelLines";
            panelLines.Padding = new Padding(10, 8, 8, 8);
            panelLines.Size = new Size(740, 238);
            panelLines.TabIndex = 0;
            // 
            // lblLines
            // 
            lblLines.AutoSize = true;
            lblLines.Dock = DockStyle.Top;
            lblLines.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
            lblLines.Location = new Point(10, 8);
            lblLines.Name = "lblLines";
            lblLines.Padding = new Padding(0, 0, 0, 4);
            lblLines.Size = new Size(165, 24);
            lblLines.TabIndex = 0;
            lblLines.Text = "2. Line items on canvas";
            // 
            // dgvLines
            // 
            dgvLines.AllowUserToAddRows = false;
            dgvLines.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvLines.BackgroundColor = Color.White;
            dgvLines.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvLines.Columns.AddRange(new DataGridViewColumn[] { colLinePR, colLineItem, colLineDesc, colLineQty, colLinePrice, colLineAmount, colLineVendor });
            dgvLines.Dock = DockStyle.Fill;
            dgvLines.Location = new Point(10, 32);
            dgvLines.Name = "dgvLines";
            dgvLines.ReadOnly = true;
            dgvLines.RowHeadersVisible = false;
            dgvLines.Size = new Size(722, 198);
            dgvLines.TabIndex = 1;
            // 
            // colLinePR
            // 
            colLinePR.FillWeight = 100F;
            colLinePR.HeaderText = "PR";
            colLinePR.Name = "colLinePR";
            colLinePR.ReadOnly = true;
            // 
            // colLineItem
            // 
            colLineItem.FillWeight = 90F;
            colLineItem.HeaderText = "Item";
            colLineItem.Name = "colLineItem";
            colLineItem.ReadOnly = true;
            // 
            // colLineDesc
            // 
            colLineDesc.FillWeight = 160F;
            colLineDesc.HeaderText = "Description";
            colLineDesc.Name = "colLineDesc";
            colLineDesc.ReadOnly = true;
            // 
            // colLineQty
            // 
            colLineQty.FillWeight = 50F;
            colLineQty.HeaderText = "Qty";
            colLineQty.Name = "colLineQty";
            colLineQty.ReadOnly = true;
            // 
            // colLinePrice
            // 
            colLinePrice.FillWeight = 70F;
            colLinePrice.HeaderText = "Unit Price";
            colLinePrice.Name = "colLinePrice";
            colLinePrice.ReadOnly = true;
            // 
            // colLineAmount
            // 
            colLineAmount.FillWeight = 70F;
            colLineAmount.HeaderText = "Amount";
            colLineAmount.Name = "colLineAmount";
            colLineAmount.ReadOnly = true;
            // 
            // colLineVendor
            // 
            colLineVendor.FillWeight = 70F;
            colLineVendor.HeaderText = "Vendor";
            colLineVendor.Name = "colLineVendor";
            colLineVendor.ReadOnly = true;
            // 
            // grpPO
            // 
            grpPO.Controls.Add(tblPoFields);
            grpPO.Dock = DockStyle.Fill;
            grpPO.Location = new Point(0, 0);
            grpPO.Name = "grpPO";
            grpPO.Padding = new Padding(10, 8, 10, 8);
            grpPO.Size = new Size(356, 238);
            grpPO.TabIndex = 0;
            grpPO.TabStop = false;
            grpPO.Text = "3. PO Details";
            // 
            // tblPoFields
            // 
            tblPoFields.ColumnCount = 2;
            tblPoFields.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 100F));
            tblPoFields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tblPoFields.Controls.Add(lblPreparedBy, 0, 0);
            tblPoFields.Controls.Add(txtPreparedBy, 1, 0);
            tblPoFields.Controls.Add(lblAuthorisedBy, 0, 1);
            tblPoFields.Controls.Add(txtAuthorisedBy, 1, 1);
            tblPoFields.Controls.Add(lblDelivery, 0, 2);
            tblPoFields.Controls.Add(dtpDelivery, 1, 2);
            tblPoFields.Controls.Add(lblCurrency, 0, 3);
            tblPoFields.Controls.Add(cmbCurrency, 1, 3);
            tblPoFields.Controls.Add(lblRemarks, 0, 4);
            tblPoFields.Controls.Add(txtRemarks, 1, 4);
            tblPoFields.Controls.Add(btnGeneratePO, 1, 5);
            tblPoFields.Dock = DockStyle.Fill;
            tblPoFields.Location = new Point(10, 24);
            tblPoFields.Name = "tblPoFields";
            tblPoFields.RowCount = 6;
            tblPoFields.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            tblPoFields.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            tblPoFields.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            tblPoFields.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            tblPoFields.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tblPoFields.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
            tblPoFields.Size = new Size(336, 206);
            tblPoFields.TabIndex = 0;
            // 
            // lblPreparedBy
            // 
            lblPreparedBy.Anchor = AnchorStyles.Left;
            lblPreparedBy.AutoSize = true;
            lblPreparedBy.Location = new Point(3, 8);
            lblPreparedBy.Name = "lblPreparedBy";
            lblPreparedBy.Size = new Size(75, 15);
            lblPreparedBy.TabIndex = 0;
            lblPreparedBy.Text = "Prepared By*";
            // 
            // txtPreparedBy
            // 
            txtPreparedBy.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            txtPreparedBy.Location = new Point(103, 4);
            txtPreparedBy.Name = "txtPreparedBy";
            txtPreparedBy.Size = new Size(230, 23);
            txtPreparedBy.TabIndex = 1;
            // 
            // lblAuthorisedBy
            // 
            lblAuthorisedBy.Anchor = AnchorStyles.Left;
            lblAuthorisedBy.AutoSize = true;
            lblAuthorisedBy.Location = new Point(3, 40);
            lblAuthorisedBy.Name = "lblAuthorisedBy";
            lblAuthorisedBy.Size = new Size(81, 15);
            lblAuthorisedBy.TabIndex = 2;
            lblAuthorisedBy.Text = "Authorised By";
            // 
            // txtAuthorisedBy
            // 
            txtAuthorisedBy.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            txtAuthorisedBy.Location = new Point(103, 36);
            txtAuthorisedBy.Name = "txtAuthorisedBy";
            txtAuthorisedBy.Size = new Size(230, 23);
            txtAuthorisedBy.TabIndex = 3;
            // 
            // lblDelivery
            // 
            lblDelivery.Anchor = AnchorStyles.Left;
            lblDelivery.AutoSize = true;
            lblDelivery.Location = new Point(3, 72);
            lblDelivery.Name = "lblDelivery";
            lblDelivery.Size = new Size(49, 15);
            lblDelivery.TabIndex = 4;
            lblDelivery.Text = "Delivery";
            // 
            // dtpDelivery
            // 
            dtpDelivery.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            dtpDelivery.Format = DateTimePickerFormat.Short;
            dtpDelivery.Location = new Point(103, 68);
            dtpDelivery.Name = "dtpDelivery";
            dtpDelivery.Size = new Size(230, 23);
            dtpDelivery.TabIndex = 5;
            // 
            // lblCurrency
            // 
            lblCurrency.Anchor = AnchorStyles.Left;
            lblCurrency.AutoSize = true;
            lblCurrency.Location = new Point(3, 104);
            lblCurrency.Name = "lblCurrency";
            lblCurrency.Size = new Size(55, 15);
            lblCurrency.TabIndex = 6;
            lblCurrency.Text = "Currency";
            // 
            // cmbCurrency
            // 
            cmbCurrency.Anchor = AnchorStyles.Left;
            cmbCurrency.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbCurrency.Location = new Point(103, 100);
            cmbCurrency.Name = "cmbCurrency";
            cmbCurrency.Size = new Size(100, 23);
            cmbCurrency.TabIndex = 7;
            // 
            // lblRemarks
            // 
            lblRemarks.Anchor = AnchorStyles.Left | AnchorStyles.Top;
            lblRemarks.AutoSize = true;
            lblRemarks.Location = new Point(3, 131);
            lblRemarks.Name = "lblRemarks";
            lblRemarks.Size = new Size(52, 15);
            lblRemarks.TabIndex = 8;
            lblRemarks.Text = "Remarks";
            // 
            // txtRemarks
            // 
            txtRemarks.Dock = DockStyle.Fill;
            txtRemarks.Location = new Point(103, 131);
            txtRemarks.Multiline = true;
            txtRemarks.Name = "txtRemarks";
            txtRemarks.Size = new Size(230, 32);
            txtRemarks.TabIndex = 9;
            // 
            // btnGeneratePO
            // 
            btnGeneratePO.Anchor = AnchorStyles.Right;
            btnGeneratePO.BackColor = Color.FromArgb(20, 90, 140);
            btnGeneratePO.Enabled = false;
            btnGeneratePO.FlatStyle = FlatStyle.Flat;
            btnGeneratePO.ForeColor = Color.White;
            btnGeneratePO.Location = new Point(163, 172);
            btnGeneratePO.Name = "btnGeneratePO";
            btnGeneratePO.Size = new Size(170, 30);
            btnGeneratePO.TabIndex = 10;
            btnGeneratePO.Text = "Generate PO";
            btnGeneratePO.UseVisualStyleBackColor = false;
            btnGeneratePO.Click += btnGeneratePO_Click;
            // 
            // frmPRtoPOWorkspace
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(240, 244, 248);
            ClientSize = new Size(1100, 700);
            Controls.Add(splitMain);
            Controls.Add(panelFooter);
            Controls.Add(panelHeader);
            MinimumSize = new Size(900, 560);
            Name = "frmPRtoPOWorkspace";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "PR to PO Drag && Drop Workspace";
            Load += frmPRtoPOWorkspace_Load;
            Shown += frmPRtoPOWorkspace_Shown;
            panelHeader.ResumeLayout(false);
            panelHeader.PerformLayout();
            panelFooter.ResumeLayout(false);
            splitMain.Panel1.ResumeLayout(false);
            splitMain.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitMain).EndInit();
            splitMain.ResumeLayout(false);
            splitTop.Panel1.ResumeLayout(false);
            splitTop.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitTop).EndInit();
            splitTop.ResumeLayout(false);
            panelSource.ResumeLayout(false);
            panelSource.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvSourcePRs).EndInit();
            panelSourceButtons.ResumeLayout(false);
            panelDropZone.ResumeLayout(false);
            panelDropZone.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvDropped).EndInit();
            panelDropButtons.ResumeLayout(false);
            splitBottom.Panel1.ResumeLayout(false);
            splitBottom.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitBottom).EndInit();
            splitBottom.ResumeLayout(false);
            panelLines.ResumeLayout(false);
            panelLines.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvLines).EndInit();
            grpPO.ResumeLayout(false);
            tblPoFields.ResumeLayout(false);
            tblPoFields.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel panelHeader;
        private Label lblTitle;
        private Label lblUser;
        private Panel panelFooter;
        private Button btnOpenPOApproval;
        private Button btnClose;
        private SplitContainer splitMain;
        private SplitContainer splitTop;
        private Panel panelSource;
        private Label lblSource;
        private Label lblSourceCount;
        private DataGridView dgvSourcePRs;
        private Panel panelSourceButtons;
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
        private Panel panelDropButtons;
        private Button btnRemoveSelected;
        private Button btnClearCanvas;
        private SplitContainer splitBottom;
        private Panel panelLines;
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
        private TableLayoutPanel tblPoFields;
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
    }
}
