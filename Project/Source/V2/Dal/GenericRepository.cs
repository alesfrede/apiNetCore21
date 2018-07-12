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
    /// <summary>
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

        /// <summary>
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="orderBy"></param>
        /// <param name="includeProperties"></param>
        /// <returns></returns>
        public virtual IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "")
        {
            IQueryable<TEntity> query = _dbSet;

            if (filter != null)
                query = query.Where(filter);

            foreach (var includeProperty in includeProperties.Split(
                new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                query = query.Include(includeProperty);

            if (orderBy != null)
                return orderBy(query).ToList();
            return query.ToList();
        }

        /// <summary>
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual TEntity GetByID(object id)
        {
            return _dbSet.Find(id);
        }

        /// <summary>
        /// </summary>
        /// <param name="entity"></param>
        public virtual void Insert(TEntity entity)
        {
            _dbSet.Add(entity);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        public virtual void Delete(object id)
        {
            var entityToDelete = _dbSet.Find(id);
            Delete(entityToDelete);
        }

        /// <summary>
        /// </summary>
        /// <param name="entityToDelete"></param>
        public virtual void Delete(TEntity entityToDelete)
        {
            if (_context.Entry(entityToDelete).State == EntityState.Detached)
                _dbSet.Attach(entityToDelete);
            _dbSet.Remove(entityToDelete);
        }

        /// <summary>
        /// </summary>
        /// <param name="entityToUpdate"></param>
        public virtual void Update(TEntity entityToUpdate)
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

        /// <summary>
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// </summary>
        /// <param name="filteringSortingParams"></param>
        /// <returns></returns>
        public IQueryable<dynamic> Get(FilteringSortingParams filteringSortingParams)
        {
            var query = SortAndFieldsAndFilterList(_dbSet, filteringSortingParams);
            query = PagingQuery(filteringSortingParams, query);
            return query.AsQueryable();
        }
        
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

        /// <summary>
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
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