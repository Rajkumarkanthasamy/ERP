namespace WinFormsApp1
{
    /// <summary>
    /// Canonical Purchase Request status values used across the app.
    /// </summary>
    public static class PRStatus
    {
        public const string Pending = "Pending";
        public const string Approved = "Approved";
        public const string Rejected = "Rejected";
        public const string OnHold = "On Hold";
        public const string Clubbed = "Clubbed";
        public const string PartiallyConverted = "Partially Converted";
        public const string FullyConverted = "Fully Converted";

        public static readonly string[] All =
        {
            Pending, Approved, Rejected, OnHold, Clubbed, PartiallyConverted, FullyConverted
        };
    }

    public static class PRLineStatus
    {
        public const string Pending = "Pending";
        public const string ConvertedToPO = "Converted to PO";
        public const string Cancelled = "Cancelled";
    }
}
