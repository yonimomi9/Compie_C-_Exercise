public abstract class SessionState
{
    public abstract string Name { get; }

    public virtual bool CanSend => false;

    public abstract bool CanTransitionTo(SessionState next);
}