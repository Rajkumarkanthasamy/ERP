// ============================================================================
//  ProjectPackingList.cs
//  Namespace : Erp_Project_With_Buttons.Project_Master
//  Framework : .NET 4.5  /  Windows Forms
//
//  NuGet required:
//      Install-Package iTextSharp
//
//  Features:
//   - ProjectBOM products + MachineBOM items (tree, drag header or item)
//   - Boxes and Pallets (same Label / LxWxH / GW / NW fields)
//   - Double-click rename (saved to PDF + DB ProductName)
//   - Search products/items, manual entry
//   - Plain PDF (no table background fills)
// ============================================================================

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Font = iTextSharp.text.Font;
using Font2 = System.Drawing.Font;
using Image = iTextSharp.text.Image;
using Rectangle = iTextSharp.text.Rectangle;

namespace Erp_Project_With_Buttons.Project_Master
{
    // =========================================================================
    //  Models
    // =========================================================================

    [Serializable]
    public class BomItem
    {
        public int ID { get; set; }
        public string ProjectCode { get; set; }
        public string ProductNo { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public string ProductType { get; set; }
        public decimal Quantity { get; set; }
        public string UOM { get; set; }

        /// <summary>PRODUCT (ProjectBOM header) or ITEM (MachineBOM row).</summary>
        public string BomKind { get; set; } = "PRODUCT";

        /// <summary>Parent ProductNo for MachineBOM items.</summary>
        public string ParentProductNo { get; set; }

        /// <summary>True when added via Manual Entry (not from BOM tables).</summary>
        public bool IsManual { get; set; }

        /// <summary>Stable in-memory key (avoids Product/Machine ID collisions).</summary>
        public string Key
        {
            get
            {
                if (IsManual) return "MAN:" + ProductCode + "|" + ProductName + "|" + BomKind;
                if (string.Equals(BomKind, "ITEM", StringComparison.OrdinalIgnoreCase))
                    return "M:" + ID;
                return "P:" + ID;
            }
        }

        public override string ToString()
        {
            string kind = string.Equals(BomKind, "ITEM", StringComparison.OrdinalIgnoreCase) ? "ITEM" : "PROD";
            return "[" + kind + "] [" + ProductCode + "]  " + ProductName + "  x" + Quantity + "  " + ProductType;
        }
    }

    public class PackingBox
    {
        public int BoxNumber { get; set; }
        public string BoxLabel { get; set; }
        /// <summary>BOX or PALLET</summary>
        public string ContainerType { get; set; } = "BOX";
        public decimal GrossWeight { get; set; }
        public decimal NetWeight { get; set; }
        public string Length { get; set; }
        public string Width { get; set; }
        public string Height { get; set; }
        public string Dimensions =>
            (!string.IsNullOrWhiteSpace(Length) || !string.IsNullOrWhiteSpace(Width) || !string.IsNullOrWhiteSpace(Height))
                ? "L" + Length + " x W" + Width + " x H" + Height
                : "";
        public List<BomItem> Items { get; set; } = new List<BomItem>();

        public override string ToString()
        {
            string prefix = string.Equals(ContainerType, "PALLET", StringComparison.OrdinalIgnoreCase) ? "Pallet" : "Box";
            return prefix + " " + BoxNumber + " — " + BoxLabel;
        }
    }

    [Serializable]
    public class DragPayload
    {
        public string Source { get; }
        public List<BomItem> Items { get; }
        public DragPayload(string source, List<BomItem> items) { Source = source; Items = items; }
    }

    // =========================================================================
    //  Form
    // =========================================================================

    public partial class ProjectPackingList : Form
    {
        private const string ConnStr =
            @"Data Source=.\SQLEXPRESS;Initial Catalog=ERP_Database;Integrated Security=True";

        private List<BomItem> _bom = new List<BomItem>();
        private List<PackingBox> _boxes = new List<PackingBox>();
        private List<BomItem> _nonBox = new List<BomItem>();
        private int _boxCtr = 0;
        private bool _loading = false;
        private string _projectCode = "";
        private string _bomSearch = "";
        DataAccessLayer DAL = new DataAccessLayer();
        frmForm2 Home = new frmForm2();
        Login.frmLoginForm Loginfrm = new Login.frmLoginForm();
        DataTable ProjectPackingListHeaderList = new DataTable();

        public ProjectPackingList()
        {
            InitializeComponent();
        }

        private void ProjectPackingList_Load(object sender, EventArgs e)
        {
            LoadProjectCombo();
            GeneratePackingNo();
            SetBoxDetailEnabled(false);
            SetStatus("Ready. Select a project and click Load BOM.");
        }

        // =========================================================================
        //  PROJECT COMBO / LOAD BOM
        // =========================================================================

        private void LoadProjectCombo()
        {
            try
            {
                cmbProject.Items.Clear();
                DataTable ProjectList = DAL.fnProjectBOMList(null).Tables[0];
                foreach (DataRow dr in ProjectList.Rows)
                {
                    if (!cmbProject.Items.Contains(dr["ProjectCode"].ToString()))
                        cmbProject.Items.Add(dr["ProjectCode"].ToString());
                }
            }
            catch (Exception ex) { Err("LoadProjectCombo", ex); }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            btnViewPrint.Enabled = true;
            PackingVersion.Text = "-";
            if (cmbProject.SelectedIndex < 0)
            { Warn("Please select a project."); return; }

            _projectCode = cmbProject.SelectedItem.ToString();
            ClearAll();
            LoadBom();
        }

        private IEnumerable<BomItem> EnumeratePackedItems()
        {
            foreach (var box in _boxes)
                foreach (var item in box.Items)
                    yield return item;
            foreach (var item in _nonBox)
                yield return item;
        }

