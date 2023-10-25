using HobbyHarbour.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HobbyHarbour.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        public DbSet<Category> Categories { get; set; }

        public DbSet<Product> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //here we can write code for seed any database entity or table. for example look at below
            //modelBuilder.Entity<Category>().HasData(
            //	new Category { CategoryID = 1, CategoryName = "Soft Toys", Description = "Soft Toys for kids" },

            //             new Category { CategoryID = 2, CategoryName = "Hobby Kit", Description = "Hobby kit for kids" },

            //             new Category { CategoryID = 3, CategoryName = "Board Games", Description = "Board games for kids" }

            //             );
        }
    }
}


