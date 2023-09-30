using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
[System.Serializable] 
public enum Race { Human, Goblin };
[System.Serializable]
public struct UnitWeightsOfChars
{
    [Tooltip("Amount in upgrade points needed to get characteristic modifier")]public int Cost;
    [Tooltip("For how much related characteristic will be upgraded for")]public int Modifier;
    [Tooltip("Rarity of modifier")]public int Weight;
    public UnitWeightsOfChars( int cost, int modifier, int characteristicWeight)
    {
        Cost=cost;Modifier=modifier;Weight=characteristicWeight;
    }
}

[System.Serializable]
public class BaseUnitCharacteristics
{
    public string UnitType;
    public Race UnitRace;
    public UnitCharacteristics Characteristics;
    //public Abilities UnitAbilities = new Abilities();
    public List<Abilities> UnitAbilities = new List<Abilities>();
    public UnitWeightsOfChars NumberOfUnitsUpgrade;
    public UnitWeightsOfChars HealthUpgrade;
    public UnitWeightsOfChars DamageUpgrade;
    public UnitWeightsOfChars InitiativeUpgrade;
    public UnitWeightsOfChars CohesionUpgrade;
    public UnitWeightsOfChars ArmourUpgrade;
    public int UnitWeight;
    public BaseUnitCharacteristics(string unitType,Race unitRace,UnitCharacteristics uChar, int unitWeight,UnitWeightsOfChars numberOfUnitsUpgrade,UnitWeightsOfChars healthUpgrade,UnitWeightsOfChars damageUpgrade,UnitWeightsOfChars initiativeUpgrade,UnitWeightsOfChars cohesionUpgrade,UnitWeightsOfChars armourUpgrade)
    {
        UnitType = unitType;
        UnitRace = unitRace;
        Characteristics = uChar;
        NumberOfUnitsUpgrade = numberOfUnitsUpgrade;
        HealthUpgrade = healthUpgrade;
        DamageUpgrade = damageUpgrade;
        InitiativeUpgrade = initiativeUpgrade;
        CohesionUpgrade = cohesionUpgrade;
        ArmourUpgrade = armourUpgrade;
        UnitWeight = unitWeight;
    }
    public BaseUnitCharacteristics(string unitType,Race unitRace,UnitCharacteristics uChar,List<UnitWeightsOfChars> listUnitWeightsOfChars, int unitWeight)
    {
        UnitType = unitType;
        UnitRace = unitRace;
        Characteristics = uChar;
        UnitAbilities.Add(Abilities.BasicAttack);
        NumberOfUnitsUpgrade = listUnitWeightsOfChars[0];
        HealthUpgrade = listUnitWeightsOfChars[1];
        DamageUpgrade = listUnitWeightsOfChars[2];
        InitiativeUpgrade = listUnitWeightsOfChars[3];
        CohesionUpgrade = listUnitWeightsOfChars[4];
        ArmourUpgrade = listUnitWeightsOfChars[5];
        UnitWeight = unitWeight;
    }
    public BaseUnitCharacteristics(BaseUnitCharacteristics serializableUnitClassItems)
    {
        UnitType = serializableUnitClassItems.UnitType;
        UnitRace = serializableUnitClassItems.UnitRace;
        Characteristics = serializableUnitClassItems.Characteristics;
        NumberOfUnitsUpgrade = serializableUnitClassItems.NumberOfUnitsUpgrade;
        HealthUpgrade = serializableUnitClassItems.HealthUpgrade;
        DamageUpgrade = serializableUnitClassItems.DamageUpgrade;
        InitiativeUpgrade = serializableUnitClassItems.InitiativeUpgrade;
        CohesionUpgrade = serializableUnitClassItems.CohesionUpgrade;
        ArmourUpgrade = serializableUnitClassItems.ArmourUpgrade;
        UnitWeight = serializableUnitClassItems.UnitWeight;
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

    [Tooltip("Amount of upgrade points with weights")]
    public List<PointsToRandomuzeUnitWeights> pointsRandomizerList;
    [Tooltip("All default unit characteristics")]
    public List<BaseUnitCharacteristics> UnitList = new List<BaseUnitCharacteristics>();
    public (string,Race, UnitUpgrades) GetRandomUnit(Race unitRace)
    {
        List<BaseUnitCharacteristics> localList = new List<BaseUnitCharacteristics>();
        foreach (BaseUnitCharacteristics _class in UnitList.FindAll(x => x.UnitRace.Equals(unitRace)))
        {
            localList.Add(_class);
        }
        var localUnitWeights = new int[localList.Count];
        for (int i = 0; i < localList.Count; i++) { localUnitWeights[i] = localList[i].UnitWeight;}
        int indexOfSelectedUnit = GetRandomWeightedIndex(localUnitWeights);

        var localPointsToRandomize = new int[pointsRandomizerList.Count];
        for (int i = 0; i < pointsRandomizerList.Count; i++) { localPointsToRandomize[i] = pointsRandomizerList[i].weight; }
        int indexOfNumberOfPoints = GetRandomWeightedIndex(localPointsToRandomize);

        
        UnitUpgrades newUnitUpgrades = GetUnitWithRandomizedStats(localList[indexOfSelectedUnit], pointsRandomizerList[indexOfNumberOfPoints].points);
        return (localList[indexOfSelectedUnit].UnitType,localList[indexOfSelectedUnit].UnitRace, newUnitUpgrades);
    }

    public UnitUpgrades GetUnitWithRandomizedStats(BaseUnitCharacteristics unit,int numberOfPointsToRandomize)
    {
        UnitUpgrades unitUpgrades = new UnitUpgrades();
        List<UnitWeightsOfChars> localWeightsOfChars= new List<UnitWeightsOfChars>
            {unit.NumberOfUnitsUpgrade,unit.HealthUpgrade,unit.DamageUpgrade,unit.InitiativeUpgrade,unit.CohesionUpgrade,unit.ArmourUpgrade };
        var localListOfStatWeights = new int[localWeightsOfChars.Count];
        for (int i = 0; i < localWeightsOfChars.Count; i++) { localListOfStatWeights[i] = localWeightsOfChars[i].Weight; }

        while (numberOfPointsToRandomize > 0)
        {
            int indexOfStat = GetRandomWeightedIndex(localListOfStatWeights);
            if (numberOfPointsToRandomize - localWeightsOfChars[indexOfStat].Cost >= 0)
            {
                numberOfPointsToRandomize -= localWeightsOfChars[indexOfStat].Cost;
                if (indexOfStat == 0) { unitUpgrades.NumberOfUnits += 1; }
                if (indexOfStat == 1) { unitUpgrades.Health += 1; }
                if (indexOfStat == 2) { unitUpgrades.Damage += 1; }
                if (indexOfStat == 3) { unitUpgrades.Initiative += 1; }
                if (indexOfStat == 4) { unitUpgrades.Cohesion += 1; }
                if (indexOfStat == 5) { unitUpgrades.Armour += 1; }
                localListOfStatWeights[indexOfStat] = Mathf.Abs(localListOfStatWeights[indexOfStat]/5);
            }
        }
        return (unitUpgrades);
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
        FillListOfUnitChars();
    }
    private void FillListOfUnitChars()
    {
        // LOOK INTO SAVE LOAD ?
        UnitList.Clear();
        var p_lmilita_b = new UnitCharacteristics(14, 3, 2, -1, -1, 0);
        List<UnitWeightsOfChars> p_militia_stats = new List<UnitWeightsOfChars>
        {
            new UnitWeightsOfChars(1, 4, 12),
            new UnitWeightsOfChars(2, 1, 10),
            new UnitWeightsOfChars(2, 1, 8),
            new UnitWeightsOfChars(2, 1, 3),
            new UnitWeightsOfChars(1, 1, 5),
            new UnitWeightsOfChars(5, 1, 2)
        };
        UnitList.Add(new BaseUnitCharacteristics("local_militia",Race.Human,p_lmilita_b,p_militia_stats,10) );

        var p_dthieves_b = new UnitCharacteristics(7, 5, 5, 2, -2, 0);
        List<UnitWeightsOfChars> p_dthieves_b_stats = new List<UnitWeightsOfChars>
        {
            new UnitWeightsOfChars(2, 4, 7),
            new UnitWeightsOfChars(1, 1, 10),
            new UnitWeightsOfChars(1, 2, 10),
            new UnitWeightsOfChars(1, 1, 12),
            new UnitWeightsOfChars(1, 1, 5),
            new UnitWeightsOfChars(5, 1, 2)
        };
        UnitList.Add(new BaseUnitCharacteristics("dirty_thieves",Race.Human,p_dthieves_b,p_dthieves_b_stats,8) );

        var p_twarriors_b = new UnitCharacteristics(5, 8, 5, 0, 2, 0);
        List<UnitWeightsOfChars> p_twarriors_b_stats = new List<UnitWeightsOfChars>
        {
            new UnitWeightsOfChars(1, 2, 7),
            new UnitWeightsOfChars(1, 2, 12),
            new UnitWeightsOfChars(2, 2, 10),
            new UnitWeightsOfChars(1, 1, 9),
            new UnitWeightsOfChars(1, 2, 5),
            new UnitWeightsOfChars(3, 1, 6)
        };
        UnitList.Add(new BaseUnitCharacteristics("trained_warriors",Race.Human,p_twarriors_b,p_twarriors_b_stats,6) );

        var e_ogre_b = new UnitCharacteristics(1, 40, 15, 0, -3, 2);
        List<UnitWeightsOfChars> p_ogre_b_stats = new List<UnitWeightsOfChars>
        {
            new UnitWeightsOfChars(4, 1, 2),
            new UnitWeightsOfChars(1, 15, 10),
            new UnitWeightsOfChars(1, 10, 10),
            new UnitWeightsOfChars(1, 1, 7),
            new UnitWeightsOfChars(1, 2, 4),
            new UnitWeightsOfChars(2, 1, 4)
        };
        UnitList.Add(new BaseUnitCharacteristics("ogre",Race.Goblin,e_ogre_b,p_ogre_b_stats,6) );

        var e_rgoblin_b = new UnitCharacteristics(12, 3, 3, 2, -1, 0);
        List<UnitWeightsOfChars> p_rgoblin_b_stats = new List<UnitWeightsOfChars>
        {
            new UnitWeightsOfChars(2, 5, 10),
            new UnitWeightsOfChars(1, 1, 8),
            new UnitWeightsOfChars(1, 1, 12),
            new UnitWeightsOfChars(1, 1, 12),
            new UnitWeightsOfChars(1, 1, 8),
            new UnitWeightsOfChars(5, 1, 2)
        };
        UnitList.Add(new BaseUnitCharacteristics("goblin_rogue",Race.Goblin,e_rgoblin_b,p_rgoblin_b_stats,8) );

        var e_pgoblin_b = new UnitCharacteristics(24, 2, 1, -1, -3, 0);
        List<UnitWeightsOfChars> p_pgoblin_b_stats = new List<UnitWeightsOfChars>
        {
            new UnitWeightsOfChars(2, 14, 12),
            new UnitWeightsOfChars(2, 1, 5),
            new UnitWeightsOfChars(3, 1, 8),
            new UnitWeightsOfChars(1, 1, 12),
            new UnitWeightsOfChars(2, 2, 5),
            new UnitWeightsOfChars(5, 1, 2)
        };
        UnitList.Add(new BaseUnitCharacteristics("goblin_punks",Race.Goblin,e_pgoblin_b,p_pgoblin_b_stats,10) );
    }
}
