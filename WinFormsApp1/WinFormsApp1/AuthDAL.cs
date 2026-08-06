using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public class AuthDAL
    {
        public bool Authenticate(string userName, string password, out string errorMessage)
        {
            errorMessage = "";
            try
            {
                using (SqlConnection con = new SqlConnection(AppConfig.ConnectionString))
                {
                    con.Open();
                    string query = @"
                        SELECT TOP 1
                            LoginId,
                            UserName,
                            ISNULL(Department, '') AS Department,
                            ISNULL(AuthorisedPreson, '') AS AuthorisedPreson,
                            ISNULL(Active, 0) AS Active,
                            ISNULL(PurchaseManager, 0) AS PurchaseManager,
                            ISNULL(POWOGenerate, 0) AS POWOGenerate,
                            ISNULL(GeneralManager, 0) AS GeneralManager,
                            ISNULL(ManufacturingHead, 0) AS ManufacturingHead,
                            ISNULL(ProductionManager, 0) AS ProductionManager
                        FROM Login
                        WHERE UserName = @UserName
                          AND LoginPassword = @Password";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@UserName", userName);
                        cmd.Parameters.AddWithValue("@Password", password);

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (!reader.Read())
                            {
                                errorMessage = "Invalid username or password.";
                                return false;
                            }

                            bool active = Convert.ToBoolean(reader["Active"]);
                            if (!active)
                            {
                                errorMessage = "This user account is inactive.";
                                return false;
                            }

                            string loginId = reader["LoginId"]?.ToString() ?? userName;
                            string name = reader["UserName"]?.ToString() ?? userName;
                            string department = reader["Department"]?.ToString() ?? "";
                            bool purchaseManager = Convert.ToBoolean(reader["PurchaseManager"]);
                            bool poGenerate = Convert.ToBoolean(reader["POWOGenerate"]);
                            bool generalManager = Convert.ToBoolean(reader["GeneralManager"]);
                            bool manufacturingHead = Convert.ToBoolean(reader["ManufacturingHead"]);
                            bool productionManager = Convert.ToBoolean(reader["ProductionManager"]);

                            string role = "User";
                            if (generalManager) role = "General Manager";
                            else if (manufacturingHead) role = "Manufacturing Head";
                            else if (purchaseManager) role = "Purchase Manager";
                            else if (productionManager) role = "Production Manager";
                            else if (!string.IsNullOrWhiteSpace(reader["AuthorisedPreson"]?.ToString()))
                                role = "Authorised Person";

                            bool canApprovePR = purchaseManager || generalManager || manufacturingHead;
                            bool canGeneratePO = poGenerate || purchaseManager || generalManager;
                            bool canApprovePO = generalManager || manufacturingHead || purchaseManager;

                            AppSession.SignIn(loginId, name, department, role, canApprovePR, canGeneratePO, canApprovePO);
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = "Login failed: " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Dev/offline fallback when Login table is unavailable.
        /// </summary>
        public void SignInAsLocalFallback(string userName)
        {
            AppSession.SignIn(
                userName,
                userName,
                "Local",
                "Material Manager",
                canApprovePR: true,
                canGeneratePO: true,
                canApprovePO: true);
        }
    }
}
