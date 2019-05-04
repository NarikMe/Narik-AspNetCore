using System;
using System.Collections.Generic;
using System.Text;

namespace Narik.Common.Shared.Interfaces
{
    public interface  IPostData
    {
        INarikViewModel GetEntity();
        
    }

    public interface IPostData<T> : IPostData
        where T : class
    {
        T Entity { get; set; }
        bool? IsRetry { get; }
    }
}
