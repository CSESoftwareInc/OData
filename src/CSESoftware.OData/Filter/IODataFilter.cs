namespace CSESoftware.OData.Filter
{
    /// <summary>
    /// Filter object to be received by your controller
    /// </summary>
    public interface IODataFilter
    {
        /// <summary>
        /// $top -- How many entities to return (page size)
        /// </summary>
        int? Take { get; set; }

        /// <summary>
        /// $skip -- How many entities to skip (page number = ($skip / $top) + 1)
        /// </summary>
        int? Skip { get; set; }

        /// <summary>
        /// $count -- Should return the total count of entities
        /// </summary>
        bool? Count { get; set; }

        /// <summary>
        /// $filter
        /// </summary>
        string Filter { get; set; }

        /// <summary>
        /// $orderBy
        /// </summary>
        string OrderBy { get; set; }

        /// <summary>
        /// $thenBy
        /// </summary>
        string ThenBy { get; set; }

        /// <summary>
        /// $expand -- Expand these foreign relations
        /// </summary>
        string Expand { get; set; }

        /// <summary>
        /// $links -- Should return HATEOS links
        /// </summary>
        bool? Links { get; set; }
    }
}