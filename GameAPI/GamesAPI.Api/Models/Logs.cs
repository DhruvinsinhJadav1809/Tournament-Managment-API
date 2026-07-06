namespace GamesAPI.Api.Models
{
    public class Logs
    {
        public Guid Id { get; set; }
        public string Level { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string Exception { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Page { get; set; } = string.Empty;
        public string FunctionName { get; set; } = string.Empty;
        public Guid? UserId { get; set; }
        public string ModuleType { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

    }
}