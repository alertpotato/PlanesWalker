using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;
[System.Serializable]
public class UnitBuff
{
    public UnitAttributes Buff; public GameObject BuffParent; public int BuffTurns;
    public UnitBuff(UnitAttributes buff, GameObject buffParent, int buffTurns)
    {
        Buff = buff;
        BuffParent = buffParent;
        BuffTurns = buffTurns;
    }
}

public class ArmyUnitClass : MonoBehaviour
{
    public string squadName;
    public Unit unit;
    public float currentSquadHealth;
    public int currentHeat;
    public int advantage;
    public List<UnitBuff> Buffs = new List<UnitBuff>();
    public List<AbilityTags> UnitAbilityTags = new List<AbilityTags>();
    public UnitFactory unitFactory;
    
    //-----------Initialization logic
    public void InitializeUnit(Unit newUnit)
    {
        unit = newUnit;
        squadName = unit.UnitName;
        //TODO TEMP solution dnt know what to do
        UpdateUnitTags();
        RebuildUnit();
    }

    private void RebuildUnit()
    {
        unit.RebuildCurrentUnitAttributes();
        currentSquadHealth = unit.CurrentUnitAttributes.Health*unit.CurrentUnitAttributes.SquadSize;
        currentHeat = 0;
        advantage = 0;
        Buffs.Clear();
    }
    

    //-----------Abilities logic
    public void InitializeAbilities(Company newCompany,FormationManager newFormationManager)
    {
        foreach (var ability in unit.CurrentUnitAttributes.SquadAbilities)
        {
            ability.InitAbility(newCompany,newFormationManager);
        }
    }
    public void InitializeAbilities(Company newCompany)
    {
        foreach (var ability in unit.CurrentUnitAttributes.SquadAbilities)
        {
            ability.ChangeCompany(newCompany);
        }
    }

    [CanBeNull]
    public UnitAbility GetPossibleAbility() //used to get ability that would be used by this unit next round
    {
        UnitAbility activeAbility = null;
        foreach (var ability in unit.CurrentUnitAttributes.SquadAbilities)
        {
            if (ability.SelectTargets()) return ability;
        }
        return activeAbility;
    }
    [CanBeNull]
    public UnitAbility GetRetaliationAbility(List<AbilityTags> retaliationTags) //used to get ability to retaliate attack
    {
        UnitAbility retaliationAbility = null;
        foreach (var ability in unit.CurrentUnitAttributes.SquadAbilities)
        {
            foreach (var tag in retaliationTags)
            {
                if (ability.Tags.Contains(tag)) return ability;
            }
        }
        return retaliationAbility;
    }
    
    //-----------Unit battle logic
    public void OnBattleStart(int[] currentSupply,List<UnitBuff> newBuffs,int minimumSupply=0)
    {
        //minimumSupply used for AI to cheat
        Buffs.Clear();
        ReciveBuffs(newBuffs);
        ApplyBuffs();
    }

    public void OnRoundEnd() //Must be called at the end of the round
    {
        CheckBuffs();
        ApplyBuffs();
    }
    public void OnBattleEnd() //Must be called at the end of the battle
    {
        Buffs.Clear();
        RebuildUnit();

    }
    //-----------Unit buffs logic
    private void CheckBuffs()
    {
        foreach (var buff in Buffs)
        {
            buff.BuffTurns -= 1;
        }
        var expiredBuffs = Buffs.Where(bf => bf.BuffTurns <=0).ToList();
        foreach (var buff in expiredBuffs)
        {
            Buffs.Remove(buff);
        }
    }
    public void ReciveBuffs(List<UnitBuff> buffs)
    {
        Buffs.AddRange(buffs);
    }

    private void ApplyBuffs() // Reverting to baseline and then applying buffs
    {
        //TODO do do
    }
    //-----------Unit stats logic
    public bool TakeDamage((int, int) statsToChange)
    {
        bool isAlive = true;
        currentSquadHealth = statsToChange.Item1;
        unit.CurrentUnitAttributes.SquadSize = statsToChange.Item2;
        // TODO is it ok? maybe check for NumberOfUnits >0
        if (currentSquadHealth <= 0) isAlive = false;
//        Debug.Log($"{UnitName} HP:{currentSquadHealth} N:{CurrentUnitCharacteristics.NumberOfUnits} IA:{isAlive.ToString()}");
        return isAlive;
    }
    public void UpgradeUnit(UnitAttributes newUpgrade)
    {
        unit.UnitUpgrades.Add(newUpgrade);
        RebuildUnit();
    }

    private void UpdateUnitTags()
    {
        foreach (var ability in unit.CurrentUnitAttributes.SquadAbilities)
        {
            UnitAbilityTags.AddRange(ability.Tags);
        }
    }
}
