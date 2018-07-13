using System.Collections.Generic;
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
            var lista = GetTestEntitiesPet();
            db.PetInputs.AddRange(lista);
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