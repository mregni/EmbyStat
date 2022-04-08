using EmbyStat.Common.Models.Query;

namespace Tests.Unit.Builders;

public class FilterBuilder
{
    private readonly Filter filter;

    public FilterBuilder(string field, string operation, string value)
    {
        filter = new Filter
        {
            Field = field,
            Operation = operation,
            Value = value
        };
    }

    public Filter Build()
    {
        return filter;
    }
}