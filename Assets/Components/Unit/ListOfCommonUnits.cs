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
    public int[] UnitSupplyReq = new int[4]; // food, weapons, money, wisdom
    public BaseUnitCharacteristics(string unitType,Race unitRace,UnitCharacteristics uChar, int unitWeight,UnitWeightsOfChars numberOfUnitsUpgrade,UnitWeightsOfChars healthUpgrade,UnitWeightsOfChars damageUpgrade,UnitWeightsOfChars initiativeUpgrade,UnitWeightsOfChars cohesionUpgrade,UnitWeightsOfChars armourUpgrade, int[] unitSupplyReq)
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
        UnitSupplyReq = unitSupplyReq;
    }
    public BaseUnitCharacteristics(string unitType,Race unitRace,UnitCharacteristics uChar,List<UnitWeightsOfChars> listUnitWeightsOfChars, int unitWeight, int[] unitSupplyReq)
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
        UnitSupplyReq = unitSupplyReq;
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
        UnitSupplyReq = serializableUnitClassItems.UnitSupplyReq;
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
    public (string, UnitUpgrades) GetRandomUnit(Race unitRace)
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
        return (localList[indexOfSelectedUnit].UnitType, newUnitUpgrades);
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
        var p_lmilita_b = new UnitCharacteristics(4, 8, 3, 0, 1, 0);
        List<UnitWeightsOfChars> p_militia_stats = new List<UnitWeightsOfChars>
        {
            new UnitWeightsOfChars(1, 2, 12),
            new UnitWeightsOfChars(1, 2, 10),
            new UnitWeightsOfChars(2, 1, 8),
            new UnitWeightsOfChars(2, 1, 3),
            new UnitWeightsOfChars(1, 1, 4),
            new UnitWeightsOfChars(5, 1, 2)
        };
        int[] p_lmilita_s = {0,1,0,0};
        UnitList.Add(new BaseUnitCharacteristics("militia",Race.Human,p_lmilita_b,p_militia_stats,9,p_lmilita_s ));
        
        var p_spearman_b = new UnitCharacteristics(5, 10, 3, 1, 2, 0);
        List<UnitWeightsOfChars> p_spearman_b_stats = new List<UnitWeightsOfChars>
        {
            new UnitWeightsOfChars(1, 2, 8),
            new UnitWeightsOfChars(1, 2, 12),
            new UnitWeightsOfChars(2, 1, 10),
            new UnitWeightsOfChars(2, 1, 6),
            new UnitWeightsOfChars(2, 2, 6),
            new UnitWeightsOfChars(4, 1, 3)
        };
        int[] p_spearman_s = {1,1,0,0};
        UnitList.Add(new BaseUnitCharacteristics("spearman",Race.Human,p_spearman_b,p_spearman_b_stats,10,p_spearman_s) );
        
        var p_archer_b = new UnitCharacteristics(4, 7, 3, 1, 2, 0);
        List<UnitWeightsOfChars> p_archer_b_stats = new List<UnitWeightsOfChars>
        {
            new UnitWeightsOfChars(1, 1, 9),
            new UnitWeightsOfChars(1, 1, 8),
            new UnitWeightsOfChars(2, 1, 12),
            new UnitWeightsOfChars(2, 1, 9),
            new UnitWeightsOfChars(1, 1, 8),
            new UnitWeightsOfChars(5, 1, 2)
        };
        int[] p_archer_s = {2,0,0,0};
        UnitList.Add(new BaseUnitCharacteristics("archer",Race.Human,p_archer_b,p_archer_b_stats,8,p_archer_s) );
        
        var p_road_bandit_b = new UnitCharacteristics(5, 8, 2, 2, -1, 0);
        List<UnitWeightsOfChars> p_road_bandit_b_stats = new List<UnitWeightsOfChars>
        {
            new UnitWeightsOfChars(2, 3, 7),
            new UnitWeightsOfChars(1, 1, 10),
            new UnitWeightsOfChars(1, 1, 10),
            new UnitWeightsOfChars(1, 1, 12),
            new UnitWeightsOfChars(2, 1, 7),
            new UnitWeightsOfChars(5, 1, 2)
        };
        int[] p_road_bandit_b_s = {0,0,1,0};
        UnitList.Add(new BaseUnitCharacteristics("road_bandit",Race.Human,p_road_bandit_b,p_road_bandit_b_stats,7,p_road_bandit_b_s) );
        
        var p_mercenaries_b = new UnitCharacteristics(5, 11, 4, 2, 0, 0);
        List<UnitWeightsOfChars> p_mercenaries_b_stats = new List<UnitWeightsOfChars>
        {
            new UnitWeightsOfChars(1, 2, 10),
            new UnitWeightsOfChars(1, 2, 10),
            new UnitWeightsOfChars(1, 1, 10),
            new UnitWeightsOfChars(2, 1, 12),
            new UnitWeightsOfChars(1, 1, 6),
            new UnitWeightsOfChars(4, 1, 3)
        };
        int[] p_mercenaries_b_s = {1,0,1,0};
        UnitList.Add(new BaseUnitCharacteristics("mercenaries",Race.Human,p_mercenaries_b,p_mercenaries_b_stats,6,p_mercenaries_b_s) );
        
        var p_hobelar_b = new UnitCharacteristics(3, 13, 4, 2, 2, 0);
        List<UnitWeightsOfChars> p_hobelar_b_stats = new List<UnitWeightsOfChars>
        {
            new UnitWeightsOfChars(2, 1, 10),
            new UnitWeightsOfChars(2, 3, 12),
            new UnitWeightsOfChars(1, 2, 7),
            new UnitWeightsOfChars(1, 1, 12),
            new UnitWeightsOfChars(1, 1, 8),
            new UnitWeightsOfChars(4, 1, 3)
        };
        int[] p_hobelar_b_s = {2,1,0,0};
        UnitList.Add(new BaseUnitCharacteristics("hobelar",Race.Human,p_hobelar_b,p_hobelar_b_stats,5,p_hobelar_b_s) );
        
        var p_hedge_knight_b = new UnitCharacteristics(2, 16, 8, 1, 4, 1);
        List<UnitWeightsOfChars> p_hedge_knight_b_stats = new List<UnitWeightsOfChars>
        {
            new UnitWeightsOfChars(3, 1, 6),
            new UnitWeightsOfChars(1, 4, 12),
            new UnitWeightsOfChars(1, 2, 12),
            new UnitWeightsOfChars(1, 1, 7),
            new UnitWeightsOfChars(1, 2, 10),
            new UnitWeightsOfChars(3, 1, 6)
        };
        int[] p_hedge_knight_b_s = {1,2,0,0};
        UnitList.Add(new BaseUnitCharacteristics("hedge_knight",Race.Human,p_hedge_knight_b,p_hedge_knight_b_stats,4,p_hedge_knight_b_s) );
        
        
        var e_ogre_b = new UnitCharacteristics(1, 20, 12, 0, 3, 0);
        List<UnitWeightsOfChars> p_ogre_b_stats = new List<UnitWeightsOfChars>
        {
            new UnitWeightsOfChars(4, 1, 6),
            new UnitWeightsOfChars(1, 10, 10),
            new UnitWeightsOfChars(1, 6, 10),
            new UnitWeightsOfChars(2, 1, 4),
            new UnitWeightsOfChars(1, 1, 7),
            new UnitWeightsOfChars(4, 1, 4)
        };
        int[] e_ogre_s = {1,0,0,0};
        UnitList.Add(new BaseUnitCharacteristics("ogre",Race.Goblin,e_ogre_b,p_ogre_b_stats,8,e_ogre_s) );

        var e_goblin_militia_b = new UnitCharacteristics(8, 4, 2, 0, -1, 0);
        List<UnitWeightsOfChars> p_goblin_militia_b_stats = new List<UnitWeightsOfChars>
        {
            new UnitWeightsOfChars(2, 3, 10),
            new UnitWeightsOfChars(2, 1, 10),
            new UnitWeightsOfChars(2, 1, 12),
            new UnitWeightsOfChars(1, 1, 12),
            new UnitWeightsOfChars(2, 1, 8),
            new UnitWeightsOfChars(6, 1, 2)
        };
        int[] e_goblin_militia_s = {0,1,0,0};
        UnitList.Add(new BaseUnitCharacteristics("goblin_militia",Race.Goblin,e_goblin_militia_b,p_goblin_militia_b_stats,8,e_goblin_militia_s) );

        var e_goblin_spearman_b = new UnitCharacteristics(6, 6, 3, 1, 2, 0);
        List<UnitWeightsOfChars> p_goblin_spearman_b_stats = new List<UnitWeightsOfChars>
        {
            new UnitWeightsOfChars(1, 2, 10),
            new UnitWeightsOfChars(1, 1, 8),
            new UnitWeightsOfChars(1, 1, 8),
            new UnitWeightsOfChars(1, 1, 8),
            new UnitWeightsOfChars(2, 1, 12),
            new UnitWeightsOfChars(5, 1, 3)
        };
        int[] e_goblin_spearman_s = {1,1,0,0};
        UnitList.Add(new BaseUnitCharacteristics("goblin_spearman",Race.Goblin,e_goblin_spearman_b,p_goblin_spearman_b_stats,10,e_goblin_spearman_s) );
        
        var e_goblin_skiermisher_b = new UnitCharacteristics(5, 4, 2, 1, 0, 0);
        List<UnitWeightsOfChars> p_goblin_skiermisher_b_stats = new List<UnitWeightsOfChars>
        {
            new UnitWeightsOfChars(1, 2, 8),
            new UnitWeightsOfChars(1, 1, 10),
            new UnitWeightsOfChars(2, 1, 12),
            new UnitWeightsOfChars(1, 1, 10),
            new UnitWeightsOfChars(2, 1, 8),
            new UnitWeightsOfChars(6, 1, 3)
        };
        int[] e_goblin_skiermisher_s = {1,0,0,0};
        UnitList.Add(new BaseUnitCharacteristics("goblin_skiermisher",Race.Goblin,e_goblin_skiermisher_b,p_goblin_skiermisher_b_stats,6,e_goblin_skiermisher_s) );
    }
}
