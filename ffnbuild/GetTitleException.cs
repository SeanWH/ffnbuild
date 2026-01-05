namespace ffnbuild;

using System;

public class GetTitleException : Exception
{
    public GetTitleException()
    {
    }

    public GetTitleException(string? message) : base(message)
    {
    }

    public GetTitleException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}