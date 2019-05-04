namespace Narik.Common.Web.Infrastructure.Authorization.RoleBased
{
    public  class NarikOverrideAuthorizeAttribute : NarikAuthorizeAttribute
    {
        public NarikOverrideAuthorizeAttribute(string[] rolesArray):base(rolesArray)
        {
        }
        public override bool IsOverride => true;
    }
}
