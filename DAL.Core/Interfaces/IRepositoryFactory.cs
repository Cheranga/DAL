namespace DAL.Core.Interfaces
{
    public interface IRepositoryFactory
    {
        IRepository<T> GetRepository<T>() where T : class, IModel;
    }
}