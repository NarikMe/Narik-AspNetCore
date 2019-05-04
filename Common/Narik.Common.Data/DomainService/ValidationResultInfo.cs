using System;
using System.Collections.Generic;
using System.Linq;

namespace Narik.Common.Data.DomainService
{
    public sealed class ValidationResultInfo : IEquatable<ValidationResultInfo>
    {
        private int _errorCode;
        private string _message;
        private IEnumerable<string> _sourceMemberNames;
        private string _stackTrace;

        public ValidationResultInfo()
        {
        }

        public ValidationResultInfo(string message, IEnumerable<string> sourceMemberNames)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            if (sourceMemberNames == null)
            {
                throw new ArgumentNullException("sourceMemberNames");
            }
            this._message = message;
            this._sourceMemberNames = sourceMemberNames;
        }

        public ValidationResultInfo(string message, int errorCode, string stackTrace, IEnumerable<string> sourceMemberNames)
        {
            if (message == null)
            {
                throw new ArgumentNullException("message");
            }
            if (sourceMemberNames == null)
            {
                throw new ArgumentNullException("sourceMemberNames");
            }
            this._message = message;
            this._errorCode = errorCode;
            this._stackTrace = stackTrace;
            this._sourceMemberNames = sourceMemberNames;
        }

        public override int GetHashCode()
        {
            return this.Message.GetHashCode();
        }

        bool IEquatable<ValidationResultInfo>.Equals(ValidationResultInfo other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            return ((((this.Message == other.Message) && (this.ErrorCode == other.ErrorCode)) && (this.StackTrace == other.StackTrace)) && this.SourceMemberNames.SequenceEqual<string>(other.SourceMemberNames));
        }

        
        public int ErrorCode
        {
            get
            {
                return this._errorCode;
            }
            set
            {
                this._errorCode = value;
            }
        }

        
        public string Message
        {
            get
            {
                return this._message;
            }
            set
            {
                this._message = value;
            }
        }

        
        public IEnumerable<string> SourceMemberNames
        {
            get
            {
                return this._sourceMemberNames;
            }
            set
            {
                this._sourceMemberNames = value;
            }
        }

        
        public string StackTrace
        {
            get
            {
                return this._stackTrace;
            }
            set
            {
                this._stackTrace = value;
            }
        }
    }
}
