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
    public Hero unitOwner;
    public List<(int X, int Y)> occupiedPositions;
    public string position0;
    public FormationType Type;
    public Company(List<(int X, int Y)> positions,FormationType type=FormationType.Frontline,Hero owner=null) 
    {
        occupiedPositions = new List<(int X, int Y)>();
        Unit = null;
        unitOwner = owner;
        occupiedPositions.AddRange(positions);
        Type = type;
        position0 = positions[0].ToString();
    }
}
//----------------------------------------------------
[CreateAssetMenu]
public class FormationManager : ScriptableObject
{
    public List<Company> PlayerFormation = new List<Company>();
    public List<Company> EnemyFormation = new List<Company>();
    [SerializeField]private int PlayerStartingLine;
    [SerializeField]private int EnemyStartingLine;
    public Hero PlayerHero;
    public Hero EnemyHero;
    [SerializeField]private List<(int X, int Y)> PlayerPossibleDeploymentPositions = new List<(int X, int Y)>();
    [SerializeField]private List<(int X, int Y)> EnemyPossibleDeploymentPositions = new List<(int X, int Y)>();
    private List<(int X, int Y)> checkAround = new List<(int X, int Y)>{(0, -1),(1, 0),(0, 1),(-1, 0)};

    public void InitializeField(Hero playerHero,Hero enemyHero) // Creating field for first time with maxArmyDepth and maxArmyWigth
    {
        PlayerFormation.Clear();
        EnemyFormation.Clear();
        PlayerHero = playerHero;
        EnemyHero = enemyHero;
    }
    public void CreateEmptyCompanies()
    {
        RemoveEmptyCompanies();
        CalculatePossibleDeploymentPositions();
        foreach (var pos in PlayerPossibleDeploymentPositions)
        {
            PlayerFormation.Add(new Company(new List<(int X, int Y)>{pos}, FormationType.Frontline,PlayerHero));
        }
        foreach (var pos in EnemyPossibleDeploymentPositions)
        {
            EnemyFormation.Add(new Company(new List<(int X, int Y)>{pos}, FormationType.Frontline,EnemyHero));
        }

        var logg = "EMPTY: ";
        foreach (var fof in PlayerFormation.Where(f => f.Unit == null))
        {
            logg += $"{fof.occupiedPositions[0].ToString()}--";
        }
        Debug.Log(logg);
    }
    //Deployment and movement logic
    public void CalculatePossibleDeploymentPositions()
    {
        PlayerPossibleDeploymentPositions.Clear();
        EnemyPossibleDeploymentPositions.Clear();
        if (PlayerFormation.Count == 0) PlayerPossibleDeploymentPositions.Add((0, PlayerStartingLine));
        else
        {
            var newPossiblePlayerList = new List<(int X, int Y)>();
            var deployedCompaniesPos = GetDeployedCompaniesPositions(PlayerHero);
            foreach (var comp in PlayerFormation)
            {
                foreach (var pos in comp.occupiedPositions)
                {
                    foreach (var checkPos in checkAround)
                    {
                        if (pos.Y+checkPos.Y>PlayerStartingLine) continue;
                        newPossiblePlayerList.Add((pos.X+checkPos.X, pos.Y+checkPos.Y));
                    }
                }
            }
            PlayerPossibleDeploymentPositions.AddRange(ListFunctions.RemoveDuplicatesAgainstOtherList(newPossiblePlayerList,deployedCompaniesPos));
        }

        if (EnemyFormation.Count == 0) EnemyPossibleDeploymentPositions.Add((0, EnemyStartingLine));
        else
        {
            var newPossiblEnemyList = new List<(int X, int Y)>();
            var deployedCompaniesPos = GetDeployedCompaniesPositions(EnemyHero);
            foreach (var comp in EnemyFormation)
            {
                foreach (var pos in comp.occupiedPositions)
                {
                    foreach (var checkPos in checkAround)
                    {
                        if (pos.Y+checkPos.Y<PlayerStartingLine) continue;
                        newPossiblEnemyList.Add((pos.X+checkPos.X, pos.Y+checkPos.Y));
                    }
                }
            }
            EnemyPossibleDeploymentPositions.AddRange(ListFunctions.RemoveDuplicatesAgainstOtherList(newPossiblEnemyList,deployedCompaniesPos));
        }
    }

    public List<(int X, int Y)> GetDeployedCompaniesPositions(Hero companyOwner = null)
    {
        var deployedCompanies = GetDeployedCompanies(companyOwner);
        var deployedCompaniesPos = new List<(int X, int Y)>();
        foreach (var comp in deployedCompanies)
        {
            deployedCompaniesPos.AddRange(comp.occupiedPositions);
        }
        return deployedCompaniesPos;
    }

