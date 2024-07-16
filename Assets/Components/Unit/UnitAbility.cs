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
    public List<(int,int)> targets = new List<(int,int)>();
    public AbilityType Type;
    public Company UnitCompany;
    public FormationField UnitField;
    public FormationField OpposingField;
    public bool IsActive = false;
    public abstract (List<GameObject>,List<GameObject>) MainFunc(bool applyDamage);
    public abstract List<(int,int)> AbilityTargets();
    public void InitAbility(Company unitCompany, FormationField unitField,FormationField opposingField)
    {
        UnitCompany = unitCompany;
        UnitField = unitField;
        OpposingField = opposingField;
    }
}
