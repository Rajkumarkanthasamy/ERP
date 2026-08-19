# Project Packing List — paste instructions

## Files to paste (other machine)

Replace your existing form files with:

1. `ProjectPackingList.cs`  (or merge into your `packingUpdatedList.cs`)
2. `ProjectPackingList.Designer.cs`  (or merge into your designer)

Optional: paste `PACKING_LIST_DAL_SNIPPET.cs` into `DataAccessLayer.cs` if you prefer
MachineBOM via DAL.

## What changed

| Feature | Behavior |
|--------|----------|
| Left panel | TreeView: ProjectBOM products with MachineBOM items under each |
| **Select All** | Checks all visible BOM nodes; drag any checked node to move **all checked** |
| **Checkboxes** | Tick products/items; parent check cascades to children |
| Drag to box/pallet | Drop on box list or box items (or use **> To Box**) |
| Drag to Non-Box | Drop on Non-Box list (or use **Non-Box** button) |
| **Dim unit** | Dropdown: inches, cm, mm, feet, m — saved in `BoxDimensions` as `L.. x W.. x H.. (unit)` |
| Search / Manual / Rename / Pallets / PDF | Same as before |

## How to use multi-select drag

1. Click **Select All** (or tick individual checkboxes).
2. Select a target box/pallet (or create one).
3. Drag from the BOM tree onto the box list / box items, **or** onto Non-Box.
4. Or use **> To Box** / **Non-Box** buttons — they also use the checked selection.

## Notes

- Repo namespace: `Erp_Project_With_Buttons.Project_Master` / class `ProjectPackingList`.
- Your local paste may use `Erp_Project_With_Buttons.PackingList` / `packingUpdatedList` — keep your namespace/class name; only merge the new methods + designer controls (`btnSelectAllBom`, `btnClearBomSelection`, `cmbDimUnit`, `tvBOM.CheckBoxes = true`).
