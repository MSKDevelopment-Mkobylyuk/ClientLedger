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
        public decimal WorkEntriesHours { get; set; } = 0;
        public AgreementType AgreementType { get; set; }
        public decimal MonthlyCost { get; set; }
        public decimal BaseRate { get; set; }

        public List<RetainerCategory> RetainerCategories { get; set; } =
            new List<RetainerCategory>();
        public decimal? TotalRemaining { get; set; }
        public decimal? TotalRemainingHours { get; set; }

        public decimal TotalAllowedHours
        {
            get
            {
                if (RetainerCategories == null)
                    return 0;
                return RetainerCategories.Sum(c => c.MaxHours);
            }
        }

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
                if (AgreementType == AgreementType.ServiceAgreement && TotalRemaining.HasValue)
                {
                    return TotalRemaining.Value.ToString("C");
                }
                else if (AgreementType == AgreementType.Retainer && TotalRemainingHours.HasValue)
                {
                    return $"{TotalRemainingHours.Value} hrs";
                }
                else
                {
                    return "N/A";
                }
            }
        }
    }

    public enum AgreementType
    {
        ServiceAgreement = 0,
        Retainer = 1,
    }
}
