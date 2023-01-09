using Newtonsoft.Json;

namespace EmbyStat.Common.Extensions;

public static class JsonExtensions
{
    public static string BuildJson<T>(this T card)
    {
        return JsonConvert.SerializeObject(card);
    }
}