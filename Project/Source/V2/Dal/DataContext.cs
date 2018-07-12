using Api213.V2.Models;
using Microsoft.EntityFrameworkCore;

namespace Api213.V2.Dal
{
    /// <inheritdoc />
    public class DataContext : DbContext
    {
        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="DataContext"/> class.
        /// DataContext
        /// </summary>
        /// <param name="options"></param>
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public DbSet<PetEntity> PetInputs { get; set; }
    }
}
