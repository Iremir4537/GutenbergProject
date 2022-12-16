namespace SoftaweEngineering.Models
{
    public class BookSearch
    {
        public int count { get; set; }
        public String next { get; set; }
        public String previous { get; set; }
        public BookData[] results { get; set; }
    }
}
