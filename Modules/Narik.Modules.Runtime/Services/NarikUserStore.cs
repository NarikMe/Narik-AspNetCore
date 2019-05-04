using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Narik.Common.Services.Core;
using Narik.Common.Shared.Models;


namespace Narik.Modules.Runtime.Services
{
    public class NarikUserStore : IUserPasswordStore<ApplicationUser>
    {
        private readonly IAccountService _accountService;
        
        public NarikUserStore(IAccountService accountService, IPasswordHasher<ApplicationUser> passwordHasher)
        {
            _accountService = accountService;
            
        }
        public void Dispose()
        {
            
        }

        public Task<string> GetUserIdAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName);
        }

        public Task<string> GetUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName); 
        }

        public Task SetUserNameAsync(ApplicationUser user, string userName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetNormalizedUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetNormalizedUserNameAsync(ApplicationUser user, string normalizedName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> DeleteAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ApplicationUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<ApplicationUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            var user =await _accountService.GetApplicationUserByUserName(normalizedUserName);
            return user;
        }

        public Task SetPasswordHashAsync(ApplicationUser user, string passwordHash, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<string> GetPasswordHashAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            var password =await _accountService.GetPasswordByUserName(user.UserName);
            return password; 
        }

        public Task<bool> HasPasswordAsync(ApplicationUser user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
