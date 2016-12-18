using ProjetoModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ProjetoService
{
    /// <summary>
    /// Entity Framework DbContext for Contoso.
    /// </summary>
    public class ProjetoContext : DbContext
    {
        private const string CsDev = @"<TODO: Insert connection string>";

        private const string CsProd = @"<TODO: Insert connection string>"; 

        /// <summary>
        /// Creates a new Contoso DbContext.
        /// </summary>
        public ProjetoContext() : base(CsProd)
        {
            AppDomain.CurrentDomain.SetData("DataDirectory",
                System.IO.Directory.GetCurrentDirectory());
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<ProjetoContext>());
            Database.CommandTimeout = 90;
        }

        /// <summary>
        /// Gets the customers DbSet.
        /// </summary>
        public DbSet<Customer> Customers { get; set; }

        /// <summary>
        /// Gets the orders DbSet.
        /// </summary>
        public DbSet<Negotiation> Orders { get; set; }

        /// <summary>
        /// Gets the products DbSet.
        /// </summary>
        public DbSet<Building> Products { get; set; }
    }
}