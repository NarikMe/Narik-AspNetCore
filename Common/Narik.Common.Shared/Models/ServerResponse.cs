using System;
using Newtonsoft.Json;

namespace Narik.Common.Shared.Models
{
    public class ServerResponse
    {
        [JsonProperty("isSucceed")]
        public bool IsSucceed { get; set; }
        [JsonProperty("errors")]
        public string[] Errors { get; set; }

        [JsonProperty("tag")]
        public Object Tag { get; set; }
    }
    public class ServerResponse<T> : ServerResponse
    {
        [JsonProperty("data")]
        public T Data { get; set; }
       
        public ServerResponse() : this(true)
        {

        }

        public ServerResponse(bool isSucceed):this(isSucceed,(string[]) null)
        {
            
        }
        public ServerResponse(bool isSucceed, T data)
        {
            IsSucceed = isSucceed;
            Data  = data;
        }
        public ServerResponse(bool isSucceed,string[] errors)
        {
            IsSucceed = isSucceed;
            Errors = errors;
        }
        public ServerResponse(bool isSucceed, string error)
        {
            IsSucceed = isSucceed;
            Errors = new []{error};
        }

        public ServerResponse( Exception ex) : this(false,ex.Message)
        {

        }
    }
}
