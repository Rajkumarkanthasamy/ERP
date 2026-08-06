namespace WinFormsApp1
{
    /// <summary>
    /// Central connection settings for Phase 1.
    /// Update ConnectionString for your SQL Server environment.
    /// </summary>
    public static class AppConfig
    {
        public static string ConnectionString { get; set; } =
            "Data Source=GTKA064W111\\SQLEXPRESS01;Initial Catalog=ERP_Database; User ID=sa;Password=Bangalore@560058";

        /// <summary>Company PR amount limit (12 Lakh).</summary>
        public const decimal PR_AMOUNT_LIMIT = 1200000m;
    }
}
