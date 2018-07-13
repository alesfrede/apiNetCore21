using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using Api213.V2.Dal.Extension;
using Api213.V2.Helper;
using Api213.V2.Models;
using Microsoft.EntityFrameworkCore;

namespace Api213.V2.Dal
{
    /// <inheritdoc />
    /// <summary>
    /// IGenericRepository
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class GenericRepository<TEntity> : IGenericRepository<TEntity>
        where TEntity : class
    {
        private readonly DbContext _context;
        private readonly DbSet<TEntity> _dbSet;
        private bool _disposed;

        /// <inheritdoc />
        public GenericRepository(DbContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="orderBy"></param>
        /// <param name="includeProperties"></param>
        /// <returns></returns>
        public IQueryable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "")
        {
            IQueryable<TEntity> query = Includes(includeProperties);

            if (filter != null)
                query = query.Where(filter);

            return orderBy?.Invoke(query).AsQueryable() ?? query;
        }

        /// <inheritdoc />
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TEntity GetByID(object id)
        {
            return _dbSet.Find(id);
        }

        /// <summary>
        /// </summary>
        /// <param name="entity"></param>
        public void Insert(TEntity entity)
        {
            _dbSet.Add(entity);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        public void Delete(object id)
        {
            var entityToDelete = _dbSet.Find(id);
            Delete(entityToDelete);
        }

        /// <summary>
        /// </summary>
        /// <param name="entityToDelete"></param>
        public void Delete(TEntity entityToDelete)
        {
            if (_context.Entry(entityToDelete).State == EntityState.Detached)
                _dbSet.Attach(entityToDelete);
            _dbSet.Remove(entityToDelete);
        }

        /// <summary>
        /// </summary>
        /// <param name="entityToUpdate"></param>
        public void Update(TEntity entityToUpdate)
        {
            _dbSet.Attach(entityToUpdate);
            _context.Entry(entityToUpdate).State = EntityState.Modified;
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public void Save()
        {
            _context.SaveChanges();
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="filteringSortingParams"></param>
        /// <returns></returns>
        public IQueryable<dynamic> Get(FilteringSortingParams filteringSortingParams)
        {
            var query1 = Includes(filteringSortingParams.IncludeProperties);

            var query = SortAndFieldsAndFilterList(query1, filteringSortingParams);

            query = PagingQuery(filteringSortingParams, query);
            
            return query.AsQueryable();
        }
        
        /// <inheritdoc />
        /// <summary>
        /// </summary>
        /// <param name="asQueryable"></param>
        /// <param name="filteringSortingParams"></param>
        /// <returns></returns>
        public IQueryable<dynamic> SortAndFieldsAndFilterList(
            IQueryable<TEntity> asQueryable,
            FilteringSortingParams filteringSortingParams)
        {
            IEnumerable<TEntity> enumerable = asQueryable;

            var listdynamic = new List<dynamic>();
            if (!string.IsNullOrEmpty(filteringSortingParams.Sort))
                enumerable = asQueryable.Sort(filteringSortingParams.Sort);

            if (string.IsNullOrEmpty(filteringSortingParams.Fields))
            {
                listdynamic.AddRange(enumerable);
                return listdynamic.AsQueryable();
            }
            
            var objectsLimitingFields = enumerable.AsQueryable()
                .LimitingFields(filteringSortingParams.Fields, typeof(PetEntity)).ToDynamicList().AsQueryable();

            var newlistdynamic = new List<dynamic>();
            foreach (object o in objectsLimitingFields)
                newlistdynamic.Add(o);
            objectsLimitingFields = newlistdynamic.AsQueryable();
            return objectsLimitingFields;
        }

       /// <inheritdoc />
       /// <summary>
       /// </summary>
       /// <param name="includeProperties"></param>
       /// <returns></returns>
        public IQueryable<TEntity> Includes(string includeProperties)
        {
            var query1 = _dbSet.AsQueryable();
            foreach (var includeProperty in includeProperties.Split(
                new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                query1 = _dbSet.Include(includeProperty);

            return query1;
        }

        /// <summary>
        /// </summary>
        /// <param name="disposing"></param>
        private void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
                _context.Dispose();
            _disposed = true;
        }

        /// <summary>
        /// </summary>
        /// <param name="filteringSortingParams"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        private static IQueryable<dynamic> PagingQuery(
            FilteringSortingParams filteringSortingParams,
            IQueryable<dynamic> query)
        {
            return PagingQuery(filteringSortingParams, query, out _);
        }

        /// <summary>
        /// </summary>
        /// <param name="filteringSortingParams"></param>
        /// <param name="query"></param>
        /// <param name="pagedList"></param>
        /// <returns></returns>
        private static IQueryable<dynamic> PagingQuery(
            FilteringSortingParams filteringSortingParams,
            IQueryable<dynamic> query,
            out PagedList<dynamic> pagedList)
        {
            pagedList = null;
            if (filteringSortingParams.Page > 0)
            {
                var paginado = new PagedList<dynamic>(query, filteringSortingParams.Page, filteringSortingParams.Count);
                query = paginado.List.AsQueryable();
            }

            return query;
        }
    }
}