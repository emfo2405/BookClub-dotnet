using BookClub.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookClub.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : base(options)
    {
        
    }

   public DbSet<AuthorModel>? Author { get; set; } 
   public DbSet<BookModel>? Book { get; set; }
   public DbSet<ChapterModel>? Chapter { get; set; }
   public DbSet<DiscussionModel>? Discussion { get; set; }
   public DbSet<ReviewModel>? Review { get; set; }    
}


