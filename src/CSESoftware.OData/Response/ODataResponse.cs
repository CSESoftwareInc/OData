using System.Collections.Generic;
using Newtonsoft.Json;

namespace CSESoftware.OData.Response
{
    /// <summary>
    /// Response object that can be serialized and returned from the API
    /// </summary>
    /// <typeparam name="T">The type of data you are returning in the Data list</typeparam>
    public class ODataResponse<T> where T : class
    {
        [JsonProperty("data")]
        public IEnumerable<T> Data { get; set; }

        [JsonProperty("count")]

        public Count Count { get; set; }

        [JsonProperty("links")]
        public IEnumerable<Link> Links { get; set; }
    }
}
