using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEditor;
[Serializable] public struct Squad
{
    public GameObject Unit;
    public Vector2Int FieldPosition;
    public CellType Type;
    public Squad(CellType type,Vector2Int pos) {
        Unit = null; 
        Type = type;
        FieldPosition = pos;
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
        for (int col = 0; col < (fieldColumnsLocal-1); col++)
        {
            var fieldRow = new List<Squad>();
            for (int row = 0; row < (fieldRowsLocal-1); row++)
            {
                fieldRow.Add(new Squad(CellType.NotAvailable,new Vector2Int(col,row)));
            }
            Field.Add(new FieldRow(fieldRow));
        }
        
        float stepFromLeft = Screen.width * 0.2f;
        float stepFromTop = Screen.height * 0.5f;
        Vector3 ScreenPos = Camera.main.ScreenToWorldPoint(new Vector3(stepFromLeft,stepFromTop,transform.position.z));
        foreach (var col in Field)
        {
            var tempList = new List<GameObject>();
            foreach (var row in col.Row)
            {
                float hexOffset =0;
                if (row.FieldPosition.y % 2 == 0) hexOffset = -hexSize * Mathf.Sqrt(3)/2;
                else hexOffset=0;
                GameObject cell = Instantiate(ArmyFieldCell,gameObject.transform);
                cell.GetComponent<ArmyCellScript>().InicializeCell(row.FieldPosition);
                cell.name = $"{col}_{row}";
                cell.GetComponent<ArmyCellScript>().ChangeSprite(UnitSprites.GetIconSpriteByName("hex"));
                cell.transform.position = new Vector3(ScreenPos.x + row.FieldPosition.x * hexSize * Mathf.Sqrt(3) + hexOffset, ScreenPos.y - row.FieldPosition.y * hexSize * 3 / 2 , transform.position.z);
                tempList.Add(cell);
            }
            hexList.Add(tempList);
            
        }
    }

    private void Update()
    {
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
    private void UpdateField()
    {
        foreach (var col in Field)
        {
            foreach (var row in col.Row)
            {
                var pos = row.FieldPosition;
                //hexList[pos.x][pos.y].transform.position = new Vector3(ScreenPos.x + sine * column.Line * armyCellSpacing, ScreenPos.y - column.Column * armyCellSpacing, transform.position.z);
                var cell = hexList[pos.x][pos.y].GetComponent<ArmyCellScript>();
                if (row.Type == CellType.NotAvailable)
                {
                    cell.ChangeSprite(UnitSprites.GetIconSpriteByName("notavailable"));
                    cell.ClearAttack();
                }
                else if (row.Type == CellType.Available)
                {
                    cell.ChangeSprite(UnitSprites.GetIconSpriteByName("available"));
                    cell.ClearAttack();
                }
                else if (row.Type == CellType.Occupied)
                {
                    cell.ChangeSprite(UnitSprites.GetIconSpriteByName(row.Unit.GetComponent<ArmyUnitClass>().UnitName));
                }
            }
        }
    }
    public bool AddUnitToField((int,int) banner, GameObject unit, Hero hero)
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
                ability.InitAbility(Field[banner.Item1].Row[banner.Item2],hero);
            }
        }
        return answer;
    }
    public void RemoveUnitFromFormation((int,int) banner)
    {
        Squad toCell = Field[banner.Item1].Row[banner.Item2];
        if (toCell.Type == CellType.Occupied)
        {
            Field[banner.Item1].RemoveUnit(CellType.Available,banner.Item2);
            verifyField();
        }
    }
    public void RemoveAllFormations()
    {
        for (int col = 0;  col < Field.Count;  col++)
        {
            for (int row = 0; row < Field[col].Row.Count; row++)
            {
                if (Field[col].Row[row].Type==CellType.Occupied) Field[col].RemoveUnit(CellType.Available,row);
            }
        }
        verifyField();
    }
    public void verifyField()
    {
        var cycle = new List<int[]>
        {
            new int[2] { 0, -1 },new int[2] { +1, -1 }, new int[2] { 1, 0 }, new int[2] { 0, 1 },new int[2] { -1, 1 }, new int[2] { -1, 0 }
        };
        List<int[]> cellsToAvailable = new List<int[]>();
        List<int[]> cellsToNotAvailable = new List<int[]>(); 
        List<int[]> cellsToRemove = new List<int[]>(); 
        // TODO to FOR and indexes
        foreach (var col in Field)
        {
            foreach (var row in col.Row)
            {
                Vector2Int startingFieldPosition = new Vector2Int(-1, -1); // TODO start pos of hero????
                bool neigbour = false;
                foreach (var couple in cycle)
                {
                    int newline = Mathf.Clamp(row.FieldPosition.x + couple[0], 0, 999);
                    int newcolumn = Mathf.Clamp(row.FieldPosition.y + couple[1], 0, 999);
                    if (Field[newline].Row[newcolumn].Type == CellType.Occupied) {neigbour = true;}
                    //Debug.Log($"{column.Line}_{column.Column} checking {newline}_{newcolumn} it is {neigbour.ToString()}");
                }
                if (neigbour && row.Type==CellType.NotAvailable) cellsToAvailable.Add(new int[2] { row.FieldPosition.x,row.FieldPosition.y});
                else if (!neigbour && row.Type==CellType.Available && (row.FieldPosition)!=(startingFieldPosition)) cellsToNotAvailable.Add(new int[2] { row.FieldPosition.x, row.FieldPosition.y });
                else if (!neigbour && row.Type==CellType.Occupied && (row.FieldPosition)!=(startingFieldPosition)) cellsToRemove.Add(new int[2] { row.FieldPosition.x, row.FieldPosition.y });
            }
        }
        foreach (var cell in cellsToAvailable)
        {
            Field[cell[0]].ChangeType(CellType.Available, cell[1]);
            //Debug.Log($"{cell[0]}_{cell[1]} now Available");
        }
        foreach (var cell in cellsToNotAvailable)
        {
            Field[cell[0]].ChangeType(CellType.NotAvailable, cell[1]);
            //Debug.Log($"{cell[0]}_{cell[1]} now NotAvailable");
        }
        foreach (var cell in cellsToRemove)
        {
            Field[cell[0]].RemoveUnit(CellType.NotAvailable, cell[1]);
            //Debug.Log($"{cell[0]}_{cell[1]} now unit removed and NotAvailable");
            verifyField();
        }
    }

    public bool IsUnitInArmy(GameObject unit)
    {
        bool answer=false;
        foreach (var col in Field)
        {
            foreach (var row in col.Row)
            {
                if (row.Unit == unit) answer = true;
            }
        }
        return answer;
    }
    private List<Squad> PossibleSquads()
    {
        List<Squad> possibleSquads = new List<Squad>();
        foreach (var col in Field)
        {
            foreach (var row in col.Row)
            {
                if (row.Type==CellType.Available) possibleSquads.Add(row);
            }
        }
        return possibleSquads;
    }
    public void RandomUnitAllocating(Hero opposingHero)
    {
        List<GameObject> avalaibleUnits = new List<GameObject>();
        foreach (var unit in opposingHero.bannersList)
        {
            if (!IsUnitInArmy(unit)) avalaibleUnits.Add(unit);
        }
        List<Squad> availableSquads = PossibleSquads();
        availableSquads = availableSquads.OrderBy(x=>x.FieldPosition.x).ToList();
        if (avalaibleUnits.Count > 0)
        {
            AddUnitToField((availableSquads[0].FieldPosition.x, availableSquads[0].FieldPosition.y), avalaibleUnits[0],opposingHero);
        }
    }

}
