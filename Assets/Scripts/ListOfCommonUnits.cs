using System.Collections.Generic;
using UnityEngine;
using Planeswalker.Scripts;

namespace Planeswalker
{
    public class ListOfCommonUnits : ScriptableObject
    {
        public static List<UnitStartingCharacteristics> plebListOfUnitBaseCharacteristics;
        public static List<UnitStartingCharacteristics> evilListOfUnitBaseCharacteristics;
        public static List<PointsToRandomuzeUnitWeights> pointsRandomizerList;
        public struct UnitWeightsOfChars 
        {
            public string unitParam; public int cost; public int paramMod; public int paramWeight;
            public UnitWeightsOfChars( string uparam, int _cost, int mparam, int wparam)
            {
                unitParam=uparam;cost=_cost;paramMod=mparam;paramWeight=wparam;
            }

        };
        public struct UnitStartingCharacteristics 
        {
            public UnitCharacteristics unitBaseStats;public int unitWeight; public List<UnitWeightsOfChars> listOfWeightedStats;
            public UnitStartingCharacteristics(UnitCharacteristics basestats,int weight, List<UnitWeightsOfChars> list)
            {
                unitBaseStats = basestats; unitWeight= weight; listOfWeightedStats= list;
            }
        };
        public struct PointsToRandomuzeUnitWeights
        {
            public int points; public int weight;
            public PointsToRandomuzeUnitWeights(int p, int w)
            { points = p;weight = w; }
        }

        private void Awake()
        {
            evilListOfUnitBaseCharacteristics = new List<UnitStartingCharacteristics>();
            plebListOfUnitBaseCharacteristics = new List<UnitStartingCharacteristics>();
            FillListOfUnitChars();
        }

        private void FillListOfUnitChars()
        {
            pointsRandomizerList = new List<PointsToRandomuzeUnitWeights>()
            {
                new PointsToRandomuzeUnitWeights( 2, 500),
                new PointsToRandomuzeUnitWeights( 3, 1000),
                new PointsToRandomuzeUnitWeights( 4, 250),
                new PointsToRandomuzeUnitWeights( 5, 10),
                new PointsToRandomuzeUnitWeights( 6, 1)
            };

            var p_lmilita_b = new UnitCharacteristics("local_militia", 14, 3, 3, -1, -1, 0);
            List<UnitWeightsOfChars> p_militia_stats = new List<UnitWeightsOfChars>
            {
                new UnitWeightsOfChars("number", 1, 5, 12),
                new UnitWeightsOfChars("health", 1, 1, 10),
                new UnitWeightsOfChars("damage", 2, 1, 8),
                new UnitWeightsOfChars("init", 1, 1, 5),
                new UnitWeightsOfChars("coh", 1, 1, 5),
                new UnitWeightsOfChars("armour", 5, 1, 2)
            };
            var p_militia = new UnitStartingCharacteristics(p_lmilita_b, 10, p_militia_stats);
            plebListOfUnitBaseCharacteristics.Add(p_militia);

            var p_dthieves_b = new UnitCharacteristics("dirty_thieves", 7, 5, 6, 0, -2, 0);
            List<UnitWeightsOfChars> p_dthieves_b_stats = new List<UnitWeightsOfChars>
            {
                new UnitWeightsOfChars("number", 2, 4, 7),
                new UnitWeightsOfChars("health", 1, 1, 10),
                new UnitWeightsOfChars("damage", 1, 2, 10),
                new UnitWeightsOfChars("init", 1, 1, 12),
                new UnitWeightsOfChars("coh", 1, 1, 5),
                new UnitWeightsOfChars("armour", 5, 1, 2)
            };
            var p_dthieves = new UnitStartingCharacteristics(p_dthieves_b, 8, p_dthieves_b_stats);
            plebListOfUnitBaseCharacteristics.Add(p_dthieves);

            var p_twarriors_b = new UnitCharacteristics("trained_warriors", 5, 8, 5, 0, 2, 0);
            List<UnitWeightsOfChars> p_twarriors_b_stats = new List<UnitWeightsOfChars>
            {
                new UnitWeightsOfChars("number", 1, 2, 7),
                new UnitWeightsOfChars("health", 1, 2, 12),
                new UnitWeightsOfChars("damage", 2, 2, 10),
                new UnitWeightsOfChars("init", 1, 1, 9),
                new UnitWeightsOfChars("coh", 1, 2, 4),
                new UnitWeightsOfChars("armour", 3, 1, 4)
            };
            var p_twarriors = new UnitStartingCharacteristics(p_twarriors_b, 6, p_twarriors_b_stats);
            plebListOfUnitBaseCharacteristics.Add(p_twarriors);

            var e_ogre_b = new UnitCharacteristics("ogre", 1, 40, 15, 0, -3, 2);
            List<UnitWeightsOfChars> p_ogre_b_stats = new List<UnitWeightsOfChars>
            {
                new UnitWeightsOfChars("number", 4, 1, 2),
                new UnitWeightsOfChars("health", 1, 15, 10),
                new UnitWeightsOfChars("damage", 1, 5, 10),
                new UnitWeightsOfChars("init", 1, 1, 7),
                new UnitWeightsOfChars("coh", 1, 2, 4),
                new UnitWeightsOfChars("armour", 2, 1, 4)
            };
            var p_ogre = new UnitStartingCharacteristics(e_ogre_b, 6, p_ogre_b_stats);
            evilListOfUnitBaseCharacteristics.Add(p_ogre);

            var e_rgoblin_b = new UnitCharacteristics("goblin_rogue", 12, 3, 3, 2, -1, 0);
            List<UnitWeightsOfChars> p_rgoblin_b_stats = new List<UnitWeightsOfChars>
            {
                new UnitWeightsOfChars("number", 2, 5, 10),
                new UnitWeightsOfChars("health", 1, 1, 8),
                new UnitWeightsOfChars("damage", 1, 1, 12),
                new UnitWeightsOfChars("init", 1, 1, 12),
                new UnitWeightsOfChars("coh", 1, 1, 8),
                new UnitWeightsOfChars("armour", 5, 1, 2)
            };
            var p_rgoblin = new UnitStartingCharacteristics(e_rgoblin_b, 8, p_rgoblin_b_stats);
            evilListOfUnitBaseCharacteristics.Add(p_rgoblin);

            var e_pgoblin_b = new UnitCharacteristics("goblin_punks", 20, 3, 2, -1, -3, 0);
            List<UnitWeightsOfChars> p_pgoblin_b_stats = new List<UnitWeightsOfChars>
            {
                new UnitWeightsOfChars("number", 2, 7, 12),
                new UnitWeightsOfChars("health", 1, 1, 5),
                new UnitWeightsOfChars("damage", 2, 1, 8),
                new UnitWeightsOfChars("init", 1, 1, 12),
                new UnitWeightsOfChars("coh", 1, 1, 5),
                new UnitWeightsOfChars("armour", 6, 1, 2)
            };
            var p_pgoblin = new UnitStartingCharacteristics(e_pgoblin_b, 10, p_pgoblin_b_stats);
            evilListOfUnitBaseCharacteristics.Add(p_pgoblin);
        }

