using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[System.Serializable]
public struct UnitWeightsOfChars
{
    public string unitParam; public int cost; public int paramMod; public int paramWeight;
    public UnitWeightsOfChars( string uparam, int _cost, int mparam, int wparam)
    {
        unitParam=uparam;cost=_cost;paramMod=mparam;paramWeight=wparam;
    }
}

[CreateAssetMenu]
public class ListOfCommonUnits : ScriptableObject
{
    [System.Serializable]
    public struct PointsToRandomuzeUnitWeights
    {
        public int points; public int weight;
        public PointsToRandomuzeUnitWeights(int p, int w)
        { points = p;weight = w; }
    }
    [System.Serializable]
    public class serializableUnitClass
    {
        public string UnitType; 
        public UnitCharacteristics UnitCharacteristics;
        public List<UnitWeightsOfChars> CharacteristicsWeightsList;
        public int UnitWeight;
        public serializableUnitClass(string unitType,UnitCharacteristics uChar, List<UnitWeightsOfChars> wList,int unitWeight)
        {
            UnitType = unitType;
            UnitCharacteristics = uChar;
            CharacteristicsWeightsList = wList;
            UnitWeight = unitWeight;
        }
    }
    public List<PointsToRandomuzeUnitWeights> pointsRandomizerList;
    public List<serializableUnitClass> UnitGoodList = new List<serializableUnitClass>();
    public List<serializableUnitClass> UnitEvilList = new List<serializableUnitClass>();
    public GameObject UnitPrefab;
    public (UnitCharacteristics, CountUnitUpgrades) GetRandomUnit(string unitClass)
    {
        List<serializableUnitClass> localList;
        if (unitClass == "p") { localList = UnitGoodList; Debug.Log("Using plebListOfUnitBaseCharacteristics"); }
        else if (unitClass == "e")
        {
            localList = UnitEvilList;
            Debug.Log("Using evilListOfUnitBaseCharacteristics");
        }
        else
        {
            Debug.LogError("Unsupported unit type!!!");
            localList = new List<serializableUnitClass>();
        }

        var localUnitWeights = new int[localList.Count];
        for (int i = 0; i < localList.Count; i++) { localUnitWeights[i] = localList[i].UnitWeight;}
        int indexOfSelectedUnit = GetRandomWeightedIndex(localUnitWeights);

        var localPointsToRandomize = new int[pointsRandomizerList.Count];
        for (int i = 0; i < pointsRandomizerList.Count; i++) { localPointsToRandomize[i] = pointsRandomizerList[i].weight; }
        int indexOfNumberOfPoints = GetRandomWeightedIndex(localPointsToRandomize);

        CountUnitUpgrades unitUpgrades = new CountUnitUpgrades();
        GameObject NewUnit = Instantiate(UnitPrefab);
        (UnitCharacteristics,CountUnitUpgrades) newUnit = GetUnitWithRandomizedStats(localList[indexOfSelectedUnit], pointsRandomizerList[indexOfNumberOfPoints].points,
            unitUpgrades);
        NewUnit.GetComponent<ArmyUnitClass>().UpdateUnitCharacteristics(newUnit.Item1,newUnit.Item2);
        return (newUnit.Item1,newUnit.Item2);
    }

    public (UnitCharacteristics,CountUnitUpgrades) GetUnitWithRandomizedStats(serializableUnitClass unit,int numberOfPointsToRandomize, CountUnitUpgrades _unitUpgrades)
    {
        var localListOfStatWeights = new int[unit.CharacteristicsWeightsList.Count];
        for (int i = 0; i < unit.CharacteristicsWeightsList.Count; i++) { localListOfStatWeights[i] = unit.CharacteristicsWeightsList[i].paramWeight; }

        while (numberOfPointsToRandomize > 0)
        {
            int indexOfStat = GetRandomWeightedIndex(localListOfStatWeights);
            if (numberOfPointsToRandomize - unit.CharacteristicsWeightsList[indexOfStat].cost >= 0)
            {
                numberOfPointsToRandomize -= unit.CharacteristicsWeightsList[indexOfStat].cost;
                if (indexOfStat == 0) { unit.UnitCharacteristics.ucnumberofunits += unit.CharacteristicsWeightsList[indexOfStat].paramMod; _unitUpgrades.cnumber += 1; }
                if (indexOfStat == 1) { unit.UnitCharacteristics.ucunithealth += unit.CharacteristicsWeightsList[indexOfStat].paramMod; _unitUpgrades.chealth += 1; }
                if (indexOfStat == 2) { unit.UnitCharacteristics.ucunitdamage += unit.CharacteristicsWeightsList[indexOfStat].paramMod; _unitUpgrades.cdamage += 1; }
                if (indexOfStat == 3) { unit.UnitCharacteristics.ucunitinitiative += unit.CharacteristicsWeightsList[indexOfStat].paramMod; _unitUpgrades.cinitiative += 1; }
                if (indexOfStat == 4) { unit.UnitCharacteristics.ucunitcohesion += unit.CharacteristicsWeightsList[indexOfStat].paramMod; _unitUpgrades.ccohesion += 1; }
                if (indexOfStat == 5) { unit.UnitCharacteristics.unitarmour += unit.CharacteristicsWeightsList[indexOfStat].paramMod; _unitUpgrades.carmour += 1; }
                localListOfStatWeights[indexOfStat] = Mathf.Abs(localListOfStatWeights[indexOfStat]/5);
            }
        }
        return (unit.UnitCharacteristics,_unitUpgrades);
    }

