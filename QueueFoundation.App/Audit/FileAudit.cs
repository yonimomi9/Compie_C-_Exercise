using Microsoft.Extensions.Options;

public class AuditOptions
{
    public string FilePath { get; set; } = "audit.log";
}

public class FileAudit : IAudit
{
    private readonly object _lock = new();
    private readonly string _filePath;
    private long _sequence;

    public FileAudit(IOptions<AuditOptions> options)
    {
        _filePath = options.Value.FilePath;
    }

    public void Record(int messageLength)
    {
        lock (_lock)
        {
            _sequence++;
            string line = $"{_sequence}|{DateTime.UtcNow:O}|{messageLength}{Environment.NewLine}";
            File.AppendAllText(_filePath, line);
        }
    }
}