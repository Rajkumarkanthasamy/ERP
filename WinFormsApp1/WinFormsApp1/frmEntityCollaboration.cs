using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace WinFormsApp1
{
    /// <summary>
    /// Comments + attachments for a PR / PO / GRN entity.
    /// </summary>
    public partial class frmEntityCollaboration : Form
    {
        private readonly CommentAttachmentDAL _dal = new CommentAttachmentDAL();
        private readonly string _entityType;
        private readonly string _entityRef;
        private readonly string _user;

        public frmEntityCollaboration(string entityType, string entityRef)
        {
            _entityType = entityType;
            _entityRef = entityRef;
            _user = AppSession.IsAuthenticated ? AppSession.UserName : Environment.UserName;
            InitializeComponent();
        }

        private void frmEntityCollaboration_Load(object sender, EventArgs e)
        {
            Text = $"{_entityType} Collaboration — {_entityRef}";
            lblHeader.Text = $"{_entityType}: {_entityRef}";
            Reload();
        }

        private void Reload()
        {
            dgvComments.DataSource = _dal.GetComments(_entityType, _entityRef);
            FormatGrid(dgvComments);
            dgvFiles.DataSource = _dal.GetAttachments(_entityType, _entityRef);
            FormatGrid(dgvFiles);
        }

        private static void FormatGrid(DataGridView g)
        {
            g.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            g.RowHeadersVisible = false;
            g.AllowUserToAddRows = false;
            g.ReadOnly = true;
            g.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }

        private void btnAddComment_Click(object sender, EventArgs e)
        {
            string text = txtComment.Text.Trim();
            if (string.IsNullOrEmpty(text))
            {
                MessageBox.Show("Enter a comment.", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (_dal.AddComment(_entityType, _entityRef, text, _user))
            {
                txtComment.Clear();
                Reload();
            }
        }

        private void btnAttach_Click(object sender, EventArgs e)
        {
            using OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = "Attach file";
            dlg.Filter = "All files|*.*|PDF|*.pdf|Images|*.png;*.jpg;*.jpeg|Excel|*.xlsx;*.xls";
            if (dlg.ShowDialog(this) != DialogResult.OK) return;

            try
            {
                string root = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    "ERP_Procurement_Attachments",
                    _entityType,
                    Sanitize(_entityRef));
                Directory.CreateDirectory(root);
                string dest = Path.Combine(root, Path.GetFileName(dlg.FileName));
                File.Copy(dlg.FileName, dest, overwrite: true);
                FileInfo fi = new FileInfo(dest);
                decimal kb = fi.Length / 1024m;

                if (_dal.AddAttachment(_entityType, _entityRef, fi.Name, dest, kb, _user, null))
                    Reload();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Attach failed:\n" + ex.Message, "Attachments", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            if (dgvFiles.CurrentRow?.DataBoundItem is not System.Data.DataRowView view) return;
            string path = view.Row["FilePath"]?.ToString() ?? "";
            if (File.Exists(path))
                Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
            else
                MessageBox.Show("File not found:\n" + path, "Attachments", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            ProcurementPrintHelper.PrintEntityPack(_entityType, _entityRef, _dal);
        }

        private void btnClose_Click(object sender, EventArgs e) => Close();

        private static string Sanitize(string value)
        {
            foreach (char c in Path.GetInvalidFileNameChars())
                value = value.Replace(c, '_');
            return value;
        }
    }
}
