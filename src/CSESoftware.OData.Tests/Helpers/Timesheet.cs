﻿using CSESoftware.Core.Entity;

namespace CSESoftware.OData.Tests.Helpers
{
    public class Timesheet : BaseEntity<int>
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
        public string Comment { get; set; }
    }
}
