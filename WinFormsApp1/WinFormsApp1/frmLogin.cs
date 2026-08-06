using System;
using System.Drawing;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class frmLogin : Form
    {
        private readonly AuthDAL _auth = new AuthDAL();

        public frmLogin()
        {
            InitializeComponent();
        }

        private void frmLogin_Load(object sender, EventArgs e)
        {
            txtUserName.Text = Environment.UserName;
            txtPassword.Focus();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string user = txtUserName.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrWhiteSpace(user))
            {
                MessageBox.Show("Enter username.", "Login", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUserName.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Enter password.", "Login", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPassword.Focus();
                return;
            }

            btnLogin.Enabled = false;
            try
            {
                if (_auth.Authenticate(user, password, out string error))
                {
                    DialogResult = DialogResult.OK;
                    Close();
                    return;
                }

                var fallback = MessageBox.Show(
                    error + "\n\nContinue with local session for development?",
                    "Login",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (fallback == DialogResult.Yes)
                {
                    _auth.SignInAsLocalFallback(user);
                    DialogResult = DialogResult.OK;
                    Close();
                }
            }
            finally
            {
                btnLogin.Enabled = true;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                btnLogin_Click(sender, e);
            }
        }
    }
}
