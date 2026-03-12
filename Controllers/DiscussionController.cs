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
using System.Security.Claims;
using System.IO.Compression;

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
            //Check if_context.Discussion is null
            if(_context.Discussion == null)
            {
                return NotFound();
            }

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

            //Check if_context.Discussion is null
            if(_context.Discussion == null)
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
        public IActionResult Create(int chapterId)
        {
            ViewData["ReturnUrl"] = Request.Headers["Referer"].ToString();

            //Håller koll på vilket kapitel som klickades på
            var chapter = _context.Chapter.FirstOrDefault(c => c.Id == chapterId);

            //Hämtar alla kapitel för boken med id BookModelId
            var chapters = _context.Chapter
            .Include(c => c.Book)
            .Where(c => c.BookModelId == chapter.BookModelId)
            //Skapar en lista där kapitel visas med nummer och namn för den valda boken
            .Select(c => new{c.Id, Display = "Kapitel " + c.Number + " - " + c.Title})
            .ToList();
            //Skapar en dropdown lista där förvalda är det kapitlet användaren klickade på
            ViewData["ChapterModelId"] = new SelectList(chapters, "Id", "Display", chapterId);
            return View();
        }

    [Authorize]
        // POST: Discussion/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Content,CreatedAt,ChapterModelId")] DiscussionModel discussionModel, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                _context.Add(discussionModel);


                //Add logged in User
                discussionModel.UserName = User.Identity?.Name ?? "Unknown";

                await _context.SaveChangesAsync();

                            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }
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

            //Check if_context.Discussion is null
            if(_context.Discussion == null)
            {
                return NotFound();
            }

            var discussionModel = await _context.Discussion.FindAsync(id);
            if (discussionModel == null)
            {
                return NotFound();
            }

            //Förhindra att andra än skaparen kan redigera inlägget
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (discussionModel.UserId != userId) return Forbid();

            ViewData["ReturnUrl"] = Request.Headers["Referer"].ToString();
            ViewData["ChapterModelId"] = new SelectList(_context.Chapter, "Id", "Number", discussionModel.ChapterModelId);
            return View(discussionModel);
        }

    [Authorize]
        // POST: Discussion/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string returnUrl, [Bind("Id,Title,Content,CreatedAt,ChapterModelId")] DiscussionModel discussionModel)
        {
            if (id != discussionModel.Id)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (ModelState.IsValid && discussionModel.UserId == userId)
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

            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
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

            //Check if_context.Discussion is null
            if(_context.Discussion == null)
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

            //Skydda view från personer som inte är skapare eller admin
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (discussionModel.UserId != userId && !User.IsInRole("Admin")) return Forbid();

            ViewData["ReturnUrl"] = Request.Headers["Referer"].ToString();
            return View(discussionModel);
        }

    [Authorize]
        // POST: Discussion/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, string returnUrl)
        {

            //Check if_context.Discussion is null
            if(_context.Discussion == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var discussionModel = await _context.Discussion.FindAsync(id);
            if (discussionModel != null && discussionModel.UserId == userId || User.IsInRole("Admin"));
            {
                _context.Discussion.Remove(discussionModel);
            }

            await _context.SaveChangesAsync();
            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction(nameof(Index));
        }

        private bool DiscussionModelExists(int id)
        {
            //Check if_context.Discussion is null
            if(_context.Discussion == null)
            {
                return false;
            }

            return _context.Discussion.Any(e => e.Id == id);
        }
    }
}
