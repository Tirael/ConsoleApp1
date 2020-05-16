using Newtonsoft.Json;

namespace TestApp.Common.Models
{
    [JsonObject("Properties")]
    public class Properties
    {
        [JsonProperty("CacheToDb")]
        public bool CacheToDb { get; set; }
    }
}