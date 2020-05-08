using Newtonsoft.Json;

namespace ConsoleApp1.Models
{
    [JsonObject("Properties")]
    public class Properties
    {
        [JsonProperty("CacheToDb")]
        public bool CacheToDb { get; set; }
    }
}