using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GameLoopSharedData))]
public class GameLoopPreBattleState : StateBehaviour
{
    public GameLoopSharedData Config;
    public GameObject StartBattleButton;
    public override void OnEnter()
    {
        Config.Battlefield.SetActive(true);
        StartBattleButton.SetActive(true);
    }
    public override void OnExit()
    {
        StartBattleButton.SetActive(false);
    }
    public override void OnUpdate()
    {
        //ChangeState<GameLoopRewardState>();
    }
    public void StartBattle()
    {
        ChangeState<GameLoopBattleState>();
    }
}
