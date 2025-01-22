using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

[RequireComponent(typeof(GameLoopSharedData))]
public class GameLoopRoundState : StateBehaviour
{
    public GameLoopSharedData Config;
    public GameObject StartRoundButton;
    public int CurrentRound = 1;
    public GameObject GameOverScreen;
    public override void OnEnter()
    {
        Config.InterfaceUI.UpdateHelpText("Battle started!","");
        StartRoundButton.SetActive(true);
        CurrentRound = 1;
        ButtonStartRound();
    }
    public void StartRound()
    {
        ButtonSkipAnimation();
        Debug.Log($"Entered round {CurrentRound}!");
        Config.InterfaceUI.UpdateHelpText($"Round {CurrentRound}, every army cohesion reduced by {CurrentRound-1}.","");
        var Logic = Config.Battlefield.GetComponent<BattlefieldLogic>();
        Logic.ApplyAbilities(this);
    }
    public void RoundEnd()
    {
        Debug.Log($"Round {CurrentRound} ended!");
        // Round ending logic
        CurrentRound += 1;
        Config.Battlefield.GetComponent<Battlefield>().logic.RemoveDeadUnits();
        TriggerOnFieldUnitsRoundEffects();
        // Formation Round ending effects
        Config.Battlefield.GetComponent<Battlefield>().logic.FrontShift(Config.PlayerFormation);
        Config.Battlefield.GetComponent<Battlefield>().logic.FrontShift(Config.EnemyFormation);
        // Update field graphic
        Config.Battlefield.GetComponent<Battlefield>().UpdateField();
        CheckWinCondition();
    }

    private void CheckWinCondition()
    {
        if (Config.EnemyFormation.GetOnFieldcompanies().Count == 0)
        {
            ChangeState<GameLoopRewardState>();
        }
        else if (Config.PlayerFormation.GetOnFieldcompanies().Count==0) StartCoroutine(EndGameScreen());
        else ButtonStartRound();
    }
    IEnumerator EndGameScreen()
    {
        GameOverScreen.SetActive(true);
        yield return new WaitForSeconds(5);
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentIndex);
    }

    public override void OnExit()
    {
        Config.Battlefield.GetComponent<Battlefield>().OnBattleEnd();
        Config.Battlefield.SetActive(false);
        StartRoundButton.SetActive(false);
        TriggerAllUnitsBattleEndEffects();
        //TODO??
        Config.EnemyHero.GetComponent<Hero>().bannersList.Clear();
    }
    public void TriggerOnFieldUnitsRoundEffects()
    {
        UnitCharacteristics debuffChar = new UnitCharacteristics(0, 0, 0, 0, -1, 0,0);
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

    public void ButtonStartRound()
    {
        StartRoundButton.GetComponent<Button>().onClick.RemoveAllListeners();
        StartRoundButton.GetComponent<Button>().onClick.AddListener(()=> StartRound());
        StartRoundButton.GetComponent<ButtonManager>().ChangeButtonText($"Start round {CurrentRound}");
    }
    public void ButtonSkipAnimation()
    {
        StartRoundButton.GetComponent<Button>().onClick.RemoveAllListeners();
        StartRoundButton.GetComponent<Button>().onClick.AddListener(()=> OnButtonClickSkip());
        StartRoundButton.GetComponent<ButtonManager>().ChangeButtonText($"Next unit action");
    }
    public void ButtonEndRound()
    {
        StartRoundButton.GetComponent<Button>().onClick.RemoveAllListeners();
        StartRoundButton.GetComponent<Button>().onClick.AddListener(()=> RoundEnd());
        StartRoundButton.GetComponent<ButtonManager>().ChangeButtonText($"End round {CurrentRound}");
    }
    private void OnButtonClickSkip()
    {
        Config.Battlefield.GetComponent<Battlefield>().logic.isAction=true;
    }
    public void RoundButtonAction()
    {
        StartRoundButton.GetComponent<Button>().onClick.Invoke();
    }
}
