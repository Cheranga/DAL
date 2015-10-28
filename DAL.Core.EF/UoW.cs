using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using DAL.Core.Interfaces;

namespace DAL.Core.EF
{
    public class UoW : IUoW
    {
        #region Members

        public virtual EFDbContext Context { get; set; }
        public virtual IRepositoryFactory repositoryFactory { get; set; }

        #endregion

        #region Constructor

        public UoW(EFDbContext context, IRepositoryFactory repositoryFactory)
        {
            if (context == null)
            {
                throw new ArgumentException("context cannot be null");
            }

            if (repositoryFactory == null)
            {
                throw new ArgumentException("repositoryFactory cannot be null");
            }

            this.Context = context;
            this.repositoryFactory = repositoryFactory;
        }

        #endregion

        #region IUoW Implementation

        public virtual IRepository<T> GetRepository<T>() where T : class, IModel
        {
            var repository = this.repositoryFactory.GetRepository<T>();

            //
            // If there's no uow set, set the uow as the current instance
            //
            if (repository != null && repository.UoW == null)
            {
                repository.UoW = this;
            }

            return repository;
        }

        public virtual IDataResult Commit(Action action = null)
        {

            try
            {

                if (action != null)
                {
                    action();
                }

                this.Context.SaveChanges();

                var dataResult = new DataResult
                {
                    Status = true
                };

                return dataResult;
            }
            catch (Exception exception)
            {

                var dataResult = new DataResult
                {
                    Exception = exception
                };

                return dataResult;
            }

        }

        public IQueryable<T> Get<T>(Expression<Func<T, bool>> filter) where T : class, IModel
        {
            var repository = this.GetRepository<T>();

            if (repository == null)
            {
                return null;
            }

            return filter == null ? repository.GetAll() : repository.GetAll().Where(filter);
        }

        #endregion
    }
}