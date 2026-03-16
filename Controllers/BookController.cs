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
    public class BookController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Book, funktion för att hämta in tabellen book från databasen
        public async Task<IActionResult> Index()
        {


            //Check if_context.Book is null
            if(_context.Book == null)
            {
                return NotFound();
            }
            var applicationDbContext = _context.Book.Include(b => b.Author).Include(b => b.Reviews);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Book/Details/5, funktion för att hämta in detaljsidan för book med givet id
        public async Task<IActionResult> Details(int? id)
        {
            //Om id inte hittas returneras NotFound
            if (id == null)
            {
                return NotFound();
            }
            //Check if_context.Book is null
            if(_context.Book == null)
            {
                return NotFound();
            }

            //Bok-modellen hämtas in och inkluderar relationer
            var bookModel = await _context.Book
                .Include(b => b.Author)
                .Include(b => b.Reviews)
                .Include(c => c.Chapters)
                .ThenInclude(d => d.Discussions)
                    .ThenInclude(e => e.User)

            //Id kontrolleras
                .FirstOrDefaultAsync(m => m.Id == id);

            //Om inget hittas returneras notfound
            if (bookModel == null)
            {
                return NotFound();
            }

            //Annars returneras bookModel
            return View(bookModel);
        }

    [Authorize]
        // GET: Book/Create, funktion för att hämta in sidan för att skapa en ny bok
        public IActionResult Create()
        {
            //Om användaren är inloggad som admin hämtas data in annars returneras access denied
            if(User.IsInRole("Admin"))
            {
                        ViewData["AuthorModelId"] = new SelectList(_context.Author, "Id", "Name");
            return View();    
            } else
            {
                return View("AccessDenied");
            }

        }

    [Authorize]
        // POST: Book/Create, funktion för att skapa en ny bok
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Year,Description,Image,AuthorModelId")] BookModel bookModel)
        {
            //Om input valideras korrekt och användaren är inloggad som admin läggs data till och returnerar bookModel
            if (ModelState.IsValid && User.IsInRole("Admin"))
            {
                _context.Add(bookModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AuthorModelId"] = new SelectList(_context.Author, "Id", "Name", bookModel.AuthorModelId);
            return View(bookModel);
        }

    [Authorize]
        // GET: Book/Edit/5, funktion för att hämta uppdateringssidan för en bok med givet id
        public async Task<IActionResult> Edit(int? id)
        {
            //Om id inte hittas returneras notfound
            if (id == null)
            {
                return NotFound();
            }
            //Check if_context.Book is null
            if(_context.Book == null)
            {
                return NotFound();
            }

            //Id kontrolleras
            var bookModel = await _context.Book.FindAsync(id);
            if (bookModel == null)
            {
                return NotFound();
            }
            //Användarroll kontrolleras och returnerar sidan vid korrekt validering annars returneras access denied
            if(User.IsInRole("Admin"))
            {
            ViewData["AuthorModelId"] = new SelectList(_context.Author, "Id", "Name", bookModel.AuthorModelId);
            return View(bookModel);                
            } else
            {
                return View("AccessDenied");
            }

        }

    [Authorize]
        // POST: Book/Edit/5, funktion för att uppdatera en bok
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Year,Description,Image,AuthorModelId")] BookModel bookModel)
        {
            //Om id inte hittas returneras notfound
            if (id != bookModel.Id)
            {
                return NotFound();
            }
            //Om validering är korrekt och användare är inloggad som admin sparas uppdateringar
            if (ModelState.IsValid && User.IsInRole("Admin"))
            {
                try
                {
                    _context.Update(bookModel);
                    await _context.SaveChangesAsync();
                }
                //Fel från databas fångas
                catch (DbUpdateConcurrencyException)
                {
                    //Om bok med givet id inte hittas returneras notfound
                    if (!BookModelExists(bookModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                //Returneras index för books
                return RedirectToAction(nameof(Index));
            }
            ViewData["AuthorModelId"] = new SelectList(_context.Author, "Id", "Name", bookModel.AuthorModelId);
            return View(bookModel);
        }

    [Authorize]
        // GET: Book/Delete/5, funktion för att hämta in sida för att radera en bok
        public async Task<IActionResult> Delete(int? id)
        {
            //Om id inte hittas returneras notfound
            if (id == null)
            {
                return NotFound();
            }
            //Check if_context.Book is null
            if(_context.Book == null)
            {
                return NotFound();
            }

            //Book hämtas in med id och författare inkluderas
            var bookModel = await _context.Book
                .Include(b => b.Author)
                .FirstOrDefaultAsync(m => m.Id == id);
            //Om den inte hittas returneras notfound
            if (bookModel == null)
            {
                return NotFound();
            }
            //Om användaren är inloggad som admin visas sidan annars returneras access denied
            if(User.IsInRole("Admin"))
            {
            return View(bookModel);                
            } else {
            return View("AccessDenied");    
            }

        }

    [Authorize]
        // POST: Book/Delete/5, funktion för att radera en bok
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //Check if_context.Book is null
            if(_context.Book == null)
            {
                return NotFound();
            }

            //Bok med givet id kontrolleras och användarens roll kontrolleras, om allt är rätt tas boken bort
            var bookModel = await _context.Book.FindAsync(id);
            if (bookModel != null &&User.IsInRole("Admin"))
            {
                _context.Book.Remove(bookModel);
            }

            //Ändringar sparas och returneras till index
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //Funktion för att hitta bok med givet id
        private bool BookModelExists(int id)
        {
            //Check if_context.Book is null
            if(_context.Book == null)
            {
                return false;
            }

            return _context.Book.Any(e => e.Id == id);
        }
    }
}
