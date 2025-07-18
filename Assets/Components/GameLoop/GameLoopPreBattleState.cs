using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using UnityEngine.Rendering;

[RequireComponent(typeof(GameLoopSharedData))]
public class GameLoopPreBattleState : StateBehaviour
{
    public GameLoopSharedData Config;
    public GameObject StartBattleButton;
    public List<GameObject> EnemyEventUnits = new List<GameObject>();
    public Dictionary<FormationType, int> EnemyEventFormation = new Dictionary<FormationType, int>();
    [SerializeField]private bool DeployingPhase = true;
    [SerializeField]private int unitsToPlace = 0;
    public override void OnEnter()
    {
        Config.Battlefield.SetActive(true);
        StartBattleButton.SetActive(true);
        Config.InterfaceUI.UpdateHelpText("Pre battle state","");
        //Deploying logic
        DeployingPhase = true;
        unitsToPlace = 1;
        Config.Battlefield.GetComponent<Battlefield>().generateEmptyFields = true;
        //TEST TEST TEST
        //Config.CreateRandomUnits(Config.PlayerHero.GetComponent<Hero>(),20,Race.Human);
        // Create and place enemy units
        FillEnemyBannerList();
        //Rebuild units, update supply and apply hero modifiers
        OnBattleStartUnitTriggers();
        // Draw deck space
        Config.DeckManager.SetActive(true);
        Config.DeckManager.GetComponent<Deck>().RebuildDeck();
        // Draw field
        Config.Battlefield.GetComponent<Battlefield>().RebuildField();
        Config.Battlefield.GetComponent<Battlefield>().UpdateField();
    }

    public void OnPlayerUnitDeployed()
    {
        unitsToPlace -= 1;
        if (unitsToPlace == 0)
        {
            EnemyUnitAllocation();
            unitsToPlace = 1;
        }
        //Config.Battlefield.GetComponent<Battlefield>().UpdateField();
    }

    private void OnBattleStartUnitTriggers()
    {
        var playerHero = Config.PlayerHero;
        var enemyHero = Config.EnemyHero;
        List<UnitBuff> playerBuffList = new List<UnitBuff>();
        List<UnitBuff> enemyBuffList = new List<UnitBuff>();
        foreach (var comp in Config.PlayerHero.GetComponent<Hero>().bannersList)
        {
           comp.GetComponent<ArmyUnitClass>().OnBattleStart(Config.WorldData.PlayerSupply,playerBuffList);
        }
        foreach (var comp in Config.EnemyHero.GetComponent<Hero>().bannersList)
        {
            comp.GetComponent<ArmyUnitClass>().OnBattleStart(Config.WorldData.EnemySupply,enemyBuffList,1);
        }
    }
    public override void OnExit()
    {
        StartBattleButton.SetActive(false);
        Config.Battlefield.GetComponent<Battlefield>().generateEmptyFields = false;
        // Clean deck space
        Config.DeckManager.GetComponent<Deck>().WipeCards();
    }
    public void StartBattle()
    {
        ChangeState<GameLoopRoundState>();
    }
    public void FillEnemyBannerList()
    {
        foreach (var unit in Config.EnemyHero.GetComponent<Hero>().bannersList)
        {
            Destroy(unit);
        }
        Config.EnemyHero.GetComponent<Hero>().bannersList.Clear();
        foreach (var unit in EnemyEventUnits)
        {
            Config.EnemyHero.GetComponent<Hero>().AddBannerList(unit);
        }
    }
    public void EnemyUnitAllocation()
    {
        Debug.Log("EnemyUnitAllocation");
        //TODO Make new logic here
    }
}
