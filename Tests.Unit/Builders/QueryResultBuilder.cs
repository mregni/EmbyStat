using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Querying;

namespace Tests.Unit.Builders;

public class QueryResultBuilder
{
    private readonly QueryResult<BaseItemDto> _result;

    public QueryResultBuilder(string name, string id)
    {
        _result = new QueryResult<BaseItemDto>
        {
            Items = new[]
            {
                new BaseItemDto
                {
                    Id = id,
                    Name = name
                }
            },
            TotalRecordCount = 1
        };
    }

    public QueryResult<BaseItemDto> Build()
    {
        return _result;
    }
}