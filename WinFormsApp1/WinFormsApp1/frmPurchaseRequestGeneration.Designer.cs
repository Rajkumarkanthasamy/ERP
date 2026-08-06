//namespace WinFormsApp1
//{
//    partial class frmPurchaseRequestGeneration
//    {
//        /// <summary>
//        /// Required designer variable.
//        /// </summary>
//        private System.ComponentModel.IContainer components = null;

//        /// <summary>
//        /// Clean up any resources being used.
//        /// </summary>
//        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
//        protected override void Dispose(bool disposing)
//        {
//            if (disposing && (components != null))
//            {
//                components.Dispose();
//            }
//            base.Dispose(disposing);
//        }

//        #region Windows Form Designer generated code

//        /// <summary>
//        /// Required method for Designer support - do not modify
//        /// the contents of this method with the code editor.
//        /// </summary>
//        private void InitializeComponent()
//        {
//            this.components = new System.ComponentModel.Container();
//            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
//            this.ClientSize = new System.Drawing.Size(800, 450);
//            this.Text = "frmPurchaseRequestGeneration";
//        }

//        #endregion
//    }
//}

namespace WinFormsApp1
{
    partial class frmPurchaseRequestGeneration
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            lblTitle = new Label();
            grpProjectSelection = new GroupBox();
            btnRefreshProjects = new Button();
            btnLoadBOM = new Button();
            cmbProductNo = new ComboBox();
            lblProductNo = new Label();
            cmbProjectCode = new ComboBox();
            lblProjectCode = new Label();
            grpBOMItems = new GroupBox();
            checkAll = new CheckBox();
            dgvBOMItems = new DataGridView();
            colSelect = new DataGridViewCheckBoxColumn();
            colProjectBOMCode = new DataGridViewTextBoxColumn();
            colItemCode = new DataGridViewTextBoxColumn();
            colItemName = new DataGridViewTextBoxColumn();
            colProductCode = new DataGridViewTextBoxColumn();
            colProductNo = new DataGridViewTextBoxColumn();
            cloissuedQty = new DataGridViewTextBoxColumn();
            colBOMQty = new DataGridViewTextBoxColumn();
            colConvertedQty = new DataGridViewTextBoxColumn();
            currentPendingQty = new DataGridViewTextBoxColumn();
            colAlreadyPurchased = new DataGridViewTextBoxColumn();
            colAvaiableQty = new DataGridViewTextBoxColumn();
            colBalanceQty = new DataGridViewTextBoxColumn();
            colUnitCost = new DataGridViewTextBoxColumn();
            colTotalCost = new DataGridViewTextBoxColumn();
            colVendor = new DataGridViewComboBoxColumn();
            colHSNCode = new DataGridViewTextBoxColumn();
            collocation = new DataGridViewTextBoxColumn();
            colStatus = new DataGridViewTextBoxColumn();
            colPRQty = new DataGridViewTextBoxColumn();
            grpPRSummary = new GroupBox();
            lblLimitStatus = new Label();
            txtLimitRemaining = new TextBox();
            lblLimitRemaining = new Label();
            txtTotalAmount = new TextBox();
            lblTotalAmount = new Label();
            txtSelectedItems = new TextBox();
            lblSelectedItems = new Label();
            txtTotalItems = new TextBox();
            lblTotalItems = new Label();
            grpVendorGrouping = new GroupBox();
            dgvVendorGroups = new DataGridView();
            colGroupVendor = new DataGridViewTextBoxColumn();
            colGroupItems = new DataGridViewTextBoxColumn();
            colGroupAmount = new DataGridViewTextBoxColumn();
            colGroupPRNumber = new DataGridViewTextBoxColumn();
            lblGroupingInfo = new Label();
            chkGroupByVendor = new CheckBox();
            grpActions = new GroupBox();
            ApprovalPagebutton = new Button();
            btnClubPR = new Button();
            btnClose = new Button();
            btnPrint = new Button();
            btnExport = new Button();
            btnViewPRHistory = new Button();
            btnClearSelection = new Button();
            btnAutoGroup = new Button();
            btnGeneratePR = new Button();
            grpProjectInfo = new GroupBox();
            txtPRNumber = new TextBox();
            lblPRNumber = new Label();
            dtpCreatedDate = new DateTimePicker();
            lblCreatedDate = new Label();
            txtCreatedBy = new TextBox();
            lblCreatedBy = new Label();
            txtProductName = new TextBox();
            lblProductName = new Label();
            txtProjectName = new TextBox();
            lblProjectName = new Label();
            grpProjectSelection.SuspendLayout();
            grpBOMItems.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvBOMItems).BeginInit();
            grpPRSummary.SuspendLayout();
            grpVendorGrouping.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvVendorGroups).BeginInit();
            grpActions.SuspendLayout();
            grpProjectInfo.SuspendLayout();
            SuspendLayout();
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Microsoft Sans Serif", 14F, FontStyle.Bold);
            lblTitle.Location = new Point(327, 10);
            lblTitle.Margin = new Padding(4, 0, 4, 0);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(412, 24);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Purchase Request Generation (BOM to PR)";
            // 
            // grpProjectSelection
            // 
            grpProjectSelection.Controls.Add(btnRefreshProjects);
            grpProjectSelection.Controls.Add(btnLoadBOM);
            grpProjectSelection.Controls.Add(cmbProductNo);
            grpProjectSelection.Controls.Add(lblProductNo);
            grpProjectSelection.Controls.Add(cmbProjectCode);
            grpProjectSelection.Controls.Add(lblProjectCode);
            grpProjectSelection.Font = new Font("Microsoft Sans Serif", 9F);
            grpProjectSelection.Location = new Point(14, 52);
            grpProjectSelection.Margin = new Padding(4, 3, 4, 3);
            grpProjectSelection.Name = "grpProjectSelection";
            grpProjectSelection.Padding = new Padding(4, 3, 4, 3);
            grpProjectSelection.Size = new Size(700, 92);
            grpProjectSelection.TabIndex = 1;
            grpProjectSelection.TabStop = false;
            grpProjectSelection.Text = "Step 1: Select Project";
            // 
            // btnRefreshProjects
            // 
            btnRefreshProjects.BackColor = Color.LightYellow;
            btnRefreshProjects.Location = new Point(280, 58);
            btnRefreshProjects.Margin = new Padding(4, 3, 4, 3);
            btnRefreshProjects.Name = "btnRefreshProjects";
            btnRefreshProjects.Size = new Size(117, 29);
            btnRefreshProjects.TabIndex = 5;
            btnRefreshProjects.Text = "Refresh";
            btnRefreshProjects.UseVisualStyleBackColor = false;
            btnRefreshProjects.Click += btnRefreshProjects_Click;
            // 
            // btnLoadBOM
            // 
            btnLoadBOM.BackColor = Color.LightBlue;
            btnLoadBOM.Location = new Point(117, 58);
            btnLoadBOM.Margin = new Padding(4, 3, 4, 3);
            btnLoadBOM.Name = "btnLoadBOM";
            btnLoadBOM.Size = new Size(140, 29);
            btnLoadBOM.TabIndex = 4;
            btnLoadBOM.Text = "Load BOM Items";
            btnLoadBOM.UseVisualStyleBackColor = false;
            btnLoadBOM.Click += btnLoadBOM_Click;
            // 
            // cmbProductNo
            // 
            cmbProductNo.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbProductNo.FormattingEnabled = true;
            cmbProductNo.Location = new Point(438, 25);
            cmbProductNo.Margin = new Padding(4, 3, 4, 3);
            cmbProductNo.Name = "cmbProductNo";
            cmbProductNo.Size = new Size(174, 23);
            cmbProductNo.TabIndex = 3;
            // 
            // lblProductNo
            // 
            lblProductNo.AutoSize = true;
            lblProductNo.Location = new Point(350, 29);
            lblProductNo.Margin = new Padding(4, 0, 4, 0);
            lblProductNo.Name = "lblProductNo";
            lblProductNo.Size = new Size(71, 15);
            lblProductNo.TabIndex = 2;
            lblProductNo.Text = "Product No:";
            // 
            // cmbProjectCode
            // 
            cmbProjectCode.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cmbProjectCode.AutoCompleteSource = AutoCompleteSource.ListItems;
            cmbProjectCode.FormattingEnabled = true;
            cmbProjectCode.Location = new Point(117, 25);
            cmbProjectCode.Margin = new Padding(4, 3, 4, 3);
            cmbProjectCode.Name = "cmbProjectCode";
            cmbProjectCode.Size = new Size(209, 23);
            cmbProjectCode.TabIndex = 1;
            cmbProjectCode.SelectedIndexChanged += cmbProjectCode_SelectedIndexChanged;
            // 
            // lblProjectCode
            // 
            lblProjectCode.AutoSize = true;
            lblProjectCode.Location = new Point(18, 29);
            lblProjectCode.Margin = new Padding(4, 0, 4, 0);
            lblProjectCode.Name = "lblProjectCode";
            lblProjectCode.Size = new Size(80, 15);
            lblProjectCode.TabIndex = 0;
            lblProjectCode.Text = "Project Code:";
            // 
            // grpBOMItems
            // 
            grpBOMItems.Controls.Add(checkAll);
            grpBOMItems.Controls.Add(dgvBOMItems);
            grpBOMItems.Font = new Font("Microsoft Sans Serif", 9F);
            grpBOMItems.Location = new Point(14, 219);
            grpBOMItems.Margin = new Padding(4, 3, 4, 3);
            grpBOMItems.Name = "grpBOMItems";
            grpBOMItems.Padding = new Padding(4, 3, 4, 3);
            grpBOMItems.Size = new Size(1283, 346);
            grpBOMItems.TabIndex = 2;
            grpBOMItems.TabStop = false;
            grpBOMItems.Text = "BOM Items - Select items to convert to PR (Drag && Drop enabled)";
            // 
            // checkAll
            // 
            checkAll.AutoSize = true;
            checkAll.Location = new Point(362, 0);
            checkAll.Name = "checkAll";
            checkAll.Padding = new Padding(8, 0, 5, 0);
            checkAll.Size = new Size(89, 19);
            checkAll.TabIndex = 1;
            checkAll.Text = "Select All";
            checkAll.UseVisualStyleBackColor = true;
            checkAll.CheckedChanged += checkAll_CheckedChanged;
            // 
            // dgvBOMItems
            // 
            dgvBOMItems.AllowUserToAddRows = false;
            dgvBOMItems.AllowUserToDeleteRows = false;
            dgvBOMItems.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvBOMItems.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvBOMItems.Columns.AddRange(new DataGridViewColumn[] { colSelect, colProjectBOMCode, colItemCode, colItemName, colProductCode, colProductNo, cloissuedQty, colBOMQty, colConvertedQty, currentPendingQty, colAlreadyPurchased, colAvaiableQty, colBalanceQty, colUnitCost, colTotalCost, colVendor, colHSNCode, collocation, colStatus });
            dgvBOMItems.Location = new Point(7, 23);
            dgvBOMItems.Margin = new Padding(4, 3, 4, 3);
            dgvBOMItems.Name = "dgvBOMItems";
            dgvBOMItems.RowHeadersVisible = false;
            dgvBOMItems.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvBOMItems.Size = new Size(1269, 316);
            dgvBOMItems.TabIndex = 0;
            dgvBOMItems.CellValueChanged += dgvBOMItems_CellValueChanged;
            dgvBOMItems.CurrentCellDirtyStateChanged += dgvBOMItems_CurrentCellDirtyStateChanged;
            // 
            // colSelect
            // 
            colSelect.HeaderText = "Select";
            colSelect.Name = "colSelect";
            // 
            // colProjectBOMCode
            // 
            colProjectBOMCode.HeaderText = "BOM Code";
            colProjectBOMCode.Name = "colProjectBOMCode";
            colProjectBOMCode.ReadOnly = true;
            // 
            // colItemCode
            // 
            colItemCode.HeaderText = "Item Code";
            colItemCode.Name = "colItemCode";
            colItemCode.ReadOnly = true;
            // 
            // colItemName
            // 
            colItemName.HeaderText = "Item Name";
            colItemName.Name = "colItemName";
            colItemName.ReadOnly = true;
            // 
            // colProductCode
            // 
            colProductCode.HeaderText = "Product code";
            colProductCode.Name = "colProductCode";
            colProductCode.ReadOnly = true;
            // 
            // colProductNo
            // 
            colProductNo.HeaderText = "Product No";
            colProductNo.Name = "colProductNo";
            colProductNo.ReadOnly = true;
            // 
            // cloissuedQty
            // 
            cloissuedQty.HeaderText = "Issued Qty";
            cloissuedQty.Name = "cloissuedQty";
            cloissuedQty.ReadOnly = true;
            // 
            // colBOMQty
            // 
            colBOMQty.HeaderText = "BOM Qty";
            colBOMQty.Name = "colBOMQty";
            colBOMQty.ReadOnly = true;
            // 
            // colConvertedQty
            // 
            colConvertedQty.HeaderText = "PR Converted";
            colConvertedQty.Name = "colConvertedQty";
            colConvertedQty.ReadOnly = true;
            // 
            // currentPendingQty
            // 
            currentPendingQty.HeaderText = "PO Pending Qty";
            currentPendingQty.Name = "currentPendingQty";
            currentPendingQty.ReadOnly = true;
            // 
            // colAlreadyPurchased
            // 
            colAlreadyPurchased.HeaderText = "Already Purchased";
            colAlreadyPurchased.Name = "colAlreadyPurchased";
            colAlreadyPurchased.ReadOnly = true;
            // 
            // colAvaiableQty
            // 
            colAvaiableQty.HeaderText = "Avaiable Qty";
            colAvaiableQty.Name = "colAvaiableQty";
            colAvaiableQty.ReadOnly = true;
            // 
            // colBalanceQty
            // 
            colBalanceQty.HeaderText = "Balance Qty";
            colBalanceQty.Name = "colBalanceQty";
            colBalanceQty.ReadOnly = true;
            // 
            // colUnitCost
            // 
            colUnitCost.HeaderText = "Unit Cost";
            colUnitCost.Name = "colUnitCost";
            // 
            // colTotalCost
            // 
            colTotalCost.HeaderText = "Total Cost";
            colTotalCost.Name = "colTotalCost";
            colTotalCost.ReadOnly = true;
            // 
            // colVendor
            // 
            colVendor.HeaderText = "Select Vendor";
            colVendor.Name = "colVendor";
            // 
            // colHSNCode
            // 
            colHSNCode.HeaderText = "HSN Code";
            colHSNCode.Name = "colHSNCode";
            // 
            // collocation
            // 
            collocation.HeaderText = "Location";
            collocation.Name = "collocation";
            // 
            // colStatus
            // 
            colStatus.HeaderText = "Status";
            colStatus.Name = "colStatus";
            colStatus.ReadOnly = true;
            // 
            // colPRQty
            // 
            colPRQty.HeaderText = "PR Qty";
            colPRQty.Name = "colPRQty";
            // 
            // grpPRSummary
            // 
            grpPRSummary.Controls.Add(lblLimitStatus);
            grpPRSummary.Controls.Add(txtLimitRemaining);
            grpPRSummary.Controls.Add(lblLimitRemaining);
            grpPRSummary.Controls.Add(txtTotalAmount);
            grpPRSummary.Controls.Add(lblTotalAmount);
            grpPRSummary.Controls.Add(txtSelectedItems);
            grpPRSummary.Controls.Add(lblSelectedItems);
            grpPRSummary.Controls.Add(txtTotalItems);
            grpPRSummary.Controls.Add(lblTotalItems);
            grpPRSummary.Font = new Font("Microsoft Sans Serif", 9F);
            grpPRSummary.Location = new Point(14, 577);
            grpPRSummary.Margin = new Padding(4, 3, 4, 3);
            grpPRSummary.Name = "grpPRSummary";
            grpPRSummary.Padding = new Padding(4, 3, 4, 3);
            grpPRSummary.Size = new Size(467, 150);
            grpPRSummary.TabIndex = 3;
            grpPRSummary.TabStop = false;
            grpPRSummary.Text = "PR Summary (Limit: 12,00,000 INR)";
            // 
            // lblLimitStatus
            // 
            lblLimitStatus.AutoSize = true;
            lblLimitStatus.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);
            lblLimitStatus.Location = new Point(292, 98);
            lblLimitStatus.Margin = new Padding(4, 0, 4, 0);
            lblLimitStatus.Name = "lblLimitStatus";
            lblLimitStatus.Size = new Size(47, 15);
            lblLimitStatus.TabIndex = 8;
            lblLimitStatus.Text = "Status";
            // 
            // txtLimitRemaining
            // 
            txtLimitRemaining.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);
            txtLimitRemaining.Location = new Point(140, 95);
            txtLimitRemaining.Margin = new Padding(4, 3, 4, 3);
            txtLimitRemaining.Name = "txtLimitRemaining";
            txtLimitRemaining.ReadOnly = true;
            txtLimitRemaining.Size = new Size(139, 21);
            txtLimitRemaining.TabIndex = 7;
            txtLimitRemaining.Text = "12,00,000.00";
            // 
            // lblLimitRemaining
            // 
            lblLimitRemaining.AutoSize = true;
            lblLimitRemaining.Location = new Point(18, 98);
            lblLimitRemaining.Margin = new Padding(4, 0, 4, 0);
            lblLimitRemaining.Name = "lblLimitRemaining";
            lblLimitRemaining.Size = new Size(101, 15);
            lblLimitRemaining.TabIndex = 6;
            lblLimitRemaining.Text = "Limit Remaining:";
            // 
            // txtTotalAmount
            // 
            txtTotalAmount.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);
            txtTotalAmount.Location = new Point(140, 60);
            txtTotalAmount.Margin = new Padding(4, 3, 4, 3);
            txtTotalAmount.Name = "txtTotalAmount";
            txtTotalAmount.ReadOnly = true;
            txtTotalAmount.Size = new Size(139, 21);
            txtTotalAmount.TabIndex = 5;
            txtTotalAmount.Text = "0.00";
            // 
            // lblTotalAmount
            // 
            lblTotalAmount.AutoSize = true;
            lblTotalAmount.Location = new Point(18, 63);
            lblTotalAmount.Margin = new Padding(4, 0, 4, 0);
            lblTotalAmount.Name = "lblTotalAmount";
            lblTotalAmount.Size = new Size(82, 15);
            lblTotalAmount.TabIndex = 4;
            lblTotalAmount.Text = "Total Amount:";
            // 
            // txtSelectedItems
            // 
            txtSelectedItems.Location = new Point(362, 25);
            txtSelectedItems.Margin = new Padding(4, 3, 4, 3);
            txtSelectedItems.Name = "txtSelectedItems";
            txtSelectedItems.ReadOnly = true;
            txtSelectedItems.Size = new Size(93, 21);
            txtSelectedItems.TabIndex = 3;
            txtSelectedItems.Text = "0";
            // 
            // lblSelectedItems
            // 
            lblSelectedItems.AutoSize = true;
            lblSelectedItems.Location = new Point(257, 29);
            lblSelectedItems.Margin = new Padding(4, 0, 4, 0);
            lblSelectedItems.Name = "lblSelectedItems";
            lblSelectedItems.Size = new Size(91, 15);
            lblSelectedItems.TabIndex = 2;
            lblSelectedItems.Text = "Selected Items:";
            // 
            // txtTotalItems
            // 
            txtTotalItems.Location = new Point(140, 25);
            txtTotalItems.Margin = new Padding(4, 3, 4, 3);
            txtTotalItems.Name = "txtTotalItems";
            txtTotalItems.ReadOnly = true;
            txtTotalItems.Size = new Size(93, 21);
            txtTotalItems.TabIndex = 1;
            txtTotalItems.Text = "0";
            // 
            // lblTotalItems
            // 
            lblTotalItems.AutoSize = true;
            lblTotalItems.Location = new Point(18, 29);
            lblTotalItems.Margin = new Padding(4, 0, 4, 0);
            lblTotalItems.Name = "lblTotalItems";
            lblTotalItems.Size = new Size(70, 15);
            lblTotalItems.TabIndex = 0;
            lblTotalItems.Text = "Total Items:";
            // 
            // grpVendorGrouping
            // 
            grpVendorGrouping.Controls.Add(dgvVendorGroups);
            grpVendorGrouping.Controls.Add(lblGroupingInfo);
            grpVendorGrouping.Controls.Add(chkGroupByVendor);
            grpVendorGrouping.Font = new Font("Microsoft Sans Serif", 9F);
            grpVendorGrouping.Location = new Point(502, 577);
            grpVendorGrouping.Margin = new Padding(4, 3, 4, 3);
            grpVendorGrouping.Name = "grpVendorGrouping";
            grpVendorGrouping.Padding = new Padding(4, 3, 4, 3);
            grpVendorGrouping.Size = new Size(467, 150);
            grpVendorGrouping.TabIndex = 4;
            grpVendorGrouping.TabStop = false;
            grpVendorGrouping.Text = "Vendor Grouping Preview";
            // 
            // dgvVendorGroups
            // 
            dgvVendorGroups.AllowUserToAddRows = false;
            dgvVendorGroups.AllowUserToDeleteRows = false;
            dgvVendorGroups.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvVendorGroups.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvVendorGroups.Columns.AddRange(new DataGridViewColumn[] { colGroupVendor, colGroupItems, colGroupAmount, colGroupPRNumber });
            dgvVendorGroups.Location = new Point(7, 52);
            dgvVendorGroups.Margin = new Padding(4, 3, 4, 3);
            dgvVendorGroups.Name = "dgvVendorGroups";
            dgvVendorGroups.ReadOnly = true;
            dgvVendorGroups.RowHeadersVisible = false;
            dgvVendorGroups.Size = new Size(453, 91);
            dgvVendorGroups.TabIndex = 2;
            // 
            // colGroupVendor
            // 
            colGroupVendor.HeaderText = "Vendor";
            colGroupVendor.Name = "colGroupVendor";
            colGroupVendor.ReadOnly = true;
            // 
            // colGroupItems
            // 
            colGroupItems.HeaderText = "Items";
            colGroupItems.Name = "colGroupItems";
            colGroupItems.ReadOnly = true;
            // 
            // colGroupAmount
            // 
            colGroupAmount.HeaderText = "Amount";
            colGroupAmount.Name = "colGroupAmount";
            colGroupAmount.ReadOnly = true;
            // 
            // colGroupPRNumber
            // 
            colGroupPRNumber.HeaderText = "PR Number";
            colGroupPRNumber.Name = "colGroupPRNumber";
            colGroupPRNumber.ReadOnly = true;
            // 
            // lblGroupingInfo
            // 
            lblGroupingInfo.AutoSize = true;
            lblGroupingInfo.Font = new Font("Microsoft Sans Serif", 8F, FontStyle.Italic);
            lblGroupingInfo.Location = new Point(222, 27);
            lblGroupingInfo.Margin = new Padding(4, 0, 4, 0);
            lblGroupingInfo.Name = "lblGroupingInfo";
            lblGroupingInfo.Size = new Size(166, 13);
            lblGroupingInfo.TabIndex = 1;
            lblGroupingInfo.Text = "Items with same vendor = one PR";
            // 
            // chkGroupByVendor
            // 
            chkGroupByVendor.AutoSize = true;
            chkGroupByVendor.Location = new Point(18, 23);
            chkGroupByVendor.Margin = new Padding(4, 3, 4, 3);
            chkGroupByVendor.Name = "chkGroupByVendor";
            chkGroupByVendor.Size = new Size(180, 19);
            chkGroupByVendor.TabIndex = 0;
            chkGroupByVendor.Text = "Auto Group by Same Vendor";
            chkGroupByVendor.UseVisualStyleBackColor = true;
            chkGroupByVendor.CheckedChanged += chkGroupByVendor_CheckedChanged;
            // 
            // grpActions
            // 
            grpActions.Controls.Add(ApprovalPagebutton);
            grpActions.Controls.Add(btnClubPR);
            grpActions.Controls.Add(btnClose);
            grpActions.Controls.Add(btnPrint);
            grpActions.Controls.Add(btnExport);
            grpActions.Controls.Add(btnViewPRHistory);
            grpActions.Controls.Add(btnClearSelection);
            grpActions.Controls.Add(btnAutoGroup);
            grpActions.Controls.Add(btnGeneratePR);
            grpActions.Font = new Font("Microsoft Sans Serif", 9F);
            grpActions.Location = new Point(14, 738);
            grpActions.Margin = new Padding(4, 3, 4, 3);
            grpActions.Name = "grpActions";
            grpActions.Padding = new Padding(4, 3, 4, 3);
            grpActions.Size = new Size(1283, 69);
            grpActions.TabIndex = 5;
            grpActions.TabStop = false;
            grpActions.Text = "Actions";
            // 
            // ApprovalPagebutton
            // 
            ApprovalPagebutton.BackColor = Color.LightGreen;
            ApprovalPagebutton.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);
            ApprovalPagebutton.Location = new Point(1128, 23);
            ApprovalPagebutton.Margin = new Padding(4, 3, 4, 3);
            ApprovalPagebutton.Name = "ApprovalPagebutton";
            ApprovalPagebutton.Size = new Size(140, 35);
            ApprovalPagebutton.TabIndex = 8;
            ApprovalPagebutton.Text = "Approval PRs →";
            ApprovalPagebutton.UseVisualStyleBackColor = false;
            ApprovalPagebutton.Click += btnApprovalPR_Click;
            // 
            // btnClubPR
            // 
            btnClubPR.BackColor = Color.LightBlue;
            btnClubPR.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);
            btnClubPR.Location = new Point(980, 23);
            btnClubPR.Margin = new Padding(4, 3, 4, 3);
            btnClubPR.Name = "btnClubPR";
            btnClubPR.Size = new Size(140, 35);
            btnClubPR.TabIndex = 7;
            btnClubPR.Text = "Club PRs →";
            btnClubPR.UseVisualStyleBackColor = false;
            btnClubPR.Click += btnClubPR_Click;
            // 
            // btnClose
            // 
            btnClose.BackColor = Color.LightCoral;
            btnClose.Location = new Point(869, 23);
            btnClose.Margin = new Padding(4, 3, 4, 3);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(93, 35);
            btnClose.TabIndex = 6;
            btnClose.Text = "Close";
            btnClose.UseVisualStyleBackColor = false;
            btnClose.Click += btnClose_Click;
            // 
            // btnPrint
            // 
            btnPrint.BackColor = Color.LightSteelBlue;
            btnPrint.Location = new Point(758, 23);
            btnPrint.Margin = new Padding(4, 3, 4, 3);
            btnPrint.Name = "btnPrint";
            btnPrint.Size = new Size(93, 35);
            btnPrint.TabIndex = 5;
            btnPrint.Text = "Print";
            btnPrint.UseVisualStyleBackColor = false;
            btnPrint.Click += btnPrint_Click;
            // 
            // btnExport
            // 
            btnExport.BackColor = Color.LightSteelBlue;
            btnExport.Location = new Point(648, 23);
            btnExport.Margin = new Padding(4, 3, 4, 3);
            btnExport.Name = "btnExport";
            btnExport.Size = new Size(93, 35);
            btnExport.TabIndex = 4;
            btnExport.Text = "Export";
            btnExport.UseVisualStyleBackColor = false;
            btnExport.Click += btnExport_Click;
            // 
            // btnViewPRHistory
            // 
            btnViewPRHistory.BackColor = Color.LightSteelBlue;
            btnViewPRHistory.Location = new Point(490, 23);
            btnViewPRHistory.Margin = new Padding(4, 3, 4, 3);
            btnViewPRHistory.Name = "btnViewPRHistory";
            btnViewPRHistory.Size = new Size(140, 35);
            btnViewPRHistory.TabIndex = 3;
            btnViewPRHistory.Text = "View PR History";
            btnViewPRHistory.UseVisualStyleBackColor = false;
            btnViewPRHistory.Click += btnViewPRHistory_Click;
            // 
            // btnClearSelection
            // 
            btnClearSelection.BackColor = Color.LightYellow;
            btnClearSelection.Location = new Point(356, 23);
            btnClearSelection.Margin = new Padding(4, 3, 4, 3);
            btnClearSelection.Name = "btnClearSelection";
            btnClearSelection.Size = new Size(117, 35);
            btnClearSelection.TabIndex = 2;
            btnClearSelection.Text = "Clear";
            btnClearSelection.UseVisualStyleBackColor = false;
            btnClearSelection.Click += btnClearSelection_Click;
            // 
            // btnAutoGroup
            // 
            btnAutoGroup.BackColor = Color.LightBlue;
            btnAutoGroup.Location = new Point(187, 23);
            btnAutoGroup.Margin = new Padding(4, 3, 4, 3);
            btnAutoGroup.Name = "btnAutoGroup";
            btnAutoGroup.Size = new Size(152, 35);
            btnAutoGroup.TabIndex = 1;
            btnAutoGroup.Text = "Auto Group";
            btnAutoGroup.UseVisualStyleBackColor = false;
            btnAutoGroup.Click += btnAutoGroup_Click;
            // 
            // btnGeneratePR
            // 
            btnGeneratePR.BackColor = Color.LightGreen;
            btnGeneratePR.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);
            btnGeneratePR.Location = new Point(18, 23);
            btnGeneratePR.Margin = new Padding(4, 3, 4, 3);
            btnGeneratePR.Name = "btnGeneratePR";
            btnGeneratePR.Size = new Size(152, 35);
            btnGeneratePR.TabIndex = 0;
            btnGeneratePR.Text = "Generate PR(s)";
            btnGeneratePR.UseVisualStyleBackColor = false;
            btnGeneratePR.Click += btnGeneratePR_Click;
            // 
            // grpProjectInfo
            // 
            grpProjectInfo.Controls.Add(txtPRNumber);
            grpProjectInfo.Controls.Add(lblPRNumber);
            grpProjectInfo.Controls.Add(dtpCreatedDate);
            grpProjectInfo.Controls.Add(lblCreatedDate);
            grpProjectInfo.Controls.Add(txtCreatedBy);
            grpProjectInfo.Controls.Add(lblCreatedBy);
            grpProjectInfo.Controls.Add(txtProductName);
            grpProjectInfo.Controls.Add(lblProductName);
            grpProjectInfo.Controls.Add(txtProjectName);
            grpProjectInfo.Controls.Add(lblProjectName);
            grpProjectInfo.Font = new Font("Microsoft Sans Serif", 9F);
            grpProjectInfo.Location = new Point(735, 52);
            grpProjectInfo.Margin = new Padding(4, 3, 4, 3);
            grpProjectInfo.Name = "grpProjectInfo";
            grpProjectInfo.Padding = new Padding(4, 3, 4, 3);
            grpProjectInfo.Size = new Size(562, 162);
            grpProjectInfo.TabIndex = 6;
            grpProjectInfo.TabStop = false;
            grpProjectInfo.Text = "Project / PR Info";
            // 
            // txtPRNumber
            // 
            txtPRNumber.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);
            txtPRNumber.Location = new Point(350, 60);
            txtPRNumber.Margin = new Padding(4, 3, 4, 3);
            txtPRNumber.Name = "txtPRNumber";
            txtPRNumber.ReadOnly = true;
            txtPRNumber.Size = new Size(186, 21);
            txtPRNumber.TabIndex = 9;
            // 
            // lblPRNumber
            // 
            lblPRNumber.AutoSize = true;
            lblPRNumber.Location = new Point(350, 29);
            lblPRNumber.Margin = new Padding(4, 0, 4, 0);
            lblPRNumber.Name = "lblPRNumber";
            lblPRNumber.Size = new Size(75, 15);
            lblPRNumber.TabIndex = 8;
            lblPRNumber.Text = "PR Number:";
            // 
            // dtpCreatedDate
            // 
            dtpCreatedDate.Format = DateTimePickerFormat.Short;
            dtpCreatedDate.Location = new Point(397, 95);
            dtpCreatedDate.Margin = new Padding(4, 3, 4, 3);
            dtpCreatedDate.Name = "dtpCreatedDate";
            dtpCreatedDate.Size = new Size(139, 21);
            dtpCreatedDate.TabIndex = 7;
            // 
            // lblCreatedDate
            // 
            lblCreatedDate.AutoSize = true;
            lblCreatedDate.Location = new Point(350, 98);
            lblCreatedDate.Margin = new Padding(4, 0, 4, 0);
            lblCreatedDate.Name = "lblCreatedDate";
            lblCreatedDate.Size = new Size(36, 15);
            lblCreatedDate.TabIndex = 6;
            lblCreatedDate.Text = "Date:";
            // 
            // txtCreatedBy
            // 
            txtCreatedBy.Location = new Point(128, 95);
            txtCreatedBy.Margin = new Padding(4, 3, 4, 3);
            txtCreatedBy.Name = "txtCreatedBy";
            txtCreatedBy.ReadOnly = true;
            txtCreatedBy.Size = new Size(209, 21);
            txtCreatedBy.TabIndex = 5;
            // 
            // lblCreatedBy
            // 
            lblCreatedBy.AutoSize = true;
            lblCreatedBy.Location = new Point(18, 98);
            lblCreatedBy.Margin = new Padding(4, 0, 4, 0);
            lblCreatedBy.Name = "lblCreatedBy";
            lblCreatedBy.Size = new Size(69, 15);
            lblCreatedBy.TabIndex = 4;
            lblCreatedBy.Text = "Created By:";
            // 
            // txtProductName
            // 
            txtProductName.Location = new Point(128, 60);
            txtProductName.Margin = new Padding(4, 3, 4, 3);
            txtProductName.Name = "txtProductName";
            txtProductName.ReadOnly = true;
            txtProductName.Size = new Size(209, 21);
            txtProductName.TabIndex = 3;
            // 
            // lblProductName
            // 
            lblProductName.AutoSize = true;
            lblProductName.Location = new Point(18, 63);
            lblProductName.Margin = new Padding(4, 0, 4, 0);
            lblProductName.Name = "lblProductName";
            lblProductName.Size = new Size(89, 15);
            lblProductName.TabIndex = 2;
            lblProductName.Text = "Product Name:";
            // 
            // txtProjectName
            // 
            txtProjectName.Location = new Point(128, 25);
            txtProjectName.Margin = new Padding(4, 3, 4, 3);
            txtProjectName.Name = "txtProjectName";
            txtProjectName.ReadOnly = true;
            txtProjectName.Size = new Size(209, 21);
            txtProjectName.TabIndex = 1;
            // 
            // lblProjectName
            // 
            lblProjectName.AutoSize = true;
            lblProjectName.Location = new Point(18, 29);
            lblProjectName.Margin = new Padding(4, 0, 4, 0);
            lblProjectName.Name = "lblProjectName";
            lblProjectName.Size = new Size(85, 15);
            lblProjectName.TabIndex = 0;
            lblProjectName.Text = "Project Name:";
            // 
            // frmPurchaseRequestGeneration
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.WhiteSmoke;
            ClientSize = new Size(1311, 820);
            Controls.Add(grpProjectInfo);
            Controls.Add(grpActions);
            Controls.Add(grpVendorGrouping);
            Controls.Add(grpPRSummary);
            Controls.Add(grpBOMItems);
            Controls.Add(grpProjectSelection);
            Controls.Add(lblTitle);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Margin = new Padding(4, 3, 4, 3);
            MaximizeBox = false;
            Name = "frmPurchaseRequestGeneration";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Purchase Request Generation";
            Load += frmPurchaseRequestGeneration_Load;
            grpProjectSelection.ResumeLayout(false);
            grpProjectSelection.PerformLayout();
            grpBOMItems.ResumeLayout(false);
            grpBOMItems.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvBOMItems).EndInit();
            grpPRSummary.ResumeLayout(false);
            grpPRSummary.PerformLayout();
            grpVendorGrouping.ResumeLayout(false);
            grpVendorGrouping.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvVendorGroups).EndInit();
            grpActions.ResumeLayout(false);
            grpProjectInfo.ResumeLayout(false);
            grpProjectInfo.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.GroupBox grpProjectSelection;
        private System.Windows.Forms.Label lblProjectCode;
        private System.Windows.Forms.ComboBox cmbProjectCode;
        private System.Windows.Forms.Label lblProductNo;
        private System.Windows.Forms.ComboBox cmbProductNo;
        private System.Windows.Forms.Button btnLoadBOM;
        private System.Windows.Forms.Button btnRefreshProjects;
        private System.Windows.Forms.GroupBox grpBOMItems;
        private System.Windows.Forms.DataGridView dgvBOMItems;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colSelect;
        private System.Windows.Forms.DataGridViewTextBoxColumn colProjectBOMCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn colItemCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn colItemName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colProductCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn colProductNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn cloissuedQty;//cloissuedQty
        private System.Windows.Forms.DataGridViewTextBoxColumn colBOMQty;//
        private System.Windows.Forms.DataGridViewTextBoxColumn colConvertedQty;//colConvertedQty
        private System.Windows.Forms.DataGridViewTextBoxColumn currentPendingQty;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAlreadyPurchased;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAvaiableQty;//colAvaiableQty
        private System.Windows.Forms.DataGridViewTextBoxColumn colBalanceQty;
        private System.Windows.Forms.DataGridViewTextBoxColumn colUnitCost;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTotalCost;
        private System.Windows.Forms.DataGridViewComboBoxColumn colVendor;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPRQty;//collocation colHSNCode
        private System.Windows.Forms.DataGridViewTextBoxColumn collocation;//collocation colHSNCode
        private System.Windows.Forms.DataGridViewTextBoxColumn colHSNCode;//collocation colHSNCode
        private System.Windows.Forms.DataGridViewTextBoxColumn colStatus;
        private System.Windows.Forms.GroupBox grpPRSummary;
        private System.Windows.Forms.Label lblTotalItems;
        private System.Windows.Forms.TextBox txtTotalItems;
        private System.Windows.Forms.Label lblSelectedItems;
        private System.Windows.Forms.TextBox txtSelectedItems;
        private System.Windows.Forms.Label lblTotalAmount;
        private System.Windows.Forms.TextBox txtTotalAmount;
        private System.Windows.Forms.Label lblLimitRemaining;
        private System.Windows.Forms.TextBox txtLimitRemaining;
        private System.Windows.Forms.Label lblLimitStatus;
        private System.Windows.Forms.GroupBox grpVendorGrouping;
        private System.Windows.Forms.CheckBox chkGroupByVendor;
        private System.Windows.Forms.Label lblGroupingInfo;
        private System.Windows.Forms.DataGridView dgvVendorGroups;
        private System.Windows.Forms.DataGridViewTextBoxColumn colGroupVendor;
        private System.Windows.Forms.DataGridViewTextBoxColumn colGroupItems;
        private System.Windows.Forms.DataGridViewTextBoxColumn colGroupAmount;
        private System.Windows.Forms.DataGridViewTextBoxColumn colGroupPRNumber;
        private System.Windows.Forms.GroupBox grpActions;
        private System.Windows.Forms.Button btnGeneratePR;
        private System.Windows.Forms.Button btnAutoGroup;
        private System.Windows.Forms.Button btnClearSelection;
        private System.Windows.Forms.Button btnViewPRHistory;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnClubPR;
        private System.Windows.Forms.GroupBox grpProjectInfo;
        private System.Windows.Forms.Label lblProjectName;
        private System.Windows.Forms.TextBox txtProjectName;
        private System.Windows.Forms.Label lblProductName;
        private System.Windows.Forms.TextBox txtProductName;
        private System.Windows.Forms.Label lblCreatedBy;
        private System.Windows.Forms.TextBox txtCreatedBy;
        private System.Windows.Forms.Label lblCreatedDate;
        private System.Windows.Forms.DateTimePicker dtpCreatedDate;
        private System.Windows.Forms.Label lblPRNumber;
        private System.Windows.Forms.TextBox txtPRNumber;
        private Button ApprovalPagebutton;
        private CheckBox checkAll;
    }
}