    public List<Company> GetDeployedCompanies(Hero companyOwner = null, bool getEmpty = false)
    {
        List<Company> deployedCompanies = new List<Company>();
        if (companyOwner == PlayerHero)
            if (getEmpty) deployedCompanies.AddRange(PlayerFormation.Where(x=>x.Unit==null));
            else deployedCompanies.AddRange(PlayerFormation.Where(x=>x.Unit!=null));
        else if (companyOwner== EnemyHero)
            if (getEmpty) deployedCompanies.AddRange(EnemyFormation.Where(x=>x.Unit==null));
            else deployedCompanies.AddRange(EnemyFormation.Where(x=>x.Unit!=null));
        else
        {
            deployedCompanies.AddRange(PlayerFormation.Where(x=>x.Unit!=null));deployedCompanies.AddRange(EnemyFormation.Where(x=>x.Unit!=null));
        }
        return deployedCompanies;
    }

    public void RemoveEmptyCompanies()
    {
        var companiesToRemove = new List<Company>();
        companiesToRemove.AddRange(PlayerFormation.Where(x=>x.Unit==null));
        foreach (var comp in companiesToRemove)
        {
            PlayerFormation.Remove(comp);
        }
        companiesToRemove.Clear();
        companiesToRemove.AddRange(EnemyFormation.Where(x=>x.Unit==null));
        foreach (var comp in companiesToRemove)
        {
            EnemyFormation.Remove(comp);
        }
    }
    //Round logic
    public void OnRoundEnd() //Must be called at the end of the round
    {
        //FrontShift();
        //FrontSquash();
    }
    public void OnBattleEnd() //Must be called at the end of the battle
    {

    }
    //Get Set logic
    public void RemoveUnitFromField(GameObject unit)
    {
        var onField = GetDeployedCompanies();
        var comp = onField.Where(un => un.Unit == unit).First();
        if (comp != null)
        {
            comp.Unit = null;
        }
    }

    public bool AddUnitToFormation(Company compTo, GameObject unit)
    {
        bool answer = false;

        if (!compTo.unitOwner.bannersList.Contains(unit))
        {
            Debug.LogWarning($"{unit.name} not in the field owner banner list");
            return answer;
        }

        if (IsCompanyOnField(unit))
        {
            Debug.LogWarning($"{unit.name} already on the field");
            return answer;
        }
        //TODO New Supply mechanic
        //if (unit.GetComponent<ArmyUnitClass>().SupplyMultiplier==0)

        compTo.Unit=unit;
        answer = true;
        unit.GetComponent<ArmyUnitClass>().InitializeAbilities(compTo,this);

        return answer;
    }

    public void ClearField() // Clear field from all units
    {
        PlayerFormation.Clear();
        EnemyFormation.Clear();
    }
    //Logic logic
    private bool IsCompanyOnField(GameObject unit) //Checks if unit already assigned to any company(field cell)
    {
        bool answer = false;
        foreach (var comp in GetDeployedCompanies())
        {
            if (comp.Unit == unit) answer = true;
        }
        return answer;
    }

    public List<Company> GetNeighbouringCompanies(Company comp,bool returnAllies = false)
    {
        List<Company> possibleCompanies = new List<Company>();
        List<(int X, int Y)> possiblePos = new List<(int X, int Y)>();
        foreach (var compPos in comp.occupiedPositions)
        {
            foreach (var checkPos in checkAround)
            {
                possiblePos.Add((compPos.X+checkPos.X, compPos.Y+checkPos.Y));
            }
        }

        foreach (var pos in possiblePos)
        {
            Company posComp = null;
            var posComps =
                EnemyFormation.Where(x => x.occupiedPositions.Contains(pos) && x.unitOwner != comp.unitOwner);
            if (posComps.Count() > 0) posComp = EnemyFormation.Where(x => x.occupiedPositions.Contains(pos) && x.unitOwner!=comp.unitOwner).First();
            if (posComp != null && !possibleCompanies.Contains(posComp))
            {
                possibleCompanies.Add(posComp);
                Debug.Log($"comp:{comp.Unit.name} {comp.unitOwner.name} posCOmp:{posComp.unitOwner.name} {posComp.occupiedPositions[0]}");
            }
        }
        return possibleCompanies;
    }
    /*private void FrontShift() // If front line is empty shift flanks or sup or reserve to front line
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
private void FrontSquash()
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
}
