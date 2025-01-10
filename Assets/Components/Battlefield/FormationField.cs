using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using System.Linq;

[Serializable]
public enum FormationType { Frontline, Support, Reserve, Flank1, Flank2 };

[Serializable]
public class Company
{
    public GameObject Unit;
    public int Position;
    public FormationType Type;
    public FormationField Field;
    public Company(int pos,FormationType type,FormationField field) 
    {
        Unit = null; 
        Position = pos;
        Type = type;
        Field = field;
    }
}
//----------------------------------------------------
[CreateAssetMenu]
public class FormationField : ScriptableObject
{
    public List<Company> Formation = new List<Company>();
    public Hero FieldOwner;

    public void OnRoundEnd() //Must be called at the end of the round
    {
        FrontShift();
        //FrontSquash();
    }
    public void OnBattleEnd() //Must be called at the end of the battle
    {

    }

    private void FrontShift() // If front line is empty shift flanks or sup or reserve to front line
    {
        var onField = GetOnFieldcompanies();
        var onFieldFront = onField.Where(company => company.Type == FormationType.Frontline).ToList();
        var onFieldflank = onField.Where(company => company.Type == FormationType.Flank1||company.Type == FormationType.Flank2).ToList();
        var onFieldSup = onField.Where(company => company.Type == FormationType.Support).ToList();
        var onFieldRes = onField.Where(company => company.Type == FormationType.Reserve).ToList();
        var frontComps = Formation.Where(company => company.Type == FormationType.Frontline).ToList();
        
        List<Company> compToShift = new List<Company>();
        if (onFieldflank.Count != 0) compToShift = onFieldflank;
        else if (onFieldSup.Count != 0) compToShift = onFieldSup;
        else if (onFieldRes.Count != 0) compToShift = onFieldRes;
        if (onFieldFront.Count == 0 && compToShift.Count!=0)
        {
            Debug.Log("Front shifting");
            foreach (var company in frontComps)
            {
                company.Unit = compToShift.First().Unit;
                compToShift.First().Unit = null;
                compToShift.Remove(compToShift.First());
                company.Unit.GetComponent<ArmyUnitClass>().InitializeAbilities(company);
                if (compToShift.Count == 0) break;
            }
        }
    }
    //TODO Temp stupid solution
    /*private void FrontSquash()
    {
        var comp1 = Formation[0].Line[1];
        var comp2 = Formation[0].Line[2];
        var comp3 = Formation[0].Line[3];
        if (comp1.Type == CompanyType.Available && comp2.Type == CompanyType.Available &&
            comp3.Type == CompanyType.Occupied)
        {
            var unit = comp3.Unit;
            RemoveUnitFromField(unit);
            comp2.Type = CompanyType.Occupied;
            comp2.Unit = unit;
            unit.GetComponent<ArmyUnitClass>().InitializeAbilities(comp2);
        }
        if (comp1.Type == CompanyType.Occupied && comp2.Type == CompanyType.Available &&
            comp3.Type == CompanyType.Available)
        {
            var unit = comp1.Unit;
            RemoveUnitFromField(unit);
            comp2.Type = CompanyType.Occupied;
            comp2.Unit = unit;
            unit.GetComponent<ArmyUnitClass>().InitializeAbilities(comp2);
        }
    }
*/
    public void RemoveUnitFromField(GameObject unit)
    {
        var onField = GetOnFieldcompanies();
        var comp = onField.Where(un => un.Unit == unit).First();
        if (comp != null)
        {
            comp.Unit = null;
        }
    }

    public bool AddUnitToFormation(Company compTo, GameObject unit,FormationField opposingFormation)
    {
        bool answer = false;

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
        if (unit.GetComponent<ArmyUnitClass>().SupplyMultiplier==0)
        {
            Debug.LogWarning($"{unit.name} does not have sufficient supplies!");
            return answer;
        }

        compTo.Unit=unit;
        answer = true;
        unit.GetComponent<ArmyUnitClass>().InitializeAbilities(compTo,this,opposingFormation);

        return answer;
    }

    public void RebuildField(Dictionary<FormationType, int> avaliablePlaces) // Clearing field making all companys in avaliablePlaces
    {
        Formation.Clear();
        foreach (var place in avaliablePlaces)
        {
            for (int i = 0; i < place.Value; i++)
            {
                Formation.Add(new Company(i,place.Key,this));
            }
        }
    }

    public void ClearField() // Clear field from all units
    {
        var onField = GetOnFieldcompanies();
        foreach (var comp in onField)
        {
            comp.Unit = null;
        }
    }

    public void InitializeField(Hero owner) // Creating field for first time with maxArmyDepth and maxArmyWigth
    {
        Formation.Clear();
        FieldOwner = owner;
    }

    public List<Company> GetOnFieldcompanies()
    {
        List<Company> compList = new List<Company>();
        foreach (var comp in Formation)
        {
            if (comp.Unit != null) compList.Add(comp);
        }
        return compList;
    }
    public List<Company> GetAvaliableFields()
    {
        List<Company> compList = new List<Company>();
        foreach (var comp in Formation)
        {
            if (comp.Unit == null) compList.Add(comp);
        }
        return compList;
    }
    private bool IsCompanyOnField(GameObject unit) //Checks if unit already assigned to any company(field cell)
    {
        bool answer = false;
        foreach (var comp in Formation)
        {
            if (comp.Unit == unit) answer = true;
        }
        return answer;
    }
}
