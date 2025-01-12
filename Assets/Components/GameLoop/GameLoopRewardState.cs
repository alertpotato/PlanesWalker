using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GameLoopSharedData))]
public class GameLoopRewardState : StateBehaviour
{
    [Header("Components")]
    public GameLoopSharedData Config;
    public GameObject SelectButton;
    public GameObject SupplyButtons;
    [Header("Reward logic")]
    public int NumberOfCardsToChoose = 5;
    public (List<(int, int)>, List<RewardWeightsAndAttributes>) Rewards;
    public int RewardCounter = 0;
    [Header("Private variables")]
    [SerializeField] private List<GameObject> RewardList = new List<GameObject>();
    [SerializeField] private List<GameObject> CardList = new List<GameObject>();
    private void OnValidate()
    {
        SelectButton.GetComponent<SelectRewardButton>().SelectedEntity = Config.SelectedUnits;
    }
    public override void OnEnter()
    {
        RewardCounter = -1;
        SelectButton.SetActive(false);
        SelectButton.GetComponent<Button>().onClick.RemoveAllListeners();
        Config.InterfaceUI.UpdateSupply(Config.WorldData.PlayerSupply);
        Config.InterfaceUI.UpdateHelpText("Claim your rewards!","");
        NextReward();
    }

    public override void OnExit()
    {
        SelectButton.SetActive(false);
    }
    private void NextReward()
    {
        RewardCounter++;
        if (RewardCounter >= Rewards.Item1.Count) ChangeState<GameLoopPreBattleState>();
        else RewardEngine(Rewards.Item1[RewardCounter]);
    }

    private void RewardEngine((int,int) rewardIds)
    {
        if (rewardIds.Item1 == 0) GetSupplyReward(rewardIds.Item2);
        else if (rewardIds.Item1 == 1) GetUnitReward();
        else if (rewardIds.Item1 == 2) GetUpgradeReward();
        else if (rewardIds.Item1 == 3) GetHeroReward(rewardIds.Item2);
        else if (rewardIds.Item1 == 4) GetFieldReward(rewardIds.Item2);
        else if (rewardIds.Item1 == 99) StartingSupplyReward();
    }
    private void StartingSupplyReward()
    {
        SupplyButtons.SetActive(true);
    }

    public void GetStartingSupplyReward(int value)
    {
        SupplyButtons.SetActive(false);
        GetSupplyReward(value);
    }

    private void GetSupplyReward(int value)
    {
        Config.WorldData.AddSupply(value, 1);
        Config.InterfaceUI.UpdateSupply(Config.WorldData.PlayerSupply);
        NextReward();
        //Config.InterfaceUI.UpdateHelpText("Choose 1 additional supply","Each unit requires a supply to be able to participate in battle. Keep in mind that each multiple combination of supply increases the number of units in the squad by the same amount.");
    }
    private void GetUnitReward()
    {
        Config.InterfaceUI.UpdateHelpText("Claim your rewards!","Choose card that would be added to your collection");
        SelectButton.SetActive(true);
        SelectButton.GetComponent<Button>().onClick.AddListener(() => AddUnitToHero());
        CreateUnits();
    }
    private void GetUpgradeReward()
    {
        Config.InterfaceUI.UpdateHelpText("Claim your rewards!","Choose card that would upgraded");
        Config.DeckManager.GetComponent<Deck>().RebuildDeck();
        SelectButton.SetActive(true);
        SelectButton.GetComponent<Button>().onClick.AddListener(() => UpgradeCard());
    }
    private void GetHeroReward(int value)
    {
        Config.PlayerHero.GetComponent<Hero>().UpgradeHero(value, 1);
        NextReward();
    }
    private void GetFieldReward(int value)
    {
        FormationType newCompanyType;
        if (value == 1) newCompanyType = FormationType.Flank1;
        else if (value == 2) newCompanyType = FormationType.Flank2;
        else if (value == 3) newCompanyType = FormationType.Support;
        else if (value == 4) newCompanyType = FormationType.Reserve;
        else newCompanyType = FormationType.Frontline;
        Config.PlayerFormation.CreateNewCompany(newCompanyType);
        NextReward();
    }

    public void UpgradeCard()
    {
        if (!Config.SelectedUnits.IsEntitySelected()) return;
        GameObject unitToUpgrade = Config.SelectedUnits.SelectedEntity.GetComponent<UnitCardMain>().RelatedUnit;
        UnitUpgrades newUpgrades = Config.listOfCommonUnits.UpgradeUnit(unitToUpgrade.GetComponent<ArmyUnitClass>().FactoryCharacteristics, Config.listOfCommonUnits.GenerateUpgradePoints());
        unitToUpgrade.GetComponent<ArmyUnitClass>().UpgradeUnit(newUpgrades);
        SelectButton.GetComponent<Button>().onClick.RemoveAllListeners();
        NextReward();
    }

    public void AddUnitToHero()
    {
        if (!Config.SelectedUnits.IsEntitySelected()) return;
        GameObject rewardUnit = Config.SelectedUnits.SelectedEntity.GetComponent<UnitCardMain>().RelatedUnit;
        Config.PlayerHero.GetComponent<Hero>().AddBannerList(rewardUnit);
        rewardUnit.transform.SetParent(Config.PlayerHero.transform);
        RewardList.Remove(rewardUnit);
        foreach (var card in CardList)
        {
            Destroy(card);
        }
        foreach (var unit in RewardList)
        {
            Destroy(unit);
        }
        CardList.Clear();
        RewardList.Clear();
        Config.SelectedUnits.DeSelectEntity();
        SelectButton.GetComponent<Button>().onClick.RemoveAllListeners();
        NextReward();
    }
    
    private void CreateUnits()
    {
        RewardList.Clear();
        for (int i=0; i<NumberOfCardsToChoose; i++)
        {
            var newUnit = Config.InstantiateRandomUnit(Race.Human,Config.RewardParent);
            RewardList.Add(newUnit);
        }
        CreateCards();
    }
    private void CreateCards()
    {
        int i = 0;
        var rewards = RewardList.Count;
        foreach (GameObject unit in RewardList)
        {
            var pos = Config.MainCamera.ScreenToWorldPoint(new Vector3((Screen.width / (rewards + 1)) * (i + 1), Screen.height/2, 5)); //z = 5 bc its distance between cards and camera
            GameObject newCard = Instantiate(Config.UnitCard);
            newCard.name = $"{unit.name}_Card";
            newCard.transform.SetParent(unit.transform);
            newCard.GetComponent<UnitCardMain>().SetUnitParameters(Config.MainCamera,unit, pos,Vector3.one*1.1f,true,10);
            CardList.Add(newCard);
            i += 1;
        }
    }
}

