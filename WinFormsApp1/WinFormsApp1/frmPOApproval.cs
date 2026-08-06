using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class frmPOApproval : Form
    {
        private readonly POApprovalDAL _dal = new POApprovalDAL();
        private string _currentUser;
        private int _selectedPOID;

        public frmPOApproval()
        {
            InitializeComponent();
            _currentUser = AppSession.IsAuthenticated ? AppSession.UserName : Environment.UserName;
        }

        private void frmPOApproval_Load(object sender, EventArgs e)
        {
            lblUser.Text = AppSession.DisplayLabel;
            LoadPOList();
        }

        private void LoadPOList()
        {
            DataTable dt = _dal.GetPOListForApproval();
            dgvPOs.DataSource = dt;
            FormatGrid();
            lblCount.Text = "Pending PO lines: " + dt.Rows.Count;

            decimal total = 0;
            foreach (DataRow row in dt.Rows)
            {
                if (row["Amount"] != DBNull.Value)
                    total += Convert.ToDecimal(row["Amount"]);
            }
            lblTotal.Text = "Pending value: ₹ " + total.ToString("N2");
            ClearDetail();
        }

        private void FormatGrid()
        {
            dgvPOs.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvPOs.RowHeadersVisible = false;
            dgvPOs.AllowUserToAddRows = false;
            dgvPOs.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvPOs.MultiSelect = true;
            dgvPOs.ReadOnly = true;

            HideCol("PMApproved");
            HideCol("MHApproved");
            if (dgvPOs.Columns.Contains("ID"))
                dgvPOs.Columns["ID"].Visible = false;
            if (dgvPOs.Columns.Contains("DBOMNo"))
                dgvPOs.Columns["DBOMNo"].HeaderText = "PR Ref";
            if (dgvPOs.Columns.Contains("RequariedQty"))
                dgvPOs.Columns["RequariedQty"].HeaderText = "Qty";
            if (dgvPOs.Columns.Contains("UnitPrice"))
                dgvPOs.Columns["UnitPrice"].HeaderText = "Unit Price";
        }

        private void HideCol(string name)
        {
            if (dgvPOs.Columns.Contains(name))
                dgvPOs.Columns[name].Visible = false;
        }

        private void dgvPOs_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvPOs.CurrentRow == null || dgvPOs.CurrentRow.DataBoundItem is not DataRowView view)
            {
                ClearDetail();
                return;
            }

            DataRow row = view.Row;
            _selectedPOID = Convert.ToInt32(row["ID"]);
            lblPOId.Text = "PO ID: " + _selectedPOID;
            lblPRRef.Text = "PR Ref: " + row["DBOMNo"];
            lblProject.Text = "Project: " + row["ProjectCode"];
            lblVendor.Text = "Vendor: " + row["VendorCode"];
            lblItem.Text = "Item: " + row["ItemCode"];
            lblAmount.Text = "Amount: ₹ " + Convert.ToDecimal(row["Amount"]).ToString("N2");

            bool pm = row["PMApproved"] != DBNull.Value && Convert.ToBoolean(row["PMApproved"]);
            bool mh = row["MHApproved"] != DBNull.Value && Convert.ToBoolean(row["MHApproved"]);
            lblPMStatus.Text = pm ? "PM: Approved (" + row["PMName"] + ")" : "PM: Pending";
            lblMHStatus.Text = mh ? "MH: Approved (" + row["MHName"] + ")" : "MH: Pending";
            lblPMStatus.ForeColor = pm ? Color.ForestGreen : Color.DarkOrange;
            lblMHStatus.ForeColor = mh ? Color.ForestGreen : Color.DarkOrange;
        }

        private void ClearDetail()
        {
            _selectedPOID = 0;
            lblPOId.Text = "PO ID: -";
            lblPRRef.Text = "PR Ref: -";
            lblProject.Text = "Project: -";
            lblVendor.Text = "Vendor: -";
            lblItem.Text = "Item: -";
            lblAmount.Text = "Amount: -";
            lblPMStatus.Text = "PM: -";
            lblMHStatus.Text = "MH: -";
        }

        private List<int> GetSelectedPOIDs()
        {
            HashSet<int> ids = new HashSet<int>();
            foreach (DataGridViewRow row in dgvPOs.SelectedRows)
            {
                if (row.DataBoundItem is DataRowView view)
                    ids.Add(Convert.ToInt32(view.Row["ID"]));
            }
            if (ids.Count == 0 && _selectedPOID > 0)
                ids.Add(_selectedPOID);
            return ids.ToList();
        }

        private void btnPMApprove_Click(object sender, EventArgs e)
        {
            var ids = GetSelectedPOIDs();
            if (ids.Count == 0)
            {
                MessageBox.Show("Select one or more PO lines.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int ok = 0;
            foreach (int id in ids)
            {
                if (_dal.PMApprovePO(id, _currentUser, txtRemarks.Text.Trim()))
                    ok++;
            }
            MessageBox.Show($"PM approved {ok} of {ids.Count} line(s).", "PM Approval", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadPOList();
        }

        private void btnMHApprove_Click(object sender, EventArgs e)
        {
            var ids = GetSelectedPOIDs();
            if (ids.Count == 0)
            {
                MessageBox.Show("Select one or more PO lines.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int ok = 0;
            foreach (int id in ids)
            {
                if (_dal.MHApprovePO(id, _currentUser, txtRemarks.Text.Trim()))
                    ok++;
            }
            MessageBox.Show($"MH approved {ok} of {ids.Count} line(s).", "MH Approval", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadPOList();
        }

        private void btnFinalApprove_Click(object sender, EventArgs e)
        {
            var ids = GetSelectedPOIDs();
            if (ids.Count == 0)
            {
                MessageBox.Show("Select one or more PO lines.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("Final-approve selected PO line(s)?", "Confirm",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
                return;

            int ok = 0;
            foreach (int id in ids)
            {
                if (_dal.ApprovePO(id, _currentUser, txtRemarks.Text.Trim()))
                    ok++;
            }
            MessageBox.Show($"Final approved {ok} of {ids.Count} line(s).", "PO Approval", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadPOList();
        }

        private void btnReject_Click(object sender, EventArgs e)
        {
            var ids = GetSelectedPOIDs();
            if (ids.Count == 0)
            {
                MessageBox.Show("Select one or more PO lines.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string reason = txtRemarks.Text.Trim();
            if (string.IsNullOrEmpty(reason))
            {
                MessageBox.Show("Enter a rejection reason in Remarks.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int ok = 0;
            foreach (int id in ids)
            {
                if (_dal.RejectPO(id, _currentUser, reason))
                    ok++;
            }
            MessageBox.Show($"Rejected {ok} of {ids.Count} line(s).", "PO Reject", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadPOList();
        }

        private void btnRefresh_Click(object sender, EventArgs e) => LoadPOList();
        private void btnClose_Click(object sender, EventArgs e) => Close();
    }
}
