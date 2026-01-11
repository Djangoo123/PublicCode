using Microsoft.EntityFrameworkCore;

namespace KataBaseProject.Infrastructure
{
    public class KataDbContext : DbContext
    {
        public KataDbContext(DbContextOptions<KataDbContext> options)
            : base(options) { }
    }

}
