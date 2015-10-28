using System;
using System.Linq;
using System.Linq.Expressions;

namespace DAL.Core.Interfaces
{
    public interface IUoW
    {
        IRepository<T> GetRepository<T>() where T : class, IModel;
        IDataResult Commit(Action action = null);
        IQueryable<T> Get<T>(Expression<Func<T, bool>> filter) where T : class, IModel;

    }
}