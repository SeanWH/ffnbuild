namespace ffnbuild.data.exceptions;

using System;

public class AddressParseException : Exception
{
    public AddressParseException(string message) : base(message)
    {
    }

    public AddressParseException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public AddressParseException() : base()
    {
    }
}