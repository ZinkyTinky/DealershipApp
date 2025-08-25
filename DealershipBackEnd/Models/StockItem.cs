using System;
using System.Collections.Generic;

namespace DealershipBackEnd.Models
{
    public class StockItem
    {
        public int Id { get; set; }

        public string RegNo { get; set; } = string.Empty; // Vehicle registration number
        public string Make { get; set; } = string.Empty;  // Manufacturer
        public string Model { get; set; } = string.Empty; // Model description
        public int ModelYear { get; set; }
        public int KMS { get; set; }                      // Current kilometre reading
        public string Colour { get; set; } = string.Empty;
        public string VIN { get; set; } = string.Empty;   // Vehicle Identification Number

        public decimal RetailPrice { get; set; }
        public decimal CostPrice { get; set; }

        public DateTime DTCreated { get; set; } = DateTime.UtcNow;
        public DateTime? DTUpdated { get; set; }

        // Navigation properties
        public ICollection<StockAccessory> Accessories { get; set; } = new List<StockAccessory>();
        public ICollection<Image> Images { get; set; } = new List<Image>();
    }
}
