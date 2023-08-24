using System;
using UnityEngine;

[System.Serializable]
public struct UnitCharacteristics 
{
    public int ucnumberofunits; public int ucunithealth; public int ucunitdamage; public int ucunitinitiative; public int ucunitcohesion; public int unitarmour;
    public UnitCharacteristics(int num, int h, int d, int init, int coh, int armour)
    {
        ucnumberofunits = num; ucunithealth = h; ucunitdamage = d; ucunitinitiative = init; ; ucunitcohesion = coh; unitarmour = armour;
    }
}
[System.Serializable]
public struct CountUnitUpgrades
{
    public int chealth; public int cnumber; public int cdamage; public int cinitiative; public int ccohesion; public int carmour;
    CountUnitUpgrades(int h, int n, int d, int i, int c, int a)
    {
        chealth=h;cnumber=n;cdamage=d;cinitiative=i;ccohesion=c; carmour=a;
    }
}
public class ArmyUnitClass : MonoBehaviour
{
    public struct NumberOfUnits { public int startnumberofunits; public int currentnumberofunits; public int regennumberofunits; }
    public struct ArmyUnitHealth { public int startunithealth; public int currentunithealth; public int regenunithealth; public int startsquadhealth; public int currentsquadhealth; }
    public struct UnitDamage { public int startunitdamage; public int currentunitdamage; }
    public struct UnitStats { public int startinitiative; public int currentinitiative; public int startcohesion; public int currentcohesion; public int startarmour; public int currentarmour; }
    public UnitCharacteristics BaseUnitCharacteristics;
    public UnitCharacteristics CurrentUnitCharacteristics;
    public NumberOfUnits numberofunits; public ArmyUnitHealth armyunithealth; public UnitDamage unitdamage; public UnitStats unitstats; public CountUnitUpgrades unitUpgrades;
    public int[] battletarget = new int[] { 0, 0 };
    
    public Race UnitRace;
    public string UnitName;
    string unitnameprefix;
    bool isranged = false;
    bool iscavalary = false;
    
    public ListOfCommonUnits BaseLineCharacteristics;

    public void UpdateUnitCharacteristics(string unitName, Race unitRace, UnitCharacteristics uchar,CountUnitUpgrades cupgrd)
    {
        BaseUnitCharacteristics = BaseLineCharacteristics.UnitList.Find(x => x.UnitType.Equals(unitName)).Characteristics;
        UnitName = unitName;
        UnitRace = unitRace;
        CurrentUnitCharacteristics = uchar;
        unitUpgrades = cupgrd;
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
    public (UnitCharacteristics,CountUnitUpgrades) GetUnitCharacteristics()
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
