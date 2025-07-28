using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClientLedger
{
    public class RetainerCategory
    {
        [Key]
        public int Id { get; set; } // Unique identifier for the category

        public int CustomerId { get; set; } // Foreign key to the Customer

        [Required]
        public string CategoryName { get; set; }

        [Range(0, double.MaxValue)]
        public decimal MaxHours { get; set; }

        [Range(0, double.MaxValue)]
        public decimal BaseRate { get; set; }

        public decimal UsedHours { get; set; } // Optional: you can track used hours here

        [NotMapped]
        public decimal TotalRemaining
        {
            get
            {
                // Can be interpreted as remaining hours or dollar amount depending on context
                var remaining = MaxHours - UsedHours;
                return remaining < 0 ? 0 : remaining;
            }
        }

        public override string ToString()
        {
            return $"{CategoryName}: {MaxHours} hrs @ {BaseRate:C} (Used: {UsedHours}, Remaining: {TotalRemaining})";
        }
    }
}
