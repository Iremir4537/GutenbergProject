using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SoftaweEngineering.Models;

namespace SoftaweEngineering.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<SoftaweEngineering.Models.User> User { get; set; }
        public DbSet<SoftaweEngineering.Models.Library> Library { get; set; }
        public DbSet<SoftaweEngineering.Models.Library_Book> Library_Book { get; set; }
    }
}