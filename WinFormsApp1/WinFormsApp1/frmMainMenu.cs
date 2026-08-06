using System;
using System.Drawing;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class frmMainMenu : Form
    {
        public frmMainMenu()
        {
            InitializeComponent();
        }

        private void frmMainMenu_Load(object sender, EventArgs e)
        {
            lblUser.Text = AppSession.DisplayLabel;
            lblFlow.Text = "Phase 3 flow: Dashboard → Kanban → PR → PO → Approval → GRN · Comments · Price variance";
        }

        private void OpenChild(Form form)
        {
            form.StartPosition = FormStartPosition.CenterParent;
            form.Show(this);
        }

        private void btnDashboard_Click(object sender, EventArgs e) => OpenChild(new frmProcurementDashboard());
        private void btnKanban_Click(object sender, EventArgs e) => OpenChild(new frmKanbanBoard());
        private void btnPRGeneration_Click(object sender, EventArgs e) => OpenChild(new frmPurchaseRequestGeneration());
        private void btnPRApproval_Click(object sender, EventArgs e) => OpenChild(new frmPurchaseRequestApproval());
        private void btnPRClubbing_Click(object sender, EventArgs e) => OpenChild(new frmPRClubbing());
        private void btnPRtoPO_Click(object sender, EventArgs e) => OpenChild(new frmPRtoPOConversion_new());
        private void btnWorkspace_Click(object sender, EventArgs e) => OpenChild(new frmPRtoPOWorkspace());
        private void btnPOApproval_Click(object sender, EventArgs e) => OpenChild(new frmPOApproval());
        private void btnGRN_Click(object sender, EventArgs e) => OpenChild(new frmGRN());
        private void btnPriceVariance_Click(object sender, EventArgs e) => OpenChild(new frmPriceVariance());
        private void btnItemCodeCreation_Click(object sender, EventArgs e) => OpenChild(new frmItemCodeCreation());
        private void btnItemCodeApproval_Click(object sender, EventArgs e) => OpenChild(new frmItemCodeApproval());

        private void btnLogout_Click(object sender, EventArgs e)
        {
            AppSession.SignOut();
            Close();
        }

        private void btnExit_Click(object sender, EventArgs e) => Application.Exit();
    }
}
