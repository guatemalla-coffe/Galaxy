using UnityEngine;

public class EatingState : BearState
{
    private GameObject resource;

    public EatingState(BearController bear, GameObject resource) : base(bear)
    {
        this.resource = resource;
    }

    public override void Enter()
    {
        Debug.Log($"{bear.name} начал поедать {resource.name}.");
        bear.bearAnimations.StartEating();
    }

    public override void Update() { /* Логика в команде */ }

    public override void Exit()
    {
        Debug.Log($"{bear.name} завершил поедание.");
        TaskManager.Instance.CompleteTask(15);
        bear.bearAnimations.StopEating();
    }
}