using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    [SerializeField]private int difficultyLevel;
    PlayerData PlayerData;
    RewardFactory RewardFactory;
    EventFactory EventFactory;
    UnitFactory unitFactory;
    public GameObject UnitPrefab;
    [SerializeField]private int RarityMultiplier = 100;
    public List<List<GameObject>> Units = new List<List<GameObject>>();
    public List<Dictionary<FormationType, int>> Fields = new List<Dictionary<FormationType, int>>();
    public void InitializeDifficulty(int startingDifficulty, PlayerData playerData, RewardFactory rewardFactory,EventFactory eventFactory,UnitFactory unitsFactory)
    {
        difficultyLevel = startingDifficulty;
        PlayerData = playerData;
        RewardFactory = rewardFactory;
        EventFactory = eventFactory;
        unitFactory = unitsFactory;
    }

    public void UpdateDifficulty(int gameDay)
    {
        //+1 level every 3 days
        if (gameDay % 3 == 0) difficultyLevel++;
        //+1 random supply every 2 days
        PlayerData.AddEnemySupply(RewardFactory.SupplyReward(PlayerData.EnemySupply), 1);
        //Increasing rarity of all new cards
        IncreaseCardRarity(RarityMultiplier);
    }

    public void IncreaseCardRarity(int increaseMulti)
    {
        List<PointsToRandomuzeUnitWeights> newList = new List<PointsToRandomuzeUnitWeights>();
        var pointsList = unitFactory.pointsRandomizerList;
        int maxW=0;
        int minW=999999;
        foreach (var point in pointsList)
        {
            if (maxW < point.weight) maxW = point.weight;
            if (minW > point.weight) minW = point.weight;
        }
        int avgW = (maxW + minW)/2;
        foreach (var point in pointsList)
        {
            float newW = point.weight;
            float multiPower = Mathf.Abs((float)(point.weight - avgW) / avgW);
            if (point.weight >=avgW) newW = point.weight - Mathf.Clamp(multiPower * increaseMulti,1,increaseMulti);
            else if (point.weight <avgW) newW = point.weight + Mathf.Clamp(multiPower * increaseMulti * Mathf.Clamp(point.weight/increaseMulti,0,1) ,1,increaseMulti);
            string logger = $"{point.weight} {multiPower} {newW}";
            newList.Add(new PointsToRandomuzeUnitWeights(point.points,Mathf.FloorToInt(newW)));
        }
        unitFactory.pointsRandomizerList = newList;
    }
    public void GenerateUnits(List<int> events,int additionalDifficulty)
    {
        ResetUnitSets();
        foreach (var gameevent in events)
        {
            var gameEvent = EventFactory.EventWeights[gameevent].Name;
            if (gameEvent == "Ogre camp") ogreCampEvent(additionalDifficulty);
            else if (gameEvent == "Caravan ambush") caravanAmbushEvent(additionalDifficulty);
            else if (gameEvent == "Goblin patrol") goblinPatrolEvent(additionalDifficulty);
            else if (gameEvent == "Sorcerer army") sorcererArmyEvent(additionalDifficulty);
        }
    }

    private void ogreCampEvent(int difficultyModifier)
    {
        //UNITS
        var ogreCount = 2;
        List<string> orgeTypes = new List<string> { "ogre" };
        var unitRaces = new List<Race> { Race.Goblin };
        var newUnits = new List<GameObject>();
        //Create 2 mandatory ogres
        for (int i = 0; i < ogreCount; i++)
        {
            var unit = InstantiateRandomUnit(unitRaces, this.gameObject, orgeTypes);
            newUnits.Add(unit);
        }

        //Additional unit based on difficulty
        int additionalRandomUnits = 1 + Random.Range(-1, 2) + difficultyLevel + difficultyModifier;
        for (int i = 0; i < additionalRandomUnits; i++)
        {
            var unit = InstantiateRandomUnit(unitRaces, this.gameObject);
            newUnits.Add(unit);
        }

        Units.Add(newUnits);
        //FORMATION
        Dictionary<FormationType, int> startingField = new Dictionary<FormationType, int> { {FormationType.Frontline,3}, {FormationType.Support,0}, {FormationType.Flank1,0},{FormationType.Flank2,0}};
        int numberOfAdditionalFields = newUnits.Count-2;
        for (int i = 0; i < numberOfAdditionalFields; i++)
        {
            int rnd = Random.Range(0, 10);
            if (rnd>-1 && rnd <=2) startingField[FormationType.Frontline] += 1;
            else if (rnd>2 && rnd <=5) startingField[FormationType.Support] += 1;
            else if (rnd>5 && rnd <=7) startingField[FormationType.Flank1] += 1;
            else if (rnd>7 && rnd <=9) startingField[FormationType.Flank2] += 1;
        }
        Fields.Add(startingField);
    }

    private void caravanAmbushEvent(int difficultyModifier)
    {
        var unitRaces = new List<Race>{Race.Human};
        var newUnits = new List<GameObject>();
        //Unit based on difficulty
        int randomUnits = 3 + Random.Range(-1,1) + difficultyLevel + difficultyModifier;
        for (int i = 0; i < randomUnits; i++)
        {
            var unit = InstantiateRandomUnit(unitRaces, this.gameObject);
            newUnits.Add(unit);
        }
        Units.Add(newUnits);
        //FORMATION
        Dictionary<FormationType, int> startingField = new Dictionary<FormationType, int> { {FormationType.Frontline,2}, {FormationType.Support,1}, {FormationType.Flank1,1},{FormationType.Flank2,0}};
        int numberOfAdditionalFields = newUnits.Count-2;
        for (int i = 0; i < numberOfAdditionalFields; i++)
        {
            int rnd = Random.Range(0, 10);
            if (rnd>-1 && rnd <=2) startingField[FormationType.Frontline] += 1;
            else if (rnd>2 && rnd <=5) startingField[FormationType.Support] += 1;
            else if (rnd>5 && rnd <=7) startingField[FormationType.Flank1] += 1;
            else if (rnd>7 && rnd <=9) startingField[FormationType.Flank2] += 1;
        }
        Fields.Add(startingField);
    }
    private void goblinPatrolEvent(int difficultyModifier)
    {
        var goblinCount = 2;
        List<string> goblinTypes = new List<string>{"goblin_skiermisher"};
        var unitRaces = new List<Race>{Race.Goblin};
        var newUnits = new List<GameObject>();
        //Create 2 mandatory goblins
        for (int i = 0; i < goblinCount; i++)
        {
            var unit = InstantiateRandomUnit(unitRaces, this.gameObject,goblinTypes);
            newUnits.Add(unit);
        }
        //Additional unit based on difficulty
        int additionalRandomUnits = 1 + Random.Range(-1,1) + difficultyLevel + difficultyModifier;
        for (int i = 0; i < additionalRandomUnits; i++)
        {
            var unit = InstantiateRandomUnit(unitRaces, this.gameObject);
            newUnits.Add(unit);
        }
        Units.Add(newUnits);
        //FORMATION
        Dictionary<FormationType, int> startingField = new Dictionary<FormationType, int> { {FormationType.Frontline,2}, {FormationType.Support,2}, {FormationType.Flank1,0},{FormationType.Flank2,0}};
        int numberOfAdditionalFields = newUnits.Count-1;
        for (int i = 0; i < numberOfAdditionalFields; i++)
        {
            int rnd = Random.Range(0, 10);
            if (rnd>-1 && rnd <=2) startingField[FormationType.Frontline] += 1;
            else if (rnd>2 && rnd <=5) startingField[FormationType.Support] += 1;
            else if (rnd>5 && rnd <=7) startingField[FormationType.Flank1] += 1;
            else if (rnd>7 && rnd <=9) startingField[FormationType.Flank2] += 1;
        }
        Fields.Add(startingField);
    }
    private void sorcererArmyEvent(int difficultyModifier)
    {
        var unitRaces = new List<Race>{Race.Human,Race.Goblin};
        var newUnits = new List<GameObject>();
        //Unit based on difficulty
        int randomUnits = 4 + Random.Range(-1,1) + difficultyLevel + difficultyModifier;
        for (int i = 0; i < randomUnits; i++)
        {
            var unit = InstantiateRandomUnit(unitRaces, this.gameObject);
            newUnits.Add(unit);
        }
        Units.Add(newUnits);
        //FORMATION
        Dictionary<FormationType, int> startingField = new Dictionary<FormationType, int> { {FormationType.Frontline,2}, {FormationType.Support,1}, {FormationType.Flank1,1},{FormationType.Flank2,1}};
        int numberOfAdditionalFields = newUnits.Count-2;
        for (int i = 0; i < numberOfAdditionalFields; i++)
        {
            int rnd = Random.Range(0, 10);
            if (rnd>-1 && rnd <=2) startingField[FormationType.Frontline] += 1;
            else if (rnd>2 && rnd <=5) startingField[FormationType.Support] += 1;
            else if (rnd>5 && rnd <=7) startingField[FormationType.Flank1] += 1;
            else if (rnd>7 && rnd <=9) startingField[FormationType.Flank2] += 1;
        }
        Fields.Add(startingField);
    }
    
    public GameObject InstantiateRandomUnit(List<Race> unitRace,GameObject parent,List<string> unitType=null)
    {
        var newUnit = unitFactory.GetRandomUnit();
        GameObject newUnitObject = Instantiate(UnitPrefab,parent.transform);
        ArmyUnitClass unitClass = newUnitObject.GetComponent<ArmyUnitClass>();
        unitClass.InitializeUnit(newUnit);
        newUnitObject.name = $"{unitClass.squadName}{newUnitObject.GetInstanceID()}";
        newUnitObject.transform.position = Vector3.zero;
        return newUnitObject;
    }
    private void ResetUnitSets()
    {
        foreach (var list in Units)
        {
            foreach (var unit in list)
            {
                Destroy(unit);
            }
        }
        Units.Clear();
        Fields.Clear();
    }
}
