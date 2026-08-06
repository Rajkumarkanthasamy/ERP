namespace WinFormsApp1
{
    /// <summary>
    /// Logged-in user session shared across all forms.
    /// </summary>
    public static class AppSession
    {
        public static bool IsAuthenticated { get; private set; }
        public static string UserName { get; private set; } = Environment.UserName;
        public static string LoginId { get; private set; } = "";
        public static string Department { get; private set; } = "";
        public static string Role { get; private set; } = "User";
        public static bool CanApprovePR { get; private set; }
        public static bool CanGeneratePO { get; private set; }
        public static bool CanApprovePO { get; private set; }

        public static void SignIn(
            string loginId,
            string userName,
            string department,
            string role,
            bool canApprovePR,
            bool canGeneratePO,
            bool canApprovePO)
        {
            LoginId = loginId ?? "";
            UserName = string.IsNullOrWhiteSpace(userName) ? Environment.UserName : userName;
            Department = department ?? "";
            Role = string.IsNullOrWhiteSpace(role) ? "User" : role;
            CanApprovePR = canApprovePR;
            CanGeneratePO = canGeneratePO;
            CanApprovePO = canApprovePO;
            IsAuthenticated = true;
        }

        public static void SignOut()
        {
            IsAuthenticated = false;
            LoginId = "";
            UserName = Environment.UserName;
            Department = "";
            Role = "User";
            CanApprovePR = false;
            CanGeneratePO = false;
            CanApprovePO = false;
        }

        public static string DisplayLabel =>
            string.IsNullOrWhiteSpace(Role)
                ? $"User: {UserName}"
                : $"User: {UserName} | Role: {Role}";
    }
}
