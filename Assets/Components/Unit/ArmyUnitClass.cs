using System;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Все дебафы хранят накладывающие объекты со ссылкой на получателя
/// </summary>
[System.Serializable]
public struct UnitCharacteristics 
{
    public int NumberOfUnits; public int Health; public int Damage; public int Initiative; public int Cohesion; public int Armour;
    public UnitCharacteristics(int num, int h, int d, int init, int coh, int armour)
    {
        NumberOfUnits = num; Health = h; Damage = d; Initiative = init; ; Cohesion = coh; Armour = armour;
    }
    public UnitCharacteristics(BaseUnitCharacteristics baseCharacteristics ,UnitUpgrades unitUpgrades, List<UnitBuff> buffs)
    {
        NumberOfUnits = baseCharacteristics.Characteristics.NumberOfUnits + baseCharacteristics.NumberOfUnitsUpgrade.Modifier*unitUpgrades.NumberOfUnits;
        Health = baseCharacteristics.Characteristics.Health + baseCharacteristics.HealthUpgrade.Modifier*unitUpgrades.Health;
        Damage = baseCharacteristics.Characteristics.Damage + baseCharacteristics.DamageUpgrade.Modifier*unitUpgrades.Damage;
        Initiative = baseCharacteristics.Characteristics.Initiative + baseCharacteristics.InitiativeUpgrade.Modifier*unitUpgrades.Initiative;
        Cohesion = baseCharacteristics.Characteristics.Cohesion + baseCharacteristics.CohesionUpgrade.Modifier*unitUpgrades.Cohesion;
        Armour = baseCharacteristics.Characteristics.Armour + baseCharacteristics.ArmourUpgrade.Modifier*unitUpgrades.Armour;
        foreach (var buff in buffs)
        {
            NumberOfUnits = NumberOfUnits + buff.Buff.NumberOfUnits;
            Health = Health + buff.Buff.Health;
            Damage = Damage + buff.Buff.Damage;
            Initiative = Initiative + buff.Buff.Initiative;
            Cohesion = Cohesion + buff.Buff.Cohesion;
            Armour = Armour + buff.Buff.Armour;
        }
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

public struct UnitBuff
{
    public UnitCharacteristics Buff; public GameObject BuffParent; public int BuffTurns;
    UnitBuff(UnitCharacteristics buff, GameObject buffParent, int buffTurns)
    {
        Buff = buff;
        BuffParent = buffParent;
        BuffTurns = buffTurns;
    }
}

public class ArmyUnitClass : MonoBehaviour
{
    public BaseUnitCharacteristics DefaultUnitCharacteristics;
    public UnitCharacteristics CurrentUnitCharacteristics;
    public int currentSquadHealth;
    public UnitUpgrades unitUpgrades;
    public List<UnitAbility> Abilities;
    public List<UnitBuff> Buffs;
    public string UnitName;
    public ListOfCommonUnits UnitFactory;

    public void InitializeUnit(string unitName, Race unitRace, UnitUpgrades upgrades)
    {
        DefaultUnitCharacteristics = UnitFactory.UnitList.Find(x => x.UnitType.Equals(unitName));
        UnitName = unitName;
        unitRace = unitRace;
        currentSquadHealth = DefaultUnitCharacteristics.Characteristics.NumberOfUnits *
                             DefaultUnitCharacteristics.Characteristics.Health;
        //TODO Заплатка!!!
        List<UnitAbility> UnitAbilities = new List<UnitAbility>();
        UnitAbilities.Add(new UnitBasicAttack());
        Abilities = UnitAbilities;
        //Abilities = DefaultUnitCharacteristics.UnitAbilities;
        
        unitUpgrades = upgrades;
        Buffs = new List<UnitBuff>();
        UpdateUnitCharacteristics(upgrades,Buffs);
    }
    public void UpdateUnitCharacteristics(UnitUpgrades upgrades,List<UnitBuff> buffs)
    {
        CurrentUnitCharacteristics = new UnitCharacteristics(DefaultUnitCharacteristics,upgrades,buffs);
    }
    public void ApplyHeroModifyers(int hminitiative, int hmcohesion) { CurrentUnitCharacteristics.Initiative = CurrentUnitCharacteristics.Initiative + hminitiative; CurrentUnitCharacteristics.Cohesion = CurrentUnitCharacteristics.Cohesion + hmcohesion; }
    
}
