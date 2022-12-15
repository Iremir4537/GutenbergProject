using Microsoft.AspNetCore.Mvc;
using SoftaweEngineering.Models;
using System.Diagnostics;
using System.Xml;
using System.Net.Http;
using System.Net;
using HtmlAgilityPack;
using SoftaweEngineering.Models;
using System.Web;
using System.Text.RegularExpressions;
using System.Text;

namespace SoftaweEngineering.Controllers
{
    public class HomeController : Controller
    {

        private readonly ILogger<HomeController> _logger;
        static readonly HttpClient client = new HttpClient();

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }

        public Book GetBook(string url, string urlN, int id)
        {
            string url1 = url + urlN;
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(url1);
            List<string> list = new List<string>();
            foreach (var item in doc.DocumentNode.SelectNodes("//head/style"))
            {
                string str = Regex.Replace(HttpUtility.HtmlEncode(item.InnerHtml.Trim()), @"\s", "");
                str = str + "div.fig{margin-left:auto!important;margin-right:auto!important}";
                list.Add(str);

            }
            foreach (var item in doc.DocumentNode.Descendants("img"))
            {
                string str1 = item.GetAttributeValue("src", null);
                string str2 = url + str1;
                item.SetAttributeValue("src", str2);
            }

            if (doc.DocumentNode.SelectSingleNode("//body/section[@class='pg-boilerplate pgheader']") != null)
            {
                doc.DocumentNode.SelectNodes("//body/section[@class='pg-boilerplate pgheader']")[0].Remove();
                doc.DocumentNode.SelectNodes("//body/section[@class='pg-boilerplate pgheader']")[0].Remove();
            }



            Book book = new Book
            {
                Id = id,
                Title = "title",
                Styles = list,
                Body = doc.DocumentNode.SelectSingleNode("//body").InnerHtml.Trim(),
            };
            return book;
        }

        [Route("book/{id:int}")]
        public IActionResult Book(int id)
        {
            Book book = GetBook("https://www.gutenberg.org/cache/epub/"+id+"/", "pg"+id+ "-images.html",id);
            return View(book);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}