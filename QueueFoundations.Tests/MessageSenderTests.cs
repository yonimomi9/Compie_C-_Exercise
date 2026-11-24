namespace QueueFoundations.Tests;

public class MessageSenderTests
{
    [Fact]
    public void Send_WhenSessionNotActive_DoesNotReachQueueOrAudit()
    {
        var session = new Session(new SuspendedState());
        var queue = new SimulatorQueue();
        var audit = new InMemoryAudit();
        var sender = new MessageSender(session, queue, audit);

        bool result = sender.Send(new byte[] { 1, 2, 3 });

        Assert.False(result);
        Assert.Empty(queue.Messages);
        Assert.Empty(audit.Entries);
    }

    [Fact]
    public void Send_WhenSessionActive_ReachesQueueAndIsAudited()
    {
        var session = new Session(new ActiveState());
        var queue = new SimulatorQueue();
        var audit = new InMemoryAudit();
        var sender = new MessageSender(session, queue, audit);
        var message = new byte[] { 1, 2, 3 };

        bool result = sender.Send(message);

        Assert.True(result);
        Assert.Single(queue.Messages);
        Assert.Equal(message, queue.Messages[0]);

        var entry = Assert.Single(audit.Entries);
        Assert.Equal(1, entry.Sequence);
        Assert.Equal(3, entry.MessageLength);
        Assert.Equal(DateTimeKind.Utc, entry.TimestampUtc.Kind);
    }

    
    [Fact]
    public void Send_WhenSuspended_IsHeldAndSentInOrderWhenBackToActive()
    {
        var session = new Session(new SuspendedState());
        var queue = new SimulatorQueue();
        var audit = new InMemoryAudit();
        var sender = new MessageSender(session, queue, audit);
        var first = new byte[] { 1 };
        var second = new byte[] { 2, 2 };

        sender.Send(first);
        sender.Send(second);

        Assert.Empty(queue.Messages);
        Assert.Equal(2, sender.PendingCount);

        session.TransitionTo(new ActiveState());

        Assert.Equal(2, queue.Messages.Count);
        Assert.Equal(first, queue.Messages[0]);
        Assert.Equal(second, queue.Messages[1]);
        Assert.Equal(0, sender.PendingCount);
        Assert.Equal(2, audit.Entries.Count);
    }
}