using Narik.Common.Services.Core;

namespace Narik.Modules.Runtime.Services
{
    public class ResourceAccessService: IResourceAccessService
    {
        public bool HasAccess(string userid, string resource)
        {
            return true;
        }
    }
}
