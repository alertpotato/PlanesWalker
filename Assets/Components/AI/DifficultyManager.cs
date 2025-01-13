using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    [SerializeField]private int difficultyLevel;
    PlayerData PlayerData;
    RewardFactory RewardFactory;
    ListOfCommonUnits UnitsFactory;
    [SerializeField]private int RarityMultiplier = 100;

    public void InitializeDifficulty(int startingDifficulty, PlayerData playerData, RewardFactory rewardFactory,ListOfCommonUnits unitsFactory)
    {
        difficultyLevel = startingDifficulty;
        PlayerData = playerData;
        RewardFactory = rewardFactory;
        UnitsFactory = unitsFactory;
    }

    public void UpdateDifficulty(int gameDay)
    {
        //+1 level every 3 days
        if (gameDay % 3 == 0) difficultyLevel++;
        //+1 random supply every 2 days
        if (gameDay % 2 == 0) PlayerData.AddEnemySupply(Random.Range(0, 1), 1);
        //Increasing rarity of all new cards
        IncreaseCardRarity(RarityMultiplier);
    }

    public void IncreaseCardRarity(int increaseMulti)
    {
        List<PointsToRandomuzeUnitWeights> newList = new List<PointsToRandomuzeUnitWeights>();
        var pointsList = UnitsFactory.pointsRandomizerList;
        int maxW=0;
        int minW=999999;
        foreach (var point in pointsList)
        {
            if (maxW < point.weight) maxW = point.weight;
            if (minW > point.weight) minW = point.weight;
        }
        int avgW = (maxW + minW)/2;
        Debug.Log($"{maxW} {minW} {avgW}");
        foreach (var point in pointsList)
        {
            float newW = point.weight;
            float multiPower = Mathf.Abs((float)(point.weight - avgW) / avgW);
            if (point.weight >=avgW) newW = point.weight - Mathf.Clamp(multiPower * increaseMulti,1,increaseMulti);
            else if (point.weight <avgW) newW = point.weight + Mathf.Clamp(multiPower * increaseMulti * Mathf.Clamp(point.weight/increaseMulti,0,1) ,1,increaseMulti);
            string logger = $"{point.weight} {multiPower} {newW}";
            Debug.Log(logger);
            newList.Add(new PointsToRandomuzeUnitWeights(point.points,Mathf.FloorToInt(newW)));
        }
        UnitsFactory.pointsRandomizerList = newList;
    }
}
