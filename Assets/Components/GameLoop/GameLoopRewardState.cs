using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(GameLoopSharedData))]
public class GameLoopRewardState : StateBehaviour
{
    [Header("Components")]
    public GameLoopSharedData Config;
    public GameObject SelectButton;
    public GameObject SupplyButtons;
    [Header("Reward logic")]
    public int NumberOfRewards = 3;
    public int NumberOfCardsToChoose = 5;
    public int leftNumberOfRewards = 0;
    
    [Header("Private variables")]
    [SerializeField] private bool isChosing;
    [SerializeField] private List<GameObject> RewardList = new List<GameObject>();
    [SerializeField] private List<GameObject> CardList = new List<GameObject>();
    private void OnValidate()
    {
        SelectButton.GetComponent<SelectRewardButton>().SelectedEntity = Config.SelectedUnits;
    }
    public override void OnEnter()
    {
        leftNumberOfRewards = NumberOfRewards;
        isChosing = false;
        SelectButton.SetActive(true);
        Config.InterfaceUI.UpdateSupply(Config.WorldData.PlayerSupply);
        Config.InterfaceUI.UpdateHelpText("Chosing rewards","Choose cards that would be added to your collection");
    }
    public override void OnUpdate()
    {
        if (isChosing==false && leftNumberOfRewards > 0)
        {
            CreateUnits();
            isChosing = true;
            leftNumberOfRewards--;
        }

        if (isChosing==false & leftNumberOfRewards == 0)
        {
            SupplyButtons.SetActive(true);
            Config.InterfaceUI.UpdateHelpText("Choose 1 supply","<color=\"yellow\">Each unit requires a supply to be able to participate in battle. Keep in mind that each multiple combination of supply increases the number of units in the squad by the same amount.</color>");
            //ChangeState<GameLoopPreBattleState>();
        }
    }

    public override void OnExit()
    {
        SelectButton.SetActive(false);
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
            newCard.GetComponent<UnitCardMain>().SetUnitParameters(Config.MainCamera,unit, pos,Vector3.one*1.1f,true);
            CardList.Add(newCard);
            i += 1;
        }
    }
    

    public void GetSupply(int value)
    {
        Config.WorldData.AddSupply(value, 1);
        Config.InterfaceUI.UpdateSupply(Config.WorldData.PlayerSupply);
        SupplyButtons.SetActive(false);
        ChangeState<GameLoopPreBattleState>();
    }

    public void AddRewardToHero()
    {
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
        isChosing = false;
        CardList.Clear();
        RewardList.Clear();
    }
}

