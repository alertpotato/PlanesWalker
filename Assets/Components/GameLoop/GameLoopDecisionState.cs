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
    public List<int> EventIds = new List<int>();
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
        Config.InterfaceUI.UpdateHelpText("Decision state","Choose your path wisely");
    }

    public void MakeDecision(int value)
    {
        //Saving reward set in REWARD STATE
        Config.RewardState.Rewards = Rewards[value];
        //Saving unit set in PRE BATTLE STATE
        Config.PreBattleState.EnemyEventUnits.Clear();
        Config.PreBattleState.EnemyEventUnits.AddRange(Config.difficultyManager.Units[value]);
        //TODO cant transfer to PRE BATTLE for some reason
        ChangeState<GameLoopPreBattleState>();
    }

    private void GeneratingDecision()
    {
        Rewards.Add(Config.RewardFactory.GenerateRewardSet(2));
        Rewards.Add(Config.RewardFactory.GenerateRewardSet(2));
        EventIds = Config.EventFactory.GenerateEventSet(2);
        Config.difficultyManager.GenerateUnits(EventIds,0);
        if (Random.Range(0, 3) == 0)
        {
            Rewards.Add(Config.RewardFactory.GenerateRewardSet(3));
            EventIds.AddRange(Config.EventFactory.GenerateEventSet(1));
            Config.difficultyManager.GenerateUnits(EventIds,1);
        }
    }

    private void UpdateDecisionUI()
    {
        int decisionIndex = 0;
        foreach (var reward in Rewards)
        {
            string eventText = $"<style=\"Title\">{Config.EventFactory.EventWeights[EventIds[decisionIndex]].Name}</style>\nYour scouts spotted:\n";
            foreach (var unitList in Config.difficultyManager.Units[decisionIndex])
            {
                eventText+=$"    -{unitList.GetComponent<ArmyUnitClass>().squadName}\n";
            }
            string rewardsText = "\nRewards:\n";
            foreach (var rew in reward.Item1)
            {
                //Debug.Log($"{reward.Item2[rew.Item1].Name} {rew.Item2}");
                rewardsText+=$"    {GenerateRewardText(reward.Item2[rew.Item1].Name,rew.Item2)}\n";
            }
            var newDecisionUI = Instantiate(DecisionBox, DecisionUI.transform);
            newDecisionUI.GetComponent<DecisionWindow>().UpdateText(eventText+rewardsText);
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

