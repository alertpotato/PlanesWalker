using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
public struct Company
{
    public GameObject Unit;
    public int Line;
    public int Column;
    public CompanyType Type;
    public Company(CompanyType type,int line,int column) {
        Unit = null; 
        Type = type;
        Line = line;
        Column = column;
    }
}
[CreateAssetMenu]
public class FormationField : ScriptableObject
{
    public List<FormationLine> Formation;
    
    [Header("Private variables")]
    public int maxArmyWigth = 5;
    public int maxArmyDepth = 3;
}
