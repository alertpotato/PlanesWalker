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
    public GameObject PlayerFieldParent;
    public GameObject EnemyFieldParent;
    public GameObject HighlitedUnit;
    [Header("FieldVars")]
    public float companySpacing = 0.2f;
    public float companyHeight = 2f;
    public float fieldZPos = -3.5f;

    public void Initialize(Camera camera,FormationManager formation)
    {
        MainCamera = camera;
        logic = transform.GetComponent<BattlefieldLogic>();
        logic.Battlefield = this;
        Formation = formation;
    }
    public void RebuildField()
    {
        InitializeField(Formation.PlayerHero, playerFieldList,PlayerFieldParent);
        InitializeField(Formation.EnemyHero, enemyFieldList,EnemyFieldParent);
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
            cell.name = $"{owner.heroName}_{comp.Type.ToString()}_{comp.occupiedPositions[0].ToString()}";
            cellList.Add(cell);
        }
    }
    public void UpdateField()
    {
        PlayerFieldParent.transform.position = new Vector3(0, -1.5f, fieldZPos);
        EnemyFieldParent.transform.position = new Vector3(0, 1.5f, fieldZPos);
        UpdateField(playerFieldList,-1);
        UpdateField(enemyFieldList,1);
    }
    private void UpdateField(List<GameObject> cellList,float sine)
    {
        foreach (GameObject comp in cellList)
        {
            var compMan = comp.GetComponent<OnFieldCompanyManager>();
            var tempPos = compMan.Company.occupiedPositions[0];
            comp.transform.localPosition = new Vector3(tempPos.X*(companySpacing+companyHeight),tempPos.Y*companyHeight,0);
            UpdateCompanySprite(compMan);
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
        //if (unitOwner.GetComponent<Hero>()!=comp.Field.FieldOwner) return false;
        if (AddUnitToFormation(newUnit, comp, compMan))
        {
            logic.Order();
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
