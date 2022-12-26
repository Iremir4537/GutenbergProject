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
    public class Library_BookController : Controller
    {
        private readonly ApplicationDbContext _context;

        public Library_BookController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Library_Book
        public async Task<IActionResult> Index()
        {
              return View(await _context.Library_Book.ToListAsync());
        }

        // GET: Library_Book/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Library_Book == null)
            {
                return NotFound();
            }

            var library_Book = await _context.Library_Book
                .FirstOrDefaultAsync(m => m.Id == id);
            if (library_Book == null)
            {
                return NotFound();
            }

            return View(library_Book);
        }

        // GET: Library_Book/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Library_Book/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,LibraryId,BookId")] Library_Book library_Book)
        {
            if (ModelState.IsValid)
            {
                _context.Add(library_Book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(library_Book);
        }

        // GET: Library_Book/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Library_Book == null)
            {
                return NotFound();
            }

            var library_Book = await _context.Library_Book.FindAsync(id);
            if (library_Book == null)
            {
                return NotFound();
            }
            return View(library_Book);
        }

        // POST: Library_Book/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,LibraryId,BookId")] Library_Book library_Book)
        {
            if (id != library_Book.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(library_Book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!Library_BookExists(library_Book.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(library_Book);
        }

        // GET: Library_Book/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Library_Book == null)
            {
                return NotFound();
            }

            var library_Book = await _context.Library_Book
                .FirstOrDefaultAsync(m => m.Id == id);
            if (library_Book == null)
            {
                return NotFound();
            }

            return View(library_Book);
        }

        // POST: Library_Book/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Library_Book == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Library_Book'  is null.");
            }
            var library_Book = await _context.Library_Book.FindAsync(id);
            if (library_Book != null)
            {
                _context.Library_Book.Remove(library_Book);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool Library_BookExists(int id)
        {
          return _context.Library_Book.Any(e => e.Id == id);
        }
    }
}
