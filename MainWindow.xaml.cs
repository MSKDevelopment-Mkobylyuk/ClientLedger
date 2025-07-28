using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using ClientLedger.Helpers;

namespace ClientLedger
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            string localDbPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "ClientLedger",
                "data.db"
            );

            Directory.CreateDirectory(Path.GetDirectoryName(localDbPath));

            DatabaseHelper.InitializeDatabase(localDbPath);

            // 🔐 Add a default admin user IF it doesn't exist
            if (!DatabaseHelper.UserExists("admin"))
            {
                DatabaseHelper.AddUser("admin", "admin");
            }

            MainFrame.Navigate(new LoginPage());
        }

        private void Dashboard_Click(object sender, RoutedEventArgs e) { }

        private void Customers_Click(object sender, RoutedEventArgs e) { }

        private void WorkEntries_Click(object sender, RoutedEventArgs e) { }

        private void Settings_Click(object sender, RoutedEventArgs e) { }
    }
}
