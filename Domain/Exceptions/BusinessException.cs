
namespace Domain.Exceptions;

public class BusinessException : Exception
{
    public override string? Message { get; }
    public BusinessException(string message)
    {
        Message = message;
    }
}
