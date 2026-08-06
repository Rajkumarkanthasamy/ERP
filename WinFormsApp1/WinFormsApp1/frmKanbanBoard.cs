using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class frmKanbanBoard : Form
    {
        private readonly KanbanDAL _dal = new KanbanDAL();
        private readonly string _user;
        private DataTable _cards = new DataTable();

        private static readonly string[] Columns =
        {
            PRStatus.Pending,
            PRStatus.OnHold,
            PRStatus.Approved,
            PRStatus.PartiallyConverted,
            PRStatus.FullyConverted,
            "PO Pending Approval",
            "PO Approved"
        };

        private static readonly Color[] ColumnColors =
        {
            Color.FromArgb(230, 240, 250),
            Color.FromArgb(255, 243, 224),
            Color.FromArgb(230, 245, 235),
            Color.FromArgb(235, 235, 255),
            Color.FromArgb(220, 245, 245),
            Color.FromArgb(255, 235, 235),
            Color.FromArgb(225, 245, 225)
        };

        public frmKanbanBoard()
        {
            InitializeComponent();
            _user = AppSession.IsAuthenticated ? AppSession.UserName : Environment.UserName;
        }

        private void frmKanbanBoard_Load(object sender, EventArgs e)
        {
            lblUser.Text = AppSession.DisplayLabel;
            BuildBoard();
            RefreshBoard();
        }

        private void BuildBoard()
        {
            flpBoard.Controls.Clear();
            for (int i = 0; i < Columns.Length; i++)
            {
                Panel col = CreateColumn(Columns[i], ColumnColors[i]);
                flpBoard.Controls.Add(col);
            }
        }

        private Panel CreateColumn(string title, Color back)
        {
            Panel col = new Panel
            {
                Width = 210,
                Height = 520,
                Margin = new Padding(8),
                BackColor = back,
                BorderStyle = BorderStyle.FixedSingle,
                Tag = title,
                AllowDrop = true
            };
            col.DragEnter += Column_DragEnter;
            col.DragDrop += Column_DragDrop;

            Label hdr = new Label
            {
                Text = title,
                Dock = DockStyle.Top,
                Height = 36,
                Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.FromArgb(20, 55, 90),
                ForeColor = Color.White
            };

            FlowLayoutPanel cards = new FlowLayoutPanel
            {
                Name = "cards",
                Dock = DockStyle.Fill,
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Padding = new Padding(6),
                AllowDrop = true,
                Tag = title
            };
            cards.DragEnter += Column_DragEnter;
            cards.DragDrop += Column_DragDrop;

            col.Controls.Add(cards);
            col.Controls.Add(hdr);
            return col;
        }

        private void RefreshBoard()
        {
            _cards = _dal.GetPipelineCards();
            Dictionary<string, FlowLayoutPanel> map = new Dictionary<string, FlowLayoutPanel>();
            foreach (Control c in flpBoard.Controls)
            {
                if (c is Panel col)
                {
                    FlowLayoutPanel? cards = col.Controls.OfType<FlowLayoutPanel>().FirstOrDefault();
                    if (cards != null)
                    {
                        cards.Controls.Clear();
                        map[col.Tag?.ToString() ?? ""] = cards;
                    }
                }
            }

            foreach (DataRow row in _cards.Rows)
            {
                string stage = NormalizeStage(row["Stage"]?.ToString() ?? "");
                if (!map.ContainsKey(stage))
                    continue;
                map[stage].Controls.Add(CreateCard(row));
            }

            lblCount.Text = "Cards: " + _cards.Rows.Count;
        }

        private static string NormalizeStage(string stage)
        {
            if (string.Equals(stage, "Approved", StringComparison.OrdinalIgnoreCase))
                return PRStatus.Approved;
            return stage;
        }

        private Label CreateCard(DataRow row)
        {
            string refNo = row["RefNo"]?.ToString() ?? "";
            string type = row["CardType"]?.ToString() ?? "";
            string vendor = row["Vendor"]?.ToString() ?? "";
            string project = row["ProjectCode"]?.ToString() ?? "";
            decimal amount = row["Amount"] != DBNull.Value ? Convert.ToDecimal(row["Amount"]) : 0m;
            int age = row["AgeDays"] != DBNull.Value ? Convert.ToInt32(row["AgeDays"]) : 0;

            Label card = new Label
            {
                Width = 180,
                Height = 88,
                Margin = new Padding(4),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(6),
                Font = new Font("Segoe UI", 8.5F),
                Text = $"{type}: {refNo}\n{project}\n{vendor}\n₹ {amount:N0} · {age}d",
                Tag = row,
                Cursor = Cursors.Hand
            };

            if (age >= 7)
                card.BackColor = Color.FromArgb(255, 245, 230);
            if (age >= 14)
                card.BackColor = Color.FromArgb(255, 230, 230);

            card.MouseDown += Card_MouseDown;
            card.DoubleClick += Card_DoubleClick;
            return card;
        }

        private void Card_MouseDown(object? sender, MouseEventArgs e)
        {
            if (sender is not Label card || e.Button != MouseButtons.Left) return;
            card.DoDragDrop(card, DragDropEffects.Move);
        }

        private void Column_DragEnter(object? sender, DragEventArgs e)
        {
            if (e.Data?.GetDataPresent(typeof(Label)) == true)
                e.Effect = DragDropEffects.Move;
        }

        private void Column_DragDrop(object? sender, DragEventArgs e)
        {
            if (e.Data?.GetData(typeof(Label)) is not Label card) return;
            if (card.Tag is not DataRow row) return;

            string targetStage = "";
            if (sender is FlowLayoutPanel flp)
                targetStage = flp.Tag?.ToString() ?? "";
            else if (sender is Panel panel)
                targetStage = panel.Tag?.ToString() ?? "";

            string type = row["CardType"]?.ToString() ?? "";
            string refNo = row["RefNo"]?.ToString() ?? "";
            string current = row["Stage"]?.ToString() ?? "";

            if (string.Equals(current, targetStage, StringComparison.OrdinalIgnoreCase))
                return;

            if (type != "PR")
            {
                MessageBox.Show("PO cards are moved via PO Approval / GRN screens.", "Kanban",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!_dal.MovePRStage(refNo, targetStage, _user, out string error))
            {
                MessageBox.Show(error, "Kanban move blocked", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            new CommentAttachmentDAL().AddComment("PR", refNo,
                $"Kanban move: {current} → {targetStage}", _user);

            RefreshBoard();
        }

        private void Card_DoubleClick(object? sender, EventArgs e)
        {
            if (sender is not Label card || card.Tag is not DataRow row) return;
            string type = row["CardType"]?.ToString() ?? "";
            string refNo = row["RefNo"]?.ToString() ?? "";
            string refId = row["RefId"]?.ToString() ?? refNo;

            if (type == "PR")
            {
                using (frmEntityCollaboration dlg = new frmEntityCollaboration("PR", refNo))
                    dlg.ShowDialog(this);
            }
            else
            {
                using (frmEntityCollaboration dlg = new frmEntityCollaboration("PO", refId))
                    dlg.ShowDialog(this);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e) => RefreshBoard();
        private void btnClose_Click(object sender, EventArgs e) => Close();
    }
}
