using CSESoftware.OData.Response;

namespace CSESoftware.OData.Links
{
    internal class LinkService
    {
        internal static Link GetLinkToFirstPage(string requestUrl, int? skip, string requestMethod = "GET")
        {
            return new Link
            {
                Relation = LinkNames.FirstPage,
                URL = requestUrl.Replace("$skip=" + skip, "$skip=0"),
                Type = requestMethod
            };
        }

        internal static Link GetLinkToPreviousPage(string requestUrl, int? skip, int? take, string requestMethod = "GET")
        {
            if (skip == null || take == null) return null;

            var newSkip = skip - (take ?? 0);

            if (newSkip < 0) newSkip = 0;

            return new Link
            {
                Relation = LinkNames.PreviousPage,
                URL = requestUrl.Replace("$skip=" + skip, "$skip=" + newSkip),
                Type = requestMethod
            };
        }

        internal static Link GetLinkToNextPage(string requestUrl, int? skip, int? take, string requestMethod = "GET")
        {
            if (take == null) return null;

            return new Link
            {
                Relation = LinkNames.NextPage,
                URL = skip == null ?
                    $"{requestUrl}$skip={take}"
                    : requestUrl.Replace("$skip=" + skip, "$skip=" + (skip + take)),
                Type = requestMethod
            };
        }

        internal static Link GetLinkToLastPage(string requestUrl, int? skip, int? take, int totalCount, string requestMethod = "GET")
        {
            if (take == null || take < 1) return null;

            var lastPageSkip = (totalCount / take);

            return new Link
            {
                Relation = LinkNames.LastPage,
                URL = skip == null ?
                    $"{requestUrl}$skip={lastPageSkip}"
                    : requestUrl.Replace("$skip=" + skip, "$skip=" + lastPageSkip),
                Type = requestMethod
            };
        }
    }
}