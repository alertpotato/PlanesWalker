using System;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable] 
public enum AbilityTags { Melee, Ranged, MeleeRetaliation, RangedRetaliation, Mounted  };
[System.Serializable] 
public enum Abilities { MeleeCombat, ArrowVolley, MountedCharge, CowardlyAttack, SuppressiveFire, KnightlyFeat};
[Serializable] 
public abstract class UnitAbility
{
    public string AbilityName = "Default ability name";
    public List<(int,int)> targets = new List<(int,int)>();
    public List<AbilityTags> Tags = new List<AbilityTags>();
    public List<AbilityTags> RetaliationTags = new List<AbilityTags>();
    public Company UnitCompany;
    public FormationField UnitField;
    public FormationField OpposingField;
    public float AbilityDamageModifier;
    public abstract bool SelectTargets();
    public (int,int,int) GetAbilityImpact()
    {
        //TODO temp solution for targets[0] - figure out what to do here - maybe predifined list?
        var opposingUnit = OpposingField.Formation[targets[0].Item1].Line[targets[0].Item2];
        var yourUnit = UnitCompany.Unit.GetComponent<ArmyUnitClass>();
        var enemyUnit = opposingUnit.Unit.GetComponent<ArmyUnitClass>();
        var yourUnitDamage = yourUnit.CurrentUnitCharacteristics.Damage;
        //TODO implement better armour solution
        int yourUnitAllDamage = (int)(yourUnit.CurrentUnitCharacteristics.NumberOfUnits * (Mathf.Clamp(yourUnitDamage-enemyUnit.CurrentUnitCharacteristics.Armour,0,yourUnitDamage)) * yourUnit.currentUnitEffectiveness * AbilityDamageModifier);
        
        //Update currentUnitEffectiveness and calculate true damage based on numbers disparity
        var engagedUnitNumber = yourUnit.CurrentUnitCharacteristics.NumberOfUnits;
        var enemyUnitNumber = enemyUnit.CurrentUnitCharacteristics.NumberOfUnits;
        if (engagedUnitNumber > enemyUnitNumber * 3)
        {
            yourUnitAllDamage = (int)(enemyUnitNumber * 3 * yourUnitDamage * yourUnit.currentUnitEffectiveness);
            engagedUnitNumber = enemyUnitNumber * 3;
        }
        var calculationResult = CalculateDamage(enemyUnit,yourUnitAllDamage,yourUnitDamage,yourUnit.name);

        return (calculationResult.Item1,calculationResult.Item2,engagedUnitNumber);
    }
    public void AssignTargetForRetaliation((int, int) banner)
    {
        targets.Clear();
        targets.Add(banner);
    }
    public void InitAbility(Company unitCompany, FormationField unitField,FormationField opposingField)
    {
        UnitCompany = unitCompany;
        UnitField = unitField;
        OpposingField = opposingField;
    }
    public void ChangeCompany(Company unitCompany)
    {
        UnitCompany = unitCompany;
    }
    
