using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(GameLoopSharedData))]
public class GameLoopRoundState : StateBehaviour
{
    public GameLoopSharedData Config;
    public GameObject StartRoundButton;
    public int CurrentRound = 0;
    public GameObject GameOverScreen;
    public TextMeshProUGUI Score;
    public override void OnEnter()
    {
        Config.UpdateHelpText("Battle started!","Defeat your enemies");
        StartRoundButton.SetActive(true);
        CurrentRound = 1;
    }
    public void StartRound()
    {
        StartRoundButton.SetActive(false);
        Debug.Log($"Entered round {CurrentRound}!");
        Config.UpdateHelpText($"Round {CurrentRound}, every army cohesion reduced by {CurrentRound-1}.","Defeat your enemies");
        var Logic = Config.Battlefield.GetComponent<BattlefieldLogic>();
        Logic.Order();
        Logic.ApplyAbilities(this);
    }
    public void RoundEnd()
    {
        // Round ending logic
        CurrentRound += 1;
        TriggerOnFieldUnitsRoundEffects();
        // Formation Round ending effects
        Config.PlayerFormation.OnRoundEnd();
        Config.EnemyFormation.OnRoundEnd();
        // Update field graphic
        Config.Battlefield.GetComponent<Battlefield>().UpdateField();
        CheckWinCondition();
    }

    private void CheckWinCondition()
    {
        if (Config.EnemyFormation.GetOnFieldcompanies().Count == 0)
        {
            Config.TempRewards();
            Config.BattlesWon += 1;
            Score.text = $"Battles Won: {Config.BattlesWon}";
            ChangeState<GameLoopRewardState>();
        }
        else if (Config.PlayerFormation.GetOnFieldcompanies().Count==0) StartCoroutine(EndGameScreen());
        else StartRoundButton.SetActive(true);
    }
    IEnumerator EndGameScreen()
    {
        GameOverScreen.SetActive(true);
        yield return new WaitForSeconds(10);
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentIndex);
    }

    public override void OnExit()
    {
        Config.Battlefield.GetComponent<Battlefield>().DestroyField();
        Config.Battlefield.SetActive(false);
        StartRoundButton.SetActive(false);
        TriggerAllUnitsBattleEndEffects();
        //TODO??
        Config.EnemyHero.GetComponent<Hero>().bannersList.Clear();
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
