using System;
using System.Collections.Generic;
using System.Data.Entity;
using DAL.Core.Interfaces;

namespace DAL.Core.EF
{
    public class RepositoryFactory : IRepositoryFactory
    {
        public readonly EFDbContext Context;

        public Dictionary<Type, object> CustomRepositoriesMappedByType { get; set; }

        public RepositoryFactory(EFDbContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context", "Context cannot be null");
            }

            this.Context = context;
        }

        public void SetCustomRepo<T>(IRepository<T> repository) where T : class, IModel
        {
            CustomRepositoriesMappedByType = CustomRepositoriesMappedByType ?? new Dictionary<Type, object>();

            var specializedType = typeof(T);
            if (CustomRepositoriesMappedByType.ContainsKey(specializedType))
            {
                CustomRepositoriesMappedByType[specializedType] = repository;
            }
            else
            {
                CustomRepositoriesMappedByType.Add(specializedType, repository);
            }
        }

        public virtual IRepository<T> GetRepository<T>() where T : class, IModel
        {
            if (this.CustomRepositoriesMappedByType == null || !this.CustomRepositoriesMappedByType.ContainsKey(typeof(T)))
            {
                return new GenericRepository<T>(this.Context);
            }

            object repository = null;
            this.CustomRepositoriesMappedByType.TryGetValue(typeof (T), out repository);

            return (IRepository<T>) (repository);

        }
    }
}