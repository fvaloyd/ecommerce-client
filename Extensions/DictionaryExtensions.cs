using System.Text;

namespace Ecommerce.Client.Extensions;

public static class DictionaryExtensions
{
    public static string ToQueryString(this Dictionary<string, string> queryParams)
    {
        var firstParam = queryParams.First();

        var builder = new StringBuilder($"?{firstParam.Key}={firstParam.Value}");

        foreach(KeyValuePair<string, string> kvp in queryParams.Skip(1))
        {
            builder.Append($"&{kvp.Key}={kvp.Value}");
        }

        return builder.ToString();
    }
}