using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(GameLoopSharedData))]
public class GameLoopPreBattleState : StateBehaviour
{
    public GameLoopSharedData Config;
    public GameObject StartBattleButton;
    public override void OnEnter()
    {
        Config.CurrentRound = 0;
        Config.Battlefield.SetActive(true);
        StartBattleButton.SetActive(true);
        Config.UpdateHelpText("Pre battle state","Q to look at owned cards, left click on any field piece to reset.");
        
        ///Init field
        var newField = new List<(int,int)>() { (0,1), (0,2), (0,3), (1,2) };
        Config.PlayerFormation.RebuildField(newField);
        Config.EnemyFormation.RebuildField(newField);
        /// Draw field
        Config.Battlefield.GetComponent<Battlefield>().RebuildField(Config.PlayerFormation,Config.EnemyFormation);
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
        ChangeState<GameLoopRoundState>();
    }
    void OnShowarmy(InputValue value)
    {
        if (!IsActive()) return;
        Debug.Log("Hello Q");
        Config.ArmyDeck.GetComponent<ArmyDeck>().UpdateDeck();
    }
}