        private bool IsPacked(BomItem candidate)
        {
            foreach (var packed in EnumeratePackedItems())
            {
                // Prefer stable Key; also match ID+code because saved rows may not know PRODUCT vs ITEM
                if (string.Equals(packed.Key, candidate.Key, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (packed.ID != 0 && packed.ID == candidate.ID
                    && string.Equals(packed.ProductCode ?? "", candidate.ProductCode ?? "", StringComparison.OrdinalIgnoreCase)
                    && string.Equals(packed.ProductNo ?? "", candidate.ProductNo ?? "", StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        private void RebuildBomBalance()
        {
            _bom = _bom
                .Where(i => !IsPacked(i))
                .GroupBy(i => i.Key)
                .Select(g => g.First())
                .ToList();
        }

        private void ParseDimensions(string dbValue, out string length, out string width, out string height)
        {
            length = width = height = "";
            if (string.IsNullOrWhiteSpace(dbValue)) return;
            var parts = dbValue.Split('x');
            foreach (var part in parts)
            {
                var p = part.Trim();
                if (p.StartsWith("L", StringComparison.OrdinalIgnoreCase))
                    length = p.Substring(1).Trim();
                else if (p.StartsWith("W", StringComparison.OrdinalIgnoreCase))
                    width = p.Substring(1).Trim();
                else if (p.StartsWith("H", StringComparison.OrdinalIgnoreCase))
                    height = p.Substring(1).Trim();
            }
        }

        private BomItem RowToProduct(DataRow rdr)
        {
            return new BomItem
            {
                ID = Convert.ToInt32(rdr["ID"]),
                ProjectCode = rdr["ProjectCode"].ToString(),
                ProductNo = rdr["ProductNo"].ToString(),
                ProductName = rdr["ProductName"].ToString(),
                ProductCode = rdr["ProductCode"].ToString(),
                ProductType = rdr.Table.Columns.Contains("ProductType") ? rdr["ProductType"].ToString() : "Product",
                Quantity = Convert.ToDecimal(rdr["Quantity"]),
                UOM = rdr.Table.Columns.Contains("UOM") && rdr["UOM"] != DBNull.Value ? rdr["UOM"].ToString() : "",
                BomKind = "PRODUCT",
                ParentProductNo = rdr["ProductNo"].ToString()
            };
        }

        private BomItem RowToMachineItem(DataRow rdr)
        {
            string itemCode = rdr["ItemName"].ToString();
            string desc = itemCode;
            if (rdr.Table.Columns.Contains("ItemDescription") && rdr["ItemDescription"] != DBNull.Value
                && !string.IsNullOrWhiteSpace(rdr["ItemDescription"].ToString()))
                desc = rdr["ItemDescription"].ToString();

            return new BomItem
            {
                ID = Convert.ToInt32(rdr["ID"]),
                ProjectCode = rdr["ProjectCode"].ToString(),
                ProductNo = rdr["ProductNo"].ToString(),
                ProductName = desc,
                ProductCode = itemCode,
                ProductType = rdr.Table.Columns.Contains("ProductType") ? rdr["ProductType"].ToString() : "Item",
                Quantity = Convert.ToDecimal(rdr["Quantity"]),
                UOM = "",
                BomKind = "ITEM",
                ParentProductNo = rdr["ProductNo"].ToString()
            };
        }

        private DataTable LoadMachineBomTable(string projectCode)
        {
            // Direct query so the form compiles even before DAL snippet is pasted.
            // Optional: prefer DAL.fnMachineBOMPackingList after pasting PACKING_LIST_DAL_SNIPPET.cs
            const string sql = @"
SELECT
    mb.ID,
    mb.ProjectBOMCode,
    mb.ProductNo,
    mb.ItemName,
    ISNULL(im.ItemDescription, mb.ItemName) AS ItemDescription,
    mb.Quantity,
    mb.ProjectCode,
    mb.ProductType
FROM [ERP_Database].[dbo].[MachineBOM] mb
LEFT JOIN [ERP_Database].[dbo].[ItemMaster] im
    ON im.ItemCode = mb.ItemName
WHERE mb.ProjectCode = @ProjectCode
ORDER BY mb.ProductNo, mb.ItemName;";

            var dt = new DataTable();
            using (var cn = Open())
            using (var cmd = new SqlCommand(sql, cn))
            {
                cmd.Parameters.AddWithValue("@ProjectCode", projectCode ?? "");
                using (var da = new SqlDataAdapter(cmd))
                    da.Fill(dt);
            }
            return dt;
        }

        private List<BomItem> LoadProjectAndMachineBom()
        {
            var list = new List<BomItem>();
            DataTable products = DAL.fnProjectBOMList(_projectCode).Tables[0];
            foreach (DataRow rdr in products.Rows)
                list.Add(RowToProduct(rdr));

            try
            {
                DataTable machines = LoadMachineBomTable(_projectCode);
                foreach (DataRow rdr in machines.Rows)
                {
                    try { list.Add(RowToMachineItem(rdr)); }
                    catch { /* skip bad rows */ }
                }
            }
            catch (Exception ex)
            {
                SetStatus("MachineBOM load skipped: " + ex.Message);
            }
            return list;
        }

        private void LoadPackingListData_FromDAL(string packingNo, int version)
        {
            _bom.Clear();
            _boxes.Clear();
            _nonBox.Clear();

            DataTable dt = DAL.fnGetProjectPackingListHeader(null, packingNo).Tables[0];

            foreach (DataRow dr in dt.Rows)
            {
                string itemType = dr["ItemType"].ToString();
                string productType = dr.Table.Columns.Contains("ProductType") ? dr["ProductType"].ToString() : "";
                string bomKind = productType.IndexOf("Item", StringComparison.OrdinalIgnoreCase) >= 0
                    || productType.Equals("ITEM", StringComparison.OrdinalIgnoreCase)
                    ? "ITEM" : "PRODUCT";

                BomItem item = new BomItem
                {
                    ID = Convert.ToInt32(dr["BomID"]),
                    ProductNo = dr["ProductNo"].ToString(),
                    ProductName = dr["ProductName"].ToString(),
                    ProductCode = dr["ProductCode"].ToString(),
                    Quantity = Convert.ToDecimal(dr["Quantity"]),
                    ProductType = productType,
                    UOM = "UOM",
                    BomKind = bomKind,
                    ParentProductNo = dr["ProductNo"].ToString()
                };

                if (itemType == "BOX" || itemType == "PALLET")
                {
                    int boxNumber = Convert.ToInt32(dr["BoxNumber"]);
                    PackingBox box = _boxes.FirstOrDefault(b => b.BoxNumber == boxNumber);

                    string length, width, height;
                    ParseDimensions(dr["BoxDimensions"].ToString(), out length, out width, out height);

                    if (box == null)
                    {
                        box = new PackingBox
                        {
                            BoxNumber = boxNumber,
                            BoxLabel = dr["BoxLabel"].ToString(),
                            ContainerType = itemType == "PALLET" ? "PALLET" : "BOX",
                            GrossWeight = Convert.ToDecimal(dr["GrossWeight"]),
                            NetWeight = Convert.ToDecimal(dr["NetWeight"]),
                            Length = length,
                            Width = width,
                            Height = height
                        };
                        _boxes.Add(box);
                    }

                    box.Items.Add(item);
                }
                else
                {
                    _nonBox.Add(item);
                }
            }
            RebuildBomBalance();
            RefreshBom();
            RefreshBoxList();
            RefreshNonBox();
        }

        private void LoadBom()
        {
            ProjectPackingListHeaderList.Clear();
            ProjectPackingListHeaderList = DAL.fnGetProjectPackingListHeader(_projectCode, null).Tables[0];

            if (ProjectPackingListHeaderList.Rows.Count > 0)
            {
                PackingVersion.Text = ProjectPackingListHeaderList.Rows[0]["version"].ToString();
                LoadPackingListData_FromDAL(
                    ProjectPackingListHeaderList.Rows[0]["PackingNo"].ToString(),
                    int.Parse(ProjectPackingListHeaderList.Rows[0]["version"].ToString()));

                _bom = LoadProjectAndMachineBom();
                RebuildBomBalance();
                RefreshBom();
                btnSave.Enabled = false;
                SetStatus("Loaded saved packing + BOM tree for [" + _projectCode + "].");
            }
            else
            {
                btnSave.Enabled = true;
                try
                {
                    _bom = LoadProjectAndMachineBom();
                    RefreshBom();
                    int prods = _bom.Count(i => i.BomKind == "PRODUCT");
                    int items = _bom.Count(i => i.BomKind == "ITEM");
                    SetStatus("Loaded " + prods + " product(s), " + items + " machine item(s) for [" + _projectCode + "].");
                }
                catch (Exception ex) { Err("LoadBom", ex); }
            }
        }

        // =========================================================================
        //  BOX / PALLET MANAGEMENT
        // =========================================================================

        private void btnAddBox_Click(object sender, EventArgs e)
        {
            AddContainer("BOX");
        }

        private void btnAddPallet_Click(object sender, EventArgs e)
        {
            AddContainer("PALLET");
        }

        private void AddContainer(string containerType)
        {
            _boxCtr++;
            string label = (containerType == "PALLET" ? "Pallet " : "Box ") + _boxCtr;
            var box = new PackingBox
            {
                BoxNumber = _boxCtr,
                BoxLabel = label,
                ContainerType = containerType
            };
            _boxes.Add(box);
            RefreshBoxList();
            lstBoxes.SelectedItem = box;
            SetStatus("Added " + box.ToString() + ".");
            MarkDirty();
        }

        private void btnRemoveBox_Click(object sender, EventArgs e)
        {
            if (!(lstBoxes.SelectedItem is PackingBox box)) return;

            if (box.Items.Count > 0 &&
                MessageBox.Show("'" + box.BoxLabel + "' has " + box.Items.Count + " item(s). Return them to BOM list?",
                    "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                _bom.AddRange(box.Items);

            _boxes.Remove(box);
            RefreshBom();
            RefreshBoxList();
            SetBoxDetailEnabled(false);
            SetStatus("Removed " + box.BoxLabel + ".");
            MarkDirty();
        }

        private void btnApplyBoxInfo_Click(object sender, EventArgs e)
        {
            MarkDirty();
            if (!(lstBoxes.SelectedItem is PackingBox box)) return;

            box.BoxLabel = txtBoxLabel.Text.Trim();
            decimal gw; decimal.TryParse(txtGW.Text, out gw); box.GrossWeight = gw;
            decimal nw; decimal.TryParse(txtNW.Text, out nw); box.NetWeight = nw;
            box.Length = txtLength.Text.Trim();
            box.Width = txtWidth.Text.Trim();
            box.Height = txtHeight.Text.Trim();

            if (string.IsNullOrWhiteSpace(box.BoxLabel))
                box.BoxLabel = (box.ContainerType == "PALLET" ? "Pallet " : "Box ") + box.BoxNumber;

            RefreshBoxList();
            lstBoxes.SelectedItem = box;
            RefreshBoxItems();
            SetStatus(box.BoxLabel + " updated — GW:" + box.GrossWeight + " kg  NW:" + box.NetWeight + " kg  " + box.Dimensions);
        }

        private void mnuRenameBox_Click(object sender, EventArgs e)
        {
            if (!(lstBoxes.SelectedItem is PackingBox box)) return;
            string n = SimplePrompt("Rename", "Label:", box.BoxLabel);
            if (!string.IsNullOrWhiteSpace(n)) { box.BoxLabel = n; txtBoxLabel.Text = n; RefreshBoxList(); MarkDirty(); }
        }

        private void mnuDeleteBox_Click(object sender, EventArgs e) => btnRemoveBox_Click(sender, e);

        private void lstBoxes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_loading) return;
            if (lstBoxes.SelectedItem is PackingBox box)
            {
                SetBoxDetailEnabled(true);
                txtBoxLabel.Text = box.BoxLabel;
                txtGW.Text = box.GrossWeight > 0 ? box.GrossWeight.ToString("G29") : "";
                txtNW.Text = box.NetWeight > 0 ? box.NetWeight.ToString("G29") : "";
                txtLength.Text = box.Length ?? "";
                txtWidth.Text = box.Width ?? "";
                txtHeight.Text = box.Height ?? "";
                grpBoxInfo.Text = box.ContainerType == "PALLET" ? "Pallet Details" : "Box Details";
                lblBoxLabel.Text = box.ContainerType == "PALLET" ? "Pallet Label :" : "Box Label :";
            }
            else
            {
                SetBoxDetailEnabled(false);
                ClearBoxDetailFields();
            }
            RefreshBoxItems();
        }

        private void SetBoxDetailEnabled(bool on)
        {
            grpBoxInfo.Enabled = on;
            if (!on) ClearBoxDetailFields();
        }

        private void ClearBoxDetailFields()
        {
            txtBoxLabel.Text = "";
            txtGW.Text = "";
            txtNW.Text = "";
            txtLength.Text = "";
            txtWidth.Text = "";
            txtHeight.Text = "";
        }

        private void MarkDirty()
        {
            btnViewPrint.Enabled = false;
            btnSave.Enabled = true;
        }

        // =========================================================================
        //  SELECTION HELPERS (TreeView)
        // =========================================================================

        private List<BomItem> GetSelectedBomItems(bool expandProductWithChildren)
        {
            var result = new List<BomItem>();
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            void Add(BomItem bi)
            {
                if (bi == null) return;
                if (seen.Add(bi.Key)) result.Add(bi);
            }

            TreeNode n = tvBOM.SelectedNode;
            if (n == null) return result;

            var item = n.Tag as BomItem;
            if (item == null) return result;

            if (expandProductWithChildren &&
                string.Equals(item.BomKind, "PRODUCT", StringComparison.OrdinalIgnoreCase))
            {
                Add(item);
                foreach (TreeNode child in n.Nodes)
                    Add(child.Tag as BomItem);
            }
            else
            {
                Add(item);
            }
            return result;
        }

        // =========================================================================
        //  BUTTON MOVES
        // =========================================================================

        private void btnMoveToBox_Click(object sender, EventArgs e)
        {
            if (!(lstBoxes.SelectedItem is PackingBox box))
            { Warn("Select a box/pallet first."); return; }

            var sel = GetSelectedBomItems(true);
            if (!sel.Any()) { Warn("Select a BOM product or item to move."); return; }

            foreach (var item in sel)
            {
                if (_bom.Contains(item) || _bom.Any(b => b.Key == item.Key))
                {
                    _bom.RemoveAll(b => b.Key == item.Key);
                    if (!box.Items.Any(x => x.Key == item.Key))
                        box.Items.Add(item);
                }
            }
            RefreshBom();
            RefreshBoxItems();
            MarkDirty();
        }

        private void btnMoveToList_Click(object sender, EventArgs e)
        {
            var selBox = lstBoxItems.SelectedItems.Cast<BomItem>().ToList();
            if (selBox.Any() && lstBoxes.SelectedItem is PackingBox box)
            {
                foreach (var i in selBox) { _bom.Add(i); box.Items.Remove(i); }
                RefreshBom(); RefreshBoxItems(); MarkDirty(); return;
            }

            var selNon = lstNonBox.SelectedItems.Cast<BomItem>().ToList();
            foreach (var i in selNon) { _bom.Add(i); _nonBox.Remove(i); }
            RefreshBom(); RefreshNonBox(); MarkDirty();
        }

        private void btnMarkNonBox_Click(object sender, EventArgs e)
        {
            var sel = GetSelectedBomItems(true);
            if (!sel.Any()) return;
            foreach (var i in sel)
            {
                _bom.RemoveAll(b => b.Key == i.Key);
                if (!_nonBox.Any(x => x.Key == i.Key))
                    _nonBox.Add(i);
            }
            RefreshBom(); RefreshNonBox(); MarkDirty();
        }

        // =========================================================================
        //  DRAG & DROP
        // =========================================================================

        private void tvBOM_ItemDrag(object sender, ItemDragEventArgs e)
        {
            if (!(e.Item is TreeNode node) || !(node.Tag is BomItem)) return;
            tvBOM.SelectedNode = node;
            var items = GetSelectedBomItems(true);
            if (items.Count == 0) return;
            tvBOM.DoDragDrop(new DragPayload("BOM", items), DragDropEffects.Move);
        }

        private void tvBOM_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = e.Data.GetDataPresent(typeof(DragPayload)) ? DragDropEffects.Move : DragDropEffects.None;
        }

        private void tvBOM_DragDrop(object sender, DragEventArgs e)
        {
            MarkDirty();
            var p = e.Data.GetData(typeof(DragPayload)) as DragPayload; if (p == null) return;
            if (p.Source == "BOX" && lstBoxes.SelectedItem is PackingBox sb)
            {
                foreach (var i in p.Items) { sb.Items.RemoveAll(x => x.Key == i.Key); if (!_bom.Any(x => x.Key == i.Key)) _bom.Add(i); }
                RefreshBom(); RefreshBoxItems();
            }
            else if (p.Source == "NONBOX")
            {
                foreach (var i in p.Items) { _nonBox.RemoveAll(x => x.Key == i.Key); if (!_bom.Any(x => x.Key == i.Key)) _bom.Add(i); }
                RefreshBom(); RefreshNonBox();
            }
        }

        private void lstBoxItems_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left || lstBoxItems.SelectedItems.Count == 0) return;
            lstBoxItems.DoDragDrop(new DragPayload("BOX", lstBoxItems.SelectedItems.Cast<BomItem>().ToList()), DragDropEffects.Move);
        }

        private void lstBoxItems_DragOver(object sender, DragEventArgs e)
        { e.Effect = e.Data.GetDataPresent(typeof(DragPayload)) ? DragDropEffects.Move : DragDropEffects.None; }

        private void lstBoxItems_DragDrop(object sender, DragEventArgs e)
        {
            MarkDirty();
            var p = e.Data.GetData(typeof(DragPayload)) as DragPayload; if (p == null) return;
            if (!(lstBoxes.SelectedItem is PackingBox dest)) return;

            if (p.Source == "BOM")
            {
                foreach (var i in p.Items)
                {
                    _bom.RemoveAll(x => x.Key == i.Key);
                    if (!dest.Items.Any(x => x.Key == i.Key)) dest.Items.Add(i);
                }
                RefreshBom();
            }
            else if (p.Source == "BOX")
            {
                foreach (var b in _boxes)
                    foreach (var i in p.Items.ToList())
                        if (b.Items.Any(x => x.Key == i.Key))
                        {
                            b.Items.RemoveAll(x => x.Key == i.Key);
                            if (!dest.Items.Any(x => x.Key == i.Key)) dest.Items.Add(i);
                        }
            }
            else if (p.Source == "NONBOX")
            {
                foreach (var i in p.Items)
                {
                    _nonBox.RemoveAll(x => x.Key == i.Key);
                    if (!dest.Items.Any(x => x.Key == i.Key)) dest.Items.Add(i);
                }
                RefreshNonBox();
            }

            RefreshBoxItems();
        }

        private void lstBoxes_MouseDown(object sender, MouseEventArgs e) { }

        private void lstBoxes_DragOver(object sender, DragEventArgs e)
        {
            int idx = lstBoxes.IndexFromPoint(lstBoxes.PointToClient(new Point(e.X, e.Y)));
            if (idx >= 0) lstBoxes.SelectedIndex = idx;
            e.Effect = e.Data.GetDataPresent(typeof(DragPayload)) ? DragDropEffects.Move : DragDropEffects.None;
        }

        private void lstBoxes_DragDrop(object sender, DragEventArgs e) => lstBoxItems_DragDrop(sender, e);

        private void lstNonBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left || lstNonBox.SelectedItems.Count == 0) return;
            lstNonBox.DoDragDrop(new DragPayload("NONBOX", lstNonBox.SelectedItems.Cast<BomItem>().ToList()), DragDropEffects.Move);
        }

        private void lstNonBox_DragOver(object sender, DragEventArgs e)
        { e.Effect = e.Data.GetDataPresent(typeof(DragPayload)) ? DragDropEffects.Move : DragDropEffects.None; }

        private void lstNonBox_DragDrop(object sender, DragEventArgs e)
        {
            MarkDirty();
            var p = e.Data.GetData(typeof(DragPayload)) as DragPayload; if (p == null) return;
            if (p.Source == "BOM")
            {
                foreach (var i in p.Items)
                {
                    _bom.RemoveAll(x => x.Key == i.Key);
                    if (!_nonBox.Any(x => x.Key == i.Key)) _nonBox.Add(i);
                }
                RefreshBom(); RefreshNonBox();
            }
        }

        // =========================================================================
        //  DOUBLE-CLICK RENAME
        // =========================================================================

        private void RenameBomItem(BomItem item)
        {
            if (item == null) return;
            string n = SimplePrompt("Rename description", "Name (ProductName):", item.ProductName);
            if (string.IsNullOrWhiteSpace(n) || n == item.ProductName) return;
            item.ProductName = n.Trim();
            // Keep packed copies in sync by Key
            foreach (var box in _boxes)
                foreach (var bi in box.Items.Where(x => x.Key == item.Key))
                    bi.ProductName = item.ProductName;
            foreach (var bi in _nonBox.Where(x => x.Key == item.Key))
                bi.ProductName = item.ProductName;
            RefreshBom(); RefreshBoxItems(); RefreshNonBox();
            MarkDirty();
            SetStatus("Renamed to: " + item.ProductName);
        }

        private void tvBOM_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            RenameBomItem(e.Node?.Tag as BomItem);
        }

