using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BookClub.Models;
using BookClub.Data;
using Microsoft.EntityFrameworkCore;

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
        var currentBook = await _context.Book
            .Include(b => b.Author)
            .OrderByDescending(b => b.Id)
            .FirstOrDefaultAsync();

            return View(currentBook);

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
