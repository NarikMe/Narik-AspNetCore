namespace Narik.Common.Web.Infrastructure.Authorization.ResourceBased
{
    public class NarikResourceOverrideAuthorizeAttribute: NarikResourceAuthorizeAttribute
    {
        public NarikResourceOverrideAuthorizeAttribute()
        {
        }

        public NarikResourceOverrideAuthorizeAttribute(string resourceKey) : base(resourceKey)
        {
        }

        public NarikResourceOverrideAuthorizeAttribute(string resourceKey, string resourceActions) : base(resourceKey, resourceActions)
        {
        }

        public NarikResourceOverrideAuthorizeAttribute(string resourceKey, string resourceActions, string resourcePattern) : base(resourceKey, resourceActions, resourcePattern)
        {
        }

        public override bool IsOverride => true;
    }
}
