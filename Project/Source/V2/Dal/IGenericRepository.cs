using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Api213.V2.Helper;

namespace Api213.V2.Dal
{
    /// <inheritdoc />
    /// <summary>
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IGenericRepository<TEntity> : IDisposable 
        where TEntity : class
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="orderBy"></param>
        /// <param name="includeProperties"></param>
        /// <returns></returns>
        IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "");

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        TEntity GetByID(object id);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        void Insert(TEntity entity);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        void Delete(object id);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityToDelete"></param>
        void Delete(TEntity entityToDelete);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityToUpdate"></param>
        void Update(TEntity entityToUpdate);

        /// <summary>
        /// 
        /// </summary>
        void Save();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filteringSortingParams"></param>
        /// <returns></returns>
        IQueryable<dynamic> Get(FilteringSortingParams filteringSortingParams);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="asQueryable"></param>
        /// <param name="filteringSortingParams"></param>
        /// <returns></returns>
        IQueryable<dynamic> SortAndFieldsAndFilterList(
            IQueryable<TEntity> asQueryable,
            FilteringSortingParams filteringSortingParams);
    }
}