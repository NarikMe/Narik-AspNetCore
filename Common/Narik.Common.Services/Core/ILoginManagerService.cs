using System.Threading.Tasks;
using Narik.Common.Shared.Models;

namespace Narik.Common.Services.Core
{
    public interface ILoginManagerService
    {
        Task<string> SignInAsync(ApplicationUser user);
        void SignOut(params string[] authenticationTypes);
    }
}
