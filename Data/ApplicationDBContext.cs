using Microsoft.EntityFrameworkCore;

namespace ProductCategorySupplier.Data
{
    public class ApplicationDBContext: DbContext
    {
        public ApplicationDBContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {

        }

        public DbSet<Models.Domains.Product> Products { get; set; }
        public DbSet<Models.Domains.Category> Categories { get; set; }
        public DbSet<Models.Domains.Supplier> Suppliers { get; set; }

    } 
     
}
