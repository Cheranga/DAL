using System.Data.Entity;
using DAL.Core.Interfaces;

namespace DAL.Core.EF
{
    public class EFDbContext : DbContext, IChangeState
    {
        public virtual void ChangeState(object entity, RecordState state)
        {
            if (entity == null)
            {
                return;
            }

            var dbEntity = this.Entry(entity);
            
            if (dbEntity == null)
            {
                return;
            }

            dbEntity.State = state.ToEntityState();
        }
    }
}