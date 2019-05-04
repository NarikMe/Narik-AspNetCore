namespace Narik.Common.Services.Core
{
    public interface IAppSettingProvider
    {
        string this[string configKey] { get; }
    }
}
