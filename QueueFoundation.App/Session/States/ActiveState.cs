public class ActiveState : SessionState
{
    public override string Name => "Active";

    public override bool CanSend => true;

    public override bool CanTransitionTo(SessionState next)
    {
        return next is SuspendedState or LoggingOutState;
    }
}