using System;
using System.Text.Json.Serialization;

namespace Narik.Common.Shared.Models
{
    public class ServerResponse
    {
        [JsonPropertyName("isSucceed")]
        public bool IsSucceed { get; set; }
        [JsonPropertyName("errors")]
        public string[] Errors { get; set; }

        [JsonPropertyName("tag")]
        public Object Tag { get; set; }
    }
    public class ServerResponse<T> : ServerResponse
    {
        [JsonPropertyName("data")]
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
