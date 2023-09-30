using System;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable] 
public enum AbilityType { Main, Secondary };
[System.Serializable] 
public enum Abilities { BasicAttack, Defend, Spell };
[Serializable]
public abstract class UnitAbility
{
    public string AbilityName = "Default ability name";
    public List<int[]> targets = new List<int[]>();
    public AbilityType Type;
    public Squad UnitSquad;
    public Hero YourHero;
    public Hero OpposingHero;
    public bool IsActive = false;
    public abstract (Hero,List<GameObject>,Hero,List<GameObject>) MainFunc(Hero opposingHero, bool applyDamage);
    public abstract List<int[]> AbilityTargets();
    public void InitAbility(Squad squad, Hero unitHero)
    {
        UnitSquad = squad;
        YourHero = unitHero;
    }
}
