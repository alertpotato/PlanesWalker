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
    public override void OnEnter()
    {
        Config.Battlefield.SetActive(true);
        StartBattleButton.SetActive(true);
        Config.InterfaceUI.UpdateHelpText("Pre battle state","");
        
        //TEST TEST TEST
        //Config.CreateRandomUnits(Config.PlayerHero.GetComponent<Hero>(),20,Race.Human);
        // Create and place enemy units
        FillEnemyArmy();
        //Rebuild units, update supply and apply hero modifiers
        OnBattleStartUnitTriggers();
        // Draw deck space
        Config.DeckManager.SetActive(true);
        Config.DeckManager.GetComponent<Deck>().RebuildDeck();
        // Draw field
        Config.Battlefield.GetComponent<Battlefield>().RebuildField(Config.PlayerFormation,Config.EnemyFormation);
        
        EnemyUnitAllocation();
        Config.Battlefield.GetComponent<Battlefield>().UpdateField();
    }
    private void OnBattleStartUnitTriggers()
    {
        var playerHero = Config.PlayerFormation.FieldOwner;
        var enemyHero = Config.EnemyFormation.FieldOwner;
        UnitCharacteristics playerBuffs = new UnitCharacteristics(0, 0, 0,playerHero.modinit , playerHero.modcoh, 0,0);
        UnitCharacteristics enemyBuffs = new UnitCharacteristics(0, 0, 0, enemyHero.modinit , enemyHero.modcoh, 0,0);
        UnitBuff playerBuff = new UnitBuff(playerBuffs, this.gameObject, 999);
        UnitBuff enemyBuff = new UnitBuff(enemyBuffs, this.gameObject, 999);
        List<UnitBuff> playerBuffList = new List<UnitBuff>();
        playerBuffList.Add(playerBuff);
        List<UnitBuff> enemyBuffList = new List<UnitBuff>();
        enemyBuffList.Add(enemyBuff);
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
        // Clean deck space
        Config.DeckManager.GetComponent<Deck>().WipeCards();
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
    public void FillEnemyArmy()
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
        var enemyUnits = new List<GameObject>();
        enemyUnits.AddRange(Config.EnemyHero.GetComponent<Hero>().bannersList);
        var rangedUnits = enemyUnits.Where(go => go.GetComponent<ArmyUnitClass>().UnitAbilityTags.Contains(AbilityTags.Ranged)).ToList();
        var avaliableSpaces = Config.EnemyFormation.GetAvaliableFields();
        var supportLine = avaliableSpaces.Where(comp => comp.Type == FormationType.Support).ToList();

        int safeIndex = 0;
        bool rangedToBackLine = true;
        while (rangedToBackLine == true)
        {
            if (supportLine.Count > 0 && rangedUnits.Count > 0)
            {
                var compMan =Config.Battlefield.GetComponent<Battlefield>().GetFieldByCompany(supportLine[0]);
                if (Config.Battlefield.GetComponent<Battlefield>().AddUnitToFormationLogic(rangedUnits[0],supportLine[0],compMan.GetComponent<OnFieldCompanyManager>(),Config.EnemyHero))
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
                var compMan =Config.Battlefield.GetComponent<Battlefield>().GetFieldByCompany(avaliableSpaces[0]);
                if (Config.Battlefield.GetComponent<Battlefield>().AddUnitToFormationLogic(enemyUnits[0],avaliableSpaces[0],compMan.GetComponent<OnFieldCompanyManager>(),Config.EnemyHero))
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
