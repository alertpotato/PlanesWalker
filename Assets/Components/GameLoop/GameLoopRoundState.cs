using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GameLoopSharedData))]
public class GameLoopRoundState : StateBehaviour
{
    public GameLoopSharedData Config;
    public override void OnEnter()
    {
        Config.CurrentRound += 1;
        Debug.Log($"Entered round {Config.CurrentRound}!");
        var Logic = Config.Battlefield.GetComponent<BattlefieldLogic>();
        Logic.ApplyAbilities();
    }
    public override void OnExit()
    {
        Config.Battlefield.SetActive(false);
    }
    public override void OnUpdate()
    {
        
    }

}
