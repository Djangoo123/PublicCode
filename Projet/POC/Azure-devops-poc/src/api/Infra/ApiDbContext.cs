using Microsoft.EntityFrameworkCore;

namespace Api.Infra
{
        public class ApiDbContext : DbContext
        {
            public ApiDbContext(DbContextOptions<ApiDbContext> options)
                : base(options) { }
        }
}
