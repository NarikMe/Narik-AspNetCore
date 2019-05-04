using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Narik.Common.Data.DomainService
{
    public class DomainServiceDescription
    {

        private readonly string[] methodPrefix = {"Insert","Delete","Update"};

        public Dictionary<Type,MethodInfo> _insertMethods=new Dictionary<Type, MethodInfo>();
        public Dictionary<Type, MethodInfo> _deleteMethods = new Dictionary<Type, MethodInfo>();
        public Dictionary<Type, MethodInfo> _updateMethods = new Dictionary<Type, MethodInfo>();
        public DomainServiceDescription(Type type)
        {
            var methods=  type.GetMethods().Where(x => 
            x.Name.StartsWith(methodPrefix[0])
            || x.Name.StartsWith(methodPrefix[1])
            || x.Name.StartsWith(methodPrefix[2])
            );
            foreach (var methodInfo in methods )
            {
                if (methodInfo.GetParameters().Length==1
                    && methodInfo.Name.StartsWith(methodPrefix[0]+ methodInfo.GetParameters()[0].ParameterType.Name))
                    _insertMethods.Add(methodInfo.GetParameters()[0].ParameterType,methodInfo);
                if (methodInfo.GetParameters().Length == 1
                    && methodInfo.Name.StartsWith(methodPrefix[1] + methodInfo.GetParameters()[0].ParameterType.Name))
                    _deleteMethods.Add(methodInfo.GetParameters()[0].ParameterType, methodInfo);
                if (methodInfo.GetParameters().Length == 1
                    && methodInfo.Name.StartsWith(methodPrefix[2] + methodInfo.GetParameters()[0].ParameterType.Name))
                    _updateMethods.Add(methodInfo.GetParameters()[0].ParameterType, methodInfo);
            }
        }
    }
}
