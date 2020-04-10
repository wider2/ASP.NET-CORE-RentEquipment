using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Bondora.Api.Models;

namespace Bondora.Api.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public ApplicationDbContext()
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

        public static string ConnectionString { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .Build();
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            ConnectionString = connectionString;

            optionsBuilder.UseSqlServer(ConnectionString);
        }

        public virtual DbSet<BondoraInventory> BondoraInventory { get; set; }

        public virtual DbSet<BondoraInventoryTypes> BondoraInventoryTypes { get; set; }

        public virtual DbSet<BondoraCart> BondoraCart { get; set; }

        public virtual DbSet<BondoraOrder> BondoraOrder { get; set; }

        public virtual DbSet<BondoraCustomer> BondoraCustomer { get; set; }

    }
}
