using System.Collections.Generic;
using UnityEngine;
public class ArmyField : MonoBehaviour
{
    public Sprite spriteAvailable;
    public Sprite spriteNotAvailable;
    [SerializeField] public List<List<GameObject>> cellList = new List<List<GameObject>>();
    public GameObject ArmyFieldCell;
    public Hero YourHero;
    public Hero EnemyHero;
    
    private void Start()
    {
        transform.position = new Vector3(0, 0, 8);
    }
    private void Update()
    {
        float step = spriteAvailable.rect.width*5f; 
        float stepFromLeft = Screen.width / 2;
        float stepFromTop = Screen.height * 0.9f;
        int xcell = -1; int ycell = -1;
        foreach (List<ArmyCell> armyCelllist in YourHero.ArmyFormation)
        {
            xcell++; ycell = -1;
            foreach (ArmyCell armyCell in armyCelllist)
            {
                ycell++;
                cellList[xcell][ycell].transform.position = Camera.main.ScreenToWorldPoint(new Vector3(stepFromLeft - xcell * step, stepFromTop - ycell * step, transform.position.z));
                if (armyCell.Type == CellType.NotAvailable)
                {
                    //cellList[xcell][ycell].GetComponent<ArmyCellScript>().ChangeSprite(listOfCommonObjects.GetSpriteByName("armyCellF", spriteUiList));
                    cellList[xcell][ycell].GetComponent<ArmyCellScript>().ChangeSprite(spriteNotAvailable);
                }
                else if (armyCell.Type == CellType.Available)
                {
                    //cellList[xcell][ycell].GetComponent<ArmyCellScript>().ChangeSprite(listOfCommonObjects.GetSpriteByName("armyCellE", spriteUiList));
                    cellList[xcell][ycell].GetComponent<ArmyCellScript>().ChangeSprite(spriteAvailable);
                }
            }
        }
    }

    public void InicializeField(Hero yourHero, Hero enemyHero)
    {
        YourHero = yourHero;
        EnemyHero = enemyHero;

        for (int column = 0; column < YourHero.ArmyFormation.Count; column++)
        {
            var tempList = new List<GameObject>();
            for (int row = 0; row < YourHero.ArmyFormation[column].Count; row++)
            {
                GameObject cell = Instantiate(ArmyFieldCell,transform);
                tempList.Add(cell);
            }
            cellList.Add(tempList);
        }
    }
}
