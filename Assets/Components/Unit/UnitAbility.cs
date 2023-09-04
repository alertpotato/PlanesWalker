using System;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable] 
public enum AbilityType { Main, Secondary };

[Serializable]
public abstract class UnitAbility
{
    public string AbilityName = "Default ability name";
    public List<int[]> targets = new List<int[]>();
    public AbilityType Type;
    public abstract (Hero,List<GameObject>,Hero,List<GameObject>) MainFunc(int[] pos,Hero yourHero,Hero opposingHero);
    public abstract List<int[]> AbilityTargets(int[] pos,Hero yourHero,Hero opposingHero);
}
