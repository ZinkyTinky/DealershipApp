namespace DealershipBackEnd.Models
{
    public class StockAccessory
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        // FK
        public int StockItemId { get; set; }
        public StockItem? StockItem { get; set; }
    }
}
