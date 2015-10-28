using System;

namespace DAL.Core.Interfaces
{
    public interface IDataResult
    {
        bool Status { get; set; }
        Exception Exception { get; set; }
    }
}