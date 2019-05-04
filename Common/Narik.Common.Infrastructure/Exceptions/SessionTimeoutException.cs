using System;

namespace Narik.Common.Infrastructure.Exceptions
{
    public class SessionTimeoutException:Exception
    {
        public  const string SessionTimeoutMessage = "SessionTimeout";

        public override string Message => SessionTimeoutMessage;

        public static bool HasSessionTimeoutException(Exception ex)
        {
            var inner = ex;
            do
            {
                if (inner is SessionTimeoutException)
                    return true;
                inner = inner.InnerException;
            } while (inner != null);
            return false;
        }
    }
}
