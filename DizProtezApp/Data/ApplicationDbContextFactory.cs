using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DizProtezApp.Data
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer("Server=.\\;Database=DizProtezDB;Trusted_Connection=True;TrustServerCertificate=true;");

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
