using System.Collections.Generic;
using UnityEngine;
public class Battlefield : MonoBehaviour
{
    [Header("Field Graphics")]
    public List<List<GameObject>> yourCellList = new List<List<GameObject>>();
    public List<List<GameObject>> enemyCellList = new List<List<GameObject>>();
    public float armyCellSpacing = 1.3f;
    
    [Header("Components")]
    public Hero YourHero;
    public Hero EnemyHero;
    public GameObject ArmyFieldCell;
    public GameObject YourHeroField;
    public GameObject EnemyHeroField;
    public UnitGraphic UnitSprites;
    public BattlefieldLogic logic;
    
    private void Start()
    {
        transform.position = new Vector3(0, 0, 8.5f);
    }
    private void Update()
    {
        UpdateHeroField(YourHero,yourCellList,-1,0.45f);
        UpdateHeroField(EnemyHero,enemyCellList,1,0.55f);
        
        foreach (var ability in logic.AbilitiesOrder)
        {
            List<List<GameObject>> cellList;
            Debug.Log("asd");
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
    public void InicializeField(Hero yourHero, Hero enemyHero)
    {
        YourHero = yourHero;
        EnemyHero = enemyHero;
        //TODO make field completely reset
        InicializeHeroField(YourHero,YourHeroField,yourCellList);
        InicializeHeroField(EnemyHero,EnemyHeroField,enemyCellList);
    }
    private void InicializeHeroField(Hero hero,GameObject parent,List<List<GameObject>> cellList)
    {
        for (int line = 0; line < hero.ArmyFormation.Count; line++)
        {
            var tempList = new List<GameObject>();
            for (int column = 0; column < hero.ArmyFormation[line].ArmyLine.Count; column++)
            {
                GameObject cell = Instantiate(ArmyFieldCell,parent.gameObject.transform);
                cell.GetComponent<ArmyCellScript>().InicializeCell(line,column);
                cell.name = $"{hero.heroName}_{line}_{column}";
                tempList.Add(cell);
            }
            cellList.Add(tempList);
        }
    }

}
