using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    [Header("Characteristics")]
    public string heroName;
    public List<GameObject> bannersList = new List<GameObject> { };
    
    [Header("HeroModifiers")]
    [SerializeField] private int modinit = 0;
    [SerializeField] private int modcoh = 0;
    private void Awake()
    {
    }
    // public void verifyField()
    // {
    //     var cycle = new List<int[]>
    //     {
    //         new int[2] { 0, 1 }, new int[2] { 0, -1 }, new int[2] { 1, 0 }, new int[2] { -1, 0 }
    //     };
    //     List<int[]> cellsToAvailable = new List<int[]>();
    //     List<int[]> cellsToNotAvailable = new List<int[]>(); 
    //     List<int[]> cellsToRemove = new List<int[]>(); 
    //     // TODO to FOR and indexes
    //     foreach (Formation line in ArmyFormation)
    //     {
    //         foreach (Squad column in line.ArmyLine)
    //         {
    //             bool neigbour = false;
    //             foreach (var couple in cycle)
    //             {
    //                 int newline = Mathf.Clamp(column.Line + couple[0], 0, maxArmyDepth - 1);
    //                 int newcolumn = Mathf.Clamp(column.Column + couple[1], 0, maxArmyWigth - 1);
    //                 if (ArmyFormation[newline].ArmyLine[newcolumn].Type == CellType.Occupied) {neigbour = true;}
    //                 //Debug.Log($"{column.Line}_{column.Column} checking {newline}_{newcolumn} it is {neigbour.ToString()}");
    //             }
    //             if (neigbour && column.Type==CellType.NotAvailable) cellsToAvailable.Add(new int[2] { column.Line, column.Column });
    //             else if (!neigbour && column.Type==CellType.Available && (column.Line,column.Column)!=(startingFieldPosition[0],startingFieldPosition[1])) cellsToNotAvailable.Add(new int[2] { column.Line, column.Column });
    //             else if (!neigbour && column.Type==CellType.Occupied && (column.Line,column.Column)!=(startingFieldPosition[0],startingFieldPosition[1])) cellsToRemove.Add(new int[2] { column.Line, column.Column });
    //         }
    //     }
    //     foreach (var cell in cellsToAvailable)
    //     {
    //         ArmyFormation[cell[0]].ChangeType(CellType.Available, cell[1]);
    //         //Debug.Log($"{cell[0]}_{cell[1]} now Available");
    //     }
    //     foreach (var cell in cellsToNotAvailable)
    //     {
    //         ArmyFormation[cell[0]].ChangeType(CellType.NotAvailable, cell[1]);
    //         //Debug.Log($"{cell[0]}_{cell[1]} now NotAvailable");
    //     }
    //     foreach (var cell in cellsToRemove)
    //     {
    //         ArmyFormation[cell[0]].RemoveUnit(CellType.NotAvailable, cell[1]);
    //         //Debug.Log($"{cell[0]}_{cell[1]} now unit removed and NotAvailable");
    //         verifyField();
    //     }
    // }
    
//------------------------------------
    public void modifyHero(string n, int init, int coh) { heroName = n; modinit = init; modcoh = coh; }
    public void AddBannerList(GameObject unit)
    {
        bannersList.Add(unit);
        unit.GetComponent<ArmyUnitClass>().ApplyHeroModifyers(modinit, modcoh);
        unit.transform.SetParent(transform);
    }
}
