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
    public UnitCharacteristics(BaseUnitCharacteristics baseCharacteristics ,UnitUpgrades unitUpgrades)
    {
        NumberOfUnits = baseCharacteristics.Characteristics.NumberOfUnits + baseCharacteristics.NumberOfUnitsUpgrade.Modifier*unitUpgrades.NumberOfUnits;
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
public class ArmyUnitClass : MonoBehaviour
{
    public BaseUnitCharacteristics DefaultUnitCharacteristics;
    public UnitCharacteristics CurrentUnitCharacteristics;
    public int currentsquadhealth;
    public UnitUpgrades unitUpgrades;
    public List<UnitAbility> Abilities;
    public string UnitName;
    public ListOfCommonUnits BaseLineCharacteristics;

    public void InitializeUnit(string unitName, Race unitRace, UnitUpgrades upgrades)
    {
        DefaultUnitCharacteristics = BaseLineCharacteristics.UnitList.Find(x => x.UnitType.Equals(unitName));
        UnitName = unitName;
        unitRace = unitRace;
        currentsquadhealth = DefaultUnitCharacteristics.Characteristics.NumberOfUnits *
                             DefaultUnitCharacteristics.Characteristics.Health;
        //TODO Заплатка!!!
        List<UnitAbility> UnitAbilities = new List<UnitAbility>();
        UnitAbilities.Add(new UnitBasicAttack());
        Abilities = UnitAbilities;
        //Abilities = DefaultUnitCharacteristics.UnitAbilities;
        
        unitUpgrades = upgrades;
        UpdateUnitCharacteristics(upgrades);
    }
    public void UpdateUnitCharacteristics(UnitUpgrades upgrades)
    {
        CurrentUnitCharacteristics = new UnitCharacteristics(DefaultUnitCharacteristics,upgrades);
    }
    public void ApplyHeroModifyers(int hminitiative, int hmcohesion) { CurrentUnitCharacteristics.Initiative = CurrentUnitCharacteristics.Initiative + hminitiative; CurrentUnitCharacteristics.Cohesion = CurrentUnitCharacteristics.Cohesion + hmcohesion; }
    
}
