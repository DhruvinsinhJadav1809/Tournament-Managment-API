using GamesAPI.Api.Data;
using GamesAPI.Api.Models;

public class LogService
    : ILogService
{
    private readonly
        AppDbContext _context;

    public LogService(
        AppDbContext context)
    {
        _context = context;
    }

    public async Task LogAsync(
        string level,
        string message,
        string category,
        string functionName,
        Guid? userId,
        string? exception = null,
        string? moduleType = null,
        string page = "")
    {
        var log =
            new Logs
            {
                Id = Guid.NewGuid(),

                Level = level,

                Message = message,

                Category = category,

                FunctionName =
                    functionName,

                UserId =
                    userId,

                Exception =
                    exception,

                ModuleType =
                    moduleType,

                CreatedAt =
                    DateTime.Now,
                Page = page
            };

        _context.Logs
            .Add(log);

        await _context
            .SaveChangesAsync();
    }
}