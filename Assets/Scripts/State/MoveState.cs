using Pathfinding;
using UnityEngine;

public class MoveState : BearState
{
    private Vector3 targetPosition;
    private float endDistance;
    public GridGraph graph;
    private Vector3 lastPosition;

    public MoveState(BearController bear, Vector3 target, float endDistance) : base(bear)
    {
        targetPosition = target;
        this.endDistance = endDistance;
    }

    public override void Enter()
    {
        bear.target.position = targetPosition;
        bear.GetComponent<AIDestinationSetter>().target = bear.target;
        bear.bearAnimations.StartMoving();
        Debug.Log($"{bear.name} starts moving.");
    }

    public override void Update()
    {
        bear.transform.position = new Vector3(bear.transform.position.x, bear.transform.position.y, 0);
        if (Vector3.Distance(bear.transform.position, targetPosition) < endDistance)
        {
            Exit();
            bear.SetState(new IdleState(bear)); // Переход в Idle после достижения точки
        }
    }

    public override void Exit()
    { 
        bear.GetComponent<AIDestinationSetter>().target = null;
        bear.bearAnimations.StopMoving();
        
        Debug.Log($"{bear.name} stops moving.");
    }
}