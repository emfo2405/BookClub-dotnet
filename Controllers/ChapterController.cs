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
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;

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
            //Check if_context.Chapter is null
            if(_context.Chapter == null)
            {
                return NotFound();
            }

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
            //Check if_context.Chapter is null
            if(_context.Chapter == null)
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
            if(User.IsInRole("Admin"))
            {
             return View(chapterModel);   
            } else
            {
                return View("AccessDenied");
            }
            
        }

    [Authorize]
        // GET: Chapter/Create
        public IActionResult Create()
        {
            if (User.IsInRole("Admin"))
            {
            ViewData["BookModelId"] = new SelectList(_context.Book, "Id", "Title");
            return View();   
            } else
            {
            return View("AccessDenied");
            }

        }

    [Authorize]
        // POST: Chapter/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Number,BookModelId")] ChapterModel chapterModel)
        {

            if (ModelState.IsValid && User.IsInRole("Admin"))
            {
                _context.Add(chapterModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BookModelId"] = new SelectList(_context.Book, "Id", "Title", chapterModel.BookModelId);
            return View(chapterModel);
        }

    [Authorize]
        // GET: Chapter/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            //Check if_context.Chapter is null
            if(_context.Chapter == null)
            {
                return NotFound();
            }


            var chapterModel = await _context.Chapter.FindAsync(id);
            if (chapterModel == null)
            {
                return NotFound();
            }
            if (User.IsInRole("Admin"))
            {
            ViewData["BookModelId"] = new SelectList(_context.Book, "Id", "Title", chapterModel.BookModelId);
            return View(chapterModel);                
            } else
            {
                return View("AccessDenied");
            }

        }

    [Authorize]
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

            if (ModelState.IsValid && User.IsInRole("Admin"))
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

    [Authorize]
        // GET: Chapter/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            //Check if_context.Chapter is null
            if(_context.Chapter == null)
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
            if(User.IsInRole("Admin"))
            {
            return View(chapterModel);    
            } else
            {
                return View("AccessDenied");
            }
            
        }

    [Authorize]
        // POST: Chapter/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            //Check if_context.Chapter is null
            if(_context.Chapter == null)
            {
                return NotFound();
            }

            var chapterModel = await _context.Chapter.FindAsync(id);
            if (chapterModel != null && User.IsInRole("Admin"))
            {
                _context.Chapter.Remove(chapterModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ChapterModelExists(int id)
        {
            //Check if_context.Chapter is null
            if(_context.Chapter == null)
            {
                return false;
            }

            return _context.Chapter.Any(e => e.Id == id);
        }
    }
}
