namespace Narik.Common.Shared.Models
{
    public class IdVersionCreator<TKey>
    {
        public TKey Id { get; set; }
        public int? Version { get; set; }
        public int? Creator { get; set; }
    }
}
