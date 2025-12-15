namespace Dx.Domain.Errors
{
    public class DomainErrorException : Exception
    {
        public DomainErrorException()
        {
        }

        public DomainErrorException(string? message) : base(message)
        {
        }

        public DomainErrorException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
