using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Serialization;
[System.Serializable]
public class UnitCharacteristics
{
    public int NumberOfUnits; public int Health; public int Damage; public int Initiative; public int Cohesion; public int Armour;
    public UnitCharacteristics(int num, int h, int d, int init, int coh, int armour)
    {
        NumberOfUnits = num; Health = h; Damage = d; Initiative = init; ; Cohesion = coh; Armour = armour;
    }
    public UnitCharacteristics(BaseUnitCharacteristics baseCharacteristics ,UnitUpgrades unitUpgrades, int supplyMultiplier)
    {
        //TODO figure out how to apply supplyMultiplier to different stats with different scaling options
        NumberOfUnits = ( baseCharacteristics.Characteristics.NumberOfUnits + baseCharacteristics.NumberOfUnitsUpgrade.Modifier*unitUpgrades.NumberOfUnits ) * supplyMultiplier;
        Health = baseCharacteristics.Characteristics.Health + baseCharacteristics.HealthUpgrade.Modifier*unitUpgrades.Health;
        Damage = baseCharacteristics.Characteristics.Damage + baseCharacteristics.DamageUpgrade.Modifier*unitUpgrades.Damage;
        Initiative = baseCharacteristics.Characteristics.Initiative + baseCharacteristics.InitiativeUpgrade.Modifier*unitUpgrades.Initiative;
        Cohesion = baseCharacteristics.Characteristics.Cohesion + baseCharacteristics.CohesionUpgrade.Modifier*unitUpgrades.Cohesion;
        Armour = baseCharacteristics.Characteristics.Armour + baseCharacteristics.ArmourUpgrade.Modifier*unitUpgrades.Armour;
        
    }
}
[System.Serializable]
public struct UnitUpgrades
{
    public int NumberOfUnits; public int Health; public int Damage; public int Initiative; public int Cohesion; public int Armour;
    UnitUpgrades(int numberOfUnits, int health, int damage, int initiative, int cohesion, int armour)
    {
        NumberOfUnits=numberOfUnits;Health=health;Damage=damage;Initiative=initiative;Cohesion=cohesion; Armour=armour;
    }
}

public class UnitBuff
{
    public UnitCharacteristics Buff; public GameObject BuffParent; public int BuffTurns;
    public UnitBuff(UnitCharacteristics buff, GameObject buffParent, int buffTurns)
    {
        Buff = buff;
        BuffParent = buffParent;
        BuffTurns = buffTurns;
    }
}

public class ArmyUnitClass : MonoBehaviour
{
    public BaseUnitCharacteristics FactoryCharacteristics;
    public UnitCharacteristics BaseCharacteristics;
    public UnitCharacteristics CurrentUnitCharacteristics;
    public int currentSquadHealth;
    public float currentUnitEffectiveness;
    public UnitUpgrades unitUpgrades;
    public List<UnitAbility> Abilities = new List<UnitAbility>();
    public List<UnitBuff> Buffs;
    public List<AbilityTags> UnitAbilityTags;
    public string UnitName;
    public ListOfCommonUnits UnitFactory;
    public int SupplyMultiplier;
    
    //-----------Initialization logic
    public void InitializeUnit(string unitName, UnitUpgrades upgrades)
    {
        FactoryCharacteristics = UnitFactory.UnitList.Find(x => x.UnitType.Equals(unitName));
        UnitName = unitName;
        foreach (var ability in FactoryCharacteristics.UnitAbilities)
        {
            Abilities.Add(ability());
        }
        //TODO TEMP solution dnt know what to do
        UpdateUnitTags();
        Buffs = new List<UnitBuff>();
        SupplyMultiplier = 1;
        currentUnitEffectiveness = 1;
        unitUpgrades = upgrades;
        
        RebuildCharacteristics();
    }
    public void RebuildCharacteristics() // Revert to baseline
    {
        CurrentUnitCharacteristics = new UnitCharacteristics(FactoryCharacteristics,unitUpgrades, SupplyMultiplier);
        BaseCharacteristics = new UnitCharacteristics(FactoryCharacteristics, unitUpgrades, Mathf.Clamp(SupplyMultiplier,1,999));
        currentSquadHealth = CurrentUnitCharacteristics.NumberOfUnits *
                             CurrentUnitCharacteristics.Health;
    }
    //-----------Abilities logic
    public void InitializeAbilities(Company newCompany,FormationField field,FormationField opposingField)
    {
        foreach (var ability in Abilities)
        {
            ability.InitAbility(newCompany,field,opposingField);
        }
    }
    public void InitializeAbilities(Company newCompany)
    {
        foreach (var ability in Abilities)
        {
            ability.ChangeCompany(newCompany);
        }
    }

