namespace Archiver5E2D.Exceptions;

public class CompressorNotFoundException : Exception
{
    public CompressorNotFoundException(string message, Exception innerException) 
        : base(message, innerException)
    {}
}