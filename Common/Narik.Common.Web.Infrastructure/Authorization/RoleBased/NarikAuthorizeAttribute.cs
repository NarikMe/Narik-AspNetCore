using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;

namespace Narik.Common.Web.Infrastructure.Authorization.RoleBased
{
    public class NarikAuthorizeAttribute : Attribute, IAuthorizeData
    {
        public const string PolicyPrefix = "$Narik_Authorize";
        public const string PolicyOverridePrefix = "$Narik_Authorize_Override";

        public NarikAuthorizeAttribute()
        {

        }
        public NarikAuthorizeAttribute(string[] rolesArray)
        {
            RolesArray = rolesArray;
            Roles = RolesArray == null ? null : String.Join(",", RolesArray);
        }


        public virtual bool IsOverride => false;
        public string[] RolesArray { get; set; }
        public string Policy { get; set; }

        public string Roles
        {
            get => null;
            set => CreatePolicy();
        }

        private void CreatePolicy()
        {
            if (RolesArray == null || !RolesArray.Any())
                Policy = null;
            else
            {
                Policy = (IsOverride ? PolicyOverridePrefix : PolicyPrefix) + String.Join(",", RolesArray);
            }
        }

        public string AuthenticationSchemes { get; set; }
    }
}
