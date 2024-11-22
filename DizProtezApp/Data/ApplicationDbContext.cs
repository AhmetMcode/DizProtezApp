using Microsoft.EntityFrameworkCore;
using DizProtezApp.Models;

namespace DizProtezApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSet tanımlamaları
        public DbSet<PlcData> PlcData { get; set; }
    }
}
