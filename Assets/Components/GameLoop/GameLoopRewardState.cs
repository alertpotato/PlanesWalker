using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Internal.Execution;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(GameLoopSharedData))]
public class GameLoopRewardState : StateBehaviour
{
    [Header("Components")]
    public GameLoopSharedData Config;
    public GameObject SelectButton;
    public TextMeshProUGUI SupplyText;
    public GameObject SupplyButtons;
    [Header("Reward logic")]
    public int NumberOfRewards = 3;
    public int NumberOfCardsToChoose = 5;
    public int leftNumberOfRewards;
    
    [Header("Private variables")]
    [SerializeField] private bool isChosing;
    [SerializeField] private List<GameObject> RewardList;
    [SerializeField] private List<GameObject> CardList;
    
    public override void OnEnter()
    {
        leftNumberOfRewards = NumberOfRewards;
        isChosing = false;
        SelectButton.SetActive(true);
        Config.UpdateHelpText("Chosing rewards","Choose cards that would be added to your collection");
        //TODO Make proper supply ui manager
        string newVal = "";
        for (int i = 0; i < 4; i++)
        {
            newVal += Config.WorldData.PlayerSupply[i];
        }
        SupplyText.text = newVal;
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
            var newUnit = Config.InstantiateRandomUnit(Race.Human);
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
            var pos = Camera.main.ScreenToWorldPoint(new Vector3((Screen.width / (rewards + 1)) * (i + 1), Screen.height/2, 8));
            GameObject newCard = Instantiate(Config.UnitCard);
            newCard.name = $"{unit.name}_Card";
            newCard.transform.SetParent(unit.transform);
            newCard.GetComponent<UnitCardMain>().SetUnitParameters(unit, pos,Vector3.one*1.1f,true);
            CardList.Add(newCard);
            i += 1;
        }
        i = 0;
        foreach (GameObject unit in RewardList)
        {
            var pos = Camera.main.ScreenToWorldPoint(new Vector3((Screen.width / (rewards + 1)) * (i + 1),
                Screen.height / 2, 8));
            unit.transform.position = pos;
            i += 1;
        }
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
            Config.UpdateHelpText("Choose 1 supply","<color=\"yellow\">Each unit requires a supply to be able to participate in battle. Keep in mind that each multiple combination of supply increases the number of units in the squad by the same amount.</color>");
            //ChangeState<GameLoopPreBattleState>();
        }
    }

    public void GetSupply(int value)
    {
        Config.WorldData.AddSupply(value, 1);
        //TODO Make proper supply ui manager
        string newVal = "";
        for (int i = 0; i < 4; i++)
        {
            newVal += Config.WorldData.PlayerSupply[i];
        }
        SupplyText.text = newVal;
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

