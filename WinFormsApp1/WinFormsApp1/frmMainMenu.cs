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
            lblFlow.Text = "Flow: Item Code → PR Generation → PR Approval → PR Clubbing → PR to PO";
        }

        private void OpenChild(Form form)
        {
            form.StartPosition = FormStartPosition.CenterParent;
            form.Show(this);
        }

        private void btnPRGeneration_Click(object sender, EventArgs e)
        {
            OpenChild(new frmPurchaseRequestGeneration());
        }

        private void btnPRApproval_Click(object sender, EventArgs e)
        {
            OpenChild(new frmPurchaseRequestApproval());
        }

        private void btnPRClubbing_Click(object sender, EventArgs e)
        {
            OpenChild(new frmPRClubbing());
        }

        private void btnPRtoPO_Click(object sender, EventArgs e)
        {
            OpenChild(new frmPRtoPOConversion_new());
        }

        private void btnItemCodeCreation_Click(object sender, EventArgs e)
        {
            OpenChild(new frmItemCodeCreation());
        }

        private void btnItemCodeApproval_Click(object sender, EventArgs e)
        {
            OpenChild(new frmItemCodeApproval());
        }

        private void btnLogout_Click(object sender, EventArgs e)
        {
            AppSession.SignOut();
            Close();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
