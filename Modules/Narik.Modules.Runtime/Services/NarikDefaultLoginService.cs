using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Narik.Common.Services.Core;
using Narik.Common.Shared.Models;

namespace Narik.Modules.Runtime.Services
{
    public class NarikDefaultLoginService : ILoginService
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAccountService _accountService;
        public NarikDefaultLoginService(
            SignInManager<ApplicationUser> signInManager, 
            UserManager<ApplicationUser> userManager,
            IAccountService accountService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _accountService = accountService;
        }
        public async Task<LoginResultModel> Login(LoginModel model)
        {
            ApplicationUser user = new ApplicationUser();
            var errors = new Dictionary<string, string>();
            var loginResult = 
                await _signInManager.PasswordSignInAsync(model.UserName, model.UserName.ToLower() + model.Password, false, false);
            if (loginResult.Succeeded)
            {
                user = await _userManager.FindByNameAsync(model.UserName);
                errors = _accountService.CustomValidateLogin(user);
            }
            else
                errors.Add("INVALID_LOGIN", "INVALID_LOGIN");
            if (errors.Any())
                return new LoginResultModel
                {
                    Succeeded = false,
                    Errors = errors,
                    LoginedUser = null
                };
            return new LoginResultModel
            {
                Succeeded = true,
                Errors = null,
                LoginedUser = new LoginUserInfo
                {
                    UserName = user.UserName,
                    UserId = user.Id,
                    Roles = user.Roles
                }
            };
        }
    }
}
