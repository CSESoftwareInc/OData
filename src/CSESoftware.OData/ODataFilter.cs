namespace CSESoftware.OData
{
    public class ODataFilter : IODataFilter
    {
        public int? Take { get; set; }
        public int? Skip { get; set; }
        public bool? Count { get; set; }
        public string Filter { get; set; }
        public string OrderBy { get; set; }
        public string ThenBy { get; set; }
        public string Expand { get; set; }
        public bool? Links { get; set; }
    }
}