    public (int newSquadHealth, int newNumberOfUnits) CalculateDamage(ArmyUnitClass unit,int incdamage, int incunitdmg, string enemyunitname)
    {
        // Local Variables
        int cohdamage = Mathf.Clamp(incunitdmg/unit.CurrentUnitCharacteristics.Health - 1,0,20); // for each time opposing unit attack is higher - lose 1 coh in that engagement
        int desserts = 0; 
        string additionallog = "";
        double decnumberof = unit.CurrentUnitCharacteristics.NumberOfUnits;
        int startnumberof = unit.CurrentUnitCharacteristics.NumberOfUnits;   //Запомнили предыдущее количества для вывода статистики боя
        double newnumberof = startnumberof;     //Расчет нового количества
        
        // Precalculation
        double maxTotalHP = unit.CurrentUnitCharacteristics.NumberOfUnits * unit.CurrentUnitCharacteristics.Health;    // Max possible total company HP
        double totalHPAfterIncDamage = Mathf.Clamp(unit.currentSquadHealth - incdamage,0,unit.currentSquadHealth); // Squad health after taking damage
        double maxTotalIncDamage = maxTotalHP - totalHPAfterIncDamage;    // Max possible damage taken as if we are full health
        
        if (totalHPAfterIncDamage<=0) return (0,0) ;
        //OLD COHDAMAGE
        //if (incunitdmg >= unit.CurrentUnitCharacteristics.Health * 2) { cohdamage = (int)Math.Log(incunitdmg / unit.CurrentUnitCharacteristics.Health, 2); }
        //else if (unit.CurrentUnitCharacteristics.Health >= incunitdmg * 2) { cohdamage = -(int)Math.Log(unit.CurrentUnitCharacteristics.Health / incunitdmg, 2); };
        
        double trueCohesion = Mathf.Clamp(unit.CurrentUnitCharacteristics.Cohesion - cohdamage,-10,10);//new cohesion after cohdamage
        
        if (unit.CurrentUnitCharacteristics.Cohesion >= 0 && trueCohesion < 0) { additionallog = " The ranks wavered"; };
        
        if (trueCohesion >= 0)
        {
            newnumberof = Math.Ceiling(decnumberof - decnumberof * Math.Pow(maxTotalIncDamage / maxTotalHP, trueCohesion + 1));
        }
        if (trueCohesion < 0)
        {
            newnumberof = Math.Ceiling(decnumberof - decnumberof * Math.Pow(maxTotalIncDamage / maxTotalHP, 1 + trueCohesion / 10));
        }
        
        if (newnumberof > totalHPAfterIncDamage)
        {
            //TODO figure out if its possible and why
            Debug.LogWarning("newnumberof > totalHPAfterIncDamage CHECK IT OUT");
            //additionallog = additionallog + $" {unit.CurrentUnitCharacteristics.NumberOfUnits - totalHPAfterIncDamage} units withstand before the death's door!";
            //newnumberof = unit.currentSquadHealth;
        }
        
        if (totalHPAfterIncDamage > newnumberof * unit.CurrentUnitCharacteristics.Health) //Проверяем, что хп отряда не больше максимального. Если да, значит часть войска сбежало с поле боя.
        {
            //desserts = (takedmg.armyunithealth.currentsquadhealth - (takedmg.numberofunits.currentnumberofunits * takedmg.armyunithealth.currentunithealth)) / takedmg.armyunithealth.currentunithealth; additionallog = additionallog + $" {desserts} cowards escaped from the battlefield.";
            //TODO That is definitely stupid, i just need to make IncDamage recalculation then coh<0
            desserts = (startnumberof - (int)newnumberof) - (int)Math.Floor((maxTotalHP - totalHPAfterIncDamage) / unit.CurrentUnitCharacteristics.Health);
            additionallog = additionallog + $" {desserts} cowards escaped from the battlefield.";
            totalHPAfterIncDamage = totalHPAfterIncDamage - desserts * unit.CurrentUnitCharacteristics.Health; //Console.WriteLine($"if squadhealth > mxhp:{decmaxhealth} sqdhp:{squadhealth} coh:{truecohesion} num:{numberof}");  //Дебаг
        }
        
        var newSquadHealth = (int)totalHPAfterIncDamage;
        var newNumberOfUnits = (int)newnumberof;
        if (newSquadHealth <= 0) newNumberOfUnits = 0;
        string battlelog = $"The {unit.UnitName} c:{trueCohesion} taken {incdamage} d from {enemyunitname} with '{AbilityName}'. {newnumberof}/{startnumberof} with {totalHPAfterIncDamage} hp.";
        //Debug.Log(battlelog + additionallog);
        
        return (newSquadHealth,newNumberOfUnits );
    }
    public List<(int,int)> GetAbilityTargets()
    {
        foreach (var target in targets)
        {
            //Debug.Log($"{UnitSquad.Unit.name}({target[0]} {target[1]}) attacks {OpposingHero.ArmyFormation[target[0]].ArmyLine[target[1]].Unit.name}");
        }
        return targets;
    }
}
