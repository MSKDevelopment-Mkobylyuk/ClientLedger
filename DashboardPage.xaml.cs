using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ClientLedger.Data;

namespace ClientLedger
{
    public partial class DashboardPage : Page
    {
        public DashboardPage()
        {
            InitializeComponent();
            LoadDashboardStats();
        }

        private void LoadDashboardStats()
        {
            var customers = CustomerRepository.GetAll();
            var workEntries = WorkEntryRepository.GetAll();

            // Total Customers
            TotalCustomersText.Text = customers.Count.ToString();

            // Total Service Agreements
            var serviceAgreements = customers
                .Where(c => c.AgreementType == AgreementType.ServiceAgreement)
                .ToList();
            TotalServiceAgreementsText.Text = serviceAgreements.Count.ToString();

            // Total Retainers
            var retainers = customers
                .Where(c => c.AgreementType == AgreementType.Retainer)
                .ToList();
            TotalRetainersText.Text = retainers.Count.ToString();

            // Total Hours Worked
            var totalHours = workEntries.Sum(e => e.Hours);
            TotalHoursWorkedText.Text = totalHours.ToString("0.##");

            // Total Work Entries
            TotalWorkEntriesText.Text = workEntries.Count.ToString();
        }
    }
}