        private void lstBoxItems_DoubleClick(object sender, EventArgs e)
        {
            if (lstBoxItems.SelectedItem is BomItem item) RenameBomItem(item);
        }

        private void lstNonBox_DoubleClick(object sender, EventArgs e)
        {
            if (lstNonBox.SelectedItem is BomItem item) RenameBomItem(item);
        }

        // =========================================================================
        //  SEARCH
        // =========================================================================

        private void txtBomSearch_TextChanged(object sender, EventArgs e)
        {
            _bomSearch = txtBomSearch.Text.Trim();
            RefreshBom();
        }

        private bool MatchesSearch(BomItem i)
        {
            if (string.IsNullOrEmpty(_bomSearch)) return true;
            string q = _bomSearch;
            return (i.ProductCode ?? "").IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0
                || (i.ProductName ?? "").IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0
                || (i.ProductNo ?? "").IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0
                || (i.ProductType ?? "").IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0
                || (i.BomKind ?? "").IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        // =========================================================================
        //  MANUAL ENTRY
        // =========================================================================

        private void btnManualEntry_Click(object sender, EventArgs e)
        {
            using (var f = new Form())
            {
                f.Text = "Manual BOM Entry";
                f.FormBorderStyle = FormBorderStyle.FixedDialog;
                f.StartPosition = FormStartPosition.CenterParent;
                f.Size = new Size(420, 260);
                f.MaximizeBox = false; f.MinimizeBox = false;

                var lblType = new Label { Text = "Type:", Left = 12, Top = 16, AutoSize = true };
                var cmbType = new ComboBox { Left = 120, Top = 12, Width = 250, DropDownStyle = ComboBoxStyle.DropDownList };
                cmbType.Items.AddRange(new object[] { "Product", "Item" });
                cmbType.SelectedIndex = 0;

                var lblCode = new Label { Text = "Code:", Left = 12, Top = 48, AutoSize = true };
                var txtCode = new TextBox { Left = 120, Top = 44, Width = 250 };

                var lblName = new Label { Text = "Name:", Left = 12, Top = 80, AutoSize = true };
                var txtName = new TextBox { Left = 120, Top = 76, Width = 250 };

                var lblQty = new Label { Text = "Qty:", Left = 12, Top = 112, AutoSize = true };
                var txtQty = new TextBox { Left = 120, Top = 108, Width = 100, Text = "1" };

                var lblHint = new Label
                {
                    Text = "Product: product code + name + type + qty\nItem: item code + name + type + qty\n(No temp BomID required)",
                    Left = 12, Top = 140, Width = 380, Height = 40
                };

                var ok = new Button { Text = "Add", Left = 200, Top = 185, Width = 80, DialogResult = DialogResult.OK };
                var ca = new Button { Text = "Cancel", Left = 290, Top = 185, Width = 80, DialogResult = DialogResult.Cancel };
                f.AcceptButton = ok; f.CancelButton = ca;
                f.Controls.AddRange(new Control[] { lblType, cmbType, lblCode, txtCode, lblName, txtName, lblQty, txtQty, lblHint, ok, ca });

                if (f.ShowDialog(this) != DialogResult.OK) return;

                string code = txtCode.Text.Trim();
                string name = txtName.Text.Trim();
                decimal qty;
                if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(name))
                { Warn("Code and Name are required."); return; }
                if (!decimal.TryParse(txtQty.Text.Trim(), out qty) || qty <= 0)
                { Warn("Enter a valid Qty."); return; }

                bool isItem = cmbType.SelectedItem.ToString() == "Item";
                var bi = new BomItem
                {
                    ID = 0,
                    ProjectCode = _projectCode,
                    ProductNo = code,
                    ProductCode = code,
                    ProductName = name,
                    ProductType = isItem ? "Item" : "Product",
                    Quantity = qty,
                    UOM = "",
                    BomKind = isItem ? "ITEM" : "PRODUCT",
                    ParentProductNo = code,
                    IsManual = true
                };
                _bom.Add(bi);
                RefreshBom();
                MarkDirty();
                SetStatus("Manual " + bi.BomKind + " added: " + bi.ProductCode);
            }
        }

