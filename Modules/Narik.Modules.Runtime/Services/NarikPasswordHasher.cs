
using Microsoft.AspNetCore.Identity;
using Narik.Common.Infrastructure.Helpers;
using Narik.Common.Shared.Models;


namespace Narik.Modules.Runtime.Services
{
    public class NarikPasswordHasher : IPasswordHasher<ApplicationUser>
    {
        public string HashPassword(ApplicationUser user, string password)
        {
            return Md5HashHelper.GetMd5Hash(password);
        }

        public PasswordVerificationResult VerifyHashedPassword(ApplicationUser user,
            string hashedPassword, string providedPassword)
        {
            if (hashedPassword == HashPassword(user,providedPassword))
                return PasswordVerificationResult.Success;
            else
                return PasswordVerificationResult.Failed;
        }
    }
}
