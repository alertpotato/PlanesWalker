using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

[System.Serializable]
public struct RewardWeightsAndAttributes
{
    [Tooltip("Reward name")]public string Name;
    [Tooltip("Amount of points to subtracted from Weight if reward is chosen")]public int Cost;
    [Tooltip("How much Weight added after any event completed")]public int Regen;
    [Tooltip("Weight of reward")]public int Weight;
    public RewardWeightsAndAttributes( string name,int cost, int regen, int rewardWeight)
    {
        Name = name;Cost=cost;Regen=regen;Weight=rewardWeight;
    }
}
[CreateAssetMenu]
public class RewardFactory : ScriptableObject
{
    [Tooltip("List of possible rewards and their attributes")]
    public List<RewardWeightsAndAttributes> rewardsWeightsAndAttributes = new List<RewardWeightsAndAttributes>();
    public PlayerData PlayerData;
    public FormationField PlayerField;
    
    public (List<(int,int)>,List<RewardWeightsAndAttributes>) GenerateRewardSet(int numberOfRewards)
    {
        //Copy current rewards to work only with that copy
        List<RewardWeightsAndAttributes> rewardsList = new List<RewardWeightsAndAttributes>();
        foreach (var reward in rewardsWeightsAndAttributes)
        {
            rewardsList.Add(reward);
        }
        //Regenerate rewards until number of possible rewards >= number of requested rewards
        while (rewardsList.Count(item => item.Weight > 0) < numberOfRewards)
        {
            RegenerateRewards(rewardsList);
        }
        //Generate index list of picked rewards
        List<(int,int)> rewardsIds = new List<(int,int)>();
        for (int r = 0; r < numberOfRewards; r++)
        {
            var localListOfWeights = new int[rewardsList.Count()];
            for (int i = 0; i < localListOfWeights.Length; i++)
                { localListOfWeights[i] = rewardsList[i].Weight; }
            int indexOfSelectedReward = WeightFunctions.GetRandomWeightedIndex(localListOfWeights);
            SelectReward(rewardsList, indexOfSelectedReward);
            int indexOfRewardValue = CalculateReward(rewardsList[indexOfSelectedReward].Name);
            rewardsIds.Add((indexOfSelectedReward,indexOfRewardValue));
        }
        return (rewardsIds, rewardsList);
    }

    private void SelectReward(List<RewardWeightsAndAttributes> rewardsList, int indexOfSelectedReward)
    {
        var selectedReward = rewardsList[indexOfSelectedReward];
        selectedReward.Weight -= selectedReward.Cost;
        rewardsList[indexOfSelectedReward] = selectedReward;
    }

    private void RegenerateRewards(List<RewardWeightsAndAttributes> rewardsToRegen)
    {
        for (int i = 0; i < rewardsToRegen.Count; i++)
        {
            var updatedReward = rewardsToRegen[i];
            updatedReward.Weight += updatedReward.Regen;
            rewardsToRegen[i] = updatedReward;
        }
    }

    private int CalculateReward(String rewardName)
    {
        int reward = 0;
        if (rewardName == "Supply") reward = SupplyReward();
        else if (rewardName == "HeroUpgrade") reward = HeroReward();
        else if (rewardName == "FieldUpgrade") reward = FieldReward();
        return reward;
    }

    private int SupplyReward()
    {
        //Hardcoded for 3 supplies now - food, weapon, money; 
        var localListOfWeights = new int[3];
        //Weight of supply is {sum of all supplies-supply}
        for (int i = 0; i < 3; i++)
        { localListOfWeights[i] = Mathf.Clamp(PlayerData.PlayerSupply.Sum() - PlayerData.PlayerSupply[i],1,999); }
        int indexOfSelectedReward = WeightFunctions.GetRandomWeightedIndex(localListOfWeights);
        return indexOfSelectedReward;
    }
    private int HeroReward()
    {
        int reward = Random.Range(0, 1);
        return reward;
    }
    private int FieldReward()
    {
        var localListOfWeights = new int[5];
        // front - flank1 - flank2 - support - reserve
        var fieldsNum = PlayerField.Formation.Count();
        var front = PlayerField.Formation.Count(comp => comp.Type == FormationType.Frontline );
        var flank1 = PlayerField.Formation.Count(comp => comp.Type == FormationType.Flank1 );
        var flank2 = PlayerField.Formation.Count(comp => comp.Type == FormationType.Flank2 );
        var support = PlayerField.Formation.Count(comp => comp.Type == FormationType.Support );
        var reserve = PlayerField.Formation.Count(comp => comp.Type == FormationType.Reserve );
        localListOfWeights[0] = fieldsNum - front * 2;
        localListOfWeights[1] = fieldsNum - flank1 * 2;
        localListOfWeights[2] = fieldsNum - flank2 * 2;
        localListOfWeights[3] = fieldsNum - support * 2;
        localListOfWeights[4] = fieldsNum - reserve * 3;
        int indexOfSelectedReward = WeightFunctions.GetRandomWeightedIndex(localListOfWeights);
        return indexOfSelectedReward;
    }

    private void UnitReward()
    {
        
    }

    private void UnitUpgradeReward()
    {
        
    }

    private void OnValidate()
    {
        rewardsWeightsAndAttributes = new List<RewardWeightsAndAttributes>();
        FillListOfRewards();
    }

    private void FillListOfRewards()
    {
        rewardsWeightsAndAttributes.Clear();
        var SupplyReward = new RewardWeightsAndAttributes("Supply",2,1,1);
        rewardsWeightsAndAttributes.Add(SupplyReward);
        
        var UnitReward = new RewardWeightsAndAttributes("Unit",2,2,3);
        rewardsWeightsAndAttributes.Add(UnitReward);
        
        var UpgradeReward = new RewardWeightsAndAttributes("Upgrade",1,1,1);
        rewardsWeightsAndAttributes.Add(UpgradeReward);
        
        var HeroUpgradeReward = new RewardWeightsAndAttributes("HeroUpgrade",3,1,-2);
        rewardsWeightsAndAttributes.Add(HeroUpgradeReward);
        
        var FieldUpgradeReward = new RewardWeightsAndAttributes("FieldUpgrade",2,1,1);
        rewardsWeightsAndAttributes.Add(FieldUpgradeReward);
    }
}
