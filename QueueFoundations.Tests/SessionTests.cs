namespace QueueFoundations.Tests;

public class SessionTests
{
    [Fact]
    public void TransitionTo_FromSuspendedToActive_Succeeds()
    {
        var session = new Session(new SuspendedState());

        session.TransitionTo(new ActiveState());

        Assert.IsType<ActiveState>(session.State);
    }

    [Fact]
    public void TransitionTo_FromDisconnectedToActive_ThrowsAndKeepsState()
    {
        var session = new Session(new DisconnectedState());

        Assert.Throws<InvalidOperationException>(() => session.TransitionTo(new ActiveState()));

        Assert.IsType<DisconnectedState>(session.State);
    }
    // Assignment part 7 tests: Reconnceting State
    [Fact]
    public void TransitionTo_FromActiveToReconnecting_Succeeds()
    {
        var session = new Session(new ActiveState());

        session.TransitionTo(new ReconnectingState());

        Assert.IsType<ReconnectingState>(session.State);
    }

    [Fact]
    public void TransitionTo_FromReconnectingToActive_ThrowsAndKeepsState()
    {
        var session = new Session(new ReconnectingState());

        Assert.Throws<InvalidOperationException>(() => session.TransitionTo(new ActiveState()));

        Assert.IsType<ReconnectingState>(session.State);
    }
}