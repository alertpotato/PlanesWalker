using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using AbilityFactory = System.Func<UnitAbility>;

[System.Serializable]
public enum AbilityTags { Melee, Ranged, MeleeRetaliation, RangedRetaliation, Mounted  };


[Serializable]
public abstract class UnitAbility
{
    public string AbilityName = "Default ability name";
    public List<Company> targets = new List<Company>();
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
        var opposingUnit = targets[0];
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
    public void AssignTargetForRetaliation(Company comp)
    {
        targets.Clear();
        targets.Add(comp);
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
    public List<Company> GetAbilityTargets()
    {
        return targets;
    }

    public List<Company> GetPossibleTargets()
    {
        var onFieldcompanies = OpposingField.GetOnFieldcompanies();
        List<Company> possibleComp = new List<Company>();
        if (UnitCompany.Type == FormationType.Frontline)
        {
            possibleComp.AddRange(onFieldcompanies.Where(comp => comp.Type==FormationType.Frontline));
            if (UnitCompany.Unit.GetComponent<ArmyUnitClass>().UnitAbilityTags.Contains(AbilityTags.Mounted))
            {
                possibleComp.AddRange(onFieldcompanies.Where(comp => comp.Type==FormationType.Flank1 || comp.Type==FormationType.Flank2));
            }
        }
        
        else if (UnitCompany.Type == FormationType.Flank1)
        {
            possibleComp.AddRange(onFieldcompanies.Where(comp => comp.Type==FormationType.Flank1));
            if (onFieldcompanies.Where(comp => comp.Type == FormationType.Flank1).Count() == 0)
            {
                possibleComp.AddRange(onFieldcompanies.Where(comp => comp.Type==FormationType.Frontline));
                if (UnitCompany.Unit.GetComponent<ArmyUnitClass>().UnitAbilityTags.Contains(AbilityTags.Mounted))
                {
                    possibleComp.AddRange(onFieldcompanies.Where(comp => comp.Type==FormationType.Support));
                }
            }
        }
        else if (UnitCompany.Type == FormationType.Flank2)
        {
            possibleComp.AddRange(onFieldcompanies.Where(comp => comp.Type==FormationType.Flank2));
            if (onFieldcompanies.Where(comp => comp.Type == FormationType.Flank2).Count() == 0)
            {
                possibleComp.AddRange(onFieldcompanies.Where(comp => comp.Type==FormationType.Frontline));
                if (UnitCompany.Unit.GetComponent<ArmyUnitClass>().UnitAbilityTags.Contains(AbilityTags.Mounted))
                {
                    possibleComp.AddRange(onFieldcompanies.Where(comp => comp.Type==FormationType.Support));
                }
            }
        }
        else if (UnitCompany.Type == FormationType.Support)
        {
            if (UnitCompany.Unit.GetComponent<ArmyUnitClass>().UnitAbilityTags.Contains(AbilityTags.Ranged))
            {
                possibleComp.AddRange(onFieldcompanies.Where(comp => comp.Type==FormationType.Frontline));
                possibleComp.AddRange(onFieldcompanies.Where(comp => comp.Type==FormationType.Flank1));
                possibleComp.AddRange(onFieldcompanies.Where(comp => comp.Type==FormationType.Flank2));
            }
            else if (UnitCompany.Unit.GetComponent<ArmyUnitClass>().UnitAbilityTags.Contains(AbilityTags.Mounted))
            {
                possibleComp.AddRange(onFieldcompanies.Where(comp => comp.Type==FormationType.Flank1 || comp.Type==FormationType.Flank2));
                if (onFieldcompanies.Where(comp => comp.Type == FormationType.Flank2).Count() == 0 ||
                    onFieldcompanies.Where(comp => comp.Type == FormationType.Flank1).Count() == 0)
                {
                    possibleComp.AddRange(onFieldcompanies.Where(comp => comp.Type==FormationType.Frontline));
                }
            }
        }
        return possibleComp;
    }
}
