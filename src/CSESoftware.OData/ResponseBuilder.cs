using System.Collections.Generic;
using System.Linq;
using CSESoftware.OData.Links;
using CSESoftware.OData.Response;

namespace CSESoftware.OData
{
    /// <summary>
    /// Tool to construct your API response
    /// </summary>
    public class ResponseBuilder<T> where T : class
    {
        private readonly ODataResponse<T> _response;
        private readonly List<Link> _links;

        public ResponseBuilder()
        {
            _response = new ODataResponse<T>();
            _links = new List<Link>();
        }

        /// <summary>
        /// Adds data to the response
        /// </summary>
        /// <param name="data"></param>
        public ResponseBuilder<T> WithData(IEnumerable<T> data)
        {
            _response.Data = data;
            return this;
        }

        /// <summary>
        /// Adds total and response count to the response
        /// </summary>
        /// <param name="totalCount"></param>
        public ResponseBuilder<T> WithCount(int totalCount)
        {
            _response.Count = new Count
                {
                    Response = _response.Data.Count(),
                    Total = totalCount
                };

            return this;
        }

        /// <summary>
        /// Adds HATEOS link to self
        /// </summary>
        /// <param name="fullPath">Full request path including host, path, and query string</param>
        /// <param name="requestMethod">HTTP request method (ex. GET)</param>
        public ResponseBuilder<T> WithLinkToSelf(string fullPath, string requestMethod)
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
        public ResponseBuilder<T> WithLinksForPagination(string fullPath, string requestMethod, int? skip, int? take)
        {
            _links.Add(LinkService.GetLinkToFirstPage(fullPath, skip, requestMethod));
            _links.Add(LinkService.GetLinkToPreviousPage(fullPath, skip, take, requestMethod));
            _links.Add(LinkService.GetLinkToNextPage(fullPath, skip, take, requestMethod));
            _links.Add(LinkService.GetLinkToLastPage(fullPath, skip, take, _response?.Count?.Total ?? 0, requestMethod));

            return this;
        }

        /// <summary>
        /// Adds HATEOS link that can be used for related API calls
        /// </summary>
        public ResponseBuilder<T> WithLink(Link link)
        {
            _links.Add(link);
            return this;
        }

        /// <summary>
        /// Adds HATEOS links that can be used for related API calls
        /// </summary>
        public ResponseBuilder<T> WithLinks(IEnumerable<Link> links)
        {
            _links.AddRange(links);
            return this;
        }

        /// <summary>
        /// Builds the response
        /// </summary>
        /// <returns>object for returning out of your API</returns>
        public ODataResponse<T> Build()
        {
            if (_links.Any())
                _response.Links = _links.Where(x => x != null);

            return _response;
        }
    }
}
