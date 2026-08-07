namespace TaskFlow.Application.Common.Exceptions;

public class AppException : Exception
{
    public int StatusCode { get; }
    public AppException(string message, int statusCode = 400) : base(message) => StatusCode = statusCode;
}
public class NotFoundException : AppException { public NotFoundException(string m) : base(m, 404) { } }
public class UnauthorizedException : AppException { public UnauthorizedException(string m = "Unauthorized") : base(m, 401) { } }
public class ForbiddenException : AppException { public ForbiddenException(string m = "Forbidden") : base(m, 403) { } }
