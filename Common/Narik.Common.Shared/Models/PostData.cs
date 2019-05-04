using Narik.Common.Shared.Interfaces;
using Newtonsoft.Json;

namespace Narik.Common.Shared.Models
{
    public class PostData<T> : IPostData<T>
        where T : class
    {
        public T Entity { get; set; }
        public bool? IsRetry { get; set; }
        public INarikViewModel GetEntity()
        {
            return Entity as INarikViewModel;
        }
    }

    public class MasterDetailPostData<T, TItem> : PostData<T>
        where T : class
        where TItem : class
    {
        [JsonProperty("changes")]
        public Change<TItem> Items { get; set; }
    }

    public class ChangePostData<T> : IPostData
        where T : class
    {
        [JsonProperty("items")]
        public Change<T> Items { get; set; }

        public INarikViewModel GetEntity()
        {
            return null;
        }
    }
}
