using System;
using UnityEngine;

public struct UnitCharacteristics 
{
    public string ucunitname; public int ucnumberofunits; public int ucunithealth; public int ucunitdamage; public int ucunitinitiative; public int ucunitcohesion; public int unitarmour;
    public UnitCharacteristics(string n, int num, int h, int d, int init, int coh, int armour) 
    {
        ucunitname = n;ucnumberofunits = num; ucunithealth = h; ucunitdamage = d; ucunitinitiative = init; ; ucunitcohesion = coh; unitarmour = armour;
    }
}
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

    public NumberOfUnits numberofunits; public ArmyUnitHealth armyunithealth; public UnitDamage unitdamage; public UnitStats unitstats; public CountUnitUpgrades unitUpgrades;

    public int[] battletarget = new int[] { 0, 0 };
    private bool ismelee = true;
    private bool isranged = false;
    private bool iscavalary = false;
    public string armytype = "infantry";
    public string unitname;
    public string unitnameprefix;

    public ArmyUnitClass(string n, int num, int h, int d, int init, int coh, int regen, int armour)
    {
        unitname = n;
        numberofunits.startnumberofunits = numberofunits.currentnumberofunits = num; numberofunits.regennumberofunits = regen;
        armyunithealth.startunithealth = armyunithealth.currentunithealth = h; armyunithealth.regenunithealth = 0;
        armyunithealth.startsquadhealth = armyunithealth.startunithealth * numberofunits.startnumberofunits; armyunithealth.currentsquadhealth = armyunithealth.startsquadhealth;
        unitdamage.startunitdamage = unitdamage.currentunitdamage = d;
        unitstats.startinitiative = unitstats.currentinitiative = init; unitstats.startcohesion = unitstats.currentcohesion = coh;
        unitstats.startarmour = unitstats.currentarmour = armour;
        if (coh < 0 & numberofunits.currentnumberofunits > 1) { unitnameprefix = "Rabble of "; }
    }
    public ArmyUnitClass(UnitCharacteristics uchar, CountUnitUpgrades uupgrd)
    {
        unitname = uchar.ucunitname;
        numberofunits.startnumberofunits = numberofunits.currentnumberofunits = uchar.ucnumberofunits;
        armyunithealth.startunithealth = armyunithealth.currentunithealth = uchar.ucunithealth; armyunithealth.regenunithealth = 0;
        armyunithealth.startsquadhealth = armyunithealth.currentsquadhealth = armyunithealth.startunithealth * numberofunits.startnumberofunits;
        unitdamage.startunitdamage = unitdamage.currentunitdamage = uchar.ucunitdamage;
        unitstats.startinitiative = unitstats.currentinitiative = uchar.ucunitinitiative; unitstats.startcohesion = unitstats.currentcohesion = uchar.ucunitcohesion;
        unitstats.startarmour = unitstats.currentarmour = uchar.unitarmour;
        unitUpgrades = uupgrd;
        if (uchar.ucunitcohesion < 0 & numberofunits.currentnumberofunits > 1) { unitnameprefix = "Rabble of "; }
    }
    public string Getinfo()
    {
        string main_text = "";
        main_text = $"{unitnameprefix}{unitname} {armytype} squad. There are {numberofunits.currentnumberofunits} of them, unit health:{armyunithealth.currentunithealth}, damage:{unitdamage.currentunitdamage}.";
        if (unitstats.currentarmour > 0) { main_text += $" Armour hardened by {unitstats.currentarmour}."; }
        main_text += $" Total squad health:{armyunithealth.currentsquadhealth}, initiative:{unitstats.currentinitiative}, cohesion:{unitstats.currentcohesion}.";
        if (unitdamage.currentunitdamage < 4) { main_text += " A pitiful sight..."; } else main_text = main_text + " Fine...";
        return main_text;
    }

    public int GetUnitHP() { return armyunithealth.currentsquadhealth; }
    public (UnitCharacteristics,CountUnitUpgrades) GetUnitCharacteristics()
    {
        var unitchar = new UnitCharacteristics(unitname, numberofunits.currentnumberofunits, armyunithealth.currentunithealth, unitdamage.currentunitdamage, unitstats.currentinitiative, unitstats.currentcohesion,unitstats.currentarmour);
        return (unitchar,unitUpgrades);
    }
    public void ApplyHeroModifyers(int hminitiative, int hmcohesion) { unitstats.currentinitiative = unitstats.startinitiative + hminitiative; unitstats.currentcohesion = unitstats.startcohesion + hmcohesion; }
    public int GetAlldamage() { return numberofunits.currentnumberofunits * unitdamage.currentunitdamage; } //Метод получения общего урона отряда
    //public int GetUnitdamage() { return unitdamage.currentunitdamage; } //Метод получения урона одного юнита
    public string GetUnitName() { return unitname; } //Метод получения урона одного юнита

    public NumberOfUnits GetNumberOfUnits() { return numberofunits; } //Метод всех количеств юнита
    public ArmyUnitHealth GetArmyUnitHealth() { return armyunithealth; } //Метод параметров жизни юнита
    public UnitDamage GetUnitDamage() { return unitdamage; } //Метод параметров урона юнита
    public UnitStats GetUnitStats() { return unitstats; } //Метод параметров урона юнита
    
}
