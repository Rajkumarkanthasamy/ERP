// =============================================================================
// Paste helpers for packingUpdatedList (local form) — Select All + Dim Unit
// =============================================================================
// 1) Designer: set tvBOM.CheckBoxes = true
// 2) Add buttons btnSelectAllBom / btnClearBomSelection (wire Click handlers)
// 3) Replace lblDimsUnit text "(inches)" with ComboBox cmbDimUnit (DropDownList)
// 4) On PackingBox add: public string DimUnit { get; set; } = "inches";
//    and append " (" + DimUnit + ")" in Dimensions
// 5) Copy these methods into the form class (rename class if needed)
// =============================================================================

/*
private static readonly string[] DimUnitOptions = { "inches", "cm", "mm", "feet", "m" };
private bool _suppressCheck = false;

private void InitDimUnitCombo()
{
    cmbDimUnit.Items.Clear();
    cmbDimUnit.Items.AddRange(DimUnitOptions);
    cmbDimUnit.SelectedIndex = 0;
}

// Call InitDimUnitCombo() from Load.

private IEnumerable<TreeNode> EnumerateNodes(TreeNodeCollection nodes)
{
    foreach (TreeNode n in nodes)
    {
        yield return n;
        foreach (var c in EnumerateNodes(n.Nodes))
            yield return c;
    }
}

private void SetAllBomChecked(bool check)
{
    _suppressCheck = true;
    tvBOM.BeginUpdate();
    try
    {
        foreach (var n in EnumerateNodes(tvBOM.Nodes))
            n.Checked = check;
    }
    finally
    {
        tvBOM.EndUpdate();
        _suppressCheck = false;
    }
}

private void btnSelectAllBom_Click(object sender, EventArgs e) => SetAllBomChecked(true);
private void btnClearBomSelection_Click(object sender, EventArgs e) => SetAllBomChecked(false);

private void tvBOM_AfterCheck(object sender, TreeViewEventArgs e)
{
    if (_suppressCheck || e.Node == null) return;
    _suppressCheck = true;
    try
    {
        foreach (TreeNode child in EnumerateNodes(e.Node.Nodes))
            child.Checked = e.Node.Checked;
        if (e.Node.Parent != null)
        {
            bool all = true;
            foreach (TreeNode sib in e.Node.Parent.Nodes)
                if (!sib.Checked) { all = false; break; }
            e.Node.Parent.Checked = all;
        }
    }
    finally { _suppressCheck = false; }
}

// Replace GetSelectedBomItems so checked nodes win over SelectedNode.
// Replace tvBOM_ItemDrag to auto-check the dragged node when nothing is checked.
// On Apply / load box: read/write box.DimUnit via cmbDimUnit.
// ParseDimensions: also extract trailing " (unit)".
*/
