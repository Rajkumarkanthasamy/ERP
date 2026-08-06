namespace WinFormsApp1
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// Phase 1: Login → Main Menu → connected procurement screens.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            using (frmLogin login = new frmLogin())
            {
                if (login.ShowDialog() != DialogResult.OK || !AppSession.IsAuthenticated)
                    return;
            }

            Application.Run(new frmMainMenu());
        }
    }
}
