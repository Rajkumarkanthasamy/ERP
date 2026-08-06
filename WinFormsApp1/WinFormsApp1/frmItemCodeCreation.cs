using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class frmItemCodeCreation : Form
    {
        private ItemCodeCreationDAL _dal;

        private readonly Dictionary<string, string> _categoryPrefixes = new Dictionary<string, string>
        {
            { "Capex / Fixed Asset", "FA" },
            { "Maintenance", "MNT" },
            { "Consumables", "CON" },
            { "Services", "SER" },
            { "Packing Items", "PM" },
            { "Kanban", "KAN" },
            { "Electrical & Electronics", "ELE" },
            { "Hydraulics", "HYD" },
            { "Mechanical", "MEC" },
            { "Test Lab", "TEL" },
            { "Consumables Non-Inventory", "NOI" },
            { "Project-Specific (Design)", "PRJ" },
            { "Machining Items", "MAC" },
            { "Mechanical Brought-Out Parts", "MBO" },
            { "R & D", "RND" }
        };

        private readonly Dictionary<string, List<string>> _authorizedCreators = new Dictionary<string, List<string>>
        {
            { "Capex / Fixed Asset", new List<string> { "Purchase Department" } },
            { "Maintenance", new List<string> { "Purchase Department" } },
            { "Consumables", new List<string> { "Purchase Department" } },
            { "Services", new List<string> { "Purchase Department" } },
            { "Packing Items", new List<string> { "Purchase Department" } },
            { "Kanban", new List<string> { "Purchase Department" } },
            { "Electrical & Electronics", new List<string> { "Mr. Ravi Naiker" } },
            { "Hydraulics", new List<string> { "Mr. Santosh", "Mr. Sanjay" } },
            { "Mechanical", new List<string> { "Mr. Santosh", "Mr. Kumar Ravi", "Mr. Vishal" } },
            { "Test Lab", new List<string> { "Mr. Manoj" } },
            { "Consumables Non-Inventory", new List<string> { "Purchase Department" } },
            { "Project-Specific (Design)", new List<string> { "Mr. Kumar Ravi", "Mr. Sachin" } },
            { "Machining Items", new List<string> { "Mr. Santosh", "Mr. Kumar Ravi", "Mr. Vishal" } },
            { "Mechanical Brought-Out Parts", new List<string> { "Mr. Santosh", "Mr. Kumar Ravi", "Mr. Vishal" } },
            { "R & D", new List<string> { "Purchase Department" } }
        };

        private readonly Dictionary<string, string> _approvalAuthorities = new Dictionary<string, string>
        {
            { "Capex / Fixed Asset", "Material Manager" },
            { "Maintenance", "Material Manager" },
            { "Consumables", "Material Manager" },
            { "Services", "Material Manager" },
            { "Packing Items", "Material Manager" },
            { "Kanban", "Material Manager" },
            { "Electrical & Electronics", "Operations Head" },
            { "Hydraulics", "Operations Head" },
            { "Mechanical", "Operations Head" },
            { "Test Lab", "Operations Head" },
            { "Consumables Non-Inventory", "Material Manager" },
            { "Project-Specific (Design)", "Operations Head" },
            { "Machining Items", "Operations Head" },
            { "Mechanical Brought-Out Parts", "Operations Head" },
            { "R & D", "Material Manager" }
        };

        private readonly List<string> _departments = new List<string>
        {
            "Purchase", "Maintenance", "Production", "R & D", "Test Lab",
            "Design", "Machining", "Hydraulics", "Electrical", "Stores",
            "Quality", "Project", "Packing", "HR", "Finance"
        };

        private readonly List<string> _unitsOfMeasure = new List<string>
        {
            "Nos", "Kg", "Meter", "Liter", "Set", "Box", "Roll", "Pair",
            "Sheet", "Packet", "Bundle", "Drum", "Can", "Bag", "Each"
        };

        private readonly List<string> _criticalityLevels = new List<string>
        {
            "A - High Critical", "B - Medium Critical", "C - Low Critical"
        };

        private readonly List<string> _approvalStatuses = new List<string>
        {
            "Pending", "Approved", "Rejected", "On Hold"
        };

        private readonly List<string> _emergencyApprovers = new List<string>
        {
            "Operations Head", "Material Manager"
        };

        public frmItemCodeCreation()
        {
            InitializeComponent();
            _dal = new ItemCodeCreationDAL();
        }

        private void frmItemCodeCreation_Load(object sender, EventArgs e)
        {
            if (!_dal.fnCheckConnection())
            {
                MessageBox.Show("Warning: Could not connect to database. Operating in offline mode.",
                    "Connection Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            dtpRequestDate.Value = DateTime.Now;
            dtpEffectiveDate.Value = DateTime.Now;

            LoadDepartments();
            LoadItemCategories();
            LoadUnitsOfMeasure();
            LoadCriticalityLevels();
            LoadApprovalStatuses();
            LoadEmergencyApprovers();

            txtRequestor.Text = AppSession.IsAuthenticated ? AppSession.UserName : Environment.UserName;
            txtPreparedBy.Text = "Material Manager";
            txtApprovedBy.Text = "Operations Head";
        }

        private void LoadDepartments()
        {
            cmbDepartment.Items.Clear();
            foreach (var dept in _departments) cmbDepartment.Items.Add(dept);
            cmbDepartment.SelectedIndex = -1;
        }

        private void LoadItemCategories()
        {
            cmbItemCategory.Items.Clear();
            foreach (var category in _categoryPrefixes.Keys) cmbItemCategory.Items.Add(category);
            cmbItemCategory.SelectedIndex = -1;
        }

        private void LoadUnitsOfMeasure()
        {
            cmbUnitOfMeasure.Items.Clear();
            foreach (var unit in _unitsOfMeasure) cmbUnitOfMeasure.Items.Add(unit);
            cmbUnitOfMeasure.SelectedIndex = -1;
        }

        private void LoadCriticalityLevels()
        {
            cmbCriticalityLevel.Items.Clear();
            foreach (var level in _criticalityLevels) cmbCriticalityLevel.Items.Add(level);
            cmbCriticalityLevel.SelectedIndex = -1;
        }

        private void LoadApprovalStatuses()
        {
            cmbApprovalStatus.Items.Clear();
            foreach (var status in _approvalStatuses) cmbApprovalStatus.Items.Add(status);
            cmbApprovalStatus.SelectedIndex = 0;
        }

        private void LoadEmergencyApprovers()
        {
            cmbEmergencyApproval.Items.Clear();
            foreach (var approver in _emergencyApprovers) cmbEmergencyApproval.Items.Add(approver);
            cmbEmergencyApproval.SelectedIndex = -1;
        }

        private void cmbItemCategory_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbItemCategory.SelectedItem != null)
            {
                string selectedCategory = cmbItemCategory.SelectedItem.ToString();

                if (_categoryPrefixes.ContainsKey(selectedCategory))
                {
                    string prefix = _categoryPrefixes[selectedCategory];
                    txtPrefix.Text = prefix;
                    txtCategoryCode.Text = prefix;

                    try
                    {
                        int nextSeq = _dal.GetNextSequence(prefix);
                        txtSequentialNumber.Text = nextSeq.ToString("D4");
                    }
                    catch
                    {
                        txtSequentialNumber.Text = "1001";
                    }
                }

                cmbAuthorizedCreator.Items.Clear();
                if (_authorizedCreators.ContainsKey(selectedCategory))
                {
                    foreach (var creator in _authorizedCreators[selectedCategory])
                        cmbAuthorizedCreator.Items.Add(creator);
                    cmbAuthorizedCreator.SelectedIndex = 0;
                }

                if (_approvalAuthorities.ContainsKey(selectedCategory))
                {
                    cmbApprovalAuthority.Items.Clear();
                    cmbApprovalAuthority.Items.Add(_approvalAuthorities[selectedCategory]);
                    cmbApprovalAuthority.SelectedIndex = 0;
                }
            }
        }

        private void txtItemDescription_TextChanged(object sender, EventArgs e)
        {
            txtItemDescription.Text = txtItemDescription.Text.ToUpper();
            txtItemDescription.SelectionStart = txtItemDescription.Text.Length;
        }

        private void btnGenerateCode_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtPrefix.Text))
            {
                MessageBox.Show("Please select an Item Category first.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtSequentialNumber.Text))
            {
                MessageBox.Show("Sequential number is required.", "Validation Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string prefix = txtPrefix.Text;
            string sequentialNum = txtSequentialNumber.Text.PadLeft(4, '0');
            string itemCode = prefix + "-XX-" + sequentialNum;
            txtGeneratedItemCode.Text = itemCode;

            MessageBox.Show("Item Code Generated: " + itemCode, "Success",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void chkEmergency_CheckedChanged(object sender, EventArgs e)
        {
            cmbEmergencyApproval.Enabled = chkEmergency.Checked;
            if (!chkEmergency.Checked)
                cmbEmergencyApproval.SelectedIndex = -1;
            else
                cmbEmergencyApproval.SelectedIndex = 0;
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (!ValidateForm()) return;

            if (string.IsNullOrWhiteSpace(txtGeneratedItemCode.Text))
            {
                DialogResult result = MessageBox.Show(
                    "Item code not generated yet. Generate now?",
                    "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                    btnGenerateCode_Click(sender, e);
                else
                    return;
            }

            ItemCodeRequest request = new ItemCodeRequest
            {
                RequestID = GenerateRequestID(),
                RequestDate = dtpRequestDate.Value,
                Requestor = txtRequestor.Text,
                Department = cmbDepartment.SelectedItem.ToString(),
                ItemCategory = cmbItemCategory.SelectedItem.ToString(),
                ItemDescription = txtItemDescription.Text,
                TechnicalSpec = txtTechnicalSpec.Text,
                UnitOfMeasure = cmbUnitOfMeasure.SelectedItem.ToString(),
                DrawingReference = txtDrawingReference.Text,
                CriticalityLevel = cmbCriticalityLevel.SelectedItem.ToString(),
                HSNCode = txtHSNCode.Text,
                ItemCode = txtGeneratedItemCode.Text,
                AuthorizedCreator = cmbAuthorizedCreator.SelectedItem.ToString(),
                ApprovalAuthority = cmbApprovalAuthority.SelectedItem.ToString(),
                ApprovalStatus = "Pending",
                StoreLocation = txtStoreLocation.Text,
                BinNumber = txtBinNumber.Text,
                IsEmergency = chkEmergency.Checked,
                EmergencyApprovedBy = chkEmergency.Checked ? cmbEmergencyApproval.SelectedItem?.ToString() : null
            };

            try
            {
                int newId = _dal.InsertItemCodeRequest(request);

                if (newId > 0)
                {
                    string msg = "Item Code Creation Request Submitted Successfully!\n\n" +
                        "Request ID: " + request.RequestID + "\n" +
                        "Database ID: " + newId + "\n\n" +
                        "Item Code: " + request.ItemCode + "\n" +
                        "Description: " + request.ItemDescription + "\n\n" +
                        "Data saved to database successfully.";

                    MessageBox.Show(msg, "Submission Successful",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);

                    DialogResult openApproval = MessageBox.Show(
                        "Do you want to open the Approval Dashboard?",
                        "Open Approval", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                    if (openApproval == DialogResult.Yes)
                    {
                        frmItemCodeApproval approvalForm = new frmItemCodeApproval();
                        approvalForm.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to save to database:\n" + ex.Message,
                    "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string GenerateRequestID()
        {
            return "ICCRF-" + DateTime.Now.ToString("yyyy") + "-" + DateTime.Now.ToString("MMddHHmmss");
        }

        private bool ValidateForm()
        {
            if (string.IsNullOrWhiteSpace(txtRequestor.Text))
            { txtRequestor.Focus(); MessageBox.Show("Requestor name is required."); return false; }
            if (cmbDepartment.SelectedIndex == -1)
            { cmbDepartment.Focus(); MessageBox.Show("Please select a Department."); return false; }
            if (cmbItemCategory.SelectedIndex == -1)
            { cmbItemCategory.Focus(); MessageBox.Show("Please select an Item Category."); return false; }
            if (string.IsNullOrWhiteSpace(txtItemDescription.Text))
            { txtItemDescription.Focus(); MessageBox.Show("Item Description is required."); return false; }
            if (txtItemDescription.Text.Length > 40)
            { txtItemDescription.Focus(); MessageBox.Show("Item Description must be 40 characters or less."); return false; }
            if (cmbUnitOfMeasure.SelectedIndex == -1)
            { cmbUnitOfMeasure.Focus(); MessageBox.Show("Please select a Unit of Measure."); return false; }
            if (cmbCriticalityLevel.SelectedIndex == -1)
            { cmbCriticalityLevel.Focus(); MessageBox.Show("Please select a Criticality Level."); return false; }
            if (cmbAuthorizedCreator.SelectedIndex == -1)
            { cmbAuthorizedCreator.Focus(); MessageBox.Show("Please select an Authorized Creator."); return false; }
            if (chkEmergency.Checked && cmbEmergencyApproval.SelectedIndex == -1)
            { cmbEmergencyApproval.Focus(); MessageBox.Show("Please select an Emergency Approval authority."); return false; }
            return true;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Clear all fields?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
                ClearAllFields();
        }

        private void ClearAllFields()
        {
            dtpRequestDate.Value = DateTime.Now;
            txtRequestor.Text = AppSession.IsAuthenticated ? AppSession.UserName : Environment.UserName;
            cmbDepartment.SelectedIndex = -1;
            cmbItemCategory.SelectedIndex = -1;
            txtItemDescription.Clear();
            txtTechnicalSpec.Clear();
            cmbUnitOfMeasure.SelectedIndex = -1;
            txtDrawingReference.Clear();
            cmbCriticalityLevel.SelectedIndex = -1;
            txtHSNCode.Clear();
            txtPrefix.Clear();
            txtCategoryCode.Clear();
            txtSequentialNumber.Clear();
            txtGeneratedItemCode.Clear();
            cmbAuthorizedCreator.Items.Clear();
            cmbApprovalAuthority.Items.Clear();
            cmbApprovalStatus.SelectedIndex = 0;
            txtApprovalRemarks.Clear();
            txtStoreLocation.Clear();
            txtBinNumber.Clear();
            chkEmergency.Checked = false;
            cmbEmergencyApproval.SelectedIndex = -1;
            dtpEffectiveDate.Value = DateTime.Now;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Exit?", "Confirm", MessageBoxButtons.YesNo) == DialogResult.Yes)
                this.Close();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            PrintDocument printDoc = new PrintDocument();
            printDoc.PrintPage += new PrintPageEventHandler(PrintDocument_PrintPage);
            PrintPreviewDialog previewDialog = new PrintPreviewDialog();
            previewDialog.Document = printDoc;
            previewDialog.ShowDialog();
        }

        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            Font font = new Font("Arial", 10);
            Font bold = new Font("Arial", 10, FontStyle.Bold);
            Font title = new Font("Arial", 14, FontStyle.Bold);
            float y = 50;
            float lh = font.GetHeight() + 4;

            g.DrawString("ITEM CODE CREATION REQUEST FORM (ICCRF)", title, Brushes.Black, 50, y);
            y += lh * 2;
            g.DrawString("Request Date: " + dtpRequestDate.Value.ToShortDateString(), font, Brushes.Black, 50, y);
            g.DrawString("Requestor: " + txtRequestor.Text, font, Brushes.Black, 350, y);
            y += lh * 2;
            g.DrawString("Item Code: " + txtGeneratedItemCode.Text, bold, Brushes.DarkRed, 50, y);
            y += lh * 2;
            g.DrawString("Description: " + txtItemDescription.Text, font, Brushes.Black, 50, y);
            y += lh;
            g.DrawString("Category: " + (cmbItemCategory.SelectedItem ?? "N/A"), font, Brushes.Black, 50, y);
            y += lh;
            g.DrawString("Department: " + (cmbDepartment.SelectedItem ?? "N/A"), font, Brushes.Black, 50, y);
        }
    }
}