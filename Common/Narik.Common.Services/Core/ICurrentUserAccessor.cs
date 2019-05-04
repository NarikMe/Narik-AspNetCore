namespace Narik.Common.Services.Core
{
    public interface ICurrentUserAccessor
    {
        int UserId { get;  }
        string UserName { get;  }
    }
}
