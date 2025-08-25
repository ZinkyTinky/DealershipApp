namespace DealershipBackEnd.DTOs
{
    public class StockItemDto
    {
        public int Id { get; set; }
        public string RegNo { get; set; } = string.Empty;
        public string Make { get; set; } = string.Empty;
        public string Model { get; set; } = string.Empty;
        public int ModelYear { get; set; }
        public int KMS { get; set; }
        public string Colour { get; set; } = string.Empty;
        public string VIN { get; set; } = string.Empty;
        public decimal RetailPrice { get; set; }
        public decimal CostPrice { get; set; }
        public DateTime DTCreated { get; set; }
        public DateTime? DTUpdated { get; set; }

        public List<StockAccessoryDto> Accessories { get; set; }
        public List<ImageDto> Images { get; set; } = new List<ImageDto>();
    }
}
