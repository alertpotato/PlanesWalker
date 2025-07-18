using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

[System.Serializable]
public enum Race { Human, Goblin };
[System.Serializable]
public enum ArmourType { NoArmour,Leather, Chainmail, Plate };
[System.Serializable]
public enum WeaponType { Blade, Polearm, Bow, Crossbow, Axe, Mace };

[System.Serializable]
public struct ArmourSet
{
    [Tooltip("Name of armour set")]public string armourName;
    [Tooltip("Armour type")]public ArmourType armourType;
    [Tooltip("Additional supply cost added to unit using this armour set")]public int[] armourCost;
    [Tooltip("Level of protection against pierce damage")]public int pierceLevel;
    [Tooltip("Level of protection against slash damage")]public int slashLevel;
    [Tooltip("Level of protection against blunt damage")]public int bluntLevel;
    public ArmourSet( string name="No armour", int[] cost=null, ArmourType aType=ArmourType.NoArmour,int pLevel=0,int sLevel=0,int bLevel=0)
    {
        armourName=name;
        armourType=aType;
        if (cost == null) armourCost=new int[4]{0,0,0,0};
        else armourCost = cost;
        pierceLevel=pLevel;
        slashLevel=sLevel;
        bluntLevel = bLevel;
    }
}
[System.Serializable]
public struct Weapon
{
    [Tooltip("Name of weapon")]public string weaponName;
    [Tooltip("Weapon type")]public WeaponType weaponType;
    [Tooltip("Additional supply cost added to unit using this weapon")]public int[] weaponCost;
    [Tooltip("Level of penetration against pierce armour")]public int pierceLevel;
    [Tooltip("Level of penetration against slash armour")]public int slashLevel;
    [Tooltip("Level of penetration against blunt armour")]public int bluntLevel;
    [Tooltip("Pierce weapon damage")]public float pierceDamage;
    [Tooltip("Slash weapon damage")]public float slashDamage;
    [Tooltip("Blunt weapon damage")]public float bluntDamage;
    public Weapon( string name, WeaponType wType, int[] cost,int pLevel=0,int sLevel=0,int bLevel=0,float pDamage=0,float sDamage=0,float bDamage=0)
    {
        weaponName=name;
        weaponType=wType;
        weaponCost = cost;
        pierceLevel=pLevel;
        slashLevel=sLevel;
        bluntLevel = bLevel;
        pierceDamage = pDamage;
        slashDamage=sDamage;
        bluntDamage=bDamage;
    }
}
[System.Serializable]
public class UnitAttributes
{
    [Header("Unit attributes")]
    [Tooltip("Name of the attribute set")]public string AttributesName;
    [Tooltip("Description of the attribute set")]public string AttributesDescription;
    [Tooltip("Health amount of individual unit")]public float Health;
    [Tooltip("Unity combat proficiency")]public int CombatProficiency;
    [Tooltip("Unit cohesion")]public int Cohesion;
    
    [Header("Squad attributes")]
    [Tooltip("Number of units in squad")]public int SquadSize;
    [Tooltip("Supply cost to deploy squad on battlefield")]public int[] CostOfDeployment;
    [Tooltip("Squad abilities")][SerializeReference]public List<UnitAbility> SquadAbilities = new List<UnitAbility>();
    
    public UnitAttributes(string name, float health, int CP, int cohesion,int squadSize,List<Func<UnitAbility>> abilities = null, int[] cost=null)
    {
        AttributesName=name;
        Health=health;
        CombatProficiency=CP;
        Cohesion=cohesion;
        SquadSize=squadSize;
        CostOfDeployment=cost;
        if (abilities != null)
            foreach (var ab in abilities)
            {
                SquadAbilities.Add(ab());
            }
        //Description fill
        MakeDesc();
    }

