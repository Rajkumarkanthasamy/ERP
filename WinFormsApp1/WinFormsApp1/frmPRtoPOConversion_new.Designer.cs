using System;
using System.Windows.Forms;
using System.Drawing;

namespace WinFormsApp1
{
    partial class frmPRtoPOConversion_new
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
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            pnlTop = new Panel();
            lblTitle = new Label();
            lblPRCount = new Label();
            lblCurrentUser = new Label();
            tlpMain = new TableLayoutPanel();
            pnlApprovedPRs = new Panel();
            dgvApprovedPRs = new DataGridView();
            lblApprovedPRs = new Label();
            pnlPRLines = new Panel();
            dgvPRLines = new DataGridView();
            pnlLinesHeader = new Panel();
            lblLineTotal = new Label();
            lblLineCount = new Label();
            chkSelectAllLines = new CheckBox();
            lblSelectedPR = new Label();
            pnlBottomSection = new Panel();
            tlpBottom = new TableLayoutPanel();
            pnlPODetails = new Panel();
            tlpPOForm = new TableLayoutPanel();
            lblPreparedBy = new Label();
            txtPreparedBy = new TextBox();
            lblAuthoriedBy = new Label();
            txtAuthoriedBy = new TextBox();
            lblPODeliveryDate = new Label();
            dtpPODeliveryDate = new DateTimePicker();
            lblCurrency = new Label();
            cmbCurrency = new ComboBox();
            lblPORemarks = new Label();
            txtPORemarks = new TextBox();
            pnlGenBtn = new Panel();
            btnGeneratePO = new Button();
            lblPODetails = new Label();
            pnlExistingPOs = new Panel();
            dgvExistingPOs = new DataGridView();
            lblExistingPOCount = new Label();
            lblExistingPOs = new Label();
            pnlButtons = new Panel();
            btnClose = new Button();
            btnRefresh = new Button();
            btnViewExistingPO = new Button();
            pnlTop.SuspendLayout();
            tlpMain.SuspendLayout();
            pnlApprovedPRs.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvApprovedPRs).BeginInit();
            pnlPRLines.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvPRLines).BeginInit();
            pnlLinesHeader.SuspendLayout();
            pnlBottomSection.SuspendLayout();
            tlpBottom.SuspendLayout();
            pnlPODetails.SuspendLayout();
            tlpPOForm.SuspendLayout();
            pnlGenBtn.SuspendLayout();
            pnlExistingPOs.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvExistingPOs).BeginInit();
            pnlButtons.SuspendLayout();
            SuspendLayout();
            // 
            // pnlTop
            // 
            pnlTop.BackColor = Color.FromArgb(26, 35, 126);
            pnlTop.Controls.Add(lblTitle);
            pnlTop.Controls.Add(lblPRCount);
            pnlTop.Controls.Add(lblCurrentUser);
            pnlTop.Dock = DockStyle.Top;
            pnlTop.Location = new Point(0, 0);
            pnlTop.Name = "pnlTop";
            pnlTop.Padding = new Padding(20, 10, 20, 10);
            pnlTop.Size = new Size(1450, 70);
            pnlTop.TabIndex = 1;
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(20, 12);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(252, 32);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "PR to PO Conversion";
            // 
            // lblPRCount
            // 
            lblPRCount.AutoSize = true;
            lblPRCount.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblPRCount.ForeColor = Color.FromArgb(129, 199, 132);
            lblPRCount.Location = new Point(20, 42);
            lblPRCount.Name = "lblPRCount";
            lblPRCount.Size = new Size(167, 19);
            lblPRCount.TabIndex = 1;
            lblPRCount.Text = "Approved PRs Ready: 0";
            // 
            // lblCurrentUser
            // 
            lblCurrentUser.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblCurrentUser.AutoSize = true;
            lblCurrentUser.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblCurrentUser.ForeColor = Color.FromArgb(200, 200, 200);
            lblCurrentUser.Location = new Point(1250, 0);
            lblCurrentUser.Name = "lblCurrentUser";
            lblCurrentUser.Size = new Size(99, 15);
            lblCurrentUser.TabIndex = 2;
            lblCurrentUser.Text = "User: Mr. Santosh";
            // 
            // tlpMain
            // 
            tlpMain.BackColor = Color.FromArgb(240, 242, 245);
            tlpMain.ColumnCount = 1;
            tlpMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tlpMain.Controls.Add(pnlApprovedPRs, 0, 0);
            tlpMain.Controls.Add(pnlPRLines, 0, 1);
            tlpMain.Controls.Add(pnlBottomSection, 0, 2);
            tlpMain.Controls.Add(pnlButtons, 0, 3);
            tlpMain.Dock = DockStyle.Fill;
            tlpMain.Location = new Point(0, 70);
            tlpMain.Name = "tlpMain";
            tlpMain.Padding = new Padding(12);
            tlpMain.RowCount = 4;
            tlpMain.RowStyles.Add(new RowStyle(SizeType.Absolute, 240F));
            tlpMain.RowStyles.Add(new RowStyle(SizeType.Absolute, 280F));
            tlpMain.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tlpMain.RowStyles.Add(new RowStyle(SizeType.Absolute, 55F));
            tlpMain.Size = new Size(1450, 830);
            tlpMain.TabIndex = 0;
            // 
            // pnlApprovedPRs
            // 
            pnlApprovedPRs.BackColor = Color.White;
            pnlApprovedPRs.BorderStyle = BorderStyle.FixedSingle;
            pnlApprovedPRs.Controls.Add(dgvApprovedPRs);
            pnlApprovedPRs.Controls.Add(lblApprovedPRs);
            pnlApprovedPRs.Dock = DockStyle.Fill;
            pnlApprovedPRs.Location = new Point(15, 15);
            pnlApprovedPRs.Name = "pnlApprovedPRs";
            pnlApprovedPRs.Padding = new Padding(10);
            pnlApprovedPRs.Size = new Size(1420, 234);
            pnlApprovedPRs.TabIndex = 0;
            // 
            // dgvApprovedPRs
            // 
            dgvApprovedPRs.AllowUserToAddRows = false;
            dgvApprovedPRs.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = Color.FromArgb(250, 250, 255);
            dgvApprovedPRs.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            dgvApprovedPRs.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            dgvApprovedPRs.BackgroundColor = Color.White;
            dgvApprovedPRs.BorderStyle = BorderStyle.None;
            dgvApprovedPRs.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvApprovedPRs.Dock = DockStyle.Fill;
            dgvApprovedPRs.Location = new Point(10, 38);
            dgvApprovedPRs.MultiSelect = false;
            dgvApprovedPRs.Name = "dgvApprovedPRs";
            dgvApprovedPRs.ReadOnly = true;
            dgvApprovedPRs.RowHeadersVisible = false;
            dgvApprovedPRs.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvApprovedPRs.Size = new Size(1398, 184);
            dgvApprovedPRs.TabIndex = 0;
            dgvApprovedPRs.CellClick += dgvApprovedPRs_CellClick;
            dgvApprovedPRs.DataError += dgvApprovedPRs_DataError;
            // 
            // lblApprovedPRs
            // 
            lblApprovedPRs.Dock = DockStyle.Top;
            lblApprovedPRs.Font = new Font("Segoe UI", 11F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblApprovedPRs.ForeColor = Color.FromArgb(26, 35, 126);
            lblApprovedPRs.Location = new Point(10, 10);
            lblApprovedPRs.Name = "lblApprovedPRs";
            lblApprovedPRs.Size = new Size(1398, 28);
            lblApprovedPRs.TabIndex = 1;
            lblApprovedPRs.Text = "Step 1: Select an Approved PR";
            lblApprovedPRs.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // pnlPRLines
            // 
            pnlPRLines.BackColor = Color.White;
            pnlPRLines.BorderStyle = BorderStyle.FixedSingle;
            pnlPRLines.Controls.Add(dgvPRLines);
            pnlPRLines.Controls.Add(pnlLinesHeader);
            pnlPRLines.Dock = DockStyle.Fill;
            pnlPRLines.Location = new Point(15, 255);
            pnlPRLines.Name = "pnlPRLines";
            pnlPRLines.Padding = new Padding(10);
            pnlPRLines.Size = new Size(1420, 274);
            pnlPRLines.TabIndex = 1;
            // 
            // dgvPRLines
            // 
            dgvPRLines.AllowUserToAddRows = false;
            dgvPRLines.AllowUserToDeleteRows = false;
            dataGridViewCellStyle2.BackColor = Color.FromArgb(250, 250, 255);
            dgvPRLines.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle2;
            dgvPRLines.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            dgvPRLines.BackgroundColor = Color.White;
            dgvPRLines.BorderStyle = BorderStyle.None;
            dgvPRLines.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvPRLines.Dock = DockStyle.Fill;
            dgvPRLines.Location = new Point(10, 42);
            dgvPRLines.Name = "dgvPRLines";
            dgvPRLines.RowHeadersVisible = false;
            dgvPRLines.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPRLines.Size = new Size(1398, 220);
            dgvPRLines.TabIndex = 0;
            dgvPRLines.CellValueChanged += dgvPRLines_CellValueChanged;
            dgvPRLines.CurrentCellDirtyStateChanged += dgvPRLines_CurrentCellDirtyStateChanged;
            dgvPRLines.DataError += dgvPRLines_DataError;
            // 
            // pnlLinesHeader
            // 
            pnlLinesHeader.BackColor = Color.White;
            pnlLinesHeader.Controls.Add(lblLineTotal);
            pnlLinesHeader.Controls.Add(lblLineCount);
            pnlLinesHeader.Controls.Add(chkSelectAllLines);
            pnlLinesHeader.Controls.Add(lblSelectedPR);
            pnlLinesHeader.Dock = DockStyle.Top;
            pnlLinesHeader.Location = new Point(10, 10);
            pnlLinesHeader.Name = "pnlLinesHeader";
            pnlLinesHeader.Size = new Size(1398, 32);
            pnlLinesHeader.TabIndex = 1;
            // 
            // lblLineTotal
            // 
            lblLineTotal.AutoSize = true;
            lblLineTotal.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblLineTotal.ForeColor = Color.FromArgb(230, 81, 0);
            lblLineTotal.Location = new Point(540, 6);
            lblLineTotal.Name = "lblLineTotal";
            lblLineTotal.Size = new Size(101, 19);
            lblLineTotal.TabIndex = 0;
            lblLineTotal.Text = "Total: Rs. 0.00";
            // 
            // lblLineCount
            // 
            lblLineCount.AutoSize = true;
            lblLineCount.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblLineCount.ForeColor = Color.FromArgb(100, 100, 100);
            lblLineCount.Location = new Point(420, 7);
            lblLineCount.Name = "lblLineCount";
            lblLineCount.Size = new Size(87, 15);
            lblLineCount.TabIndex = 1;
            lblLineCount.Text = "Selected: 0 / 0";
            // 
            // chkSelectAllLines
            // 
            chkSelectAllLines.AutoSize = true;
            chkSelectAllLines.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            chkSelectAllLines.Location = new Point(320, 6);
            chkSelectAllLines.Name = "chkSelectAllLines";
            chkSelectAllLines.Size = new Size(74, 19);
            chkSelectAllLines.TabIndex = 2;
            chkSelectAllLines.Text = "Select All";
            chkSelectAllLines.CheckedChanged += chkSelectAllLines_CheckedChanged;
            // 
            // lblSelectedPR
            // 
            lblSelectedPR.AutoSize = true;
            lblSelectedPR.Font = new Font("Segoe UI", 11F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblSelectedPR.ForeColor = Color.FromArgb(26, 35, 126);
            lblSelectedPR.Location = new Point(0, 5);
            lblSelectedPR.Name = "lblSelectedPR";
            lblSelectedPR.Size = new Size(257, 20);
            lblSelectedPR.TabIndex = 3;
            lblSelectedPR.Text = "Step 2: Select Line Items to Convert";
            // 
            // pnlBottomSection
            // 
            pnlBottomSection.BackColor = Color.FromArgb(240, 242, 245);
            pnlBottomSection.Controls.Add(tlpBottom);
            pnlBottomSection.Dock = DockStyle.Fill;
            pnlBottomSection.Location = new Point(15, 535);
            pnlBottomSection.Name = "pnlBottomSection";
            pnlBottomSection.Size = new Size(1420, 225);
            pnlBottomSection.TabIndex = 2;
            // 
            // tlpBottom
            // 
            tlpBottom.ColumnCount = 2;
            tlpBottom.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55F));
            tlpBottom.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45F));
            tlpBottom.Controls.Add(pnlPODetails, 0, 0);
            tlpBottom.Controls.Add(pnlExistingPOs, 1, 0);
            tlpBottom.Dock = DockStyle.Fill;
            tlpBottom.Location = new Point(0, 0);
            tlpBottom.Name = "tlpBottom";
            tlpBottom.RowCount = 1;
            tlpBottom.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tlpBottom.Size = new Size(1420, 225);
            tlpBottom.TabIndex = 0;
            // 
            // pnlPODetails
            // 
            pnlPODetails.BackColor = Color.White;
            pnlPODetails.BorderStyle = BorderStyle.FixedSingle;
            pnlPODetails.Controls.Add(tlpPOForm);
            pnlPODetails.Controls.Add(lblPODetails);
            pnlPODetails.Dock = DockStyle.Fill;
            pnlPODetails.Enabled = false;
            pnlPODetails.Location = new Point(3, 3);
            pnlPODetails.Name = "pnlPODetails";
            pnlPODetails.Padding = new Padding(12);
            pnlPODetails.Size = new Size(775, 219);
            pnlPODetails.TabIndex = 0;
            // 
            // tlpPOForm
            // 
            tlpPOForm.ColumnCount = 4;
            tlpPOForm.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 105F));
            tlpPOForm.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tlpPOForm.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 105F));
            tlpPOForm.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tlpPOForm.Controls.Add(lblPreparedBy, 0, 0);
            tlpPOForm.Controls.Add(txtPreparedBy, 1, 0);
            tlpPOForm.Controls.Add(lblAuthoriedBy, 2, 0);
            tlpPOForm.Controls.Add(txtAuthoriedBy, 3, 0);
            tlpPOForm.Controls.Add(lblPODeliveryDate, 0, 1);
            tlpPOForm.Controls.Add(dtpPODeliveryDate, 1, 1);
            tlpPOForm.Controls.Add(lblCurrency, 2, 1);
            tlpPOForm.Controls.Add(cmbCurrency, 3, 1);
            tlpPOForm.Controls.Add(lblPORemarks, 0, 2);
            tlpPOForm.Controls.Add(txtPORemarks, 1, 2);
            tlpPOForm.Controls.Add(pnlGenBtn, 0, 3);
            tlpPOForm.Dock = DockStyle.Fill;
            tlpPOForm.Location = new Point(12, 40);
            tlpPOForm.Name = "tlpPOForm";
            tlpPOForm.Padding = new Padding(0, 8, 0, 0);
            tlpPOForm.RowCount = 4;
            tlpPOForm.RowStyles.Add(new RowStyle(SizeType.Absolute, 34F));
            tlpPOForm.RowStyles.Add(new RowStyle(SizeType.Absolute, 34F));
            tlpPOForm.RowStyles.Add(new RowStyle(SizeType.Absolute, 34F));
            tlpPOForm.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
            tlpPOForm.Size = new Size(749, 165);
            tlpPOForm.TabIndex = 0;
            // 
            // lblPreparedBy
            // 
            lblPreparedBy.Dock = DockStyle.Fill;
            lblPreparedBy.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblPreparedBy.Location = new Point(3, 8);
            lblPreparedBy.Name = "lblPreparedBy";
            lblPreparedBy.Size = new Size(99, 34);
            lblPreparedBy.TabIndex = 0;
            lblPreparedBy.Text = "Prepared By:*";
            lblPreparedBy.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtPreparedBy
            // 
            txtPreparedBy.Dock = DockStyle.Fill;
            txtPreparedBy.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtPreparedBy.Location = new Point(108, 11);
            txtPreparedBy.Name = "txtPreparedBy";
            txtPreparedBy.Size = new Size(263, 24);
            txtPreparedBy.TabIndex = 1;
            // 
            // lblAuthoriedBy
            // 
            lblAuthoriedBy.Dock = DockStyle.Fill;
            lblAuthoriedBy.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblAuthoriedBy.Location = new Point(377, 8);
            lblAuthoriedBy.Name = "lblAuthoriedBy";
            lblAuthoriedBy.Size = new Size(99, 34);
            lblAuthoriedBy.TabIndex = 2;
            lblAuthoriedBy.Text = "Authoried By:";
            lblAuthoriedBy.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtAuthoriedBy
            // 
            txtAuthoriedBy.Dock = DockStyle.Fill;
            txtAuthoriedBy.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtAuthoriedBy.Location = new Point(482, 11);
            txtAuthoriedBy.Name = "txtAuthoriedBy";
            txtAuthoriedBy.Size = new Size(264, 24);
            txtAuthoriedBy.TabIndex = 3;
            // 
            // lblPODeliveryDate
            // 
            lblPODeliveryDate.Dock = DockStyle.Fill;
            lblPODeliveryDate.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblPODeliveryDate.Location = new Point(3, 42);
            lblPODeliveryDate.Name = "lblPODeliveryDate";
            lblPODeliveryDate.Size = new Size(99, 34);
            lblPODeliveryDate.TabIndex = 4;
            lblPODeliveryDate.Text = "Delivery Date:*";
            lblPODeliveryDate.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // dtpPODeliveryDate
            // 
            dtpPODeliveryDate.Dock = DockStyle.Fill;
            dtpPODeliveryDate.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular, GraphicsUnit.Point, 0);
            dtpPODeliveryDate.Format = DateTimePickerFormat.Short;
            dtpPODeliveryDate.Location = new Point(108, 45);
            dtpPODeliveryDate.Name = "dtpPODeliveryDate";
            dtpPODeliveryDate.Size = new Size(263, 24);
            dtpPODeliveryDate.TabIndex = 5;
            // 
            // lblCurrency
            // 
            lblCurrency.Dock = DockStyle.Fill;
            lblCurrency.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblCurrency.Location = new Point(377, 42);
            lblCurrency.Name = "lblCurrency";
            lblCurrency.Size = new Size(99, 34);
            lblCurrency.TabIndex = 6;
            lblCurrency.Text = "Currency:";
            lblCurrency.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // cmbCurrency
            // 
            cmbCurrency.Dock = DockStyle.Fill;
            cmbCurrency.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbCurrency.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular, GraphicsUnit.Point, 0);
            cmbCurrency.Items.AddRange(new object[] { "INR", "USD", "EUR", "GBP" });
            cmbCurrency.Location = new Point(482, 45);
            cmbCurrency.Name = "cmbCurrency";
            cmbCurrency.Size = new Size(264, 25);
            cmbCurrency.TabIndex = 7;
            // 
            // lblPORemarks
            // 
            lblPORemarks.Dock = DockStyle.Fill;
            lblPORemarks.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblPORemarks.Location = new Point(3, 76);
            lblPORemarks.Name = "lblPORemarks";
            lblPORemarks.Size = new Size(99, 34);
            lblPORemarks.TabIndex = 8;
            lblPORemarks.Text = "Remarks:";
            lblPORemarks.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtPORemarks
            // 
            tlpPOForm.SetColumnSpan(txtPORemarks, 3);
            txtPORemarks.Dock = DockStyle.Fill;
            txtPORemarks.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtPORemarks.Location = new Point(108, 79);
            txtPORemarks.Multiline = true;
            txtPORemarks.Name = "txtPORemarks";
            txtPORemarks.ScrollBars = ScrollBars.Vertical;
            txtPORemarks.Size = new Size(638, 28);
            txtPORemarks.TabIndex = 9;
            // 
            // pnlGenBtn
            // 
            tlpPOForm.SetColumnSpan(pnlGenBtn, 4);
            pnlGenBtn.Controls.Add(btnGeneratePO);
            pnlGenBtn.Dock = DockStyle.Fill;
            pnlGenBtn.Location = new Point(3, 113);
            pnlGenBtn.Name = "pnlGenBtn";
            pnlGenBtn.Size = new Size(743, 49);
            pnlGenBtn.TabIndex = 10;
            // 
            // btnGeneratePO
            // 
            btnGeneratePO.BackColor = Color.FromArgb(26, 35, 126);
            btnGeneratePO.Cursor = Cursors.Hand;
            btnGeneratePO.Dock = DockStyle.Left;
            btnGeneratePO.FlatAppearance.BorderSize = 0;
            btnGeneratePO.FlatStyle = FlatStyle.Flat;
            btnGeneratePO.Font = new Font("Segoe UI", 11F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnGeneratePO.ForeColor = Color.White;
            btnGeneratePO.Location = new Point(0, 0);
            btnGeneratePO.Name = "btnGeneratePO";
            btnGeneratePO.Size = new Size(170, 49);
            btnGeneratePO.TabIndex = 0;
            btnGeneratePO.Text = "Generate PO";
            btnGeneratePO.UseVisualStyleBackColor = false;
            btnGeneratePO.Click += btnGeneratePO_Click;
            // 
            // lblPODetails
            // 
            lblPODetails.Dock = DockStyle.Top;
            lblPODetails.Font = new Font("Segoe UI", 11F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblPODetails.ForeColor = Color.FromArgb(26, 35, 126);
            lblPODetails.Location = new Point(12, 12);
            lblPODetails.Name = "lblPODetails";
            lblPODetails.Size = new Size(749, 28);
            lblPODetails.TabIndex = 1;
            lblPODetails.Text = "Step 3: Enter PO Details";
            lblPODetails.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // pnlExistingPOs
            // 
            pnlExistingPOs.BackColor = Color.White;
            pnlExistingPOs.BorderStyle = BorderStyle.FixedSingle;
            pnlExistingPOs.Controls.Add(dgvExistingPOs);
            pnlExistingPOs.Controls.Add(lblExistingPOCount);
            pnlExistingPOs.Controls.Add(lblExistingPOs);
            pnlExistingPOs.Dock = DockStyle.Fill;
            pnlExistingPOs.Location = new Point(784, 3);
            pnlExistingPOs.Name = "pnlExistingPOs";
            pnlExistingPOs.Padding = new Padding(10);
            pnlExistingPOs.Size = new Size(633, 219);
            pnlExistingPOs.TabIndex = 1;
            // 
            // dgvExistingPOs
            // 
            dgvExistingPOs.AllowUserToAddRows = false;
            dgvExistingPOs.AllowUserToDeleteRows = false;
            dataGridViewCellStyle3.BackColor = Color.FromArgb(250, 250, 255);
            dgvExistingPOs.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle3;
            dgvExistingPOs.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            dgvExistingPOs.BackgroundColor = Color.White;
            dgvExistingPOs.BorderStyle = BorderStyle.None;
            dgvExistingPOs.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvExistingPOs.Dock = DockStyle.Fill;
            dgvExistingPOs.Location = new Point(10, 38);
            dgvExistingPOs.Name = "dgvExistingPOs";
            dgvExistingPOs.ReadOnly = true;
            dgvExistingPOs.RowHeadersVisible = false;
            dgvExistingPOs.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvExistingPOs.Size = new Size(611, 145);
            dgvExistingPOs.TabIndex = 0;
            // 
            // lblExistingPOCount
            // 
            lblExistingPOCount.Dock = DockStyle.Bottom;
            lblExistingPOCount.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblExistingPOCount.ForeColor = Color.FromArgb(100, 100, 100);
            lblExistingPOCount.Location = new Point(10, 183);
            lblExistingPOCount.Name = "lblExistingPOCount";
            lblExistingPOCount.Size = new Size(611, 24);
            lblExistingPOCount.TabIndex = 1;
            lblExistingPOCount.Text = "Existing POs: 0";
            lblExistingPOCount.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblExistingPOs
            // 
            lblExistingPOs.Dock = DockStyle.Top;
            lblExistingPOs.Font = new Font("Segoe UI", 11F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblExistingPOs.ForeColor = Color.FromArgb(26, 35, 126);
            lblExistingPOs.Location = new Point(10, 10);
            lblExistingPOs.Name = "lblExistingPOs";
            lblExistingPOs.Size = new Size(611, 28);
            lblExistingPOs.TabIndex = 2;
            lblExistingPOs.Text = "Existing POs for Selected PR";
            lblExistingPOs.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // pnlButtons
            // 
            pnlButtons.BackColor = Color.White;
            pnlButtons.Controls.Add(btnClose);
            pnlButtons.Controls.Add(btnRefresh);
            pnlButtons.Controls.Add(btnViewExistingPO);
            pnlButtons.Dock = DockStyle.Fill;
            pnlButtons.Location = new Point(15, 766);
            pnlButtons.Name = "pnlButtons";
            pnlButtons.Padding = new Padding(12, 10, 12, 10);
            pnlButtons.Size = new Size(1420, 49);
            pnlButtons.TabIndex = 3;
            // 
            // btnClose
            // 
            btnClose.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnClose.BackColor = Color.FromArgb(117, 117, 117);
            btnClose.Cursor = Cursors.Hand;
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnClose.ForeColor = Color.White;
            btnClose.Location = new Point(1220, 10);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(110, 35);
            btnClose.TabIndex = 0;
            btnClose.Text = "Close";
            btnClose.UseVisualStyleBackColor = false;
            btnClose.Click += btnClose_Click;
            // 
            // btnRefresh
            // 
            btnRefresh.BackColor = Color.FromArgb(33, 150, 243);
            btnRefresh.Cursor = Cursors.Hand;
            btnRefresh.FlatAppearance.BorderSize = 0;
            btnRefresh.FlatStyle = FlatStyle.Flat;
            btnRefresh.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnRefresh.ForeColor = Color.White;
            btnRefresh.Location = new Point(190, 10);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(110, 35);
            btnRefresh.TabIndex = 1;
            btnRefresh.Text = "Refresh";
            btnRefresh.UseVisualStyleBackColor = false;
            btnRefresh.Click += btnRefresh_Click;
            // 
            // btnViewExistingPO
            // 
            btnViewExistingPO.BackColor = Color.FromArgb(96, 125, 139);
            btnViewExistingPO.Cursor = Cursors.Hand;
            btnViewExistingPO.FlatAppearance.BorderSize = 0;
            btnViewExistingPO.FlatStyle = FlatStyle.Flat;
            btnViewExistingPO.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnViewExistingPO.ForeColor = Color.White;
            btnViewExistingPO.Location = new Point(12, 10);
            btnViewExistingPO.Name = "btnViewExistingPO";
            btnViewExistingPO.Size = new Size(160, 35);
            btnViewExistingPO.TabIndex = 2;
            btnViewExistingPO.Text = "View Existing POs";
            btnViewExistingPO.UseVisualStyleBackColor = false;
            btnViewExistingPO.Click += btnViewExistingPO_Click;
            // 
            // frmPRtoPOConversion_new
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(240, 242, 245);
            ClientSize = new Size(1450, 900);
            Controls.Add(tlpMain);
            Controls.Add(pnlTop);
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Name = "frmPRtoPOConversion_new";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "PR to PO Conversion";
            Load += frmPRtoPOConversion_Load;
            pnlTop.ResumeLayout(false);
            pnlTop.PerformLayout();
            tlpMain.ResumeLayout(false);
            pnlApprovedPRs.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvApprovedPRs).EndInit();
            pnlPRLines.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvPRLines).EndInit();
            pnlLinesHeader.ResumeLayout(false);
            pnlLinesHeader.PerformLayout();
            pnlBottomSection.ResumeLayout(false);
            tlpBottom.ResumeLayout(false);
            pnlPODetails.ResumeLayout(false);
            tlpPOForm.ResumeLayout(false);
            tlpPOForm.PerformLayout();
            pnlGenBtn.ResumeLayout(false);
            pnlExistingPOs.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvExistingPOs).EndInit();
            pnlButtons.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        // Top Header
        private Panel pnlTop;
        private Label lblTitle;
        private Label lblPRCount;
        private Label lblCurrentUser;

        // Main Layout
        private TableLayoutPanel tlpMain;

        // Row 1: Approved PRs
        private Panel pnlApprovedPRs;
        private Label lblApprovedPRs;
        private DataGridView dgvApprovedPRs;

        // Row 2: PR Lines
        private Panel pnlPRLines;
        private Panel pnlLinesHeader;
        private Label lblSelectedPR;
        private CheckBox chkSelectAllLines;
        private Label lblLineCount;
        private Label lblLineTotal;
        private DataGridView dgvPRLines;

        // Row 3: Bottom section
        private Panel pnlBottomSection;
        private TableLayoutPanel tlpBottom;

        // PO Details
        private Panel pnlPODetails;
        private Label lblPODetails;
        private Label lblPreparedBy;
        private TextBox txtPreparedBy;
        private Label lblAuthoriedBy;
        private TextBox txtAuthoriedBy;
        private Label lblPODeliveryDate;
        private DateTimePicker dtpPODeliveryDate;
        private Label lblCurrency;
        private ComboBox cmbCurrency;
        private Label lblPORemarks;
        private TextBox txtPORemarks;
        private Button btnGeneratePO;

        // Existing POs
        private Panel pnlExistingPOs;
        private Label lblExistingPOs;
        private DataGridView dgvExistingPOs;
        private Label lblExistingPOCount;

        // Row 4: Buttons
        private Panel pnlButtons;
        private Button btnViewExistingPO;
        private Button btnRefresh;
        private Button btnClose;
        private TableLayoutPanel tlpPOForm;
        private Panel pnlGenBtn;
    }
}