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

        // GET: Chapter, funktion för att hämta in chapter från databas
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

        // GET: Chapter/Details/5, funktion för att hämta in detaljsida om kapitel
        public async Task<IActionResult> Details(int? id)
        {
            //Om id inte hittas returneras notfound
            if (id == null)
            {
                return NotFound();
            }
            //Check if_context.Chapter is null
            if(_context.Chapter == null)
            {
                return NotFound();
            }

            //Om allt är rätt hämtas chapter med givet id och relationer inkluderas
            var chapterModel = await _context.Chapter
                .Include(c => c.Book)
                .FirstOrDefaultAsync(m => m.Id == id);
            //Om det inte hittas returneras notfound
            if (chapterModel == null)
            {
                return NotFound();
            }
            //Om användaren är inloggad som admin returneras sidan annars returneras access denied
            if(User.IsInRole("Admin"))
            {
             return View(chapterModel);   
            } else
            {
                return View("AccessDenied");
            }
            
        }

    [Authorize]
        // GET: Chapter/Create, funktion för att hämta in en skapa-sida för kapitel
        public IActionResult Create()
        {
            //Kontroll om användaren är inloggad som admin och visar antingen sidan eller access denied
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
        // POST: Chapter/Create, funktion för att skapa ett nytt kapitel
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Number,BookModelId")] ChapterModel chapterModel)
        {
            //Validering av input och användare i roll admin
            if (ModelState.IsValid && User.IsInRole("Admin"))
            {
                //Om allt är rätt skapas ett nytt kapitel och sparas
                _context.Add(chapterModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BookModelId"] = new SelectList(_context.Book, "Id", "Title", chapterModel.BookModelId);
            return View(chapterModel);
        }

    [Authorize]
        // GET: Chapter/Edit/5, funktion för att visa uppdatera-sidan för ett kapitel
        public async Task<IActionResult> Edit(int? id)
        {
            //Om id inte hittas returneras not found
            if (id == null)
            {
                return NotFound();
            }
            //Check if_context.Chapter is null
            if(_context.Chapter == null)
            {
                return NotFound();
            }

            //Hämtar in kapitel med givet id
            var chapterModel = await _context.Chapter.FindAsync(id);
            //Om det inte hittas returneras not found
            if (chapterModel == null)
            {
                return NotFound();
            }
            //Om användare är inloggad som admin visas sidan annars visas access denied
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
        // POST: Chapter/Edit/5, funktion för att uppdatera ett kapitel
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Number,BookModelId")] ChapterModel chapterModel)
        {
            //Om inte id hittas returneras notfound
            if (id != chapterModel.Id)
            {
                return NotFound();
            }
            //Om validering är korrekt och användare är inloggad som admin uppdateras inlägg
            if (ModelState.IsValid && User.IsInRole("Admin"))
            {
                try
                {
                    _context.Update(chapterModel);
                    await _context.SaveChangesAsync();
                }
                //Fel från databasen fångas upp
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
        // GET: Chapter/Delete/5, funktion för att hämta radera-sidan för ett kapitel
        public async Task<IActionResult> Delete(int? id)
        {
            //Kontroll av id
            if (id == null)
            {
                return NotFound();
            }
            //Check if_context.Chapter is null
            if(_context.Chapter == null)
            {
                return NotFound();
            }

            //Kapitel hämtas in med relationer inkluderade
            var chapterModel = await _context.Chapter
                .Include(c => c.Book)
                .FirstOrDefaultAsync(m => m.Id == id);
            //Om kapitel inte hittas returneras notfound
            if (chapterModel == null)
            {
                return NotFound();
            }
            //Om användare är i roll admin visas sidan annars visas access denied
            if(User.IsInRole("Admin"))
            {
            return View(chapterModel);    
            } else
            {
                return View("AccessDenied");
            }
            
        }

    [Authorize]
        // POST: Chapter/Delete/5, funktion för att radera ett kapitel
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            //Check if_context.Chapter is null
            if(_context.Chapter == null)
            {
                return NotFound();
            }
            //Kontroll av id och användare som tar bort inlägg om allt är korrekt
            var chapterModel = await _context.Chapter.FindAsync(id);
            if (chapterModel != null && User.IsInRole("Admin"))
            {
                _context.Chapter.Remove(chapterModel);
            }
            //Ändringar sparas
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //Funktion för att hitta ett kapitel med givet id
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
