using CommonServiceLocator;
using Microsoft.AspNetCore.Mvc.Filters;
using Narik.Common.Services.Core;

namespace Narik.Common.Web.Infrastructure.Filters
{
    public class  NarikExceptionFilter: ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            base.OnException(context);
            ServiceLocator.Current.GetInstance<ILoggingService>().Log(context.Exception);
        }
    }
}
