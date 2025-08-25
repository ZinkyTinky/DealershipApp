using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DealershipBackEnd.Models
{
    public class Image
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public byte[] ImageBinary { get; set; }   // 👈 Add this

        // Foreign key
        public int StockItemId { get; set; }
        public StockItem StockItem { get; set; }
    }
}
