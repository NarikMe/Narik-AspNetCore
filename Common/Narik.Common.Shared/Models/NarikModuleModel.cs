using Narik.Common.Shared.Interfaces;
using Newtonsoft.Json;

namespace Narik.Common.Shared.Models
{
    public class NarikModuleModel:INarikModuleModel
    {
        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("assemeblyName")]
        public string AssemeblyName { get; set; }

        [JsonProperty("initOrder")]
        public int InitOrder { get; set; }
    }

    public class NarikModulesConfig
    {
        [JsonProperty("modules")]
        public NarikModuleModel[] modules { get; set; }

        [JsonProperty("secret")]
        public string Secret { get; set; }

        [JsonProperty("useCamelCase")]
        public bool? UseCamelCase { get; set; }

        [JsonProperty("addDefaultAuthorizeFilter")]
        public bool? AddDefaultAuthorizeFilter { get; set; }

        [JsonProperty("authenticationMode")]
        public string AuthenticationMode { get; set; }

        [JsonProperty("odataMaxTop")]
        public int? OdataMaxTop { get; set; }

        [JsonProperty("connectionStringKey")]
        public string ConnectionStringKey { get; set; }

        [JsonProperty("apiPrefixes")]
        public string ApiPrefixes { get; set; }

        [JsonProperty("appDirectories")]
        public string AppDirectories { get; set; }
    }

   
}
