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
    public override void OnEnter()
    {
        Config.Battlefield.SetActive(true);
        StartBattleButton.SetActive(true);
        Config.InterfaceUI.UpdateHelpText("Pre battle state","Left click on card you want to pick, then left click again on the field. Right click on any field piece to reset.");
        // Update unit supply
        UpdateUnitSupplies();
        // Draw deck space
        Config.ArmyDeck.GetComponent<ArmyDeck>().ShowCards();
        // Init field
        var newField = new Dictionary<FormationType, int> { {FormationType.Frontline,3}, {FormationType.Support,1}, {FormationType.Flank1,1}, {FormationType.Flank2,1}};
        Config.PlayerFormation.RebuildField(newField);
        Config.EnemyFormation.RebuildField(newField);
        // Draw field
        Config.Battlefield.GetComponent<Battlefield>().RebuildField(Config.PlayerFormation,Config.EnemyFormation);
        Config.Battlefield.GetComponent<Battlefield>().UpdateField();
        // Create and place enemy units
        Config.CreateRandomUnits(Config.EnemyHero.GetComponent<Hero>(),4,Race.Goblin);
        EnemyUnitAllocation();
        Config.Battlefield.GetComponent<Battlefield>().UpdateField();
    }

    private void UpdateUnitSupplies()
    {
        List<GameObject> unitList = new List<GameObject>();
        unitList.AddRange(Config.EnemyHero.GetComponent<Hero>().bannersList);
        unitList.AddRange(Config.PlayerHero.GetComponent<Hero>().bannersList);
        foreach (var unit in unitList)
        {
            unit.GetComponent<ArmyUnitClass>().UpdateSupply(Config.WorldData.PlayerSupply);
        }
    }

    public override void OnExit()
    {
        StartBattleButton.SetActive(false);
        // Clean deck space
        Config.ArmyDeck.GetComponent<ArmyDeck>().WipeCards();
    }
    public override void OnUpdate()
    {
        //ChangeState<GameLoopRewardState>();
    }
    public void StartBattle()
    {
        ChangeState<GameLoopRoundState>();
    }
    /*void OnShowarmy(InputValue value)
    {
        if (!IsActive()) return;
        Debug.Log("Hello Q");
        Config.ArmyDeck.GetComponent<ArmyDeck>().UpdateDeck();
    }*/
    public void EnemyUnitAllocation()
    {
        var enemyUnits = Config.EnemyHero.GetComponent<Hero>().bannersList;
        foreach (var unit in enemyUnits)
        {
            unit.GetComponent<ArmyUnitClass>().UpdateSupply(Config.WorldData.PlayerSupply);
        }
        var rangedUnits = enemyUnits.Where(go => go.GetComponent<ArmyUnitClass>().UnitAbilityTags.Contains(AbilityTags.Ranged)).ToList();
        var avaliableSpaces = Config.EnemyFormation.GetAvaliableFields();
        var supportLine = avaliableSpaces.Where(comp => comp.Type == FormationType.Support).ToList();

        int safeIndex = 0;
        bool rangedToBackLine = true;
        while (rangedToBackLine == true)
        {
            if (supportLine.Count > 0 && rangedUnits.Count > 0)
            {
                if (Config.EnemyFormation.AddUnitToFormation(supportLine[0], rangedUnits[0],
                        Config.PlayerFormation))
                {
                    enemyUnits.Remove(rangedUnits[0]);
                    rangedUnits.Remove(rangedUnits[0]);
                    avaliableSpaces.Remove(supportLine[0]);
                    supportLine.Remove(supportLine[0]);
                }
            }
            else rangedToBackLine = false;

            safeIndex++;
            if (safeIndex>99) rangedToBackLine = false;
        }

        safeIndex = 0;
        bool otherPlaces = true;
        while (otherPlaces == true)
        {
            if (avaliableSpaces.Count > 0 && enemyUnits.Count > 0)
            {
                if (Config.EnemyFormation.AddUnitToFormation(avaliableSpaces[0], enemyUnits[0],
                        Config.PlayerFormation))
                {
                    enemyUnits.Remove(enemyUnits[0]);
                    avaliableSpaces.Remove(avaliableSpaces[0]);
                }
            }
            else otherPlaces = false;
            safeIndex++;
            if (safeIndex>99) otherPlaces = false;
        }
    }
}
