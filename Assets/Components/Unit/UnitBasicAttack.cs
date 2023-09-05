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
    public override (Hero,List<GameObject>,Hero,List<GameObject>) MainFunc()
    {
        //Unit
        if (UnitPosition[0] == 0) targets.Add(UnitPosition);
        return (YourHero, new List<GameObject>(), OpposingHero, new List<GameObject>());
    }
    public override List<int[]> AbilityTargets()
    {
        return targets;
    }
}
