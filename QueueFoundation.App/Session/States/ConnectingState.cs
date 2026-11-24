public class ConnectingState : SessionState
{
    public override string Name => "Connecting";

    public override bool CanTransitionTo(SessionState next)
    {
        return next is LoggingInState or DisconnectedState;
    }
}