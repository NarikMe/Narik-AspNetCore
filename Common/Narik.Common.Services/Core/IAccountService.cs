using System.Collections.Generic;
using System.Threading.Tasks;
using Narik.Common.Shared.Models;


namespace Narik.Common.Services.Core
{
    public interface IAccountService
    {
        Task<ApplicationUser>  GetApplicationUserByUserName(string userName);
        Task<ServerResponse<string>> ChangePassword(int userId, string oldPassword, string newPassword);
        object CreateReturnUserResult(ApplicationUser user);
        Dictionary<string,string> CustomValidateLogin(ApplicationUser user);
        Task<string> GetPasswordByUserName(string userName);
    }
}
