using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Core.Interfaces;

namespace DAL.Core.EF
{
    public static class DALExtensions
    {
        public static EntityState ToEntityState(this RecordState recordState)
        {
            switch (recordState)
            {
                case RecordState.Added:
                    return EntityState.Added;

                case RecordState.Deleted:
                    return EntityState.Deleted;

                case RecordState.Updated:
                    return EntityState.Modified;

                default:
                    return EntityState.Unchanged;
            }

        }
    }
}