    [CanBeNull]
    public UnitAbility GetPossibleAbility() //used to get ability that would be used by this unit next round
    {
        UnitAbility activeAbility = null;
        foreach (var ability in Abilities)
        {
            if (ability.SelectTargets()) return ability;
        }
        return activeAbility;
    }
    [CanBeNull]
    public UnitAbility GetRetaliationAbility(List<AbilityTags> retaliationTags) //used to get ability to retaliate attack
    {
        UnitAbility retaliationAbility = null;
        foreach (var ability in Abilities)
        {
            foreach (var tag in retaliationTags)
            {
                if (ability.Tags.Contains(tag)) return ability;
            }
        }
        return retaliationAbility;
    }
    
    //-----------Unit stats logic
    public void OnRoundEnd() //Must be called at the end of the round
    {
        currentUnitEffectiveness = 1;
        CheckBuffs();
        ApplyBuffs();
    }
    public void OnBattleEnd() //Must be called at the end of the battle
    {
        currentUnitEffectiveness = 1;
        Buffs.Clear();
        RebuildCharacteristics();
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
        var bc = BaseCharacteristics;
        var cur = CurrentUnitCharacteristics;
        cur.Damage = bc.Damage;
        cur.Initiative = bc.Initiative;
        cur.Cohesion = bc.Cohesion;
        cur.Armour = bc.Armour;
        foreach (var buff in Buffs)
        {
            //TODO Figure out what to do with NumberOfUnits and Health Buffs
            
            //cur.NumberOfUnits = cur.NumberOfUnits + buff.Buff.NumberOfUnits;
            //cur.Health = cur.Health + buff.Buff.Health;
            cur.Damage = cur.Damage + buff.Buff.Damage;
            cur.Initiative = cur.Initiative + buff.Buff.Initiative;
            cur.Cohesion = cur.Cohesion + buff.Buff.Cohesion;
            cur.Armour = cur.Armour + buff.Buff.Armour;
        }
    }
    //-----------Unit stats logic
    public void UpdateSupply(int[] supply)
    {
        int newSupplyMultiplier = 999;
        for (int i = 0;i<4;i++ )
        {
            if (FactoryCharacteristics.UnitSupplyReq[i]==0) continue;
            var x = Mathf.CeilToInt(supply[i] / FactoryCharacteristics.UnitSupplyReq[i]);
            //Debug.Log(UnitName+" "+i+"+"+supply[0]+supply[1]+supply[2]+supply[3]+"|"+FactoryCharacteristics.UnitSupplyReq[0]+FactoryCharacteristics.UnitSupplyReq[1]+FactoryCharacteristics.UnitSupplyReq[2]+FactoryCharacteristics.UnitSupplyReq[3]+"|"+x);
            x = Mathf.Clamp(x, 0, 999);
            if (x < newSupplyMultiplier) newSupplyMultiplier = x;
            //Debug.Log(newSupplyMultiplier);
        }
        if (newSupplyMultiplier == 999) newSupplyMultiplier = 0;
        
        SupplyMultiplier = newSupplyMultiplier;
        RebuildCharacteristics();
    }
    public bool TakeDamage((int, int) statsToChange)
    {
        bool isAlive = true;
        currentSquadHealth = statsToChange.Item1;
        CurrentUnitCharacteristics.NumberOfUnits = statsToChange.Item2;
        // TODO is it ok? maybe check for NumberOfUnits >0
        if (currentSquadHealth <= 0) isAlive = false;
//        Debug.Log($"{UnitName} HP:{currentSquadHealth} N:{CurrentUnitCharacteristics.NumberOfUnits} IA:{isAlive.ToString()}");
        return isAlive;
    }

    public void UpdateEffectiveness(int engagedUnits)
    {
        // TODO make units get out of the field so that do not happaned
        if (CurrentUnitCharacteristics.NumberOfUnits>0)
            currentUnitEffectiveness = Mathf.Clamp(currentUnitEffectiveness - (engagedUnits/CurrentUnitCharacteristics.NumberOfUnits)*0.75f, 0, 1);
    }

    private void UpdateUnitTags()
    {
        foreach (var ability in Abilities)
        {
            UnitAbilityTags.AddRange(ability.Tags);
        }
    }
}
