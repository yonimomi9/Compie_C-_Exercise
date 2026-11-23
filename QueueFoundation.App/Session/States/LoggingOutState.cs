public class LoggingOutState : SessionState
{
    public override string Name => "LoggingOut";

    public override bool CanTransitionTo(SessionState next)
    {
        return next is DisconnectedState;
    }
}