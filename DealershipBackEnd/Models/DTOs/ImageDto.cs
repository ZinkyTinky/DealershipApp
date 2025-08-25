namespace DealershipBackEnd.DTOs
{
    public class ImageDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; } // <-- URL to fetch the image
    }
}