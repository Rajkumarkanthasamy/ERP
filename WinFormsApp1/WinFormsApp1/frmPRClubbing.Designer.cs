//namespace WinFormsApp1
//{
//    partial class frmPRClubbing
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
//            this.Text = "frmPRClubbing";
//        }

//        #endregion
//    }
//}

namespace WinFormsApp1
{
    partial class frmPRClubbing
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
            grpAvailablePRs = new GroupBox();
            dgvAvailablePRs = new DataGridView();
            colPRSelect = new DataGridViewCheckBoxColumn();
            colPRNumber = new DataGridViewTextBoxColumn();
            colPRProjectCode = new DataGridViewTextBoxColumn();
            colPRVendor = new DataGridViewTextBoxColumn();
            colPRAmount = new DataGridViewTextBoxColumn();
            ApprovedBy = new DataGridViewTextBoxColumn();
            colPRCurrency = new DataGridViewTextBoxColumn();
            colPRProductNo = new DataGridViewTextBoxColumn();
            grpFilter = new GroupBox();
            btnClearFilter = new Button();
            btnFilter = new Button();
            cmbFilterVendor = new ComboBox();
            lblFilterVendor = new Label();
            grpClubbedPreview = new GroupBox();
            dgvClubbedPR = new DataGridView();
            colClubItemCode = new DataGridViewTextBoxColumn();
            colClubItemName = new DataGridViewTextBoxColumn();
            colClubQty = new DataGridViewTextBoxColumn();
            colClubUnitCost = new DataGridViewTextBoxColumn();
            colClubTotal = new DataGridViewTextBoxColumn();
            colClubVendor = new DataGridViewTextBoxColumn();
            colClubSourcePR = new DataGridViewTextBoxColumn();
            grpSummary = new GroupBox();
            txtNewPRNumber = new TextBox();
            lblNewPRNumber = new Label();
            txtLimitRemaining = new TextBox();
            lblLimitRemaining = new Label();
            txtClubbedAmount = new TextBox();
            lblClubbedAmount = new Label();
            txtSelectedPRs = new TextBox();
            lblSelectedPRs = new Label();
            txtTotalPRs = new TextBox();
            lblTotalPRs = new Label();
            grpActions = new GroupBox();
            btnClose = new Button();
            btnPrint = new Button();
            btnExport = new Button();
            btnPreview = new Button();
            btnGenerateClubbedPR = new Button();
            grpAvailablePRs.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvAvailablePRs).BeginInit();
            grpFilter.SuspendLayout();
            grpClubbedPreview.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvClubbedPR).BeginInit();
            grpSummary.SuspendLayout();
            grpActions.SuspendLayout();
            SuspendLayout();
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Microsoft Sans Serif", 14F, FontStyle.Bold);
            lblTitle.Location = new Point(350, 10);
            lblTitle.Margin = new Padding(4, 0, 4, 0);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(230, 24);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "PR Clubbing by Vendor";
            // 
            // grpAvailablePRs
            // 
            grpAvailablePRs.Controls.Add(dgvAvailablePRs);
            grpAvailablePRs.Font = new Font("Microsoft Sans Serif", 9F);
            grpAvailablePRs.Location = new Point(14, 115);
            grpAvailablePRs.Margin = new Padding(4, 3, 4, 3);
            grpAvailablePRs.Name = "grpAvailablePRs";
            grpAvailablePRs.Padding = new Padding(4, 3, 4, 3);
            grpAvailablePRs.Size = new Size(992, 231);
            grpAvailablePRs.TabIndex = 1;
            grpAvailablePRs.TabStop = false;
            grpAvailablePRs.Text = "Available Approved PRs (Draft Status, Below 12L)";
            grpAvailablePRs.Enter += grpAvailablePRs_Enter;
            // 
            // dgvAvailablePRs
            // 
            dgvAvailablePRs.AllowUserToAddRows = false;
            dgvAvailablePRs.AllowUserToDeleteRows = false;
            dgvAvailablePRs.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvAvailablePRs.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvAvailablePRs.Columns.AddRange(new DataGridViewColumn[] { colPRSelect, colPRNumber, colPRProjectCode, colPRVendor, colPRAmount, ApprovedBy, colPRCurrency });
            dgvAvailablePRs.Location = new Point(7, 23);
            dgvAvailablePRs.Margin = new Padding(4, 3, 4, 3);
            dgvAvailablePRs.Name = "dgvAvailablePRs";
            dgvAvailablePRs.RowHeadersVisible = false;
            dgvAvailablePRs.Size = new Size(978, 201);
            dgvAvailablePRs.TabIndex = 0;
            dgvAvailablePRs.CellValueChanged += dgvAvailablePRs_CellValueChanged;
            dgvAvailablePRs.CurrentCellDirtyStateChanged += dgvAvailablePRs_CurrentCellDirtyStateChanged;
            // 
            // colPRSelect
            // 
            colPRSelect.HeaderText = "Select";
            colPRSelect.Name = "colPRSelect";
            // 
            // colPRNumber
            // 
            colPRNumber.HeaderText = "PR Number";
            colPRNumber.Name = "colPRNumber";
            // 
            // colPRProjectCode
            // 
            colPRProjectCode.HeaderText = "Project Code";
            colPRProjectCode.Name = "colPRProjectCode";
            // 
            // colPRVendor
            // 
            colPRVendor.HeaderText = "Vendor";
            colPRVendor.Name = "colPRVendor";
            // 
            // colPRAmount
            // 
            colPRAmount.HeaderText = "Amount";
            colPRAmount.Name = "colPRAmount";
            // 
            // ApprovedBy
            // 
            ApprovedBy.HeaderText = "Approved By";
            ApprovedBy.Name = "ApprovedBy";
            // 
            // colPRCurrency
            // 
            colPRCurrency.HeaderText = "Currency";
            colPRCurrency.Name = "colPRCurrency";
            // 
            // colPRProductNo
            // 
            colPRProductNo.HeaderText = "Product No";
            colPRProductNo.Name = "colPRProductNo";
            // 
            // grpFilter
            // 
            grpFilter.Controls.Add(btnClearFilter);
            grpFilter.Controls.Add(btnFilter);
            grpFilter.Controls.Add(cmbFilterVendor);
            grpFilter.Controls.Add(lblFilterVendor);
            grpFilter.Font = new Font("Microsoft Sans Serif", 9F);
            grpFilter.Location = new Point(14, 52);
            grpFilter.Margin = new Padding(4, 3, 4, 3);
            grpFilter.Name = "grpFilter";
            grpFilter.Padding = new Padding(4, 3, 4, 3);
            grpFilter.Size = new Size(992, 63);
            grpFilter.TabIndex = 2;
            grpFilter.TabStop = false;
            grpFilter.Text = "Filter";
            // 
            // btnClearFilter
            // 
            btnClearFilter.BackColor = Color.LightYellow;
            btnClearFilter.Location = new Point(443, 23);
            btnClearFilter.Margin = new Padding(4, 3, 4, 3);
            btnClearFilter.Name = "btnClearFilter";
            btnClearFilter.Size = new Size(93, 29);
            btnClearFilter.TabIndex = 3;
            btnClearFilter.Text = "Clear";
            btnClearFilter.UseVisualStyleBackColor = false;
            btnClearFilter.Click += btnClearFilter_Click;
            // 
            // btnFilter
            // 
            btnFilter.BackColor = Color.LightBlue;
            btnFilter.Location = new Point(338, 23);
            btnFilter.Margin = new Padding(4, 3, 4, 3);
            btnFilter.Name = "btnFilter";
            btnFilter.Size = new Size(93, 29);
            btnFilter.TabIndex = 2;
            btnFilter.Text = "Apply";
            btnFilter.UseVisualStyleBackColor = false;
            btnFilter.Click += btnFilter_Click;
            // 
            // cmbFilterVendor
            // 
            cmbFilterVendor.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbFilterVendor.FormattingEnabled = true;
            cmbFilterVendor.Location = new Point(82, 25);
            cmbFilterVendor.Margin = new Padding(4, 3, 4, 3);
            cmbFilterVendor.Name = "cmbFilterVendor";
            cmbFilterVendor.Size = new Size(233, 23);
            cmbFilterVendor.TabIndex = 1;
            // 
            // lblFilterVendor
            // 
            lblFilterVendor.AutoSize = true;
            lblFilterVendor.Location = new Point(18, 29);
            lblFilterVendor.Margin = new Padding(4, 0, 4, 0);
            lblFilterVendor.Name = "lblFilterVendor";
            lblFilterVendor.Size = new Size(49, 15);
            lblFilterVendor.TabIndex = 0;
            lblFilterVendor.Text = "Vendor:";
            // 
            // grpClubbedPreview
            // 
            grpClubbedPreview.Controls.Add(dgvClubbedPR);
            grpClubbedPreview.Font = new Font("Microsoft Sans Serif", 9F);
            grpClubbedPreview.Location = new Point(14, 358);
            grpClubbedPreview.Margin = new Padding(4, 3, 4, 3);
            grpClubbedPreview.Name = "grpClubbedPreview";
            grpClubbedPreview.Padding = new Padding(4, 3, 4, 3);
            grpClubbedPreview.Size = new Size(992, 231);
            grpClubbedPreview.TabIndex = 3;
            grpClubbedPreview.TabStop = false;
            grpClubbedPreview.Text = "Clubbed PR Preview";
            // 
            // dgvClubbedPR
            // 
            dgvClubbedPR.AllowUserToAddRows = false;
            dgvClubbedPR.AllowUserToDeleteRows = false;
            dgvClubbedPR.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvClubbedPR.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvClubbedPR.Columns.AddRange(new DataGridViewColumn[] { colClubItemCode, colClubItemName, colClubQty, colClubUnitCost, colClubTotal, colClubVendor, colClubSourcePR });
            dgvClubbedPR.Location = new Point(7, 23);
            dgvClubbedPR.Margin = new Padding(4, 3, 4, 3);
            dgvClubbedPR.Name = "dgvClubbedPR";
            dgvClubbedPR.ReadOnly = true;
            dgvClubbedPR.RowHeadersVisible = false;
            dgvClubbedPR.Size = new Size(978, 201);
            dgvClubbedPR.TabIndex = 0;
            // 
            // colClubItemCode
            // 
            colClubItemCode.HeaderText = "Item Code";
            colClubItemCode.Name = "colClubItemCode";
            colClubItemCode.ReadOnly = true;
            // 
            // colClubItemName
            // 
            colClubItemName.HeaderText = "Item Name";
            colClubItemName.Name = "colClubItemName";
            colClubItemName.ReadOnly = true;
            // 
            // colClubQty
            // 
            colClubQty.HeaderText = "Qty";
            colClubQty.Name = "colClubQty";
            colClubQty.ReadOnly = true;
            // 
            // colClubUnitCost
            // 
            colClubUnitCost.HeaderText = "Unit Cost";
            colClubUnitCost.Name = "colClubUnitCost";
            colClubUnitCost.ReadOnly = true;
            // 
            // colClubTotal
            // 
            colClubTotal.HeaderText = "Total";
            colClubTotal.Name = "colClubTotal";
            colClubTotal.ReadOnly = true;
            // 
            // colClubVendor
            // 
            colClubVendor.HeaderText = "Vendor";
            colClubVendor.Name = "colClubVendor";
            colClubVendor.ReadOnly = true;
            // 
            // colClubSourcePR
            // 
            colClubSourcePR.HeaderText = "Source PR";
            colClubSourcePR.Name = "colClubSourcePR";
            colClubSourcePR.ReadOnly = true;
            // 
            // grpSummary
            // 
            grpSummary.Controls.Add(txtNewPRNumber);
            grpSummary.Controls.Add(lblNewPRNumber);
            grpSummary.Controls.Add(txtLimitRemaining);
            grpSummary.Controls.Add(lblLimitRemaining);
            grpSummary.Controls.Add(txtClubbedAmount);
            grpSummary.Controls.Add(lblClubbedAmount);
            grpSummary.Controls.Add(txtSelectedPRs);
            grpSummary.Controls.Add(lblSelectedPRs);
            grpSummary.Controls.Add(txtTotalPRs);
            grpSummary.Controls.Add(lblTotalPRs);
            grpSummary.Font = new Font("Microsoft Sans Serif", 9F);
            grpSummary.Location = new Point(14, 600);
            grpSummary.Margin = new Padding(4, 3, 4, 3);
            grpSummary.Name = "grpSummary";
            grpSummary.Padding = new Padding(4, 3, 4, 3);
            grpSummary.Size = new Size(467, 173);
            grpSummary.TabIndex = 4;
            grpSummary.TabStop = false;
            grpSummary.Text = "Summary (Limit: 12L)";
            // 
            // txtNewPRNumber
            // 
            txtNewPRNumber.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);
            txtNewPRNumber.Location = new Point(140, 129);
            txtNewPRNumber.Margin = new Padding(4, 3, 4, 3);
            txtNewPRNumber.Name = "txtNewPRNumber";
            txtNewPRNumber.ReadOnly = true;
            txtNewPRNumber.Size = new Size(174, 21);
            txtNewPRNumber.TabIndex = 9;
            // 
            // lblNewPRNumber
            // 
            lblNewPRNumber.AutoSize = true;
            lblNewPRNumber.Location = new Point(18, 133);
            lblNewPRNumber.Margin = new Padding(4, 0, 4, 0);
            lblNewPRNumber.Name = "lblNewPRNumber";
            lblNewPRNumber.Size = new Size(103, 15);
            lblNewPRNumber.TabIndex = 8;
            lblNewPRNumber.Text = "New PR Number:";
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
            // txtClubbedAmount
            // 
            txtClubbedAmount.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);
            txtClubbedAmount.Location = new Point(140, 60);
            txtClubbedAmount.Margin = new Padding(4, 3, 4, 3);
            txtClubbedAmount.Name = "txtClubbedAmount";
            txtClubbedAmount.ReadOnly = true;
            txtClubbedAmount.Size = new Size(139, 21);
            txtClubbedAmount.TabIndex = 5;
            txtClubbedAmount.Text = "0.00";
            // 
            // lblClubbedAmount
            // 
            lblClubbedAmount.AutoSize = true;
            lblClubbedAmount.Location = new Point(18, 63);
            lblClubbedAmount.Margin = new Padding(4, 0, 4, 0);
            lblClubbedAmount.Name = "lblClubbedAmount";
            lblClubbedAmount.Size = new Size(101, 15);
            lblClubbedAmount.TabIndex = 4;
            lblClubbedAmount.Text = "Clubbed Amount:";
            // 
            // txtSelectedPRs
            // 
            txtSelectedPRs.Location = new Point(362, 25);
            txtSelectedPRs.Margin = new Padding(4, 3, 4, 3);
            txtSelectedPRs.Name = "txtSelectedPRs";
            txtSelectedPRs.ReadOnly = true;
            txtSelectedPRs.Size = new Size(93, 21);
            txtSelectedPRs.TabIndex = 3;
            txtSelectedPRs.Text = "0";
            // 
            // lblSelectedPRs
            // 
            lblSelectedPRs.AutoSize = true;
            lblSelectedPRs.Location = new Point(257, 29);
            lblSelectedPRs.Margin = new Padding(4, 0, 4, 0);
            lblSelectedPRs.Name = "lblSelectedPRs";
            lblSelectedPRs.Size = new Size(84, 15);
            lblSelectedPRs.TabIndex = 2;
            lblSelectedPRs.Text = "Selected PRs:";
            // 
            // txtTotalPRs
            // 
            txtTotalPRs.Location = new Point(140, 25);
            txtTotalPRs.Margin = new Padding(4, 3, 4, 3);
            txtTotalPRs.Name = "txtTotalPRs";
            txtTotalPRs.ReadOnly = true;
            txtTotalPRs.Size = new Size(93, 21);
            txtTotalPRs.TabIndex = 1;
            txtTotalPRs.Text = "0";
            // 
            // lblTotalPRs
            // 
            lblTotalPRs.AutoSize = true;
            lblTotalPRs.Location = new Point(18, 29);
            lblTotalPRs.Margin = new Padding(4, 0, 4, 0);
            lblTotalPRs.Name = "lblTotalPRs";
            lblTotalPRs.Size = new Size(63, 15);
            lblTotalPRs.TabIndex = 0;
            lblTotalPRs.Text = "Total PRs:";
            // 
            // grpActions
            // 
            grpActions.Controls.Add(btnClose);
            grpActions.Controls.Add(btnPrint);
            grpActions.Controls.Add(btnExport);
            grpActions.Controls.Add(btnPreview);
            grpActions.Controls.Add(btnGenerateClubbedPR);
            grpActions.Font = new Font("Microsoft Sans Serif", 9F);
            grpActions.Location = new Point(502, 600);
            grpActions.Margin = new Padding(4, 3, 4, 3);
            grpActions.Name = "grpActions";
            grpActions.Padding = new Padding(4, 3, 4, 3);
            grpActions.Size = new Size(504, 69);
            grpActions.TabIndex = 5;
            grpActions.TabStop = false;
            grpActions.Text = "Actions";
            // 
            // btnClose
            // 
            btnClose.BackColor = Color.LightCoral;
            btnClose.Location = new Point(18, 23);
            btnClose.Margin = new Padding(4, 3, 4, 3);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(93, 35);
            btnClose.TabIndex = 4;
            btnClose.Text = "Close";
            btnClose.UseVisualStyleBackColor = false;
            btnClose.Visible = false;
            btnClose.Click += btnClose_Click;
            // 
            // btnPrint
            // 
            btnPrint.BackColor = Color.LightSteelBlue;
            btnPrint.Location = new Point(391, 23);
            btnPrint.Margin = new Padding(4, 3, 4, 3);
            btnPrint.Name = "btnPrint";
            btnPrint.Size = new Size(93, 35);
            btnPrint.TabIndex = 3;
            btnPrint.Text = "Print";
            btnPrint.UseVisualStyleBackColor = false;
            btnPrint.Click += btnPrint_Click;
            // 
            // btnExport
            // 
            btnExport.BackColor = Color.LightSteelBlue;
            btnExport.Location = new Point(286, 23);
            btnExport.Margin = new Padding(4, 3, 4, 3);
            btnExport.Name = "btnExport";
            btnExport.Size = new Size(93, 35);
            btnExport.TabIndex = 2;
            btnExport.Text = "Export";
            btnExport.UseVisualStyleBackColor = false;
            btnExport.Click += btnExport_Click;
            // 
            // btnPreview
            // 
            btnPreview.BackColor = Color.LightBlue;
            btnPreview.Location = new Point(181, 23);
            btnPreview.Margin = new Padding(4, 3, 4, 3);
            btnPreview.Name = "btnPreview";
            btnPreview.Size = new Size(93, 35);
            btnPreview.TabIndex = 1;
            btnPreview.Text = "Preview";
            btnPreview.UseVisualStyleBackColor = false;
            btnPreview.Click += btnPreview_Click;
            // 
            // btnGenerateClubbedPR
            // 
            btnGenerateClubbedPR.BackColor = Color.LightGreen;
            btnGenerateClubbedPR.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);
            btnGenerateClubbedPR.Location = new Point(18, 23);
            btnGenerateClubbedPR.Margin = new Padding(4, 3, 4, 3);
            btnGenerateClubbedPR.Name = "btnGenerateClubbedPR";
            btnGenerateClubbedPR.Size = new Size(152, 35);
            btnGenerateClubbedPR.TabIndex = 0;
            btnGenerateClubbedPR.Text = "Generate Clubbed PR";
            btnGenerateClubbedPR.UseVisualStyleBackColor = false;
            btnGenerateClubbedPR.Click += btnGenerateClubbedPR_Click;
            // 
            // frmPRClubbing
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.WhiteSmoke;
            ClientSize = new Size(1020, 797);
            Controls.Add(grpActions);
            Controls.Add(grpSummary);
            Controls.Add(grpClubbedPreview);
            Controls.Add(grpFilter);
            Controls.Add(grpAvailablePRs);
            Controls.Add(lblTitle);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Margin = new Padding(4, 3, 4, 3);
            MaximizeBox = false;
            Name = "frmPRClubbing";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "PR Clubbing by Vendor";
            Load += frmPRClubbing_Load;
            grpAvailablePRs.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvAvailablePRs).EndInit();
            grpFilter.ResumeLayout(false);
            grpFilter.PerformLayout();
            grpClubbedPreview.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvClubbedPR).EndInit();
            grpSummary.ResumeLayout(false);
            grpSummary.PerformLayout();
            grpActions.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.GroupBox grpAvailablePRs;
        private System.Windows.Forms.DataGridView dgvAvailablePRs;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colPRSelect;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPRNumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPRProjectCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPRProductNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPRVendor;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPRAmount;
        private System.Windows.Forms.DataGridViewTextBoxColumn ApprovedBy;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPRCurrency;
        private System.Windows.Forms.GroupBox grpFilter;
        private System.Windows.Forms.Label lblFilterVendor;
        private System.Windows.Forms.ComboBox cmbFilterVendor;
        private System.Windows.Forms.Button btnFilter;
        private System.Windows.Forms.Button btnClearFilter;
        private System.Windows.Forms.GroupBox grpClubbedPreview;
        private System.Windows.Forms.DataGridView dgvClubbedPR;
        private System.Windows.Forms.DataGridViewTextBoxColumn colClubItemCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn colClubItemName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colClubQty;
        private System.Windows.Forms.DataGridViewTextBoxColumn colClubUnitCost;
        private System.Windows.Forms.DataGridViewTextBoxColumn colClubTotal;
        private System.Windows.Forms.DataGridViewTextBoxColumn colClubVendor;
        private System.Windows.Forms.DataGridViewTextBoxColumn colClubSourcePR;
        private System.Windows.Forms.GroupBox grpSummary;
        private System.Windows.Forms.Label lblTotalPRs;
        private System.Windows.Forms.TextBox txtTotalPRs;
        private System.Windows.Forms.Label lblSelectedPRs;
        private System.Windows.Forms.TextBox txtSelectedPRs;
        private System.Windows.Forms.Label lblClubbedAmount;
        private System.Windows.Forms.TextBox txtClubbedAmount;
        private System.Windows.Forms.Label lblLimitRemaining;
        private System.Windows.Forms.TextBox txtLimitRemaining;
        private System.Windows.Forms.Label lblNewPRNumber;
        private System.Windows.Forms.TextBox txtNewPRNumber;
        private System.Windows.Forms.GroupBox grpActions;
        private System.Windows.Forms.Button btnGenerateClubbedPR;
        private System.Windows.Forms.Button btnPreview;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.Button btnClose;
    }
}