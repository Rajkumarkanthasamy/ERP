using System;
using System.Windows.Forms;
using System.Drawing;

namespace WinFormsApp1
{
    partial class frmPRtoPOConversion
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
            pnlBottom = new Panel();
            btnClose = new Button();
            btnRefresh = new Button();
            btnViewExistingPO = new Button();
            pnlCenter = new Panel();
            tlpCenter = new TableLayoutPanel();
            pnlPRLines = new Panel();
            dgvPRLines = new DataGridView();
            pnlLineStats = new Panel();
            lblLineTotal = new Label();
            lblLineCount = new Label();
            lblSelectedPR = new Label();
            pnlPODetails = new Panel();
            tlpPOForm = new TableLayoutPanel();
            btnGeneratePO = new Button();
            lblPreparedBy = new Label();
            txtPreparedBy = new TextBox();
            lblPODeliveryDate = new Label();
            dtpPODeliveryDate = new DateTimePicker();
            lblPORemarks = new Label();
            txtPORemarks = new TextBox();
            lblAuthoriedBy = new Label();
            txtAuthoriedBy = new TextBox();
            lblCurrency = new Label();
            cmbCurrency = new ComboBox();
            lblPODetails = new Label();
            lblExistingPOs = new Label();
            pnlLeft = new Panel();
            dgvApprovedPRs = new DataGridView();
            lblApprovedPRs = new Label();
            lblExistingPOCount = new Label();
            dgvExistingPOs = new DataGridView();
            pnlRight = new Panel();
            panel1 = new Panel();
            panel2 = new Panel();
            panel4 = new Panel();
            panel3 = new Panel();
            pnlTop.SuspendLayout();
            pnlBottom.SuspendLayout();
            pnlCenter.SuspendLayout();
            tlpCenter.SuspendLayout();
            pnlPRLines.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvPRLines).BeginInit();
            pnlLineStats.SuspendLayout();
            pnlPODetails.SuspendLayout();
            tlpPOForm.SuspendLayout();
            pnlLeft.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvApprovedPRs).BeginInit();
            ((System.ComponentModel.ISupportInitialize)dgvExistingPOs).BeginInit();
            pnlRight.SuspendLayout();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            panel4.SuspendLayout();
            panel3.SuspendLayout();
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
            pnlTop.TabIndex = 2;
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTitle.ForeColor = Color.White;
            lblTitle.Location = new Point(20, 15);
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
            // pnlBottom
            // 
            pnlBottom.BackColor = Color.White;
            pnlBottom.Controls.Add(btnClose);
            pnlBottom.Controls.Add(btnRefresh);
            pnlBottom.Controls.Add(btnViewExistingPO);
            pnlBottom.Dock = DockStyle.Bottom;
            pnlBottom.Location = new Point(0, 795);
            pnlBottom.Name = "pnlBottom";
            pnlBottom.Padding = new Padding(15, 10, 15, 10);
            pnlBottom.Size = new Size(1450, 55);
            pnlBottom.TabIndex = 1;
            // 
            // btnClose
            // 
            btnClose.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnClose.BackColor = Color.FromArgb(117, 117, 117);
            btnClose.Cursor = Cursors.Hand;
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.FlatStyle = FlatStyle.Flat;
            btnClose.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnClose.ForeColor = Color.White;
            btnClose.Location = new Point(1250, 10);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(100, 35);
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
            btnRefresh.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnRefresh.ForeColor = Color.White;
            btnRefresh.Location = new Point(180, 10);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(100, 35);
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
            btnViewExistingPO.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnViewExistingPO.ForeColor = Color.White;
            btnViewExistingPO.Location = new Point(15, 10);
            btnViewExistingPO.Name = "btnViewExistingPO";
            btnViewExistingPO.Size = new Size(150, 35);
            btnViewExistingPO.TabIndex = 2;
            btnViewExistingPO.Text = "View Existing POs";
            btnViewExistingPO.UseVisualStyleBackColor = false;
            btnViewExistingPO.Click += btnViewExistingPO_Click;
            // 
            // pnlCenter
            // 
            pnlCenter.BackColor = Color.White;
            pnlCenter.BorderStyle = BorderStyle.FixedSingle;
            pnlCenter.Controls.Add(tlpCenter);
            pnlCenter.Dock = DockStyle.Fill;
            pnlCenter.Location = new Point(0, 0);
            pnlCenter.Name = "pnlCenter";
            pnlCenter.Padding = new Padding(10);
            pnlCenter.Size = new Size(1448, 289);
            pnlCenter.TabIndex = 1;
            // 
            // tlpCenter
            // 
            tlpCenter.ColumnCount = 1;
            tlpCenter.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tlpCenter.Controls.Add(pnlPRLines, 0, 0);
            tlpCenter.Dock = DockStyle.Left;
            tlpCenter.Location = new Point(10, 10);
            tlpCenter.Name = "tlpCenter";
            tlpCenter.RowCount = 1;
            tlpCenter.RowStyles.Add(new RowStyle(SizeType.Percent, 55F));
            tlpCenter.Size = new Size(955, 267);
            tlpCenter.TabIndex = 0;
            // 
            // pnlPRLines
            // 
            pnlPRLines.BackColor = Color.White;
            pnlPRLines.Controls.Add(dgvPRLines);
            pnlPRLines.Controls.Add(pnlLineStats);
            pnlPRLines.Controls.Add(lblSelectedPR);
            pnlPRLines.Dock = DockStyle.Fill;
            pnlPRLines.Location = new Point(3, 3);
            pnlPRLines.Name = "pnlPRLines";
            pnlPRLines.Padding = new Padding(0, 0, 0, 5);
            pnlPRLines.Size = new Size(949, 261);
            pnlPRLines.TabIndex = 0;
            // 
            // dgvPRLines
            // 
            dgvPRLines.AllowUserToAddRows = false;
            dgvPRLines.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = Color.FromArgb(250, 250, 255);
            dgvPRLines.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            dgvPRLines.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            dgvPRLines.BackgroundColor = Color.White;
            dgvPRLines.BorderStyle = BorderStyle.None;
            dgvPRLines.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvPRLines.Dock = DockStyle.Fill;
            dgvPRLines.Location = new Point(0, 28);
            dgvPRLines.Name = "dgvPRLines";
            dgvPRLines.ReadOnly = true;
            dgvPRLines.RowHeadersVisible = false;
            dgvPRLines.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPRLines.Size = new Size(949, 200);
            dgvPRLines.TabIndex = 0;
            dgvPRLines.DataError += dgvPRLines_DataError;
            // 
            // pnlLineStats
            // 
            pnlLineStats.BackColor = Color.White;
            pnlLineStats.Controls.Add(lblLineTotal);
            pnlLineStats.Controls.Add(lblLineCount);
            pnlLineStats.Dock = DockStyle.Bottom;
            pnlLineStats.Location = new Point(0, 228);
            pnlLineStats.Name = "pnlLineStats";
            pnlLineStats.Size = new Size(949, 28);
            pnlLineStats.TabIndex = 1;
            // 
            // lblLineTotal
            // 
            lblLineTotal.AutoSize = true;
            lblLineTotal.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblLineTotal.ForeColor = Color.FromArgb(230, 81, 0);
            lblLineTotal.Location = new Point(120, 4);
            lblLineTotal.Name = "lblLineTotal";
            lblLineTotal.Size = new Size(132, 19);
            lblLineTotal.TabIndex = 0;
            lblLineTotal.Text = "Line Total: Rs. 0.00";
            // 
            // lblLineCount
            // 
            lblLineCount.AutoSize = true;
            lblLineCount.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblLineCount.Location = new Point(0, 5);
            lblLineCount.Name = "lblLineCount";
            lblLineCount.Size = new Size(48, 15);
            lblLineCount.TabIndex = 1;
            lblLineCount.Text = "Lines: 0";
            // 
            // lblSelectedPR
            // 
            lblSelectedPR.Dock = DockStyle.Top;
            lblSelectedPR.Font = new Font("Segoe UI", 11F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblSelectedPR.ForeColor = Color.FromArgb(26, 35, 126);
            lblSelectedPR.Location = new Point(0, 0);
            lblSelectedPR.Name = "lblSelectedPR";
            lblSelectedPR.Size = new Size(949, 28);
            lblSelectedPR.TabIndex = 2;
            lblSelectedPR.Text = "Selected PR: None";
            lblSelectedPR.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // pnlPODetails
            // 
            pnlPODetails.BackColor = Color.FromArgb(248, 249, 250);
            pnlPODetails.BorderStyle = BorderStyle.FixedSingle;
            pnlPODetails.Controls.Add(tlpPOForm);
            pnlPODetails.Controls.Add(lblPODetails);
            pnlPODetails.Dock = DockStyle.Fill;
            pnlPODetails.Enabled = false;
            pnlPODetails.Location = new Point(0, 0);
            pnlPODetails.Name = "pnlPODetails";
            pnlPODetails.Padding = new Padding(12);
            pnlPODetails.Size = new Size(440, 286);
            pnlPODetails.TabIndex = 1;
            // 
            // tlpPOForm
            // 
            tlpPOForm.ColumnCount = 4;
            tlpPOForm.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 109F));
            tlpPOForm.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tlpPOForm.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 28F));
            tlpPOForm.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tlpPOForm.Controls.Add(btnGeneratePO, 1, 5);
            tlpPOForm.Controls.Add(lblPreparedBy, 0, 0);
            tlpPOForm.Controls.Add(txtPreparedBy, 1, 0);
            tlpPOForm.Controls.Add(lblPODeliveryDate, 0, 1);
            tlpPOForm.Controls.Add(dtpPODeliveryDate, 1, 1);
            tlpPOForm.Controls.Add(lblPORemarks, 0, 2);
            tlpPOForm.Controls.Add(txtPORemarks, 1, 2);
            tlpPOForm.Controls.Add(lblAuthoriedBy, 0, 3);
            tlpPOForm.Controls.Add(txtAuthoriedBy, 1, 3);
            tlpPOForm.Controls.Add(lblCurrency, 0, 4);
            tlpPOForm.Controls.Add(cmbCurrency, 1, 4);
            tlpPOForm.Dock = DockStyle.Fill;
            tlpPOForm.Location = new Point(12, 35);
            tlpPOForm.Name = "tlpPOForm";
            tlpPOForm.Padding = new Padding(0, 8, 0, 0);
            tlpPOForm.RowCount = 6;
            tlpPOForm.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            tlpPOForm.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            tlpPOForm.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
            tlpPOForm.RowStyles.Add(new RowStyle(SizeType.Absolute, 31F));
            tlpPOForm.RowStyles.Add(new RowStyle(SizeType.Absolute, 33F));
            tlpPOForm.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));
            tlpPOForm.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tlpPOForm.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tlpPOForm.Size = new Size(414, 237);
            tlpPOForm.TabIndex = 0;
            // 
            // btnGeneratePO
            // 
            btnGeneratePO.BackColor = Color.FromArgb(26, 35, 126);
            tlpPOForm.SetColumnSpan(btnGeneratePO, 2);
            btnGeneratePO.Cursor = Cursors.Hand;
            btnGeneratePO.FlatAppearance.BorderSize = 0;
            btnGeneratePO.FlatStyle = FlatStyle.Flat;
            btnGeneratePO.Font = new Font("Segoe UI", 10F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnGeneratePO.ForeColor = Color.White;
            btnGeneratePO.Location = new Point(112, 171);
            btnGeneratePO.Name = "btnGeneratePO";
            btnGeneratePO.Size = new Size(137, 36);
            btnGeneratePO.TabIndex = 0;
            btnGeneratePO.Text = "Generate PO";
            btnGeneratePO.UseVisualStyleBackColor = false;
            btnGeneratePO.Click += btnGeneratePO_Click;
            // 
            // lblPreparedBy
            // 
            lblPreparedBy.Dock = DockStyle.Fill;
            lblPreparedBy.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblPreparedBy.Location = new Point(3, 8);
            lblPreparedBy.Name = "lblPreparedBy";
            lblPreparedBy.Size = new Size(103, 32);
            lblPreparedBy.TabIndex = 0;
            lblPreparedBy.Text = "Prepared By:*";
            lblPreparedBy.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtPreparedBy
            // 
            tlpPOForm.SetColumnSpan(txtPreparedBy, 3);
            txtPreparedBy.Dock = DockStyle.Fill;
            txtPreparedBy.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtPreparedBy.Location = new Point(112, 11);
            txtPreparedBy.Name = "txtPreparedBy";
            txtPreparedBy.Size = new Size(299, 23);
            txtPreparedBy.TabIndex = 1;
            // 
            // lblPODeliveryDate
            // 
            lblPODeliveryDate.Dock = DockStyle.Fill;
            lblPODeliveryDate.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblPODeliveryDate.Location = new Point(3, 40);
            lblPODeliveryDate.Name = "lblPODeliveryDate";
            lblPODeliveryDate.Size = new Size(103, 32);
            lblPODeliveryDate.TabIndex = 4;
            lblPODeliveryDate.Text = "Delivery Date:*";
            lblPODeliveryDate.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // dtpPODeliveryDate
            // 
            tlpPOForm.SetColumnSpan(dtpPODeliveryDate, 3);
            dtpPODeliveryDate.Dock = DockStyle.Fill;
            dtpPODeliveryDate.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            dtpPODeliveryDate.Format = DateTimePickerFormat.Short;
            dtpPODeliveryDate.Location = new Point(112, 43);
            dtpPODeliveryDate.Name = "dtpPODeliveryDate";
            dtpPODeliveryDate.Size = new Size(299, 23);
            dtpPODeliveryDate.TabIndex = 5;
            // 
            // lblPORemarks
            // 
            lblPORemarks.Dock = DockStyle.Fill;
            lblPORemarks.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblPORemarks.Location = new Point(3, 72);
            lblPORemarks.Name = "lblPORemarks";
            lblPORemarks.Size = new Size(103, 32);
            lblPORemarks.TabIndex = 8;
            lblPORemarks.Text = "Remarks:";
            lblPORemarks.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtPORemarks
            // 
            tlpPOForm.SetColumnSpan(txtPORemarks, 3);
            txtPORemarks.Dock = DockStyle.Fill;
            txtPORemarks.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtPORemarks.Location = new Point(112, 75);
            txtPORemarks.Multiline = true;
            txtPORemarks.Name = "txtPORemarks";
            txtPORemarks.ScrollBars = ScrollBars.Vertical;
            txtPORemarks.Size = new Size(299, 26);
            txtPORemarks.TabIndex = 9;
            // 
            // lblAuthoriedBy
            // 
            lblAuthoriedBy.Dock = DockStyle.Fill;
            lblAuthoriedBy.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblAuthoriedBy.Location = new Point(3, 104);
            lblAuthoriedBy.Name = "lblAuthoriedBy";
            lblAuthoriedBy.Size = new Size(103, 31);
            lblAuthoriedBy.TabIndex = 2;
            lblAuthoriedBy.Text = "Authoried By:";
            lblAuthoriedBy.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtAuthoriedBy
            // 
            tlpPOForm.SetColumnSpan(txtAuthoriedBy, 3);
            txtAuthoriedBy.Dock = DockStyle.Fill;
            txtAuthoriedBy.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtAuthoriedBy.Location = new Point(112, 107);
            txtAuthoriedBy.Name = "txtAuthoriedBy";
            txtAuthoriedBy.Size = new Size(299, 23);
            txtAuthoriedBy.TabIndex = 3;
            // 
            // lblCurrency
            // 
            lblCurrency.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblCurrency.Location = new Point(3, 135);
            lblCurrency.Name = "lblCurrency";
            lblCurrency.Size = new Size(94, 32);
            lblCurrency.TabIndex = 6;
            lblCurrency.Text = "Currency:";
            lblCurrency.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // cmbCurrency
            // 
            tlpPOForm.SetColumnSpan(cmbCurrency, 3);
            cmbCurrency.Dock = DockStyle.Fill;
            cmbCurrency.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbCurrency.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            cmbCurrency.Items.AddRange(new object[] { "INR", "USD", "EUR", "GBP" });
            cmbCurrency.Location = new Point(112, 138);
            cmbCurrency.Name = "cmbCurrency";
            cmbCurrency.Size = new Size(299, 23);
            cmbCurrency.TabIndex = 7;
            // 
            // lblPODetails
            // 
            lblPODetails.Dock = DockStyle.Top;
            lblPODetails.Font = new Font("Segoe UI", 11F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblPODetails.ForeColor = Color.FromArgb(26, 35, 126);
            lblPODetails.Location = new Point(12, 12);
            lblPODetails.Name = "lblPODetails";
            lblPODetails.Size = new Size(414, 23);
            lblPODetails.TabIndex = 1;
            lblPODetails.Text = "PO Generation Details";
            lblPODetails.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblExistingPOs
            // 
            lblExistingPOs.Dock = DockStyle.Top;
            lblExistingPOs.Font = new Font("Segoe UI", 11F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblExistingPOs.ForeColor = Color.FromArgb(26, 35, 126);
            lblExistingPOs.Location = new Point(10, 10);
            lblExistingPOs.Name = "lblExistingPOs";
            lblExistingPOs.Size = new Size(1428, 30);
            lblExistingPOs.TabIndex = 2;
            lblExistingPOs.Text = "Existing POs for PR";
            lblExistingPOs.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // pnlLeft
            // 
            pnlLeft.BackColor = Color.White;
            pnlLeft.BorderStyle = BorderStyle.FixedSingle;
            pnlLeft.Controls.Add(dgvApprovedPRs);
            pnlLeft.Controls.Add(lblApprovedPRs);
            pnlLeft.Dock = DockStyle.Fill;
            pnlLeft.Location = new Point(0, 0);
            pnlLeft.Name = "pnlLeft";
            pnlLeft.Padding = new Padding(10);
            pnlLeft.Size = new Size(1450, 204);
            pnlLeft.TabIndex = 0;
            // 
            // dgvApprovedPRs
            // 
            dgvApprovedPRs.AllowUserToAddRows = false;
            dgvApprovedPRs.AllowUserToDeleteRows = false;
            dataGridViewCellStyle2.BackColor = Color.FromArgb(250, 250, 255);
            dgvApprovedPRs.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle2;
            dgvApprovedPRs.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            dgvApprovedPRs.BackgroundColor = Color.White;
            dgvApprovedPRs.BorderStyle = BorderStyle.None;
            dgvApprovedPRs.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvApprovedPRs.Dock = DockStyle.Fill;
            dgvApprovedPRs.Location = new Point(10, 37);
            dgvApprovedPRs.MultiSelect = false;
            dgvApprovedPRs.Name = "dgvApprovedPRs";
            dgvApprovedPRs.ReadOnly = true;
            dgvApprovedPRs.RowHeadersVisible = false;
            dgvApprovedPRs.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvApprovedPRs.Size = new Size(1428, 155);
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
            lblApprovedPRs.Size = new Size(1428, 27);
            lblApprovedPRs.TabIndex = 1;
            lblApprovedPRs.Text = "Approved PRs (Click to Select)";
            lblApprovedPRs.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblExistingPOCount
            // 
            lblExistingPOCount.Dock = DockStyle.Bottom;
            lblExistingPOCount.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblExistingPOCount.Location = new Point(10, 172);
            lblExistingPOCount.Name = "lblExistingPOCount";
            lblExistingPOCount.Size = new Size(1428, 24);
            lblExistingPOCount.TabIndex = 1;
            lblExistingPOCount.Text = "Existing POs: 0";
            lblExistingPOCount.TextAlign = ContentAlignment.MiddleLeft;
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
            dgvExistingPOs.Location = new Point(10, 40);
            dgvExistingPOs.Name = "dgvExistingPOs";
            dgvExistingPOs.ReadOnly = true;
            dgvExistingPOs.RowHeadersVisible = false;
            dgvExistingPOs.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvExistingPOs.Size = new Size(1428, 132);
            dgvExistingPOs.TabIndex = 0;
            // 
            // pnlRight
            // 
            pnlRight.BackColor = Color.White;
            pnlRight.BorderStyle = BorderStyle.FixedSingle;
            pnlRight.Controls.Add(dgvExistingPOs);
            pnlRight.Controls.Add(lblExistingPOCount);
            pnlRight.Controls.Add(lblExistingPOs);
            pnlRight.Dock = DockStyle.Fill;
            pnlRight.Location = new Point(0, 0);
            pnlRight.Name = "pnlRight";
            pnlRight.Padding = new Padding(10);
            pnlRight.Size = new Size(1450, 208);
            pnlRight.TabIndex = 2;
            // 
            // panel1
            // 
            panel1.Controls.Add(pnlLeft);
            panel1.Location = new Point(0, 76);
            panel1.Name = "panel1";
            panel1.Size = new Size(1450, 204);
            panel1.TabIndex = 3;
            // 
            // panel2
            // 
            panel2.Controls.Add(panel4);
            panel2.Controls.Add(pnlCenter);
            panel2.Location = new Point(2, 286);
            panel2.Name = "panel2";
            panel2.Size = new Size(1448, 289);
            panel2.TabIndex = 4;
            // 
            // panel4
            // 
            panel4.Controls.Add(pnlPODetails);
            panel4.Location = new Point(981, 3);
            panel4.Name = "panel4";
            panel4.Size = new Size(440, 286);
            panel4.TabIndex = 2;
            // 
            // panel3
            // 
            panel3.Controls.Add(pnlRight);
            panel3.Location = new Point(0, 581);
            panel3.Name = "panel3";
            panel3.Size = new Size(1450, 208);
            panel3.TabIndex = 5;
            // 
            // frmPRtoPOConversion
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(245, 245, 250);
            ClientSize = new Size(1450, 850);
            Controls.Add(panel3);
            Controls.Add(panel2);
            Controls.Add(panel1);
            Controls.Add(pnlBottom);
            Controls.Add(pnlTop);
            Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Name = "frmPRtoPOConversion";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "PR to PO Conversion";
            Load += frmPRtoPOConversion_Load;
            pnlTop.ResumeLayout(false);
            pnlTop.PerformLayout();
            pnlBottom.ResumeLayout(false);
            pnlCenter.ResumeLayout(false);
            tlpCenter.ResumeLayout(false);
            pnlPRLines.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvPRLines).EndInit();
            pnlLineStats.ResumeLayout(false);
            pnlLineStats.PerformLayout();
            pnlPODetails.ResumeLayout(false);
            tlpPOForm.ResumeLayout(false);
            tlpPOForm.PerformLayout();
            pnlLeft.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgvApprovedPRs).EndInit();
            ((System.ComponentModel.ISupportInitialize)dgvExistingPOs).EndInit();
            pnlRight.ResumeLayout(false);
            panel1.ResumeLayout(false);
            panel2.ResumeLayout(false);
            panel4.ResumeLayout(false);
            panel3.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        // Top Header
        private Panel pnlTop;
        private Label lblTitle;
        private Label lblPRCount;
        private Label lblCurrentUser;

        // Bottom Panel
        private Panel pnlBottom;
        private Button btnViewExistingPO;
        private Button btnRefresh;
        private Button btnClose;
        private Panel pnlCenter;
        private TableLayoutPanel tlpCenter;
        private Panel pnlPRLines;
        private DataGridView dgvPRLines;
        private Panel pnlLineStats;
        private Label lblLineTotal;
        private Label lblLineCount;
        private Label lblSelectedPR;
        private Panel pnlPODetails;
        private TableLayoutPanel tlpPOForm;
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
        private Label lblPODetails;
        private Panel pnlRight;
        private DataGridView dgvExistingPOs;
        private Label lblExistingPOCount;
        private Panel pnlLeft;
        private DataGridView dgvApprovedPRs;
        private Label lblApprovedPRs;
        private Label lblExistingPOs;
        private Panel panel1;
        private Panel panel2;
        private Panel panel3;
        private Panel panel4;
    }
}