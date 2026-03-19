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
using Microsoft.Extensions.Configuration.UserSecrets;
using OpenAI.Chat;
using System.Diagnostics;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;

namespace BookClub.Controllers
{
    public class DiscussionController : Controller
    {


        private readonly ApplicationDbContext _context;
        private readonly ChatClient _chatClient;

        public DiscussionController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            //API-key för openAI inkluderas och AI-modell inkluderas
            var openAIKey = configuration["OpenAI:ApiKey"];
            var modelName = configuration["OpenAI:Model"];
            //Ny chatClient instans skapas
            _chatClient = new ChatClient(modelName, openAIKey);            
        }


        // GET: Discussion, funktion för att hämta in diskussion från databas
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

        // GET: Discussion/Details/5, funktion för att hämta in en detaljsida för diskussionsinlägg
        public async Task<IActionResult> Details(int? id)
        {
            //Om id inte hittas returneras notfound
            if (id == null)
            {
                return NotFound();
            }

            //Check if_context.Discussion is null
            if(_context.Discussion == null)
            {
                return NotFound();
            }
            //Om kontroller är korrekt hämtas diskussion med givet id och relation
            var discussionModel = await _context.Discussion
                .Include(d => d.Chapter)
                .FirstOrDefaultAsync(m => m.Id == id);
            //Annars returneras not found
            if (discussionModel == null)
            {
                return NotFound();
            }

            return View(discussionModel);
        }

    [Authorize]
        // GET: Discussion/Create, funktion för att hämta sidan för att skapa ett diskussionsinlägg
        public IActionResult Create(int chapterId)
        {
            //ReturnURL sparas som viewdata för att användas för en tillbaka knapp
            ViewData["ReturnUrl"] = Request.Headers["Referer"].ToString();

            //Håller koll på vilket kapitel som klickades på och hämtar det
            var chapter = _context.Chapter.FirstOrDefault(c => c.Id == chapterId);

            //Hämtar alla kapitel för boken med id BookModelId
            var chapters = _context.Chapter
            .Include(c => c.Book)
            .Where(c => c.BookModelId == chapter.BookModelId)
            //Skapar en lista där kapitel visas med nummer och namn för den valda boken
            .Select(c => new{c.Id, Display = "Kapitel " + c.Number + " - " + c.Book.Title})
            .ToList();
            //Skapar en dropdown lista där förvalda är det kapitlet användaren klickade på
            ViewData["ChapterModelId"] = new SelectList(chapters, "Id", "Display", chapterId);
            ViewData["BookId"] = chapter.BookModelId;
            return View();
        }

    [Authorize]
        // POST: Discussion/Create, funktion för att skapa ett diskussionsinlägg
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Content,CreatedAt,ChapterModelId")] DiscussionModel discussionModel, string returnUrl, int bookId)
        {
            //Hämtar alla kapitel för boken med id BookModelId
            var chapters = _context.Chapter
            .Include(c => c.Book)
            .Where(c => c.BookModelId == bookId)
            //Skapar en lista där kapitel visas med nummer och namn för den valda boken
            .Select(c => new{c.Id, Display = "Kapitel " + c.Number + " - " + c.Book.Title})
            .ToList();
            
            //Kontroll av input
            if (ModelState.IsValid)
            {

              //Inloggad användare läggs till till inlägget
                discussionModel.UserName = User.Identity?.Name ?? "Unknown";
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                discussionModel.UserId = userId;



            //Lägg till promt till AI där svar ska returneras som JSON med givet format
            try
                {
                    var moderationPromt = @"Du är en AI för innehållsmoderering för en bokklubb och ska 
                    kontrollera innehåll som publiceras till ett diskussionsforum. Analysera följande 
                    innehåll skapat av en användare och bedöm om det är lämpligt innehåll för diskussionsforumet.
                    Svara på svenska.
                    Svara i ren , korrekt JSON utan markdown i följande format:
                    {
                    IsAllowed: true/false,
                    Motivation: text
                    }
                    där du förklarar och motiverar din bedömning.
                    Användarinnehåll: ''" + discussionModel.Content + @"''";

                    //Prompt skickas till AI-modell och svaret tas emot
                    var response = await _chatClient.CompleteChatAsync(moderationPromt);
                    var aiResponse = response.Value.Content[0].Text;

                    //Svar görs om enligt modellen Moderation
                    var moderationResult = System.Text.Json.JsonSerializer.Deserialize<Moderation>(aiResponse);

                    //Om svar inte kan tolkas
                    if(moderationResult == null)
                    {
                    ViewBag.ResponseMessage = "Kunde inte tolka svaret...";
            ViewData["ChapterModelId"] = new SelectList(chapters, "Id", "Display", discussionModel.ChapterModelId);
                    return View(discussionModel);
                    }
                    //Om användarens inlägg är tillåtet sparas inlägget och läggs till i forumet
                    if(moderationResult.IsAllowed)
                    {
                        _context.Add(discussionModel);
                        await _context.SaveChangesAsync();
                    ViewBag.ResponseMessage = "Inlägget är godkänt för publicering"; 
                    ViewBag.Motivation = moderationResult.Motivation;
            ViewData["ChapterModelId"] = new SelectList(chapters, "Id", "Display", discussionModel.ChapterModelId);
                        return View(discussionModel);
                    }
                    //Om användarens inlägg inte är tillåtet och läggs inte till
                    ViewBag.ResponseMessage = "Inlägget är inte tillåtet"; 
                    ViewBag.Motivation = moderationResult.Motivation;
            ViewData["ChapterModelId"] = new SelectList(chapters, "Id", "Display", discussionModel.ChapterModelId);
                    return View(discussionModel);
                //Fel fångas upp
                } catch (Exception ex)
                {
                 Console.WriteLine("Error " + ex);
                             ViewData["ChapterModelId"] = new SelectList(chapters, "Id", "Display", discussionModel.ChapterModelId);
                return View(discussionModel);
                }
                    
                
            }
            ViewData["ChapterModelId"] = new SelectList(chapters, "Id", "Display", discussionModel.ChapterModelId);
            return View(discussionModel);
        }

