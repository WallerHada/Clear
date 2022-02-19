using Microsoft.EntityFrameworkCore;
using ProjectWebAPI.Models.Tables;

namespace ProjectWebAPI.Models
{
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions<MyContext> options)
          : base(options)
        {
        }

        public DbSet<Blog> Blog { get; set; }
    }
}
