using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Planeswalker.Scripts;
using UnityEngine;

namespace Planeswalker
{
    class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        //[STAThread]
        static void Main(string[] args)
        {
            Start11();
            Debug.Log(message: "Smth");
            Start11();

        }

        public void Func()
        {
        }
        static void Start11()
        {
            Debug.Log(message: "Smth");
        }
        /* static void Start1()
         {
             Random rnd = new Random(); //объ€вл€ем функцию рандома
             for (int i = 1; i == 1;)
             {
                 Console.WriteLine("---=========---");
                 Hero YourHero = new Hero("", rnd.Next(0, 3) - 1, rnd.Next(0, 3) - 1);
                 Hero EvilHero = new Hero("Dark Lord", rnd.Next(0, 3) - 1, rnd.Next(0, 3) - 1);
                 Console.WriteLine("What is your name milord?");

                 YourHero.name = Console.ReadLine();
                 Console.WriteLine("---===BEHOLD===---");

                 YourHero.Getinfo();
                 Console.WriteLine("But wait...");
                 EvilHero.Getinfo();
                 Console.WriteLine("Milord, do you want to see your army?");
                 Console.ReadKey();

                 Console.WriteLine("---===Your army===---");
                 ArmyUnitClass firstSquad = new ArmyUnitClass("Local Militia", rnd.Next(1, 10) + 5, rnd.Next(0, 3) + 4, 3 + rnd.Next(0, 3), rnd.Next(0, 3) - 1, rnd.Next(0, 3) - 1,5);
                 ArmyUnitClass secondSquad = new ArmyUnitClass("Local Militia", rnd.Next(1, 10) + 5, rnd.Next(0, 3) + 4, 3 + rnd.Next(0, 3), rnd.Next(0, 3) - 1, rnd.Next(0, 3) - 1, 5);
                 ArmyUnitClass thirdSquad = new ArmyUnitClass("Local Militia", rnd.Next(1, 10) + 5, rnd.Next(0, 3) + 4, 3 + rnd.Next(0, 3), rnd.Next(0, 3) - 1, rnd.Next(0, 3) - 1, 5);
                 firstSquad.Getinfo(); secondSquad.Getinfo(); thirdSquad.Getinfo();
                 Console.WriteLine("---===Evil army===---");
                 ArmyUnitClass firstEvilSquad = new ArmyUnitClass("Goblin rogues", rnd.Next(1, 20) + 17, rnd.Next(0, 2) + 2, rnd.Next(0, 2) + 2, rnd.Next(0, 3) - 1, rnd.Next(0, 2) - 1, 5);
                 ArmyUnitClass seconEvildSquad = new ArmyUnitClass("Goblin rogues", rnd.Next(1, 20) + 17, rnd.Next(0, 2) + 2, rnd.Next(0, 2) + 2, rnd.Next(0, 3) - 1, rnd.Next(0, 2) - 1, 5);
                 ArmyUnitClass thirdEvilSquad = new ArmyUnitClass("Goblin rogues", rnd.Next(1, 20) + 17, rnd.Next(0, 2) + 2, rnd.Next(0, 2) + 2, rnd.Next(0, 3) - 1, rnd.Next(0, 2) - 1, 5);
                 firstEvilSquad.Getinfo(); seconEvildSquad.Getinfo(); thirdEvilSquad.Getinfo();

                 Console.ReadKey();
                 Console.Clear();

                 //YourHero.AddBannerToArray(ref thirdEvilSquad, 0);
                 //YourHero.bannersArray[0].Getinfo();


                 //thirdEvilSquad.Getinfo();
                 //YourHero.bannersArray[0].Getinfo();

                 //YourHero.AddBannerList(ref thirdEvilSquad);
                 //YourHero.bannersList[0].Getinfo();

                 //YourHero.bannersList[0].Getinfo();
                 //Console.ReadKey();
                 //Console.Clear();

             }
         }
         static void Start2()
         {

             Random rnd = new Random(); //объ€вл€ем функцию рандома
             for (int cycle = 1; cycle == 1;)
             {
                 Console.WriteLine("---=========---");
                 Hero YourHero = new Hero("Chosen one", rnd.Next(0, 3) - 1, rnd.Next(0, 3) - 1);
                 Hero EvilHero = new Hero("Dark Lord", rnd.Next(0, 3) - 1, rnd.Next(0, 3) - 1);
                 YourHero.Getinfo();
                 EvilHero.Getinfo();

                 Console.ReadKey();

                 Console.WriteLine("---===Your army===---");
                 ArmyUnitClass firstSquad = new ArmyUnitClass("1 Local Militia", rnd.Next(1, 10) + 5, rnd.Next(0, 3) + 4, 2 + rnd.Next(0, 3), rnd.Next(0, 3) - 1, rnd.Next(0, 3) - 1, 5);
                 ArmyUnitClass secondSquad = new ArmyUnitClass("2 Local Militia", rnd.Next(1, 10) + 5, rnd.Next(0, 3) + 4, 2 + rnd.Next(0, 3), rnd.Next(0, 3) - 1, rnd.Next(0, 3) - 1, 5);
                 ArmyUnitClass thirdSquad = new ArmyUnitClass("Lord Militia", 10, 5, 5, 1, 3, 5);
                 firstSquad.Getinfo(); secondSquad.Getinfo(); thirdSquad.Getinfo();
                 Console.WriteLine("---===Evil army===---");
                 ArmyUnitClass firstEvilSquad = new ArmyUnitClass("1 Goblin rogues", rnd.Next(1, 20) + 17, rnd.Next(0, 2) + 2, rnd.Next(0, 2) + 1, rnd.Next(0, 3) - 1, rnd.Next(0, 2) - 1, 5);
                 ArmyUnitClass seconEvildSquad = new ArmyUnitClass("2 Goblin rogues", rnd.Next(1, 20) + 17, rnd.Next(0, 2) + 2, rnd.Next(0, 2) + 1, rnd.Next(0, 3) - 1, rnd.Next(0, 2) - 1, 5);
                 ArmyUnitClass thirdEvilSquad = new ArmyUnitClass("3 Goblin rogues", rnd.Next(1, 20) + 17, rnd.Next(0, 2) + 2, rnd.Next(0, 2) + 1, rnd.Next(0, 3) - 1, rnd.Next(0, 2) - 1, 5);
                 firstEvilSquad.Getinfo(); seconEvildSquad.Getinfo(); thirdEvilSquad.Getinfo();

                 Console.ReadKey();

                 YourHero.AddBannerList(1, 1, ref firstSquad); YourHero.AddBannerList(1, 2, ref secondSquad); YourHero.AddBannerList(1, 3, ref thirdSquad);
                 EvilHero.AddBannerList(1, 1, ref firstEvilSquad); EvilHero.AddBannerList(1, 2, ref seconEvildSquad); EvilHero.AddBannerList(1, 3, ref thirdEvilSquad);

                 Battle battle = new Battle(YourHero, EvilHero);
                 battle.BattleLoop();


                 Console.ReadKey();
                 Console.Clear();
                 Console.WriteLine("---===Second Battle===---");

                 Console.ReadKey();
                 Console.Clear();
             }

         }
         static void Start3()
         {
             Random rnd = new Random();
             ArmyUnitClass OgreSquad = new ArmyUnitClass("Ogre", 1, 1000, 5, 0, -5, 5);
             //ArmyUnitClass GoblinSquad = new ArmyUnitClass("1 Goblin rogues", rnd.Next(1, 20) + 17, rnd.Next(0, 2) + 2, rnd.Next(0, 2) + 1, rnd.Next(0, 3) - 1, -1, 5);
             //ArmyUnitClass firstSquad = new ArmyUnitClass("Local Militia", 10, 5, 4, 1, 0, 5);
             ArmyUnitClass secondSquad = new ArmyUnitClass("Lord Militia", 100, 10, 5, 1, -10, 5);
             OgreSquad.Getinfo();// GoblinSquad.Getinfo(); firstSquad.Getinfo();
             secondSquad.Getinfo();
             Console.WriteLine($"{ Math.Ceiling(10 - 10 * Math.Pow(1 / 100, -6 / 10))} { Math.Pow(1 / 90, 1 -7 / 10)} {1 / 100}");
             Hero YourHero = new Hero("Chosen one", 0, 0);
             Hero EvilHero = new Hero("Dark Lord", 0, 0);
             YourHero.AddBannerList(1, 1, ref secondSquad);
             EvilHero.AddBannerList(1, 1, ref OgreSquad);
             Battle battle = new Battle(YourHero, EvilHero);
             battle.BattleLoop();
         }
         static void Start4()
         {
             Random rnd = new Random();
             Hero YourHero = new Hero("Chosen one", rnd.Next(0, 3) - 1, rnd.Next(0, 3) - 1);
             Hero EvilHero = new Hero("Dark Lord", rnd.Next(0, 3) - 1, rnd.Next(0, 3) - 1);
             ArmyUnitClass firstSquad = new ArmyUnitClass("Local Militia 1", rnd.Next(1, 10) + 5, rnd.Next(0, 3) + 4, 2 + rnd.Next(0, 3), rnd.Next(0, 3) - 1, rnd.Next(0, 3) - 1, 5);
             ArmyUnitClass secondSquad = new ArmyUnitClass("Local Militia 2", rnd.Next(1, 10) + 5, rnd.Next(0, 3) + 4, 2 + rnd.Next(0, 3), rnd.Next(0, 3) - 1, rnd.Next(0, 3) - 1, 5);
             ArmyUnitClass thirdSquad = new ArmyUnitClass("Local Militia 3", rnd.Next(1, 10) + 5, rnd.Next(0, 3) + 4, 2 + rnd.Next(0, 3), rnd.Next(0, 3) - 1, rnd.Next(0, 3) - 1, 5);
             YourHero.AddBannerList(1, 1, ref firstSquad); YourHero.AddBannerList(1, 2, ref secondSquad); YourHero.AddBannerList(1, 3, ref thirdSquad);
             ArmyUnitClass firstEvilSquad = new ArmyUnitClass("1 Goblin rogues", rnd.Next(1, 20) + 17, rnd.Next(0, 2) + 2, rnd.Next(0, 2) + 1, rnd.Next(0, 3) - 1, rnd.Next(0, 2) - 1, 5);
             ArmyUnitClass seconEvildSquad = new ArmyUnitClass("2 Goblin rogues", rnd.Next(1, 20) + 17, rnd.Next(0, 2) + 2, rnd.Next(0, 2) + 1, rnd.Next(0, 3) - 1, rnd.Next(0, 2) - 1, 5);
             ArmyUnitClass thirdEvilSquad = new ArmyUnitClass("3 Goblin rogues", rnd.Next(1, 20) + 17, rnd.Next(0, 2) + 2, rnd.Next(0, 2) + 1, rnd.Next(0, 3) - 1, rnd.Next(0, 2) - 1, 5);
             EvilHero.AddBannerList(1, 1, ref firstEvilSquad); EvilHero.AddBannerList(1, 2, ref seconEvildSquad); EvilHero.AddBannerList(1, 3, ref thirdEvilSquad);

             YourHero.ChooseTargetInBattle(EvilHero.bannersList);
             for (int i = 0; i < YourHero.bannersList.Count; i++) { Console.WriteLine($"-----==uniot {i} {YourHero.bannersList[i].unit.battletarget[0]} {YourHero.bannersList[i].unit.battletarget[1]} == -----"); };
             Console.ReadKey();

             //for (int i = 0; i < EvilHero.bannersList.Count; i++)
             //{
             //    if (YourHero.bannersList[0].unit.battletarget[0] == EvilHero.bannersList[i].lineNumber && YourHero.bannersList[0].unit.battletarget[1] == EvilHero.bannersList[i].columnNumber)
             //    {
             //        return YourHero.bannersList[i];
             //    };
             //};
         }
        static void Start5()
        {
            Console.Clear();
            Random rnd = new Random(); //объ€вл€ем функцию рандома
            Hero YourHero = new Hero("Chosen one", 1, 1);
            Hero EvilHero = new Hero("Dark Lord", 1, 1);
            List<ArmyUnitClass> newArmyList = new List<ArmyUnitClass>();
            List<int> listOfStrings = new List<int>(); listOfStrings.Add(1); listOfStrings.Add(2); listOfStrings.Add(3);

            Console.WriteLine("---===Hero!===---");
            Console.WriteLine("Quickly! Enemies at the gate!");
            Console.WriteLine("Choose the three best squads and run to the defense of the city as soon as possible!");
            Console.ReadKey();

            for (int i = 0; i != 3; i++)
            {
                //Console.WriteLine($"---==={i + 1} small cycle start===---");
                int intsquadnumber = 0; string strsquadnumber = "";
                Console.WriteLine("");
                if (i == 0) { Console.WriteLine("You ran to the closest barracks and you find..."); }
                else { Console.WriteLine("You ran to the next barracks and you find..."); };

                ArmyUnitClass firstSquad = new ArmyUnitClass("Local Militia", rnd.Next(10, 35), rnd.Next(0, 3) + 4, 2 + rnd.Next(0, 3), rnd.Next(0, 3) - 1, rnd.Next(0, 3) - 1, 5);
                ArmyUnitClass secondSquad = new ArmyUnitClass("Dirty Thieves", rnd.Next(10, 25), rnd.Next(0, 3) + 4, 5 + rnd.Next(0, 3), rnd.Next(1, 5) - 1, rnd.Next(0, 3) - 1, 5);
                ArmyUnitClass thirdSquad = new ArmyUnitClass("Trained warriors", rnd.Next(7, 16), rnd.Next(2, 6) + 8, 3 + rnd.Next(1, 4), rnd.Next(1, 5) - 1, rnd.Next(2, 6) - 1, 5);
                newArmyList.Add(firstSquad); newArmyList.Add(secondSquad); newArmyList.Add(thirdSquad);
                newArmyList[0].Getinfo(); newArmyList[1].Getinfo(); newArmyList[2].Getinfo();

                for (int forcycle1 = 1; forcycle1 == 1;)
                {
                    Console.WriteLine($"---===Choose your {i + 1} squad (write a number 1-3)===---");
                    strsquadnumber = Console.ReadLine();
                    if (strsquadnumber == "") { strsquadnumber = "0"; }
                    else { intsquadnumber = Int32.Parse(strsquadnumber); };

                    if (listOfStrings.Contains(intsquadnumber))
                    {
                        YourHero.AddBannerList(1, 1, newArmyList[intsquadnumber - 1]);

                        newArmyList.Clear();
                        //Console.WriteLine($"ok? {newArmyList.Count}");
                        break;
                    }
                    else { Console.WriteLine("wrong"); };
                    Console.WriteLine("wtf?");
                }
                //Console.WriteLine("squad?");
                //YourHero.bannersList[i].unit.Getinfo();
            }

            Console.Clear();
            Console.WriteLine("And are you going to let this rabble die under the walls of Stormwind ? ");
            Console.WriteLine("");
            Console.WriteLine("What a poor choice...");
            Console.ReadKey();
            Console.WriteLine("");
            YourHero.bannersList[0].unit.Getinfo(); YourHero.bannersList[1].unit.Getinfo(); YourHero.bannersList[2].unit.Getinfo();
            Console.WriteLine("");
            Console.ReadKey();
            Console.WriteLine("Behold your enemy!");
            Console.WriteLine("");
            EvilHero.Getinfo();
            ArmyUnitClass firstEvilSquad = new ArmyUnitClass("Ogre", 1, rnd.Next(125, 200), rnd.Next(25, 35), rnd.Next(-2, 2) - 1, rnd.Next(-5, 0) - 1, 75);
            ArmyUnitClass seconEvildSquad = new ArmyUnitClass("Goblin rogues", rnd.Next(20, 70), rnd.Next(0, 2) + 2, rnd.Next(0, 2) + 2, rnd.Next(0, 3) - 1, rnd.Next(-4, 1), 5);
            ArmyUnitClass thirdEvilSquad = new ArmyUnitClass("Goblin punks", rnd.Next(75, 125), rnd.Next(0, 2) + 2, 1, rnd.Next(0, 3) - 1, rnd.Next(-10, -1), 5);
            firstEvilSquad.Getinfo(); seconEvildSquad.Getinfo(); thirdEvilSquad.Getinfo();
            EvilHero.AddBannerList(1, 1, firstEvilSquad); EvilHero.AddBannerList(1, 2, seconEvildSquad); EvilHero.AddBannerList(1, 3, thirdEvilSquad);

            Console.ReadKey();
            Console.WriteLine("");
            Console.WriteLine("To Battle!");
            Console.WriteLine("");

            Battle battle = new Battle(YourHero, EvilHero);
            battle.BattleLoop();

            Console.ReadKey();

        }*/
    }
}
