public class Session
{
    public SessionState State { get; private set; }

    public Session() : this(new DisconnectedState())
    {
    }

    public Session(SessionState initialState)
    {
        State = initialState;
    }

    public bool CanSend => State.CanSend;

    public void TransitionTo(SessionState next)
    {
        if (!State.CanTransitionTo(next))
        {
            throw new InvalidOperationException(
                $"Transition from {State.Name} to {next.Name} is not allowed.");
        }

        State = next;
    }
}