    private void MakeDesc()
    {
        AttributesDescription = AttributesName;
        if (Health>0) AttributesDescription+=$"\n  Health: +{Health}";
        if (CombatProficiency>0) AttributesDescription+=$"\n  Combat Proficiency: +{CombatProficiency}";
        if (Cohesion!=0) AttributesDescription+=$"\n  Cohesion: +{Cohesion}";
        if (SquadSize>0) AttributesDescription+=$"\n  Squad Size: +{SquadSize}";
        if (CostOfDeployment != null)
        {
            if (CostOfDeployment[0]>0) AttributesDescription+=$"\n  Food cost: +{CostOfDeployment[0]}";
            if (CostOfDeployment[1]>0) AttributesDescription+=$"\n  Weapons cost: +{CostOfDeployment[1]}";
            if (CostOfDeployment[2]>0) AttributesDescription+=$"\n  Gold cost: +{CostOfDeployment[2]}";
            if (CostOfDeployment[3]>0) AttributesDescription+=$"\n  Wisdom cost: +{CostOfDeployment[3]}";
        }
        /*
        if (Armour.armourType != ArmourType.NoArmour)
        {
            AttributesDescription+=$"\n  Armour quality: ";
            if (Armour.pierceLevel>0) AttributesDescription+=$"+{Armour.pierceLevel} pierce";
            if (Armour.slashLevel>0) AttributesDescription+=$"+{Armour.slashLevel} slash";
            if (Armour.bluntLevel>0) AttributesDescription+=$"+{Armour.bluntLevel} blunt";
        }

        if (.Count>0)
        {
            foreach (var weapon in Weapons)
            {
                AttributesDescription+=$"\n{weapon.weaponName}({weapon.weaponType}): ";
                AttributesDescription += $"Pierce {weapon.pierceDamage}:{weapon.pierceLevel}";
                AttributesDescription += $"Slash {weapon.slashDamage}:{weapon.slashLevel}";
                AttributesDescription += $"Blunt {weapon.bluntDamage}:{weapon.bluntLevel}";
            }
        }
        */
        if (SquadAbilities.Count>0)
        {
            foreach (var ab in SquadAbilities)
            {
                AttributesDescription+=$"\n  Ability: {ab.AbilityName})";
            }
        }
    }

    public UnitAttributes(UnitAttributes attributesToCopy)
    {
        AttributesName=attributesToCopy.AttributesName;
        AttributesDescription =attributesToCopy.AttributesDescription;
        Health=attributesToCopy.Health;
        CombatProficiency=attributesToCopy.CombatProficiency;
        Cohesion=attributesToCopy.Cohesion;
        SquadSize = attributesToCopy.SquadSize;
        CostOfDeployment=attributesToCopy.CostOfDeployment;
        SquadAbilities=attributesToCopy.SquadAbilities;
    }