        // =========================================================================
        //  REFRESH HELPERS
        // =========================================================================

        private static string NormProductNo(string s)
        {
            s = (s ?? "").Trim();
            int n;
            if (int.TryParse(s, out n)) return n.ToString();
            return s;
        }

        private void RefreshBom()
        {
            tvBOM.BeginUpdate();
            tvBOM.Nodes.Clear();

            var products = _bom.Where(i => string.Equals(i.BomKind, "PRODUCT", StringComparison.OrdinalIgnoreCase)).ToList();
            var items = _bom.Where(i => string.Equals(i.BomKind, "ITEM", StringComparison.OrdinalIgnoreCase)).ToList();

            // Group machine items under matching ProductNo (normalize numeric codes)
            var itemsByProduct = items.GroupBy(i => NormProductNo(i.ParentProductNo ?? i.ProductNo), StringComparer.OrdinalIgnoreCase)
                                      .ToDictionary(g => g.Key, g => g.ToList(), StringComparer.OrdinalIgnoreCase);

            int visible = 0;
            foreach (var prod in products.OrderBy(p => p.ProductNo).ThenBy(p => p.ProductName))
            {
                string pno = NormProductNo(prod.ProductNo);
                List<BomItem> children;
                itemsByProduct.TryGetValue(pno, out children);
                if (children == null) children = new List<BomItem>();

                bool prodMatch = MatchesSearch(prod);
                var matchedChildren = children.Where(MatchesSearch).ToList();
                if (!prodMatch && matchedChildren.Count == 0 && !string.IsNullOrEmpty(_bomSearch))
                    continue;

                var pNode = new TreeNode(prod.ToString()) { Tag = prod };
                foreach (var child in (string.IsNullOrEmpty(_bomSearch) ? children : matchedChildren)
                         .OrderBy(c => c.ProductCode))
                {
                    pNode.Nodes.Add(new TreeNode("    " + child.ToString()) { Tag = child });
                    visible++;
                }
                // remove claimed children
                itemsByProduct.Remove(pno);
                tvBOM.Nodes.Add(pNode);
                visible++;
                if (!string.IsNullOrEmpty(_bomSearch)) pNode.Expand();
            }

            // Orphan machine items (no matching product header still in _bom)
            foreach (var kv in itemsByProduct.OrderBy(k => k.Key))
            {
                foreach (var child in kv.Value.Where(MatchesSearch).OrderBy(c => c.ProductCode))
                {
                    tvBOM.Nodes.Add(new TreeNode(child.ToString()) { Tag = child });
                    visible++;
                }
            }

            // Manual products without children already added; any leftover PRODUCT already handled.
            // Manual ITEMs without parent already in orphan path if BomKind=ITEM.

            tvBOM.EndUpdate();
            lblBOMCount.Text = visible + " node(s)" + (string.IsNullOrEmpty(_bomSearch) ? "" : " (filtered)");
        }

