using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using CommonServiceLocator;
using Narik.Common.Data.DomainService;
using Narik.Common.Services.Core;


namespace Narik.Common.Infrastructure.Startup
{
    public static class AutoMapperConfig
    {
        public static void Configure(IModuleService moduleService)
        {
            Mapper.Initialize(cfg =>
            {
                foreach (var moduleAssembliesValue in moduleService.ModuleAssemblies.Values)
                    cfg.AddProfiles(moduleAssembliesValue);

                cfg.CreateMissingTypeMaps = true;
                cfg.ValidateInlineMaps = false;

                cfg.CreateMap<List<ChangeSetEntry>, List<ChangeSetEntry>>()
                    .ConvertUsing<ChangeSetEntryListConverter>();

                cfg.ForAllMaps((t, ex) =>
                {
                    ex.PreserveReferences();
                    
                  //  ex.ReverseMap();
                  //  ex.ForAllOtherMembers(opt => opt.Ignore());
                });
            });
        }
    }

    public class ChangeSetEntryListConverter : ITypeConverter<List<ChangeSetEntry>, List<ChangeSetEntry>>
    {
        private static readonly ConcurrentDictionary<Type, Type> Types = new ConcurrentDictionary<Type, Type>();
        public List<ChangeSetEntry> Convert(List<ChangeSetEntry> source, List<ChangeSetEntry> destination, ResolutionContext context)
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
                    var destinations = Mapper.Configuration.GetAllTypeMaps()
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
