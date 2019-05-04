using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Narik.Common.Services.Core;
using Narik.Common.Shared.Models;
namespace Narik.Modules.Runtime.Services
{
    public class NarikJwtLoginService : ILoginService
    {
        private readonly IUserPasswordStore<ApplicationUser> _userPasswordStore;
        private readonly IPasswordHasher<ApplicationUser> _passwordHasher;
        private readonly NarikModulesConfig _config;



        public NarikJwtLoginService(
            IUserPasswordStore<ApplicationUser> userPasswordStore,
            IPasswordHasher<ApplicationUser> passwordHasher, NarikModulesConfig config)
        {
            _userPasswordStore = userPasswordStore;
            _passwordHasher = passwordHasher;
            _config = config;
        }

        public async Task<LoginResultModel> Login(LoginModel model)
        {
            var errors = new Dictionary<string, string>();
            var user = await _userPasswordStore.FindByNameAsync(model.UserName, new CancellationToken());
            if (user != null)
            {
                var password = await _userPasswordStore.GetPasswordHashAsync(user, new CancellationToken());
                if (_passwordHasher.VerifyHashedPassword(user, password, model.UserName.ToLower() + model.Password) == PasswordVerificationResult.Success)
                {
                    var clientId = Guid.NewGuid().ToString();
                    var loginResult = new LoginResultModel
                    {
                        Succeeded = true,
                        Errors = null,
                        LoginedUser = new LoginUserInfo()
                        {
                            UserName = user.UserName,
                            UserId = user.UserId.ToString(),
                            Roles = user.Roles,
                            Title = user.Title
                        }
                    };
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var key = Encoding.ASCII.GetBytes(_config.Secret);

                    var claims = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name, loginResult.LoginedUser.UserName),
                        new Claim("UserId", loginResult.LoginedUser.UserId),
                        new Claim("ClientId",clientId )
                    });
                    if (loginResult.LoginedUser.Roles!=null)
                        claims.AddClaims(loginResult.LoginedUser.Roles.Select(role => new Claim(ClaimTypes.Role, role.ToString())));
                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = claims,
                        Expires = DateTime.UtcNow.AddDays(7),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                    };
                    var token = tokenHandler.CreateToken(tokenDescriptor);
                    loginResult.Token = tokenHandler.WriteToken(token);

                    return loginResult;
                }
                else
                {
                    errors.Add("INVALID_LOGIN", "INVALID_LOGIN");
                }
            }
            else
            {
                errors.Add("INVALID_LOGIN", "INVALID_LOGIN");
            }

            return new LoginResultModel
            {
                Succeeded = false,
                Errors = errors,
                LoginedUser = null
            };


        }
    }
}
