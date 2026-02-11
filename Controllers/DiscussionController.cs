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
    public class DiscussionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DiscussionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Discussion
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Discussion.Include(d => d.Chapter);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Discussion/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var discussionModel = await _context.Discussion
                .Include(d => d.Chapter)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (discussionModel == null)
            {
                return NotFound();
            }

            return View(discussionModel);
        }

    [Authorize]
        // GET: Discussion/Create
        public IActionResult Create()
        {
            ViewData["ChapterModelId"] = new SelectList(_context.Chapter, "Id", "Number");
            return View();
        }

    [Authorize]
        // POST: Discussion/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Content,CreatedAt,ChapterModelId")] DiscussionModel discussionModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(discussionModel);


                //Add logged in User
                discussionModel.UserName = User.Identity?.Name ?? "Unknown";

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ChapterModelId"] = new SelectList(_context.Chapter, "Id", "Number", discussionModel.ChapterModelId);
            return View(discussionModel);
        }

    [Authorize]
        // GET: Discussion/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var discussionModel = await _context.Discussion.FindAsync(id);
            if (discussionModel == null)
            {
                return NotFound();
            }
            ViewData["ChapterModelId"] = new SelectList(_context.Chapter, "Id", "Number", discussionModel.ChapterModelId);
            return View(discussionModel);
        }

    [Authorize]
        // POST: Discussion/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Content,CreatedAt,ChapterModelId")] DiscussionModel discussionModel)
        {
            if (id != discussionModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(discussionModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DiscussionModelExists(discussionModel.Id))
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
            ViewData["ChapterModelId"] = new SelectList(_context.Chapter, "Id", "Number", discussionModel.ChapterModelId);
            return View(discussionModel);
        }

    [Authorize]
        // GET: Discussion/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var discussionModel = await _context.Discussion
                .Include(d => d.Chapter)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (discussionModel == null)
            {
                return NotFound();
            }

            return View(discussionModel);
        }

    [Authorize]
        // POST: Discussion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var discussionModel = await _context.Discussion.FindAsync(id);
            if (discussionModel != null)
            {
                _context.Discussion.Remove(discussionModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DiscussionModelExists(int id)
        {
            return _context.Discussion.Any(e => e.Id == id);
        }
    }
}
