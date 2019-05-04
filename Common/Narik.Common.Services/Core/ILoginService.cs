using System.Threading.Tasks;
using Narik.Common.Shared.Models;

namespace Narik.Common.Services.Core
{
    public interface ILoginService
    {
        Task<LoginResultModel> Login(LoginModel model);
    }
}
