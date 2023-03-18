namespace Ecommerce.Client.BackendClient.Exceptions;

internal sealed class NotFoundException : Exception
{
    public string Reason { get; set; } = string.Empty;
    public NotFoundException(string reason)
        : base($"The backend response with Not found status with the reason: {reason}")
    {
        Reason = reason;
    }
}