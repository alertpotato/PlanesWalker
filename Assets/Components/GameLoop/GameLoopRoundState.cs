using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(GameLoopSharedData))]
public class GameLoopRoundState : StateBehaviour
{
    public GameLoopSharedData Config;
    public GameObject StartRoundButton;
    public int CurrentRound = 0;
    public override void OnEnter()
    {
        Config.UpdateHelpText("Battle started, Round "+CurrentRound,"Defeat your enemies");
        StartRoundButton.SetActive(true);
        CurrentRound = 1;
    }
    public void StartRound()
    {
        Debug.Log($"Entered round {CurrentRound}!");
        Config.UpdateHelpText("Battle started, Round "+CurrentRound,"Defeat your enemies");
        var Logic = Config.Battlefield.GetComponent<BattlefieldLogic>();
        Logic.ApplyAbilities();
        // Update field graphic
        Config.Battlefield.GetComponent<Battlefield>().UpdateField();
        // Round ending logic
        CurrentRound += 1;
        TriggerOnFieldUnitsRoundEffects();
        // Formation Round ending effects
        Config.PlayerFormation.OnRoundEnd();
        Config.EnemyFormation.OnRoundEnd();
    }
    public override void OnExit()
    {
        Config.Battlefield.SetActive(false);
        StartRoundButton.SetActive(false);
        TriggerAllUnitsBattleEndEffects();
    }
    public void TriggerOnFieldUnitsRoundEffects()
    {
        UnitCharacteristics debuffChar = new UnitCharacteristics(0, 0, 0, 0, -1, 0);
        UnitBuff debuff = new UnitBuff(debuffChar, Config.GameObject(), 999);
        List<UnitBuff> BuffList = new List<UnitBuff>();
        BuffList.Add(debuff);
        
        foreach (var comp in Config.PlayerFormation.GetOnFieldcompanies())
        {
            if (CurrentRound > 1) comp.Unit.GetComponent<ArmyUnitClass>().ReciveBuffs(BuffList);
            comp.Unit.GetComponent<ArmyUnitClass>().OnRoundEnd();
        }
        foreach (var comp in Config.EnemyFormation.GetOnFieldcompanies())
        {
            if (CurrentRound > 1) comp.Unit.GetComponent<ArmyUnitClass>().ReciveBuffs(BuffList);
            comp.Unit.GetComponent<ArmyUnitClass>().OnRoundEnd();
        }
    }
    public void TriggerAllUnitsBattleEndEffects()
    {
        foreach (var unit in Config.PlayerHero.GetComponent<Hero>().bannersList)
        {
            unit.GetComponent<ArmyUnitClass>().OnBattleEnd();
        }
        foreach (var unit in Config.EnemyHero.GetComponent<Hero>().bannersList)
        {
            unit.GetComponent<ArmyUnitClass>().OnBattleEnd();
        }
    }
}
