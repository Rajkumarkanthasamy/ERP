using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class frmPriceVariance : Form
    {
        private readonly PriceIntelligenceDAL _dal = new PriceIntelligenceDAL();
        private DataTable _variance = new DataTable();
        private string _prNumber = "";

        public frmPriceVariance()
        {
            InitializeComponent();
        }

        public frmPriceVariance(string prNumber) : this()
        {
            _prNumber = prNumber;
        }

        private void frmPriceVariance_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(_prNumber))
                txtPRNumber.Text = _prNumber;
            if (!string.IsNullOrWhiteSpace(txtPRNumber.Text))
                LoadVariance();
        }

        private void btnLoad_Click(object sender, EventArgs e) => LoadVariance();

        private void LoadVariance()
        {
            _prNumber = txtPRNumber.Text.Trim();
            if (string.IsNullOrEmpty(_prNumber))
            {
                MessageBox.Show("Enter a PR number.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _variance = _dal.GetPriceVarianceForPR(_prNumber);
            dgvVariance.DataSource = _variance;
            dgvVariance.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvVariance.RowHeadersVisible = false;
            dgvVariance.AllowUserToAddRows = false;
            dgvVariance.ReadOnly = true;

            int alerts = 0;
            foreach (DataRow row in _variance.Rows)
            {
                string flag = row["Flag"]?.ToString() ?? "";
                if (flag.StartsWith("ALERT") || flag.StartsWith("Watch"))
                    alerts++;
            }
            lblSummary.Text = $"{_variance.Rows.Count} item(s) · {alerts} variance flag(s)";
            ColorRows();
        }

        private void ColorRows()
        {
            foreach (DataGridViewRow row in dgvVariance.Rows)
            {
                string flag = row.Cells["Flag"].Value?.ToString() ?? "";
                if (flag.StartsWith("ALERT"))
                    row.DefaultCellStyle.BackColor = Color.FromArgb(255, 230, 230);
                else if (flag.StartsWith("Watch"))
                    row.DefaultCellStyle.BackColor = Color.FromArgb(255, 245, 220);
                else if (flag == "OK")
                    row.DefaultCellStyle.BackColor = Color.FromArgb(230, 245, 235);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            if (_variance.Rows.Count == 0)
            {
                MessageBox.Show("Load a PR first.", "Print", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            ProcurementPrintHelper.PrintPriceVariance(_prNumber, _variance);
        }

        private void btnClose_Click(object sender, EventArgs e) => Close();
    }
}
