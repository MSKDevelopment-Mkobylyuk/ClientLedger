using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ClientLedger.Data;
using Newtonsoft.Json;

namespace ClientLedger
{
    public partial class CustomersPage : Page
    {
        private ObservableCollection<Customer> customers = new ObservableCollection<Customer>();

        public CustomersPage()
        {
            InitializeComponent();
            CustomersDataGrid.ItemsSource = customers;

            LoadCustomers();
        }

        private void LoadCustomers()
        {
            customers.Clear();

            foreach (var c in CustomerRepository.GetAll())
            {
                if (c.AgreementType == AgreementType.Retainer)
                {
                    var categories = RetainerCategoryRepository.GetByCustomerId(c.Id);
                    c.RetainerCategories = categories ?? new List<RetainerCategory>();
                }
                else
                {
                    c.RetainerCategories = new List<RetainerCategory>();
                }

                customers.Add(c);
            }
        }

        private void AddCustomerBtn_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CustomerDialog();
            if (dialog.ShowDialog() == true)
            {
                LoadCustomers();
            }
        }

        private void EditCustomerBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CustomersDataGrid.SelectedItem is Customer selected)
            {
                var dialog = new CustomerDialog(selected);
                if (dialog.ShowDialog() == true)
                {
                    CustomerRepository.Update(dialog.Customer);
                    LoadCustomers();
                }
            }
            else
            {
                MessageBox.Show(
                    "Please select a customer to edit.",
                    "No Selection",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
            }
        }

        private void DeleteCustomerBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CustomersDataGrid.SelectedItem is Customer selected)
            {
                if (
                    MessageBox.Show(
                        $"Delete customer '{selected.Name}'?",
                        "Confirm Delete",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question
                    ) == MessageBoxResult.Yes
                )
                {
                    CustomerRepository.Delete(selected.Id);
                    LoadCustomers();
                }
            }
            else
            {
                MessageBox.Show(
                    "Please select a customer to delete.",
                    "No Selection",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
            }
        }
    }
}
