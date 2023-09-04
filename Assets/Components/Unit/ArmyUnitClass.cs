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
    public struct NumberOfUnits { public int startnumberofunits; public int currentnumberofunits; public int regennumberofunits; }
    public struct ArmyUnitHealth { public int startunithealth; public int currentunithealth; public int regenunithealth; public int startsquadhealth; public int currentsquadhealth; }
    public struct UnitDamage { public int startunitdamage; public int currentunitdamage; }
    public struct UnitStats { public int startinitiative; public int currentinitiative; public int startcohesion; public int currentcohesion; public int startarmour; public int currentarmour; }
    public BaseUnitCharacteristics DefaultUnitCharacteristics;
    public UnitCharacteristics CurrentUnitCharacteristics;
    public NumberOfUnits numberofunits; public ArmyUnitHealth armyunithealth; public UnitDamage unitdamage; public UnitStats unitstats;
    public UnitUpgrades unitUpgrades;
    public List<UnitAbility> Abilities;
    public Race unitRace;
    public string UnitName;
    string unitnameprefix;
    bool isranged = false;
    bool iscavalary = false;
    
    public ListOfCommonUnits BaseLineCharacteristics;

    public void InitializeUnit(string unitName, Race unitRace, UnitUpgrades upgrades)
    {
        DefaultUnitCharacteristics = BaseLineCharacteristics.UnitList.Find(x => x.UnitType.Equals(unitName));
        UnitName = unitName;
        unitRace = unitRace;
        Abilities = DefaultUnitCharacteristics.UnitAbilities;
        unitUpgrades = upgrades;
        UpdateUnitCharacteristics(upgrades);
    }
    public void UpdateUnitCharacteristics(UnitUpgrades upgrades)
    {
        CurrentUnitCharacteristics = new UnitCharacteristics(DefaultUnitCharacteristics,upgrades);
    }

    public string Getinfo()
    {
        string main_text = "";
        main_text = $"{unitnameprefix}{UnitName} squad. There are {numberofunits.currentnumberofunits} of them, unit health:{armyunithealth.currentunithealth}, damage:{unitdamage.currentunitdamage}.";
        if (unitstats.currentarmour > 0) { main_text += $" Armour hardened by {unitstats.currentarmour}."; }
        main_text += $" Total squad health:{armyunithealth.currentsquadhealth}, initiative:{unitstats.currentinitiative}, cohesion:{unitstats.currentcohesion}.";
        if (unitdamage.currentunitdamage < 4) { main_text += " A pitiful sight..."; } else main_text = main_text + " Fine...";
        return main_text;
    }

    public int GetUnitHP() { return armyunithealth.currentsquadhealth; }
    public (UnitCharacteristics,UnitUpgrades) GetUnitCharacteristics()
    {
        var unitchar = new UnitCharacteristics(numberofunits.currentnumberofunits, armyunithealth.currentunithealth, unitdamage.currentunitdamage, unitstats.currentinitiative, unitstats.currentcohesion,unitstats.currentarmour);
        return (unitchar,unitUpgrades);
    }
    public void ApplyHeroModifyers(int hminitiative, int hmcohesion) { unitstats.currentinitiative = unitstats.startinitiative + hminitiative; unitstats.currentcohesion = unitstats.startcohesion + hmcohesion; }
    public int GetAlldamage() { return numberofunits.currentnumberofunits * unitdamage.currentunitdamage; } //Метод получения общего урона отряда
    //public int GetUnitdamage() { return unitdamage.currentunitdamage; } //Метод получения урона одного юнита
    public string GetUnitName() { return UnitName; } //Метод получения урона одного юнита

    public NumberOfUnits GetNumberOfUnits() { return numberofunits; } //Метод всех количеств юнита
    public ArmyUnitHealth GetArmyUnitHealth() { return armyunithealth; } //Метод параметров жизни юнита
    public UnitDamage GetUnitDamage() { return unitdamage; } //Метод параметров урона юнита
    public UnitStats GetUnitStats() { return unitstats; } //Метод параметров урона юнита
    
}
