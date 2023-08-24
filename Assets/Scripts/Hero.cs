using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public enum cellType { Forbidden, Empty, Occupied };
public struct ArmyCell
{
    GameObject unit; public cellType type { get; set; }
    int initiativeMod; int cohesionMod; int defenceMod; int regenCohMod; int damageMod;
    public ArmyCell(cellType _type) { unit = null; type = _type; initiativeMod = 0; cohesionMod = 0; defenceMod = 0; regenCohMod = 0; damageMod = 0; }
    public ArmyCell(GameObject _unit, int _init, int _coh, int _def, int _reg, int _dam) { unit = _unit; type = cellType.Occupied; initiativeMod = _init; cohesionMod = _coh; defenceMod = _def; regenCohMod = _reg; damageMod = _dam; }
}
public class Hero : MonoBehaviour
{
    public string heroName;
    [SerializeField] private int modinit = 0;
    [SerializeField] private int modcoh = 0;
    public List<GameObject> bannersList = new List<GameObject> { };
    public struct Army { public int lineNumber; public int columnNumber; public ArmyUnitClass unit; }
    public List<List<ArmyCell>> ArmyFormation = new List<List<ArmyCell>>();
    private void Awake()
    {

    }
    private void Start()
    {
        for (int i = 0; i < 5; i++)
        {
            var _tempList = new List<ArmyCell>();
            for (int _i = 0; _i < 5; _i++)
            {
                _tempList.Add(new ArmyCell(cellType.Forbidden));
            }
            ArmyFormation.Add(_tempList);
        }
        ArmyFormation[0][1] = new ArmyCell(cellType.Empty);
        ArmyFormation[0][2] = new ArmyCell(cellType.Empty);
        ArmyFormation[0][3] = new ArmyCell(cellType.Empty);
        ArmyFormation[1][2] = new ArmyCell(cellType.Empty);
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
    public void IsUnitAlive(ArmyUnitClass checkedunit)
    {
        //if (checkedunit.unit.GetUnitHP() == 0) { Console.WriteLine($"--Unit {checkedunit.unit.unitname} DESTOYED--"); bannersList.Remove(checkedunit); checkedunit.unit = null; }
        //if (checkedunit.GetUnitHP() == 0) { Console.WriteLine($"--Unit {checkedunit.unitname} DESTOYED--"); bannersList.Remove(checkedunit); checkedunit = null; }
    }
    public void AddBannerList(GameObject unit)
    {
        bannersList.Add(unit);
        unit.GetComponent<ArmyUnitClass>().ApplyHeroModifyers(modinit, modcoh);
        unit.transform.SetParent(transform);
    }
    public int GetMaxInitiative()
    {
        int MaxInit = bannersList[0].GetComponent<ArmyUnitClass>().GetUnitCharacteristics().Item1.ucunitinitiative;
        for (int i = 0; i < bannersList.Count; i++)
        {
            if (MaxInit < bannersList[i].GetComponent<ArmyUnitClass>().GetUnitCharacteristics().Item1.ucunitinitiative) { MaxInit = bannersList[i].GetComponent<ArmyUnitClass>().GetUnitCharacteristics().Item1.ucunitinitiative; }
        }
        return MaxInit;
    }
    public int GetMinInitiative()
    {
        int MinInit = bannersList[0].GetComponent<ArmyUnitClass>().GetUnitCharacteristics().Item1.ucunitinitiative;
        for (int i = 0; i < bannersList.Count; i++)
        {
            if (MinInit > bannersList[i].GetComponent<ArmyUnitClass>().GetUnitCharacteristics().Item1.ucunitinitiative) { MinInit = bannersList[i].GetComponent<ArmyUnitClass>().GetUnitCharacteristics().Item1.ucunitinitiative; }
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
