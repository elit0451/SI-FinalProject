namespace LoggerService.Model
{
    public enum LogLevel {
        INFO,
        WARNING,
        CRITICAL
    }

    public class Log
    {
        public LogLevel Level { get; set; }
        public string Message { get; set; }    
        public string Service { get; set; }    
    }
}