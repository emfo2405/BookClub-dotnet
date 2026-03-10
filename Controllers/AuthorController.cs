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

        // GET: Author
        public async Task<IActionResult> Index()
        {
            //Check if_context.Author is null
            if(_context.Author == null)
            {
                return NotFound();
            }

            return View(await _context.Author.ToListAsync());
        }

        // GET: Author/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            //Check if_context.Author is null
            if(_context.Author == null)
            {
                return NotFound();
            }


            var authorModel = await _context.Author
                .FirstOrDefaultAsync(m => m.Id == id);
            if (authorModel == null)
            {
                return NotFound();
            }
            if (User.IsInRole("Admin"))
            {
            return View(authorModel);                
            } else
            {
                return View("AccessDenied");               
            }

        }

    [Authorize]
        // GET: Author/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Author/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Country")] AuthorModel authorModel)
        {
            if (ModelState.IsValid && User.IsInRole("Admin"))
            {
                _context.Add(authorModel);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(authorModel);
        }

    [Authorize]
        // GET: Author/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            //Check if_context.Author is null
            if(_context.Author == null)
            {
                return NotFound();
            }


            var authorModel = await _context.Author.FindAsync(id);
            if (authorModel == null)
            {
                return NotFound();
            }
            if(User.IsInRole("Admin"))
            {
            return View(authorModel);                
            } else
            {
                return View("AccessDenied");
            }

        }

    [Authorize]
        // POST: Author/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Country")] AuthorModel authorModel)
        {
            if (id != authorModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid && User.IsInRole("Admin"))
            {
                try
                {
                    _context.Update(authorModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AuthorModelExists(authorModel.Id))
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
            return View(authorModel);
        }

    [Authorize]
        // GET: Author/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            //Check if_context.Author is null
            if(_context.Author == null)
            {
                return NotFound();
            }


            var authorModel = await _context.Author
                .FirstOrDefaultAsync(m => m.Id == id);
            if (authorModel == null)
            {
                return NotFound();
            }
            if (User.IsInRole("Admin"))
            {
            return View(authorModel);                
            } else
            {
                return View("AccessDenied");
            }

        }

    [Authorize]
        // POST: Author/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //Check if_context.Author is null
            if(_context.Author == null)
            {
                return NotFound();
            }

            var authorModel = await _context.Author.FindAsync(id);
            if (authorModel != null && User.IsInRole("Admin"))
            {
                _context.Author.Remove(authorModel);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

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
