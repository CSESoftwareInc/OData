using CSESoftware.Core.Entity;

namespace CSESoftware.OData.Tests.Helpers
{
    public class Employee : BaseEntity<int>
    {
        public string Name { get; set; }
    }
}
