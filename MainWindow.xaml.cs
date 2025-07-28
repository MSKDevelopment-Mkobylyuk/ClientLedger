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

            MainFrame.Navigate(new LoginPage());
        }

        public void EnableNavigation()
        {
            DashboardBtn.IsEnabled = true;
            CustomersBtn.IsEnabled = true;
            WorkEntriesBtn.IsEnabled = true;
            SettingsBtn.IsEnabled = true;
        }

        private void Dashboard_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new DashboardPage());
        }

        private void Customers_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new CustomersPage());
        }

        private void WorkEntries_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new WorkEntriesPage());
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new SettingsPage());
        }
    }
}
