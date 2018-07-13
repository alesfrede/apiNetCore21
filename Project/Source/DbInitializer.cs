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
            context.Owners.AddRange(GetTestEntities().Item2);
            context.PetInputs.AddRange(GetTestEntities().Item1);
            context.SaveChanges();
            context.PetInputs.First(p => p.Id == 1).Owner = context.Owners.First(o => o.Id == 1);
            context.PetInputs.First(p => p.Id == 2).Owner = context.Owners.First(o => o.Id == 2);
            context.PetInputs.First(p => p.Id == 3).Owner = context.Owners.First(o => o.Id == 2);
            context.SaveChanges();
        }

        /// <summary>
        /// GetTestEntities
        /// </summary>
        /// <returns></returns>
        public static(IEnumerable<PetEntity>, IEnumerable<Owner>) GetTestEntities()
        {
            var owners = new[]
            {
             new Owner {Id = 1, Name = "Juan"},
             new Owner { Id = 2, Name = "Pedro" }
            };
            return (new[]
            {
                new PetEntity { Id = 1, Name = "Iga", Description = "Iga", Owner = null },
                new PetEntity { Id = 2, Name = "Dogui", Description = "Dogui", Owner = null},
                new PetEntity { Id = 3, Name = "Bebe", Description = "Bebe", Owner = null }
            }, owners);
        }
    }
}
