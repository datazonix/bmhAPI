using Microsoft.EntityFrameworkCore;

namespace bmhAPI.Data  // 👈 match your project namespace
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Example: Add tables here
      
    }
}
