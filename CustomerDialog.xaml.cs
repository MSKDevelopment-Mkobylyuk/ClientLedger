using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using ClientLedger.Data;
using Newtonsoft.Json;

namespace ClientLedger
{
    public partial class CustomerDialog : Window
    {
        public Customer Customer { get; private set; }

        // Observable collection for retainer categories binding
        public ObservableCollection<RetainerCategory> RetainerCategories { get; set; } =
            new ObservableCollection<RetainerCategory>();

        public CustomerDialog()
        {
            InitializeComponent();

            RetainerCategoryRepository.EnsureTable();

            Customer = new Customer();
            AgreementTypeComboBox.SelectedIndex = 0; // Default to Service Agreement

            RetainerCategoriesList.ItemsSource = RetainerCategories;

            // Optional: Handle AgreementType change event to update panels and RetainerCategories visibility
            AgreementTypeComboBox.SelectionChanged += AgreementTypeComboBox_SelectionChanged;
        }

        public CustomerDialog(Customer customer)
            : this()
        {
            // Deep copy customer properties (including MonthlyCost!)
            Customer = new Customer
            {
                Id = customer.Id,
                Name = customer.Name,
                ContactName = customer.ContactName ?? "",
                AgreementType = customer.AgreementType,
                BaseRate = customer.BaseRate,
                MonthlyCost = customer.MonthlyCost,
            };

            BusinessNameBox.Text = Customer.Name;
            ContactNameBox.Text = Customer.ContactName;

            AgreementTypeComboBox.SelectedIndex = (int)Customer.AgreementType;

            // Load RetainerCategories if Retainer type
            if (Customer.AgreementType == AgreementType.Retainer)
            {
                var categories = RetainerCategoryRepository.GetByCustomerId(Customer.Id);
                RetainerCategories.Clear();

                if (categories != null)
                {
                    foreach (var c in categories)
                    {
                        RetainerCategories.Add(c);
                    }
                }
            }
            else
            {
                RetainerCategories.Clear();
            }

            UpdateAgreementTypePanels();

            // Set MonthlyCost and BaseRate textboxes accordingly
            MonthlyCostBox.Text = Customer.MonthlyCost.ToString(CultureInfo.InvariantCulture);
            BaseRateBox.Text = Customer.BaseRate.ToString(CultureInfo.InvariantCulture);
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Save_Click started");

            if (string.IsNullOrWhiteSpace(BusinessNameBox.Text))
            {
                MessageBox.Show("Please enter a business name.");
                return;
            }

            if (AgreementTypeComboBox.SelectedIndex < 0)
            {
                MessageBox.Show("Please select an agreement type.");
                return;
            }

            Customer.Name = BusinessNameBox.Text.Trim();
            Customer.ContactName = ContactNameBox.Text.Trim();
            Customer.AgreementType = (AgreementType)AgreementTypeComboBox.SelectedIndex;

            if (Customer.AgreementType == AgreementType.ServiceAgreement)
            {
                // For ServiceAgreement: MonthlyCost is required
                if (
                    !decimal.TryParse(MonthlyCostBox.Text, out decimal monthlyCost)
                    || monthlyCost < 0
                )
                {
                    MessageBox.Show("Please enter a valid positive monthly cost.");
                    return;
                }
                Customer.MonthlyCost = monthlyCost;

                // BaseRate is optional or 0 here
                if (!decimal.TryParse(BaseRateBox.Text, out decimal baseRate) || baseRate < 0)
                {
                    baseRate = 0;
                }
                Customer.BaseRate = baseRate;
            }
            else if (Customer.AgreementType == AgreementType.Retainer)
            {
                // For Retainer: validate MaxHours in each category BEFORE saving

                if (RetainerCategories == null || RetainerCategories.Count == 0)
                {
                    MessageBox.Show("Please add at least one retainer category.");
                    return;
                }

                foreach (var category in RetainerCategories)
                {
                    if (category.MaxHours <= 0)
                    {
                        MessageBox.Show(
                            $"Category '{category.CategoryName}' must have MaxHours greater than zero."
                        );
                        return;
                    }
                }

                // BaseRate and MonthlyCost are ignored or set zero for Retainer
                Customer.BaseRate = 0;
                Customer.MonthlyCost = 0;
            }
            else
            {
                // If you have other agreement types, handle here...
                Customer.BaseRate = 0;
                Customer.MonthlyCost = 0;
            }

            // Add or update customer
            if (Customer.Id == 0)
            {
                System.Diagnostics.Debug.WriteLine("Calling CustomerRepository.Add...");
                Customer.Id = CustomerRepository.Add(Customer);
                System.Diagnostics.Debug.WriteLine($"Added customer with Id: {Customer.Id}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine(
                    $"Calling CustomerRepository.Update for Id: {Customer.Id}"
                );
                CustomerRepository.Update(Customer);
            }

            // Retainer-specific categories save
            if (Customer.AgreementType == AgreementType.Retainer)
            {
                RetainerCategoryRepository.DeleteByCustomerId(Customer.Id);

                foreach (var category in RetainerCategories)
                {
                    category.CustomerId = Customer.Id;
                    RetainerCategoryRepository.Add(Customer.Id, category);
                }
            }

            System.Diagnostics.Debug.WriteLine("Save_Click finished successfully");
            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void AgreementTypeComboBox_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e
        )
        {
            UpdateAgreementTypePanels();

            // If switched to Retainer, ensure RetainerCategories are ready
            if (
                AgreementTypeComboBox.SelectedIndex == (int)AgreementType.Retainer
                && RetainerCategories.Count == 0
            )
            {
                RetainerCategories.Clear();
            }
        }

        private void UpdateAgreementTypePanels()
        {
            if (AgreementTypeComboBox.SelectedIndex == (int)AgreementType.ServiceAgreement)
            {
                BaseRatePanel.Visibility = Visibility.Visible;
                RetainerCategoriesPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                BaseRatePanel.Visibility = Visibility.Collapsed;
                RetainerCategoriesPanel.Visibility = Visibility.Visible;
            }
        }

        private void AddCategory_Click(object sender, RoutedEventArgs e)
        {
            RetainerCategories.Add(new RetainerCategory { CategoryName = "", MaxHours = 0 });
        }

        private void RemoveCategory_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement fe && fe.Tag is RetainerCategory category)
            {
                RetainerCategories.Remove(category);
            }
        }
    }
}
