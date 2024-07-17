using System;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class UnitBasicAttack : UnitAbility
{
    public static Abilities UniqueType = Abilities.BasicAttack;
    public UnitBasicAttack()
    {
        AbilityName = "Basic attack";
        Type = AbilityType.Main;
    }
    public override (List<GameObject>,List<GameObject>) MainFunc(bool applyDamage)
    {
        IsActive = false;
        targets.Clear();
        (int,int) unitPosition = UnitCompany.Banner;
        var opposingUnit = OpposingField.Formation[unitPosition.Item1].Line[unitPosition.Item2];
        if (unitPosition.Item1 == 0 && opposingUnit.Type == CompanyType.Occupied)
        {
            targets.Add(unitPosition);
            IsActive = true;
            
            var yourUnit = UnitCompany.Unit.GetComponent<ArmyUnitClass>();
            var enemyUnit = opposingUnit.Unit.GetComponent<ArmyUnitClass>();
            var yourUnitDamage = yourUnit.CurrentUnitCharacteristics.Damage;
            int yourUnitAllDamage = (int)(yourUnit.CurrentUnitCharacteristics.NumberOfUnits * yourUnitDamage * yourUnit.currentUnitEffectiveness);
            var enemyUnitDamage = enemyUnit.CurrentUnitCharacteristics.Damage;
            int enemyUnitAllDamage = (int)(enemyUnit.CurrentUnitCharacteristics.NumberOfUnits * enemyUnitDamage * enemyUnit.currentUnitEffectiveness);
            var yourUnitNumber = yourUnit.CurrentUnitCharacteristics.NumberOfUnits;
            var enemyUnitNumber = enemyUnit.CurrentUnitCharacteristics.NumberOfUnits;
            //Update currentUnitEffectiveness and calculate true damage based on numbers disparity
            if (yourUnitNumber > enemyUnitNumber * 3)
            {
                yourUnitAllDamage = (int)(enemyUnitNumber * 3 * yourUnitDamage * yourUnit.currentUnitEffectiveness);
                yourUnit.UpdateEffectiveness(enemyUnitNumber * 3, applyDamage);
            }
            else yourUnit.UpdateEffectiveness(yourUnitNumber, applyDamage);
            if (enemyUnitNumber > yourUnitNumber * 3)
            {
                enemyUnitAllDamage = (int)(yourUnitNumber * 3 * enemyUnitDamage * enemyUnit.currentUnitEffectiveness);
                enemyUnit.UpdateEffectiveness(yourUnitNumber * 3, applyDamage);
            }
            else enemyUnit.UpdateEffectiveness(enemyUnitNumber, applyDamage);

            TakeDamage(enemyUnit,yourUnitAllDamage,yourUnitDamage,yourUnit.name,applyDamage);
            TakeDamage(yourUnit,enemyUnitAllDamage,enemyUnitDamage,enemyUnit.UnitName,applyDamage);
            //Debug.Log($"{UnitSquad.Unit.name}({unitPosition[0]} {unitPosition[1]}) attack {OpposingHero.ArmyFormation[unitPosition[0]].ArmyLine[unitPosition[1]].Unit.name}");
        }
        return ( new List<GameObject>(), new List<GameObject>());
    }
    public override List<(int,int)> AbilityTargets()
    {
        foreach (var target in targets)
        {
            //Debug.Log($"{UnitSquad.Unit.name}({target[0]} {target[1]}) attacks {OpposingHero.ArmyFormation[target[0]].ArmyLine[target[1]].Unit.name}");
        }
        return targets;
    }
    public void TakeDamage(ArmyUnitClass unit,int incsquaddmg, int incunitdmg, string enemyunitname, bool applyDamage)
    {
        // Local Variables
        int cohdamage = Mathf.Clamp(incunitdmg/unit.CurrentUnitCharacteristics.Health - 1,0,20); // for each time opposing unit attack is higher - lose 1 coh
        int desserts = 0; 
        string additionallog = "";
        double decnumberof = unit.CurrentUnitCharacteristics.NumberOfUnits;
        int startnumberof = unit.CurrentUnitCharacteristics.NumberOfUnits;   //Запомнили предыдущее количества для вывода статистики боя
        double newnumberof = startnumberof;     //Расчет нового количества
        double decmaxhealth = unit.CurrentUnitCharacteristics.NumberOfUnits * unit.CurrentUnitCharacteristics.Health;    //Максимально возможное хп отряда
        int incdamage = incsquaddmg;
        // Precalculation
        double decsquadhealth = Mathf.Clamp(unit.currentSquadHealth - incdamage,0,unit.currentSquadHealth);    //Реальное хп после получения урона
        //Пока отключаем демедж по собранности
        //if (incunitdmg >= unit.CurrentUnitCharacteristics.Health * 2) { cohdamage = (int)Math.Log(incunitdmg / unit.CurrentUnitCharacteristics.Health, 2); }
        //else if (unit.CurrentUnitCharacteristics.Health >= incunitdmg * 2) { cohdamage = -(int)Math.Log(unit.CurrentUnitCharacteristics.Health / incunitdmg, 2); };
        double truecohesion = Mathf.Clamp(unit.CurrentUnitCharacteristics.Cohesion - cohdamage,-10,10);     //Записали сплоченность
        if (unit.CurrentUnitCharacteristics.Cohesion >= 0 && truecohesion < 0) { additionallog = " The ranks wavered"; };
        double decincdamage = decmaxhealth - decsquadhealth;    //Реально полученный урон учитывая макс хп
        
        if (incdamage <= 0) { Debug.LogWarning("Damage 0"); } //Проверяем что входящий урон не 0 и не меньше 0, чтобы не пересчитывать количество отряда при резкой смене сплоченности(и не ломать игру).
        else
        {
            if (truecohesion >= 0)
            {
                //Console.WriteLine("---truecohesion >= 0---"); //Дебаг
                newnumberof = Math.Ceiling(decnumberof - decnumberof * Math.Pow(decincdamage / decmaxhealth, truecohesion + 1));
                //Console.WriteLine($"mxhp:{decmaxhealth} sqdhp:{decsquadhealth} coh:{truecohesion} num:{numberof} decnm:{newnumberof}"); //Дебаг
            }
            if (truecohesion < 0)
            {
                //Console.WriteLine($"---truecohesion < 0---"); //Дебаг
                //debug_pow = Math.Pow(decincdamage / decmaxhealth, 1 + truecohesion / 10);
                //Console.WriteLine($"DEC:{decnumberof} DAM:{decincdamage} MH:{decmaxhealth} COH:{truecohesion} WTF:{debug_pow}"); //Дебаг
                newnumberof = Math.Ceiling(decnumberof - decnumberof * Math.Pow(decincdamage / decmaxhealth, 1 + truecohesion / 10));
                //Console.WriteLine($"mxhp:{decmaxhealth} sqdhp:{decsquadhealth} coh:{truecohesion} num:{unit.numberofunits.currentnumberofunits} decnm:{newnumberof}"); //Дебаг
            }
            
            if (newnumberof > decsquadhealth)
            {
                additionallog = additionallog + $" {unit.CurrentUnitCharacteristics.NumberOfUnits - decsquadhealth} units withstand before the death's door!"; newnumberof = unit.currentSquadHealth;
                //Console.WriteLine($"if numberof > squadhealth mxhp:{decmaxhealth} sqdhp:{squadhealth} coh:{truecohesion} num:{numberof}"); //Дебаг
            }
            
            if (decsquadhealth > unit.CurrentUnitCharacteristics.NumberOfUnits * unit.CurrentUnitCharacteristics.Health) //Проверяем, что хп отряда не больше максимального. Если да, значит часть войска сбежало с поле боя.
            {
                //Console.WriteLine($"ESCAPE DEBUG 1:{takedmg.armyunithealth.currentsquadhealth} 2:{takedmg.numberofunits.currentnumberofunits} 3:{takedmg.armyunithealth.currentunithealth}"); //Дебаг
                //Console.WriteLine($"ESCAPE DEBUG 11:{startnumberof} 22:{takedmg.numberofunits.currentnumberofunits} 1:{decmaxhealth} 2:{decsquadhealth} 3:{incdamage}"); //Дебаг
                //desserts = (takedmg.armyunithealth.currentsquadhealth - (takedmg.numberofunits.currentnumberofunits * takedmg.armyunithealth.currentunithealth)) / takedmg.armyunithealth.currentunithealth; additionallog = additionallog + $" {desserts} cowards escaped from the battlefield.";
                desserts = (startnumberof - unit.CurrentUnitCharacteristics.NumberOfUnits) - (int)Math.Floor((decmaxhealth - decsquadhealth) / unit.CurrentUnitCharacteristics.Health); additionallog = additionallog + $" {desserts} cowards escaped from the battlefield.";
                decsquadhealth = decsquadhealth - desserts * unit.CurrentUnitCharacteristics.Health; //Console.WriteLine($"if squadhealth > mxhp:{decmaxhealth} sqdhp:{squadhealth} coh:{truecohesion} num:{numberof}");  //Дебаг
            }

            if (applyDamage)
            {
                unit.currentSquadHealth = (int)decsquadhealth; //Обновили текущее хп отряда
                if ((int)newnumberof < unit.CurrentUnitCharacteristics.NumberOfUnits) unit.CurrentUnitCharacteristics.NumberOfUnits = (int)newnumberof;
                if (decsquadhealth <= 0)
                {
                    unit.CurrentUnitCharacteristics.NumberOfUnits = 0;
                }
            }
        }
        string battlelog = $"The {unit.UnitName} c:{truecohesion} squad taken {incdamage} damage from {enemyunitname}. There are {newnumberof}/{startnumberof} with {decsquadhealth} hp.";
        Debug.Log(battlelog + additionallog);
    }
}
