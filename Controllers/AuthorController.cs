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

    public class AuthorController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AuthorController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Author, funktion för att hämta in information om författare
        public async Task<IActionResult> Index()
        {
            //Check if_context.Author is null
            if(_context.Author == null)
            {
                //Om context är null returneras en notFound-sida
                return NotFound();
            }
            //Annars returneras data som sparats i databasen om författare
            return View(await _context.Author.ToListAsync());
        }

        // GET: Author/Details/5, Funktion för att hämta in information om en specifik författare
        public async Task<IActionResult> Details(int? id)
        {
            //Om inte id hittas returneras NotFound-sida
            if (id == null)
            {
                return NotFound();
            }
            //Check if_context.Author is null, om context är null returneras en NotFound-sida
            if(_context.Author == null)
            {
                return NotFound();
            }

            //Author hämtas in från databasen och filtreras på id som skickas med
            var authorModel = await _context.Author
                .FirstOrDefaultAsync(m => m.Id == id);
            //Om författaren inte hittas returneras notFound
            if (authorModel == null)
            {
                return NotFound();
            }
            //En detaljsida för författare returneras
            return View(authorModel);                

        }

    [Authorize]
        // GET: Author/Create, funktion för att hämta sidan för att skapa en ny författare
        public IActionResult Create()
        {
            //Om användaren är inloggad som admin visas sidan annars returneras access denied-sidan
            if(User.IsInRole("Admin"))
            {
            return View();                
            } else
            {
                return View("AccessDenied");
            }

        }

        // POST: Author/Create, funktion för att posta en ny författare
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Country")] AuthorModel authorModel)
        {
            //Om användaren är i admin-roll kan en ny författare skapas 
            if (ModelState.IsValid && User.IsInRole("Admin"))
            {
                _context.Add(authorModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(authorModel);
        }

    [Authorize]
        // GET: Author/Edit/5, funkton för att hämta en redigera-sida för författare
        public async Task<IActionResult> Edit(int? id)
        {
            //Om inget id för författaren hittas returneras not found
            if (id == null)
            {
                return NotFound();
            }
            //Check if_context.Author is null, vid null returneras not found
            if(_context.Author == null)
            {
                return NotFound();
            }

            //Id hittas för författare och om det inte träffar i databasen returneras notfound
            var authorModel = await _context.Author.FindAsync(id);
            if (authorModel == null)
            {
                return NotFound();
            }
            //Om användaren är inloggad som admin returneras viewn för uppdateringen, annars returneras access denied
            if(User.IsInRole("Admin"))
            {
            return View(authorModel);                
            } else
            {
                return View("AccessDenied");
            }

        }

    [Authorize]
        // POST: Author/Edit/5, funktion för att kunna uppdatera information om en författare
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Country")] AuthorModel authorModel)
        {
            //Om det medskickade id inte är lika med det i modellen returneras notfound
            if (id != authorModel.Id)
            {
                return NotFound();
            }

            //Om allt är rätt med modellen och användaren är i rollen admin kan en författare uppdateras
            if (ModelState.IsValid && User.IsInRole("Admin"))
            {
                try
                {
                    _context.Update(authorModel);
                    await _context.SaveChangesAsync();
                }
                //Fångar upp fel från databasen
                catch (DbUpdateConcurrencyException)
                {
                    //Om posten med id inte finns kvar returneras notFound
                    if (!AuthorModelExists(authorModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                //Om uppdatering lyckas skickas man vidare till index
                return RedirectToAction(nameof(Index));
            }
            //Om uppdatering misslyckas stannar man kvar
            return View(authorModel);
        }

    [Authorize]
        // GET: Author/Delete/5, funktion för att hämta sidan för att kunna radera poster
        public async Task<IActionResult> Delete(int? id)
        {
            //Om id inte hittas returneras notfound
            if (id == null)
            {
                return NotFound();
            }
            //Check if_context.Author is null
            if(_context.Author == null)
            {
                return NotFound();
            }

            //Author hämtas in och id kontrolleras
            var authorModel = await _context.Author
                .FirstOrDefaultAsync(m => m.Id == id);
            //Om författaren inte hittas returneras notfound
            if (authorModel == null)
            {
                return NotFound();
            }
            //Om användaren är inloggad som admin returneras rätt sida
            if (User.IsInRole("Admin"))
            {
            return View(authorModel);                
            } else
            //Annars returneras access denied
            {
                return View("AccessDenied");
            }

        }

    [Authorize]
        // POST: Author/Delete/5, funktion för att radera ett inlägg
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //Check if_context.Author is null
            if(_context.Author == null)
            {
                return NotFound();
            }

            //Kontroll av författare med inskickat id och att användaren är inloggad som admin
            var authorModel = await _context.Author.FindAsync(id);
            if (authorModel != null && User.IsInRole("Admin"))
            //Om kraven uppfylls tas författaren bort
            {
                _context.Author.Remove(authorModel);
            }
            //Ändringar sparas
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

//Kontroll om en författare med ett visst id finns i databasen
        private bool AuthorModelExists(int id)
        {
            //Check if_context.Author is null
            if(_context.Author == null)
            {
                return false;
            }

            return _context.Author.Any(e => e.Id == id);
        }
    }
}
