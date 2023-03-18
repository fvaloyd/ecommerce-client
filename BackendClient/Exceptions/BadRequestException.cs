namespace Ecommerce.Client.BackendClient.Exceptions;

internal sealed class BadRequestException : Exception
{
    public string Reason { get; set; }
    public BadRequestException(string reason)
        : base($"The backend response with bad request status with the reason: {reason}")
    {
        Reason = reason;
    }
}