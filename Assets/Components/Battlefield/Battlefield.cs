using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
[RequireComponent(typeof(BattlefieldLogic))]
public class Battlefield : MonoBehaviour
{
    [Header("Field Graphics")]
    public List<GameObject> playerFieldList = new List<GameObject>();
    public List<GameObject> enemyFieldList = new List<GameObject>();
    public float armyCellSpacing = 2f;

    [Header("Components")] 
    public Camera MainCamera;
    public GameObject ArmyFieldCell;
    public GameObject YourHeroField;
    public GameObject EnemyHeroField;
    public UnitGraphic UnitSprites;
    public BattlefieldLogic logic;
    public FormationField PlayerFormation;
    public FormationField EnemyFormation;

    public void Initialize(Camera camera,FormationField playerFormation,FormationField enemyFormation)
    {
        MainCamera = camera;
        logic = transform.GetComponent<BattlefieldLogic>();
        logic.Battlefield = this;
        PlayerFormation = playerFormation;
        EnemyFormation = enemyFormation;
    }

    private void Start()
    {
        transform.position = new Vector3(0, 0, 8.5f);
    }
    public void UpdateField()
    {
        UpdateField(playerFieldList,-1,0.5f);
        UpdateField(enemyFieldList,1,0.65f);
        /*
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
        }*/
    }

    public void RebuildField(FormationField playerFormation, FormationField enemyFormation)
    {
        InitializeField(playerFormation, YourHeroField, playerFieldList);
        InitializeField(enemyFormation, EnemyHeroField, enemyFieldList);
        PlayerFormation = playerFormation;
        EnemyFormation = enemyFormation;
    }

    private void UpdateField(List<GameObject> cellList,float sine,float xMulti)
    {
        float stepFromLeft = (Screen.width * xMulti);
        float stepFromTop = Screen.height * 0.9f;
        var ScreenPos = Camera.main.ScreenToWorldPoint(new Vector3(stepFromLeft, stepFromTop, 8) );
        
        foreach (GameObject cell in cellList)
        {
            var cellscript = cell.GetComponent<ArmyCellScript>();
            cell.transform.position = new Vector3(ScreenPos.x + sine * cellscript.Company.Position * armyCellSpacing, ScreenPos.y - cellscript.Company.Position * armyCellSpacing, transform.position.z);
            if (cellscript.Company.Unit == null)
            {
                cellscript.ChangeSprite(UnitSprites.GetIconSpriteByName("available"));
                cellscript.ClearAttack();
                cellscript.DisableCellText();
            }
            else
            {
                cellscript.ChangeSprite(UnitSprites.GetIconSpriteByName(cellscript.Company.Unit.GetComponent<ArmyUnitClass>().UnitName));
                cellscript.UpdateCellText();
            }
    }
    }
    private void InitializeField(FormationField formation,GameObject parent, List<GameObject> cellList)
    {
        cellList.Clear();
        foreach (var comp in formation.Formation)
        {
            GameObject cell = Instantiate(ArmyFieldCell,parent.transform);
            cell.GetComponent<ArmyCellScript>().InitializeCell(comp);
            cell.name = $"{formation.FieldOwner.heroName}_{comp.Type.ToString()}_{comp.Position}";
            cellList.Add(cell);
        }
    }

    public void DestroyField()
    {
        var tempList = new List<GameObject>();
        tempList.AddRange(playerFieldList);
        tempList.AddRange(enemyFieldList);
        for (int i = 0; i < tempList.Count(); i++)
        {
            Destroy(tempList[i]);
        }
        playerFieldList.Clear();
        enemyFieldList.Clear();
    }
}
