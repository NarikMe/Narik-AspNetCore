using System.Threading.Tasks;
using CommonServiceLocator;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Narik.Common.Services.Core;
using Narik.Common.Shared.Models;
using Narik.Modules.Runtime.Dto;

namespace Narik.Modules.Runtime.Controllers
{
    public class AccountController: ControllerBase
    {
        private readonly ILoginService _loginService;
        private readonly IEventAggregator _eventAggregator;
        private readonly IAccountService _accountService;

        public AccountController(ILoginService loginService, IEventAggregator eventAggregator, 
            IAccountService accountService)
        {
            _loginService = loginService;
            _eventAggregator = eventAggregator;
            _accountService = accountService;
        }

        [AllowAnonymous]
        public async Task<LoginResultModel> Authenticate([FromBody]LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var loginResult = await _loginService.Login(model);
                if (loginResult.Succeeded)
                    _eventAggregator.Raise("UserLogin", loginResult.LoginedUser);
                return loginResult;
            }
            return null;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<bool> Logout()
        {
            return await Task.FromResult(true);
        }


        [HttpPost]
        public async Task<ServerResponse<string>> ChangePassword([FromBody]ChangePasswordDto password)
        {
            var userAccessor = ServiceLocator.Current.GetInstance<ICurrentUserAccessor>();
            return await _accountService.ChangePassword(
                userAccessor.UserId
                , password.OldPassword, password.Password);
        }


    }
}
