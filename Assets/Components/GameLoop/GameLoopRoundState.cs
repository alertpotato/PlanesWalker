using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GameLoopSharedData))]
public class GameLoopRoundState : StateBehaviour
{
    public GameLoopSharedData Config;
    public GameObject StartRoundButton;
    public override void OnEnter()
    {
        Config.UpdateHelpText("Battle started, Round "+Config.CurrentRound,"Defeat your enemies");
        StartRoundButton.SetActive(true);
    }
    public void StartRound()
    {
        Config.CurrentRound += 1;
        Debug.Log($"Entered round {Config.CurrentRound}!");
        Config.UpdateHelpText("Battle started, Round "+Config.CurrentRound,"Defeat your enemies");
        var Logic = Config.Battlefield.GetComponent<BattlefieldLogic>();
        Logic.ApplyAbilities();

    }
    public override void OnExit()
    {
        Config.Battlefield.SetActive(false);
        StartRoundButton.SetActive(false);
    }
    public override void OnUpdate()
    {
        
    }

}
