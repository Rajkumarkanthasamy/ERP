
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Data;

namespace WinFormsApp1
{
    class Global
    {
        public int iStatus; //Return 1 if data successfully inserted else 0
        public int iSuccess;    //Return 1 if data successfully inserted else 0
        public double dNewValue;
        public static string sFormSelection;
        public static uint iAvailableQtyFilter;

        public const uint uINSERT = 0;
        public const uint uUPDATE = 1;
        public const uint uDELETE = 2;
        public const uint uFORM_CLOSE = 0;
        public const uint uITEM_CODE_SEARCH = 0;
        public const uint uITEM_DESCRIPTION_SEARCH = 1;
        public const uint uITEM_TYPE_SEARCH = 2;
        public const uint uITEM_STATUS_SEARCH = 3;
        public const uint uVENDOR_CODE_SEARCH = 4;
        public const uint uWONUMBER_SEARCH = 5;
        public const uint uPREPARED_BY = 6;
        public const uint uAUTHORISED_BY = 7;
        public const string sBUTTON_SAVE_TEXT_ISUPDATE = "Update";
        public const string sBUTTON_SAVE_TEXT_ISSAVE = "Save";
        public const string sCOMBOBOX_DEFAULT_TEXT = "None";
        public const string sCLOSE_MESSAGE_RESULT_YES = "Yes";
        public const string sINDENTFORM = "IndentForm";
        public const string sBOMINDENT = "BOMIndent";
        public const string sWORKORDERFORM = "WorkOrderForm";
        public const string sLogo = "C:\\Databasepath\\Bisslogo.jpg";
        public const string sLogo1 = "C:\\Databasepath\\Bisslogo1.jpg";
        public const string sLogo12 = "C:\\Databasepath\\Biss Labs Logo (1).jpg";
        public const string sInsronLogo = "C:\\Databasepath\\InstronLogo.jpg";
        public const string sTQLogo = "C:\\Databasepath\\TQLogo.jpg";
        public const string sTQBiSSLogo = "C:\\Databasepath\\TQBiSSLogo.jpg";
        public const string sTQTestLabLogo = "C:\\Databasepath\\TQTestLabLogo.jpg";
        public const string sITWLogo = "C:\\Databasepath\\ITWLogo.jpg";
        public const string sPreview = "C:\\Databasepath\\Preview.jpg";
        public const string sCompanyLogo = "C:\\Databasepath\\BiSSQuoteimage.jpg";
        public const string sInstronCompanyLogo = "C:\\Databasepath\\InstronQuoteimage.jpg";
        public const string sSTSDTSCompanyLogo = "C:\\Databasepath\\BiSSQuoteimageSTSDTS.jpg";
        public const string sCompanyName = "ITW India Private Limited - Instron India";
        public static string sFormName = string.Empty;
        private static int iAddItem;
        public string sMessageResult = string.Empty;
        public bool bDBConnection = false;
        //Change Both the values here when ERP updated
        public static double uERPVersion = 3.06;
        public const string sVersion = @"\\192.168.1.82\Documents\IT Software\ERP\ERP v3.02";

        public int fnGetAddItem         //Pass Variable from this to other forms
        {
            get
            {
                return iAddItem;
            }
            set
            {
                iAddItem = value;
            }
        }
        public uint AvailableQtyFilter         //Pass Variable from this to other forms
        {
            get
            {
                return iAvailableQtyFilter;
            }
            set
            {
                iAvailableQtyFilter = value;
            }
        }
        public string fnFormSelection
        {
            get
            {
                return sFormSelection;
            }
            set
            {
                sFormSelection = value;
            }

        }


        public void fnIsFloatingNumber(TextBox txt)
        {
            int iIndex = 0;
            double fNumberOut;
            if (!double.TryParse(txt.Text, out fNumberOut))
            {
                if (txt.TextLength == 0)
                {
                    txt.Text = txt.Text.Substring(0, 0);
                }
                else
                {
                    iIndex = 1;
                    MessageBox.Show("Numbers only allowed here", "EBM");
                    txt.Text = txt.Text.Substring(0, txt.TextLength - iIndex);
                }
                txt.SelectionStart = txt.TextLength;
                if (txt.TextLength > 0)
                    fnIsFloatingNumber(txt);
            }
        }


        public DataView RowFilter(DataTable dtItemTable, int Search, string sRowfilter)
        {
            DataView dvItemList = new DataView();
            if (dtItemTable.Rows.Count > 0)
            {
                switch (Search)
                {
                    case 0:
                        if (sRowfilter.Length > 5)
                        {
                            dvItemList = new DataView(dtItemTable);
                            dvItemList.RowFilter = "ItemCode like '%" + sRowfilter + "%'";
                        }
                        break;
                    case 1:
                        if (sRowfilter.Length > 2)
                        {
                            dvItemList = new DataView(dtItemTable);
                            dvItemList.RowFilter = "ItemDescription LIKE '%" + sRowfilter + "%'";
                        }
                        break;
                }
            }

            return dvItemList;
        }

