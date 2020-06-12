namespace CSESoftware.OData.QueryBuilder
{
    public enum Operation
    {
        Equals,
        NotEqualTo,
        GreaterThan,
        LessThan,
        GreaterThanOrEqualTo,
        LessThanOrEqualTo,
        Contains
    }

    public static class OperationExtensions
    {
        public static string ToOperationString(this Operation operation)
        {
            switch (operation)
            {
                case Operation.Equals:
                    return "eq";
                case Operation.NotEqualTo:
                    return "ne";
                case Operation.GreaterThan:
                    return "gt";
                case Operation.LessThan:
                    return "lt";
                case Operation.GreaterThanOrEqualTo:
                    return "ge";
                case Operation.LessThanOrEqualTo:
                    return "le";
                case Operation.Contains:
                    return "contains";
                default:
                    return "";
            }
        }
    }
}
