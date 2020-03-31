using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoMapper;
using CommonServiceLocator;
using Microsoft.Extensions.DependencyInjection;
using Narik.Common.Data.DomainService;
using Narik.Common.Services.Core;
using Unity;

namespace Narik.Common.Infrastructure.Startup
{
    public static class AutoMapperConfig
    {
        private static bool ImplementsGenericInterface(this Type type, Type interfaceType)
           => type.IsGenericType(interfaceType) || type.GetTypeInfo().ImplementedInterfaces.Any(@interface => @interface.IsGenericType(interfaceType));

        private static bool IsGenericType(this Type type, Type genericType)
            => type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == genericType;

        public static void Configure(IUnityContainer container, IServiceCollection services, IModuleService moduleService)
        {
            var assembliesToScan = moduleService.ModuleAssemblies.Values;

            var allTypes = assembliesToScan
                .Where(a => !a.IsDynamic && a.GetName().Name != nameof(AutoMapper))
                .Distinct() // avoid AutoMapper.DuplicateTypeMapConfigurationException
                .SelectMany(a => a.DefinedTypes)
                .ToArray();

            var openTypes = new[]
           {
                typeof(IValueResolver<,,>),
                typeof(IMemberValueResolver<,,,>),
                typeof(ITypeConverter<,>),
                typeof(IValueConverter<,>),
                typeof(IMappingAction<,>)
            };
            foreach (var type in openTypes.SelectMany(openType => allTypes
                .Where(t => t.IsClass
                    && !t.IsAbstract
                    && t.AsType().ImplementsGenericInterface(openType))))
            {
                container.RegisterType(type.AsType());
            }

            var mapperConfiguration=new MapperConfiguration(cfg => {
                cfg.AddMaps(moduleService.ModuleAssemblies.Values);
                cfg.ForAllMaps((t, ex) => { ex.PreserveReferences(); });
                cfg.CreateMap<List<ChangeSetEntry>, List<ChangeSetEntry>>()
                    .ConvertUsing<ChangeSetEntryListConverter>();

                

                cfg.ForAllMaps((t, ex) =>
                {
                    ex.PreserveReferences();
                });
            });
            container.RegisterInstance<IConfigurationProvider>(mapperConfiguration);

            var mapper= new Mapper(mapperConfiguration, ( Type t)=> container.Resolve(t));
            container.RegisterInstance<IMapper>(mapper);
            // services.AddAutoMapper(cfg =>
            // {
            //     // TODO: CORE_3
            //     // cfg.CreateMissingTypeMaps = true;
            //     cfg.AddMaps(moduleService.ModuleAssemblies.Values);
            //     cfg.ForAllMaps((t, ex) => { ex.PreserveReferences(); });
            //     cfg.CreateMap<List<ChangeSetEntry>, List<ChangeSetEntry>>()
            //         .ConvertUsing<ChangeSetEntryListConverter>();

            //     cfg.ForAllMaps((t, ex) =>
            //     {
            //         ex.PreserveReferences();
            //     });
            // },assemblies:new List<Assembly>(),ServiceLifetime.Singleton
            //);

        }
    }

    public class ChangeSetEntryListConverter : ITypeConverter<List<ChangeSetEntry>, List<ChangeSetEntry>>
    {
        private static readonly ConcurrentDictionary<Type, Type> Types = new ConcurrentDictionary<Type, Type>();
        public List<ChangeSetEntry> Convert(List<ChangeSetEntry> source,
            List<ChangeSetEntry> destination, 
            ResolutionContext context)
        {
            var result = new List<ChangeSetEntry>();

            var newContext = new ResolutionContext(context.Options, context.Mapper);
            foreach (var changeSetEntry in source)
            {
                Type destType;
                var entity = changeSetEntry.Entity;

                if (Types.ContainsKey(entity.GetType()))
                    destType = Types[entity.GetType()];
                else
                {
                    var destinations = context.Mapper.ConfigurationProvider.GetAllTypeMaps()
                        .Where(t => t.SourceType == entity.GetType()).ToList();

                    if (destinations.Count == 0)
                    {
                        var environment = ServiceLocator.Current.GetInstance<IEnvironment>();
                        var destType0 = (
                            from assembly in environment.ModelAssemblies
                            from type in assembly.GetTypes()
                            where type.Name == entity.GetType().Name.Replace("ViewModel", "")
                            select type).FirstOrDefault();
                        if (destType0 != null)
                            destType = destType0;
                        else
                            throw new Exception("Mapping not found for " + entity.GetType());
                    }
                    else
                        destType = destinations.FirstOrDefault()?.DestinationType;
                    Types.TryAdd(entity.GetType(), destType);
                }
                try
                {
                    var resultEntity = context.Mapper.Map(entity, null, entity.GetType(), destType, newContext);
                    var resultItem = new ChangeSetEntry
                    {
                        Entity = resultEntity,
                        Operation = changeSetEntry.Operation,
                        ReturnEntity = changeSetEntry.ReturnEntity,
                        EntityUpdateFiledsInfo = changeSetEntry.EntityUpdateFiledsInfo
                    };
                    result.Add(resultItem);
                }
                catch
                {
                    throw;
                }

            }
            return result;
        }
    }
}
