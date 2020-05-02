using Newtonsoft.Json;

namespace CSESoftware.OData.Links
{
    /// <summary>
    /// HATEOS link
    /// </summary>
    public class Link
    {
        [JsonProperty(PropertyName = "rel")]
        public string Relation { get; set; }

        [JsonProperty(PropertyName = "href")]
        public string URL { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }
    }
}