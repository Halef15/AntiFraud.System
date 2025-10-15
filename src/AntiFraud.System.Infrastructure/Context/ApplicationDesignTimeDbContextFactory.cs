using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AntiFraud.System.Infrastructure.Context
{
    /// <summary>
    /// Classe responsável por funcionalidades de applicationdesigntimedbcontextfactory.
    /// </summary>
    public class ApplicationDesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        #region Public Methods/Operators
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=AntiFraudDb_Dev;Username=postgres;Password=AntiFraud@123");

            return new ApplicationDbContext(optionsBuilder.Options);
        }
        #endregion
    }
}