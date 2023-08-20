using System;
class Battle
{
    //  public struct UnitCharacteristics { public string ucunitname; public int ucnumberofunits; public int ucunithealth; public int ucunitdamage; public int ucunitinitiative; public int ucunitcohesion; }
    private Hero hero1;
    private Hero hero2;
    public Battle(Hero inputhero1, Hero inputhero2)
    {
        hero1 = inputhero1; hero2 = inputhero2;
    }

    public void BattleLoop()
    {

        for (int i = 1; BattleRound(i); i++) ;
    }
    private bool BattleRound(int roundnumber)
    {
        bool battlecontinue = true;
        if (hero2.GetArmyTotalHp() == 0 || hero1.GetArmyTotalHp() == 0) { Console.WriteLine($"{hero1.name} or {hero2.name} have no army to fight..."); return false; }

        Random rnd = new Random();
        int LeftI = rnd.Next(0, hero2.bannersList.Count);
        int RightI = rnd.Next(0, hero1.bannersList.Count);

        Console.WriteLine($"---=== Round {roundnumber}, Left army hp {hero2.GetArmyTotalHp()}, Right army hp {hero1.GetArmyTotalHp()},");

        int hero1unitdmg = hero1.bannersList[RightI].GetUnitDamage().currentunitdamage; int hero1squaddmg = hero1.bannersList[RightI].GetAlldamage(); string hero1unitname = hero1.bannersList[RightI].unitname;
        int hero2unitdmg = hero2.bannersList[LeftI].GetUnitDamage().currentunitdamage; int hero2squaddmg = hero2.bannersList[LeftI].GetAlldamage(); string hero2unitname = hero2.bannersList[LeftI].unitname;


        TakeDamage(hero2.bannersList[LeftI], hero1squaddmg, hero1unitdmg, hero1unitname);
        TakeDamage(hero1.bannersList[RightI], hero2squaddmg, hero2unitdmg, hero2unitname);

        hero2.IsUnitAlive(hero2.bannersList[LeftI]); hero1.IsUnitAlive(hero1.bannersList[RightI]);

        if (hero2.GetArmyTotalHp() == 0) { Console.WriteLine($"{hero2.name} defeted..."); battlecontinue = false; }
        if (hero1.GetArmyTotalHp() == 0) { Console.WriteLine($"{hero1.name} defeted..."); battlecontinue = false; }
        Console.ReadKey();
        return battlecontinue;
    }

