using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[Serializable] public struct Squad
{
    public GameObject Unit;
    public GameObject Cell;
    public int Line;
    public int Column;
    public CellType Type;
    public Squad(CellType type,int line,int column,GameObject cell) {
        Unit = null; 
        Type = type;
        Line = line;
        Column = column;
        Cell = cell;
    }
}
[Serializable] public class FieldRow
{
    public List<Squad> Row;
    public FieldRow(List<Squad> army)
    {
        Row = army;
    }
    public void ChangeType(CellType type,int squadIndex)
    {
        var squad = Row[squadIndex];
        squad.Type = type;
        Row[squadIndex] = squad;
    }
    public void ChangeUnit(GameObject unit,int squadIndex)
    {
        var squad = Row[squadIndex];
        squad.Type = CellType.Occupied;
        squad.Unit = unit;
        Row[squadIndex] = squad;
    }
    public void RemoveUnit(CellType type,int squadIndex)
    {
        var squad = Row[squadIndex];
        squad.Type = type;
        squad.Unit = null;
        Row[squadIndex] = squad;
    }
}
public class Battlefield : MonoBehaviour
{
    [Header("Field Graphics")]
    public List<List<GameObject>> yourCellList = new List<List<GameObject>>();
    public List<List<GameObject>> enemyCellList = new List<List<GameObject>>();
    public float armyCellSpacing = 1.3f;
    public List<List<GameObject>> hexList = new List<List<GameObject>>();
    public float hexSize = 1;
    public float fieldColumns = 9;
    public float fieldRows = 6;
    public List<FieldRow> Field;
    
    [Header("Components")]
    public Hero YourHero;
    public Hero EnemyHero;
    public GameObject ArmyFieldCell;
    public GameObject YourHeroField;
    public GameObject EnemyHeroField;
    public UnitGraphic UnitSprites;
    public BattlefieldLogic logic;

    private void Awake()
    {
        Field = new List<FieldRow>();
    }

    private void Start()
    {
        transform.position = new Vector3(0, 0, 8.5f);
    }

    public void InicializeHexField(int fieldColumnsLocal, int fieldRowsLocal)
    {
        float stepFromLeft = Screen.width * 0.2f;
        float stepFromTop = Screen.height * 0.5f;
        Vector3 ScreenPos = Camera.main.ScreenToWorldPoint(new Vector3(stepFromLeft,stepFromTop,transform.position.z));
        for (int col = 0; col < (fieldColumnsLocal-1); col++)
        {
            var tempList = new List<GameObject>();
            var fieldRow = new List<Squad>();
            for (int row = 0; row < (fieldRowsLocal-1); row++)
            {
                float hexOffset =0;
                if (row % 2 == 0) hexOffset = -hexSize * Mathf.Sqrt(3)/2;
                else hexOffset=0;
                
                GameObject cell = Instantiate(ArmyFieldCell,gameObject.transform);
                cell.GetComponent<ArmyCellScript>().InicializeCell(col,row);
                cell.name = $"{col}_{row}";
                cell.GetComponent<ArmyCellScript>().ChangeSprite(UnitSprites.GetIconSpriteByName("hex"));
                cell.transform.position = new Vector3(ScreenPos.x + col * hexSize * Mathf.Sqrt(3) + hexOffset, ScreenPos.y - row * hexSize * 3 / 2 , transform.position.z);
                tempList.Add(cell);
                fieldRow.Add(new Squad(CellType.NotAvailable,col,row,cell));
            }
            hexList.Add(tempList);
            Field.Add(new FieldRow(fieldRow));
        }
    }

    private void Update()
    {
        UpdateHeroField(YourHero,yourCellList,-1,0.45f);
        UpdateHeroField(EnemyHero,enemyCellList,1,0.55f);
        
        foreach (var ability in logic.AbilitiesOrder)
        {
            List<List<GameObject>> cellList;
            if (ability.YourHero == YourHero)
            {
                cellList = enemyCellList;
                foreach (var target in ability.AbilityTargets())
                {
                    cellList[target[0]][target[1]].GetComponent<ArmyCellScript>().GetAttackedFromLeft();
                }
            }
            else
            {
                cellList = yourCellList;
                foreach (var target in ability.AbilityTargets())
                {
                    cellList[target[0]][target[1]].GetComponent<ArmyCellScript>().GetAttackedFromRight();
                }
            }
        }
    }
    private void UpdateHeroField(Hero hero,List<List<GameObject>> cellList,float sine,float xMulti)
    {
        float stepFromLeft = (Screen.width * xMulti);
        float stepFromTop = Screen.height * 0.9f;
        Vector3 ScreenPos = Camera.main.ScreenToWorldPoint(new Vector3(stepFromLeft,stepFromTop,transform.position.z));
        foreach (Formation line in hero.ArmyFormation)
        {
            foreach (Squad column in line.ArmyLine)
            {
                cellList[column.Line][column.Column].transform.position = new Vector3(ScreenPos.x + sine * column.Line * armyCellSpacing, ScreenPos.y - column.Column * armyCellSpacing, transform.position.z);
                var cellArmy = cellList[column.Line][column.Column].GetComponent<ArmyCellScript>();
                if (column.Type == CellType.NotAvailable)
                {
                    cellArmy.ChangeSprite(UnitSprites.GetIconSpriteByName("notavailable"));
                    cellArmy.ClearAttack();
                }
                else if (column.Type == CellType.Available)
                {
                    cellArmy.ChangeSprite(UnitSprites.GetIconSpriteByName("available"));
                    cellArmy.ClearAttack();
                }
                else if (column.Type == CellType.Occupied)
                {
                    cellArmy.ChangeSprite(UnitSprites.GetIconSpriteByName(column.Unit.GetComponent<ArmyUnitClass>().UnitName));
                }
            }
        }
    }
    public bool AddUnitToFormation((int,int) banner, GameObject unit)
    {
        bool answer = false;
        Squad toCell = Field[banner.Item1].Row[banner.Item2];
        if (toCell.Type == CellType.NotAvailable) {Debug.LogWarning($"{toCell} not available");return answer;}
        if (IsUnitInArmy(unit)) {Debug.LogWarning($"{unit.name} already on the field");return answer;}
        if (toCell.Type == CellType.Available)
        {
            Field[banner.Item1].ChangeUnit(unit,banner.Item2);
            verifyField();
            answer = true;
            foreach (var ability in unit.GetComponent<ArmyUnitClass>().Abilities)
            {
                ability.InitAbility(Field[banner.Item1].Row[banner.Item2],this);
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
            verifyField();
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
        verifyField();
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

}
