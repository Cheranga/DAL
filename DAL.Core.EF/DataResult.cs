using System;
using DAL.Core.Interfaces;

namespace DAL.Core.EF
{
    public class DataResult : IDataResult
    {
        public bool Status { get; set; }

        public Exception Exception { get; set; }
    }
}