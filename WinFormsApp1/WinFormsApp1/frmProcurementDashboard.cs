using System;
using System.Drawing;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class frmProcurementDashboard : Form
    {
        private readonly ProcurementDashboardDAL _dal = new ProcurementDashboardDAL();

        public frmProcurementDashboard()
        {
            InitializeComponent();
        }

        private void frmProcurementDashboard_Load(object sender, EventArgs e)
        {
            lblUser.Text = AppSession.DisplayLabel;
            RefreshDashboard();
        }

        private void btnRefresh_Click(object sender, EventArgs e) => RefreshDashboard();

        private void RefreshDashboard()
        {
            flpMetrics.Controls.Clear();
            var summary = _dal.GetInboxSummary();
            Color[] accents =
            {
                Color.FromArgb(20, 90, 140),
                Color.FromArgb(40, 120, 90),
                Color.FromArgb(150, 90, 40),
                Color.FromArgb(90, 70, 140)
            };

            int i = 0;
            foreach (System.Data.DataRow row in summary.Rows)
            {
                flpMetrics.Controls.Add(CreateMetricCard(
                    row["Metric"].ToString() ?? "",
                    Convert.ToInt32(row["Count"]),
                    Convert.ToDecimal(row["Amount"]),
                    accents[i % accents.Length]));
                i++;
            }

            dgvAging.DataSource = _dal.GetAgingPRs();
            FormatGrid(dgvAging);
            dgvActivity.DataSource = _dal.GetRecentActivity();
            FormatGrid(dgvActivity);
        }

        private Panel CreateMetricCard(string title, int count, decimal amount, Color accent)
        {
            Panel card = new Panel
            {
                Width = 220,
                Height = 110,
                Margin = new Padding(8),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            Panel stripe = new Panel
            {
                Dock = DockStyle.Left,
                Width = 6,
                BackColor = accent
            };

            Label lblTitle = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(80, 90, 100),
                Location = new Point(16, 12),
                AutoSize = true,
                MaximumSize = new Size(190, 40)
            };

            Label lblCount = new Label
            {
                Text = count.ToString("N0"),
                Font = new Font("Segoe UI Semibold", 22F, FontStyle.Bold),
                ForeColor = accent,
                Location = new Point(16, 48),
                AutoSize = true
            };

            Label lblAmt = new Label
            {
                Text = "₹ " + amount.ToString("N0"),
                Font = new Font("Segoe UI", 9F),
                ForeColor = Color.FromArgb(100, 110, 120),
                Location = new Point(16, 84),
                AutoSize = true
            };

            card.Controls.Add(lblAmt);
            card.Controls.Add(lblCount);
            card.Controls.Add(lblTitle);
            card.Controls.Add(stripe);
            return card;
        }

        private static void FormatGrid(DataGridView grid)
        {
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.RowHeadersVisible = false;
            grid.AllowUserToAddRows = false;
            grid.ReadOnly = true;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        private void btnOpenPRApproval_Click(object sender, EventArgs e)
        {
            new frmPurchaseRequestApproval().Show(this);
        }

        private void btnOpenPOApproval_Click(object sender, EventArgs e)
        {
            new frmPOApproval().Show(this);
        }

        private void btnOpenWorkspace_Click(object sender, EventArgs e)
        {
            new frmPRtoPOWorkspace().Show(this);
        }

        private void btnClose_Click(object sender, EventArgs e) => Close();
    }
}
