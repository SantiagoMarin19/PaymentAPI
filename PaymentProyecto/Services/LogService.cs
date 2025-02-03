using System;
using System.IO;
using System.Threading.Tasks;

public class LogService
{
    private readonly string logDirectory;

    public LogService()
    {
        logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
        if (!Directory.Exists(logDirectory))
        {
            Directory.CreateDirectory(logDirectory);
        }
    }

    public async Task LogTransactionAsync(string message)
    {
        string logFileName = $"transacciones_{DateTime.Now:yyyyMMdd}.log";
        string logFilePath = Path.Combine(logDirectory, logFileName);
        string logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}{Environment.NewLine}";

        await File.AppendAllTextAsync(logFilePath, logMessage);
    }
}