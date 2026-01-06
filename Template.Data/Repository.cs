using Microsoft.EntityFrameworkCore;
using Template.Core;
using Template.Core.Domain.Entities;

namespace Template.Data
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {

        #region Fields

        private readonly ApplicationDbContext _context;
        private DbSet<T> _entities;

        #endregion Fields

        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="context">Object context</param>
        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _entities = _context.Set<T>();
        }

        #endregion Ctor


        #region Methods

        /// <summary>
        /// Get entity by identifier
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <returns>Entity</returns>
        public virtual T? GetById(object id)
        {
            return _entities.Find(id);
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

                _entities.Add(entity);

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
                    _entities.Add(entity);

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

                _entities.Remove(entity);

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
                    _entities.Remove(entity);

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

        public virtual IQueryable<T> Table => _entities;

        public virtual IQueryable<T> TableNoTracking => _entities.AsNoTracking();

        #endregion
    }
}
