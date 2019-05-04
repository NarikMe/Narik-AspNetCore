namespace Narik.Common.Shared.Interfaces
{
    public interface INarikModuleEntity
    {
        string Version { get; set; }
        string Key { get; set; }
        int InitOrder { get; set; }

        int MenuOrder { get; set; }

        int Id { set; get; }
    }
}
