public class InMemoryAudit : IAudit
{
    private readonly object _lock = new();
    private readonly List<AuditEntry> _entries = new();
    private long _sequence;

    public IReadOnlyList<AuditEntry> Entries
    {
        get
        {
            lock (_lock)
            {
                return _entries.ToList();
            }
        }
    }

    public void Record(int messageLength)
    {
        lock (_lock)
        {
            _sequence++;
            _entries.Add(new AuditEntry(_sequence, DateTime.UtcNow, messageLength));
        }
    }
}