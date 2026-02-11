using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BookClub.Data;
using BookClub.Models;
using Microsoft.AspNetCore.Authorization;

namespace BookClub.Controllers
{
    public class ReviewController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReviewController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Review
        public async Task<IActionResult> Index()
        {

            //Check if_context.Review is null
            if(_context.Review == null)
            {
                return NotFound();
            }

            var applicationDbContext = _context.Review.Include(r => r.Book);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Review/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //Check if_context.Review is null
            if(_context.Review == null)
            {
                return NotFound();
            }

            var reviewModel = await _context.Review
                .Include(r => r.Book)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reviewModel == null)
            {
                return NotFound();
            }

            return View(reviewModel);
        }

    [Authorize]
        // GET: Review/Create
        public IActionResult Create()
        {
            ViewData["BookModelId"] = new SelectList(_context.Book, "Id", "Title");
            return View();
        }

    [Authorize]
        // POST: Review/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Review,Comment,CreatedAt,BookModelId")] ReviewModel reviewModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(reviewModel);

                //Add logged in User
                reviewModel.UserName = User.Identity?.Name ?? "Unknown";

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BookModelId"] = new SelectList(_context.Book, "Id", "Title", reviewModel.BookModelId);
            return View(reviewModel);
        }

    [Authorize]
        // GET: Review/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //Check if_context.Review is null
            if(_context.Review == null)
            {
                return NotFound();
            }

            var reviewModel = await _context.Review.FindAsync(id);
            if (reviewModel == null)
            {
                return NotFound();
            }
            ViewData["BookModelId"] = new SelectList(_context.Book, "Id", "Title", reviewModel.BookModelId);
            return View(reviewModel);
        }

    [Authorize]
        // POST: Review/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Review,Comment,CreatedAt,BookModelId,UserId")] ReviewModel reviewModel)
        {
            if (id != reviewModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reviewModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReviewModelExists(reviewModel.Id))
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
            ViewData["BookModelId"] = new SelectList(_context.Book, "Id", "Title", reviewModel.BookModelId);
            return View(reviewModel);
        }

    [Authorize]
        // GET: Review/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //Check if_context.Review is null
            if(_context.Review == null)
            {
                return NotFound();
            }

            var reviewModel = await _context.Review
                .Include(r => r.Book)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reviewModel == null)
            {
                return NotFound();
            }

            return View(reviewModel);
        }

    [Authorize]
        // POST: Review/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            //Check if_context.Review is null
            if(_context.Review == null)
            {
                return NotFound();
            }

            var reviewModel = await _context.Review.FindAsync(id);
            if (reviewModel != null)
            {
                _context.Review.Remove(reviewModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReviewModelExists(int id)
        {
            //Check if_context.Review is null
            if(_context.Review == null)
            {
                return false;
            }
            
            return _context.Review.Any(e => e.Id == id);
        }
    }
}
