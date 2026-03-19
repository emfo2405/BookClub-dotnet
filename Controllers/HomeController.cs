using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BookClub.Models;
using BookClub.Data;
using Microsoft.EntityFrameworkCore;
using BookClub.Models;

namespace BookClub.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        //Visa senaste bok och inkludera relationer, sorteras på id
        var currentBook = await _context.Book
            .Include(b => b.Author)
            .OrderByDescending(b => b.Id)
            .FirstOrDefaultAsync();


    //Visa högst betygsatta böcker och inkludera relationer, sorteras efter högst medelvärde av betyg och visar tre böcker
    var topRated = await _context.Book
        .Include(b => b.Author)
        .Include(b => b.Reviews)
        .OrderByDescending(b => b.AverageRating ?? 0)
        .Take(3)
        .ToListAsync();

        //Skapar en modell för att kunna visa data på hemskärmen
        var showView = new HomeModel
        {
            CurrentBook = currentBook,
            TopRated = topRated,
        };

            //Skickas modellen till viewn
            return View(showView);


    }


    //Funktion för att returnera en privacy-sida
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
