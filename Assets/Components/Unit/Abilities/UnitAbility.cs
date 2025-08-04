using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using AbilityFactory = System.Func<UnitAbility>;

[System.Serializable]
public enum AbilityTags { Melee, Ranged, MeleeRetaliation, RangedRetaliation, Mounted  };


[System.Serializable]
public abstract class UnitAbility
{
    public string AbilityName = "Default ability name";
    public string AbilityDescription = "Default ability description";
    public List<(int X, int Y)> targets = new List<(int X, int Y)>();
    public List<AbilityTags> Tags = new List<AbilityTags>();
    public List<AbilityTags> RetaliationTags = new List<AbilityTags>();
    public Company UnitCompany;
    public FormationManager formation;
    public float AbilityDamageModifier;
    public int InitiativeModifier = 0;
    public int MoveSpeed = 0;
    public int MoveDirection = 1;
    public abstract bool SelectTargets();
    public abstract bool Edvance();
    public (int,int,int) GetAbilityImpact(Company retaliationCompany = null)
    {
        /*
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
        */
        //TODO TEMP
        return (0, 0, 0);
    }
    public void AssignTargetForRetaliation(Company comp)
    {
        targets.Clear();
        targets.Add(comp.Position);
    }
    public void InitAbility(Company unitCompany, FormationManager formationManager)
    {
        UnitCompany = unitCompany;
        formation = formationManager;
    }
    public void ChangeCompany(Company unitCompany)
    {
        UnitCompany = unitCompany;
    }
    
    public (int newSquadHealth, int newNumberOfUnits) CalculateDamage(ArmyUnitClass unit,int incdamage, int incunitdmg, string enemyunitname)
    {
        /*
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
        */
        //TODO TEMP
        return (0,0);
    }
    public List<(int X, int Y)> GetAbilityTargets()
    {
        return targets;
    }
    
    public bool TryGetCompanyTargets(out List<Company> list)
    {
        //TODO You can target your own troops?
        List<Company> comps = new List<Company>();
        bool result = false;
        foreach (var pos in targets)
        {
            var newcomp = formation.FormationGrid.GetCompanyAt(pos.X, pos.Y);
            if (newcomp != UnitCompany && newcomp != null && newcomp?.Unit != null)
            {
                comps.Add(newcomp);
                result = true;
            }
        }
        list = comps;
        return result;
    }

    public List<Company> GetPossibleTargets()
    {
        List<Hero> EnemyHero = new List<Hero>();
        if (UnitCompany.unitOwner==formation.EnemyHero) EnemyHero.Add(formation.PlayerHero);
        else EnemyHero.Add(formation.EnemyHero);
        return formation.FormationGrid.GetNeighbors(UnitCompany, EnemyHero);
    }

    public (int,int) GetMoveTarget()
    {
        
        List<Hero> AllHeroes = new List<Hero>(){formation.EnemyHero,formation.PlayerHero};
        var origPos = UnitCompany.Position;
        (int,int) taget = origPos;
        if (MoveSpeed > 0)
        {
            int adjustedMoveDirection = MoveDirection;
            if (UnitCompany.unitOwner == formation.EnemyHero) adjustedMoveDirection = -MoveDirection;
            for (int i = 0; i < MoveSpeed; i++)
            {
                int newPosX = origPos.x;
                int newPosY = origPos.y + (MoveSpeed - i) * adjustedMoveDirection;
                Company compInTheWay = formation.FormationGrid.GetCompanyAt(newPosX, newPosY);
                if (compInTheWay == null || compInTheWay?.Unit==null)
                {
                    taget=(newPosX,newPosY);
                    //Debug.Log($"{UnitCompany.Unit.name} POS {UnitCompany.Position} GetMoveTarget at {newPosX}:{newPosY}");
                    return taget;
                }
            }
        }
        return taget;
    }

    public bool Move()
    {
        bool isMoved = false;
        int adjustedMoveDirection = MoveDirection;
        if (UnitCompany.unitOwner == formation.EnemyHero) adjustedMoveDirection = -MoveDirection;
        for (int i = 0; i < MoveSpeed; i++)
        {
            if (UnitCompany.MoveCompany(UnitCompany.X, UnitCompany.Y + adjustedMoveDirection, formation.FormationGrid))
            {
                isMoved=true;
                if (UnitCompany.Position==targets[0]) break;
            }
            else break;
        }
        return isMoved;
    }
}
