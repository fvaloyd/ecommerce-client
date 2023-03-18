using System.Text;
using Newtonsoft.Json;

namespace Ecommerce.Client.Extensions;

public static class HttpClientExtensions
{
    public static async Task<T> GetContentObject<T>(this HttpResponseMessage httpResponseMessage)
    {
        var content = await httpResponseMessage.Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject<T>(content)!;
    }

    public static async Task<T> GetAsyncWrapper<T>(this HttpClient client, string url, string? routeValue = null, Dictionary<string, string>? query = null)
    {
        
        StringBuilder stringBuilder = new StringBuilder(url);
        
        if (routeValue is not null)
        {
            stringBuilder.Append(routeValue);
        }

        if (query is not null && query.Count > 0)
        {
            stringBuilder.Append(query.ToQueryString());
        }

        HttpResponseMessage response = await client.GetAsync(stringBuilder.ToString());

        return await response.GetContentObject<T>();
    }
}