using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNetCore.Mvc;
using Narik.Common.Data;
using Narik.Common.Data.DomainService;
using Narik.Common.Shared.Attributes;
using Narik.Common.Shared.Interfaces;
using Narik.Common.Shared.Models;

namespace Narik.Common.Web.Infrastructure.OData
{

    public class ODataHelper
    {



        public static void RegisterControllers(ODataModelBuilder builder, Assembly assembly)
        {

            var method = builder.GetType().GetMethod("EntitySet");
            var method2 = builder.GetType().GetMethod("EntityType");


            var funcReturnsCollectionMethod = typeof(FunctionConfiguration).GetMethod("ReturnsCollection");
            var returnsMethod = typeof(FunctionConfiguration)
                .GetMethods().FirstOrDefault(x => x.Name == "Returns" && x.IsGenericMethod);
            var returnsFromEntitySet = typeof(FunctionConfiguration).GetMethod("ReturnsFromEntitySet");
            var funcParameterMethod = typeof(FunctionConfiguration)
                .GetMethods().FirstOrDefault(x => x.Name == "Parameter" && x.IsGenericMethod);

            var returnsCollectionFromEntitySetMethod = typeof(ActionConfiguration).GetMethods()
                .FirstOrDefault(x => x.Name == "ReturnsCollectionFromEntitySet"
            && x.IsGenericMethod);
            var collectionParameterMethod = typeof(ActionConfiguration).GetMethods()
                .FirstOrDefault(x => x.Name == "CollectionParameter"
                                     && x.IsGenericMethod);

            var parameterMethod = typeof(ActionConfiguration).GetMethods().FirstOrDefault(x => x.Name == "Parameter" && x.IsGenericMethod); ;


            var controllers = assembly.GetTypes().Where(x => typeof(INarikODataController).IsAssignableFrom(x)).ToList();
            foreach (var controller in controllers)
            {
                if (!controller.IsAbstract && controller.BaseType != null)
                {
                    var originType = GetOriginType(controller.BaseType);
                    var name = controller.Name.Replace("Controller", "");

                    //Default.PostMasterDetailEntity

                    Type viewModelType;
                    if (controller.BaseType.FullName != null && controller.BaseType.FullName.Contains("NarikBaseDataODataController"))
                    {
                        viewModelType = typeof(BaseDataViewModel);
                    }
                    else
                    {
                        viewModelType = originType.GenericTypeArguments[1];
                    }


                    var m = method.MakeGenericMethod(viewModelType);
                    var m2 = method2.MakeGenericMethod(viewModelType);
                    m.Invoke(builder, new object[] { name });

                    if (controller.BaseType.FullName != null && controller.BaseType.FullName.Contains("NarikBaseDataODataController"))
                    {
                        builder.AddEntityType(typeof(BaseDataListViewModel));
                    }
                    else
                    {
                        if (originType.GenericTypeArguments[1] != originType.GenericTypeArguments[2])
                            builder.AddEntityType(originType.GenericTypeArguments[2]);
                    }



                    var ret = m2.Invoke(builder, null);
                    var collection = ret.GetType().GetProperty("Collection").GetValue(ret, null);
                    var returnsCollectionFromEntitySetMethodGeneric = returnsCollectionFromEntitySetMethod.MakeGenericMethod(viewModelType);


                    //Delete
                    var tkeyType = originType.GenericTypeArguments[5];
                    var idVersionCreatorGenericType = typeof(IdVersionCreator<>).MakeGenericType(tkeyType);
                    var act = collection.GetType().GetMethod("Action").Invoke(collection, new object[] { "Delete" }) as ActionConfiguration;
                    var actConfig = act.Returns<IActionResult>();
                    var collectionParameterMethodGeneric = collectionParameterMethod.MakeGenericMethod(idVersionCreatorGenericType);
                    collectionParameterMethodGeneric.Invoke(actConfig, new object[] { "items" });


                    //Complete
                    var completeMethod = controller.GetMethod("Complete");
                    if (completeMethod != null)
                    {
                        var actComplete = collection.GetType().GetMethod("Action").Invoke(collection, new object[] { "Complete" }) as ActionConfiguration;
                        actComplete.ReturnsCollection<NarikDto>().Parameter<string>("filter");
                    }
                   

                    //GetForSelector
                    var getForSelectorMethod = controller.GetMethod("GetForSelector");
                    if (getForSelectorMethod != null)
                    {
                        var func = collection.GetType().GetMethod("Function").Invoke(collection, new object[] { "GetForSelector" }) as FunctionConfiguration;
                        var funcReturnsCollectionMethodGeneric = funcReturnsCollectionMethod.MakeGenericMethod(getForSelectorMethod.ReturnParameter.ParameterType.GetGenericArguments()[0]);
                        funcReturnsCollectionMethodGeneric.Invoke(func, null);
                    }

                    var methods = controller.GetMethods();
                    foreach (var methodInfo in methods)
                    {
                        var attr = methodInfo.GetCustomAttribute<OdataFunctionAttribute>();
                        if (attr != null)
                        {
                            var func = collection.GetType()
                                          .GetMethod("Function")
                                          .Invoke(collection, new object[] { methodInfo.Name })
                                          as FunctionConfiguration;

                            Type returnType;
                            bool isListMethod = false;
                            if (methodInfo.ReturnType.IsGenericType)
                            {
                                returnType = methodInfo.ReturnType.GenericTypeArguments[0];
                                isListMethod = true;
                            }
                            else
                            {
                                returnType = methodInfo.ReturnType;
                                isListMethod = false;
                            }

                            var entityType = originType.GenericTypeArguments[1];

                            if (returnType != entityType)
                            {
                                if (isListMethod)
                                {
                                    var funcReturnsCollectionMethodGeneric =
                                        funcReturnsCollectionMethod.MakeGenericMethod(returnType);
                                    funcReturnsCollectionMethodGeneric.Invoke(func, null);
                                }
                                else
                                {
                                    var method0 =
                                        returnsMethod.MakeGenericMethod(returnType);
                                    method0.Invoke(func, null);
                                }
                            }
                            else
                            {
                                if (isListMethod)
                                {
                                    var returnsCollectionFromEntitySetMethod2 = typeof(FunctionConfiguration)
                                        .GetMethods()
                                        .FirstOrDefault(x => x.Name == "ReturnsCollectionFromEntitySet"
                                                             && x.IsGenericMethod);
                                    var returnsCollectionFromEntitySetMethodGeneric2 =
                                        returnsCollectionFromEntitySetMethod2.MakeGenericMethod(
                                            originType.GenericTypeArguments[1]);
                                    returnsCollectionFromEntitySetMethodGeneric2.Invoke(func,
                                        new object[] { name + "_" + methodInfo.Name });
                                }
                                else
                                {
                                    var method0 =
                                        returnsFromEntitySet.MakeGenericMethod(returnType);
                                    method0.Invoke(func, new object[] { name });
                                }
                            }

                            foreach (var parameter in methodInfo.GetParameters())
                            {
                                var funcParameterMethodMethodGeneric =
                                    funcParameterMethod.MakeGenericMethod(parameter.ParameterType);
                                funcParameterMethodMethodGeneric.Invoke(func, new object[] { parameter.Name });
                            }
                        }

                        var actionAttributes = methodInfo.GetCustomAttributes<OdataActionParameterInfoAttribute>().ToList();
                        if (actionAttributes != null && actionAttributes.Count != 0)
                        {
                            var actPostMasterDetail = collection.GetType().GetMethod("Action")
                                .Invoke(collection, new object[] { method.Name }) as ActionConfiguration;

                            actPostMasterDetail = returnsCollectionFromEntitySetMethodGeneric.Invoke(actPostMasterDetail,
                                new object[] { name }) as ActionConfiguration;


                            foreach (var parameter in actionAttributes)
                            {
                                var parameterMethodGeneric = parameterMethod.MakeGenericMethod(parameter.Type);
                                parameterMethodGeneric.Invoke(actPostMasterDetail, new object[] { parameter.Name });
                            }
                        }
                    }
                }
            }
        }

        private static Type GetOriginType(Type type)
        {
            while (type.BaseType != null && type.BaseType != typeof(ODataController))
                type = type.BaseType;
            return type;
        }
    }
}
