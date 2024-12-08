using UnityEngine;

public class IdleState : BearState
{
    public IdleState(BearController bear) : base(bear) { }

    public override void Enter()
    {
        Debug.Log($"{bear.name} is now Idle.");
    }

    public override void Update() { /* Ничего не делать */ }
    public override void Exit() => Debug.Log($"{bear.name} exits Idle.");
}