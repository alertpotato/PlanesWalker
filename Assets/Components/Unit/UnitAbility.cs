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
    public GameObject Unit;
    public Hero YourHero;
    public Hero OpposingHero;
    public int[] UnitPosition;
    public abstract (Hero,List<GameObject>,Hero,List<GameObject>) MainFunc();
    public abstract List<int[]> AbilityTargets();
    public void InitAbility(int[] pos, GameObject unit, Hero unitHero,Hero oppHero)
    {
        UnitPosition = pos;
        Unit = unit;
        YourHero = unitHero;
        OpposingHero = oppHero;
    }
}
