namespace WinFormsApp1
{
    partial class frmItemCodeApproval
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            lblTitle = new Label();
            grpPendingRequests = new GroupBox();
            dgvPendingRequests = new DataGridView();
            colSelect = new DataGridViewCheckBoxColumn();
            colRequestID = new DataGridViewTextBoxColumn();
            colRequestDate = new DataGridViewTextBoxColumn();
            colRequestor = new DataGridViewTextBoxColumn();
            colDepartment = new DataGridViewTextBoxColumn();
            colItemCategory = new DataGridViewTextBoxColumn();
            colItemDescription = new DataGridViewTextBoxColumn();
            colItemCode = new DataGridViewTextBoxColumn();
            colAuthorizedCreator = new DataGridViewTextBoxColumn();
            colApprovalStatus = new DataGridViewTextBoxColumn();
            grpFilters = new GroupBox();
            btnClearFilter = new Button();
            btnFilter = new Button();
            cmbFilterStatus = new ComboBox();
            lblFilterStatus = new Label();
            cmbFilterDepartment = new ComboBox();
            lblFilterDepartment = new Label();
            cmbFilterCategory = new ComboBox();
            lblFilterCategory = new Label();
            grpSelectedRequest = new GroupBox();
            txtSelectedHSN = new TextBox();
            lblSelectedHSN = new Label();
            txtSelectedCriticality = new TextBox();
            lblSelectedCriticality = new Label();
            txtSelectedUOM = new TextBox();
            lblSelectedUOM = new Label();
            txtSelectedTechnicalSpec = new TextBox();
            lblSelectedTechnicalSpec = new Label();
            txtSelectedDepartment = new TextBox();
            lblSelectedDepartment = new Label();
            txtSelectedRequestor = new TextBox();
            lblSelectedRequestor = new Label();
            txtSelectedCategory = new TextBox();
            lblSelectedCategory = new Label();
            txtSelectedDescription = new TextBox();
            lblSelectedDescription = new Label();
            txtSelectedItemCode = new TextBox();
            lblSelectedItemCode = new Label();
            txtSelectedRequestID = new TextBox();
            lblSelectedRequestID = new Label();
            grpApprovalAction = new GroupBox();
            dtpApprovalDate = new DateTimePicker();
            lblApprovalDate = new Label();
            txtApproverName = new TextBox();
            lblApproverName = new Label();
            txtApprovalRemarks = new TextBox();
            lblApprovalRemarks = new Label();
            cmbAction = new ComboBox();
            lblAction = new Label();
            btnApprove = new Button();
            btnReject = new Button();
            btnHold = new Button();
            grpBulkActions = new GroupBox();
            btnBulkHold = new Button();
            btnBulkReject = new Button();
            btnBulkApprove = new Button();
            btnRefresh = new Button();
            btnExport = new Button();
            btnPrint = new Button();
            btnClose = new Button();
            lblTotalPending = new Label();
            txtTotalPending = new TextBox();
            lblTotalApproved = new Label();
            txtTotalApproved = new TextBox();
            lblTotalRejected = new Label();
            txtTotalRejected = new TextBox();
            lblUserRole = new Label();
            txtStoreLocation = new Label();
            txtBinNumber = new Label();
            txtUserRole = new TextBox();
            grpPendingRequests.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvPendingRequests).BeginInit();
            grpFilters.SuspendLayout();
            grpSelectedRequest.SuspendLayout();
            grpApprovalAction.SuspendLayout();
            grpBulkActions.SuspendLayout();
            SuspendLayout();
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Microsoft Sans Serif", 14F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTitle.Location = new Point(373, 10);
            lblTitle.Margin = new Padding(4, 0, 4, 0);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(301, 24);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Item Code Approval Dashboard";
            // 
            // grpPendingRequests
            // 
            grpPendingRequests.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            grpPendingRequests.Controls.Add(dgvPendingRequests);
            grpPendingRequests.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            grpPendingRequests.Location = new Point(14, 115);
            grpPendingRequests.Margin = new Padding(4, 3, 4, 3);
            grpPendingRequests.Name = "grpPendingRequests";
            grpPendingRequests.Padding = new Padding(4, 3, 4, 3);
            grpPendingRequests.Size = new Size(1050, 288);
            grpPendingRequests.TabIndex = 1;
            grpPendingRequests.TabStop = false;
            grpPendingRequests.Text = "Pending Item Code Requests";
            // 
            // dgvPendingRequests
            // 
            dgvPendingRequests.AccessibleRole = AccessibleRole.SplitButton;
            dgvPendingRequests.AllowUserToAddRows = false;
            dgvPendingRequests.AllowUserToDeleteRows = false;
            dgvPendingRequests.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvPendingRequests.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvPendingRequests.Columns.AddRange(new DataGridViewColumn[] { colSelect, colRequestID, colRequestDate, colRequestor, colDepartment, colItemCategory, colItemDescription, colItemCode, colAuthorizedCreator, colApprovalStatus });
            dgvPendingRequests.Dock = DockStyle.Fill;
            dgvPendingRequests.Location = new Point(4, 17);
            dgvPendingRequests.Margin = new Padding(4, 3, 4, 3);
            dgvPendingRequests.MultiSelect = false;
            dgvPendingRequests.Name = "dgvPendingRequests";
            dgvPendingRequests.ReadOnly = true;
            dgvPendingRequests.RowHeadersVisible = false;
            dgvPendingRequests.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPendingRequests.Size = new Size(1042, 268);
            dgvPendingRequests.TabIndex = 0;
            dgvPendingRequests.CellClick += dgvPendingRequests_CellClick;
            dgvPendingRequests.SelectionChanged += dgvPendingRequests_SelectionChanged;
            // 
            // colSelect
            // 
            colSelect.HeaderText = "Select";
            colSelect.Name = "colSelect";
            colSelect.ReadOnly = true;
            // 
            // colRequestID
            // 
            colRequestID.HeaderText = "Request ID";
            colRequestID.Name = "colRequestID";
            colRequestID.ReadOnly = true;
            // 
            // colRequestDate
            // 
            colRequestDate.HeaderText = "Request Date";
            colRequestDate.Name = "colRequestDate";
            colRequestDate.ReadOnly = true;
            // 
            // colRequestor
            // 
            colRequestor.HeaderText = "Requestor";
            colRequestor.Name = "colRequestor";
            colRequestor.ReadOnly = true;
            // 
            // colDepartment
            // 
            colDepartment.HeaderText = "Department";
            colDepartment.Name = "colDepartment";
            colDepartment.ReadOnly = true;
            // 
            // colItemCategory
            // 
            colItemCategory.HeaderText = "Item Category";
            colItemCategory.Name = "colItemCategory";
            colItemCategory.ReadOnly = true;
            // 
            // colItemDescription
            // 
            colItemDescription.HeaderText = "Description";
            colItemDescription.Name = "colItemDescription";
            colItemDescription.ReadOnly = true;
            // 
            // colItemCode
            // 
            colItemCode.HeaderText = "Item Code";
            colItemCode.Name = "colItemCode";
            colItemCode.ReadOnly = true;
            // 
            // colAuthorizedCreator
            // 
            colAuthorizedCreator.HeaderText = "Creator";
            colAuthorizedCreator.Name = "colAuthorizedCreator";
            colAuthorizedCreator.ReadOnly = true;
            // 
            // colApprovalStatus
            // 
            colApprovalStatus.HeaderText = "Status";
            colApprovalStatus.Name = "colApprovalStatus";
            colApprovalStatus.ReadOnly = true;
            // 
            // grpFilters
            // 
            grpFilters.Controls.Add(btnClearFilter);
            grpFilters.Controls.Add(btnFilter);
            grpFilters.Controls.Add(cmbFilterStatus);
            grpFilters.Controls.Add(lblFilterStatus);
            grpFilters.Controls.Add(cmbFilterDepartment);
            grpFilters.Controls.Add(lblFilterDepartment);
            grpFilters.Controls.Add(cmbFilterCategory);
            grpFilters.Controls.Add(lblFilterCategory);
            grpFilters.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            grpFilters.Location = new Point(14, 52);
            grpFilters.Margin = new Padding(4, 3, 4, 3);
            grpFilters.Name = "grpFilters";
            grpFilters.Padding = new Padding(4, 3, 4, 3);
            grpFilters.Size = new Size(1050, 63);
            grpFilters.TabIndex = 2;
            grpFilters.TabStop = false;
            grpFilters.Text = "Filters";
            // 
            // btnClearFilter
            // 
            btnClearFilter.BackColor = Color.LightYellow;
            btnClearFilter.Location = new Point(875, 23);
            btnClearFilter.Margin = new Padding(4, 3, 4, 3);
            btnClearFilter.Name = "btnClearFilter";
            btnClearFilter.Size = new Size(93, 29);
            btnClearFilter.TabIndex = 7;
            btnClearFilter.Text = "Clear";
            btnClearFilter.UseVisualStyleBackColor = false;
            btnClearFilter.Click += btnClearFilter_Click;
            // 
            // btnFilter
            // 
            btnFilter.BackColor = Color.LightBlue;
            btnFilter.Location = new Point(770, 23);
            btnFilter.Margin = new Padding(4, 3, 4, 3);
            btnFilter.Name = "btnFilter";
            btnFilter.Size = new Size(93, 29);
            btnFilter.TabIndex = 6;
            btnFilter.Text = "Apply Filter";
            btnFilter.UseVisualStyleBackColor = false;
            btnFilter.Click += btnFilter_Click;
            // 
            // cmbFilterStatus
            // 
            cmbFilterStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbFilterStatus.FormattingEnabled = true;
            cmbFilterStatus.Location = new Point(607, 25);
            cmbFilterStatus.Margin = new Padding(4, 3, 4, 3);
            cmbFilterStatus.Name = "cmbFilterStatus";
            cmbFilterStatus.Size = new Size(139, 23);
            cmbFilterStatus.TabIndex = 5;
            // 
            // lblFilterStatus
            // 
            lblFilterStatus.AutoSize = true;
            lblFilterStatus.Location = new Point(548, 29);
            lblFilterStatus.Margin = new Padding(4, 0, 4, 0);
            lblFilterStatus.Name = "lblFilterStatus";
            lblFilterStatus.Size = new Size(44, 15);
            lblFilterStatus.TabIndex = 4;
            lblFilterStatus.Text = "Status:";
            // 
            // cmbFilterDepartment
            // 
            cmbFilterDepartment.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbFilterDepartment.FormattingEnabled = true;
            cmbFilterDepartment.Location = new Point(362, 25);
            cmbFilterDepartment.Margin = new Padding(4, 3, 4, 3);
            cmbFilterDepartment.Name = "cmbFilterDepartment";
            cmbFilterDepartment.Size = new Size(174, 23);
            cmbFilterDepartment.TabIndex = 3;
            // 
            // lblFilterDepartment
            // 
            lblFilterDepartment.AutoSize = true;
            lblFilterDepartment.Location = new Point(268, 29);
            lblFilterDepartment.Margin = new Padding(4, 0, 4, 0);
            lblFilterDepartment.Name = "lblFilterDepartment";
            lblFilterDepartment.Size = new Size(75, 15);
            lblFilterDepartment.TabIndex = 2;
            lblFilterDepartment.Text = "Department:";
            // 
            // cmbFilterCategory
            // 
            cmbFilterCategory.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbFilterCategory.FormattingEnabled = true;
            cmbFilterCategory.Location = new Point(82, 25);
            cmbFilterCategory.Margin = new Padding(4, 3, 4, 3);
            cmbFilterCategory.Name = "cmbFilterCategory";
            cmbFilterCategory.Size = new Size(174, 23);
            cmbFilterCategory.TabIndex = 1;
            // 
            // lblFilterCategory
            // 
            lblFilterCategory.AutoSize = true;
            lblFilterCategory.Location = new Point(12, 29);
            lblFilterCategory.Margin = new Padding(4, 0, 4, 0);
            lblFilterCategory.Name = "lblFilterCategory";
            lblFilterCategory.Size = new Size(58, 15);
            lblFilterCategory.TabIndex = 0;
            lblFilterCategory.Text = "Category:";
            // 
            // grpSelectedRequest
            // 
            grpSelectedRequest.Controls.Add(txtSelectedHSN);
            grpSelectedRequest.Controls.Add(lblSelectedHSN);
            grpSelectedRequest.Controls.Add(txtSelectedCriticality);
            grpSelectedRequest.Controls.Add(lblSelectedCriticality);
            grpSelectedRequest.Controls.Add(txtSelectedUOM);
            grpSelectedRequest.Controls.Add(lblSelectedUOM);
            grpSelectedRequest.Controls.Add(txtSelectedTechnicalSpec);
            grpSelectedRequest.Controls.Add(lblSelectedTechnicalSpec);
            grpSelectedRequest.Controls.Add(txtSelectedDepartment);
            grpSelectedRequest.Controls.Add(lblSelectedDepartment);
            grpSelectedRequest.Controls.Add(txtSelectedRequestor);
            grpSelectedRequest.Controls.Add(lblSelectedRequestor);
            grpSelectedRequest.Controls.Add(txtSelectedCategory);
            grpSelectedRequest.Controls.Add(lblSelectedCategory);
            grpSelectedRequest.Controls.Add(txtSelectedDescription);
            grpSelectedRequest.Controls.Add(lblSelectedDescription);
            grpSelectedRequest.Controls.Add(txtSelectedItemCode);
            grpSelectedRequest.Controls.Add(lblSelectedItemCode);
            grpSelectedRequest.Controls.Add(txtSelectedRequestID);
            grpSelectedRequest.Controls.Add(lblSelectedRequestID);
            grpSelectedRequest.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            grpSelectedRequest.Location = new Point(14, 415);
            grpSelectedRequest.Margin = new Padding(4, 3, 4, 3);
            grpSelectedRequest.Name = "grpSelectedRequest";
            grpSelectedRequest.Padding = new Padding(4, 3, 4, 3);
            grpSelectedRequest.Size = new Size(560, 323);
            grpSelectedRequest.TabIndex = 3;
            grpSelectedRequest.TabStop = false;
            grpSelectedRequest.Text = "Selected Request Details";
            // 
            // txtSelectedHSN
            // 
            txtSelectedHSN.Location = new Point(140, 285);
            txtSelectedHSN.Margin = new Padding(4, 3, 4, 3);
            txtSelectedHSN.Name = "txtSelectedHSN";
            txtSelectedHSN.ReadOnly = true;
            txtSelectedHSN.Size = new Size(139, 21);
            txtSelectedHSN.TabIndex = 19;
            // 
            // lblSelectedHSN
            // 
            lblSelectedHSN.AutoSize = true;
            lblSelectedHSN.Location = new Point(18, 288);
            lblSelectedHSN.Margin = new Padding(4, 0, 4, 0);
            lblSelectedHSN.Name = "lblSelectedHSN";
            lblSelectedHSN.Size = new Size(68, 15);
            lblSelectedHSN.TabIndex = 18;
            lblSelectedHSN.Text = "HSN Code:";
            // 
            // txtSelectedCriticality
            // 
            txtSelectedCriticality.Location = new Point(362, 250);
            txtSelectedCriticality.Margin = new Padding(4, 3, 4, 3);
            txtSelectedCriticality.Name = "txtSelectedCriticality";
            txtSelectedCriticality.ReadOnly = true;
            txtSelectedCriticality.Size = new Size(174, 21);
            txtSelectedCriticality.TabIndex = 17;
            // 
            // lblSelectedCriticality
            // 
            lblSelectedCriticality.AutoSize = true;
            lblSelectedCriticality.Location = new Point(280, 254);
            lblSelectedCriticality.Margin = new Padding(4, 0, 4, 0);
            lblSelectedCriticality.Name = "lblSelectedCriticality";
            lblSelectedCriticality.Size = new Size(58, 15);
            lblSelectedCriticality.TabIndex = 16;
            lblSelectedCriticality.Text = "Criticality:";
            // 
            // txtSelectedUOM
            // 
            txtSelectedUOM.Location = new Point(140, 250);
            txtSelectedUOM.Margin = new Padding(4, 3, 4, 3);
            txtSelectedUOM.Name = "txtSelectedUOM";
            txtSelectedUOM.ReadOnly = true;
            txtSelectedUOM.Size = new Size(116, 21);
            txtSelectedUOM.TabIndex = 15;
            // 
            // lblSelectedUOM
            // 
            lblSelectedUOM.AutoSize = true;
            lblSelectedUOM.Location = new Point(18, 254);
            lblSelectedUOM.Margin = new Padding(4, 0, 4, 0);
            lblSelectedUOM.Name = "lblSelectedUOM";
            lblSelectedUOM.Size = new Size(39, 15);
            lblSelectedUOM.TabIndex = 14;
            lblSelectedUOM.Text = "UOM:";
            // 
            // txtSelectedTechnicalSpec
            // 
            txtSelectedTechnicalSpec.Location = new Point(140, 198);
            txtSelectedTechnicalSpec.Margin = new Padding(4, 3, 4, 3);
            txtSelectedTechnicalSpec.Multiline = true;
            txtSelectedTechnicalSpec.Name = "txtSelectedTechnicalSpec";
            txtSelectedTechnicalSpec.ReadOnly = true;
            txtSelectedTechnicalSpec.Size = new Size(396, 46);
            txtSelectedTechnicalSpec.TabIndex = 13;
            // 
            // lblSelectedTechnicalSpec
            // 
            lblSelectedTechnicalSpec.AutoSize = true;
            lblSelectedTechnicalSpec.Location = new Point(18, 202);
            lblSelectedTechnicalSpec.Margin = new Padding(4, 0, 4, 0);
            lblSelectedTechnicalSpec.Name = "lblSelectedTechnicalSpec";
            lblSelectedTechnicalSpec.Size = new Size(94, 15);
            lblSelectedTechnicalSpec.TabIndex = 12;
            lblSelectedTechnicalSpec.Text = "Technical Spec:";
            // 
            // txtSelectedDepartment
            // 
            txtSelectedDepartment.Location = new Point(140, 164);
            txtSelectedDepartment.Margin = new Padding(4, 3, 4, 3);
            txtSelectedDepartment.Name = "txtSelectedDepartment";
            txtSelectedDepartment.ReadOnly = true;
            txtSelectedDepartment.Size = new Size(174, 21);
            txtSelectedDepartment.TabIndex = 11;
            // 
            // lblSelectedDepartment
            // 
            lblSelectedDepartment.AutoSize = true;
            lblSelectedDepartment.Location = new Point(18, 167);
            lblSelectedDepartment.Margin = new Padding(4, 0, 4, 0);
            lblSelectedDepartment.Name = "lblSelectedDepartment";
            lblSelectedDepartment.Size = new Size(75, 15);
            lblSelectedDepartment.TabIndex = 10;
            lblSelectedDepartment.Text = "Department:";
            // 
            // txtSelectedRequestor
            // 
            txtSelectedRequestor.Location = new Point(140, 129);
            txtSelectedRequestor.Margin = new Padding(4, 3, 4, 3);
            txtSelectedRequestor.Name = "txtSelectedRequestor";
            txtSelectedRequestor.ReadOnly = true;
            txtSelectedRequestor.Size = new Size(174, 21);
            txtSelectedRequestor.TabIndex = 9;
            // 
            // lblSelectedRequestor
            // 
            lblSelectedRequestor.AutoSize = true;
            lblSelectedRequestor.Location = new Point(18, 133);
            lblSelectedRequestor.Margin = new Padding(4, 0, 4, 0);
            lblSelectedRequestor.Name = "lblSelectedRequestor";
            lblSelectedRequestor.Size = new Size(67, 15);
            lblSelectedRequestor.TabIndex = 8;
            lblSelectedRequestor.Text = "Requestor:";
            // 
            // txtSelectedCategory
            // 
            txtSelectedCategory.Location = new Point(140, 95);
            txtSelectedCategory.Margin = new Padding(4, 3, 4, 3);
            txtSelectedCategory.Name = "txtSelectedCategory";
            txtSelectedCategory.ReadOnly = true;
            txtSelectedCategory.Size = new Size(174, 21);
            txtSelectedCategory.TabIndex = 7;
            // 
            // lblSelectedCategory
            // 
            lblSelectedCategory.AutoSize = true;
            lblSelectedCategory.Location = new Point(18, 98);
            lblSelectedCategory.Margin = new Padding(4, 0, 4, 0);
            lblSelectedCategory.Name = "lblSelectedCategory";
            lblSelectedCategory.Size = new Size(58, 15);
            lblSelectedCategory.TabIndex = 6;
            lblSelectedCategory.Text = "Category:";
            // 
            // txtSelectedDescription
            // 
            txtSelectedDescription.Location = new Point(140, 60);
            txtSelectedDescription.Margin = new Padding(4, 3, 4, 3);
            txtSelectedDescription.Name = "txtSelectedDescription";
            txtSelectedDescription.ReadOnly = true;
            txtSelectedDescription.Size = new Size(396, 21);
            txtSelectedDescription.TabIndex = 5;
            // 
            // lblSelectedDescription
            // 
            lblSelectedDescription.AutoSize = true;
            lblSelectedDescription.Location = new Point(18, 63);
            lblSelectedDescription.Margin = new Padding(4, 0, 4, 0);
            lblSelectedDescription.Name = "lblSelectedDescription";
            lblSelectedDescription.Size = new Size(72, 15);
            lblSelectedDescription.TabIndex = 4;
            lblSelectedDescription.Text = "Description:";
            // 
            // txtSelectedItemCode
            // 
            txtSelectedItemCode.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            txtSelectedItemCode.Location = new Point(385, 25);
            txtSelectedItemCode.Margin = new Padding(4, 3, 4, 3);
            txtSelectedItemCode.Name = "txtSelectedItemCode";
            txtSelectedItemCode.ReadOnly = true;
            txtSelectedItemCode.Size = new Size(151, 21);
            txtSelectedItemCode.TabIndex = 3;
            // 
            // lblSelectedItemCode
            // 
            lblSelectedItemCode.AutoSize = true;
            lblSelectedItemCode.Location = new Point(303, 29);
            lblSelectedItemCode.Margin = new Padding(4, 0, 4, 0);
            lblSelectedItemCode.Name = "lblSelectedItemCode";
            lblSelectedItemCode.Size = new Size(66, 15);
            lblSelectedItemCode.TabIndex = 2;
            lblSelectedItemCode.Text = "Item Code:";
            // 
            // txtSelectedRequestID
            // 
            txtSelectedRequestID.Location = new Point(140, 25);
            txtSelectedRequestID.Margin = new Padding(4, 3, 4, 3);
            txtSelectedRequestID.Name = "txtSelectedRequestID";
            txtSelectedRequestID.ReadOnly = true;
            txtSelectedRequestID.Size = new Size(139, 21);
            txtSelectedRequestID.TabIndex = 1;
            // 
            // lblSelectedRequestID
            // 
            lblSelectedRequestID.AutoSize = true;
            lblSelectedRequestID.Location = new Point(18, 29);
            lblSelectedRequestID.Margin = new Padding(4, 0, 4, 0);
            lblSelectedRequestID.Name = "lblSelectedRequestID";
            lblSelectedRequestID.Size = new Size(71, 15);
            lblSelectedRequestID.TabIndex = 0;
            lblSelectedRequestID.Text = "Request ID:";
            // 
            // grpApprovalAction
            // 
            grpApprovalAction.Controls.Add(dtpApprovalDate);
            grpApprovalAction.Controls.Add(lblApprovalDate);
            grpApprovalAction.Controls.Add(txtApproverName);
            grpApprovalAction.Controls.Add(lblApproverName);
            grpApprovalAction.Controls.Add(txtApprovalRemarks);
            grpApprovalAction.Controls.Add(lblApprovalRemarks);
            grpApprovalAction.Controls.Add(cmbAction);
            grpApprovalAction.Controls.Add(lblAction);
            grpApprovalAction.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            grpApprovalAction.Location = new Point(595, 415);
            grpApprovalAction.Margin = new Padding(4, 3, 4, 3);
            grpApprovalAction.Name = "grpApprovalAction";
            grpApprovalAction.Padding = new Padding(4, 3, 4, 3);
            grpApprovalAction.Size = new Size(467, 208);
            grpApprovalAction.TabIndex = 4;
            grpApprovalAction.TabStop = false;
            grpApprovalAction.Text = "Approval Action";
            // 
            // dtpApprovalDate
            // 
            dtpApprovalDate.Format = DateTimePickerFormat.Short;
            dtpApprovalDate.Location = new Point(140, 158);
            dtpApprovalDate.Margin = new Padding(4, 3, 4, 3);
            dtpApprovalDate.Name = "dtpApprovalDate";
            dtpApprovalDate.Size = new Size(139, 21);
            dtpApprovalDate.TabIndex = 7;
            // 
            // lblApprovalDate
            // 
            lblApprovalDate.AutoSize = true;
            lblApprovalDate.Location = new Point(18, 162);
            lblApprovalDate.Margin = new Padding(4, 0, 4, 0);
            lblApprovalDate.Name = "lblApprovalDate";
            lblApprovalDate.Size = new Size(36, 15);
            lblApprovalDate.TabIndex = 6;
            lblApprovalDate.Text = "Date:";
            // 
            // txtApproverName
            // 
            txtApproverName.Location = new Point(140, 123);
            txtApproverName.Margin = new Padding(4, 3, 4, 3);
            txtApproverName.Name = "txtApproverName";
            txtApproverName.Size = new Size(233, 21);
            txtApproverName.TabIndex = 5;
            // 
            // lblApproverName
            // 
            lblApproverName.AutoSize = true;
            lblApproverName.Location = new Point(18, 127);
            lblApproverName.Margin = new Padding(4, 0, 4, 0);
            lblApproverName.Name = "lblApproverName";
            lblApproverName.Size = new Size(95, 15);
            lblApproverName.TabIndex = 4;
            lblApproverName.Text = "Approver Name:";
            // 
            // txtApprovalRemarks
            // 
            txtApprovalRemarks.Location = new Point(140, 60);
            txtApprovalRemarks.Margin = new Padding(4, 3, 4, 3);
            txtApprovalRemarks.Multiline = true;
            txtApprovalRemarks.Name = "txtApprovalRemarks";
            txtApprovalRemarks.Size = new Size(303, 57);
            txtApprovalRemarks.TabIndex = 3;
            // 
            // lblApprovalRemarks
            // 
            lblApprovalRemarks.AutoSize = true;
            lblApprovalRemarks.Location = new Point(18, 63);
            lblApprovalRemarks.Margin = new Padding(4, 0, 4, 0);
            lblApprovalRemarks.Name = "lblApprovalRemarks";
            lblApprovalRemarks.Size = new Size(60, 15);
            lblApprovalRemarks.TabIndex = 2;
            lblApprovalRemarks.Text = "Remarks:";
            // 
            // cmbAction
            // 
            cmbAction.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbAction.FormattingEnabled = true;
            cmbAction.Location = new Point(140, 25);
            cmbAction.Margin = new Padding(4, 3, 4, 3);
            cmbAction.Name = "cmbAction";
            cmbAction.Size = new Size(174, 23);
            cmbAction.TabIndex = 1;
            // 
            // lblAction
            // 
            lblAction.AutoSize = true;
            lblAction.Location = new Point(18, 29);
            lblAction.Margin = new Padding(4, 0, 4, 0);
            lblAction.Name = "lblAction";
            lblAction.Size = new Size(43, 15);
            lblAction.TabIndex = 0;
            lblAction.Text = "Action:";
            // 
            // btnApprove
            // 
            btnApprove.BackColor = Color.LightGreen;
            btnApprove.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnApprove.Location = new Point(595, 635);
            btnApprove.Margin = new Padding(4, 3, 4, 3);
            btnApprove.Name = "btnApprove";
            btnApprove.Size = new Size(140, 40);
            btnApprove.TabIndex = 5;
            btnApprove.Text = "Approve";
            btnApprove.UseVisualStyleBackColor = false;
            btnApprove.Click += btnApprove_Click;
            // 
            // btnReject
            // 
            btnReject.BackColor = Color.LightCoral;
            btnReject.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnReject.Location = new Point(758, 635);
            btnReject.Margin = new Padding(4, 3, 4, 3);
            btnReject.Name = "btnReject";
            btnReject.Size = new Size(140, 40);
            btnReject.TabIndex = 6;
            btnReject.Text = "Reject";
            btnReject.UseVisualStyleBackColor = false;
            btnReject.Click += btnReject_Click;
            // 
            // btnHold
            // 
            btnHold.BackColor = Color.LightYellow;
            btnHold.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnHold.Location = new Point(922, 635);
            btnHold.Margin = new Padding(4, 3, 4, 3);
            btnHold.Name = "btnHold";
            btnHold.Size = new Size(140, 40);
            btnHold.TabIndex = 7;
            btnHold.Text = "On Hold";
            btnHold.UseVisualStyleBackColor = false;
            btnHold.Click += btnHold_Click;
            // 
            // grpBulkActions
            // 
            grpBulkActions.Controls.Add(btnBulkHold);
            grpBulkActions.Controls.Add(btnBulkReject);
            grpBulkActions.Controls.Add(btnBulkApprove);
            grpBulkActions.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            grpBulkActions.Location = new Point(14, 750);
            grpBulkActions.Margin = new Padding(4, 3, 4, 3);
            grpBulkActions.Name = "grpBulkActions";
            grpBulkActions.Padding = new Padding(4, 3, 4, 3);
            grpBulkActions.Size = new Size(560, 69);
            grpBulkActions.TabIndex = 8;
            grpBulkActions.TabStop = false;
            grpBulkActions.Text = "Bulk Actions (Multiple Selection)";
            // 
            // btnBulkHold
            // 
            btnBulkHold.BackColor = Color.LightYellow;
            btnBulkHold.Location = new Point(373, 25);
            btnBulkHold.Margin = new Padding(4, 3, 4, 3);
            btnBulkHold.Name = "btnBulkHold";
            btnBulkHold.Size = new Size(152, 32);
            btnBulkHold.TabIndex = 2;
            btnBulkHold.Text = "Bulk On Hold";
            btnBulkHold.UseVisualStyleBackColor = false;
            btnBulkHold.Click += btnBulkHold_Click;
            // 
            // btnBulkReject
            // 
            btnBulkReject.BackColor = Color.LightCoral;
            btnBulkReject.Location = new Point(198, 25);
            btnBulkReject.Margin = new Padding(4, 3, 4, 3);
            btnBulkReject.Name = "btnBulkReject";
            btnBulkReject.Size = new Size(152, 32);
            btnBulkReject.TabIndex = 1;
            btnBulkReject.Text = "Bulk Reject";
            btnBulkReject.UseVisualStyleBackColor = false;
            btnBulkReject.Click += btnBulkReject_Click;
            // 
            // btnBulkApprove
            // 
            btnBulkApprove.BackColor = Color.LightGreen;
            btnBulkApprove.Location = new Point(23, 25);
            btnBulkApprove.Margin = new Padding(4, 3, 4, 3);
            btnBulkApprove.Name = "btnBulkApprove";
            btnBulkApprove.Size = new Size(152, 32);
            btnBulkApprove.TabIndex = 0;
            btnBulkApprove.Text = "Bulk Approve";
            btnBulkApprove.UseVisualStyleBackColor = false;
            btnBulkApprove.Click += btnBulkApprove_Click;
            // 
            // btnRefresh
            // 
            btnRefresh.BackColor = Color.LightSteelBlue;
            btnRefresh.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnRefresh.Location = new Point(595, 750);
            btnRefresh.Margin = new Padding(4, 3, 4, 3);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(105, 32);
            btnRefresh.TabIndex = 9;
            btnRefresh.Text = "Refresh";
            btnRefresh.UseVisualStyleBackColor = false;
            btnRefresh.Click += btnRefresh_Click;
            // 
            // btnExport
            // 
            btnExport.BackColor = Color.LightSteelBlue;
            btnExport.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnExport.Location = new Point(712, 750);
            btnExport.Margin = new Padding(4, 3, 4, 3);
            btnExport.Name = "btnExport";
            btnExport.Size = new Size(105, 32);
            btnExport.TabIndex = 10;
            btnExport.Text = "Export";
            btnExport.UseVisualStyleBackColor = false;
            btnExport.Click += btnExport_Click;
            // 
            // btnPrint
            // 
            btnPrint.BackColor = Color.LightSteelBlue;
            btnPrint.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnPrint.Location = new Point(828, 750);
            btnPrint.Margin = new Padding(4, 3, 4, 3);
            btnPrint.Name = "btnPrint";
            btnPrint.Size = new Size(105, 32);
            btnPrint.TabIndex = 11;
            btnPrint.Text = "Print";
            btnPrint.UseVisualStyleBackColor = false;
            btnPrint.Click += btnPrint_Click;
            // 
            // btnClose
            // 
            btnClose.BackColor = Color.LightCoral;
            btnClose.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnClose.Location = new Point(957, 750);
            btnClose.Margin = new Padding(4, 3, 4, 3);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(105, 32);
            btnClose.TabIndex = 12;
            btnClose.Text = "Close";
            btnClose.UseVisualStyleBackColor = false;
            btnClose.Click += btnClose_Click;
            // 
            // lblTotalPending
            // 
            lblTotalPending.AutoSize = true;
            lblTotalPending.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTotalPending.Location = new Point(18, 831);
            lblTotalPending.Margin = new Padding(4, 0, 4, 0);
            lblTotalPending.Name = "lblTotalPending";
            lblTotalPending.Size = new Size(100, 15);
            lblTotalPending.TabIndex = 13;
            lblTotalPending.Text = "Total Pending:";
            // 
            // txtTotalPending
            // 
            txtTotalPending.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            txtTotalPending.Location = new Point(128, 827);
            txtTotalPending.Margin = new Padding(4, 3, 4, 3);
            txtTotalPending.Name = "txtTotalPending";
            txtTotalPending.ReadOnly = true;
            txtTotalPending.Size = new Size(58, 21);
            txtTotalPending.TabIndex = 14;
            txtTotalPending.Text = "0";
            // 
            // lblTotalApproved
            // 
            lblTotalApproved.AutoSize = true;
            lblTotalApproved.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTotalApproved.Location = new Point(210, 831);
            lblTotalApproved.Margin = new Padding(4, 0, 4, 0);
            lblTotalApproved.Name = "lblTotalApproved";
            lblTotalApproved.Size = new Size(106, 15);
            lblTotalApproved.TabIndex = 15;
            lblTotalApproved.Text = "Total Approved:";
            // 
            // txtTotalApproved
            // 
            txtTotalApproved.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            txtTotalApproved.Location = new Point(338, 827);
            txtTotalApproved.Margin = new Padding(4, 3, 4, 3);
            txtTotalApproved.Name = "txtTotalApproved";
            txtTotalApproved.ReadOnly = true;
            txtTotalApproved.Size = new Size(58, 21);
            txtTotalApproved.TabIndex = 16;
            txtTotalApproved.Text = "0";
            // 
            // lblTotalRejected
            // 
            lblTotalRejected.AutoSize = true;
            lblTotalRejected.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTotalRejected.Location = new Point(420, 831);
            lblTotalRejected.Margin = new Padding(4, 0, 4, 0);
            lblTotalRejected.Name = "lblTotalRejected";
            lblTotalRejected.Size = new Size(104, 15);
            lblTotalRejected.TabIndex = 17;
            lblTotalRejected.Text = "Total Rejected:";
            // 
            // txtTotalRejected
            // 
            txtTotalRejected.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            txtTotalRejected.Location = new Point(542, 827);
            txtTotalRejected.Margin = new Padding(4, 3, 4, 3);
            txtTotalRejected.Name = "txtTotalRejected";
            txtTotalRejected.ReadOnly = true;
            txtTotalRejected.Size = new Size(58, 21);
            txtTotalRejected.TabIndex = 18;
            txtTotalRejected.Text = "0";
            // 
            // lblUserRole
            // 
            lblUserRole.AutoSize = true;
            lblUserRole.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblUserRole.Location = new Point(642, 831);
            lblUserRole.Margin = new Padding(4, 0, 4, 0);
            lblUserRole.Name = "lblUserRole";
            lblUserRole.Size = new Size(75, 15);
            lblUserRole.TabIndex = 19;
            lblUserRole.Text = "User Role:";
            // 
            // txtStoreLocation
            // 
            txtStoreLocation.Location = new Point(0, 0);
            txtStoreLocation.Name = "txtStoreLocation";
            txtStoreLocation.Size = new Size(100, 23);
            txtStoreLocation.TabIndex = 0;
            // 
            // txtBinNumber
            // 
            txtBinNumber.AutoSize = true;
            txtBinNumber.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            txtBinNumber.Location = new Point(550, 720);
            txtBinNumber.Name = "txtBinNumber";
            txtBinNumber.Size = new Size(69, 15);
            txtBinNumber.TabIndex = 19;
            txtBinNumber.Text = "txtBinNumber";
            // 
            // txtUserRole
            // 
            txtUserRole.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            txtUserRole.Location = new Point(729, 827);
            txtUserRole.Margin = new Padding(4, 3, 4, 3);
            txtUserRole.Name = "txtUserRole";
            txtUserRole.ReadOnly = true;
            txtUserRole.Size = new Size(233, 21);
            txtUserRole.TabIndex = 20;
            txtUserRole.Text = "txtUserRole:";
            // 
            // frmItemCodeApproval
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.WhiteSmoke;
            ClientSize = new Size(1078, 867);
            Controls.Add(txtUserRole);
            Controls.Add(lblUserRole);
            Controls.Add(txtTotalRejected);
            Controls.Add(lblTotalRejected);
            Controls.Add(txtTotalApproved);
            Controls.Add(lblTotalApproved);
            Controls.Add(txtTotalPending);
            Controls.Add(lblTotalPending);
            Controls.Add(btnClose);
            Controls.Add(btnPrint);
            Controls.Add(btnExport);
            Controls.Add(btnRefresh);
            Controls.Add(grpBulkActions);
            Controls.Add(btnHold);
            Controls.Add(btnReject);
            Controls.Add(btnApprove);
            Controls.Add(grpApprovalAction);
            Controls.Add(grpSelectedRequest);
            Controls.Add(grpFilters);
            Controls.Add(grpPendingRequests);
            Controls.Add(lblTitle);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Margin = new Padding(4, 3, 4, 3);
            MaximizeBox = false;
            Name = "frmItemCodeApproval";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Item Code Approval Dashboard";
            Load += frmItemCodeApproval_Load;
            grpPendingRequests.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvPendingRequests).EndInit();
            grpFilters.ResumeLayout(false);
            grpFilters.PerformLayout();
            grpSelectedRequest.ResumeLayout(false);
            grpSelectedRequest.PerformLayout();
            grpApprovalAction.ResumeLayout(false);
            grpApprovalAction.PerformLayout();
            grpBulkActions.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.GroupBox grpPendingRequests;
        private System.Windows.Forms.DataGridView dgvPendingRequests;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colSelect;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRequestID;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRequestDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRequestor;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDepartment;
        private System.Windows.Forms.DataGridViewTextBoxColumn colItemCategory;
        private System.Windows.Forms.DataGridViewTextBoxColumn colItemDescription;
        private System.Windows.Forms.DataGridViewTextBoxColumn colItemCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAuthorizedCreator;
        private System.Windows.Forms.DataGridViewTextBoxColumn colApprovalStatus;
        private System.Windows.Forms.GroupBox grpFilters;
        private System.Windows.Forms.Label lblFilterCategory;
        private System.Windows.Forms.ComboBox cmbFilterCategory;
        private System.Windows.Forms.Label lblFilterDepartment;
        private System.Windows.Forms.ComboBox cmbFilterDepartment;
        private System.Windows.Forms.Label lblFilterStatus;
        private System.Windows.Forms.ComboBox cmbFilterStatus;
        private System.Windows.Forms.Button btnFilter;
        private System.Windows.Forms.Button btnClearFilter;
        private System.Windows.Forms.GroupBox grpSelectedRequest;
        private System.Windows.Forms.Label lblSelectedRequestID;
        private System.Windows.Forms.TextBox txtSelectedRequestID;
        private System.Windows.Forms.Label lblSelectedItemCode;
        private System.Windows.Forms.TextBox txtSelectedItemCode;
        private System.Windows.Forms.Label lblSelectedDescription;
        private System.Windows.Forms.TextBox txtSelectedDescription;
        private System.Windows.Forms.Label lblSelectedCategory;
        private System.Windows.Forms.TextBox txtSelectedCategory;
        private System.Windows.Forms.Label lblSelectedRequestor;
        private System.Windows.Forms.TextBox txtSelectedRequestor;
        private System.Windows.Forms.Label lblSelectedDepartment;
        private System.Windows.Forms.TextBox txtSelectedDepartment;
        private System.Windows.Forms.Label lblSelectedTechnicalSpec;
        private System.Windows.Forms.TextBox txtSelectedTechnicalSpec;
        private System.Windows.Forms.Label lblSelectedUOM;
        private System.Windows.Forms.TextBox txtSelectedUOM;
        private System.Windows.Forms.Label lblSelectedCriticality;
        private System.Windows.Forms.TextBox txtSelectedCriticality;
        private System.Windows.Forms.Label lblSelectedHSN;
        private System.Windows.Forms.TextBox txtSelectedHSN;
        private System.Windows.Forms.GroupBox grpApprovalAction;
        private System.Windows.Forms.Label lblAction;
        private System.Windows.Forms.ComboBox cmbAction;
        private System.Windows.Forms.Label lblApprovalRemarks;
        private System.Windows.Forms.TextBox txtApprovalRemarks;
        private System.Windows.Forms.Label lblApproverName;
        private System.Windows.Forms.TextBox txtApproverName;
        private System.Windows.Forms.Label lblApprovalDate;
        private System.Windows.Forms.DateTimePicker dtpApprovalDate;
        private System.Windows.Forms.Button btnApprove;
        private System.Windows.Forms.Button btnReject;
        private System.Windows.Forms.Button btnHold;
        private System.Windows.Forms.GroupBox grpBulkActions;
        private System.Windows.Forms.Button btnBulkApprove;
        private System.Windows.Forms.Button btnBulkReject;
        private System.Windows.Forms.Button btnBulkHold;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Label lblTotalPending;
        private System.Windows.Forms.TextBox txtTotalPending;
        private System.Windows.Forms.Label lblTotalApproved;
        private System.Windows.Forms.TextBox txtTotalApproved;
        private System.Windows.Forms.Label lblTotalRejected;
        private System.Windows.Forms.TextBox txtTotalRejected;
        private System.Windows.Forms.Label lblUserRole;
        private System.Windows.Forms.Label txtStoreLocation;
        private System.Windows.Forms.Label txtBinNumber;
        private System.Windows.Forms.TextBox txtUserRole;
    }
}