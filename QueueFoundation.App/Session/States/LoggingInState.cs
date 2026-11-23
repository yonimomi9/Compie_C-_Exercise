public class LoggingInState : SessionState
{
    public override string Name => "LoggingIn";

    public override bool CanTransitionTo(SessionState next)
    {
        return next is ActiveState or DisconnectedState;
    }
}