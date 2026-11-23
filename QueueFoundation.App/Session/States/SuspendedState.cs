public class SuspendedState : SessionState
{
    public override string Name => "Suspended";

    public override bool CanTransitionTo(SessionState next)
    {
        var s = new Session();
        return next is ActiveState or LoggingOutState;
    }
}