    public void AddNewAttribute(UnitAttributes newAttributes)
    {
        Health+=newAttributes.Health;
        CombatProficiency+=newAttributes.CombatProficiency;
        Cohesion+=newAttributes.Cohesion;
        SquadSize+=newAttributes.SquadSize;
        //New Cost
        for (int i = 0; i < CostOfDeployment.Length; i++)
        {
            CostOfDeployment[i]+=newAttributes.CostOfDeployment[i];
        }
        //New AB
        SquadAbilities.AddRange(newAttributes.SquadAbilities);
        //New Armour
        /*
        int[] newArmourCost = Armour.armourCost; 
        for (int i = 0; i < Armour.armourCost.Length; i++)
        {
            newArmourCost[i]+=newAttributes.Armour.armourCost[i];
        }
        ArmourSet newArmourSet = new ArmourSet(Armour.armourName, newArmourCost, Armour.armourType,Armour.pierceLevel+newAttributes.Armour.pierceLevel,Armour.slashLevel+newAttributes.Armour.slashLevel,Armour.bluntLevel+newAttributes.Armour.bluntLevel);
        Armour = newArmourSet;
        //New Weapons
        //TODO This is purely TEMP
        foreach (var weaponToAdd in newAttributes.Weapons)
        {
            var newWeaponsList = new List<Weapon>();
            foreach (var weapon in Weapons)
            {
                int[] newWeaponCost = weapon.weaponCost;
                for (int i = 0; i < weapon.weaponCost.Length; i++)
                {
                    newWeaponCost[i] += weaponToAdd.weaponCost[i];
                }
                Weapon newWeapon = new Weapon(weapon.weaponName,weapon.weaponType,newWeaponCost,weapon.pierceLevel+weaponToAdd.pierceLevel,weapon.slashLevel+weaponToAdd.slashLevel,weapon.bluntLevel+weaponToAdd.bluntLevel,weapon.pierceDamage+weaponToAdd.pierceDamage,weapon.slashDamage+weaponToAdd.slashDamage,weapon.bluntDamage+weaponToAdd.bluntDamage);
                newWeaponsList.Add(newWeapon);
            }
            Weapons = newWeaponsList;
        }
        */
    }
}
[System.Serializable]
public class Unit
{
    [Header("Main attributes")]
    [Tooltip("Unit name")]public string UnitName;
    [Tooltip("Unit race")]public Race UnitRace;
    [Tooltip("Current unit stats, used for incombat calculations")]public UnitAttributes CurrentUnitAttributes;
    [Tooltip("Saved unit pre combat stats, used for comparing and restoring unit max values")]public UnitAttributes SavedUnitAttributes;
    [Tooltip("Starting unit attributes")]public UnitAttributes BaseUnitAttributes;
    [Tooltip("List of all upgrades, buffs, training improvements, perks ex.")]public List<UnitAttributes> UnitUpgrades = new List<UnitAttributes>();
    [Header("Unit equipment")]
    [Tooltip("Unit armour")]public ArmourSet armour;
    [Tooltip("Unit weapons")]public List<Weapon> weapons = new List<Weapon>();
    public Unit(Race unitRace,UnitAttributes baseUnitAttributes,ArmourSet Armour=new ArmourSet(),List<Weapon> Weapons=null)
    {
        UnitName = baseUnitAttributes.AttributesName;
        UnitRace = unitRace;
        BaseUnitAttributes = baseUnitAttributes;
        armour = Armour;
        weapons.AddRange(Weapons);
        RebuildCurrentUnitAttributes();
    }

    public void RebuildCurrentUnitAttributes()
    {
        SavedUnitAttributes = new UnitAttributes(BaseUnitAttributes);
        foreach (var UnitAttr in UnitUpgrades)
        {
            SavedUnitAttributes.AddNewAttribute(UnitAttr);
        }
        CurrentUnitAttributes = new UnitAttributes(SavedUnitAttributes);
    }

    public void AddUpgrades(List<UnitAttributes> upgrades)
    {
        foreach (var UA in upgrades)
        {
            UnitUpgrades.Add(UA);
        }
    }
}

[System.Serializable]
public class UnitTemplates
{
    [Tooltip("UnitAttributes class")]public UnitAttributes templateUnitAttributes;
    [Tooltip("Weight of attr")]public int weight;
    [Tooltip("Wealth that unit has to buy equipment")]public int wealth;

    public UnitTemplates(UnitAttributes newEventUnit,int newWeight,int newWealth)
    {
        templateUnitAttributes=newEventUnit;
        weight = newWeight;
        wealth = newWealth;
    }
}

[System.Serializable]
public struct PointsToRandomuzeUnitWeights
{
    public int points; public int weight;
    public PointsToRandomuzeUnitWeights(int p, int w)
    { points = p;weight = w; }
}

[CreateAssetMenu]
public class UnitFactory : ScriptableObject
{
    [Tooltip("Amount of upgrade points with weights")]
    public List<PointsToRandomuzeUnitWeights> pointsRandomizerList;

    public List<ArmourSet> armourSetList;
    public List<Weapon> weaponList;

    [Tooltip("All default unit characteristics")]
    public List<UnitTemplates> templates;


    public void InizializeUnitFactory()
    {
        FillListOfPoints();
        FillTemplates();
        FillArmourSet();

    }