        private void RefreshBoxList()
        {
            if (_boxes.Count > 0)
                _boxCtr = Math.Max(_boxCtr, _boxes.Max(b => b.BoxNumber));
            _loading = true;
            var sel = lstBoxes.SelectedItem as PackingBox;
            lstBoxes.BeginUpdate();
            lstBoxes.Items.Clear();
            _boxes.Sort((a, b) => a.BoxNumber.CompareTo(b.BoxNumber));
            foreach (var b in _boxes) lstBoxes.Items.Add(b);
            lstBoxes.EndUpdate();
            if (sel != null && _boxes.Contains(sel)) lstBoxes.SelectedItem = sel;
            else if (_boxes.Count > 0) lstBoxes.SelectedIndex = _boxes.Count - 1;
            _loading = false;
            RefreshBoxItems();
        }

        private void RefreshBoxItems()
        {
            lstBoxItems.BeginUpdate();
            lstBoxItems.Items.Clear();
            if (lstBoxes.SelectedItem is PackingBox box)
            {
                foreach (var i in box.Items) lstBoxItems.Items.Add(i);
                lblBoxItemCount.Text = box.Items.Count + " item(s) in " + box.BoxLabel;
            }
            else
            { lblBoxItemCount.Text = ""; }
            lstBoxItems.EndUpdate();
        }

