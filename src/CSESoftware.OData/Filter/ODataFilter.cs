using Microsoft.AspNetCore.Mvc;

namespace CSESoftware.OData.Filter
{
    public class ODataFilter : IODataFilter
    {
        [FromQuery(Name = "$top")]
        public int? Take { get; set; }

        [FromQuery(Name = "$skip")]
        public int? Skip { get; set; }

        [FromQuery(Name = "$count")]
        public bool? Count { get; set; }

        [FromQuery(Name = "$filter")]
        public string Filter { get; set; }

        [FromQuery(Name = "$orderBy")]
        public string OrderBy { get; set; }

        [FromQuery(Name = "$thenBy")]
        public string ThenBy { get; set; }

        [FromQuery(Name = "$expand")]
        public string Expand { get; set; }

        [FromQuery(Name = "$links")]
        public bool? Links { get; set; }
    }
}