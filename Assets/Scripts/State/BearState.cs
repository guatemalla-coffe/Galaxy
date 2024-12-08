public abstract class BearState
{
    protected BearController bear;

    public BearState(BearController bear)
    {
        this.bear = bear;
    }

    public abstract void Enter();
    public abstract void Update();
    public abstract void Exit();
}