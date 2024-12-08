using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScienceState : BearState
{
    private GameObject craftable;
    private float craftDuration;

    public ScienceState(BearController bear, GameObject craftable, float craftDuration) : base(bear)
    {
        this.craftable = craftable;
        this.craftDuration = craftDuration;
    }

    public override void Enter()
    {
        Debug.Log($"{bear.name} начал изучение {craftable.name}.");
        bear.bearAnimations.StartScience();
        
    }

    public override void Update()
    {
        // Здесь можно добавить дополнительные проверки
    }

    public override void Exit()
    {
        bear.bearAnimations.StopScience();
        Debug.Log($"{bear.name} завершил изучение {craftable.name}.");
    }
}
