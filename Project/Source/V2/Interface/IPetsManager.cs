using System.Collections.Generic;
using System.Threading.Tasks;
using Api213.V2.Helper;
using Api213.V2.Models;

namespace Api213.V2.Interface
{
    /// <summary>
    /// IPetsManager
    /// </summary>
    public interface IPetsManager
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Task<PetEntity> ReadOne(string name);

  /// <summary>
  /// 
  /// </summary>
  /// <param name="pet"></param>
  /// <returns></returns>
        Task<PetEntity> Create(PetEntity pet);

        /// <summary>
        /// repalce entity
        /// </summary>
        /// <param name="pet"></param>
        /// <returns></returns>
        Task<PetEntity> Replace(PetEntity pet);

        /// <summary>
        /// update any field
        /// </summary>
        /// <param name="pet"></param>
        /// <returns></returns>
        Task<PetEntity> Update(PetEntity pet);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Task<PetEntity> Delete(string name);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filteringSortingParams"></param>
        /// <returns></returns>
        Task<IEnumerable<dynamic>> Get(FilteringSortingParams filteringSortingParams);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="namelike"></param>
        /// <param name="filteringSortingParams"></param>
        /// <returns></returns>
        IEnumerable<dynamic> GetByNameSubstring(string namelike, FilteringSortingParams filteringSortingParams);
    }
}