    public Unit GetRandomUnit()
    {
        var localUnitWeights = new int[templates.Count];
        for (int i = 0; i < templates.Count; i++)
        {
            localUnitWeights[i] = templates[i].weight;
        }

        int indexOfSelectedUnit = WeightFunctions.GetRandomWeightedIndex(localUnitWeights);

        return new Unit(Race.Human, templates[indexOfSelectedUnit].templateUnitAttributes, Weapons: new List<Weapon> { weaponList[0] });
    }

public int GenerateUpgradePoints()
    {
        var localPointsToRandomize = new int[pointsRandomizerList.Count];
        for (int i = 0; i < pointsRandomizerList.Count; i++) { localPointsToRandomize[i] = pointsRandomizerList[i].weight; }
        int indexOfNumberOfPoints = WeightFunctions.GetRandomWeightedIndex(localPointsToRandomize);
        return pointsRandomizerList[indexOfNumberOfPoints].points;
    }
    /*
    public UnitUpgrades UpgradeUnit(BaseUnitCharacteristics unit,int numberOfPointsToRandomize)
    {
        UnitUpgrades unitUpgrades = new UnitUpgrades();
        List<UnitWeightsOfChars> localWeightsOfChars= new List<UnitWeightsOfChars>
            {unit.NumberOfUnitsUpgrade,unit.HealthUpgrade,unit.DamageUpgrade,unit.InitiativeUpgrade,unit.CohesionUpgrade,unit.ArmourUpgrade };
        var localListOfStatWeights = new int[localWeightsOfChars.Count];
        for (int i = 0; i < localWeightsOfChars.Count; i++) { localListOfStatWeights[i] = localWeightsOfChars[i].Weight; }
        //TODO >0 might not work if you have 1 last point and cheapest upgrade >1
        while (numberOfPointsToRandomize > 0)
        {
            int indexOfStat = WeightFunctions.GetRandomWeightedIndex(localListOfStatWeights);
            if (numberOfPointsToRandomize - localWeightsOfChars[indexOfStat].Cost >= 0)
            {
                numberOfPointsToRandomize -= localWeightsOfChars[indexOfStat].Cost;
                if (indexOfStat == 0) { unitUpgrades.NumberOfUnits += 1; }
                if (indexOfStat == 1) { unitUpgrades.Health += 1; }
                if (indexOfStat == 2) { unitUpgrades.Damage += 1; }
                if (indexOfStat == 3) { unitUpgrades.Initiative += 1; }
                if (indexOfStat == 4) { unitUpgrades.Cohesion += 1; }
                if (indexOfStat == 5) { unitUpgrades.Armour += 1; }
                localListOfStatWeights[indexOfStat] = Mathf.Abs(localListOfStatWeights[indexOfStat]/5);
            }
        }
        return (unitUpgrades);
    }
    */
    private void FillListOfPoints()
    {
        pointsRandomizerList.Clear();
        pointsRandomizerList.Add(new PointsToRandomuzeUnitWeights(0, 2000));
        pointsRandomizerList.Add(new PointsToRandomuzeUnitWeights(1, 6000));
        pointsRandomizerList.Add(new PointsToRandomuzeUnitWeights(2, 6000));
        pointsRandomizerList.Add(new PointsToRandomuzeUnitWeights(3, 2000));
        pointsRandomizerList.Add(new PointsToRandomuzeUnitWeights(4, 500));
        pointsRandomizerList.Add(new PointsToRandomuzeUnitWeights(5, 125));
        pointsRandomizerList.Add(new PointsToRandomuzeUnitWeights(6, 10));
        pointsRandomizerList.Add(new PointsToRandomuzeUnitWeights(7, 5));
        pointsRandomizerList.Add(new PointsToRandomuzeUnitWeights(8, 3));
        pointsRandomizerList.Add(new PointsToRandomuzeUnitWeights(9, 2));
        pointsRandomizerList.Add(new PointsToRandomuzeUnitWeights(10, 1));
        pointsRandomizerList.Add(new PointsToRandomuzeUnitWeights(11, 0));
        pointsRandomizerList.Add(new PointsToRandomuzeUnitWeights(12, 0));
    }
    private void FillArmourSet()
    {
        armourSetList.Clear();
        weaponList.Clear();
        ArmourSet LeatherArmourSet = new ArmourSet("Leather Armour",new int[4]{0,1,0,0},ArmourType.Leather,0,1,1);
        ArmourSet QuiltedLeatherArmourSet = new ArmourSet("Quilted Leather Armour",new int[4]{1,1,0,0},ArmourType.Leather,1,2,1);
        ArmourSet ChainArmourSet = new ArmourSet("Chainmail",new int[4]{0,1,1,0},ArmourType.Chainmail,1,3,2);
        ArmourSet ChainPlusArmourSet = new ArmourSet("Good Chainmail",new int[4]{0,2,1,0},ArmourType.Chainmail,2,3,2);
        armourSetList.Add(LeatherArmourSet);
        armourSetList.Add(QuiltedLeatherArmourSet);
        armourSetList.Add(ChainArmourSet);
        armourSetList.Add(ChainPlusArmourSet);

        Weapon militiaWeapon = new Weapon("Bad weapon", WeaponType.Polearm, new int[4] { 0, 1, 0, 0 },1,1,1,2,2,2);
        Weapon MenAtArmsWeapon = new Weapon("MenAtArms weapon", WeaponType.Blade, new int[4] { 0, 2, 0, 0 },2,2,1,3,4,1);
        Weapon KnightWeapon = new Weapon("Knight weapon", WeaponType.Mace, new int[4] { 0, 2, 1, 0 },0,1,3,0,2,6);
        Weapon ArcherWeapon = new Weapon("Bow", WeaponType.Bow, new int[4] { 1, 1, 0, 0 },2,0,0,4,0,0);
        weaponList.Add(militiaWeapon);
        weaponList.Add(MenAtArmsWeapon);
        weaponList.Add(KnightWeapon);
        weaponList.Add(ArcherWeapon);
    }

