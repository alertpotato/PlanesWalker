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
    public GameObject DecisionUI;
    public GameObject DecisionBox;
    public List<(List<(int,int)>,List<RewardWeightsAndAttributes>)> Rewards = new List<(List<(int,int)>,List<RewardWeightsAndAttributes>)>();
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
        Config.RewardState.Rewards = Rewards[value];
        ChangeState<GameLoopPreBattleState>();
    }

    private void GeneratingDecision()
    {
        Rewards.Add(Config.RewardFactory.GenerateRewardSet(2));
        Rewards.Add(Config.RewardFactory.GenerateRewardSet(2));
    }

    private void UpdateDecisionUI()
    {
        int decisionIndex = 0;
        foreach (var reward in Rewards)
        {
            string rewardsText = "Rewards:\n";
            foreach (var rew in reward.Item1)
            {
                //Debug.Log($"{reward.Item2[rew.Item1].Name} {rew.Item2}");
                rewardsText+=$"*{GenerateRewardText(reward.Item2[rew.Item1].Name,rew.Item2)}\n";
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

    private string GenerateRewardText(string rewardName, int value)
    {
        string rewardText = "error";
        if (rewardName == "Unit") rewardText="New Unit card";
        else if (rewardName == "Upgrade") rewardText="Unit card Upgrade";
        else if (rewardName == "Supply")
        {
            if (value==0) rewardText = "+1 Food supply";
            else if (value==1) rewardText = "+1 Weapon supply";
            else if (value==2) rewardText = "+1 Gold supply";
        }
        else if (rewardName == "HeroUpgrade")
        {
            if (value==0) rewardText="Hero +1 initiative";
            else if (value==1) rewardText="Hero +1 cohesion";
        }
        else if (rewardName == "FieldUpgrade")
        {
            if (value==0) rewardText = "+ Frontline field";
            else if (value==1) rewardText = "+ Right flank field";
            else if (value==2) rewardText = "+ Left flank field";
            else if (value==3) rewardText = "+ Support field";
            else if (value==4) rewardText = "+ Reserve field";
        }
        return rewardText;
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

