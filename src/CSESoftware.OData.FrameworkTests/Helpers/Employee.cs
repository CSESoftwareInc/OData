using CSESoftware.Core.Entity;

namespace CSESoftware.OData.FrameworkTests.Helpers
{
    public class Employee : BaseEntity<int>
    {
        public string Name { get; set; }
    }
}
