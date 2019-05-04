namespace Narik.Common.Services.Core
{
    public interface IUserAccessLevelService
    {
        bool UserHasAccesslevel(string formEngName, string accessItemEngName);
       
    }
}
