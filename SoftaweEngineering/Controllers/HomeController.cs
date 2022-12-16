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
using Nancy.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

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
        [HttpGet]
        public IActionResult NameGet()
        {
            string qr = Request.Form["SearchName"];
            string url = "localhost:7184/books/search/?query="+qr+"&page=1";
            Console.WriteLine(url);
            return RedirectToRoute(url);
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


        [Route("books/search/")]
        public async Task<IActionResult> BookSearch(string query, string page = "1")
        {
            query = query.ToLower();
            query.Replace(" ", "%20");
            string url = "https://gutendex.com/books?search=" + query+"&page="+page;
            HttpResponseMessage res = await client.GetAsync(url);
            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            BookSearch search = javaScriptSerializer.Deserialize<BookSearch>(res.Content.ReadAsStringAsync().Result);
            List<BookCard> bc = new List<BookCard>();
            foreach (BookData bd in search.results)
            {
                string imgUrl = "https://www.gutenberg.org/cache/epub/" + bd.id + "/"+ bd.id +"-cover.png";
                HttpResponseMessage res1 = await client.GetAsync(imgUrl);
                if (res1.StatusCode.ToString() == "NotFound")
                {
                    imgUrl = "https://www.gutenberg.org/cache/epub/" + bd.id + "/images/cover.jpg";
                }
                
                BookCard bc1 = new BookCard
                {   
                    Id = bd.id,
                    Next = search.next != null,
                    Title = bd.title,
                    ImageUrl = imgUrl,
                };
                bc.Add(bc1);

            }
            return View(bc);
        }

        [Route("books/topic/")]
        public async Task<IActionResult> BooksTopic(string topic ,string page = "1")
        {
            topic = topic.ToLower();
            topic.Replace(" ", "%20");
            string url = "https://gutendex.com/books/?page=" + page + "&topic=" + topic;
            HttpResponseMessage res = await client.GetAsync(url);
            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            BookSearch search = javaScriptSerializer.Deserialize<BookSearch>(res.Content.ReadAsStringAsync().Result);
            List<BookCard> bc = new List<BookCard>();
            foreach (BookData bd in search.results)
            {
                string imgUrl = "https://www.gutenberg.org/cache/epub/" + bd.id + "/images/cover.jpg";
                HttpResponseMessage res1 = await client.GetAsync(imgUrl);
                if (res1.StatusCode.ToString() == "NotFound")
                {
                    imgUrl = "https://www.gutenberg.org/cache/epub/" + bd.id + "/" + bd.id + "-cover.png";
                }
                HttpResponseMessage res2 = await client.GetAsync(imgUrl);
                if (res2.StatusCode.ToString() == "NotFound")
                {
                    imgUrl = "https://www.gutenberg.org/cache/epub/" + bd.id + "/pg" + bd.id + ".cover.medium.jpg";
                }
                BookCard bc1 = new BookCard
                {   
                    Id = bd.id,
                    Next = search.next != null,
                    Title = bd.title,
                    ImageUrl = imgUrl,
                };
                bc.Add(bc1);
                
            }
            return View(bc);
            
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