using System.Collections.Generic;
using System.Linq;
using Api213;
using Api213.V2.Dal;
using Api213.V2.Models;


namespace ApiXUnitTestProject
{
    /// <summary>
    /// 
    /// </summary>
    public static class Utilities
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="db"></param>
        public static void InitializeDbForTests(DataContext db)
        {
          

            db.Owners.AddRange(GetTestEntitiesOwner());
            db.PetInputs.AddRange(GetTestEntitiesPet());
            db.SaveChanges();
            db.PetInputs.First(p => p.Id == 1).Owner = db.Owners.First(o => o.Id == 1);
            db.PetInputs.First(p => p.Id == 2).Owner = db.Owners.First(o => o.Id == 2);
            db.PetInputs.First(p => p.Id == 3).Owner = db.Owners.First(o => o.Id == 2);
           db.SaveChanges();
        }
    

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<PetEntity> GetTestEntitiesPet()
        {
            return DbInitializer.GetTestEntities().Item1;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Owner> GetTestEntitiesOwner()
        {
            return DbInitializer.GetTestEntities().Item2;
        }

    }
}