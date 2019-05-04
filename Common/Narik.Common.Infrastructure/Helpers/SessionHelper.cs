


using CommonServiceLocator;
using Narik.Common.Services.Core;

namespace Narik.Common.Infrastructure.Helpers
{
    public static class SessionHelper
    {
        public static IUserEnvironment UserEnvironmentService => ServiceLocator.Current.GetInstance<IUserEnvironment>();
     
    }
}