        public void fnTextBoxValidation(TextBox txtText)
        {
            int iIndex;
            string s = string.Empty;
            if (txtText.TextLength > 0)
                s = txtText.Text.Substring((txtText.TextLength - 1), 1);
            if (s == "," || s == "'")
            {
                if (txtText.TextLength == 0)
                    txtText.Text = txtText.Text.Substring(0, 0);
                else
                {
                    iIndex = 1;
                    MessageBox.Show("Symbols ,and ' are not allowed", "Error");
                    txtText.Text = txtText.Text.Substring(0, txtText.TextLength - iIndex);
                }
                txtText.SelectionStart = txtText.TextLength;
            }
        }

        public DialogResult fnClearallMessage()
        {
            DialogResult DR = MessageBox.Show("The entered values will be cleared are you sure to continue", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            return DR;
        }



        public string fnMessageResult()
        {
            DialogResult Result = MessageBox.Show("Are you sure you want to Exit this window?", "Exit confirmation",
            MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            return Result.ToString();
        }

        public string fnMessageResultquit()
        {
            DialogResult Result = MessageBox.Show("Are you sure you want to Quit Application?", "Quit confirmation",
            MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            return Result.ToString();
        }




        public void fnClearAll(System.Windows.Forms.Control Control)
        {
            foreach (Control TabControls in Control.Controls)
            {

                switch (TabControls.GetType().ToString())
                {
                    case "System.Windows.Forms.TextBox":
                        {
                            TextBox txt = (TextBox)TabControls;
                            txt.ResetText();
                            break;
                        }
                    case "System.Windows.Forms.ComboBox":
                        {
                            ComboBox cmb = (ComboBox)TabControls;
                            if (cmb.Items.Count > 0)
                                cmb.Text = Global.sCOMBOBOX_DEFAULT_TEXT;
                            break;
                        }
                    case "System.Windows.Forms.DateTimePicker":
                        {
                            DateTimePicker dtp = (DateTimePicker)TabControls;
                            dtp.ResetText();
                            break;
                        }
                    case "System.Windows.Forms.CheckBox":
                        {
                            CheckBox check = (CheckBox)TabControls;
                            check.Checked = true;
                            break;
                        }
                    default:
                        {
                            break;
                        }
                }
            }
        }

        public TreeNode[] fnTreeFromTable(DataTable dt, string sParent, string sNode, string sName, string sImageIndex)
        {
            List<TreeNode> TreeNode = new List<TreeNode>();
            string[] sColumn = new string[4];
            sColumn[0] = sParent;
            sColumn[1] = sNode;
            sColumn[2] = sName;
            sColumn[3] = sImageIndex;
            if (!dt.Columns.Contains(sParent) && !dt.Columns.Contains(sNode) && !dt.Columns.Contains(sName) && !dt.Columns.Contains(sImageIndex))
            {
                return TreeNode.ToArray();
            }
            else
            {
                dt.DefaultView.RowFilter = "[" + sParent + "]=0";
                foreach (DataRowView DRV in dt.DefaultView)
                {
                    TreeNode.Add(this.fnReadNodes(sColumn, DRV, dt));
                }
                return TreeNode.ToArray();
            }
        }
        private TreeNode fnReadNodes(string[] sColumn, DataRowView DRV, DataTable dt)
        {
            TreeNode TN = new TreeNode();
            TN.Text = DRV[sColumn[2]].ToString();
            TN.Name = DRV[sColumn[2]].ToString();
            string s = DRV[sColumn[3]].ToString();
            int i = Convert.ToInt16(s);
            TN.StateImageIndex = i;
            fnGetAllNodesFromTable(sColumn, ref TN, dt, (int)Convert.ToInt32(DRV[sColumn[1]]));
            return TN;
        }
        private void fnGetAllNodesFromTable(string[] sColumn, ref TreeNode TN, DataTable dt, int iNodeID)
        {
            dt.DefaultView.RowFilter = "[" + sColumn[0].ToString() + "]=" + iNodeID.ToString();
            foreach (DataRowView DRV in dt.DefaultView)
            {
                TreeNode TreeNode = new TreeNode();
                TreeNode.Text = DRV[sColumn[2]].ToString();
                TreeNode.Name = DRV[sColumn[2]].ToString();
                string s = DRV[sColumn[3]].ToString();
                TreeNode.StateImageIndex = Convert.ToInt32(s);
                TN.Nodes.Add(TreeNode);
                fnGetAllNodesFromTable(sColumn, ref TreeNode, dt, (int)Convert.ToInt32(DRV[sColumn[1]]));
            }
        }
    }
}
