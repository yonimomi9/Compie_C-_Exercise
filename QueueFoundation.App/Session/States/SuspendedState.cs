public class SuspendedState : SessionState
{
    public override string Name => "Suspended";

    public override bool CanTransitionTo(SessionState next)
    {
        return next is ActiveState or LoggingOutState;
    }
}