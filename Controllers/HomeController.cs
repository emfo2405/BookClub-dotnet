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
        //Visa senaste bok och senaste kommentar
        var currentBook = await _context.Book
            .Include(b => b.Author)
            .Include(d => d.Chapters)
            .ThenInclude(c => c.Discussions)
            .ThenInclude(d => d.User)
            .OrderByDescending(b => b.Id)
            .FirstOrDefaultAsync();


        var latestComment = currentBook?.Chapters
        .SelectMany(c => c.Discussions)
        .OrderByDescending(d => d.Id)
        .FirstOrDefault();


    //Visa högst betygsatta böcker
    var topRated = await _context.Book
        .Include(b => b.Author)
        .Include(b => b.Reviews)
        .OrderByDescending(b => b.Reviews.Any()
        ? b.Reviews.Average(r => r.Review) : 0)
        .Take(3)
        .ToListAsync();

        var showView = new HomeModel
        {
            CurrentBook = currentBook,
            TopRated = topRated,
            LatestComment = latestComment
        };

            return View(showView);


    }


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