        private void RefreshNonBox()
        {
            lstNonBox.BeginUpdate();
            lstNonBox.Items.Clear();
            foreach (var i in _nonBox) lstNonBox.Items.Add(i);
            lstNonBox.EndUpdate();
        }

        // =========================================================================
        //  SAVE
        // =========================================================================

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_projectCode)) { Warn("Load a project first."); return; }
            if (string.IsNullOrWhiteSpace(txtPackingNo.Text)) { Warn("Packing No is required."); return; }

            if (lstBoxes.SelectedItem is PackingBox)
                btnApplyBoxInfo_Click(null, null);

            try
            {
                try
                {
                    string pn = txtPackingNo.Text.Trim();
                    int version = 1;
                    if (ProjectPackingListHeaderList.Rows.Count > 0)
                    {
                        int temVersion = int.Parse(ProjectPackingListHeaderList.Rows[0]["version"].ToString());
                        version = temVersion + 1;
                    }

                    int boxesAndPallets = _boxes.Count;
                    int fnProjectPackingListHeaderStatus = DAL.fnProjectPackingListHeader(
                        0, pn, _projectCode, dtpDate.Value.Date.ToString("yyyy-MM-dd"),
                        boxesAndPallets.ToString(),
                        Loginfrm.GetUserDetails[2].ToString(),
                        "", version);

                    int sl = 1;
                    if (fnProjectPackingListHeaderStatus == 1)
                    {
                        foreach (var box in _boxes)
                        {
                            string itemType = string.Equals(box.ContainerType, "PALLET", StringComparison.OrdinalIgnoreCase)
                                ? "PALLET" : "BOX";
                            foreach (var item in box.Items)
                            {
                                DAL.fnProjectPackingListDetail(
                                    0, pn, sl.ToString(),
                                    box.BoxNumber.ToString(), box.BoxLabel, box.Dimensions,
                                    box.GrossWeight.ToString(), box.NetWeight.ToString(),
                                    item.ID.ToString(), item.ProductNo, item.ProductName, item.ProductCode,
                                    item.Quantity.ToString(), itemType, version, item.ProductType);
                                sl++;
                            }
                        }

                        foreach (var item in _nonBox)
                        {
                            DAL.fnProjectPackingListDetail(
                                0, pn, sl.ToString(), "0", "Non-Box", "", "0", "0",
                                item.ID.ToString(), item.ProductNo, item.ProductName, item.ProductCode,
                                item.Quantity.ToString(), "NONBOX", version, item.ProductType);
                            sl++;
                        }
                    }

                    int palletCount = _boxes.Count(b => b.ContainerType == "PALLET");
                    int boxCount = _boxes.Count(b => b.ContainerType != "PALLET");
                    SetStatus("Packing list [" + pn + "] saved — " + boxCount + " box(es), "
                        + palletCount + " pallet(s), " + _nonBox.Count + " non-box item(s).");
                    MessageBox.Show("Saved successfully!", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    btnViewPrint.Enabled = true;
                    btnSave.Enabled = false;
                    GeneratePackingNo();
                }
                catch (Exception ERROR) { MessageBox.Show(ERROR.ToString()); }
            }
            catch (Exception ex) { Err("Save", ex); }
        }

        // =========================================================================
        //  PDF PRINT (plain — no table background fills)
        // =========================================================================

        private void btnViewPrint_Click(object sender, EventArgs e)
        {
            if (_boxes.Count == 0 && _nonBox.Count == 0)
            { Warn("Nothing to print yet."); return; }

            if (lstBoxes.SelectedItem is PackingBox)
                btnApplyBoxInfo_Click(null, null);

            string path = Path.Combine(
                @"C:\Users\Kanthara095\OneDrive - Illinois Tool Works, Inc\Documents\",
                "PackingList_" + txtPackingNo.Text + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".pdf");
            try
            {
                BuildPdf(path);
                System.Diagnostics.Process.Start(path);
                SetStatus("PDF opened: " + path);
            }
            catch (Exception ex) { Err("PDF", ex); }
        }

        private static PdfPCell PlainCell(string text, Font font, int align = Element.ALIGN_LEFT)
        {
            return new PdfPCell(new Phrase(text ?? "", font))
            {
                BackgroundColor = BaseColor.WHITE,
                Padding = 3,
                HorizontalAlignment = align,
                BorderColor = BaseColor.BLACK
            };
        }

