using DAL.Core.Interfaces;

namespace DAL.Core.EF
{
    public interface IChangeState
    {
        void ChangeState(object entity, RecordState state);
    }
}