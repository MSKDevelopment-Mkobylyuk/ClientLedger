using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ClientLedger.Helpers;

namespace ClientLedger
{
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameBox.Text.Trim();
            string password = PasswordBox.Password;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                ShowError("Please enter both username and password.");
                return;
            }

            bool isValid = DatabaseHelper.ValidateUser(username, password);

            if (isValid)
            {
                // Clear error and navigate
                ErrorMessage.Visibility = Visibility.Collapsed;
                NavigationService?.Navigate(new DashboardPage());
            }
            else
            {
                ShowError("Invalid username or password.");
            }
        }

        // Inside your LoginPage class
        private void Input_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Login_Click(sender, new RoutedEventArgs());
            }
        }

        private void ShowError(string message)
        {
            ErrorMessage.Text = message;
            ErrorMessage.Visibility = Visibility.Visible;
        }
    }
}
