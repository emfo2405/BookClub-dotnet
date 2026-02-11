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
            var applicationDbContext = _context.Discussion.Include(d => d.Chapter).Include(d => d.User);
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
                .Include(d => d.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (discussionModel == null)
            {
                return NotFound();
            }

            return View(discussionModel);
        }

        // GET: Discussion/Create
        public IActionResult Create()
        {
            ViewData["ChapterModelId"] = new SelectList(_context.Chapter, "Id", "Id");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Discussion/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Content,CreatedAt,ChapterModelId,UserId")] DiscussionModel discussionModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(discussionModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ChapterModelId"] = new SelectList(_context.Chapter, "Id", "Id", discussionModel.ChapterModelId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", discussionModel.UserId);
            return View(discussionModel);
        }

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
            ViewData["ChapterModelId"] = new SelectList(_context.Chapter, "Id", "Id", discussionModel.ChapterModelId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", discussionModel.UserId);
            return View(discussionModel);
        }

        // POST: Discussion/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Content,CreatedAt,ChapterModelId,UserId")] DiscussionModel discussionModel)
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
            ViewData["ChapterModelId"] = new SelectList(_context.Chapter, "Id", "Id", discussionModel.ChapterModelId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", discussionModel.UserId);
            return View(discussionModel);
        }

        // GET: Discussion/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var discussionModel = await _context.Discussion
                .Include(d => d.Chapter)
                .Include(d => d.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (discussionModel == null)
            {
                return NotFound();
            }

            return View(discussionModel);
        }

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
