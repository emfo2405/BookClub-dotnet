using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookClub.Data;
using BookClub.Models;

namespace BookClub.Controllers
{
    public class ChapterController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ChapterController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Chapter
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Chapter.Include(c => c.Book);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Chapter/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chapterModel = await _context.Chapter
                .Include(c => c.Book)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (chapterModel == null)
            {
                return NotFound();
            }

            return View(chapterModel);
        }

        // GET: Chapter/Create
        public IActionResult Create()
        {
            ViewData["BookModelId"] = new SelectList(_context.Book, "Id", "Title");
            return View();
        }

        // POST: Chapter/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Number,BookModelId")] ChapterModel chapterModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(chapterModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BookModelId"] = new SelectList(_context.Book, "Id", "Title", chapterModel.BookModelId);
            return View(chapterModel);
        }

        // GET: Chapter/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chapterModel = await _context.Chapter.FindAsync(id);
            if (chapterModel == null)
            {
                return NotFound();
            }
            ViewData["BookModelId"] = new SelectList(_context.Book, "Id", "Title", chapterModel.BookModelId);
            return View(chapterModel);
        }

        // POST: Chapter/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Number,BookModelId")] ChapterModel chapterModel)
        {
            if (id != chapterModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(chapterModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChapterModelExists(chapterModel.Id))
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
            ViewData["BookModelId"] = new SelectList(_context.Book, "Id", "Title", chapterModel.BookModelId);
            return View(chapterModel);
        }

        // GET: Chapter/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var chapterModel = await _context.Chapter
                .Include(c => c.Book)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (chapterModel == null)
            {
                return NotFound();
            }

            return View(chapterModel);
        }

        // POST: Chapter/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var chapterModel = await _context.Chapter.FindAsync(id);
            if (chapterModel != null)
            {
                _context.Chapter.Remove(chapterModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ChapterModelExists(int id)
        {
            return _context.Chapter.Any(e => e.Id == id);
        }
    }
}
