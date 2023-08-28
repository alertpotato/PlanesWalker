using System.Collections.Generic;
using UnityEngine;
public class ArmyField : MonoBehaviour
{
    [SerializeField] private List<Sprite> spriteUiList;
    [SerializeField] public List<List<GameObject>> cellList;
    public GameObject ArmyFieldCell;
    public Hero YourHero;
    public Hero EnemyHero;

    private void Awake()
    {
        cellList = new List<List<GameObject>>();
    }

    private void Start()
    {
        spriteUiList = new List<Sprite>(Resources.LoadAll<Sprite>("Sprites/ui"));
        transform.position = new Vector3(0, 0, -2);
    }
    private void Update()
    {
        float step = spriteUiList[0].rect.width * 3.5f;
        float stepFromLeft = Screen.width / 2;
        float stepFromTop = Screen.height * 0.9f;
        int xcell = -1; int ycell = -1;
        foreach (List<ArmyCell> _armyCelllist in YourHero.ArmyFormation)
        {
            xcell++; ycell = -1;
            foreach (ArmyCell _armyCell in _armyCelllist)
            {
                ycell++;
                cellList[xcell][ycell].transform.position = Camera.main.ScreenToWorldPoint(new Vector3(stepFromLeft - xcell * step, stepFromTop - ycell * step, 8));
                if (_armyCell.Type == CellType.NotAvailable)
                {
                    //cellList[xcell][ycell].GetComponent<ArmyCellScript>().ChangeSprite(listOfCommonObjects.GetSpriteByName("armyCellF", spriteUiList));
                    cellList[xcell][ycell].GetComponent<ArmyCellScript>().ChangeSprite(spriteUiList[1]);
                }
                else if (_armyCell.Type == CellType.Available)
                {
                    //cellList[xcell][ycell].GetComponent<ArmyCellScript>().ChangeSprite(listOfCommonObjects.GetSpriteByName("armyCellE", spriteUiList));
                    cellList[xcell][ycell].GetComponent<ArmyCellScript>().ChangeSprite(spriteUiList[0]);
                }
            }
        }
    }
    public void InicializeField(Hero yourHero,Hero enemyHero)
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
