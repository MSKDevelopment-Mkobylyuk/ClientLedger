using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ClientLedger.Data;

namespace ClientLedger
{
    public partial class WorkEntriesPage : Page
    {
        private List<WorkEntry> _workEntries = new List<WorkEntry>();

        public WorkEntriesPage()
        {
            InitializeComponent();
            Loaded += WorkEntriesPage_Loaded;
            WorkEntriesDataGrid.SelectionChanged += WorkEntriesDataGrid_SelectionChanged;
            CustomerComboBox.SelectionChanged += CustomerComboBox_SelectionChanged;
        }

        private void WorkEntriesPage_Loaded(object sender, RoutedEventArgs e)
        {
            LoadCustomersIntoComboBox();
            ClearForm(clearCustomer: true); // Clear everything on first load
        }

        private void LoadCustomersIntoComboBox()
        {
            var customers = CustomerRepository.GetAll();
            CustomerComboBox.ItemsSource = customers;
            CustomerComboBox.DisplayMemberPath = "Name";
            CustomerComboBox.SelectedValuePath = "Id";
        }

        private void LoadWorkEntries(int? customerId = null)
        {
            if (customerId.HasValue)
            {
                // Load entries only for selected customer
                _workEntries = WorkEntryRepository.GetByCustomerId(customerId.Value);
            }
            else
            {
                // Load all entries if no customer specified
                _workEntries = WorkEntryRepository.GetAll();
            }

            WorkEntriesDataGrid.ItemsSource = null;
            WorkEntriesDataGrid.ItemsSource = _workEntries;
        }

        private void ClearForm(bool clearCustomer = false)
        {
            if (clearCustomer)
                CustomerComboBox.SelectedIndex = -1;

            WorkDatePicker.SelectedDate = DateTime.Now;
            HoursTextBox.Clear();
            DescriptionTextBox.Clear();
            WorkEntriesDataGrid.SelectedIndex = -1;
        }

        private void CustomerComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CustomerComboBox.SelectedItem is Customer selectedCustomer)
            {
                // Load work entries for selected customer
                LoadWorkEntries(selectedCustomer.Id);

                if (selectedCustomer.AgreementType == AgreementType.Retainer)
                {
                    CategoryPanel.Visibility = Visibility.Visible;
                    var categories = RetainerCategoryRepository.GetByCustomerId(
                        selectedCustomer.Id
                    );
                    CategoryComboBox.ItemsSource = categories;
                    if (categories.Any())
                        CategoryComboBox.SelectedIndex = 0;
                }
                else
                {
                    CategoryPanel.Visibility = Visibility.Collapsed;
                    CategoryComboBox.ItemsSource = null;
                }
            }
        }

        private void AddEntry_Click(object sender, RoutedEventArgs e)
        {
            if (CustomerComboBox.SelectedValue == null)
            {
                MessageBox.Show("Please select a customer.");
                return;
            }

            var selectedCustomer = CustomerRepository.GetById((int)CustomerComboBox.SelectedValue);

            if (
                selectedCustomer.AgreementType == AgreementType.Retainer
                && CategoryComboBox.SelectedValue == null
            )
            {
                MessageBox.Show("Please select a category for this retainer customer.");
                return;
            }

            if (!double.TryParse(HoursTextBox.Text, out double hours) || hours <= 0)
            {
                MessageBox.Show("Please enter a valid positive number for hours worked.");
                return;
            }
            if (WorkDatePicker.SelectedDate == null)
            {
                MessageBox.Show("Please select a work date.");
                return;
            }

            var selectedCategory = CategoryComboBox.SelectedItem as RetainerCategory;

            var newEntry = new WorkEntry
            {
                CustomerId = (int)CustomerComboBox.SelectedValue,
                WorkDate = WorkDatePicker.SelectedDate.Value,
                Hours = hours,
                Description = DescriptionTextBox.Text,
                CategoryId =
                    selectedCustomer.AgreementType == AgreementType.Retainer
                        ? (int?)CategoryComboBox.SelectedValue
                        : null,
                CategoryName = selectedCategory?.CategoryName,
            };

            WorkEntryRepository.Add(newEntry);

            LoadWorkEntries(newEntry.CustomerId);
            ClearForm(clearCustomer: false);
        }

        private void EditEntry_Click(object sender, RoutedEventArgs e)
        {
            var selectedEntry = WorkEntriesDataGrid.SelectedItem as WorkEntry;
            if (selectedEntry == null)
            {
                MessageBox.Show("Please select a work entry to edit.");
                return;
            }

            if (CustomerComboBox.SelectedValue == null)
            {
                MessageBox.Show("Please select a customer.");
                return;
            }
            double hours;
            if (!double.TryParse(HoursTextBox.Text, out hours) || hours <= 0)
            {
                MessageBox.Show("Please enter a valid positive number for hours worked.");
                return;
            }
            if (WorkDatePicker.SelectedDate == null)
            {
                MessageBox.Show("Please select a work date.");
                return;
            }

            selectedEntry.CustomerId = (int)CustomerComboBox.SelectedValue;
            selectedEntry.WorkDate = WorkDatePicker.SelectedDate.Value;
            selectedEntry.Hours = hours;
            selectedEntry.Description = DescriptionTextBox.Text;

            WorkEntryRepository.Update(selectedEntry);
            LoadWorkEntries();
            ClearForm();
        }

        private void DeleteEntry_Click(object sender, RoutedEventArgs e)
        {
            var selectedEntry = WorkEntriesDataGrid.SelectedItem as WorkEntry;
            if (selectedEntry == null)
            {
                MessageBox.Show("Please select a work entry to delete.");
                return;
            }

            var result = MessageBox.Show(
                "Are you sure you want to delete this work entry?",
                "Confirm Delete",
                MessageBoxButton.YesNo
            );
            if (result == MessageBoxResult.Yes)
            {
                WorkEntryRepository.Delete(selectedEntry.Id);
                LoadWorkEntries();
                ClearForm();
            }
        }

        private void WorkEntriesDataGrid_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e
        )
        {
            var selectedEntry = WorkEntriesDataGrid.SelectedItem as WorkEntry;
            if (selectedEntry != null)
            {
                var customer = CustomerRepository.GetById(selectedEntry.CustomerId);
                CustomerComboBox.SelectedValue = customer.Id;

                if (customer.AgreementType == AgreementType.Retainer)
                {
                    CategoryPanel.Visibility = Visibility.Visible;
                    var categories = RetainerCategoryRepository.GetByCustomerId(customer.Id);
                    CategoryComboBox.ItemsSource = categories;

                    if (categories.Any())
                    {
                        // Preselect the correct category from the entry
                        CategoryComboBox.SelectedValue = selectedEntry.CategoryId;
                    }
                }
                else
                {
                    CategoryPanel.Visibility = Visibility.Collapsed;
                    CategoryComboBox.ItemsSource = null;
                }

                WorkDatePicker.SelectedDate = selectedEntry.WorkDate;
                HoursTextBox.Text = selectedEntry.Hours.ToString();
                DescriptionTextBox.Text = selectedEntry.Description;
            }
            else
            {
                ClearForm();
            }
        }
    }
}
