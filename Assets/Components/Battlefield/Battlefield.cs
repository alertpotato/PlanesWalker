using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public class Battlefield : MonoBehaviour
{
    [Header("Field Graphics")]
    public List<GameObject> playerFieldList = new List<GameObject>();
    public List<GameObject> enemyFieldList = new List<GameObject>();
    public float armyCellSpacing = 1.3f;

    [Header("Components")] 
    public Camera MainCamera;
    public GameObject ArmyFieldCell;
    public GameObject YourHeroField;
    public GameObject EnemyHeroField;
    public UnitGraphic UnitSprites;
    public BattlefieldLogic logic;
    public FormationField PlayerFormation;
    public FormationField EnemyFormation;
    
    private void Start()
    {
        transform.position = new Vector3(0, 0, 8.5f);
    }
    public void UpdateField()
    {
        UpdateField(playerFieldList,-1,0.55f);
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
        float stepFromTop = Screen.height * 0.8f;
        var ScreenPos = Camera.main.ScreenToWorldPoint(new Vector3(stepFromLeft, stepFromTop, 8) );
        
        foreach (GameObject cell in cellList)
        {
            var cellscript = cell.GetComponent<ArmyCellScript>();
            if (cellscript.Company.Type != CompanyType.NotAvailable)
            {
                cell.transform.position = new Vector3(ScreenPos.x + sine * cellscript.Company.Banner.Item1 * armyCellSpacing, ScreenPos.y - cellscript.Company.Banner.Item2 * armyCellSpacing, transform.position.z);
                //cellArmy.ChangeSprite(UnitSprites.GetIconSpriteByName("notavailable"));
                //cellArmy.ClearAttack();
                if (cellscript.Company.Type == CompanyType.Available)
                {
                    cellscript.ChangeSprite(UnitSprites.GetIconSpriteByName("available"));
                    cellscript.ClearAttack();
                    cellscript.DisableCellText();
                }
                else if (cellscript.Company.Type == CompanyType.Occupied)
                {
                    cellscript.ChangeSprite(UnitSprites.GetIconSpriteByName(cellscript.Company.Unit.GetComponent<ArmyUnitClass>().UnitName));
                    cellscript.UpdateCellText();
                }
            }
        }
    }
    private void InitializeField(FormationField formation,GameObject parent, List<GameObject> cellList)
    {
        cellList.Clear();
        for (int line = 0; line < formation.Formation.Count; line++)
        {
            for (int column = 0; column < formation.Formation[line].Line.Count; column++)
            {
                Company company = formation.GetCompany((line, column));
                //if (company.Type != CompanyType.NotAvailable) Debug.Log(parent.gameObject.name+" | "+company+" | "+(line, column));
                GameObject cell = Instantiate(ArmyFieldCell,parent.transform);
                cell.GetComponent<ArmyCellScript>().InitializeCell(company);
                cell.name = $"{formation.FieldOwner.heroName}_{line}_{column}";
                cellList.Add(cell);
                if (company.Type == CompanyType.NotAvailable)
                {
                    cell.SetActive(false);
                }
            }
        }
    }
}