        public (UnitCharacteristics, CountUnitUpgrades) GetRandomUnit(string unitClass)
        {
            List<UnitStartingCharacteristics> localList;
            if (unitClass == "p") { localList = plebListOfUnitBaseCharacteristics; }
            else if (unitClass == "e") { localList = evilListOfUnitBaseCharacteristics; }
            else { localList = new List<UnitStartingCharacteristics>(); }

            var localUnitWeights = new int[localList.Count];
            for (int i = 0; i < localList.Count; i++) { localUnitWeights[i] = localList[i].unitWeight;}
            int indexOfSelectedUnit = GetRandomWeightedIndex(localUnitWeights);

            var localPointsToRandomize = new int[pointsRandomizerList.Count];
            for (int i = 0; i < pointsRandomizerList.Count; i++) { localPointsToRandomize[i] = pointsRandomizerList[i].weight; }
            int indexOfNumberOfPoints = GetRandomWeightedIndex(localPointsToRandomize);

            CountUnitUpgrades unitUpgrades = new CountUnitUpgrades();
            return GetUnitWithRandomizedStats(localList[indexOfSelectedUnit], pointsRandomizerList[indexOfNumberOfPoints].points, unitUpgrades);
        }

        public (UnitCharacteristics,CountUnitUpgrades) GetUnitWithRandomizedStats(UnitStartingCharacteristics unit,int numberOfPointsToRandomize, CountUnitUpgrades _unitUpgrades)
        {
            var localListOfStatWeights = new int[unit.listOfWeightedStats.Count];
            for (int i = 0; i < unit.listOfWeightedStats.Count; i++) { localListOfStatWeights[i] = unit.listOfWeightedStats[i].paramWeight; }

            while (numberOfPointsToRandomize > 0)
            {
                int indexOfStat = GetRandomWeightedIndex(localListOfStatWeights);
                if (numberOfPointsToRandomize - unit.listOfWeightedStats[indexOfStat].cost >= 0)
                {
                    numberOfPointsToRandomize -= unit.listOfWeightedStats[indexOfStat].cost;
                    if (indexOfStat == 0) { unit.unitBaseStats.ucnumberofunits += unit.listOfWeightedStats[indexOfStat].paramMod; _unitUpgrades.cnumber += 1; }
                    if (indexOfStat == 1) { unit.unitBaseStats.ucunithealth += unit.listOfWeightedStats[indexOfStat].paramMod; _unitUpgrades.chealth += 1; }
                    if (indexOfStat == 2) { unit.unitBaseStats.ucunitdamage += unit.listOfWeightedStats[indexOfStat].paramMod; _unitUpgrades.cdamage += 1; }
                    if (indexOfStat == 3) { unit.unitBaseStats.ucunitinitiative += unit.listOfWeightedStats[indexOfStat].paramMod; _unitUpgrades.cinitiative += 1; }
                    if (indexOfStat == 4) { unit.unitBaseStats.ucunitcohesion += unit.listOfWeightedStats[indexOfStat].paramMod; _unitUpgrades.ccohesion += 1; }
                    if (indexOfStat == 5) { unit.unitBaseStats.unitarmour += unit.listOfWeightedStats[indexOfStat].paramMod; _unitUpgrades.carmour += 1; }
                    localListOfStatWeights[indexOfStat] = Mathf.Abs(localListOfStatWeights[indexOfStat]/5);
                }
            }
            return (unit.unitBaseStats,_unitUpgrades);
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

    }
}
