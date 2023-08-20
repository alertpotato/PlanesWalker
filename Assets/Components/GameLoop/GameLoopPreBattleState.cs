using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GameLoopSharedData))]
public class GameLoopPreBattleState : StateBehaviour
{
    private GameLoopSharedData Config;
    public override void OnEnter()
    {

    }

    public override void OnUpdate()
    {
        ChangeState<GameLoopRewardState>();
    }
}
