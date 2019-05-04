using System;

namespace Narik.Common.Data
{
    public interface  INarikModel<out T>
    {
        T Id { get;  }
    }

    public interface ITrackableModel
    {
        DateTime InsertDate { get; set; }
        DateTime LastUpdateDate { get; set; }
        int Creator { get; set; }
        int LastEditor { get; set; }
    }
}
