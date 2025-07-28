using System;
using System.Collections.Generic;
using System.Linq;

namespace ClientLedger
{
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ContactName { get; set; }

        // Total allowed max hours (for Retainer)
        public decimal TotalAllowedHours
        {
            get
            {
                if (RetainerCategories == null)
                    return 0;
                return RetainerCategories.Sum(c => c.MaxHours);
            }
        }

        // Hours worked from work entries (should be set when loading)
        public decimal WorkEntriesHours { get; set; } = 0;

        public AgreementType AgreementType { get; set; }
        public decimal MonthlyCost { get; set; }
        public decimal BaseRate { get; set; }

        public List<RetainerCategory> RetainerCategories { get; set; } =
            new List<RetainerCategory>();

        public string CategorySummary
        {
            get
            {
                if (AgreementType == AgreementType.Retainer)
                {
                    return "Retainer";
                }
                else if (AgreementType == AgreementType.ServiceAgreement)
                {
                    return "Service Agreement";
                }
                else
                {
                    return "Unknown";
                }
            }
        }

        public string TotalDisplay
        {
            get
            {
                if (AgreementType == AgreementType.Retainer)
                {
                    return $"{TotalAllowedHours} hrs";
                }
                else if (AgreementType == AgreementType.ServiceAgreement)
                {
                    return MonthlyCost.ToString("C");
                }
                else
                {
                    return "N/A";
                }
            }
        }

        public string TotalRemainingDisplay
        {
            get
            {
                // Leaving empty for now
                return string.Empty;
            }
        }
    }

    public enum AgreementType
    {
        ServiceAgreement = 0,
        Retainer = 1,
    }
}
