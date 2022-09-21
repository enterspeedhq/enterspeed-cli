using System.Runtime.Serialization;

namespace Enterspeed.Cli.Domain.Exceptions;

[Serializable]
public class InvalidIdFormatException : Exception
{
    public InvalidIdFormatException(string type)
        : base($"Invalid Id format for: {type}")
    {
    }

    public InvalidIdFormatException()
    {
    }

    public InvalidIdFormatException(string message, Exception innerException) : base(message, innerException)
    {
    }

    protected InvalidIdFormatException(SerializationInfo serializationInfo, StreamingContext streamingContext) : base(serializationInfo, streamingContext)
    {
    }
}