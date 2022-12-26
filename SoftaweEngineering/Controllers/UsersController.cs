using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SoftaweEngineering.Data;
using SoftaweEngineering.Models;

namespace SoftaweEngineering.Controllers
{
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            if (email == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .FirstOrDefaultAsync(m => m.Email == email);
            if (user == null)
            {
                return NotFound();
            }
            if(user.Password != password)
            {
                return NotFound();
            }
            Response.Cookies.Append("SESSION", email);
            return RedirectToAction("Index","Home");
        }

        
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register([Bind("Email,Name,Password")] User user)
        {
            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                User usr = await _context.User.FirstOrDefaultAsync(m => m.Email == user.Email);
                Library library = new Library()
                {
                    Name = "MyLibrary",
                    UserId = usr.Id,
                };
                _context.Add(library);
                await _context.SaveChangesAsync(); ;
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction("Login");
        }
        public IActionResult Register()
        {
            return View();
        }

        public IActionResult Logout()
        {
            Response.Cookies.Delete("SESSION");
            return RedirectToAction("Index");
        }

        // GET: Users
        public async Task<IActionResult> Index()
        {
            string email = Request.Cookies["SESSION"];
            var user = await _context.User.FirstOrDefaultAsync(m => m.Email == email);
            if(user == null)
            {
                return RedirectToAction("Login");
            }

              return View(user);
        }

        private bool UserExists(int id)
        {
          return _context.User.Any(e => e.Id == id);
        }
    }
}
