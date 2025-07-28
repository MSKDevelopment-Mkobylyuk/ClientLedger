using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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

            // Set up AppData directory
            string appDataDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "ClientLedger"
            );
            Directory.CreateDirectory(appDataDir);

            // Use your custom DB names here
            string userDbPath = Path.Combine(appDataDir, "MSK_User_mst.db"); // for users/authentication
            string custDbPath = Path.Combine(appDataDir, "MSK_Cust_mst.db"); // for customers/financial data

            // Initialize both databases
            MSKUserHelper.InitializeDatabase(userDbPath); // User DB
            MSKCustHelper.InitializeDatabase(custDbPath); // Customer DB

            // Ensure tables for ledger (customer etc.)
            CustomerRepository.EnsureTable();
            WorkEntryRepository.EnsureTable();

            // Add default admin user to MSK_User_mst.db if it doesn't exist
            if (!MSKUserHelper.UserExists("admin"))
            {
                MSKUserHelper.AddUser("admin", "admin");
            }
        }
    }
}
