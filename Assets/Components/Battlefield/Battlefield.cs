using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
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
    public OtherGraphic IconSprites;
    public BattlefieldLogic logic;
    public FormationManager Formation;
    public GameLoopPreBattleState PreBattleState;
    public GameObject PlayerFieldParent;
    public GameObject EnemyFieldParent;
    public GameObject HighlitedUnit;
    public GridManager Grid;
    [Header("FieldVars")]
    public float companySpacing = 0.2f;
    public float companyHeight = 2f;
    public float fieldZPos = -3f;
    public bool generateEmptyFields = false;

    // GameLoopPreBattleState ==> RebuildField() -> UpdateField()
    private void Awake()
    {
        Grid = transform.GetComponent<GridManager>();
    }
    public void Initialize(Camera camera,FormationManager formation,GameLoopPreBattleState preBattleState)
    {
        MainCamera = camera;
        logic = transform.GetComponent<BattlefieldLogic>();
        logic.Battlefield = this;
        Formation = formation;
        Formation.FormationGrid = Grid;
        PreBattleState = preBattleState;
        PlayerFieldParent.transform.position = new Vector3(0, -1.5f, fieldZPos);
        EnemyFieldParent.transform.position = new Vector3(0, -1.5f, fieldZPos);
    }
    public void RebuildField()
    {
        Formation.ClearField();
        InitializeField(Formation.PlayerHero, playerFieldList,PlayerFieldParent);
        InitializeField(Formation.EnemyHero, enemyFieldList,EnemyFieldParent);
        UpdateField();
    }
    private void InitializeField(Hero owner, List<GameObject> cellList,GameObject parent)
    {
        cellList.Clear();
        var companies = Formation.GetDeployedCompanies(owner);
        foreach (var comp in companies)
        {
            GameObject cell = Instantiate(OnFieldCompanyPrefab,parent.transform);
            //Mirror abilities interface position
            float AbilitiesPosMod = -75;
            if (parent==EnemyFieldParent) AbilitiesPosMod = 125;
            cell.GetComponent<OnFieldCompanyManager>().InitializeCell(comp,this,AbilitiesPosMod);
            cell.name = $"{owner.heroName}_{comp.Type.ToString()}_{comp.Position.ToString()}";
            cellList.Add(cell);
        }
    }

    public void GenerateEmptyCells()
    {
        Formation.CreateEmptyCompanies();
        AddNewEmptyCells(Formation.PlayerHero, playerFieldList, PlayerFieldParent);
        AddNewEmptyCells(Formation.EnemyHero, enemyFieldList, EnemyFieldParent);
    }
    public void RemoveEmptyCells()
    {
        var cellsToRemove = new List<GameObject>();
        cellsToRemove.AddRange(playerFieldList.Where(x=>x.GetComponent<OnFieldCompanyManager>().Company.Unit==null));
        cellsToRemove.AddRange(enemyFieldList.Where(x=>x.GetComponent<OnFieldCompanyManager>().Company.Unit==null));
        foreach (var cell in cellsToRemove)
        {
            playerFieldList.Remove(cell);
            enemyFieldList.Remove(cell);
            Destroy(cell);
        }
        Formation.RemoveEmptyCompanies();
    }
    public void UpdateField()
    {
        if (Formation.GetDeployedCompanies().Count == 0)
        {
            Formation.ClearField();
        }
        RemoveEmptyCells();
        if (generateEmptyFields)
        {
            GenerateEmptyCells();
        }
        UpdateField(playerFieldList,-1);
        UpdateField(enemyFieldList,1);
    }
    private void UpdateField(List<GameObject> cellList,float sine)
    {
        foreach (GameObject comp in cellList)
        {
            var compMan = comp.GetComponent<OnFieldCompanyManager>();
            var tempPos = compMan.Company.Position;
            comp.transform.localPosition = new Vector3(tempPos.x*(companySpacing+companyHeight),tempPos.y*companyHeight,0);
            //TODO temp fix to hide empty enemy comp
            if (compMan.Company.Unit == null && compMan.Company.unitOwner == Formation.EnemyHero)
            {
                comp.transform.localPosition = new Vector3(999,999,0);
            }
            UpdateCompanySprite(compMan);
        }
    }
    
    
    private void AddNewEmptyCells(Hero owner, List<GameObject> cellList,GameObject parent)
    {
        var companies = Formation.GetDeployedCompanies(owner,true);
        foreach (var comp in companies)
        {
            if (comp.Unit != null) continue;
            GameObject cell = Instantiate(OnFieldCompanyPrefab,parent.transform);
            //Mirror abilities interface position
            float AbilitiesPosMod = -75;
            if (parent==EnemyFieldParent) AbilitiesPosMod = 125;
            cell.GetComponent<OnFieldCompanyManager>().InitializeCell(comp,this,AbilitiesPosMod);
            cell.name = $"{owner.heroName}_{comp.Type.ToString()}_{comp.Position.ToString()}";
            cellList.Add(cell);
            //Debug.Log($"Adding cell {comp.occupiedPositions[0].ToString()}");
        }
    }

    private void UpdateCompanySprite(OnFieldCompanyManager comp)
    {
        if (comp.Company.Unit == null)
        {
            comp.ChangeSprite(null);
            comp.DisableCellText();
        }
        else
        {
            comp.ChangeSprite(UnitSprites.GetCardSpriteByName(comp.Company.Unit.GetComponent<ArmyUnitClass>().squadName));
            comp.UpdateCellText();
        }
    }
    
    public bool AddUnitToFormationLogic(GameObject newUnit, Company comp, OnFieldCompanyManager compMan,GameObject unitOwner)
    {
        if (AddUnitToFormation(newUnit, comp, compMan))
        {
            logic.Order();
            //If its player unit call unit deploy logic
            if(comp.unitOwner==Formation.PlayerHero) PreBattleState.OnPlayerUnitDeployed();
            return true;
        }
        else {Debug.LogWarning($"Unsuccessful AddUnitToFormation Params:{newUnit} {comp} {compMan}");
            return false;
        }
    }
    public bool AddUnitToFormation(GameObject newUnit, Company comp, OnFieldCompanyManager compMan)
    {
        bool activateAbilityButtons = false;
        if (comp.unitOwner == Formation.PlayerHero) {activateAbilityButtons=true;}
        bool answer = false;
        if (Formation.AddUnitToFormation(comp, newUnit))
        {
            answer=true;
        }
        else return answer;
        compMan.ResetAbilityButtons();
        int index = 0;
        foreach (var ability in comp.Unit.GetComponent<ArmyUnitClass>().unit.CurrentUnitAttributes.SquadAbilities)
        {
            compMan.CreateAbilityButton(index, logic, IconSprites.GetSpriteByName(ability.AbilityName),activateAbilityButtons);
            index++;
        }
        return answer;
    }

    public void RemoveUnitFromFormationLogic(Company comp, OnFieldCompanyManager compMan)
    {
        if (comp.Unit != null)
        {
            RemoveUnitFromFormation(comp, compMan);
        }
        else
        {
            foreach (var field in playerFieldList)
            {
                var CompMan = field.GetComponent<OnFieldCompanyManager>();
                if (CompMan.Company.Unit != null)
                {
                    RemoveUnitFromFormation(CompMan.Company, CompMan);
                }
            }
        }
        UpdateField();
        logic.Order();
    }

    public void RemoveUnitFromFormation(Company comp,OnFieldCompanyManager compMan)
    {
        Formation.RemoveUnitFromField(comp.Unit);
        compMan.ResetAbilityButtons();
        compMan.DeHighlightField();
    }

    public void OnBattleEnd()
    {
        DestroyField();
        logic.BattlefieldOrder.Clear();
        Formation.ClearField();
        logic.DestroyUI();
    }

    public void HighlightUnitUI(GameObject unit)
    {
        if (HighlitedUnit != null) DeHighlightUnitUI();
        
        UnitInOrder HighlitedUnitInOrder;
        var searchForUnit = logic.BattlefieldOrder.Where(x => x.UnitCompany.Unit == unit);
        if (searchForUnit.Count() == 0) return;
        else HighlitedUnitInOrder = searchForUnit.First();
        HighlitedUnit = unit;
        //Highlight objects
        HighlitedUnitInOrder.FieldManager.GetComponent<OnFieldCompanyManager>().HighlightField();
        logic.AbilitiesUI[HighlitedUnitInOrder.OrderIndex].GetComponent<AbilityOrderUI>().HighlightElement();
        if (HighlitedUnitInOrder.FieldManager.GetComponentInChildren<TargetPointerManager>()) HighlitedUnitInOrder.FieldManager.GetComponentInChildren<TargetPointerManager>().HighlightElement();
    }

    public void DeHighlightUnitUI()
    {
        if (HighlitedUnit == null) return;
        
        UnitInOrder HighlitedUnitInOrder;
        var searchForUnit = logic.BattlefieldOrder.Where(x => x.UnitCompany.Unit == HighlitedUnit);
        if (searchForUnit.Count() == 0) return;
        else HighlitedUnitInOrder = searchForUnit.First();
        //DeHighlight objects
        HighlitedUnitInOrder.FieldManager.GetComponent<OnFieldCompanyManager>().DeHighlightField();
        logic.AbilitiesUI[HighlitedUnitInOrder.OrderIndex].GetComponent<AbilityOrderUI>().DeHighlightElement();
        if (HighlitedUnitInOrder.FieldManager.GetComponentInChildren<TargetPointerManager>()) HighlitedUnitInOrder.FieldManager.GetComponentInChildren<TargetPointerManager>().DeHighlightElement();
        HighlitedUnit = null;
    }

    private void DestroyField()
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

    [CanBeNull]
    public GameObject GetFieldByCompany(Company compToFind)
    {
        List<GameObject> allFiedlsList = new List<GameObject>();
        allFiedlsList.AddRange(playerFieldList);
        allFiedlsList.AddRange(enemyFieldList);
        var fields = allFiedlsList.Where(field => field.GetComponent<OnFieldCompanyManager>().Company == compToFind);
        GameObject field=null;
        if (fields.Count()>0) field=fields.First();
        return field;
    }
}
