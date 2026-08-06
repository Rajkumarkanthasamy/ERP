//namespace WinFormsApp1
//{
//    partial class Form1
//    {
//        /// <summary>
//        ///  Required designer variable.
//        /// </summary>
//        private System.ComponentModel.IContainer components = null;

//        /// <summary>
//        ///  Clean up any resources being used.
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
//        ///  Required method for Designer support - do not modify
//        ///  the contents of this method with the code editor.
//        /// </summary>
//        private void InitializeComponent()
//        {
//            components = new System.ComponentModel.Container();
//            AutoScaleMode = AutoScaleMode.Font;
//            ClientSize = new Size(800, 450);
//            Text = "Form1";
//        }

//        #endregion
//    }
//}


namespace WinFormsApp1
{
    partial class frmItemCodeCreation
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
            grpRequestDetails = new GroupBox();
            lblCriticalityLevel = new Label();
            cmbCriticalityLevel = new ComboBox();
            lblDrawingReference = new Label();
            txtDrawingReference = new TextBox();
            lblUnitOfMeasure = new Label();
            cmbUnitOfMeasure = new ComboBox();
            lblTechnicalSpec = new Label();
            txtTechnicalSpec = new TextBox();
            lblItemDescription = new Label();
            txtItemDescription = new TextBox();
            lblItemCategory = new Label();
            cmbItemCategory = new ComboBox();
            lblDepartment = new Label();
            cmbDepartment = new ComboBox();
            lblRequestor = new Label();
            txtRequestor = new TextBox();
            lblRequestDate = new Label();
            dtpRequestDate = new DateTimePicker();
            lblHSNCode = new Label();
            txtHSNCode = new TextBox();
            grpItemCodeStructure = new GroupBox();
            btnGenerateCode = new Button();
            txtGeneratedItemCode = new TextBox();
            lblGeneratedItemCode = new Label();
            txtSequentialNumber = new TextBox();
            lblSequentialNumber = new Label();
            txtCategoryCode = new TextBox();
            lblCategoryCode = new Label();
            txtPrefix = new TextBox();
            lblPrefix = new Label();
            grpApproval = new GroupBox();
            txtApprovalRemarks = new TextBox();
            lblApprovalRemarks = new Label();
            cmbApprovalStatus = new ComboBox();
            lblApprovalStatus = new Label();
            cmbApprovalAuthority = new ComboBox();
            lblApprovalAuthority = new Label();
            cmbAuthorizedCreator = new ComboBox();
            lblAuthorizedCreator = new Label();
            grpItemStructure = new GroupBox();
            grpLocation = new GroupBox();
            txtBinNumber = new TextBox();
            lblBinNumber = new Label();
            txtStoreLocation = new TextBox();
            lblStoreLocation = new Label();
            grpEmergency = new GroupBox();
            cmbEmergencyApproval = new ComboBox();
            lblEmergencyApproval = new Label();
            chkEmergency = new CheckBox();
            btnSubmit = new Button();
            btnClear = new Button();
            btnExit = new Button();
            btnPrint = new Button();
            lblPreparedBy = new Label();
            txtPreparedBy = new TextBox();
            lblApprovedBy = new Label();
            txtApprovedBy = new TextBox();
            lblEffectiveDate = new Label();
            dtpEffectiveDate = new DateTimePicker();
            grpRequestDetails.SuspendLayout();
            grpItemCodeStructure.SuspendLayout();
            grpApproval.SuspendLayout();
            grpLocation.SuspendLayout();
            grpEmergency.SuspendLayout();
            SuspendLayout();
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Microsoft Sans Serif", 14F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTitle.Location = new Point(327, 10);
            lblTitle.Margin = new Padding(4, 0, 4, 0);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(400, 24);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "SOP - Item Code Creation in ERP (ICCRF)";
            // 
            // grpRequestDetails
            // 
            grpRequestDetails.Controls.Add(lblCriticalityLevel);
            grpRequestDetails.Controls.Add(cmbCriticalityLevel);
            grpRequestDetails.Controls.Add(lblDrawingReference);
            grpRequestDetails.Controls.Add(txtDrawingReference);
            grpRequestDetails.Controls.Add(lblUnitOfMeasure);
            grpRequestDetails.Controls.Add(cmbUnitOfMeasure);
            grpRequestDetails.Controls.Add(lblTechnicalSpec);
            grpRequestDetails.Controls.Add(txtTechnicalSpec);
            grpRequestDetails.Controls.Add(lblItemDescription);
            grpRequestDetails.Controls.Add(txtItemDescription);
            grpRequestDetails.Controls.Add(lblItemCategory);
            grpRequestDetails.Controls.Add(cmbItemCategory);
            grpRequestDetails.Controls.Add(lblDepartment);
            grpRequestDetails.Controls.Add(cmbDepartment);
            grpRequestDetails.Controls.Add(lblRequestor);
            grpRequestDetails.Controls.Add(txtRequestor);
            grpRequestDetails.Controls.Add(lblRequestDate);
            grpRequestDetails.Controls.Add(dtpRequestDate);
            grpRequestDetails.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            grpRequestDetails.Location = new Point(14, 52);
            grpRequestDetails.Margin = new Padding(4, 3, 4, 3);
            grpRequestDetails.Name = "grpRequestDetails";
            grpRequestDetails.Padding = new Padding(4, 3, 4, 3);
            grpRequestDetails.Size = new Size(560, 369);
            grpRequestDetails.TabIndex = 1;
            grpRequestDetails.TabStop = false;
            grpRequestDetails.Text = "Step 1: Item Code Request Initiation";
            // 
            // lblCriticalityLevel
            // 
            lblCriticalityLevel.AutoSize = true;
            lblCriticalityLevel.Location = new Point(18, 335);
            lblCriticalityLevel.Margin = new Padding(4, 0, 4, 0);
            lblCriticalityLevel.Name = "lblCriticalityLevel";
            lblCriticalityLevel.Size = new Size(92, 15);
            lblCriticalityLevel.TabIndex = 16;
            lblCriticalityLevel.Text = "Criticality (ABC):";
            // 
            // cmbCriticalityLevel
            // 
            cmbCriticalityLevel.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbCriticalityLevel.FormattingEnabled = true;
            cmbCriticalityLevel.Location = new Point(175, 330);
            cmbCriticalityLevel.Margin = new Padding(4, 3, 4, 3);
            cmbCriticalityLevel.Name = "cmbCriticalityLevel";
            cmbCriticalityLevel.Size = new Size(139, 23);
            cmbCriticalityLevel.TabIndex = 17;
            // 
            // lblDrawingReference
            // 
            lblDrawingReference.AutoSize = true;
            lblDrawingReference.Location = new Point(18, 300);
            lblDrawingReference.Margin = new Padding(4, 0, 4, 0);
            lblDrawingReference.Name = "lblDrawingReference";
            lblDrawingReference.Size = new Size(116, 15);
            lblDrawingReference.TabIndex = 14;
            lblDrawingReference.Text = "Drawing Reference:";
            // 
            // txtDrawingReference
            // 
            txtDrawingReference.Location = new Point(175, 295);
            txtDrawingReference.Margin = new Padding(4, 3, 4, 3);
            txtDrawingReference.Name = "txtDrawingReference";
            txtDrawingReference.Size = new Size(233, 21);
            txtDrawingReference.TabIndex = 15;
            // 
            // lblUnitOfMeasure
            // 
            lblUnitOfMeasure.AutoSize = true;
            lblUnitOfMeasure.Location = new Point(18, 265);
            lblUnitOfMeasure.Margin = new Padding(4, 0, 4, 0);
            lblUnitOfMeasure.Name = "lblUnitOfMeasure";
            lblUnitOfMeasure.Size = new Size(97, 15);
            lblUnitOfMeasure.TabIndex = 12;
            lblUnitOfMeasure.Text = "Unit of Measure:";
            // 
            // cmbUnitOfMeasure
            // 
            cmbUnitOfMeasure.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbUnitOfMeasure.FormattingEnabled = true;
            cmbUnitOfMeasure.Location = new Point(175, 261);
            cmbUnitOfMeasure.Margin = new Padding(4, 3, 4, 3);
            cmbUnitOfMeasure.Name = "cmbUnitOfMeasure";
            cmbUnitOfMeasure.Size = new Size(139, 23);
            cmbUnitOfMeasure.TabIndex = 13;
            // 
            // lblTechnicalSpec
            // 
            lblTechnicalSpec.AutoSize = true;
            lblTechnicalSpec.Location = new Point(18, 208);
            lblTechnicalSpec.Margin = new Padding(4, 0, 4, 0);
            lblTechnicalSpec.Name = "lblTechnicalSpec";
            lblTechnicalSpec.Size = new Size(122, 15);
            lblTechnicalSpec.TabIndex = 10;
            lblTechnicalSpec.Text = "Technical Spec / Ref:";
            // 
            // txtTechnicalSpec
            // 
            txtTechnicalSpec.Location = new Point(175, 203);
            txtTechnicalSpec.Margin = new Padding(4, 3, 4, 3);
            txtTechnicalSpec.Multiline = true;
            txtTechnicalSpec.Name = "txtTechnicalSpec";
            txtTechnicalSpec.Size = new Size(349, 46);
            txtTechnicalSpec.TabIndex = 11;
            // 
            // lblItemDescription
            // 
            lblItemDescription.AutoSize = true;
            lblItemDescription.Location = new Point(18, 173);
            lblItemDescription.Margin = new Padding(4, 0, 4, 0);
            lblItemDescription.Name = "lblItemDescription";
            lblItemDescription.Size = new Size(99, 15);
            lblItemDescription.TabIndex = 8;
            lblItemDescription.Text = "Item Description:";
            // 
            // txtItemDescription
            // 
            txtItemDescription.Location = new Point(175, 168);
            txtItemDescription.Margin = new Padding(4, 3, 4, 3);
            txtItemDescription.MaxLength = 40;
            txtItemDescription.Name = "txtItemDescription";
            txtItemDescription.Size = new Size(349, 21);
            txtItemDescription.TabIndex = 9;
            txtItemDescription.TextChanged += txtItemDescription_TextChanged;
            // 
            // lblItemCategory
            // 
            lblItemCategory.AutoSize = true;
            lblItemCategory.Location = new Point(18, 138);
            lblItemCategory.Margin = new Padding(4, 0, 4, 0);
            lblItemCategory.Name = "lblItemCategory";
            lblItemCategory.Size = new Size(85, 15);
            lblItemCategory.TabIndex = 6;
            lblItemCategory.Text = "Item Category:";
            // 
            // cmbItemCategory
            // 
            cmbItemCategory.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbItemCategory.FormattingEnabled = true;
            cmbItemCategory.Location = new Point(175, 134);
            cmbItemCategory.Margin = new Padding(4, 3, 4, 3);
            cmbItemCategory.Name = "cmbItemCategory";
            cmbItemCategory.Size = new Size(233, 23);
            cmbItemCategory.TabIndex = 7;
            cmbItemCategory.SelectedIndexChanged += cmbItemCategory_SelectedIndexChanged;
            // 
            // lblDepartment
            // 
            lblDepartment.AutoSize = true;
            lblDepartment.Location = new Point(18, 104);
            lblDepartment.Margin = new Padding(4, 0, 4, 0);
            lblDepartment.Name = "lblDepartment";
            lblDepartment.Size = new Size(75, 15);
            lblDepartment.TabIndex = 4;
            lblDepartment.Text = "Department:";
            // 
            // cmbDepartment
            // 
            cmbDepartment.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbDepartment.FormattingEnabled = true;
            cmbDepartment.Location = new Point(175, 99);
            cmbDepartment.Margin = new Padding(4, 3, 4, 3);
            cmbDepartment.Name = "cmbDepartment";
            cmbDepartment.Size = new Size(233, 23);
            cmbDepartment.TabIndex = 5;
            // 
            // lblRequestor
            // 
            lblRequestor.AutoSize = true;
            lblRequestor.Location = new Point(18, 69);
            lblRequestor.Margin = new Padding(4, 0, 4, 0);
            lblRequestor.Name = "lblRequestor";
            lblRequestor.Size = new Size(67, 15);
            lblRequestor.TabIndex = 2;
            lblRequestor.Text = "Requestor:";
            // 
            // txtRequestor
            // 
            txtRequestor.Location = new Point(175, 65);
            txtRequestor.Margin = new Padding(4, 3, 4, 3);
            txtRequestor.Name = "txtRequestor";
            txtRequestor.Size = new Size(233, 21);
            txtRequestor.TabIndex = 3;
            // 
            // lblRequestDate
            // 
            lblRequestDate.AutoSize = true;
            lblRequestDate.Location = new Point(18, 35);
            lblRequestDate.Margin = new Padding(4, 0, 4, 0);
            lblRequestDate.Name = "lblRequestDate";
            lblRequestDate.Size = new Size(85, 15);
            lblRequestDate.TabIndex = 0;
            lblRequestDate.Text = "Request Date:";
            // 
            // dtpRequestDate
            // 
            dtpRequestDate.Format = DateTimePickerFormat.Short;
            dtpRequestDate.Location = new Point(175, 30);
            dtpRequestDate.Margin = new Padding(4, 3, 4, 3);
            dtpRequestDate.Name = "dtpRequestDate";
            dtpRequestDate.Size = new Size(139, 21);
            dtpRequestDate.TabIndex = 1;
            // 
            // lblHSNCode
            // 
            lblHSNCode.AutoSize = true;
            lblHSNCode.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblHSNCode.Location = new Point(18, 35);
            lblHSNCode.Margin = new Padding(4, 0, 4, 0);
            lblHSNCode.Name = "lblHSNCode";
            lblHSNCode.Size = new Size(68, 15);
            lblHSNCode.TabIndex = 0;
            lblHSNCode.Text = "HSN Code:";
            // 
            // txtHSNCode
            // 
            txtHSNCode.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtHSNCode.Location = new Point(175, 30);
            txtHSNCode.Margin = new Padding(4, 3, 4, 3);
            txtHSNCode.Name = "txtHSNCode";
            txtHSNCode.Size = new Size(174, 21);
            txtHSNCode.TabIndex = 1;
            // 
            // grpItemCodeStructure
            // 
            grpItemCodeStructure.Controls.Add(btnGenerateCode);
            grpItemCodeStructure.Controls.Add(txtGeneratedItemCode);
            grpItemCodeStructure.Controls.Add(lblGeneratedItemCode);
            grpItemCodeStructure.Controls.Add(txtSequentialNumber);
            grpItemCodeStructure.Controls.Add(lblSequentialNumber);
            grpItemCodeStructure.Controls.Add(txtCategoryCode);
            grpItemCodeStructure.Controls.Add(lblCategoryCode);
            grpItemCodeStructure.Controls.Add(txtPrefix);
            grpItemCodeStructure.Controls.Add(lblPrefix);
            grpItemCodeStructure.Controls.Add(txtHSNCode);
            grpItemCodeStructure.Controls.Add(lblHSNCode);
            grpItemCodeStructure.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            grpItemCodeStructure.Location = new Point(595, 52);
            grpItemCodeStructure.Margin = new Padding(4, 3, 4, 3);
            grpItemCodeStructure.Name = "grpItemCodeStructure";
            grpItemCodeStructure.Padding = new Padding(4, 3, 4, 3);
            grpItemCodeStructure.Size = new Size(537, 231);
            grpItemCodeStructure.TabIndex = 2;
            grpItemCodeStructure.TabStop = false;
            grpItemCodeStructure.Text = "Step 2 & 5: Item Code Creation / Naming Standards";
            // 
            // btnGenerateCode
            // 
            btnGenerateCode.BackColor = Color.LightBlue;
            btnGenerateCode.Location = new Point(373, 166);
            btnGenerateCode.Margin = new Padding(4, 3, 4, 3);
            btnGenerateCode.Name = "btnGenerateCode";
            btnGenerateCode.Size = new Size(140, 29);
            btnGenerateCode.TabIndex = 10;
            btnGenerateCode.Text = "Generate Code";
            btnGenerateCode.UseVisualStyleBackColor = false;
            btnGenerateCode.Click += btnGenerateCode_Click;
            // 
            // txtGeneratedItemCode
            // 
            txtGeneratedItemCode.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            txtGeneratedItemCode.Location = new Point(175, 168);
            txtGeneratedItemCode.Margin = new Padding(4, 3, 4, 3);
            txtGeneratedItemCode.Name = "txtGeneratedItemCode";
            txtGeneratedItemCode.ReadOnly = true;
            txtGeneratedItemCode.Size = new Size(174, 21);
            txtGeneratedItemCode.TabIndex = 9;
            // 
            // lblGeneratedItemCode
            // 
            lblGeneratedItemCode.AutoSize = true;
            lblGeneratedItemCode.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblGeneratedItemCode.Location = new Point(18, 173);
            lblGeneratedItemCode.Margin = new Padding(4, 0, 4, 0);
            lblGeneratedItemCode.Name = "lblGeneratedItemCode";
            lblGeneratedItemCode.Size = new Size(147, 15);
            lblGeneratedItemCode.TabIndex = 8;
            lblGeneratedItemCode.Text = "Generated Item Code:";
            // 
            // txtSequentialNumber
            // 
            txtSequentialNumber.Location = new Point(175, 134);
            txtSequentialNumber.Margin = new Padding(4, 3, 4, 3);
            txtSequentialNumber.Name = "txtSequentialNumber";
            txtSequentialNumber.Size = new Size(116, 21);
            txtSequentialNumber.TabIndex = 7;
            // 
            // lblSequentialNumber
            // 
            lblSequentialNumber.AutoSize = true;
            lblSequentialNumber.Location = new Point(18, 138);
            lblSequentialNumber.Margin = new Padding(4, 0, 4, 0);
            lblSequentialNumber.Name = "lblSequentialNumber";
            lblSequentialNumber.Size = new Size(117, 15);
            lblSequentialNumber.TabIndex = 6;
            lblSequentialNumber.Text = "Sequential Number:";
            // 
            // txtCategoryCode
            // 
            txtCategoryCode.Location = new Point(175, 99);
            txtCategoryCode.Margin = new Padding(4, 3, 4, 3);
            txtCategoryCode.Name = "txtCategoryCode";
            txtCategoryCode.ReadOnly = true;
            txtCategoryCode.Size = new Size(116, 21);
            txtCategoryCode.TabIndex = 5;
            // 
            // lblCategoryCode
            // 
            lblCategoryCode.AutoSize = true;
            lblCategoryCode.Location = new Point(18, 104);
            lblCategoryCode.Margin = new Padding(4, 0, 4, 0);
            lblCategoryCode.Name = "lblCategoryCode";
            lblCategoryCode.Size = new Size(90, 15);
            lblCategoryCode.TabIndex = 4;
            lblCategoryCode.Text = "Category Code:";
            // 
            // txtPrefix
            // 
            txtPrefix.Location = new Point(175, 65);
            txtPrefix.Margin = new Padding(4, 3, 4, 3);
            txtPrefix.Name = "txtPrefix";
            txtPrefix.ReadOnly = true;
            txtPrefix.Size = new Size(116, 21);
            txtPrefix.TabIndex = 3;
            // 
            // lblPrefix
            // 
            lblPrefix.AutoSize = true;
            lblPrefix.Location = new Point(18, 69);
            lblPrefix.Margin = new Padding(4, 0, 4, 0);
            lblPrefix.Name = "lblPrefix";
            lblPrefix.Size = new Size(41, 15);
            lblPrefix.TabIndex = 2;
            lblPrefix.Text = "Prefix:";
            // 
            // grpApproval
            // 
            grpApproval.Controls.Add(txtApprovalRemarks);
            grpApproval.Controls.Add(lblApprovalRemarks);
            grpApproval.Controls.Add(cmbApprovalStatus);
            grpApproval.Controls.Add(lblApprovalStatus);
            grpApproval.Controls.Add(cmbApprovalAuthority);
            grpApproval.Controls.Add(lblApprovalAuthority);
            grpApproval.Controls.Add(cmbAuthorizedCreator);
            grpApproval.Controls.Add(lblAuthorizedCreator);
            grpApproval.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            grpApproval.Location = new Point(14, 433);
            grpApproval.Margin = new Padding(4, 3, 4, 3);
            grpApproval.Name = "grpApproval";
            grpApproval.Padding = new Padding(4, 3, 4, 3);
            grpApproval.Size = new Size(560, 208);
            grpApproval.TabIndex = 3;
            grpApproval.TabStop = false;
            grpApproval.Text = "Step 3: Approval & Activation";
            // 
            // txtApprovalRemarks
            // 
            txtApprovalRemarks.Location = new Point(175, 134);
            txtApprovalRemarks.Margin = new Padding(4, 3, 4, 3);
            txtApprovalRemarks.Multiline = true;
            txtApprovalRemarks.Name = "txtApprovalRemarks";
            txtApprovalRemarks.Size = new Size(349, 57);
            txtApprovalRemarks.TabIndex = 7;
            // 
            // lblApprovalRemarks
            // 
            lblApprovalRemarks.AutoSize = true;
            lblApprovalRemarks.Location = new Point(18, 138);
            lblApprovalRemarks.Margin = new Padding(4, 0, 4, 0);
            lblApprovalRemarks.Name = "lblApprovalRemarks";
            lblApprovalRemarks.Size = new Size(60, 15);
            lblApprovalRemarks.TabIndex = 6;
            lblApprovalRemarks.Text = "Remarks:";
            // 
            // cmbApprovalStatus
            // 
            cmbApprovalStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbApprovalStatus.FormattingEnabled = true;
            cmbApprovalStatus.Location = new Point(175, 99);
            cmbApprovalStatus.Margin = new Padding(4, 3, 4, 3);
            cmbApprovalStatus.Name = "cmbApprovalStatus";
            cmbApprovalStatus.Size = new Size(174, 23);
            cmbApprovalStatus.TabIndex = 5;
            // 
            // lblApprovalStatus
            // 
            lblApprovalStatus.AutoSize = true;
            lblApprovalStatus.Location = new Point(18, 104);
            lblApprovalStatus.Margin = new Padding(4, 0, 4, 0);
            lblApprovalStatus.Name = "lblApprovalStatus";
            lblApprovalStatus.Size = new Size(94, 15);
            lblApprovalStatus.TabIndex = 4;
            lblApprovalStatus.Text = "Approval Status:";
            // 
            // cmbApprovalAuthority
            // 
            cmbApprovalAuthority.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbApprovalAuthority.FormattingEnabled = true;
            cmbApprovalAuthority.Location = new Point(175, 65);
            cmbApprovalAuthority.Margin = new Padding(4, 3, 4, 3);
            cmbApprovalAuthority.Name = "cmbApprovalAuthority";
            cmbApprovalAuthority.Size = new Size(233, 23);
            cmbApprovalAuthority.TabIndex = 3;
            // 
            // lblApprovalAuthority
            // 
            lblApprovalAuthority.AutoSize = true;
            lblApprovalAuthority.Location = new Point(18, 69);
            lblApprovalAuthority.Margin = new Padding(4, 0, 4, 0);
            lblApprovalAuthority.Name = "lblApprovalAuthority";
            lblApprovalAuthority.Size = new Size(106, 15);
            lblApprovalAuthority.TabIndex = 2;
            lblApprovalAuthority.Text = "Approval Authority:";
            // 
            // cmbAuthorizedCreator
            // 
            cmbAuthorizedCreator.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbAuthorizedCreator.FormattingEnabled = true;
            cmbAuthorizedCreator.Location = new Point(175, 30);
            cmbAuthorizedCreator.Margin = new Padding(4, 3, 4, 3);
            cmbAuthorizedCreator.Name = "cmbAuthorizedCreator";
            cmbAuthorizedCreator.Size = new Size(233, 23);
            cmbAuthorizedCreator.TabIndex = 1;
            // 
            // lblAuthorizedCreator
            // 
            lblAuthorizedCreator.AutoSize = true;
            lblAuthorizedCreator.Location = new Point(18, 35);
            lblAuthorizedCreator.Margin = new Padding(4, 0, 4, 0);
            lblAuthorizedCreator.Name = "lblAuthorizedCreator";
            lblAuthorizedCreator.Size = new Size(111, 15);
            lblAuthorizedCreator.TabIndex = 0;
            lblAuthorizedCreator.Text = "Authorized Creator:";
            // 
            // grpItemStructure
            // 
            grpItemStructure.Location = new Point(0, 0);
            grpItemStructure.Name = "grpItemStructure";
            grpItemStructure.Size = new Size(200, 100);
            grpItemStructure.TabIndex = 0;
            grpItemStructure.TabStop = false;
            // 
            // grpLocation
            // 
            grpLocation.Controls.Add(txtBinNumber);
            grpLocation.Controls.Add(lblBinNumber);
            grpLocation.Controls.Add(txtStoreLocation);
            grpLocation.Controls.Add(lblStoreLocation);
            grpLocation.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            grpLocation.Location = new Point(595, 294);
            grpLocation.Margin = new Padding(4, 3, 4, 3);
            grpLocation.Name = "grpLocation";
            grpLocation.Padding = new Padding(4, 3, 4, 3);
            grpLocation.Size = new Size(537, 127);
            grpLocation.TabIndex = 4;
            grpLocation.TabStop = false;
            grpLocation.Text = "Step 4: Store Location (Upon GIN)";
            // 
            // txtBinNumber
            // 
            txtBinNumber.Location = new Point(175, 65);
            txtBinNumber.Margin = new Padding(4, 3, 4, 3);
            txtBinNumber.Name = "txtBinNumber";
            txtBinNumber.Size = new Size(174, 21);
            txtBinNumber.TabIndex = 3;
            // 
            // lblBinNumber
            // 
            lblBinNumber.AutoSize = true;
            lblBinNumber.Location = new Point(18, 69);
            lblBinNumber.Margin = new Padding(4, 0, 4, 0);
            lblBinNumber.Name = "lblBinNumber";
            lblBinNumber.Size = new Size(76, 15);
            lblBinNumber.TabIndex = 2;
            lblBinNumber.Text = "Bin Number:";
            // 
            // txtStoreLocation
            // 
            txtStoreLocation.Location = new Point(175, 30);
            txtStoreLocation.Margin = new Padding(4, 3, 4, 3);
            txtStoreLocation.Name = "txtStoreLocation";
            txtStoreLocation.Size = new Size(233, 21);
            txtStoreLocation.TabIndex = 1;
            // 
            // lblStoreLocation
            // 
            lblStoreLocation.AutoSize = true;
            lblStoreLocation.Location = new Point(18, 35);
            lblStoreLocation.Margin = new Padding(4, 0, 4, 0);
            lblStoreLocation.Name = "lblStoreLocation";
            lblStoreLocation.Size = new Size(89, 15);
            lblStoreLocation.TabIndex = 0;
            lblStoreLocation.Text = "Store Location:";
            // 
            // grpEmergency
            // 
            grpEmergency.Controls.Add(cmbEmergencyApproval);
            grpEmergency.Controls.Add(lblEmergencyApproval);
            grpEmergency.Controls.Add(chkEmergency);
            grpEmergency.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            grpEmergency.Location = new Point(595, 433);
            grpEmergency.Margin = new Padding(4, 3, 4, 3);
            grpEmergency.Name = "grpEmergency";
            grpEmergency.Padding = new Padding(4, 3, 4, 3);
            grpEmergency.Size = new Size(537, 127);
            grpEmergency.TabIndex = 5;
            grpEmergency.TabStop = false;
            grpEmergency.Text = "Exceptions & Escalation";
            // 
            // cmbEmergencyApproval
            // 
            cmbEmergencyApproval.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbEmergencyApproval.Enabled = false;
            cmbEmergencyApproval.FormattingEnabled = true;
            cmbEmergencyApproval.Location = new Point(175, 59);
            cmbEmergencyApproval.Margin = new Padding(4, 3, 4, 3);
            cmbEmergencyApproval.Name = "cmbEmergencyApproval";
            cmbEmergencyApproval.Size = new Size(233, 23);
            cmbEmergencyApproval.TabIndex = 2;
            // 
            // lblEmergencyApproval
            // 
            lblEmergencyApproval.AutoSize = true;
            lblEmergencyApproval.Location = new Point(18, 63);
            lblEmergencyApproval.Margin = new Padding(4, 0, 4, 0);
            lblEmergencyApproval.Name = "lblEmergencyApproval";
            lblEmergencyApproval.Size = new Size(122, 15);
            lblEmergencyApproval.TabIndex = 1;
            lblEmergencyApproval.Text = "Emergency Approval:";
            // 
            // chkEmergency
            // 
            chkEmergency.AutoSize = true;
            chkEmergency.Location = new Point(21, 29);
            chkEmergency.Margin = new Padding(4, 3, 4, 3);
            chkEmergency.Name = "chkEmergency";
            chkEmergency.Size = new Size(218, 19);
            chkEmergency.TabIndex = 0;
            chkEmergency.Text = "Emergency Item Creation Required";
            chkEmergency.UseVisualStyleBackColor = true;
            chkEmergency.CheckedChanged += chkEmergency_CheckedChanged;
            // 
            // btnSubmit
            // 
            btnSubmit.BackColor = Color.LightGreen;
            btnSubmit.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnSubmit.Location = new Point(14, 658);
            btnSubmit.Margin = new Padding(4, 3, 4, 3);
            btnSubmit.Name = "btnSubmit";
            btnSubmit.Size = new Size(140, 40);
            btnSubmit.TabIndex = 6;
            btnSubmit.Text = "Submit";
            btnSubmit.UseVisualStyleBackColor = false;
            btnSubmit.Click += btnSubmit_Click;
            // 
            // btnClear
            // 
            btnClear.BackColor = Color.LightYellow;
            btnClear.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnClear.Location = new Point(175, 658);
            btnClear.Margin = new Padding(4, 3, 4, 3);
            btnClear.Name = "btnClear";
            btnClear.Size = new Size(140, 40);
            btnClear.TabIndex = 7;
            btnClear.Text = "Clear";
            btnClear.UseVisualStyleBackColor = false;
            btnClear.Click += btnClear_Click;
            // 
            // btnExit
            // 
            btnExit.BackColor = Color.LightCoral;
            btnExit.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnExit.Location = new Point(336, 658);
            btnExit.Margin = new Padding(4, 3, 4, 3);
            btnExit.Name = "btnExit";
            btnExit.Size = new Size(140, 40);
            btnExit.TabIndex = 8;
            btnExit.Text = "Exit";
            btnExit.UseVisualStyleBackColor = false;
            btnExit.Click += btnExit_Click;
            // 
            // btnPrint
            // 
            btnPrint.BackColor = Color.LightSteelBlue;
            btnPrint.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnPrint.Location = new Point(497, 658);
            btnPrint.Margin = new Padding(4, 3, 4, 3);
            btnPrint.Name = "btnPrint";
            btnPrint.Size = new Size(140, 40);
            btnPrint.TabIndex = 9;
            btnPrint.Text = "Print";
            btnPrint.UseVisualStyleBackColor = false;
            btnPrint.Click += btnPrint_Click;
            // 
            // lblPreparedBy
            // 
            lblPreparedBy.AutoSize = true;
            lblPreparedBy.Font = new Font("Microsoft Sans Serif", 8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblPreparedBy.Location = new Point(665, 658);
            lblPreparedBy.Margin = new Padding(4, 0, 4, 0);
            lblPreparedBy.Name = "lblPreparedBy";
            lblPreparedBy.Size = new Size(68, 13);
            lblPreparedBy.TabIndex = 10;
            lblPreparedBy.Text = "Prepared By:";
            // 
            // txtPreparedBy
            // 
            txtPreparedBy.Font = new Font("Microsoft Sans Serif", 8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtPreparedBy.Location = new Point(750, 654);
            txtPreparedBy.Margin = new Padding(4, 3, 4, 3);
            txtPreparedBy.Name = "txtPreparedBy";
            txtPreparedBy.ReadOnly = true;
            txtPreparedBy.Size = new Size(139, 20);
            txtPreparedBy.TabIndex = 11;
            txtPreparedBy.Text = "Material Manager";
            // 
            // lblApprovedBy
            // 
            lblApprovedBy.AutoSize = true;
            lblApprovedBy.Font = new Font("Microsoft Sans Serif", 8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblApprovedBy.Location = new Point(665, 684);
            lblApprovedBy.Margin = new Padding(4, 0, 4, 0);
            lblApprovedBy.Name = "lblApprovedBy";
            lblApprovedBy.Size = new Size(71, 13);
            lblApprovedBy.TabIndex = 12;
            lblApprovedBy.Text = "Approved By:";
            // 
            // txtApprovedBy
            // 
            txtApprovedBy.Font = new Font("Microsoft Sans Serif", 8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtApprovedBy.Location = new Point(750, 681);
            txtApprovedBy.Margin = new Padding(4, 3, 4, 3);
            txtApprovedBy.Name = "txtApprovedBy";
            txtApprovedBy.ReadOnly = true;
            txtApprovedBy.Size = new Size(139, 20);
            txtApprovedBy.TabIndex = 13;
            txtApprovedBy.Text = "Operations Head";
            // 
            // lblEffectiveDate
            // 
            lblEffectiveDate.AutoSize = true;
            lblEffectiveDate.Font = new Font("Microsoft Sans Serif", 8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblEffectiveDate.Location = new Point(665, 711);
            lblEffectiveDate.Margin = new Padding(4, 0, 4, 0);
            lblEffectiveDate.Name = "lblEffectiveDate";
            lblEffectiveDate.Size = new Size(78, 13);
            lblEffectiveDate.TabIndex = 14;
            lblEffectiveDate.Text = "Effective Date:";
            // 
            // dtpEffectiveDate
            // 
            dtpEffectiveDate.Font = new Font("Microsoft Sans Serif", 8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            dtpEffectiveDate.Format = DateTimePickerFormat.Short;
            dtpEffectiveDate.Location = new Point(750, 707);
            dtpEffectiveDate.Margin = new Padding(4, 3, 4, 3);
            dtpEffectiveDate.Name = "dtpEffectiveDate";
            dtpEffectiveDate.Size = new Size(139, 20);
            dtpEffectiveDate.TabIndex = 15;
            // 
            // frmItemCodeCreation
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.WhiteSmoke;
            ClientSize = new Size(1148, 740);
            Controls.Add(dtpEffectiveDate);
            Controls.Add(lblEffectiveDate);
            Controls.Add(txtApprovedBy);
            Controls.Add(lblApprovedBy);
            Controls.Add(txtPreparedBy);
            Controls.Add(lblPreparedBy);
            Controls.Add(btnPrint);
            Controls.Add(btnExit);
            Controls.Add(btnClear);
            Controls.Add(btnSubmit);
            Controls.Add(grpEmergency);
            Controls.Add(grpLocation);
            Controls.Add(grpApproval);
            Controls.Add(grpItemCodeStructure);
            Controls.Add(grpRequestDetails);
            Controls.Add(lblTitle);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Margin = new Padding(4, 3, 4, 3);
            MaximizeBox = false;
            Name = "frmItemCodeCreation";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Item Code Creation - ERP";
            Load += frmItemCodeCreation_Load;
            grpRequestDetails.ResumeLayout(false);
            grpRequestDetails.PerformLayout();
            grpItemCodeStructure.ResumeLayout(false);
            grpItemCodeStructure.PerformLayout();
            grpApproval.ResumeLayout(false);
            grpApproval.PerformLayout();
            grpLocation.ResumeLayout(false);
            grpLocation.PerformLayout();
            grpEmergency.ResumeLayout(false);
            grpEmergency.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.GroupBox grpRequestDetails;
        private System.Windows.Forms.Label lblRequestDate;
        private System.Windows.Forms.DateTimePicker dtpRequestDate;
        private System.Windows.Forms.Label lblRequestor;
        private System.Windows.Forms.TextBox txtRequestor;
        private System.Windows.Forms.Label lblDepartment;
        private System.Windows.Forms.ComboBox cmbDepartment;
        private System.Windows.Forms.Label lblItemCategory;
        private System.Windows.Forms.ComboBox cmbItemCategory;
        private System.Windows.Forms.Label lblItemDescription;
        private System.Windows.Forms.TextBox txtItemDescription;
        private System.Windows.Forms.Label lblTechnicalSpec;
        private System.Windows.Forms.TextBox txtTechnicalSpec;
        private System.Windows.Forms.Label lblUnitOfMeasure;
        private System.Windows.Forms.ComboBox cmbUnitOfMeasure;
        private System.Windows.Forms.Label lblDrawingReference;
        private System.Windows.Forms.TextBox txtDrawingReference;
        private System.Windows.Forms.Label lblCriticalityLevel;
        private System.Windows.Forms.ComboBox cmbCriticalityLevel;
        private System.Windows.Forms.Label lblHSNCode;
        private System.Windows.Forms.TextBox txtHSNCode;
        private System.Windows.Forms.GroupBox grpItemCodeStructure;
        private System.Windows.Forms.Label lblPrefix;
        private System.Windows.Forms.TextBox txtPrefix;
        private System.Windows.Forms.Label lblCategoryCode;
        private System.Windows.Forms.TextBox txtCategoryCode;
        private System.Windows.Forms.Label lblSequentialNumber;
        private System.Windows.Forms.TextBox txtSequentialNumber;
        private System.Windows.Forms.Label lblGeneratedItemCode;
        private System.Windows.Forms.TextBox txtGeneratedItemCode;
        private System.Windows.Forms.Button btnGenerateCode;
        private System.Windows.Forms.GroupBox grpApproval;//
        private System.Windows.Forms.GroupBox grpItemStructure;//grpItemStructure
        private System.Windows.Forms.Label lblAuthorizedCreator;
        private System.Windows.Forms.ComboBox cmbAuthorizedCreator;
        private System.Windows.Forms.Label lblApprovalAuthority;
        private System.Windows.Forms.ComboBox cmbApprovalAuthority;
        private System.Windows.Forms.Label lblApprovalStatus;
        private System.Windows.Forms.ComboBox cmbApprovalStatus;
        private System.Windows.Forms.Label lblApprovalRemarks;
        private System.Windows.Forms.TextBox txtApprovalRemarks;
        private System.Windows.Forms.GroupBox grpLocation;
        private System.Windows.Forms.Label lblStoreLocation;
        private System.Windows.Forms.TextBox txtStoreLocation;
        private System.Windows.Forms.Label lblBinNumber;
        private System.Windows.Forms.TextBox txtBinNumber;
        private System.Windows.Forms.GroupBox grpEmergency;
        private System.Windows.Forms.CheckBox chkEmergency;
        private System.Windows.Forms.Label lblEmergencyApproval;
        private System.Windows.Forms.ComboBox cmbEmergencyApproval;
        private System.Windows.Forms.Button btnSubmit;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Button btnPrint;
        private System.Windows.Forms.Label lblPreparedBy;
        private System.Windows.Forms.TextBox txtPreparedBy;
        private System.Windows.Forms.Label lblApprovedBy;
        private System.Windows.Forms.TextBox txtApprovedBy;
        private System.Windows.Forms.Label lblEffectiveDate;
        private System.Windows.Forms.DateTimePicker dtpEffectiveDate;
    }
}