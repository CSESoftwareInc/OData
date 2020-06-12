using System.Collections.Generic;

namespace CSESoftware.OData.Response
{
    /// <summary>
    /// Response object that can be serialized and returned from the API
    /// </summary>
    /// <typeparam name="T">The type of data you are returning in the Data list</typeparam>
    public class ODataResponse<T> where T : class
    {
        public IEnumerable<T> Data { get; set; }
        public Count Count { get; set; }
        public IEnumerable<Link> Links { get; set; }
    }
}