        private void BuildPdf(string path)
        {
            float contentHeight = 650f;
            float headerHeight = 100f;
            float footerHeight = 92f;
            float pageWidth = PageSize.A4.Width;
            float pageHeight = contentHeight + headerHeight + footerHeight;
            var customPage = new Rectangle(pageWidth, pageHeight);

            var doc = new Document(customPage, 36, 36, headerHeight, footerHeight);
            var writer = PdfWriter.GetInstance(doc, new FileStream(path, FileMode.Create));
            writer.PageEvent = new InstronHeaderFooter(headerHeight, footerHeight);
            doc.Open();

            var fTitle = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 10, BaseColor.BLACK);
            var fHead = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 8, BaseColor.BLACK);
            var fCell = FontFactory.GetFont(FontFactory.HELVETICA, 8, BaseColor.BLACK);
            var fBold = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 8, BaseColor.BLACK);
            var fMeta = FontFactory.GetFont(FontFactory.HELVETICA, 7, BaseColor.DARK_GRAY);

            DataTable ProjectDetails = DAL.getProjectDetailsPacking(_projectCode).Tables[0];

            doc.Add(new Paragraph("PACKING LIST - " + ProjectDetails.Rows[0]["customer"], fTitle) { Alignment = Element.ALIGN_CENTER });
            doc.Add(new Paragraph("Kind Attention -" + ProjectDetails.Rows[0]["ContactPerson"], fBold) { SpacingBefore = 10, Alignment = Element.ALIGN_LEFT });
            doc.Add(new Paragraph("Ship to - " + ProjectDetails.Rows[0]["customer"], fBold) { SpacingBefore = 3, Alignment = Element.ALIGN_LEFT });
            doc.Add(new Paragraph("" + ProjectDetails.Rows[0]["ShiptoCustomer"], fBold) { SpacingBefore = 3, Alignment = Element.ALIGN_LEFT });
            doc.Add(new Paragraph("PO ref - " + ProjectDetails.Rows[0]["PONo"] + " Date:" + ProjectDetails.Rows[0]["PODate"], fBold)
            { SpacingBefore = 3, SpacingAfter = 10, Alignment = Element.ALIGN_LEFT });

            foreach (var box in _boxes)
            {
                if (box.Items.Count == 0) continue;

                string kind = box.ContainerType == "PALLET" ? "PALLET" : "BOX";
                string hdr = "  " + kind + " — " + (box.BoxLabel ?? "").ToUpper();
                if (!string.IsNullOrWhiteSpace(box.Dimensions)) hdr += "   (" + box.Dimensions + ")";
                if (box.GrossWeight > 0) hdr += "   GW: " + box.GrossWeight + " kg   NW: " + box.NetWeight + " kg";

                var bt = new PdfPTable(1) { WidthPercentage = 100, SpacingBefore = 10 };
                bt.AddCell(PlainCell(hdr, fHead));
                doc.Add(bt);

                var t = new PdfPTable(new float[] { 5, 22, 38, 8, 20 }) { WidthPercentage = 100 };
                foreach (var h in new[] { "S/N", "Product Code", "Product Name", "Qty", "Type" })
                    t.AddCell(PlainCell(h, fHead, Element.ALIGN_CENTER));

                int sn = 1;
                foreach (var item in box.Items)
                {
                    t.AddCell(PlainCell((sn++).ToString(), fCell));
                    t.AddCell(PlainCell(item.ProductCode, fCell));
                    t.AddCell(PlainCell(item.ProductName, fCell));
                    t.AddCell(PlainCell(item.Quantity.ToString("G29"), fCell));
                    t.AddCell(PlainCell(item.ProductType, fCell));
                }
                doc.Add(t);
            }

            if (_nonBox.Count > 0)
            {
                var nbt = new PdfPTable(1) { WidthPercentage = 100, SpacingBefore = 14 };
                nbt.AddCell(PlainCell("  NON-BOX ITEMS  (SHIP LOOSE)", fHead));
                doc.Add(nbt);

                var t = new PdfPTable(new float[] { 5, 22, 44, 8, 14 }) { WidthPercentage = 100 };
                foreach (var h in new[] { "S/N", "Product No", "Product Name", "Qty", "Type" })
                    t.AddCell(PlainCell(h, fHead, Element.ALIGN_CENTER));

                int sn = 1;
                foreach (var item in _nonBox)
                {
                    t.AddCell(PlainCell((sn++).ToString(), fCell));
                    t.AddCell(PlainCell(item.ProductCode, fCell));
                    t.AddCell(PlainCell(item.ProductName, fCell));
                    t.AddCell(PlainCell(item.Quantity.ToString("G29"), fCell));
                    t.AddCell(PlainCell(item.ProductType, fCell));
                }
                doc.Add(t);
            }

            int totalBoxItems = _boxes.Sum(b => b.Items.Count);
            int palletCount = _boxes.Count(b => b.ContainerType == "PALLET");
            int boxCount = _boxes.Count(b => b.ContainerType != "PALLET");
            doc.Add(new Paragraph(
                "\nTotal Boxes: " + boxCount + "   |   Pallets: " + palletCount
                + "   |   Packed Items: " + totalBoxItems
                + "   |   Non-Box Items: " + _nonBox.Count
                + "   |   Grand Total: " + (totalBoxItems + _nonBox.Count),
                fBold)
            { SpacingBefore = 10, Alignment = Element.ALIGN_RIGHT });

            doc.Add(new Paragraph(
                "\nGenerated: " + DateTime.Now.ToString("dd-MMM-yyyy HH:mm") + "   |   By: " + Environment.UserName, fMeta)
            { Alignment = Element.ALIGN_RIGHT });

            doc.Close();
        }

        public class InstronHeaderFooter : PdfPageEventHelper
        {
            private Image _instronLogo;
            private Image _itwLogo;
            private readonly BaseColor _redLine = new BaseColor(180, 20, 60);
            private readonly Font _fFooter = FontFactory.GetFont(FontFactory.HELVETICA, 7, BaseColor.DARK_GRAY);
            private readonly float _headerHeight;
            private readonly float _footerHeight;
            private bool _imagesLoaded = false;

            public InstronHeaderFooter(float headerHeight, float footerHeight)
            {
                _headerHeight = headerHeight;
                _footerHeight = footerHeight;
            }

            private void LoadImages()
            {
                if (_imagesLoaded) return;

                try
                {
                    string instronPath = @"C:\Databasepath\InstronLogo3.jpg";
                    string itwPath = @"C:\Databasepath\ITWLogo.jpg";

                    if (File.Exists(instronPath))
                    {
                        _instronLogo = Image.GetInstance(instronPath);
                        _instronLogo.ScalePercent(40);
                    }
                    if (File.Exists(itwPath))
                    {
                        _itwLogo = Image.GetInstance(itwPath);
                        _itwLogo.ScalePercent(40);
                    }
                }
                catch { }
                _imagesLoaded = true;
            }

            public override void OnStartPage(PdfWriter writer, Document document)
            {
                base.OnStartPage(writer, document);
                LoadImages();

                var cb = writer.DirectContent;
                var pageHeight = document.PageSize.Height;

                // Header positioned at very top
                var headerTable = new PdfPTable(2) { WidthPercentage = 100 };
                headerTable.SetWidths(new float[] { 70, 30 });
                headerTable.TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin;

                var instronCell = new PdfPCell() { Border = Rectangle.NO_BORDER, VerticalAlignment = Element.ALIGN_BOTTOM };
                if (_instronLogo != null) instronCell.AddElement(_instronLogo);
                headerTable.AddCell(instronCell);

                var itwCell = new PdfPCell() { Border = Rectangle.NO_BORDER, VerticalAlignment = Element.ALIGN_BOTTOM, HorizontalAlignment = Element.ALIGN_RIGHT };
                if (_itwLogo != null) itwCell.AddElement(_itwLogo);
                headerTable.AddCell(itwCell);

                // Write header starting 10 points from top
                headerTable.WriteSelectedRows(0, -1, document.LeftMargin, pageHeight - 10, cb);

                // Red line at bottom of header area
                cb.SetColorStroke(_redLine);
                cb.SetLineWidth(2f);
                float lineY = pageHeight - _headerHeight + 10; // 10 points above content area
                cb.MoveTo(document.LeftMargin, lineY);
                cb.LineTo(document.PageSize.Width - document.RightMargin, lineY);
                cb.Stroke();
            }

            public override void OnEndPage(PdfWriter writer, Document document)
            {
                base.OnEndPage(writer, document);

                var cb = writer.DirectContent;

                // Red line at top of footer area
                cb.SetColorStroke(_redLine);
                cb.SetLineWidth(1f);
                float lineY = _footerHeight - 5; // 5 points above footer text
                cb.MoveTo(document.LeftMargin, lineY);
                cb.LineTo(document.PageSize.Width - document.RightMargin, lineY);
                cb.Stroke();

                // Footer table
                var footerTable = new PdfPTable(2) { WidthPercentage = 100 };
                footerTable.SetWidths(new float[] { 60, 40 });
                footerTable.TotalWidth = document.PageSize.Width - document.LeftMargin - document.RightMargin;

                var leftCell = new PdfPCell() { Border = Rectangle.NO_BORDER };
                leftCell.AddElement(new Paragraph("ITW India Private Limited (Instron Division)",
                    FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 8, BaseColor.BLACK)));
                leftCell.AddElement(new Paragraph("No. 497E, 14th Cross, 4th Phase, PIA, Bangalore – 560 058, India", _fFooter));
                leftCell.AddElement(new Paragraph("Ph: +91 (80) 28360184, Fax: +91 (80) 28360047", _fFooter));

                var webEmail = new Paragraph();
                webEmail.Add(new Chunk("www.instron.com ", FontFactory.GetFont(FontFactory.HELVETICA, 7, BaseColor.BLUE)));
                webEmail.Add(new Chunk("Email: sales.india@instron.com", _fFooter));
                leftCell.AddElement(webEmail);
                footerTable.AddCell(leftCell);

                var rightCell = new PdfPCell() { Border = Rectangle.NO_BORDER };
                rightCell.AddElement(new Paragraph("Registered Office:",
                    FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 8, BaseColor.BLACK)));
                rightCell.AddElement(new Paragraph("ITW India Private Limited.", _fFooter));
                rightCell.AddElement(new Paragraph("Plot Nos. 50-59, Sector-25, Faridabad, Ballabgarh,", _fFooter));
                rightCell.AddElement(new Paragraph("Haryana, India- 121004", _fFooter));
                rightCell.AddElement(new Paragraph("CIN No.- U32301HR1979PTC038643", _fFooter));
                footerTable.AddCell(rightCell);

                // Write footer at bottom of page
                footerTable.WriteSelectedRows(0, -1, document.LeftMargin, _footerHeight - 10, cb);
            }
        }


        // =========================================================================
        //  CLEAR
        // =========================================================================

        private void btnClear_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Clear all packing list data?", "Confirm",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                ClearAll();
        }

        private void ClearAll()
        {
            _bom.Clear(); _boxes.Clear(); _nonBox.Clear(); _boxCtr = 0; _bomSearch = "";
            if (txtBomSearch != null) txtBomSearch.Text = "";
            RefreshBom(); RefreshBoxList(); RefreshNonBox();
            SetBoxDetailEnabled(false);
            GeneratePackingNo();
            SetStatus("Cleared.");
        }

        private SqlConnection Open()
        {
            var cn = new SqlConnection(ConnStr);
            cn.Open();
            return cn;
        }

        private void GeneratePackingNo() =>
            txtPackingNo.Text = "PKG-" + DateTime.Now.ToString("yyyyMMdd-HHmmss");

        private void SetStatus(string msg)
        {
            lblStatus.Text = msg;
            lblStatus.ForeColor = msg.StartsWith("Error") ? Color.Firebrick : Color.DimGray;
        }

        private static void Warn(string msg) =>
            MessageBox.Show(msg, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);

        private static void Err(string ctx, Exception ex) =>
            MessageBox.Show("Error in " + ctx + ":\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

        private static string SimplePrompt(string title, string label, string def = "")
        {
            var f = new Form
            {
                Text = title,
                // Client area ~480x200; Height includes title bar / borders
                Size = new Size(520, 240),
                MinimumSize = new Size(420, 200),
                FormBorderStyle = FormBorderStyle.Sizable,
                StartPosition = FormStartPosition.CenterParent,
                MaximizeBox = false,
                MinimizeBox = false
            };
            var lb = new Label { Text = label, AutoSize = true, Location = new Point(12, 12) };
            var tb = new TextBox
            {
                Location = new Point(12, 36),
                Width = 480,
                Height = 110,
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                Text = def,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };
            var ok = new Button
            {
                Text = "OK",
                DialogResult = DialogResult.OK,
                Location = new Point(300, 160),
                Width = 90,
                Height = 28,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right
            };
            var ca = new Button
            {
                Text = "Cancel",
                DialogResult = DialogResult.Cancel,
                Location = new Point(400, 160),
                Width = 90,
                Height = 28,
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right
            };
            f.AcceptButton = ok; f.CancelButton = ca;
            f.Controls.AddRange(new Control[] { lb, tb, ok, ca });
            return f.ShowDialog() == DialogResult.OK ? tb.Text.Trim() : def;
        }

        private void fnClose()
        {
            this.Hide();
            Home.Show();
        }

        private void frmProjectmaster_FormClosing(object sender, FormClosingEventArgs e)
        {
            fnClose();
        }
    }
}
