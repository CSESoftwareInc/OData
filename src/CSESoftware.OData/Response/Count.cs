using Newtonsoft.Json;

namespace CSESoftware.OData.Response
{
    public class Count
    {
        [JsonProperty("response")]
        public int Response { get; set; }

        [JsonProperty("total")]
        public int Total { get; set; }
    }
}
