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

namespace BookClub.Controllers
{
    public class ReviewController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReviewController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Review, funktion för att hämta in recensioner från databas
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

        // GET: Review/Details/5, funktion för att hämta in recension med givet id
        public async Task<IActionResult> Details(int? id)
        {
            //Om id inte hittas returneras notfound
            if (id == null)
            {
                return NotFound();
            }

            //Check if_context.Review is null
            if(_context.Review == null)
            {
                return NotFound();
            }


            //Review hämtas in med givet id och bok inkluderas
            var reviewModel = await _context.Review
                .Include(r => r.Book)
                .FirstOrDefaultAsync(m => m.Id == id);
            //Om inget hittas returneras notfound
            if (reviewModel == null)
            {
                return NotFound();
            }


            
            return View(reviewModel);
        }

    [Authorize]
        // GET: Review/Create, funktion för att hämta in en sida för att skapa en ny recension
        public IActionResult Create(int bookId)
        {
            //Data skickas med med ett returnURL från föregående sida och bokId för vald bok skickas med
            ViewData["ReturnUrl"] = Request.Headers["Referer"].ToString();
            ViewData["BookModelId"] = new SelectList(_context.Book, "Id", "Title", bookId);
            return View();
        }

    [Authorize]
        // POST: Review/Create, funktion för att skapa en ny recension
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Review,Comment,CreatedAt,BookModelId")] ReviewModel reviewModel, string returnUrl)
        {
            //Inloggad användare hämtas in
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            //Kontrollera om ett betyg redan är lämnat av användaren
            bool reviewExists = await _context.Review
            .AnyAsync(r => r.BookModelId == reviewModel.BookModelId && r.UserId == userId);

            //Om ett betyg redan har getts av användaren
            if(reviewExists)
            {
            ModelState.AddModelError("", "Ett betyg har redan lämnats för den här boken");
            }

            //Om validering är korrekt skapas ett nytt inlägg
            if (ModelState.IsValid)
            {

                //Inloggad användare läggs till till inlägget
                reviewModel.UserName = User.Identity?.Name ?? "Unknown";
                reviewModel.UserId = userId;
                _context.Add(reviewModel);

                //Ändringar sparas
                await _context.SaveChangesAsync();

            //Om det finns en sparad returnURL skickas användaren tillbaka till den annars till index sida för books
            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }
                return RedirectToAction("Index", "Book");                    
                

            }
            ViewData["BookModelId"] = new SelectList(_context.Book, "Id", "Title", reviewModel.BookModelId);
            return View(reviewModel);
        }

    [Authorize]
        // GET: Review/Edit/5, funktion för att hämta en redigeringssida för recensioner
        public async Task<IActionResult> Edit(int? id)
        {
            //Om id inte hittas returneras notfound
            if (id == null)
            {
                return NotFound();
            }

            //Check if_context.Review is null
            if(_context.Review == null)
            {
                return NotFound();
            }

            //Recension med givet id hittas
            var reviewModel = await _context.Review.FindAsync(id);
            //Om recension med givet id inte finns returneras notfound
            if (reviewModel == null)
            {
                return NotFound();
            }

            //Endast skaparen av en recension ska kunna gå in på den
            //Inloggad användare hämtas
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //Användare jämförs med skapare
            if (reviewModel.UserId != userId) return Forbid();
            //ReturnURL från header och Bok-id skickas med som förvalt i select-list
            ViewData["ReturnUrl"] = Request.Headers["Referer"].ToString();
            ViewData["BookModelId"] = new SelectList(_context.Book, "Id", "Title", reviewModel.BookModelId);
            return View(reviewModel);
        }

    [Authorize]
        // POST: Review/Edit/5, funktion för att kunna uppdatera en recension
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string returnUrl, [Bind("Id,Review,Comment,CreatedAt,BookModelId,UserId, UserName")] ReviewModel reviewModel)
        {
            //Kontroll av id, om det inte hittas returneras not found
            if (id != reviewModel.Id)
            {
                return NotFound();
            }
            //Inloggad användare hämtas in
           var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
           //Om kontroll är korrekt och inloggad användare är samma som skapare kan inlägget uppdateras
            if (ModelState.IsValid && reviewModel.UserId == userId)
            {
                try
                {
                    _context.Update(reviewModel);
                    await _context.SaveChangesAsync();
                }
                //Fel fångas upp från databas
                catch (DbUpdateConcurrencyException)
                {
                    //Kontroll om inlägget finns
                    if (!ReviewModelExists(reviewModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            //Om returnurl finns skickas användaren tillbaka till föregående sida annars till index för books
            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }

                return RedirectToAction("Index", "Book"); 
            }
            ViewData["BookModelId"] = new SelectList(_context.Book, "Id", "Title", reviewModel.BookModelId);
            return View(reviewModel);
        }

    [Authorize]
        // GET: Review/Delete/5, funktion för att hämta radera-sidan för en recension
        public async Task<IActionResult> Delete(int? id)
        {
            //Om id inte finns returneras notfound
            if (id == null)
            {
                return NotFound();
            }

            //Check if_context.Review is null
            if(_context.Review == null)
            {
                return NotFound();
            }

            //Recension med givet id hämtas med bok-relation
            var reviewModel = await _context.Review
                .Include(r => r.Book)
                .FirstOrDefaultAsync(m => m.Id == id);
            //Om recensionen inte hittas returneras notfound
            if (reviewModel == null)
            {
                return NotFound();
            }

            //Skydda view för personer som inte skapade inlägget
            //Hämta in inloggad användare
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //kontroll om användare är skapare eller admin
            if (reviewModel.UserId != userId && !User.IsInRole("Admin")) return Forbid();

            ViewData["ReturnUrl"] = Request.Headers["Referer"].ToString();

            return View(reviewModel);
        }

    [Authorize]
        // POST: Review/Delete/5, funktion för att radera ett inlägg
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, string returnUrl)
        {

            //Check if_context.Review is null
            if(_context.Review == null)
            {
                return NotFound();
            }

            //Hämta inloggad användare
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //Hämta recension med givet id
            var reviewModel = await _context.Review.FindAsync(id);
            //Om validering är korrekt och användare är skapare eller admin kan radera ett inlägg
            if (reviewModel != null && reviewModel.UserId == userId || User.IsInRole("Admin"))
            {
                _context.Review.Remove(reviewModel);
            } else
            {
                return Forbid();
            }
            //Ändringarna sparas
            await _context.SaveChangesAsync();
            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }


            return RedirectToAction("Index", "Book"); 
        }

        //Kollar om en recension med specifiks id finns
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
