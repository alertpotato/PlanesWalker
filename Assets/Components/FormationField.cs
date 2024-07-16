using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using System.Linq;

[Serializable]
public enum CompanyType { Available, NotAvailable, Occupied };

[Serializable]
public class FormationLine
{
    public List<Company> Line;
    public FormationLine(List<Company> army)
    {
        Line = army;
    }
    public void ChangeType(CompanyType type,int index)
    {
        var company = Line[index];
        company.Type = type;
        Line[index] = company;
    }
    public void ChangeUnit(GameObject unit,int index)
    {
        var company = Line[index];
        company.Type = CompanyType.Occupied;
        company.Unit = unit;
        Line[index] = company;
    }
    public void RemoveUnit(CompanyType type,int index)
    {
        var company = Line[index];
        company.Type = type;
        company.Unit = null;
        Line[index] = company;
    }
}
[Serializable]
public class Company
{
    public GameObject Unit;
    public (int, int) Banner;
    public CompanyType Type;
    public FormationField Field;
    public Company(CompanyType type,(int,int) banner,FormationField field) {
        Unit = null; 
        Type = type;
        Banner = banner;
        Field = field;
    }
}
//----------------------------------------------------
[CreateAssetMenu]
public class FormationField : ScriptableObject
{
    public List<FormationLine> Formation = new List<FormationLine>();
    public Hero FieldOwner;

    [Header("Private variables")] public int maxArmyWigth = 5;
    public int maxArmyDepth = 3;
    
    public Company GetCompany((int, int) banner)
    {
        var ASD = Formation.SelectMany(formationLine => formationLine.Line).Where(line => line.Banner == banner);
        //Debug.Log(ASD.First() + " | " + ASD.First().Banner + " | " + banner);
        return ASD.First();
    }

    public bool AddUnitToFormation((int, int) banner, GameObject unit,FormationField opposingFormation)
    {
        bool answer = false;
        Company toComp = Formation[banner.Item1].Line[banner.Item2];
        if (toComp.Type == CompanyType.NotAvailable)
        {
            Debug.LogWarning($"{toComp} not available");
            return answer;
        }

        if (!FieldOwner.GetComponent<Hero>().bannersList.Contains(unit))
        {
            Debug.LogWarning($"{unit.name} not in the field owner banner list");
            return answer;
        }

        if (IsCompanyOnField(unit))
        {
            Debug.LogWarning($"{unit.name} already on the field");
            return answer;
        }

        if (toComp.Type == CompanyType.Available)
        {
            Formation[banner.Item1].ChangeUnit(unit, banner.Item2);
            answer = true;
            foreach (var ability in unit.GetComponent<ArmyUnitClass>().Abilities)
            {
                ability.InitAbility(toComp,this,opposingFormation);
            }
        }
        return answer;
        
    }

    public void RebuildField(List<(int, int)> avaliablePlaces) // Clearing field making all companys in avaliablePlaces
    {
        for (int line = 0; line < maxArmyDepth; line++)
        {
            for (int column = 0; column < maxArmyWigth; column++)
            {
                Formation[line].RemoveUnit(CompanyType.NotAvailable, column);
                if (avaliablePlaces.Contains((line, column)))
                {
                    Formation[line].RemoveUnit(CompanyType.Available, column);
                }
            }
        }
    }

    public void ClearField()
    {
        for (int line = 0; line < maxArmyDepth; line++)
        {
            for (int column = 0; column < maxArmyWigth; column++)
            {
                if (Formation[line].Line[column].Type==CompanyType.Occupied)
                {
                    Formation[line].RemoveUnit(CompanyType.Available, column);
                }
            }
        }
    }

    public void InitializeField(Hero owner) // Creating field for first time with maxArmyDepth and maxArmyWigth
    {
        Formation.Clear();
        for (int line = 0; line < maxArmyDepth; line++)
        {
            var formation = new List<Company>();
            for (int column = 0; column < maxArmyWigth; column++)
            {
                formation.Add(new Company(CompanyType.Available, (line, column),this));
            }

            Formation.Add(new FormationLine(formation));
        }

        FieldOwner = owner;
    }

    public List<Company> GetOnFieldcompanies()
    {
        List<Company> compList = new List<Company>();
        foreach (var formation in Formation)
        {
            foreach (var company in formation.Line)
            {
                if (company.Type == CompanyType.Occupied) compList.Add(company);
            }
        }
        return compList;
    }
    private bool IsCompanyOnField(GameObject unit) //Checks if unit already assigned to any company(field cell)
    {
        bool answer = false;
        foreach (var formation in Formation)
        {
            foreach (var company in formation.Line)
            {
                if (company.Unit == unit) answer = true;
            }
        }

        return answer;
    }
}
