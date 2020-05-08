using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;

namespace ConsoleApp1.Models
{
    [DebuggerDisplay("ResultCount:{ResultCount}")]
    public class ApiResponse<T> where T : class
    {
        [JsonProperty("resultCount", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public long? ResultCount { get; set; }
        [JsonProperty("results", Required = Required.DisallowNull, NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<T> Results { get; set; }
    }
}