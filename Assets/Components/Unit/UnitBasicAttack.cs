using System;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class UnitBasicAttack : UnitAbility
{
    public UnitBasicAttack()
    {
        AbilityName = "Basic attack";
        Type = AbilityType.Main;
    }
    public override (Hero,List<GameObject>,Hero,List<GameObject>) MainFunc(Hero opposingHero)
    {
        OpposingHero = opposingHero;
        IsActive = false;
        int[] unitPosition = new int[2] { UnitSquad.Line,UnitSquad.Column};
        if (unitPosition[0] == 0 && OpposingHero.ArmyFormation[unitPosition[0]].ArmyLine[unitPosition[1]].Type ==
            CellType.Occupied)
        {
            targets.Clear();
            targets.Add(unitPosition);
            IsActive = true;
            Debug.Log(UnitSquad.Unit.name);
            Debug.Log(OpposingHero.ArmyFormation[unitPosition[0]].ArmyLine[unitPosition[1]].Unit.name);
            Debug.Log($"{UnitSquad.Unit.name} attack {OpposingHero.ArmyFormation[unitPosition[0]].ArmyLine[unitPosition[1]].Unit.name}");
        }
        return (YourHero, new List<GameObject>(), OpposingHero, new List<GameObject>());
    }
    public override List<int[]> AbilityTargets()
    {
        return targets;
    }
}
