using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public enum CellType { Available, NotAvailable, Occupied };
[Serializable]
public class Formation
{
    //public ArmyCell Army;
    public List<ArmyCell> ArmyList;
}
[Serializable]
public struct ArmyCell
{
    public GameObject Unit; public CellType Type { get; set; }
    public ArmyCell(CellType type) { Unit = null; Type = type; }
    public ArmyCell(GameObject unit) { Unit = unit; Type = CellType.Occupied; }
}
public class Hero : MonoBehaviour
{
    public string heroName;
    [SerializeField] private int modinit = 0;
    [SerializeField] private int modcoh = 0;
    public List<GameObject> bannersList = new List<GameObject> { };
    public struct Army { public int lineNumber; public int columnNumber; public ArmyUnitClass unit; }
    public List<List<ArmyCell>> ArmyFormation;
    public List<Formation> Test;
    private void Awake()
    {
        ArmyFormation = new List<List<ArmyCell>>();
        for (int i = 0; i < 3; i++)
        {
            var tempList = new List<ArmyCell>();
            for (int _i = 0; _i < 5; _i++)
            {
                tempList.Add(new ArmyCell(CellType.NotAvailable));
            }
            ArmyFormation.Add(tempList);
        }
        ArmyFormation[0][2] = new ArmyCell(CellType.Available);
    }
    public void modifyHero(string n, int init, int coh) { heroName = n; modinit = init; modcoh = coh; }
    public string Getinfo()
    {
        string info = $"{heroName}. There are {bannersList.Count} units under his command. His commanding skills improved army initiative by {modinit} and all unit cohesion by {modcoh}!";
        return info;
    }
    public int GetArmyTotalHp()
    {
        int totalHP = 0;
        for (int i = 0; i != bannersList.Count; i++)
        {
            totalHP = totalHP + bannersList[i].GetComponent<ArmyUnitClass>().GetUnitHP();
        }
        return totalHP;
    }
    public void AddBannerList(GameObject unit)
    {
        bannersList.Add(unit);
        unit.GetComponent<ArmyUnitClass>().ApplyHeroModifyers(modinit, modcoh);
        unit.transform.SetParent(transform);
    }
    public int GetMaxInitiative()
    {
        int MaxInit = bannersList[0].GetComponent<ArmyUnitClass>().GetUnitCharacteristics().Item1.Initiative;
        for (int i = 0; i < bannersList.Count; i++)
        {
            if (MaxInit < bannersList[i].GetComponent<ArmyUnitClass>().GetUnitCharacteristics().Item1.Initiative) { MaxInit = bannersList[i].GetComponent<ArmyUnitClass>().GetUnitCharacteristics().Item1.Initiative; }
        }
        return MaxInit;
    }
    public int GetMinInitiative()
    {
        int MinInit = bannersList[0].GetComponent<ArmyUnitClass>().GetUnitCharacteristics().Item1.Initiative;
        for (int i = 0; i < bannersList.Count; i++)
        {
            if (MinInit > bannersList[i].GetComponent<ArmyUnitClass>().GetUnitCharacteristics().Item1.Initiative) { MinInit = bannersList[i].GetComponent<ArmyUnitClass>().GetUnitCharacteristics().Item1.Initiative; }
        }
        return MinInit;
    }
    public string GetBannerList()
    {
        string asd = ""; int i = 1;
        foreach (GameObject banner in bannersList)
        { asd = $"{asd} {i} {banner.GetComponent<ArmyUnitClass>().UnitName}"; i++; }
        return asd;
    }
}
