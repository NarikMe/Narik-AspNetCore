using System;
using Microsoft.AspNetCore.Authorization;

namespace Narik.Common.Web.Infrastructure.Authorization.ResourceBased
{
    public class NarikResourceAuthorizeAttribute : Attribute, IAuthorizeData
    {
        private string _resourceKey;
        private string _resourceActions;
        private string _resourcePattern;
        public const string PolicyPrefix = "$Narik_Resource_Authorize";
        public const string PolicyOverridePrefix = "$Narik_Resource_Authorize_Override";

        public NarikResourceAuthorizeAttribute()
        {

        }
        public NarikResourceAuthorizeAttribute(string resourceKey) : this(resourceKey,null,null)
        {
        }
        public NarikResourceAuthorizeAttribute(string resourceKey,
            string resourceActions) : this(resourceKey, resourceActions, null)
        {
        }
        public NarikResourceAuthorizeAttribute(string resourceKey,string resourceActions,
            string resourcePattern)
        {
            ResourceKey = resourceKey;
            ResourceActions = resourceActions;
            ResourcePattern = resourcePattern;
        }

        public string ResourceKey
        {
            get => _resourceKey;
            set
            {
                _resourceKey = value;
                CreatePolicy();
            }
        }

        public string ResourceActions
        {
            get => _resourceActions;
            set
            {
                _resourceActions = value;
                CreatePolicy();
            }
        }

        public string ResourcePattern
        {
            get => _resourcePattern;
            set
            {
                _resourcePattern = value;
                CreatePolicy();
            }
        }

        public virtual bool IsOverride => false;
        public string Policy { get; set; }
        public string Roles { get; set; }


        private void CreatePolicy()
        {
            var policyPrefix = IsOverride ? PolicyOverridePrefix : PolicyPrefix;
            if (string.IsNullOrEmpty(ResourcePattern)
            && string.IsNullOrEmpty(ResourceKey))
                Policy = null;
            else if (!string.IsNullOrEmpty(ResourcePattern))
                Policy = policyPrefix + ResourcePattern;
            else if(!string.IsNullOrEmpty(ResourceActions))
                Policy = policyPrefix + $"({ResourceKey}#{ResourcePattern})";
            else
                Policy = policyPrefix + $"({ResourceKey})";
        }

        public string AuthenticationSchemes { get; set; }
    }
}
