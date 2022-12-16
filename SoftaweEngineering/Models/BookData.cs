namespace SoftaweEngineering.Models
{
    public class BookData
    {
        public int id { get; set; }
        public string title { get; set; }
        public object[] authors { get; set; }
        public object[] translators { get; set; }
        public String[] subjects { get; set; }
        public String[] bookshelves { get; set; }
        public String[] languages { get; set; }
        public Boolean copyright { get; set; }
        public String media_type { get; set; }
        public object formats { get; set; }
        public int download_count { get; set; }

    }
}
