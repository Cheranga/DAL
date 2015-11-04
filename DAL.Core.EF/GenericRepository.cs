using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using DAL.Core.Interfaces;

namespace DAL.Core.EF
{
    public class GenericRepository<T> : IRepository<T> where T : class, IModel
    {
        #region Members

        public readonly EFDbContext Context;

        protected readonly DbSet<T> dbSet = null;

        #endregion

        #region Constructor

        public GenericRepository(EFDbContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context", "context cannot be null");
            }

            this.Context = context;

            this.dbSet = this.Context.Set<T>();

            if (this.dbSet == null)
            {
                throw new NullReferenceException("There is no entity defined in this DbContext");
            }
        }

        #endregion

        #region IRepository Implementations

        public virtual IUoW UoW { get; set; }

        public virtual T GetById(int id)
        {
            return this.dbSet.Find(id);
        }

        public virtual IQueryable<T> GetAll()
        {
            return this.dbSet;
        }

        public virtual T Add(T entity)
        {
            if (entity == null)
            {
                return null;
            }

            var addedEntity = this.dbSet.Add(entity);
            return addedEntity;
        }

        public virtual void Delete(T entity)
        {
            if (entity == null)
            {
                return;
            }

            var existingEntity = this.GetById(entity.Id);
            if (existingEntity == null)
            {
                return;
            }

            this.Context.ChangeState(existingEntity, RecordState.Deleted);
        }

        public virtual void Delete(int id)
        {
            var entity = this.GetById(id);
            this.Delete(entity);
        }

        public virtual void Update(T entity)
        {
            if (entity == null)
            {
                return;
            }

            var existingEntity = this.GetById(entity.Id);
            if (existingEntity == null)
            {
                return;
            }

            this.Context.ChangeState(existingEntity, RecordState.Updated);


        }
        

        #endregion

        #region Helper Functions

        public virtual IQueryable<T> Get(Expression<Func<T, bool>> filter)
        {
            return filter == null ? GetAll() : this.dbSet.Where(filter).AsQueryable();
        }

        #endregion
    }
}