    private void FillTemplates()
    {
        templates.Clear();
        //Sets of abilities 
        var melee = new List<Func<UnitAbility>> { () => new MeleeCombatAbility() };
        var knight = new List<Func<UnitAbility>> { () => new KnightlyFeatAbility(), () => new MeleeCombatAbility()};
        var merc = new List<Func<UnitAbility>> { () => new MeleeCombatAbility(),() => new SuppressiveFireAbility() };
        var ranged = new List<Func<UnitAbility>> {() => new ArrowVolleyAbility(),() => new MeleeCombatAbility() };
        var mounted = new List<Func<UnitAbility>> { () => new MountedChargeAbility(),() => new MeleeCombatAbility() };
        // LOOK INTO SAVE LOAD ?
        var unitAttrMilitia = new UnitAttributes("Militia",10,1,1,8,melee,new int[]{1,0,0,0});
        templates.Add(new UnitTemplates(unitAttrMilitia,1,1));
        var unitAttrSpear = new UnitAttributes("Spearman",10,2,2,6,melee,new int[]{1,0,0,0});
        templates.Add(new UnitTemplates(unitAttrSpear,1,2));
        var unitAttrArcher = new UnitAttributes("Archer",10,2,1,5,melee,new int[]{1,0,0,0});
        templates.Add(new UnitTemplates(unitAttrArcher,1,2));
        var unitAttrBandit = new UnitAttributes("Bandits",10,2,0,7,melee,new int[]{1,0,0,0});
        templates.Add(new UnitTemplates(unitAttrBandit,1,1));
        var unitAttrMerc = new UnitAttributes("Mercenaries",10,3,2,7,melee,new int[]{1,0,0,0});
        templates.Add(new UnitTemplates(unitAttrMerc,1,3));
        var unitAttrHobelar = new UnitAttributes("Hobelar",10,3,2,4,melee,new int[]{1,0,0,0});
        templates.Add(new UnitTemplates(unitAttrHobelar,1,3));
        var unitAttrKnight = new UnitAttributes("Hedge Knight",10,4,3,2,melee,new int[]{1,0,0,0});
        templates.Add(new UnitTemplates(unitAttrKnight,1,4));
    }

}
