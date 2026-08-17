// ============================================================================
// PASTE into your DataAccessLayer.cs (near fnProjectBOMList)
// Required by ProjectPackingList MachineBOM tree load.
// ============================================================================

public DataSet fnMachineBOMPackingList(string sProjectCode)
{
    try
    {
        SQLDataset = new DataSet();
        SQLCon.Open();
        SQLCmd = new SqlCommand(@"
SELECT
    mb.ID,
    mb.ProjectBOMCode,
    mb.ProductNo,
    mb.ItemName,
    ISNULL(im.ItemDescription, mb.ItemName) AS ItemDescription,
    mb.Quantity,
    mb.ProjectCode,
    mb.ProductType,
    mb.MfgPartNo,
    mb.Make,
    mb.Specification
FROM [ERP_Database].[dbo].[MachineBOM] mb
LEFT JOIN [ERP_Database].[dbo].[ItemMaster] im
    ON im.ItemCode = mb.ItemName
WHERE mb.ProjectCode = '" + sProjectCode + @"'
ORDER BY mb.ProductNo, mb.ItemName;", SQLCon);
        SQLDadpr = new SqlDataAdapter(SQLCmd);
        SQLDadpr.Fill(SQLDataset);
        SQLCon.Close();
        return SQLDataset;
    }
    catch (Exception ex)
    {
        MessageBox.Show(ex.Message);
        SQLCon.Close();
        return null;
    }
}
