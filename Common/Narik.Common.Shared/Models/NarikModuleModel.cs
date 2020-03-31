using Narik.Common.Shared.Interfaces;
using System.Text.Json.Serialization;

namespace Narik.Common.Shared.Models
{
    public class NarikModuleModel:INarikModuleModel
    {
        [JsonPropertyName("key")]
        public string Key { get; set; }

        [JsonPropertyName("assemblyName")]
        public string AssemblyName { get; set; }

        [JsonPropertyName("dependencies")]
        public string Dependencies { get; set; }

        [JsonPropertyName("initOrder")]
        public int InitOrder { get; set; }
    }

    public class NarikModulesConfig
    {
        [JsonPropertyName("modules")]
        public NarikModuleModel[] Modules { get; set; }

        [JsonPropertyName("secret")]
        public string Secret { get; set; }

        [JsonPropertyName("useCamelCase")]
        public bool? UseCamelCase { get; set; }

        [JsonPropertyName("addDefaultAuthenticationPolicy")]
        public bool? AddDefaultAuthenticationPolicy { get; set; }

        [JsonPropertyName("authenticationMode")]
        public string AuthenticationMode { get; set; }

        [JsonPropertyName("odataMaxTop")]
        public int? OdataMaxTop { get; set; }

        [JsonPropertyName("connectionStringKey")]
        public string ConnectionStringKey { get; set; }

        [JsonPropertyName("apiPrefixes")]
        public string ApiPrefixes { get; set; }

        [JsonPropertyName("appDirectories")]
        public string AppDirectories { get; set; }
    }

   
}
