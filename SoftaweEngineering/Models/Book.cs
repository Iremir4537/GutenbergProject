using HtmlAgilityPack;
namespace SoftaweEngineering.Models    

{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public List<String> Styles { get; set; }
        public String Body { get; set; }
        public int Page { get; set; }
    }
}
