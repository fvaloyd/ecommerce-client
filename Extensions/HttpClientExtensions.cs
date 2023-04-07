using System.Text;
using Newtonsoft.Json;

namespace Ecommerce.Client.Extensions;

public static class HttpClientExtensions
{
    public static async Task<T> GetContentObjectAsync<T>(this HttpResponseMessage httpResponseMessage)
    {
        var content = await httpResponseMessage.Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject<T>(content)!;
    }

    public static async Task<T> GetAsyncWrapper<T>(this HttpClient client, string url, string? routeValue = null, Dictionary<string, string>? queryString = null)
    {
        
        StringBuilder stringBuilder = new StringBuilder(url);
        
        if (routeValue is not null)
        {
            stringBuilder.Append(routeValue);
        }

        if (queryString is not null && queryString.Count > 0)
        {
            stringBuilder.Append(queryString.ToQueryString());
        }

        HttpResponseMessage response = await client.GetAsync(stringBuilder.ToString());

        return await response.GetContentObjectAsync<T>();
    }
}