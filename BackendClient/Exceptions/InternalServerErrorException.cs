namespace Ecommerce.Client.BackendClient.Exceptions;

internal sealed class InternalServerErrorException : Exception
{
    public string Reason { get; set; }
    public InternalServerErrorException(string reason) 
        : base($"The backend response with internal server error with the reason: {reason}")
    {
        Reason = reason;
    }
}