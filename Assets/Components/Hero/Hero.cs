using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public enum CellType { Available, NotAvailable, Occupied };
[Serializable]
public class Formation
{
    public Formation(List<Squad> army)
    {
        ArmyLine = army;
    }

    public List<Squad> ArmyLine;
}
[Serializable]
public struct Squad
{
    public ArmyUnitClass Unit;
    public int Line;
    public int Column;
    public CellType Type;
    public Squad(CellType type,int line,int column) {
        Unit = null; 
        Type = type;
        Line = line;
        Column = column;
    }
    public void ChangeType(CellType type)
    {
        Type = type;
    }
    public void ChangeUnit(ArmyUnitClass unit)
    {
        Unit = unit;
        Type = CellType.Occupied;
    }
}
public class Hero : MonoBehaviour
{
    [Header("Characteristics")]
    public string heroName;
    public List<GameObject> bannersList = new List<GameObject> { };
    public List<Formation> ArmyFormation;
    
    [Header("HeroModifiers")]
    [SerializeField] private int modinit = 0;
    [SerializeField] private int modcoh = 0;
    
    [Header("Private variables")]
    public int maxArmyWigth = 5;
    public int maxArmyDepth = 3;
    private void Awake()
    {
        ArmyFormation = new List<Formation>();
        for (int line = 0; line < maxArmyDepth; line++)
        {
            var formation = new List<Squad>();
            for (int column = 0; column < maxArmyWigth; column++)
            {
                formation.Add(new Squad(CellType.NotAvailable,line,column));
            }
            ArmyFormation.Add(new Formation(formation));
        }
        ArmyFormation[0].ArmyLine[2].ChangeType(CellType.Available);
    }
    public void AddUnitToFormation(Squad toCell, ArmyUnitClass unit)
    {
        if (toCell.Type == CellType.NotAvailable) {Debug.LogWarning($"{toCell} occupied");return;}
        if (bannersList.Contains(unit.transform.parent.gameObject)) { Debug.LogWarning($"{unit.UnitName} not in the banner list");return;}
        if (IsUnitInArmy(unit)) {Debug.LogWarning($"{unit.UnitName} already on the field");return;}

        if (toCell.Type == CellType.Available)
        {
            toCell.ChangeUnit(unit);
        }
    }
    public bool IsUnitInArmy(ArmyUnitClass unit)
    {
        bool answer=false;
        foreach (var formation in ArmyFormation)
        {
            foreach (var line in formation.ArmyLine)
            {
                if (line.Unit == unit) answer = true;
            }
        }
        return answer;
    }

    public void modifyHero(string n, int init, int coh) { heroName = n; modinit = init; modcoh = coh; ArmyFormation[0].ArmyLine[2].ChangeType(CellType.Available);
        ArmyFormation[0].ArmyLine[3].ChangeType(CellType.Occupied);}
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
