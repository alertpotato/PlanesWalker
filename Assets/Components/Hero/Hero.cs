using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public enum CellType { Available, NotAvailable, Occupied };
[Serializable]
public class Formation
{
    public List<Squad> ArmyLine;
    public Formation(List<Squad> army)
    {
        ArmyLine = army;
    }
    public void ChangeType(CellType type,int squadIndex)
    {
        var squad = ArmyLine[squadIndex];
        squad.Type = type;
        ArmyLine[squadIndex] = squad;
    }
    public void ChangeUnit(GameObject unit,int squadIndex)
    {
        var squad = ArmyLine[squadIndex];
        squad.Type = CellType.Occupied;
        squad.Unit = unit;
        ArmyLine[squadIndex] = squad;
    }
    public void RemoveUnit(CellType type,int squadIndex)
    {
        var squad = ArmyLine[squadIndex];
        squad.Type = type;
        squad.Unit = null;
        ArmyLine[squadIndex] = squad;
    }
}
[Serializable]
public struct Squad
{
    public GameObject Unit;
    public int Line;
    public int Column;
    public CellType Type;
    public Squad(CellType type,int line,int column) {
        Unit = null; 
        Type = type;
        Line = line;
        Column = column;
    }
}
public class Hero : MonoBehaviour
{
    public List<Formation> ArmyFormation;
    [Header("Characteristics")]
    public string heroName;
    public List<GameObject> bannersList = new List<GameObject> { };
    
    [Header("HeroModifiers")]
    [SerializeField] private int modinit = 0;
    [SerializeField] private int modcoh = 0;
    
    [Header("Private variables")]
    public int maxArmyWigth = 5;
    public int maxArmyDepth = 3;
    public int[] startingFieldPosition= new int[2] { 0, 2 };
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
        ArmyFormation[0].ChangeType(CellType.Available, 2);
        ArmyFormation[0].ChangeType(CellType.Available, 3);
        ArmyFormation[0].ChangeType(CellType.Available, 1);
        ArmyFormation[1].ChangeType(CellType.Available, 2);
    }
    public bool AddUnitToFormation((int,int) banner, GameObject unit)
    {
        bool answer = false;
        Squad toCell = ArmyFormation[banner.Item1].ArmyLine[banner.Item2];
        if (toCell.Type == CellType.NotAvailable) {Debug.LogWarning($"{toCell} not available");return answer;}
        if (!bannersList.Contains(unit)) { Debug.LogWarning($"{unit.name} not in the banner list");return answer;}
        if (IsUnitInArmy(unit)) {Debug.LogWarning($"{unit.name} already on the field");return answer;}
        if (toCell.Type == CellType.Available)
        {
            ArmyFormation[banner.Item1].ChangeUnit(unit,banner.Item2);
            //verifyField();
            answer = true;
            foreach (var ability in unit.GetComponent<ArmyUnitClass>().Abilities)
            {
                ability.InitAbility(ArmyFormation[banner.Item1].ArmyLine[banner.Item2],this);
            }
        }
        return answer;
    }
    public void RemoveUnitFromFormation((int,int) banner)
    {
        Squad toCell = ArmyFormation[banner.Item1].ArmyLine[banner.Item2];
        if (toCell.Type == CellType.Occupied)
        {
            ArmyFormation[banner.Item1].RemoveUnit(CellType.Available,banner.Item2);
            //verifyField();
        }
    }
    public void RemoveAllFormations()
    {
        for (int line = 0;  line < ArmyFormation.Count;  line++)
        {
            for (int column = 0; column < ArmyFormation[line].ArmyLine.Count; column++)
            {
                if (ArmyFormation[line].ArmyLine[column].Type==CellType.Occupied) ArmyFormation[line].RemoveUnit(CellType.Available,column);
            }
        }
        //verifyField();
    }
    public void verifyField()
    {
        var cycle = new List<int[]>
        {
            new int[2] { 0, 1 }, new int[2] { 0, -1 }, new int[2] { 1, 0 }, new int[2] { -1, 0 }
        };
        List<int[]> cellsToAvailable = new List<int[]>();
        List<int[]> cellsToNotAvailable = new List<int[]>(); 
        List<int[]> cellsToRemove = new List<int[]>(); 
        // TODO to FOR and indexes
        foreach (Formation line in ArmyFormation)
        {
            foreach (Squad column in line.ArmyLine)
            {
                bool neigbour = false;
                foreach (var couple in cycle)
                {
                    int newline = Mathf.Clamp(column.Line + couple[0], 0, maxArmyDepth - 1);
                    int newcolumn = Mathf.Clamp(column.Column + couple[1], 0, maxArmyWigth - 1);
                    if (ArmyFormation[newline].ArmyLine[newcolumn].Type == CellType.Occupied) {neigbour = true;}
                    //Debug.Log($"{column.Line}_{column.Column} checking {newline}_{newcolumn} it is {neigbour.ToString()}");
                }
                if (neigbour && column.Type==CellType.NotAvailable) cellsToAvailable.Add(new int[2] { column.Line, column.Column });
                else if (!neigbour && column.Type==CellType.Available && (column.Line,column.Column)!=(startingFieldPosition[0],startingFieldPosition[1])) cellsToNotAvailable.Add(new int[2] { column.Line, column.Column });
                else if (!neigbour && column.Type==CellType.Occupied && (column.Line,column.Column)!=(startingFieldPosition[0],startingFieldPosition[1])) cellsToRemove.Add(new int[2] { column.Line, column.Column });
            }
        }
        foreach (var cell in cellsToAvailable)
        {
            ArmyFormation[cell[0]].ChangeType(CellType.Available, cell[1]);
            //Debug.Log($"{cell[0]}_{cell[1]} now Available");
        }
        foreach (var cell in cellsToNotAvailable)
        {
            ArmyFormation[cell[0]].ChangeType(CellType.NotAvailable, cell[1]);
            //Debug.Log($"{cell[0]}_{cell[1]} now NotAvailable");
        }
        foreach (var cell in cellsToRemove)
        {
            ArmyFormation[cell[0]].RemoveUnit(CellType.NotAvailable, cell[1]);
            //Debug.Log($"{cell[0]}_{cell[1]} now unit removed and NotAvailable");
            verifyField();
        }
    }

    public bool IsUnitInArmy(GameObject unit)
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
    private List<Squad> PossibleSquads()
    {
        List<Squad> possibleSquads = new List<Squad>();
        foreach (Formation line in ArmyFormation)
        {
            foreach (Squad column in line.ArmyLine)
            {
                if (column.Type==CellType.Available) possibleSquads.Add(column);
            }
        }
        return possibleSquads;
    }
    public void RandomUnitAllocating(Hero opposingHero)
    {
        List<GameObject> avalaibleUnits = new List<GameObject>();
        foreach (var unit in bannersList)
        {
            if (!IsUnitInArmy(unit)) avalaibleUnits.Add(unit);
        }
        List<Squad> availableSquads = PossibleSquads();
        availableSquads = availableSquads.OrderBy(x=>x.Column).ToList();
        if (avalaibleUnits.Count > 0)
        {
            AddUnitToFormation((availableSquads[0].Line, availableSquads[0].Column), avalaibleUnits[0]);
        }
    }

//------------------------------------
    public void modifyHero(string n, int init, int coh) { heroName = n; modinit = init; modcoh = coh; }
    public void AddBannerList(GameObject unit)
    {
        bannersList.Add(unit);
        unit.GetComponent<ArmyUnitClass>().ApplyHeroModifyers(modinit, modcoh);
        unit.transform.SetParent(transform);
    }
}
