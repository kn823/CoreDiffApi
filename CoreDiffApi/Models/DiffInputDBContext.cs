using Microsoft.EntityFrameworkCore;

namespace CoreDiffApi.Models
{
    /// <summary>
    /// DiffInputDBContext provides database connection and access.
    /// Program.cs configures this as in In-Memory database.
    /// </summary>
    public class DiffInputDBContext : DbContext
    {
        public DbSet<DiffInput> DiffInputs { get; set; }

        public DiffInputDBContext(DbContextOptions<DiffInputDBContext> options)
            : base(options)
        {

        }
    }
}