    [Authorize]
        // GET: Discussion/Edit/5, funktion för att hämta in en redigeringssida för diskussionsinlägg
        public async Task<IActionResult> Edit(int? id)
        {
            //Om id inte hittas returneras notfound
            if (id == null)
            {
                return NotFound();
            }

            //Check if_context.Discussion is null
            if(_context.Discussion == null)
            {
                return NotFound();
            }

            //Diskussion hittas med givet id, annars returneras notfound
            var discussionModel = await _context.Discussion.FindAsync(id);
            if (discussionModel == null)
            {
                return NotFound();
            }

            //Förhindra att andra än skaparen kan redigera inlägget
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (discussionModel.UserId != userId) return Forbid();

            //URL sparas från föregående sida, kapitel-id skickas med från föregående sida
            ViewData["ReturnUrl"] = Request.Headers["Referer"].ToString();
            ViewData["ChapterModelId"] = new SelectList(_context.Chapter, "Id", "Number", discussionModel.ChapterModelId);
            return View(discussionModel);
        }

    [Authorize]
        // POST: Discussion/Edit/5, funktion för att uppdatera ett inlägg
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string returnUrl, [Bind("Id,Title,Content,CreatedAt,ChapterModelId, UserId, UserName")] DiscussionModel discussionModel)
        {
            //Om id inte hittas returneras notfound
            if (id != discussionModel.Id)
            {
                return NotFound();
            }

            //Hämtar id från inloggad användare
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //Kontrollerar input och att den inloggade användaren är den som har skapat inlägget
            if (ModelState.IsValid && discussionModel.UserId == userId)
            {
                //Om kontroller lyckas sparas uppdatering
                try
                {
                    _context.Update(discussionModel);
                    await _context.SaveChangesAsync();
                }
                //Fel fångas upp från databas
                catch (DbUpdateConcurrencyException)
                {
                    //Om inget inlägg med givet id finns returneras not found
                    if (!DiscussionModelExists(discussionModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

            //Kontroll av returnURL som ska ta användaren till föregående sida
            if (!string.IsNullOrEmpty(returnUrl))
            {
                //Om det finns returneras användaren annars skickas den till index för books
                return Redirect(returnUrl);
            }
                return RedirectToAction("Index", "Book");
            }
            ViewData["ChapterModelId"] = new SelectList(_context.Chapter, "Id", "Number", discussionModel.ChapterModelId);
            return View(discussionModel);
        }

    [Authorize]
        // GET: Discussion/Delete/5, funktion frö att hämta sidan för att radera ett inlägg
        public async Task<IActionResult> Delete(int? id)
        {
            //Om id inte hittas returneras notfound
            if (id == null)
            {
                return NotFound();
            }

            //Check if_context.Discussion is null
            if(_context.Discussion == null)
            {
                return NotFound();
            }
            //Inlägg hämtas in med givet id och relationer inkluderas
            var discussionModel = await _context.Discussion
                .Include(d => d.Chapter)
                .FirstOrDefaultAsync(m => m.Id == id);
            //Om det inte hittas returneras notfound
            if (discussionModel == null)
            {
                return NotFound();
            }

            //Hämta in inloggad användare och skydda view från personer som inte är skapare eller admin
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (discussionModel.UserId != userId && !User.IsInRole("Admin")) return Forbid();

            ViewData["ReturnUrl"] = Request.Headers["Referer"].ToString();
            return View(discussionModel);
        }

    [Authorize]
        // POST: Discussion/Delete/5, funktion för att radera ett inlägg
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id, string returnUrl)
        {

            //Check if_context.Discussion is null
            if(_context.Discussion == null)
            {
                return NotFound();
            }

            //Hämta in inloggad användare och inlägg med givet id och kontrollera uppgifter
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var discussionModel = await _context.Discussion.FindAsync(id);
            if (discussionModel != null && discussionModel.UserId == userId || User.IsInRole("Admin"));
            //Om allt är korrekt tas inlägget bort
            {
                _context.Discussion.Remove(discussionModel);
            }
            //Ändringar sparas och användaren tas tillbaka till föregående sida
            await _context.SaveChangesAsync();
            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Book");
        }

        //Funktion för att hitta ett diskussionsinlägg med givet id
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
