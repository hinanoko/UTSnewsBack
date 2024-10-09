namespace WebApplication1.Models
{
    public class LostAndFoundModels
    {
        public int Id { get; set; } // 唯一标识符
        public string ItemName { get; set; }
        public string Description { get; set; }
        public string Phone { get; set; }

        public string LostOrFound { get; set; }

        public string Publisher { get; set; }

        public string PublisherID { get; set; }

        public string Time { get; set; }

        public string Location { get; set; }

        public string image {  get; set; }
    }
}
