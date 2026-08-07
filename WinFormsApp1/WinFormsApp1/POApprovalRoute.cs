namespace WinFormsApp1
{
    /// <summary>
    /// PO approval route result — .NET Framework 4.5 compatible (no System.ValueTuple).
    /// Thresholds use Amount + GST:
    /// &lt;= 50,000 PC final; &gt; 50,000 to 2,50,000 Vivek G/OM; &gt; 2,50,000 GM.
    /// </summary>
    public class POApprovalRoute
    {
        public string Tier;
        public string NextApprover;
        public int PCCase;
        public int OMCase;

        public POApprovalRoute()
        {
            Tier = "GM_REQUIRED";
            NextApprover = "GM (after OM)";
            PCCase = 10;
            OMCase = 17;
        }
    }
}
