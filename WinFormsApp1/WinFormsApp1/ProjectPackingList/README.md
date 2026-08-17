# Project Packing List — paste instructions

## Files to paste (other machine)

Replace your existing form files with:

1. `ProjectPackingList.cs`
2. `ProjectPackingList.Designer.cs`

Optional: paste `PACKING_LIST_DAL_SNIPPET.cs` into `DataAccessLayer.cs` if you prefer
MachineBOM via DAL. The form already loads MachineBOM with a direct SQL query, so the
snippet is not required for compile/run.

## What changed

| Feature | Behavior |
|--------|----------|
| Left panel | TreeView: ProjectBOM products with MachineBOM items under each (match ProjectCode + ProductNo) |
| Drag header | Product node drag moves product **+ all its machine children** |
| Drag item | Child node drag moves **only that machine item** |
| Search | Filters products/items in the left tree |
| Manual entry | Code, Name, Type (Product/Item), Qty — no temp BomID |
| Double-click | Renames description; saved to DB `ProductName` and PDF |
| Pallets | Same fields as boxes (Label, L×W×H, GW, NW); multiple allowed; saved as `ItemType=PALLET` |
| Non-box | Still ship-loose (no dims) |
| PDF | Plain tables — **no blue/grey background fills** |

## Notes

- Namespace remains `Erp_Project_With_Buttons.Project_Master`.
- If `fnMachineBOMPackingList` is missing, the form tries a fallback and may skip MachineBOM with a status message.
