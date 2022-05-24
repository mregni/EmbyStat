using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace EmbyStat.Common.Configuration;

public class WritableJsonConfigurationSource: JsonConfigurationSource
{
    public override IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        EnsureDefaults(builder);
        return new WritableJsonConfigurationProvider(this);
    }
}