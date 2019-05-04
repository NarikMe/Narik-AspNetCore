namespace Narik.Common.Services.Core
{
    public interface IResourceAccessService
    {
        bool HasAccess(string userid,string resource);
    }
}
