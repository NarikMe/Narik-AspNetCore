namespace Narik.Common.Shared.Interfaces
{
    public interface INarikModuleModel
    {
        string Key { get; set; }
        string AssemblyName { get; set; }
        int InitOrder { get; set; }

    }
}
