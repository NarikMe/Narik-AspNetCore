

namespace Narik.Common.Shared.Interfaces
{
    public interface INarikViewModel<T>
    {
        T ViewModelId { get; set; }
    }

    public interface INarikViewModel :INarikViewModel<long>
    {
    }
}
