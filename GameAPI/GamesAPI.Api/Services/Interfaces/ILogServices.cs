public interface ILogService
{
    Task LogAsync(
        string level,
        string message,
        string category,
        string functionName,
        Guid? userId,
        string? exception = null,
        string? moduleType = null,
        string page = ""
    );
}