    private void TakeDamage(ArmyUnitClass takedmg, int incsquaddmg, int incunitdmg, string enemyunitname)
    {
        int cohdamage = 0; int desserts = 0; string additionallog = "";
        double decmaxhealth = takedmg.numberofunits.currentnumberofunits * takedmg.armyunithealth.currentunithealth;    //Максимально возможное хп отряда
        int incdamage = incsquaddmg;
        double decsquadhealth = takedmg.armyunithealth.currentsquadhealth - incdamage;    //Реальное хп после получения урона
        takedmg.armyunithealth.currentsquadhealth = (int)decsquadhealth;      //Обновили текущее хп отряда
        if (incunitdmg >= takedmg.armyunithealth.currentunithealth * 2) { cohdamage = (int)Math.Log(incunitdmg / takedmg.armyunithealth.currentunithealth, 2); }
        else if (takedmg.armyunithealth.currentunithealth >= incunitdmg * 2) { cohdamage = -(int)Math.Log(takedmg.armyunithealth.currentunithealth / incunitdmg, 2); };
        double truecohesion = takedmg.unitstats.currentcohesion - cohdamage;     //Записали сплоченность
        if (takedmg.unitstats.currentcohesion >= 0 & truecohesion < 0) { additionallog = " The ranks wavered"; };
        double decincdamage = decmaxhealth - decsquadhealth;    //Реально полученный урон учитывая макс хп
        double decnumberof = takedmg.numberofunits.currentnumberofunits;
        int startnumberof = takedmg.numberofunits.currentnumberofunits;   //Запомнили предыдущее количества для вывода статистики боя
        double newnumberof;     //Расчет нового количества

        if (truecohesion < -10) { truecohesion = -10; }//Если вдруг перешли за -10 то возвращаем -10, так как это всё равно 100% урон по отряду
                                                       //Console.WriteLine($"mxhp:{decmaxhealth} sqdhp:{decsquadhealth} coh:{takedmg.unitstats.currentcohesion} truecoh:{truecohesion} num:{takedmg.numberofunits.currentnumberofunits}"); //Дебаг

        if (decsquadhealth <= 0) { takedmg.armyunithealth.currentsquadhealth = 0; takedmg.numberofunits.currentnumberofunits = 0; }//Если хп после получения урона < ноля закончить бой
        else if (incdamage <= 0) { Console.WriteLine("Damage 0"); } //Проверяем что входящий урон не 0 и не меньше 0, чтобы не пересчитывать количество отряда при резкой смене сплоченности(и не ломать игру).
        else
        {
            if (truecohesion >= 0)
            {
                //Console.WriteLine("---truecohesion >= 0---"); //Дебаг
                newnumberof = Math.Ceiling(decnumberof - decnumberof * Math.Pow(decincdamage / decmaxhealth, truecohesion + 1));
                if ((int)newnumberof >= takedmg.numberofunits.currentnumberofunits) { } else { takedmg.numberofunits.currentnumberofunits = (int)newnumberof; }
                //Console.WriteLine($"mxhp:{decmaxhealth} sqdhp:{decsquadhealth} coh:{truecohesion} num:{numberof} decnm:{newnumberof}"); //Дебаг
            }
            if (truecohesion < 0)
            {
                //Console.WriteLine($"---truecohesion < 0---"); //Дебаг
                //debug_pow = Math.Pow(decincdamage / decmaxhealth, 1 + truecohesion / 10);
                //Console.WriteLine($"DEC:{decnumberof} DAM:{decincdamage} MH:{decmaxhealth} COH:{truecohesion} WTF:{debug_pow}"); //Дебаг
                newnumberof = Math.Ceiling(decnumberof - decnumberof * Math.Pow(decincdamage / decmaxhealth, 1 + truecohesion / 10));
                if ((int)newnumberof > takedmg.numberofunits.currentnumberofunits) { } else { takedmg.numberofunits.currentnumberofunits = (int)newnumberof; }
                //Console.WriteLine($"mxhp:{decmaxhealth} sqdhp:{decsquadhealth} coh:{truecohesion} num:{takedmg.numberofunits.currentnumberofunits} decnm:{newnumberof}"); //Дебаг
            }

            if (takedmg.numberofunits.currentnumberofunits > takedmg.armyunithealth.currentsquadhealth)
            {
                additionallog = additionallog + $" {takedmg.numberofunits.currentnumberofunits - takedmg.armyunithealth.currentsquadhealth} units withstand before the death's door!"; takedmg.numberofunits.currentnumberofunits = takedmg.armyunithealth.currentsquadhealth;
                //Console.WriteLine($"if numberof > squadhealth mxhp:{decmaxhealth} sqdhp:{squadhealth} coh:{truecohesion} num:{numberof}"); //Дебаг
            }


            if (takedmg.armyunithealth.currentsquadhealth > takedmg.numberofunits.currentnumberofunits * takedmg.armyunithealth.currentunithealth) //Проверяем, что хп отряда не больше максимального. Если да, значит часть войска сбежало с поле боя.
            {
                //Console.WriteLine($"ESCAPE DEBUG 1:{takedmg.armyunithealth.currentsquadhealth} 2:{takedmg.numberofunits.currentnumberofunits} 3:{takedmg.armyunithealth.currentunithealth}"); //Дебаг
                //Console.WriteLine($"ESCAPE DEBUG 11:{startnumberof} 22:{takedmg.numberofunits.currentnumberofunits} 1:{decmaxhealth} 2:{decsquadhealth} 3:{incdamage}"); //Дебаг
                //desserts = (takedmg.armyunithealth.currentsquadhealth - (takedmg.numberofunits.currentnumberofunits * takedmg.armyunithealth.currentunithealth)) / takedmg.armyunithealth.currentunithealth; additionallog = additionallog + $" {desserts} cowards escaped from the battlefield.";
                desserts = (startnumberof - takedmg.numberofunits.currentnumberofunits) - (int)Math.Floor((decmaxhealth - decsquadhealth) / takedmg.armyunithealth.currentunithealth); additionallog = additionallog + $" {desserts} cowards escaped from the battlefield.";
                takedmg.armyunithealth.currentsquadhealth = takedmg.armyunithealth.currentsquadhealth - desserts * takedmg.armyunithealth.currentunithealth; //Console.WriteLine($"if squadhealth > mxhp:{decmaxhealth} sqdhp:{squadhealth} coh:{truecohesion} num:{numberof}");  //Дебаг
            }
        }
        string battlelog = $"The {takedmg.unitname} {takedmg.armytype} squad taken {incdamage} damage from {enemyunitname}. There are {takedmg.numberofunits.currentnumberofunits}/{startnumberof} with {takedmg.armyunithealth.currentsquadhealth} hp.";
        Console.WriteLine(battlelog + additionallog);
    }
}
