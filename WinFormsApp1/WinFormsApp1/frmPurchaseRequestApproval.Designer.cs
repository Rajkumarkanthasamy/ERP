//namespace WinFormsApp1
//{
//    partial class frmPurchaseRequestApproval
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
//            this.Text = "frmPurchaseRequestApproval";
//        }

//        #endregion
//    }
//}

namespace WinFormsApp1
{
    partial class frmPurchaseRequestApproval
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

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            lblTitle = new Label();
            pnlFilters = new Panel();
            btnClearFilter = new Button();
            btnApplyFilter = new Button();
            txtSearchPR = new TextBox();
            lblSearch = new Label();
            cmbVendor = new ComboBox();
            lblVendor = new Label();
            cmbProject = new ComboBox();
            lblProject = new Label();
            cmbStatus = new ComboBox();
            lblStatus = new Label();
            pnlStats = new Panel();
            lblTotalAmount = new Label();
            lblTotalAmountTitle = new Label();
            lblOnHoldCount = new Label();
            lblOnHoldTitle = new Label();
            lblRejectedCount = new Label();
            lblRejectedTitle = new Label();
            lblApprovedCount = new Label();
            lblApprovedTitle = new Label();
            lblPendingCount = new Label();
            lblPendingTitle = new Label();
            dgvPRList = new DataGridView();
            Select = new DataGridViewCheckBoxColumn();
            PRNumber = new DataGridViewTextBoxColumn();
            ProjectCode = new DataGridViewTextBoxColumn();
            VendorName = new DataGridViewTextBoxColumn();
            ItemCount = new DataGridViewTextBoxColumn();
            TotalAmount = new DataGridViewTextBoxColumn();
            Status = new DataGridViewTextBoxColumn();
            RequestedBy = new DataGridViewTextBoxColumn();
            RequestDate = new DataGridViewTextBoxColumn();
            pnlDetails = new Panel();
            panel2 = new Panel();
            dgvLineItems = new DataGridView();
            ItemCode = new DataGridViewTextBoxColumn();
            ItemDescription = new DataGridViewTextBoxColumn();
            Quantity = new DataGridViewTextBoxColumn();
            UOM = new DataGridViewTextBoxColumn();
            UnitCost = new DataGridViewTextBoxColumn();
            TotalCost = new DataGridViewTextBoxColumn();
            pnlActions = new Panel();
            btnSubmitApproval = new Button();
            btnHold = new Button();
            btnReject = new Button();
            btnApprove = new Button();
            txtApprovalRemarks = new TextBox();
            lblApprovalRemarks = new Label();
            cmbApprovalAction = new ComboBox();
            lblApprovalAction = new Label();
            lblActionTitle = new Label();
            lblLineItemsTitle = new Label();
            lblDetailAmount = new Label();
            lblDetailAmountLabel = new Label();
            lblDetailRequestor = new Label();
            lblDetailRequestorLabel = new Label();
            lblDetailVendor = new Label();
            lblDetailVendorLabel = new Label();
            lblDetailProject = new Label();
            lblDetailProjectLabel = new Label();
            lblDetailStatus = new Label();
            lblDetailPRNumber = new Label();
            lblDetailTitle = new Label();
            pnlButtons = new Panel();
            btnClose = new Button();
            btnPrint = new Button();
            btnExport = new Button();
            btnRefresh = new Button();
            btnBulkReject = new Button();
            btnBulkApprove = new Button();
            lblCurrentUser = new Label();
            panel1 = new Panel();
            pnlFilters.SuspendLayout();
            pnlStats.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvPRList).BeginInit();
            pnlDetails.SuspendLayout();
            panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvLineItems).BeginInit();
            pnlActions.SuspendLayout();
            pnlButtons.SuspendLayout();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTitle.ForeColor = Color.FromArgb(26, 35, 126);
            lblTitle.Location = new Point(20, 15);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(327, 32);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Purchase Request Approval";
            // 
            // pnlFilters
            // 
            pnlFilters.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pnlFilters.BackColor = Color.White;
            pnlFilters.BorderStyle = BorderStyle.FixedSingle;
            pnlFilters.Controls.Add(btnClearFilter);
            pnlFilters.Controls.Add(btnApplyFilter);
            pnlFilters.Controls.Add(txtSearchPR);
            pnlFilters.Controls.Add(lblSearch);
            pnlFilters.Controls.Add(cmbVendor);
            pnlFilters.Controls.Add(lblVendor);
            pnlFilters.Controls.Add(cmbProject);
            pnlFilters.Controls.Add(lblProject);
            pnlFilters.Controls.Add(cmbStatus);
            pnlFilters.Controls.Add(lblStatus);
            pnlFilters.Location = new Point(20, 60);
            pnlFilters.Name = "pnlFilters";
            pnlFilters.Size = new Size(1140, 55);
            pnlFilters.TabIndex = 1;
            // 
            // btnClearFilter
            // 
            btnClearFilter.BackColor = Color.FromArgb(245, 245, 245);
            btnClearFilter.FlatStyle = FlatStyle.Flat;
            btnClearFilter.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnClearFilter.Location = new Point(955, 12);
            btnClearFilter.Name = "btnClearFilter";
            btnClearFilter.Size = new Size(90, 28);
            btnClearFilter.TabIndex = 9;
            btnClearFilter.Text = "Clear";
            btnClearFilter.UseVisualStyleBackColor = false;
            btnClearFilter.Click += btnClearFilter_Click;
            // 
            // btnApplyFilter
            // 
            btnApplyFilter.BackColor = Color.FromArgb(26, 35, 126);
            btnApplyFilter.FlatStyle = FlatStyle.Flat;
            btnApplyFilter.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnApplyFilter.ForeColor = Color.White;
            btnApplyFilter.Location = new Point(855, 12);
            btnApplyFilter.Name = "btnApplyFilter";
            btnApplyFilter.Size = new Size(90, 28);
            btnApplyFilter.TabIndex = 8;
            btnApplyFilter.Text = "Apply";
            btnApplyFilter.UseVisualStyleBackColor = false;
            btnApplyFilter.Click += btnApplyFilter_Click;
            // 
            // txtSearchPR
            // 
            txtSearchPR.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtSearchPR.Location = new Point(660, 15);
            txtSearchPR.Name = "txtSearchPR";
            txtSearchPR.Size = new Size(180, 23);
            txtSearchPR.TabIndex = 7;
            // 
            // lblSearch
            // 
            lblSearch.AutoSize = true;
            lblSearch.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblSearch.Location = new Point(600, 18);
            lblSearch.Name = "lblSearch";
            lblSearch.Size = new Size(55, 15);
            lblSearch.TabIndex = 6;
            lblSearch.Text = "Search #:";
            // 
            // cmbVendor
            // 
            cmbVendor.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbVendor.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            cmbVendor.FormattingEnabled = true;
            cmbVendor.Location = new Point(445, 15);
            cmbVendor.Name = "cmbVendor";
            cmbVendor.Size = new Size(140, 23);
            cmbVendor.TabIndex = 5;
            // 
            // lblVendor
            // 
            lblVendor.AutoSize = true;
            lblVendor.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblVendor.Location = new Point(395, 18);
            lblVendor.Name = "lblVendor";
            lblVendor.Size = new Size(47, 15);
            lblVendor.TabIndex = 4;
            lblVendor.Text = "Vendor:";
            // 
            // cmbProject
            // 
            cmbProject.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbProject.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            cmbProject.FormattingEnabled = true;
            cmbProject.Location = new Point(240, 15);
            cmbProject.Name = "cmbProject";
            cmbProject.Size = new Size(140, 23);
            cmbProject.TabIndex = 3;
            // 
            // lblProject
            // 
            lblProject.AutoSize = true;
            lblProject.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblProject.Location = new Point(190, 18);
            lblProject.Name = "lblProject";
            lblProject.Size = new Size(47, 15);
            lblProject.TabIndex = 2;
            lblProject.Text = "Project:";
            // 
            // cmbStatus
            // 
            cmbStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbStatus.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            cmbStatus.FormattingEnabled = true;
            cmbStatus.Items.AddRange(new object[] { "All", "Pending", "Approved", "Rejected", "On Hold" });
            cmbStatus.Location = new Point(55, 15);
            cmbStatus.Name = "cmbStatus";
            cmbStatus.Size = new Size(120, 23);
            cmbStatus.TabIndex = 1;
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblStatus.Location = new Point(10, 18);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(42, 15);
            lblStatus.TabIndex = 0;
            lblStatus.Text = "Status:";
            // 
            // pnlStats
            // 
            pnlStats.Controls.Add(lblTotalAmount);
            pnlStats.Controls.Add(lblTotalAmountTitle);
            pnlStats.Controls.Add(lblOnHoldCount);
            pnlStats.Controls.Add(lblOnHoldTitle);
            pnlStats.Controls.Add(lblRejectedCount);
            pnlStats.Controls.Add(lblRejectedTitle);
            pnlStats.Controls.Add(lblApprovedCount);
            pnlStats.Controls.Add(lblApprovedTitle);
            pnlStats.Controls.Add(lblPendingCount);
            pnlStats.Controls.Add(lblPendingTitle);
            pnlStats.Location = new Point(20, 125);
            pnlStats.Name = "pnlStats";
            pnlStats.Size = new Size(1140, 70);
            pnlStats.TabIndex = 2;
            // 
            // lblTotalAmount
            // 
            lblTotalAmount.AutoSize = true;
            lblTotalAmount.Font = new Font("Segoe UI", 20F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTotalAmount.ForeColor = Color.FromArgb(21, 101, 192);
            lblTotalAmount.Location = new Point(890, 28);
            lblTotalAmount.Name = "lblTotalAmount";
            lblTotalAmount.Size = new Size(77, 37);
            lblTotalAmount.TabIndex = 9;
            lblTotalAmount.Text = "Rs. 0";
            // 
            // lblTotalAmountTitle
            // 
            lblTotalAmountTitle.AutoSize = true;
            lblTotalAmountTitle.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTotalAmountTitle.ForeColor = Color.FromArgb(21, 101, 192);
            lblTotalAmountTitle.Location = new Point(890, 10);
            lblTotalAmountTitle.Name = "lblTotalAmountTitle";
            lblTotalAmountTitle.Size = new Size(99, 15);
            lblTotalAmountTitle.TabIndex = 8;
            lblTotalAmountTitle.Text = "TOTAL AMOUNT";
            // 
            // lblOnHoldCount
            // 
            lblOnHoldCount.AutoSize = true;
            lblOnHoldCount.Font = new Font("Segoe UI", 20F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblOnHoldCount.ForeColor = Color.FromArgb(21, 101, 192);
            lblOnHoldCount.Location = new Point(670, 28);
            lblOnHoldCount.Name = "lblOnHoldCount";
            lblOnHoldCount.Size = new Size(33, 37);
            lblOnHoldCount.TabIndex = 7;
            lblOnHoldCount.Text = "0";
            // 
            // lblOnHoldTitle
            // 
            lblOnHoldTitle.AutoSize = true;
            lblOnHoldTitle.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblOnHoldTitle.ForeColor = Color.FromArgb(21, 101, 192);
            lblOnHoldTitle.Location = new Point(670, 10);
            lblOnHoldTitle.Name = "lblOnHoldTitle";
            lblOnHoldTitle.Size = new Size(61, 15);
            lblOnHoldTitle.TabIndex = 6;
            lblOnHoldTitle.Text = "ON HOLD";
            // 
            // lblRejectedCount
            // 
            lblRejectedCount.AutoSize = true;
            lblRejectedCount.Font = new Font("Segoe UI", 20F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblRejectedCount.ForeColor = Color.FromArgb(198, 40, 40);
            lblRejectedCount.Location = new Point(450, 28);
            lblRejectedCount.Name = "lblRejectedCount";
            lblRejectedCount.Size = new Size(33, 37);
            lblRejectedCount.TabIndex = 5;
            lblRejectedCount.Text = "0";
            // 
            // lblRejectedTitle
            // 
            lblRejectedTitle.AutoSize = true;
            lblRejectedTitle.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblRejectedTitle.ForeColor = Color.FromArgb(198, 40, 40);
            lblRejectedTitle.Location = new Point(450, 10);
            lblRejectedTitle.Name = "lblRejectedTitle";
            lblRejectedTitle.Size = new Size(61, 15);
            lblRejectedTitle.TabIndex = 4;
            lblRejectedTitle.Text = "REJECTED";
            // 
            // lblApprovedCount
            // 
            lblApprovedCount.AutoSize = true;
            lblApprovedCount.Font = new Font("Segoe UI", 20F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblApprovedCount.ForeColor = Color.FromArgb(46, 125, 50);
            lblApprovedCount.Location = new Point(230, 28);
            lblApprovedCount.Name = "lblApprovedCount";
            lblApprovedCount.Size = new Size(33, 37);
            lblApprovedCount.TabIndex = 3;
            lblApprovedCount.Text = "0";
            // 
            // lblApprovedTitle
            // 
            lblApprovedTitle.AutoSize = true;
            lblApprovedTitle.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblApprovedTitle.ForeColor = Color.FromArgb(46, 125, 50);
            lblApprovedTitle.Location = new Point(230, 10);
            lblApprovedTitle.Name = "lblApprovedTitle";
            lblApprovedTitle.Size = new Size(69, 15);
            lblApprovedTitle.TabIndex = 2;
            lblApprovedTitle.Text = "APPROVED";
            // 
            // lblPendingCount
            // 
            lblPendingCount.AutoSize = true;
            lblPendingCount.Font = new Font("Segoe UI", 20F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblPendingCount.ForeColor = Color.FromArgb(230, 81, 0);
            lblPendingCount.Location = new Point(10, 28);
            lblPendingCount.Name = "lblPendingCount";
            lblPendingCount.Size = new Size(33, 37);
            lblPendingCount.TabIndex = 1;
            lblPendingCount.Text = "0";
            // 
            // lblPendingTitle
            // 
            lblPendingTitle.AutoSize = true;
            lblPendingTitle.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblPendingTitle.ForeColor = Color.FromArgb(230, 81, 0);
            lblPendingTitle.Location = new Point(10, 10);
            lblPendingTitle.Name = "lblPendingTitle";
            lblPendingTitle.Size = new Size(60, 15);
            lblPendingTitle.TabIndex = 0;
            lblPendingTitle.Text = "PENDING";
            // 
            // dgvPRList
            // 
            dgvPRList.AllowUserToAddRows = false;
            dgvPRList.AllowUserToDeleteRows = false;
            dgvPRList.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            dgvPRList.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvPRList.BackgroundColor = Color.White;
            dgvPRList.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvPRList.Columns.AddRange(new DataGridViewColumn[] { Select, PRNumber, ProjectCode, VendorName, ItemCount, TotalAmount, Status, RequestedBy, RequestDate });
            dgvPRList.Location = new Point(0, 0);
            dgvPRList.MultiSelect = false;
            dgvPRList.Name = "dgvPRList";
            dgvPRList.ReadOnly = true;
            dgvPRList.RowHeadersVisible = false;
            dgvPRList.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPRList.Size = new Size(1140, 228);
            dgvPRList.TabIndex = 3;
            dgvPRList.CellClick += dgvPRList_CellClick;
            // 
            // Select
            // 
            Select.HeaderText = "";
            Select.Name = "Select";
            Select.ReadOnly = true;
            // 
            // PRNumber
            // 
            PRNumber.DataPropertyName = "PRNumber";
            PRNumber.HeaderText = "PR Number";
            PRNumber.Name = "PRNumber";
            PRNumber.ReadOnly = true;
            // 
            // ProjectCode
            // 
            ProjectCode.DataPropertyName = "ProjectCode";
            ProjectCode.HeaderText = "Project";
            ProjectCode.Name = "ProjectCode";
            ProjectCode.ReadOnly = true;
            // 
            // VendorName
            // 
            VendorName.DataPropertyName = "VendorName";
            VendorName.HeaderText = "Vendor";
            VendorName.Name = "VendorName";
            VendorName.ReadOnly = true;
            // 
            // ItemCount
            // 
            ItemCount.DataPropertyName = "ItemCount";
            ItemCount.HeaderText = "Items";
            ItemCount.Name = "ItemCount";
            ItemCount.ReadOnly = true;
            // 
            // TotalAmount
            // 
            TotalAmount.DataPropertyName = "TotalAmount";
            TotalAmount.HeaderText = "Total Amount";
            TotalAmount.Name = "TotalAmount";
            TotalAmount.ReadOnly = true;
            // 
            // Status
            // 
            Status.DataPropertyName = "Status";
            Status.HeaderText = "Status";
            Status.Name = "Status";
            Status.ReadOnly = true;
            // 
            // RequestedBy
            // 
            RequestedBy.DataPropertyName = "RequestedBy";
            RequestedBy.HeaderText = "Requested By";
            RequestedBy.Name = "RequestedBy";
            RequestedBy.ReadOnly = true;
            // 
            // RequestDate
            // 
            RequestDate.DataPropertyName = "RequestDate";
            RequestDate.HeaderText = "Date";
            RequestDate.Name = "RequestDate";
            RequestDate.ReadOnly = true;
            // 
            // pnlDetails
            // 
            pnlDetails.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pnlDetails.BackColor = Color.White;
            pnlDetails.BorderStyle = BorderStyle.FixedSingle;
            pnlDetails.Controls.Add(panel2);
            pnlDetails.Controls.Add(pnlActions);
            pnlDetails.Controls.Add(lblLineItemsTitle);
            pnlDetails.Controls.Add(lblDetailAmount);
            pnlDetails.Controls.Add(lblDetailAmountLabel);
            pnlDetails.Controls.Add(lblDetailRequestor);
            pnlDetails.Controls.Add(lblDetailRequestorLabel);
            pnlDetails.Controls.Add(lblDetailVendor);
            pnlDetails.Controls.Add(lblDetailVendorLabel);
            pnlDetails.Controls.Add(lblDetailProject);
            pnlDetails.Controls.Add(lblDetailProjectLabel);
            pnlDetails.Controls.Add(lblDetailStatus);
            pnlDetails.Controls.Add(lblDetailPRNumber);
            pnlDetails.Controls.Add(lblDetailTitle);
            pnlDetails.Location = new Point(20, 435);
            pnlDetails.Name = "pnlDetails";
            pnlDetails.Size = new Size(1140, 320);
            pnlDetails.TabIndex = 4;
            pnlDetails.Visible = false;
            // 
            // panel2
            // 
            panel2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panel2.Controls.Add(dgvLineItems);
            panel2.Location = new Point(11, 98);
            panel2.Name = "panel2";
            panel2.Size = new Size(1119, 136);
            panel2.TabIndex = 14;
            // 
            // dgvLineItems
            // 
            dgvLineItems.AllowUserToAddRows = false;
            dgvLineItems.AllowUserToDeleteRows = false;
            dgvLineItems.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            dgvLineItems.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvLineItems.BackgroundColor = Color.White;
            dgvLineItems.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvLineItems.Columns.AddRange(new DataGridViewColumn[] { ItemCode, ItemDescription, Quantity, UOM, UnitCost, TotalCost });
            dgvLineItems.Location = new Point(0, 0);
            dgvLineItems.Name = "dgvLineItems";
            dgvLineItems.ReadOnly = true;
            dgvLineItems.RowHeadersVisible = false;
            dgvLineItems.Size = new Size(1119, 136);
            dgvLineItems.TabIndex = 12;
            // 
            // ItemCode
            // 
            ItemCode.HeaderText = "Item Code";
            ItemCode.Name = "ItemCode";
            ItemCode.ReadOnly = true;
            // 
            // ItemDescription
            // 
            ItemDescription.HeaderText = "Description";
            ItemDescription.Name = "ItemDescription";
            ItemDescription.ReadOnly = true;
            // 
            // Quantity
            // 
            Quantity.HeaderText = "Qty";
            Quantity.Name = "Quantity";
            Quantity.ReadOnly = true;
            // 
            // UOM
            // 
            UOM.HeaderText = "UOM";
            UOM.Name = "UOM";
            UOM.ReadOnly = true;
            // 
            // UnitCost
            // 
            UnitCost.HeaderText = "Unit Cost";
            UnitCost.Name = "UnitCost";
            UnitCost.ReadOnly = true;
            // 
            // TotalCost
            // 
            TotalCost.HeaderText = "Total";
            TotalCost.Name = "TotalCost";
            TotalCost.ReadOnly = true;
            // 
            // pnlActions
            // 
            pnlActions.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pnlActions.BackColor = Color.FromArgb(248, 249, 250);
            pnlActions.Controls.Add(btnSubmitApproval);
            pnlActions.Controls.Add(btnHold);
            pnlActions.Controls.Add(btnReject);
            pnlActions.Controls.Add(btnApprove);
            pnlActions.Controls.Add(txtApprovalRemarks);
            pnlActions.Controls.Add(lblApprovalRemarks);
            pnlActions.Controls.Add(cmbApprovalAction);
            pnlActions.Controls.Add(lblApprovalAction);
            pnlActions.Controls.Add(lblActionTitle);
            pnlActions.Location = new Point(11, 240);
            pnlActions.Name = "pnlActions";
            pnlActions.Size = new Size(1120, 75);
            pnlActions.TabIndex = 13;
            // 
            // btnSubmitApproval
            // 
            btnSubmitApproval.BackColor = Color.FromArgb(26, 35, 126);
            btnSubmitApproval.FlatStyle = FlatStyle.Flat;
            btnSubmitApproval.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnSubmitApproval.ForeColor = Color.White;
            btnSubmitApproval.Location = new Point(960, 30);
            btnSubmitApproval.Name = "btnSubmitApproval";
            btnSubmitApproval.Size = new Size(100, 28);
            btnSubmitApproval.TabIndex = 8;
            btnSubmitApproval.Text = "Submit";
            btnSubmitApproval.UseVisualStyleBackColor = false;
            btnSubmitApproval.Click += btnSubmitApproval_Click;
            // 
            // btnHold
            // 
            btnHold.BackColor = Color.FromArgb(255, 152, 0);
            btnHold.FlatStyle = FlatStyle.Flat;
            btnHold.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnHold.ForeColor = Color.White;
            btnHold.Location = new Point(860, 30);
            btnHold.Name = "btnHold";
            btnHold.Size = new Size(90, 28);
            btnHold.TabIndex = 7;
            btnHold.Text = "On Hold";
            btnHold.UseVisualStyleBackColor = false;
            btnHold.Click += btnHold_Click;
            // 
            // btnReject
            // 
            btnReject.BackColor = Color.FromArgb(244, 67, 54);
            btnReject.FlatStyle = FlatStyle.Flat;
            btnReject.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnReject.ForeColor = Color.White;
            btnReject.Location = new Point(760, 30);
            btnReject.Name = "btnReject";
            btnReject.Size = new Size(90, 28);
            btnReject.TabIndex = 6;
            btnReject.Text = "Reject";
            btnReject.UseVisualStyleBackColor = false;
            btnReject.Click += btnReject_Click;
            // 
            // btnApprove
            // 
            btnApprove.BackColor = Color.FromArgb(76, 175, 80);
            btnApprove.FlatStyle = FlatStyle.Flat;
            btnApprove.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnApprove.ForeColor = Color.White;
            btnApprove.Location = new Point(660, 30);
            btnApprove.Name = "btnApprove";
            btnApprove.Size = new Size(90, 28);
            btnApprove.TabIndex = 5;
            btnApprove.Text = "Approve";
            btnApprove.UseVisualStyleBackColor = false;
            btnApprove.Click += btnApprove_Click;
            // 
            // txtApprovalRemarks
            // 
            txtApprovalRemarks.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtApprovalRemarks.Location = new Point(245, 32);
            txtApprovalRemarks.Name = "txtApprovalRemarks";
            txtApprovalRemarks.Size = new Size(400, 23);
            txtApprovalRemarks.TabIndex = 4;
            // 
            // lblApprovalRemarks
            // 
            lblApprovalRemarks.AutoSize = true;
            lblApprovalRemarks.Font = new Font("Segoe UI", 8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblApprovalRemarks.Location = new Point(190, 35);
            lblApprovalRemarks.Name = "lblApprovalRemarks";
            lblApprovalRemarks.Size = new Size(53, 13);
            lblApprovalRemarks.TabIndex = 3;
            lblApprovalRemarks.Text = "Remarks:";
            // 
            // cmbApprovalAction
            // 
            cmbApprovalAction.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbApprovalAction.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            cmbApprovalAction.FormattingEnabled = true;
            cmbApprovalAction.Items.AddRange(new object[] { "Approve", "Reject", "On Hold" });
            cmbApprovalAction.Location = new Point(55, 32);
            cmbApprovalAction.Name = "cmbApprovalAction";
            cmbApprovalAction.Size = new Size(120, 23);
            cmbApprovalAction.TabIndex = 2;
            // 
            // lblApprovalAction
            // 
            lblApprovalAction.AutoSize = true;
            lblApprovalAction.Font = new Font("Segoe UI", 8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblApprovalAction.Location = new Point(10, 35);
            lblApprovalAction.Name = "lblApprovalAction";
            lblApprovalAction.Size = new Size(43, 13);
            lblApprovalAction.TabIndex = 1;
            lblApprovalAction.Text = "Action:";
            // 
            // lblActionTitle
            // 
            lblActionTitle.AutoSize = true;
            lblActionTitle.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblActionTitle.Location = new Point(10, 10);
            lblActionTitle.Name = "lblActionTitle";
            lblActionTitle.Size = new Size(96, 15);
            lblActionTitle.TabIndex = 0;
            lblActionTitle.Text = "Approval Action";
            // 
            // lblLineItemsTitle
            // 
            lblLineItemsTitle.AutoSize = true;
            lblLineItemsTitle.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblLineItemsTitle.Location = new Point(10, 80);
            lblLineItemsTitle.Name = "lblLineItemsTitle";
            lblLineItemsTitle.Size = new Size(65, 15);
            lblLineItemsTitle.TabIndex = 11;
            lblLineItemsTitle.Text = "Line Items";
            // 
            // lblDetailAmount
            // 
            lblDetailAmount.AutoSize = true;
            lblDetailAmount.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblDetailAmount.Location = new Point(600, 60);
            lblDetailAmount.Name = "lblDetailAmount";
            lblDetailAmount.Size = new Size(50, 15);
            lblDetailAmount.TabIndex = 10;
            lblDetailAmount.Text = "Rs. 0.00";
            // 
            // lblDetailAmountLabel
            // 
            lblDetailAmountLabel.AutoSize = true;
            lblDetailAmountLabel.Font = new Font("Segoe UI", 8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblDetailAmountLabel.ForeColor = Color.Gray;
            lblDetailAmountLabel.Location = new Point(600, 45);
            lblDetailAmountLabel.Name = "lblDetailAmountLabel";
            lblDetailAmountLabel.Size = new Size(75, 13);
            lblDetailAmountLabel.TabIndex = 9;
            lblDetailAmountLabel.Text = "Total Amount";
            // 
            // lblDetailRequestor
            // 
            lblDetailRequestor.AutoSize = true;
            lblDetailRequestor.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblDetailRequestor.Location = new Point(400, 60);
            lblDetailRequestor.Name = "lblDetailRequestor";
            lblDetailRequestor.Size = new Size(72, 15);
            lblDetailRequestor.TabIndex = 8;
            lblDetailRequestor.Text = "Mr. Santosh";
            // 
            // lblDetailRequestorLabel
            // 
            lblDetailRequestorLabel.AutoSize = true;
            lblDetailRequestorLabel.Font = new Font("Segoe UI", 8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblDetailRequestorLabel.ForeColor = Color.Gray;
            lblDetailRequestorLabel.Location = new Point(400, 45);
            lblDetailRequestorLabel.Name = "lblDetailRequestorLabel";
            lblDetailRequestorLabel.Size = new Size(77, 13);
            lblDetailRequestorLabel.TabIndex = 7;
            lblDetailRequestorLabel.Text = "Requested By";
            // 
            // lblDetailVendor
            // 
            lblDetailVendor.AutoSize = true;
            lblDetailVendor.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblDetailVendor.Location = new Point(200, 60);
            lblDetailVendor.Name = "lblDetailVendor";
            lblDetailVendor.Size = new Size(84, 15);
            lblDetailVendor.TabIndex = 6;
            lblDetailVendor.Text = "ABC Suppliers";
            // 
            // lblDetailVendorLabel
            // 
            lblDetailVendorLabel.AutoSize = true;
            lblDetailVendorLabel.Font = new Font("Segoe UI", 8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblDetailVendorLabel.ForeColor = Color.Gray;
            lblDetailVendorLabel.Location = new Point(200, 45);
            lblDetailVendorLabel.Name = "lblDetailVendorLabel";
            lblDetailVendorLabel.Size = new Size(44, 13);
            lblDetailVendorLabel.TabIndex = 5;
            lblDetailVendorLabel.Text = "Vendor";
            // 
            // lblDetailProject
            // 
            lblDetailProject.AutoSize = true;
            lblDetailProject.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblDetailProject.Location = new Point(10, 60);
            lblDetailProject.Name = "lblDetailProject";
            lblDetailProject.Size = new Size(53, 15);
            lblDetailProject.TabIndex = 4;
            lblDetailProject.Text = "PRJ-001";
            // 
            // lblDetailProjectLabel
            // 
            lblDetailProjectLabel.AutoSize = true;
            lblDetailProjectLabel.Font = new Font("Segoe UI", 8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblDetailProjectLabel.ForeColor = Color.Gray;
            lblDetailProjectLabel.Location = new Point(10, 45);
            lblDetailProjectLabel.Name = "lblDetailProjectLabel";
            lblDetailProjectLabel.Size = new Size(42, 13);
            lblDetailProjectLabel.TabIndex = 3;
            lblDetailProjectLabel.Text = "Project";
            // 
            // lblDetailStatus
            // 
            lblDetailStatus.AutoSize = true;
            lblDetailStatus.BackColor = Color.FromArgb(255, 243, 224);
            lblDetailStatus.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblDetailStatus.ForeColor = Color.FromArgb(230, 81, 0);
            lblDetailStatus.Location = new Point(260, 13);
            lblDetailStatus.Name = "lblDetailStatus";
            lblDetailStatus.Padding = new Padding(8, 2, 8, 2);
            lblDetailStatus.Size = new Size(68, 19);
            lblDetailStatus.TabIndex = 2;
            lblDetailStatus.Text = "Pending";
            // 
            // lblDetailPRNumber
            // 
            lblDetailPRNumber.AutoSize = true;
            lblDetailPRNumber.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblDetailPRNumber.ForeColor = Color.FromArgb(26, 35, 126);
            lblDetailPRNumber.Location = new Point(120, 10);
            lblDetailPRNumber.Name = "lblDetailPRNumber";
            lblDetailPRNumber.Size = new Size(114, 21);
            lblDetailPRNumber.TabIndex = 1;
            lblDetailPRNumber.Text = "PR-2026-0000";
            // 
            // lblDetailTitle
            // 
            lblDetailTitle.AutoSize = true;
            lblDetailTitle.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblDetailTitle.ForeColor = Color.FromArgb(26, 35, 126);
            lblDetailTitle.Location = new Point(10, 10);
            lblDetailTitle.Name = "lblDetailTitle";
            lblDetailTitle.Size = new Size(91, 21);
            lblDetailTitle.TabIndex = 0;
            lblDetailTitle.Text = "PR Details:";
            // 
            // pnlButtons
            // 
            pnlButtons.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pnlButtons.Controls.Add(btnClose);
            pnlButtons.Controls.Add(btnPrint);
            pnlButtons.Controls.Add(btnExport);
            pnlButtons.Controls.Add(btnRefresh);
            pnlButtons.Controls.Add(btnBulkReject);
            pnlButtons.Controls.Add(btnBulkApprove);
            pnlButtons.Location = new Point(20, 760);
            pnlButtons.Name = "pnlButtons";
            pnlButtons.Size = new Size(1140, 40);
            pnlButtons.TabIndex = 5;
            // 
            // btnClose
            // 
            btnClose.BackColor = Color.FromArgb(117, 117, 117);
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnClose.ForeColor = Color.White;
            btnClose.Location = new Point(1050, 5);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(90, 30);
            btnClose.TabIndex = 5;
            btnClose.Text = "Close";
            btnClose.UseVisualStyleBackColor = false;
            btnClose.Click += btnClose_Click;
            // 
            // btnPrint
            // 
            btnPrint.BackColor = Color.FromArgb(96, 125, 139);
            btnPrint.FlatStyle = FlatStyle.Flat;
            btnPrint.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnPrint.ForeColor = Color.White;
            btnPrint.Location = new Point(460, 5);
            btnPrint.Name = "btnPrint";
            btnPrint.Size = new Size(90, 30);
            btnPrint.TabIndex = 4;
            btnPrint.Text = "Print";
            btnPrint.UseVisualStyleBackColor = false;
            btnPrint.Click += btnPrint_Click;
            // 
            // btnExport
            // 
            btnExport.BackColor = Color.FromArgb(96, 125, 139);
            btnExport.FlatStyle = FlatStyle.Flat;
            btnExport.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnExport.ForeColor = Color.White;
            btnExport.Location = new Point(360, 5);
            btnExport.Name = "btnExport";
            btnExport.Size = new Size(90, 30);
            btnExport.TabIndex = 3;
            btnExport.Text = "Export";
            btnExport.UseVisualStyleBackColor = false;
            btnExport.Click += btnExport_Click;
            // 
            // btnRefresh
            // 
            btnRefresh.BackColor = Color.FromArgb(33, 150, 243);
            btnRefresh.FlatStyle = FlatStyle.Flat;
            btnRefresh.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnRefresh.ForeColor = Color.White;
            btnRefresh.Location = new Point(260, 5);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(90, 30);
            btnRefresh.TabIndex = 2;
            btnRefresh.Text = "Refresh";
            btnRefresh.UseVisualStyleBackColor = false;
            btnRefresh.Click += btnRefresh_Click;
            // 
            // btnBulkReject
            // 
            btnBulkReject.BackColor = Color.FromArgb(244, 67, 54);
            btnBulkReject.FlatStyle = FlatStyle.Flat;
            btnBulkReject.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnBulkReject.ForeColor = Color.White;
            btnBulkReject.Location = new Point(130, 5);
            btnBulkReject.Name = "btnBulkReject";
            btnBulkReject.Size = new Size(120, 30);
            btnBulkReject.TabIndex = 1;
            btnBulkReject.Text = "Bulk Reject";
            btnBulkReject.UseVisualStyleBackColor = false;
            btnBulkReject.Click += btnBulkReject_Click;
            // 
            // btnBulkApprove
            // 
            btnBulkApprove.BackColor = Color.FromArgb(76, 175, 80);
            btnBulkApprove.FlatStyle = FlatStyle.Flat;
            btnBulkApprove.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnBulkApprove.ForeColor = Color.White;
            btnBulkApprove.Location = new Point(0, 5);
            btnBulkApprove.Name = "btnBulkApprove";
            btnBulkApprove.Size = new Size(120, 30);
            btnBulkApprove.TabIndex = 0;
            btnBulkApprove.Text = "Bulk Approve";
            btnBulkApprove.UseVisualStyleBackColor = false;
            btnBulkApprove.Click += btnBulkApprove_Click;
            // 
            // lblCurrentUser
            // 
            lblCurrentUser.AutoSize = true;
            lblCurrentUser.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblCurrentUser.ForeColor = Color.Gray;
            lblCurrentUser.Location = new Point(20, 810);
            lblCurrentUser.Name = "lblCurrentUser";
            lblCurrentUser.Size = new Size(230, 15);
            lblCurrentUser.TabIndex = 6;
            lblCurrentUser.Text = "User: Mr. Santosh | Role: Material Manager";
            // 
            // panel1
            // 
            panel1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panel1.Controls.Add(dgvPRList);
            panel1.Location = new Point(20, 201);
            panel1.Name = "panel1";
            panel1.Size = new Size(1140, 228);
            panel1.TabIndex = 7;
            // 
            // frmPurchaseRequestApproval
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(245, 245, 245);
            ClientSize = new Size(1184, 841);
            Controls.Add(panel1);
            Controls.Add(lblCurrentUser);
            Controls.Add(pnlButtons);
            Controls.Add(pnlDetails);
            Controls.Add(pnlStats);
            Controls.Add(pnlFilters);
            Controls.Add(lblTitle);
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Name = "frmPurchaseRequestApproval";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Purchase Request Approval";
            Load += frmPurchaseRequestApproval_Load;
            pnlFilters.ResumeLayout(false);
            pnlFilters.PerformLayout();
            pnlStats.ResumeLayout(false);
            pnlStats.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvPRList).EndInit();
            pnlDetails.ResumeLayout(false);
            pnlDetails.PerformLayout();
            panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvLineItems).EndInit();
            pnlActions.ResumeLayout(false);
            pnlActions.PerformLayout();
            pnlButtons.ResumeLayout(false);
            panel1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Panel pnlFilters;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.ComboBox cmbStatus;
        private System.Windows.Forms.Label lblProject;
        private System.Windows.Forms.ComboBox cmbProject;
        private System.Windows.Forms.Label lblVendor;
        private System.Windows.Forms.ComboBox cmbVendor;
        private System.Windows.Forms.Label lblSearch;
        private System.Windows.Forms.TextBox txtSearchPR;
        private System.Windows.Forms.Button btnApplyFilter;
        private System.Windows.Forms.Button btnClearFilter;
        private System.Windows.Forms.Panel pnlStats;
        private System.Windows.Forms.Label lblPendingTitle;
        private System.Windows.Forms.Label lblPendingCount;
        private System.Windows.Forms.Label lblApprovedTitle;
        private System.Windows.Forms.Label lblApprovedCount;
        private System.Windows.Forms.Label lblRejectedTitle;
        private System.Windows.Forms.Label lblRejectedCount;
        private System.Windows.Forms.Label lblOnHoldTitle;
        private System.Windows.Forms.Label lblOnHoldCount;
        private System.Windows.Forms.Label lblTotalAmountTitle;
        private System.Windows.Forms.Label lblTotalAmount;
        private System.Windows.Forms.DataGridView dgvPRList;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Select;
        private System.Windows.Forms.DataGridViewTextBoxColumn PRNumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn ProjectCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn VendorName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ItemCount;
        private System.Windows.Forms.DataGridViewTextBoxColumn TotalAmount;
        private System.Windows.Forms.DataGridViewTextBoxColumn Status;
        private System.Windows.Forms.DataGridViewTextBoxColumn RequestedBy;
        private System.Windows.Forms.DataGridViewTextBoxColumn RequestDate;
        private System.Windows.Forms.Panel pnlDetails;
        private System.Windows.Forms.Label lblDetailTitle;
        private System.Windows.Forms.Label lblDetailPRNumber;
        private System.Windows.Forms.Label lblDetailStatus;
        private System.Windows.Forms.Label lblDetailProjectLabel;
        private System.Windows.Forms.Label lblDetailProject;
        private System.Windows.Forms.Label lblDetailVendorLabel;
        private System.Windows.Forms.Label lblDetailVendor;
        private System.Windows.Forms.Label lblDetailRequestorLabel;
        private System.Windows.Forms.Label lblDetailRequestor;
        private System.Windows.Forms.Label lblDetailAmountLabel;
        private System.Windows.Forms.Label lblDetailAmount;
        private System.Windows.Forms.Label lblLineItemsTitle;
        private System.Windows.Forms.DataGridView dgvLineItems;
        private System.Windows.Forms.DataGridViewTextBoxColumn ItemCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn ItemDescription;
        private System.Windows.Forms.DataGridViewTextBoxColumn Quantity;
        private System.Windows.Forms.DataGridViewTextBoxColumn UOM;
        private System.Windows.Forms.DataGridViewTextBoxColumn UnitCost;
        private System.Windows.Forms.DataGridViewTextBoxColumn TotalCost;
        private System.Windows.Forms.Panel pnlActions;
        private System.Windows.Forms.Label lblActionTitle;
        private System.Windows.Forms.Label lblApprovalAction;
        private System.Windows.Forms.ComboBox cmbApprovalAction;
        private System.Windows.Forms.Label lblApprovalRemarks;
        private System.Windows.Forms.TextBox txtApprovalRemarks;
        private System.Windows.Forms.Button btnApprove;
        private System.Windows.Forms.Button btnReject;
        private System.Windows.Forms.Button btnHold;
        private System.Windows.Forms.Button btnSubmitApproval;
        private System.Windows.Forms.Panel pnlButtons;
        private System.Windows.Forms.Button btnBulkApprove;
        private System.Windows.Forms.Button btnBulkReject;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label lblCurrentUser;
        private Panel panel1;
        private Panel panel2;
    }
}