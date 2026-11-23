public class ReconnectingState : SessionState
{
    public override string Name => "Reconnecting";

    public override bool CanTransitionTo(SessionState next)
    {
        return next is LoggingInState or DisconnectedState;
    }
}