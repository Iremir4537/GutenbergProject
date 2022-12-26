using Microsoft.AspNetCore.Mvc;
using SoftaweEngineering.Models;
using System.Diagnostics;
using System.Xml;
using System.Net.Http;
using System.Net;
using HtmlAgilityPack;
using System.Web;
using System.Text.RegularExpressions;
using System.Text;
using Nancy.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using SoftaweEngineering.Data;
using Microsoft.EntityFrameworkCore;

namespace SoftaweEngineering.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        private readonly ILogger<HomeController> _logger;
        static readonly HttpClient client = new HttpClient();

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            String[] idStr = {"2641","145","37106","16389","67979", "100", "2701", "6761", "394", "2160", "4085", "6593", "5197", "1259", "46", "84", "1342", "25344", "11", "174" };
            List<BookCard> bc = new List<BookCard>();
            foreach (var bd in idStr)
            {


                string url = "https://gutendex.com/books/?ids=" + bd;
                HttpResponseMessage res = await client.GetAsync(url);
                JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                BookSearch search = javaScriptSerializer.Deserialize<BookSearch>(res.Content.ReadAsStringAsync().Result);

                string imgUrl = "https://www.gutenberg.org/cache/epub/" + bd+ "/images/cover.jpg";
                HttpResponseMessage res1 = await client.GetAsync(imgUrl);
                if (res1.StatusCode.ToString() == "NotFound")
                {
                    imgUrl = "https://www.gutenberg.org/cache/epub/" + bd+ "/" + bd+ "-cover.png";
                }
                HttpResponseMessage res2 = await client.GetAsync(imgUrl);
                if (res2.StatusCode.ToString() == "NotFound")
                {
                    imgUrl = "https://www.gutenberg.org/cache/epub/" + bd+ "/pg" + bd+ ".cover.medium.jpg";
                }
                BookCard bc1 = new BookCard
                {
                    Id = int.Parse(bd),
                    Next = false,
                    Title = search.results[0].title,
                    ImageUrl = imgUrl,
                };
                bc.Add(bc1);
            }

            return View(bc);
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
                Page = 0,
            };
            return book;
        }

        [Route("book/{id:int}")]
        public async Task<IActionResult> Book(int id)
        {
            Book book = GetBook("https://www.gutenberg.org/cache/epub/"+id+"/", "pg"+id+ "-images.html",id);
            string email = Request.Cookies["SESSION"];
            if(email != null)
            {
            var user = await _context.User.FirstOrDefaultAsync(m => m.Email == email);
            var library = await _context.Library.FirstOrDefaultAsync(m => m.UserId == user.Id);
            var lb = await _context.Library_Book.FirstOrDefaultAsync(m => m.BookId == id && m.LibraryId == library.Id);
                if(lb != null)
                {
                book.Page = lb.Page;
                }
            }

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/library/[action]")]
        public async Task<IActionResult> AddToLibrary(string Id)
        {
            string email = Request.Cookies["SESSION"];
            var user = await _context.User.FirstOrDefaultAsync(m => m.Email == email);
            var library = await _context.Library.FirstOrDefaultAsync(m => m.UserId == user.Id);
            Library_Book lb = new Library_Book()
            {
                BookId = int.Parse(Id),
                LibraryId = library.Id,
                Page = 0,
            };
            _context.Add(lb);
            await _context.SaveChangesAsync();
            return RedirectToAction("Library");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("/library/[action]")]
        public async Task<IActionResult> RemoveFromLibrary(string Id)
        {
            string email = Request.Cookies["SESSION"];
            var user = await _context.User.FirstOrDefaultAsync(m => m.Email == email);
            var library = await _context.Library.FirstOrDefaultAsync(m => m.UserId == user.Id);
            var lb = await _context.Library_Book.FirstOrDefaultAsync(m => m.BookId == int.Parse(Id));
            _context.Remove(lb);
            await _context.SaveChangesAsync();
            return RedirectToAction("Library");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPage(string Id, string BookId)
        {            
            string email = Request.Cookies["SESSION"];
            var user = await _context.User.FirstOrDefaultAsync(m => m.Email == email);
            var library = await _context.Library.FirstOrDefaultAsync(m => m.UserId == user.Id);
            var lb = await _context.Library_Book.FirstOrDefaultAsync(m => m.BookId == int.Parse(BookId) && m.LibraryId == library.Id);
            if(lb != null)
            {
                lb.Page = int.Parse(Id);
                _context.Update(lb);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Library");
        }

        [Route("/library")]
        public async Task<IActionResult> Library()
        {
            string email = Request.Cookies["SESSION"];
            var user = await _context.User.FirstOrDefaultAsync(m => m.Email == email);
            var library = await _context.Library.FirstOrDefaultAsync(m => m.UserId == user.Id);
            var lb = _context.Library_Book.Where(x => x.LibraryId == library.Id);

            List<BookCard> bc = new List<BookCard>();
            foreach (var bd in lb)
            {


                string url = "https://gutendex.com/books/?ids=" +bd.BookId;
                HttpResponseMessage res = await client.GetAsync(url);
                JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
                BookSearch search = javaScriptSerializer.Deserialize<BookSearch>(res.Content.ReadAsStringAsync().Result);

                string imgUrl = "https://www.gutenberg.org/cache/epub/" + bd.BookId + "/images/cover.jpg";
                HttpResponseMessage res1 = await client.GetAsync(imgUrl);
                if (res1.StatusCode.ToString() == "NotFound")
                {
                    imgUrl = "https://www.gutenberg.org/cache/epub/" + bd.BookId + "/" + bd.BookId + "-cover.png";
                }
                HttpResponseMessage res2 = await client.GetAsync(imgUrl);
                if (res2.StatusCode.ToString() == "NotFound")
                {
                    imgUrl = "https://www.gutenberg.org/cache/epub/" + bd.BookId + "/pg" + bd.BookId + ".cover.medium.jpg";
                }
                BookCard bc1 = new BookCard
                {
                    Id = bd.BookId,
                    Next = false,
                    Title = search.results[0].title,
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