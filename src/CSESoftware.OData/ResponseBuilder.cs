using System.Collections.Generic;
using System.Linq;
using CSESoftware.OData.Links;

namespace CSESoftware.OData
{
    /// <summary>
    /// Helps build your API response
    /// </summary>
    public class ResponseBuilder
    {
        private readonly Dictionary<string, object> _response;
        private readonly List<Link> _links;

        public ResponseBuilder()
        {
            _response = new Dictionary<string, object>();
            _links = new List<Link>();
        }

        /// <summary>
        /// Adds data to the response
        /// </summary>
        /// <param name="data"></param>
        public ResponseBuilder WithData(object data)
        {
            _response.Add("data", data);
            return this;
        }

        /// <summary>
        /// Adds total and response count to the response
        /// </summary>
        /// <param name="responseCount"></param>
        /// <param name="totalCount"></param>
        public ResponseBuilder WithCount(int responseCount, int totalCount)
        {
            _response.Add("count", new Dictionary<string, int>
            {
                { "response", responseCount },
                { "total", totalCount }
            });
            return this;
        }

        /// <summary>
        /// Adds HATEOS link to self
        /// </summary>
        /// <param name="fullPath">Full request path including host, path, and query string</param>
        /// <param name="requestMethod">HTTP request method (ex. GET)</param>
        public ResponseBuilder WithLinkToSelf(string fullPath, string requestMethod)
        {
            _links.Add(new Link
            {
                Relation = LinkNames.Self,
                URL = fullPath,
                Type = requestMethod
            });
            return this;
        }

        /// <summary>
        /// Adds HATEOS links for pagination
        /// </summary>
        /// <param name="fullPath">Full request path including host, path, and query string</param>
        /// <param name="requestMethod">HTTP request method (ex. GET)</param>
        /// <param name="skip">number of records skipped</param>
        /// <param name="take">page size</param>
        /// <param name="totalCount">total number of records across all pages</param>
        public ResponseBuilder WithLinksForPagination(string fullPath, string requestMethod, int? skip, int? take, int? totalCount)
        {
            _links.Add(LinkService.GetLinkToFirstPage(fullPath, skip, requestMethod));
            _links.Add(LinkService.GetLinkToPreviousPage(fullPath, skip, take, requestMethod));
            _links.Add(LinkService.GetLinkToNextPage(fullPath, skip, take, requestMethod));
            _links.Add(LinkService.GetLinkToLastPage(fullPath, skip, take, totalCount ?? 0, requestMethod));

            return this;
        }

        /// <summary>
        /// Adds HATEOS link that can be used for related API calls
        /// </summary>
        public ResponseBuilder WithLink(Link link)
        {
            _links.Add(link);
            return this;
        }

        /// <summary>
        /// Adds HATEOS links that can be used for related API calls
        /// </summary>
        public ResponseBuilder WithLinks(IEnumerable<Link> links)
        {
            _links.AddRange(links);
            return this;
        }

        /// <summary>
        /// Adds another property to the response
        /// </summary>
        /// <param name="key">property name</param>
        /// <param name="value">property value</param>
        public ResponseBuilder WithProperty(string key, object value)
        {
            _response.Add(key, value);
            return this;
        }

        /// <summary>
        /// Builds the response
        /// </summary>
        /// <returns>Dictionary for returning out of your API</returns>
        public Dictionary<string, object> Build()
        {
            if (_links.Any())
                _response.Add("links", _links.Where(x => x != null));

            return _response;
        }
    }
}