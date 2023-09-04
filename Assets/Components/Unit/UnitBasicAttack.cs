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
    public override (Hero,List<GameObject>,Hero,List<GameObject>) MainFunc(int[] pos,Hero yourHero,Hero opposingHero)
    {
        if (pos[0] == 0) targets.Add(pos);
        return (yourHero, new List<GameObject>(), opposingHero, new List<GameObject>());
    }

    public override List<int[]> AbilityTargets(int[] pos, Hero yourHero, Hero opposingHero)
    {
        return targets;
    }
}
