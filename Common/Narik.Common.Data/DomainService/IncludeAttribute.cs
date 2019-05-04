using System;

namespace Narik.Common.Data.DomainService
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public sealed class IncludeAttribute : Attribute
    {
        private string _memberName;
        private string _path;

        public IncludeAttribute()
        {
        }

        public IncludeAttribute(string path, string memberName)
        {
            this._path = path;
            this._memberName = memberName;
        }

        private bool IsAttributeValid(out string errorMessage)
        {
            errorMessage = null;
            if ((this._path != null) || (this._memberName != null))
            {
                if (string.IsNullOrEmpty(this._path))
                {
                    errorMessage = "InvalidMemberProjection_EmptyPath";
                }
                if (string.IsNullOrEmpty(this._memberName))
                {
                    errorMessage = "InvalidMemberProjection_EmptyMemberName";
                }
            }
            return (errorMessage == null);
        }

        private void ThrowIfAttributeNotValid()
        {
            string errorMessage = null;
            if (!this.IsAttributeValid(out errorMessage))
            {
                throw new InvalidOperationException(errorMessage);
            }
        }

        public bool IsProjection
        {
            get
            {
                bool flag = (this._path != null) || (this._memberName != null);
                if (flag)
                {
                    this.ThrowIfAttributeNotValid();
                }
                return flag;
            }
        }

        public string MemberName
        {
            get
            {
                return this._memberName;
            }
        }

        public string Path
        {
            get
            {
                return this._path;
            }
        }

        public override object TypeId
        {
            get
            {
                return this;
            }
        }
    }
}
