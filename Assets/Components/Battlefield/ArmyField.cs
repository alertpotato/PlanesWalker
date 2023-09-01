using System.Collections.Generic;
using UnityEngine;
public class ArmyField : MonoBehaviour
{
    [Header("Field Graphics")]
    public List<List<GameObject>> cellList = new List<List<GameObject>>();
    public float armyCellSpacing = 1.3f;
    
    [Header("Components")]
    public GameObject ArmyFieldCell;
    public Hero YourHero;
    public Hero EnemyHero;
    public UnitGraphic UnitSprites;
    private void Start()
    {
        transform.position = new Vector3(0, 0, 8);
    }
    private void Update()
    {
        float stepFromLeft = (Screen.width / 2.5f);
        float stepFromTop = Screen.height * 0.9f;
        Vector3 ScreenPos = Camera.main.ScreenToWorldPoint(new Vector3(stepFromLeft,stepFromTop,transform.position.z));
        foreach (Formation line in YourHero.ArmyFormation)
        {
            foreach (Squad column in line.ArmyLine)
            {
                cellList[column.Line][column.Column].transform.position = new Vector3(ScreenPos.x - column.Line * armyCellSpacing, ScreenPos.y - column.Column * armyCellSpacing, transform.position.z);
                if (column.Type == CellType.NotAvailable)
                {
                    cellList[column.Line][column.Column].GetComponent<ArmyCellScript>().ChangeSprite(UnitSprites.GetIconSpriteByName("notavailable"));
                }
                else if (column.Type == CellType.Available)
                {
                    cellList[column.Line][column.Column].GetComponent<ArmyCellScript>().ChangeSprite(UnitSprites.GetIconSpriteByName("available"));
                }
                else if (column.Type == CellType.Occupied)
                {
                    cellList[column.Line][column.Column].GetComponent<ArmyCellScript>().ChangeSprite(UnitSprites.GetIconSpriteByName(column.Unit.UnitName));
                }
            }
        }
    }

    public void InicializeField(Hero yourHero, Hero enemyHero)
    {
        YourHero = yourHero;
        EnemyHero = enemyHero;

        for (int line = 0; line < YourHero.ArmyFormation.Count; line++)
        {
            var tempList = new List<GameObject>();
            for (int column = 0; column < YourHero.ArmyFormation[line].ArmyLine.Count; column++)
            {
                GameObject cell = Instantiate(ArmyFieldCell,transform);
                cell.GetComponent<ArmyCellScript>().InicializeCell(YourHero.ArmyFormation[line].ArmyLine[column]);
                cell.name = $"Squad_{line}_{column}";
                tempList.Add(cell);
            }
            cellList.Add(tempList);
        }
    }
}
