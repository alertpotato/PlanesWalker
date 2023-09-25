using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GameLoopSharedData))]
public class GameLoopBattleState : StateBehaviour
{
    public GameLoopSharedData Config;
    public override void OnEnter()
    {
    }
    public override void OnExit()
    {
        Config.Battlefield.SetActive(false);
    }
    public override void OnUpdate()
    {
    }
}
