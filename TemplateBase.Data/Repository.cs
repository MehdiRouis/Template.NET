using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using TemplateBase.Core;

namespace TemplateBase.Data
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {

        #region Fields

        private readonly ApplicationDbContext _context;
        private DbSet<T> _entities;
        //private readonly IDbContext _context;
        //private DbSet<T> _entities;

        #endregion

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="context">Object context</param>
        public Repository(ApplicationDbContext context)
        {
            this._context = context;
        }

        #endregion


        #region Methods

        /// <summary>
        /// Get entity by identifier
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <returns>Entity</returns>
        public virtual T GetById(object id)
        {
            //see some suggested performance optimization (not tested)
            //http://stackoverflow.com/questions/11686225/dbset-find-method-ridiculously-slow-compared-to-singleordefault-on-id/11688189#comment34876113_11688189
            return Entities.Find(id);
        }

        /// <summary>
        /// Insert entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public virtual void Insert(T entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException(nameof(entity));

                Entities.Add(entity);

                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                //ensure that the detailed error text is saved in the Log
                //throw new CustomException(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Insert entities
        /// </summary>
        /// <param name="entities">Entities</param>
        public virtual void Insert(IEnumerable<T> entities)
        {
            try
            {
                if (entities == null)
                    throw new ArgumentNullException(nameof(entities));

                foreach (var entity in entities)
                    Entities.Add(entity);

                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                //ensure that the detailed error text is saved in the Log
                //throw new CustomException(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Update entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public virtual void Update(T entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException(nameof(entity));

                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                //ensure that the detailed error text is saved in the Log
                //throw new CustomException(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Update entities
        /// </summary>
        /// <param name="entities">Entities</param>
        public virtual void Update(IEnumerable<T> entities)
        {
            try
            {
                if (entities == null)
                    throw new ArgumentNullException(nameof(entities));

                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                //ensure that the detailed error text is saved in the Log
                //throw new CustomException(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Delete entity
        /// </summary>
        /// <param name="entity">Entity</param>
        public virtual void Delete(T entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException(nameof(entity));

                Entities.Remove(entity);

                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                //ensure that the detailed error text is saved in the Log
                //throw new CustomException(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Delete entities
        /// </summary>
        /// <param name="entities">Entities</param>
        public virtual void Delete(IEnumerable<T> entities)
        {
            try
            {
                if (entities == null)
                    throw new ArgumentNullException(nameof(entities));

                foreach (var entity in entities)
                    Entities.Remove(entity);

                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                //ensure that the detailed error text is saved in the Log
                //throw new CustomException(ex.Message);
                throw new Exception(ex.Message);
            }
        }/// <summary>
         ///     Detach entity
         /// </summary>
         /// <param name="entity"></param>
         /// <returns></returns>
        public virtual T Detach(T entity)
        {
            _context.Entry(entity).State = EntityState.Detached;
            if (entity.GetType().GetProperty("Id") != null) entity.GetType().GetProperty("Id")?.SetValue(entity, 0);
            return entity;
        }

        /// <summary>
        ///     Detach entities
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public virtual IList<T> Detach(IList<T> entities)
        {
            for (var i = 0; i < entities.Count; i++) Detach(entities[i]);
            return entities;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a table
        /// </summary>
        public virtual IQueryable<T> Table
        {
            get
            {
                var query = Entities.AsQueryable();

                /*foreach (var property in _context.Model.FindEntityType(typeof(T)).GetNavigations())
                    query = query.Include(property.Name);*/

                return query;
            }
        }

        /// <summary>
        /// Gets a table with "no tracking" enabled (EF feature) Use it only when you load record(s) only for read-only operations
        /// </summary>
        public virtual IQueryable<T> TableNoTracking
        {
            get
            {
                var query = Entities.AsQueryable().AsNoTracking();

                /*foreach (var property in _context.Model.FindEntityType(typeof(T)).GetNavigations())
                    query = query.Include(property.Name);*/

                return query;
            }
        }

        /// <summary>
        /// Entities
        /// </summary>
        protected virtual DbSet<T> Entities
        {
            get
            {
                if (_entities == null)
                    _entities = _context.Set<T>();
                return _entities;
            }
        }

        #endregion
    }
}
