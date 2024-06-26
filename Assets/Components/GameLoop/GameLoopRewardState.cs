using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(GameLoopSharedData))]
public class GameLoopRewardState : StateBehaviour
{
    public GameLoopSharedData Config;
    public int NumberOfRewards = 3;
    public int NumberOfChoices = 5;
    public int leftNumberOfRewards;
    [SerializeField] private bool isChosing;
    [SerializeField] private List<GameObject> RewardList;
    [SerializeField] private List<GameObject> CardList;
    public override void OnEnter()
    {
        leftNumberOfRewards = NumberOfRewards;
        isChosing = false;
    }
    private void CreateUnits()
    {
        RewardList.Clear();
        for (int i=0; i<NumberOfChoices; i++)
        {
            var newUnit = Config.InstantiateRandomUnit(Race.Human);
            RewardList.Add(newUnit);
        }
        CreateCards(RewardList.Count);
    }
    private void CreateCards(int rewards)
    {
        for (int i = 0; i < rewards; i++)
        {
            var currUnit = RewardList[i];
            var pos = Camera.main.ScreenToWorldPoint(new Vector3((Screen.width / (rewards + 2)) * (i + 1), Screen.height/2, 8));
            GameObject newCard = Instantiate(Config.UnitCard);
            newCard.name = $"{currUnit.name}_Card";
            newCard.transform.SetParent(currUnit.transform);
            newCard.GetComponent<UnitCardMain>().SetUnitParameters(currUnit, pos,false);
            CardList.Add(newCard);
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
            ChangeState<GameLoopPreBattleState>();
        }

        for (int i = 0; i < CardList.Count; i++)
        {
            var pos = Camera.main.ScreenToWorldPoint(new Vector3((Screen.width / (CardList.Count + 2)) * (i + 1),
                Screen.height / 2, 8));
            CardList[i].transform.position = pos;
        }
    }

    void OnClick(InputValue value)
    {
        Ray ray = Config.MainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, Config.UnitLayer))
        {
            Config.SelectedUnits.SelectEntity(hit.collider.gameObject);
        }
    }

    private void OnAlternateClick(InputValue value)
    {
        Config.SelectedUnits.DeSelectEntity();
    }
    public void AddRewardToHero()
    {
        Debug.Log("AddRewardToHero");
        GameObject rewardUnit = Config.SelectedUnits.SelectedEntity.GetComponent<UnitCardMain>().RelatedUnit;
        Config.YourHero.GetComponent<Hero>().AddBannerList(rewardUnit);
        rewardUnit.transform.SetParent(Config.YourHero.transform);
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

