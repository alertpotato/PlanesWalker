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
    

    [Header("Components")]
    public Camera MainCamera;
    public GameObject OnFieldCompanyPrefab;
    public UnitGraphic UnitSprites;
    public BattlefieldLogic logic;
    public FormationField PlayerFormation;
    public FormationField EnemyFormation;
    public GameObject PlayerFieldParent;
    public GameObject EnemyFieldParent;
    [Header("FieldVars")]
    public float companySpacing = 0.2f;
    public float companyHeight = 2f;
    public float fieldZPos = -1f;

    public void Initialize(Camera camera,FormationField playerFormation,FormationField enemyFormation)
    {
        MainCamera = camera;
        logic = transform.GetComponent<BattlefieldLogic>();
        logic.Battlefield = this;
        PlayerFormation = playerFormation;
        EnemyFormation = enemyFormation;
    }
    public void UpdateField()
    {
        PlayerFieldParent.transform.position = new Vector3(-2, 0, fieldZPos);
        EnemyFieldParent.transform.position = new Vector3(2, 0, fieldZPos);
        UpdateField(playerFieldList,-1);
        UpdateField(enemyFieldList,1);
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
        InitializeField(playerFormation, playerFieldList,PlayerFieldParent);
        InitializeField(enemyFormation, enemyFieldList,EnemyFieldParent);
        PlayerFormation = playerFormation;
        EnemyFormation = enemyFormation;
    }

    private void UpdateField(List<GameObject> cellList,float sine)
    {
        //calc front width and middle index
        int frontCount = cellList
            .Where(x => x.GetComponent<OnFieldCompanyManager>().Company.Type == FormationType.Frontline)
            .ToList().Count;
        int middlePosition =(frontCount -1) / 2;
        
        foreach (GameObject comp in cellList)
        {
            var compMan = comp.GetComponent<OnFieldCompanyManager>();
            var truePos = GetTruePosition(compMan,frontCount, middlePosition);
            //x adjustment for flanks and reserve
            float stepX = 0;
            if (compMan.Company.Type == FormationType.Flank1 || compMan.Company.Type == FormationType.Flank2)
                stepX = companyHeight / 2;
            if (compMan.Company.Type == FormationType.Support) stepX = companyHeight *1.5f;
            else if (compMan.Company.Type == FormationType.Reserve) stepX = companyHeight *3.5f;
            comp.transform.localPosition = new Vector3(stepX*sine,-truePos * (companySpacing+companyHeight),0);
            //UpdateCompanySprite(compMan);
        }
    }

    private int GetTruePosition(OnFieldCompanyManager comp,int frontCount, int middlePos)
    {
        //Func to make middle of Frontline index = 0, and adjust flanks
        int truePos = comp.Company.Position - middlePos;
        if (comp.Company.Type == FormationType.Flank1) truePos = truePos - middlePos;
        else if (comp.Company.Type == FormationType.Flank2) truePos = truePos + frontCount;
        return truePos;
    }

    private void UpdateCompanySprite(OnFieldCompanyManager comp)
    {
        if (comp.Company.Unit == null)
        {
            comp.ChangeSprite(UnitSprites.GetIconSpriteByName("available"));
            comp.DisableCellText();
        }
        else
        {
            comp.ChangeSprite(UnitSprites.GetIconSpriteByName(comp.Company.Unit.GetComponent<ArmyUnitClass>().UnitName));
            comp.UpdateCellText();
        }
    }

    private void InitializeField(FormationField formation, List<GameObject> cellList,GameObject parent)
    {
        cellList.Clear();
        foreach (var comp in formation.Formation)
        {
            GameObject cell = Instantiate(OnFieldCompanyPrefab,parent.transform);
            cell.GetComponent<OnFieldCompanyManager>().InitializeCell(comp);
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
