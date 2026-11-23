public class DisconnectedState : SessionState
{
    public override string Name => "Disconnected";

    public override bool CanTransitionTo(SessionState next)
    {
        return next is ConnectingState;
    }
}