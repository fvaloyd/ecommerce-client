using Ecommerce.Client.BackendClient.Exceptions;

using System.Net;

namespace Ecommerce.Client.BackendClient.MessageHandlers;

public class ThrowExceptionHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var response = await base.SendAsync(request, cancellationToken);

        return response.StatusCode switch {
            HttpStatusCode.BadRequest => throw new BadRequestException(await response.Content.ReadAsStringAsync()),
            HttpStatusCode.NotFound => throw new NotFoundException(await response.Content.ReadAsStringAsync()),
            HttpStatusCode.InternalServerError => throw new InternalServerErrorException(await response.Content.ReadAsStringAsync()),
            _ => response
        };
    }
}