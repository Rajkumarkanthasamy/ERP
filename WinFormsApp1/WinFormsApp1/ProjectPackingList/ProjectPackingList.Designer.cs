namespace Erp_Project_With_Buttons.Project_Master
{
    partial class ProjectPackingList
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ProjectPackingList));

            this.pnlTop = new System.Windows.Forms.Panel();
            this.lblProject = new System.Windows.Forms.Label();
            this.projectName = new System.Windows.Forms.Label();
            this.cmbProject = new System.Windows.Forms.ComboBox();
            this.btnLoad = new System.Windows.Forms.Button();
            this.lblPackingNo = new System.Windows.Forms.Label();
            this.txtPackingNo = new System.Windows.Forms.TextBox();
            this.lblDate = new System.Windows.Forms.Label();
            this.dtpDate = new System.Windows.Forms.DateTimePicker();
            this.lblversion = new System.Windows.Forms.Label();
            this.PackingVersion = new System.Windows.Forms.Label();

            this.tblMain = new System.Windows.Forms.TableLayoutPanel();
            this.grpBOM = new System.Windows.Forms.GroupBox();
            this.pnlBomTools = new System.Windows.Forms.Panel();
            this.lblBomSearch = new System.Windows.Forms.Label();
            this.txtBomSearch = new System.Windows.Forms.TextBox();
            this.btnManualEntry = new System.Windows.Forms.Button();
            this.btnSelectAllBom = new System.Windows.Forms.Button();
            this.btnClearBomSelection = new System.Windows.Forms.Button();
            this.tvBOM = new System.Windows.Forms.TreeView();
            this.lblBOMCount = new System.Windows.Forms.Label();

            this.pnlButtons = new System.Windows.Forms.Panel();
            this.btnAddBox = new System.Windows.Forms.Button();
            this.btnAddPallet = new System.Windows.Forms.Button();
            this.btnRemoveBox = new System.Windows.Forms.Button();
            this.btnMoveToBox = new System.Windows.Forms.Button();
            this.btnMoveToList = new System.Windows.Forms.Button();
            this.btnMarkNonBox = new System.Windows.Forms.Button();

            this.splitRight = new System.Windows.Forms.SplitContainer();
            this.grpBoxes = new System.Windows.Forms.GroupBox();
            this.splitBoxes = new System.Windows.Forms.SplitContainer();
            this.lstBoxes = new System.Windows.Forms.ListBox();
            this.pnlBoxDetail = new System.Windows.Forms.Panel();
            this.grpBoxInfo = new System.Windows.Forms.GroupBox();
            this.lblBoxLabel = new System.Windows.Forms.Label();
            this.txtBoxLabel = new System.Windows.Forms.TextBox();
            this.lblGW = new System.Windows.Forms.Label();
            this.txtGW = new System.Windows.Forms.TextBox();
            this.lblNW = new System.Windows.Forms.Label();
            this.txtNW = new System.Windows.Forms.TextBox();
            this.lblDims = new System.Windows.Forms.Label();
            this.txtLength = new System.Windows.Forms.TextBox();
            this.lblDimsX1 = new System.Windows.Forms.Label();
            this.txtWidth = new System.Windows.Forms.TextBox();
            this.lblDimsX2 = new System.Windows.Forms.Label();
            this.txtHeight = new System.Windows.Forms.TextBox();
            this.lblDimsUnit = new System.Windows.Forms.Label();
            this.cmbDimUnit = new System.Windows.Forms.ComboBox();
            this.btnApplyBoxInfo = new System.Windows.Forms.Button();
            this.lstBoxItems = new System.Windows.Forms.ListBox();
            this.lblBoxItemCount = new System.Windows.Forms.Label();
            this.grpNonBox = new System.Windows.Forms.GroupBox();
            this.lstNonBox = new System.Windows.Forms.ListBox();

            this.pnlBottom = new System.Windows.Forms.Panel();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnViewPrint = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();

            this.ctxBox = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuRenameBox = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuDeleteBox = new System.Windows.Forms.ToolStripMenuItem();

            this.pnlTop.SuspendLayout();
            this.tblMain.SuspendLayout();
            this.grpBOM.SuspendLayout();
            this.pnlBomTools.SuspendLayout();
            this.pnlButtons.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitRight)).BeginInit();
            this.splitRight.Panel1.SuspendLayout();
            this.splitRight.Panel2.SuspendLayout();
            this.splitRight.SuspendLayout();
            this.grpBoxes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitBoxes)).BeginInit();
            this.splitBoxes.Panel1.SuspendLayout();
            this.splitBoxes.Panel2.SuspendLayout();
            this.splitBoxes.SuspendLayout();
            this.pnlBoxDetail.SuspendLayout();
            this.grpBoxInfo.SuspendLayout();
            this.grpNonBox.SuspendLayout();
            this.pnlBottom.SuspendLayout();
            this.ctxBox.SuspendLayout();
            this.SuspendLayout();

            // pnlTop
            this.pnlTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTop.Height = 100;
            this.pnlTop.BackColor = System.Drawing.Color.FromArgb(235, 240, 250);

            this.lblProject.AutoSize = true;
            this.lblProject.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblProject.Location = new System.Drawing.Point(8, 14);
            this.lblProject.Text = "Project :";
            this.lblProject.Size = new System.Drawing.Size(59, 15);

            this.projectName.AutoSize = true;
            this.projectName.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.projectName.Location = new System.Drawing.Point(8, 38);
            this.projectName.Text = "projectName :";
            this.projectName.Size = new System.Drawing.Size(59, 15);

            this.cmbProject.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
            this.cmbProject.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.cmbProject.Location = new System.Drawing.Point(72, 11);
            this.cmbProject.Size = new System.Drawing.Size(260, 23);
            this.cmbProject.TabIndex = 0;
            this.cmbProject.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbProject.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;

            this.btnLoad.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.btnLoad.Location = new System.Drawing.Point(340, 10);
            this.btnLoad.Size = new System.Drawing.Size(90, 26);
            this.btnLoad.Text = "Load BOM";
            this.btnLoad.BackColor = System.Drawing.Color.SteelBlue;
            this.btnLoad.ForeColor = System.Drawing.Color.White;
            this.btnLoad.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLoad.TabIndex = 1;
            this.btnLoad.UseVisualStyleBackColor = false;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);

            this.lblPackingNo.AutoSize = true;
            this.lblPackingNo.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblPackingNo.Location = new System.Drawing.Point(445, 14);
            this.lblPackingNo.Text = "Packing No :";
            this.lblPackingNo.Size = new System.Drawing.Size(83, 15);

            this.txtPackingNo.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtPackingNo.Location = new System.Drawing.Point(534, 11);
            this.txtPackingNo.Size = new System.Drawing.Size(160, 23);
            this.txtPackingNo.TabIndex = 2;

            this.lblDate.AutoSize = true;
            this.lblDate.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblDate.Location = new System.Drawing.Point(710, 14);
            this.lblDate.Text = "Date :";
            this.lblDate.Size = new System.Drawing.Size(38, 15);

            this.dtpDate.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.dtpDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpDate.Location = new System.Drawing.Point(752, 11);
            this.dtpDate.Size = new System.Drawing.Size(120, 23);
            this.dtpDate.TabIndex = 3;

            this.lblversion.AutoSize = true;
            this.lblversion.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblversion.Location = new System.Drawing.Point(8, 38);
            this.lblversion.Text = "Version :";
            this.lblversion.Size = new System.Drawing.Size(38, 15);

            this.PackingVersion.AutoSize = true;
            this.PackingVersion.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.PackingVersion.Location = new System.Drawing.Point(70, 38);
            this.PackingVersion.Text = "-";
            this.PackingVersion.Size = new System.Drawing.Size(38, 15);
            this.PackingVersion.TabIndex = 4;

            this.pnlTop.Controls.Add(this.lblProject);
            this.pnlTop.Controls.Add(this.cmbProject);
            this.pnlTop.Controls.Add(this.btnLoad);
            this.pnlTop.Controls.Add(this.lblPackingNo);
            this.pnlTop.Controls.Add(this.txtPackingNo);
            this.pnlTop.Controls.Add(this.lblDate);
            this.pnlTop.Controls.Add(this.dtpDate);
            this.pnlTop.Controls.Add(this.lblversion);
            this.pnlTop.Controls.Add(this.PackingVersion);

            // tblMain
            this.tblMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblMain.ColumnCount = 3;
            this.tblMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 340F));
            this.tblMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 108F));
            this.tblMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblMain.RowCount = 1;
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblMain.Controls.Add(this.grpBOM, 0, 0);
            this.tblMain.Controls.Add(this.pnlButtons, 1, 0);
            this.tblMain.Controls.Add(this.splitRight, 2, 0);
            this.tblMain.Padding = new System.Windows.Forms.Padding(4);
            this.tblMain.TabIndex = 10;

            // grpBOM
            this.grpBOM.Text = "BOM Products / Items  (Unassigned)";
            this.grpBOM.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.grpBOM.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpBOM.Padding = new System.Windows.Forms.Padding(4);
            this.grpBOM.TabIndex = 0;

            this.pnlBomTools.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlBomTools.Height = 82;
            this.pnlBomTools.TabIndex = 2;

            this.lblBomSearch.AutoSize = true;
            this.lblBomSearch.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.lblBomSearch.Location = new System.Drawing.Point(4, 6);
            this.lblBomSearch.Text = "Search :";

            this.txtBomSearch.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.txtBomSearch.Location = new System.Drawing.Point(55, 3);
            this.txtBomSearch.Size = new System.Drawing.Size(260, 22);
            this.txtBomSearch.TabIndex = 0;
            this.txtBomSearch.TextChanged += new System.EventHandler(this.txtBomSearch_TextChanged);

            this.btnManualEntry.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this.btnManualEntry.Location = new System.Drawing.Point(55, 28);
            this.btnManualEntry.Size = new System.Drawing.Size(120, 24);
            this.btnManualEntry.Text = "+ Manual Entry";
            this.btnManualEntry.BackColor = System.Drawing.Color.DarkSlateGray;
            this.btnManualEntry.ForeColor = System.Drawing.Color.White;
            this.btnManualEntry.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnManualEntry.UseVisualStyleBackColor = false;
            this.btnManualEntry.Click += new System.EventHandler(this.btnManualEntry_Click);

            this.btnSelectAllBom.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this.btnSelectAllBom.Location = new System.Drawing.Point(55, 54);
            this.btnSelectAllBom.Size = new System.Drawing.Size(90, 24);
            this.btnSelectAllBom.Text = "Select All";
            this.btnSelectAllBom.BackColor = System.Drawing.Color.SteelBlue;
            this.btnSelectAllBom.ForeColor = System.Drawing.Color.White;
            this.btnSelectAllBom.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSelectAllBom.UseVisualStyleBackColor = false;
            this.btnSelectAllBom.Click += new System.EventHandler(this.btnSelectAllBom_Click);

            this.btnClearBomSelection.Font = new System.Drawing.Font("Segoe UI", 8F);
            this.btnClearBomSelection.Location = new System.Drawing.Point(151, 54);
            this.btnClearBomSelection.Size = new System.Drawing.Size(90, 24);
            this.btnClearBomSelection.Text = "Clear Sel";
            this.btnClearBomSelection.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClearBomSelection.Click += new System.EventHandler(this.btnClearBomSelection_Click);

            this.pnlBomTools.Controls.Add(this.lblBomSearch);
            this.pnlBomTools.Controls.Add(this.txtBomSearch);
            this.pnlBomTools.Controls.Add(this.btnManualEntry);
            this.pnlBomTools.Controls.Add(this.btnSelectAllBom);
            this.pnlBomTools.Controls.Add(this.btnClearBomSelection);

            this.tvBOM.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvBOM.Font = new System.Drawing.Font("Consolas", 8.5F);
            this.tvBOM.AllowDrop = true;
            this.tvBOM.CheckBoxes = true;
            this.tvBOM.HideSelection = false;
            this.tvBOM.FullRowSelect = true;
            this.tvBOM.ShowNodeToolTips = true;
            this.tvBOM.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tvBOM.TabIndex = 0;
            this.tvBOM.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.tvBOM_ItemDrag);
            this.tvBOM.DragOver += new System.Windows.Forms.DragEventHandler(this.tvBOM_DragOver);
            this.tvBOM.DragDrop += new System.Windows.Forms.DragEventHandler(this.tvBOM_DragDrop);
            this.tvBOM.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.tvBOM_AfterCheck);
            this.tvBOM.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.tvBOM_NodeMouseDoubleClick);

            this.lblBOMCount.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblBOMCount.Height = 18;
            this.lblBOMCount.Font = new System.Drawing.Font("Segoe UI", 7.5F);
            this.lblBOMCount.ForeColor = System.Drawing.Color.DimGray;
            this.lblBOMCount.Text = "0 items";
            this.lblBOMCount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblBOMCount.TabIndex = 1;

            this.grpBOM.Controls.Add(this.tvBOM);
            this.grpBOM.Controls.Add(this.lblBOMCount);
            this.grpBOM.Controls.Add(this.pnlBomTools);

            // pnlButtons
            this.pnlButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlButtons.TabIndex = 1;

            int bx = 6, bw = 96, bh = 28, by = 40, gap = 8;

            this.btnAddBox.SetBounds(bx, by, bw, bh); by += bh + gap;
            this.btnAddBox.Text = "+ Add Box";
            this.btnAddBox.BackColor = System.Drawing.Color.FromArgb(34, 139, 34);
            this.btnAddBox.ForeColor = System.Drawing.Color.White;
            this.btnAddBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddBox.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.btnAddBox.TabIndex = 0;
            this.btnAddBox.UseVisualStyleBackColor = false;
            this.btnAddBox.Click += new System.EventHandler(this.btnAddBox_Click);

            this.btnAddPallet.SetBounds(bx, by, bw, bh); by += bh + gap;
            this.btnAddPallet.Text = "+ Add Pallet";
            this.btnAddPallet.BackColor = System.Drawing.Color.Teal;
            this.btnAddPallet.ForeColor = System.Drawing.Color.White;
            this.btnAddPallet.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAddPallet.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this.btnAddPallet.TabIndex = 5;
            this.btnAddPallet.UseVisualStyleBackColor = false;
            this.btnAddPallet.Click += new System.EventHandler(this.btnAddPallet_Click);

            this.btnRemoveBox.SetBounds(bx, by, bw, bh); by += bh + gap * 2;
            this.btnRemoveBox.Text = "- Remove";
            this.btnRemoveBox.BackColor = System.Drawing.Color.Firebrick;
            this.btnRemoveBox.ForeColor = System.Drawing.Color.White;
            this.btnRemoveBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRemoveBox.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.btnRemoveBox.TabIndex = 1;
            this.btnRemoveBox.UseVisualStyleBackColor = false;
            this.btnRemoveBox.Click += new System.EventHandler(this.btnRemoveBox_Click);

            this.btnMoveToBox.SetBounds(bx, by, bw, bh); by += bh + gap;
            this.btnMoveToBox.Text = "> To Box";
            this.btnMoveToBox.BackColor = System.Drawing.Color.SteelBlue;
            this.btnMoveToBox.ForeColor = System.Drawing.Color.White;
            this.btnMoveToBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMoveToBox.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.btnMoveToBox.TabIndex = 2;
            this.btnMoveToBox.UseVisualStyleBackColor = false;
            this.btnMoveToBox.Click += new System.EventHandler(this.btnMoveToBox_Click);

            this.btnMoveToList.SetBounds(bx, by, bw, bh); by += bh + gap;
            this.btnMoveToList.Text = "< To List";
            this.btnMoveToList.BackColor = System.Drawing.Color.DarkOrange;
            this.btnMoveToList.ForeColor = System.Drawing.Color.White;
            this.btnMoveToList.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMoveToList.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.btnMoveToList.TabIndex = 3;
            this.btnMoveToList.UseVisualStyleBackColor = false;
            this.btnMoveToList.Click += new System.EventHandler(this.btnMoveToList_Click);

            this.btnMarkNonBox.SetBounds(bx, by, bw, bh);
            this.btnMarkNonBox.Text = "Non-Box";
            this.btnMarkNonBox.BackColor = System.Drawing.Color.SlateGray;
            this.btnMarkNonBox.ForeColor = System.Drawing.Color.White;
            this.btnMarkNonBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMarkNonBox.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.btnMarkNonBox.TabIndex = 4;
            this.btnMarkNonBox.UseVisualStyleBackColor = false;
            this.btnMarkNonBox.Click += new System.EventHandler(this.btnMarkNonBox_Click);

            this.pnlButtons.Controls.Add(this.btnAddBox);
            this.pnlButtons.Controls.Add(this.btnAddPallet);
            this.pnlButtons.Controls.Add(this.btnRemoveBox);
            this.pnlButtons.Controls.Add(this.btnMoveToBox);
            this.pnlButtons.Controls.Add(this.btnMoveToList);
            this.pnlButtons.Controls.Add(this.btnMarkNonBox);

            // splitRight
            this.splitRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitRight.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.splitRight.SplitterDistance = 420;
            this.splitRight.TabIndex = 2;
            this.splitRight.Panel1.Controls.Add(this.grpBoxes);
            this.splitRight.Panel2.Controls.Add(this.grpNonBox);

            // grpBoxes
            this.grpBoxes.Text = "Boxes / Pallets";
            this.grpBoxes.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.grpBoxes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpBoxes.Padding = new System.Windows.Forms.Padding(4);
            this.grpBoxes.TabIndex = 0;

            this.splitBoxes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitBoxes.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.splitBoxes.SplitterDistance = 90;
            this.splitBoxes.TabIndex = 0;
            this.splitBoxes.Panel1.Controls.Add(this.lstBoxes);
            this.splitBoxes.Panel2.Controls.Add(this.pnlBoxDetail);

            this.lstBoxes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstBoxes.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lstBoxes.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lstBoxes.ItemHeight = 18;
            this.lstBoxes.TabIndex = 0;
            this.lstBoxes.ContextMenuStrip = this.ctxBox;
            this.lstBoxes.AllowDrop = true;
            this.lstBoxes.SelectedIndexChanged += new System.EventHandler(this.lstBoxes_SelectedIndexChanged);
            this.lstBoxes.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lstBoxes_MouseDown);
            this.lstBoxes.DragOver += new System.Windows.Forms.DragEventHandler(this.lstBoxes_DragOver);
            this.lstBoxes.DragDrop += new System.Windows.Forms.DragEventHandler(this.lstBoxes_DragDrop);

            this.grpBoxes.Controls.Add(this.splitBoxes);

            // pnlBoxDetail
            this.pnlBoxDetail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlBoxDetail.TabIndex = 0;

            this.grpBoxInfo.Text = "Box Details";
            this.grpBoxInfo.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.grpBoxInfo.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpBoxInfo.Height = 180;
            this.grpBoxInfo.TabIndex = 0;

            this.lblBoxLabel.AutoSize = true;
            this.lblBoxLabel.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblBoxLabel.Location = new System.Drawing.Point(8, 22);
            this.lblBoxLabel.Size = new System.Drawing.Size(60, 15);
            this.lblBoxLabel.Text = "Box Label :";
            this.lblBoxLabel.TabIndex = 0;

            this.txtBoxLabel.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.txtBoxLabel.Location = new System.Drawing.Point(86, 20);
            this.txtBoxLabel.Size = new System.Drawing.Size(150, 22);
            this.txtBoxLabel.TabIndex = 1;

            this.lblGW.AutoSize = true;
            this.lblGW.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblGW.Location = new System.Drawing.Point(8, 50);
            this.lblGW.Size = new System.Drawing.Size(60, 15);
            this.lblGW.Text = "GW (kg) :";
            this.lblGW.TabIndex = 2;

            this.txtGW.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.txtGW.Location = new System.Drawing.Point(86, 48);
            this.txtGW.Size = new System.Drawing.Size(80, 22);
            this.txtGW.TabIndex = 3;

            this.lblNW.AutoSize = true;
            this.lblNW.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblNW.Location = new System.Drawing.Point(8, 78);
            this.lblNW.Size = new System.Drawing.Size(60, 15);
            this.lblNW.Text = "NW (kg) :";
            this.lblNW.TabIndex = 4;

            this.txtNW.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.txtNW.Location = new System.Drawing.Point(86, 76);
            this.txtNW.Size = new System.Drawing.Size(80, 22);
            this.txtNW.TabIndex = 5;

            this.lblDims.AutoSize = true;
            this.lblDims.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblDims.Location = new System.Drawing.Point(8, 106);
            this.lblDims.Size = new System.Drawing.Size(72, 15);
            this.lblDims.Text = "Dims (L x W x H) :";
            this.lblDims.TabIndex = 6;

            this.txtLength.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.txtLength.Location = new System.Drawing.Point(130, 104);
            this.txtLength.Size = new System.Drawing.Size(44, 22);
            this.txtLength.TabIndex = 7;
            this.txtLength.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;

            this.lblDimsX1.AutoSize = true;
            this.lblDimsX1.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.lblDimsX1.Location = new System.Drawing.Point(178, 107);
            this.lblDimsX1.Text = "x";
            this.lblDimsX1.TabIndex = 8;

            this.txtWidth.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.txtWidth.Location = new System.Drawing.Point(190, 104);
            this.txtWidth.Size = new System.Drawing.Size(44, 22);
            this.txtWidth.TabIndex = 9;
            this.txtWidth.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;

            this.lblDimsX2.AutoSize = true;
            this.lblDimsX2.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.lblDimsX2.Location = new System.Drawing.Point(238, 107);
            this.lblDimsX2.Text = "x";
            this.lblDimsX2.TabIndex = 10;

            this.txtHeight.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.txtHeight.Location = new System.Drawing.Point(250, 104);
            this.txtHeight.Size = new System.Drawing.Size(44, 22);
            this.txtHeight.TabIndex = 11;
            this.txtHeight.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;

            this.lblDimsUnit.AutoSize = true;
            this.lblDimsUnit.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblDimsUnit.Location = new System.Drawing.Point(8, 134);
            this.lblDimsUnit.Text = "Unit :";
            this.lblDimsUnit.TabIndex = 12;

            this.cmbDimUnit.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDimUnit.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.cmbDimUnit.Location = new System.Drawing.Point(86, 131);
            this.cmbDimUnit.Size = new System.Drawing.Size(100, 23);
            this.cmbDimUnit.TabIndex = 13;

            this.btnApplyBoxInfo.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.btnApplyBoxInfo.Location = new System.Drawing.Point(200, 128);
            this.btnApplyBoxInfo.Size = new System.Drawing.Size(80, 27);
            this.btnApplyBoxInfo.Text = "Apply";
            this.btnApplyBoxInfo.BackColor = System.Drawing.Color.FromArgb(34, 139, 34);
            this.btnApplyBoxInfo.ForeColor = System.Drawing.Color.White;
            this.btnApplyBoxInfo.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnApplyBoxInfo.TabIndex = 14;
            this.btnApplyBoxInfo.UseVisualStyleBackColor = false;
            this.btnApplyBoxInfo.Click += new System.EventHandler(this.btnApplyBoxInfo_Click);

            this.grpBoxInfo.Controls.Add(this.lblBoxLabel);
            this.grpBoxInfo.Controls.Add(this.txtBoxLabel);
            this.grpBoxInfo.Controls.Add(this.lblGW);
            this.grpBoxInfo.Controls.Add(this.txtGW);
            this.grpBoxInfo.Controls.Add(this.lblNW);
            this.grpBoxInfo.Controls.Add(this.txtNW);
            this.grpBoxInfo.Controls.Add(this.lblDims);
            this.grpBoxInfo.Controls.Add(this.txtLength);
            this.grpBoxInfo.Controls.Add(this.lblDimsX1);
            this.grpBoxInfo.Controls.Add(this.txtWidth);
            this.grpBoxInfo.Controls.Add(this.lblDimsX2);
            this.grpBoxInfo.Controls.Add(this.txtHeight);
            this.grpBoxInfo.Controls.Add(this.lblDimsUnit);
            this.grpBoxInfo.Controls.Add(this.cmbDimUnit);
            this.grpBoxInfo.Controls.Add(this.btnApplyBoxInfo);

            this.lstBoxItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstBoxItems.Font = new System.Drawing.Font("Consolas", 8.5F);
            this.lstBoxItems.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstBoxItems.AllowDrop = true;
            this.lstBoxItems.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lstBoxItems.ItemHeight = 17;
            this.lstBoxItems.TabIndex = 1;
            this.lstBoxItems.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lstBoxItems_MouseDown);
            this.lstBoxItems.DragOver += new System.Windows.Forms.DragEventHandler(this.lstBoxItems_DragOver);
            this.lstBoxItems.DragDrop += new System.Windows.Forms.DragEventHandler(this.lstBoxItems_DragDrop);
            this.lstBoxItems.DoubleClick += new System.EventHandler(this.lstBoxItems_DoubleClick);

            this.lblBoxItemCount.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblBoxItemCount.Height = 18;
            this.lblBoxItemCount.Font = new System.Drawing.Font("Segoe UI", 7.5F);
            this.lblBoxItemCount.ForeColor = System.Drawing.Color.DimGray;
            this.lblBoxItemCount.Text = "0 items in box";
            this.lblBoxItemCount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblBoxItemCount.TabIndex = 2;

            this.pnlBoxDetail.Controls.Add(this.lstBoxItems);
            this.pnlBoxDetail.Controls.Add(this.lblBoxItemCount);
            this.pnlBoxDetail.Controls.Add(this.grpBoxInfo);

            // grpNonBox
            this.grpNonBox.Text = "Non-Box Items  (Ship Loose)";
            this.grpNonBox.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.grpNonBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpNonBox.Padding = new System.Windows.Forms.Padding(4);
            this.grpNonBox.TabIndex = 0;

            this.lstNonBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstNonBox.Font = new System.Drawing.Font("Consolas", 8.5F);
            this.lstNonBox.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstNonBox.AllowDrop = true;
            this.lstNonBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lstNonBox.ItemHeight = 17;
            this.lstNonBox.TabIndex = 0;
            this.lstNonBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.lstNonBox_MouseDown);
            this.lstNonBox.DragOver += new System.Windows.Forms.DragEventHandler(this.lstNonBox_DragOver);
            this.lstNonBox.DragDrop += new System.Windows.Forms.DragEventHandler(this.lstNonBox_DragDrop);
            this.lstNonBox.DoubleClick += new System.EventHandler(this.lstNonBox_DoubleClick);

            this.grpNonBox.Controls.Add(this.lstNonBox);

            // pnlBottom
            this.pnlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBottom.Height = 44;
            this.pnlBottom.BackColor = System.Drawing.Color.FromArgb(240, 240, 240);
            this.pnlBottom.TabIndex = 11;

            this.btnSave.Location = new System.Drawing.Point(8, 8);
            this.btnSave.Size = new System.Drawing.Size(130, 28);
            this.btnSave.Text = "Save Packing List";
            this.btnSave.BackColor = System.Drawing.Color.FromArgb(34, 139, 34);
            this.btnSave.ForeColor = System.Drawing.Color.White;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);

            this.btnViewPrint.Location = new System.Drawing.Point(148, 8);
            this.btnViewPrint.Size = new System.Drawing.Size(110, 28);
            this.btnViewPrint.Text = "View / Print";
            this.btnViewPrint.BackColor = System.Drawing.Color.SteelBlue;
            this.btnViewPrint.ForeColor = System.Drawing.Color.White;
            this.btnViewPrint.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnViewPrint.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnViewPrint.UseVisualStyleBackColor = false;
            this.btnViewPrint.Click += new System.EventHandler(this.btnViewPrint_Click);

            this.btnClear.Location = new System.Drawing.Point(268, 8);
            this.btnClear.Size = new System.Drawing.Size(80, 28);
            this.btnClear.Text = "Clear";
            this.btnClear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClear.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);

            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(370, 14);
            this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblStatus.ForeColor = System.Drawing.Color.DimGray;
            this.lblStatus.Text = "Ready.";

            this.pnlBottom.Controls.Add(this.btnSave);
            this.pnlBottom.Controls.Add(this.btnViewPrint);
            this.pnlBottom.Controls.Add(this.btnClear);
            this.pnlBottom.Controls.Add(this.lblStatus);

            // ctxBox
            this.mnuRenameBox.Text = "Rename";
            this.mnuRenameBox.Click += new System.EventHandler(this.mnuRenameBox_Click);
            this.mnuDeleteBox.Text = "Delete";
            this.mnuDeleteBox.Click += new System.EventHandler(this.mnuDeleteBox_Click);
            this.ctxBox.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this.mnuRenameBox, this.mnuDeleteBox });

            // Form
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1100, 680);
            this.Controls.Add(this.tblMain);
            this.Controls.Add(this.pnlBottom);
            this.Controls.Add(this.pnlTop);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.MinimumSize = new System.Drawing.Size(900, 560);
            this.Name = "ProjectPackingList";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Project Packing List";
            this.Load += new System.EventHandler(this.ProjectPackingList_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmProjectmaster_FormClosing);

            this.pnlTop.ResumeLayout(false);
            this.pnlTop.PerformLayout();
            this.tblMain.ResumeLayout(false);
            this.grpBOM.ResumeLayout(false);
            this.pnlBomTools.ResumeLayout(false);
            this.pnlBomTools.PerformLayout();
            this.pnlButtons.ResumeLayout(false);
            this.splitRight.Panel1.ResumeLayout(false);
            this.splitRight.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitRight)).EndInit();
            this.splitRight.ResumeLayout(false);
            this.grpBoxes.ResumeLayout(false);
            this.splitBoxes.Panel1.ResumeLayout(false);
            this.splitBoxes.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitBoxes)).EndInit();
            this.splitBoxes.ResumeLayout(false);
            this.pnlBoxDetail.ResumeLayout(false);
            this.grpBoxInfo.ResumeLayout(false);
            this.grpBoxInfo.PerformLayout();
            this.grpNonBox.ResumeLayout(false);
            this.pnlBottom.ResumeLayout(false);
            this.pnlBottom.PerformLayout();
            this.ctxBox.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel pnlTop;
        private System.Windows.Forms.Label lblProject;
        private System.Windows.Forms.Label projectName;
        private System.Windows.Forms.ComboBox cmbProject;
        private System.Windows.Forms.Button btnLoad;
        private System.Windows.Forms.Label lblPackingNo;
        private System.Windows.Forms.TextBox txtPackingNo;
        private System.Windows.Forms.Label lblDate;
        private System.Windows.Forms.DateTimePicker dtpDate;
        private System.Windows.Forms.Label lblversion;
        private System.Windows.Forms.Label PackingVersion;

        private System.Windows.Forms.TableLayoutPanel tblMain;

        private System.Windows.Forms.GroupBox grpBOM;
        private System.Windows.Forms.Panel pnlBomTools;
        private System.Windows.Forms.Label lblBomSearch;
        private System.Windows.Forms.TextBox txtBomSearch;
        private System.Windows.Forms.Button btnManualEntry;
        private System.Windows.Forms.Button btnSelectAllBom;
        private System.Windows.Forms.Button btnClearBomSelection;
        private System.Windows.Forms.TreeView tvBOM;
        private System.Windows.Forms.Label lblBOMCount;

        private System.Windows.Forms.Panel pnlButtons;
        private System.Windows.Forms.Button btnAddBox;
        private System.Windows.Forms.Button btnAddPallet;
        private System.Windows.Forms.Button btnRemoveBox;
        private System.Windows.Forms.Button btnMoveToBox;
        private System.Windows.Forms.Button btnMoveToList;
        private System.Windows.Forms.Button btnMarkNonBox;

        private System.Windows.Forms.SplitContainer splitRight;

        private System.Windows.Forms.GroupBox grpBoxes;
        private System.Windows.Forms.SplitContainer splitBoxes;
        private System.Windows.Forms.ListBox lstBoxes;

        private System.Windows.Forms.Panel pnlBoxDetail;
        private System.Windows.Forms.GroupBox grpBoxInfo;
        private System.Windows.Forms.Label lblBoxLabel;
        private System.Windows.Forms.TextBox txtBoxLabel;
        private System.Windows.Forms.Label lblGW;
        private System.Windows.Forms.TextBox txtGW;
        private System.Windows.Forms.Label lblNW;
        private System.Windows.Forms.TextBox txtNW;
        private System.Windows.Forms.Label lblDims;
        private System.Windows.Forms.TextBox txtLength;
        private System.Windows.Forms.Label lblDimsX1;
        private System.Windows.Forms.TextBox txtWidth;
        private System.Windows.Forms.Label lblDimsX2;
        private System.Windows.Forms.TextBox txtHeight;
        private System.Windows.Forms.Label lblDimsUnit;
        private System.Windows.Forms.ComboBox cmbDimUnit;
        private System.Windows.Forms.Button btnApplyBoxInfo;
        private System.Windows.Forms.ListBox lstBoxItems;
        private System.Windows.Forms.Label lblBoxItemCount;

        private System.Windows.Forms.GroupBox grpNonBox;
        private System.Windows.Forms.ListBox lstNonBox;

        private System.Windows.Forms.Panel pnlBottom;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnViewPrint;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Label lblStatus;

        private System.Windows.Forms.ContextMenuStrip ctxBox;
        private System.Windows.Forms.ToolStripMenuItem mnuRenameBox;
        private System.Windows.Forms.ToolStripMenuItem mnuDeleteBox;
    }
}
