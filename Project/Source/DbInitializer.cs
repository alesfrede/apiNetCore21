using System.Collections.Generic;
using System.Linq;
using Api213.V2.Dal;
using Api213.V2.Models;

namespace Api213
{/// <summary>
/// DbInitializer
/// </summary>
    public static class DbInitializer
    {
        /// <summary>
        /// Initialize
        /// </summary>
        /// <param name="context"></param>
        public static void Initialize(DataContext context)
        {
           context.PetInputs.AddRange(GetTestEntities());
            context.SaveChanges();
        }

        /// <summary>
        /// GetTestEntities
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<PetEntity> GetTestEntities()
        {
            return new[]
            {
                new PetEntity { Id = 1, Name = "Iga", Description = "Iga" },
                new PetEntity { Id = 2, Name = "Dogui", Description = "Dogui" },
                new PetEntity { Id = 3, Name = "Bebe", Description = "Bebe" }
            }.ToList();
        }
    }
}
