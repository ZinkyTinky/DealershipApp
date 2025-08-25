using DealershipBackEnd.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace DealershipBackEnd.DTOs
{
    public class StockItemUpdateDto
    {
        public string RegNo { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public int ModelYear { get; set; }
        public int KMS { get; set; }
        public string Colour { get; set; }
        public string VIN { get; set; }
        public decimal RetailPrice { get; set; }
        public decimal CostPrice { get; set; }

        public string? RemoveImageIds { get; set; }
        public string? RemoveAccessoryIds { get; set; }

        // For new uploads
        public List<IFormFile>? NewImages { get; set; }
        public List<StockAccessoryDto>? NewAccessories { get; set; }
    }
}