    public int GetRandomWeightedIndex(int[] weights)
    {
        if (weights == null || weights.Length == 0) return -1;

        int weightSum = 0;
        int i;
        for (i = 0; i < weights.Length; i++)
        {
            if (weights[i] >= 0) weightSum += weights[i];
        }

        float r = Random.value;
        float s = 0f;

        for (i = 0; i < weights.Length; i++)
        {
            if (weights[i] <= 0f) continue;

            s += (float)weights[i] / weightSum;
            if (s >= r) return i;
        }

        return -1;
    }
    private void OnEnable()
    {
        //FillListOfUnitChars();
        //RefillList();
    }
    private void FillListOfUnitChars()
    {
        UnitGoodList.Clear();
        UnitEvilList.Clear();
        var p_lmilita_b = new UnitCharacteristics(14, 3, 3, -1, -1, 0);
        List<UnitWeightsOfChars> p_militia_stats = new List<UnitWeightsOfChars>
        {
            new UnitWeightsOfChars("number", 1, 5, 12),
            new UnitWeightsOfChars("health", 1, 1, 10),
            new UnitWeightsOfChars("damage", 2, 1, 8),
            new UnitWeightsOfChars("init", 1, 1, 5),
            new UnitWeightsOfChars("coh", 1, 1, 5),
            new UnitWeightsOfChars("armour", 5, 1, 2)
        };
        UnitGoodList.Add(new serializableUnitClass("local_militia",p_lmilita_b,p_militia_stats,10) );

        var p_dthieves_b = new UnitCharacteristics(7, 5, 6, 0, -2, 0);
        List<UnitWeightsOfChars> p_dthieves_b_stats = new List<UnitWeightsOfChars>
        {
            new UnitWeightsOfChars("number", 2, 4, 7),
            new UnitWeightsOfChars("health", 1, 1, 10),
            new UnitWeightsOfChars("damage", 1, 2, 10),
            new UnitWeightsOfChars("init", 1, 1, 12),
            new UnitWeightsOfChars("coh", 1, 1, 5),
            new UnitWeightsOfChars("armour", 5, 1, 2)
        };
        UnitGoodList.Add(new serializableUnitClass("dirty_thieves",p_dthieves_b,p_dthieves_b_stats,8) );

        var p_twarriors_b = new UnitCharacteristics(5, 8, 5, 0, 2, 0);
        List<UnitWeightsOfChars> p_twarriors_b_stats = new List<UnitWeightsOfChars>
        {
            new UnitWeightsOfChars("number", 1, 2, 7),
            new UnitWeightsOfChars("health", 1, 2, 12),
            new UnitWeightsOfChars("damage", 2, 2, 10),
            new UnitWeightsOfChars("init", 1, 1, 9),
            new UnitWeightsOfChars("coh", 1, 2, 4),
            new UnitWeightsOfChars("armour", 3, 1, 4)
        };
        UnitGoodList.Add(new serializableUnitClass("trained_warriors",p_twarriors_b,p_twarriors_b_stats,6) );

        var e_ogre_b = new UnitCharacteristics(1, 40, 15, 0, -3, 2);
        List<UnitWeightsOfChars> p_ogre_b_stats = new List<UnitWeightsOfChars>
        {
            new UnitWeightsOfChars("number", 4, 1, 2),
            new UnitWeightsOfChars("health", 1, 15, 10),
            new UnitWeightsOfChars("damage", 1, 5, 10),
            new UnitWeightsOfChars("init", 1, 1, 7),
            new UnitWeightsOfChars("coh", 1, 2, 4),
            new UnitWeightsOfChars("armour", 2, 1, 4)
        };
        UnitEvilList.Add(new serializableUnitClass("ogre",e_ogre_b,p_ogre_b_stats,6) );

        var e_rgoblin_b = new UnitCharacteristics(12, 3, 3, 2, -1, 0);
        List<UnitWeightsOfChars> p_rgoblin_b_stats = new List<UnitWeightsOfChars>
        {
            new UnitWeightsOfChars("number", 2, 5, 10),
            new UnitWeightsOfChars("health", 1, 1, 8),
            new UnitWeightsOfChars("damage", 1, 1, 12),
            new UnitWeightsOfChars("init", 1, 1, 12),
            new UnitWeightsOfChars("coh", 1, 1, 8),
            new UnitWeightsOfChars("armour", 5, 1, 2)
        };
        UnitEvilList.Add(new serializableUnitClass("goblin_rogue",e_rgoblin_b,p_rgoblin_b_stats,8) );

        var e_pgoblin_b = new UnitCharacteristics(20, 3, 2, -1, -3, 0);
        List<UnitWeightsOfChars> p_pgoblin_b_stats = new List<UnitWeightsOfChars>
        {
            new UnitWeightsOfChars("number", 2, 7, 12),
            new UnitWeightsOfChars("health", 1, 1, 5),
            new UnitWeightsOfChars("damage", 2, 1, 8),
            new UnitWeightsOfChars("init", 1, 1, 12),
            new UnitWeightsOfChars("coh", 1, 1, 5),
            new UnitWeightsOfChars("armour", 6, 1, 2)
        };
        UnitEvilList.Add(new serializableUnitClass("goblin_punks",e_pgoblin_b,p_pgoblin_b_stats,10) );
    }
}
