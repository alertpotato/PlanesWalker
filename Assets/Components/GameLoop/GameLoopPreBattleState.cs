using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GameLoopSharedData))]
public class GameLoopPreBattleState : StateBehaviour
{
    public GameLoopSharedData Config;
    public override void OnEnter()
    {
        Config.Battlefield.SetActive(true); 
    }
    public override void OnUpdate()
    {
        //ChangeState<GameLoopRewardState>();
    }
}
