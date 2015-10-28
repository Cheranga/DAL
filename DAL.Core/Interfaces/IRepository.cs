using System.Linq;

namespace DAL.Core.Interfaces
{
    public interface IRepository<T> where T : class, IModel
    {
        T GetById(int id);

        IQueryable<T> GetAll();

        T Add(T entity);

        void Delete(T entity);

        void Delete(int id);

        void Update(T entity);

        IUoW UoW { get; set; }
    }

    public enum RecordState
    {
        Added,
        Updated,
        Deleted,
        Unchanged
    }
}
