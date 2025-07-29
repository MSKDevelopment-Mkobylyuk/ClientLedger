using System;
using System.IO;
using System.Windows;
using ClientLedger.Data;
using ClientLedger.Helpers;

namespace ClientLedger
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            SetupAppDirectoriesAndDatabases();
        }

        private void SetupAppDirectoriesAndDatabases()
        {
            string appDataDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "ClientLedger"
            );
            Directory.CreateDirectory(appDataDir);

            string userDbPath = Path.Combine(appDataDir, "MSK_User_mst.db");
            string custDbPath = Path.Combine(appDataDir, "MSK_Cust_mst.db");

            MSKUserHelper.InitializeDatabase(userDbPath);
            MSKCustHelper.InitializeDatabase(custDbPath);

            CustomerRepository.EnsureTable();
            WorkEntryRepository.EnsureTable();

            if (!MSKUserHelper.UserExists("admin"))
            {
                MSKUserHelper.AddUser("admin", "admin");
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            // No cloud upload needed here now.
        }
    }
}
