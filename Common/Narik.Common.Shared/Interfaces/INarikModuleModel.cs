namespace Narik.Common.Shared.Interfaces
{
    public interface INarikModuleModel
    {
        string Key { get; set; }
        string AssemeblyName { get; set; }
        int InitOrder { get; set; }

    }
}
