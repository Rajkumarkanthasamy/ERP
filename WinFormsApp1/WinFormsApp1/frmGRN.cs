using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class frmGRN : Form
    {
        private readonly GRNDAL _dal = new GRNDAL();
        private readonly string _user;
        private DataTable _openPOs = new DataTable();

        public frmGRN()
        {
            InitializeComponent();
            _user = AppSession.IsAuthenticated ? AppSession.UserName : Environment.UserName;
        }

        private void frmGRN_Load(object sender, EventArgs e)
        {
            lblUser.Text = AppSession.DisplayLabel;
            txtReceivedBy.Text = _user;
            txtGRNNumber.Text = _dal.GetNextGRNNumber();
            LoadOpenPOs();
            LoadHistory();
        }

        private void LoadOpenPOs()
        {
            _openPOs = _dal.GetApprovedPOsForReceipt();
            dgvOpenPO.DataSource = _openPOs;
            dgvOpenPO.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvOpenPO.RowHeadersVisible = false;
            dgvOpenPO.AllowUserToAddRows = false;
            dgvOpenPO.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvOpenPO.MultiSelect = true;
            dgvOpenPO.ReadOnly = true;
            if (dgvOpenPO.Columns.Contains("ID"))
                dgvOpenPO.Columns["ID"].Visible = false;
            lblOpenCount.Text = "Open approved PO lines: " + _openPOs.Rows.Count;
        }

        private void LoadHistory()
        {
            dgvHistory.DataSource = _dal.GetRecentGRNs();
            dgvHistory.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvHistory.RowHeadersVisible = false;
            dgvHistory.AllowUserToAddRows = false;
            dgvHistory.ReadOnly = true;
        }

        private void btnAddSelected_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dgvOpenPO.SelectedRows)
            {
                if (row.DataBoundItem is not DataRowView view) continue;
                DataRow src = view.Row;
                string poId = src["ID"].ToString() ?? "";
                bool exists = false;
                foreach (DataGridViewRow existing in dgvReceive.Rows)
                {
                    if (existing.Cells["colPOID"].Value?.ToString() == poId)
                    {
                        exists = true;
                        break;
                    }
                }
                if (exists) continue;

                decimal remaining = Convert.ToDecimal(src["RemainingQty"]);
                dgvReceive.Rows.Add(
                    false,
                    src["ID"],
                    src["PORef"],
                    src["ItemCode"],
                    src["OrderedQty"],
                    remaining,
                    remaining, // default receive = remaining
                    src["UnitPrice"],
                    (remaining * Convert.ToDecimal(src["UnitPrice"])).ToString("N2"),
                    src["UOM"],
                    src["VendorCode"],
                    src["ProjectCode"],
                    "");
            }
            UpdateReceiveTotal();
        }

        private void dgvReceive_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (dgvReceive.Columns[e.ColumnIndex].Name != "colRecvQty") return;

            DataGridViewRow row = dgvReceive.Rows[e.RowIndex];
            decimal.TryParse(row.Cells["colRecvQty"].Value?.ToString(), out decimal recv);
            decimal.TryParse(row.Cells["colRemaining"].Value?.ToString(), out decimal rem);
            decimal.TryParse(row.Cells["colUnitPrice"].Value?.ToString(), out decimal price);

            if (recv > rem)
            {
                MessageBox.Show("Received qty cannot exceed remaining qty.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                row.Cells["colRecvQty"].Value = rem;
                recv = rem;
            }
            if (recv < 0)
            {
                row.Cells["colRecvQty"].Value = 0;
                recv = 0;
            }
            row.Cells["colAmount"].Value = (recv * price).ToString("N2");
            UpdateReceiveTotal();
        }

        private void UpdateReceiveTotal()
        {
            decimal total = 0;
            int lines = 0;
            foreach (DataGridViewRow row in dgvReceive.Rows)
            {
                decimal.TryParse(row.Cells["colRecvQty"].Value?.ToString(), out decimal qty);
                decimal.TryParse(row.Cells["colUnitPrice"].Value?.ToString(), out decimal price);
                if (qty > 0)
                {
                    lines++;
                    total += qty * price;
                }
            }
            lblReceiveTotal.Text = $"Receive lines: {lines} · Total: ₹ {total:N2}";
        }

        private void btnPostGRN_Click(object sender, EventArgs e)
        {
            DataTable lines = new DataTable();
            lines.Columns.Add("POID", typeof(int));
            lines.Columns.Add("ItemCode");
            lines.Columns.Add("OrderedQty", typeof(decimal));
            lines.Columns.Add("ReceivedQty", typeof(decimal));
            lines.Columns.Add("UnitPrice", typeof(decimal));
            lines.Columns.Add("Amount", typeof(decimal));
            lines.Columns.Add("UOM");
            lines.Columns.Add("Remarks");

            string? poRef = null;
            string? vendor = null;
            string? project = null;

            foreach (DataGridViewRow row in dgvReceive.Rows)
            {
                decimal.TryParse(row.Cells["colRecvQty"].Value?.ToString(), out decimal qty);
                if (qty <= 0) continue;

                decimal.TryParse(row.Cells["colOrdered"].Value?.ToString(), out decimal ordered);
                decimal.TryParse(row.Cells["colUnitPrice"].Value?.ToString(), out decimal price);
                lines.Rows.Add(
                    Convert.ToInt32(row.Cells["colPOID"].Value),
                    row.Cells["colItem"].Value?.ToString(),
                    ordered,
                    qty,
                    price,
                    qty * price,
                    row.Cells["colUOM"].Value?.ToString(),
                    row.Cells["colRemarks"].Value?.ToString());

                poRef ??= row.Cells["colPORef"].Value?.ToString();
                vendor ??= row.Cells["colVendor"].Value?.ToString();
                project ??= row.Cells["colProject"].Value?.ToString();
            }

            if (lines.Rows.Count == 0)
            {
                MessageBox.Show("Add lines and enter received quantities.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string grn = _dal.CreateGRN(
                poRef ?? "",
                vendor ?? "",
                project ?? "",
                txtReceivedBy.Text.Trim(),
                txtRemarks.Text.Trim(),
                lines,
                out string error);

            if (string.IsNullOrEmpty(grn))
            {
                MessageBox.Show("GRN failed:\n" + error + "\n\nEnsure Phase3_Schema_Alignment.sql was run.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            new CommentAttachmentDAL().AddComment("GRN", grn,
                $"Goods received against PO/PR ref {poRef}. Lines={lines.Rows.Count}",
                _user);

            MessageBox.Show($"GRN posted: {grn}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            dgvReceive.Rows.Clear();
            txtGRNNumber.Text = _dal.GetNextGRNNumber();
            LoadOpenPOs();
            LoadHistory();
            UpdateReceiveTotal();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            dgvReceive.Rows.Clear();
            UpdateReceiveTotal();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadOpenPOs();
            LoadHistory();
            txtGRNNumber.Text = _dal.GetNextGRNNumber();
        }

        private void btnCollaborate_Click(object sender, EventArgs e)
        {
            if (dgvHistory.CurrentRow?.DataBoundItem is DataRowView view)
            {
                string grn = view.Row["GRNNumber"]?.ToString() ?? "";
                if (!string.IsNullOrEmpty(grn))
                    new frmEntityCollaboration("GRN", grn).ShowDialog(this);
            }
        }

        private void btnClose_Click(object sender, EventArgs e) => Close();
    }
}
