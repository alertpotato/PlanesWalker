using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GameLoopSharedData))]
public class GameLoopDecisionState : StateBehaviour
{
    [Header("Components")]
    public GameLoopSharedData Config;
    public RewardFactory RewardFactory;
    public GameObject DecisionUI;
    public GameObject DecisionBox;
    public List<(List<int>,List<RewardWeightsAndAttributes>)> Rewards = new List<(List<int>,List<RewardWeightsAndAttributes>)>();
    public List<GameObject> DecisionWindows = new List<GameObject>();
    private void OnValidate()
    {
    }
    public override void OnEnter()
    {
        ClearUI();
        Rewards.Clear();
        DecisionUI.gameObject.SetActive(true);

        GeneratingDecision();
        UpdateDecisionUI();
    }

    public void MakeDecision(int value)
    {
        Debug.Log($"Making Decision: {value}");
        ChangeState<GameLoopRewardState>();
    }

    private void GeneratingDecision()
    {
        Rewards.Add(RewardFactory.GenerateRewardSet(2));
        Rewards.Add(RewardFactory.GenerateRewardSet(2));
        
    }

    private void UpdateDecisionUI()
    {
        int decisionIndex = 0;
        foreach (var reward in Rewards)
        {
            string rewardsText = "";
            foreach (var rew in reward.Item1)
            {
                rewardsText+=" Name:"+RewardFactory.rewardsWeightsAndAttributes[rew].Name;
            }
            var newDecisionUI = Instantiate(DecisionBox, DecisionUI.transform);
            newDecisionUI.GetComponent<DecisionWindow>().UpdateText(rewardsText);
            Button b = newDecisionUI.GetComponent<Button>();
            int indexBuffer = decisionIndex;
            b.onClick.AddListener(() => MakeDecision(indexBuffer));
            DecisionWindows.Add(newDecisionUI);
            decisionIndex++;
        }
    }

    private void ClearUI()
    {
        foreach (var window in DecisionWindows)
        {
            Destroy(window);
        }
        DecisionWindows.Clear();
    }

    public override void OnExit()
    {
        ClearUI();
        DecisionUI.gameObject.SetActive(false);
    }
    
}

