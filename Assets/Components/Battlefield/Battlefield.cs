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
    public FormationField PlayerFormation;
    public FormationField EnemyFormation;
    public GameObject PlayerFieldParent;
    public GameObject EnemyFieldParent;
    public GameObject HighlitedUnit;
    [Header("FieldVars")]
    public float companySpacing = 0.2f;
    public float companyHeight = 2f;
    public float fieldZPos = -3.5f;

    public void Initialize(Camera camera,FormationField playerFormation,FormationField enemyFormation)
    {
        MainCamera = camera;
        logic = transform.GetComponent<BattlefieldLogic>();
        logic.Battlefield = this;
        PlayerFormation = playerFormation;
        EnemyFormation = enemyFormation;
    }
    public void RebuildField(FormationField playerFormation, FormationField enemyFormation)
    {
        InitializeField(playerFormation, playerFieldList,PlayerFieldParent);
        InitializeField(enemyFormation, enemyFieldList,EnemyFieldParent);
        PlayerFormation = playerFormation;
        EnemyFormation = enemyFormation;
    }
    private void InitializeField(FormationField formation, List<GameObject> cellList,GameObject parent)
    {
        cellList.Clear();
        foreach (var comp in formation.Formation)
        {
            GameObject cell = Instantiate(OnFieldCompanyPrefab,parent.transform);
            //Mirror abilities interface position
            float AbilitiesPosMod = -75;
            if (formation==EnemyFormation) AbilitiesPosMod = 125;
            cell.GetComponent<OnFieldCompanyManager>().InitializeCell(comp,this,AbilitiesPosMod);
            cell.name = $"{formation.FieldOwner.heroName}_{comp.Type.ToString()}_{comp.Position}";
            cellList.Add(cell);
        }
    }
    public void UpdateField()
    {
        PlayerFieldParent.transform.position = new Vector3(0, -1.5f, fieldZPos);
        EnemyFieldParent.transform.position = new Vector3(0, 1.5f, fieldZPos);
        int frontCountPlayer = playerFieldList
            .Where(x => x.GetComponent<OnFieldCompanyManager>().Company.Type == FormationType.Frontline)
            .ToList().Count;
        //Centering both fields
        int middlePosition = (frontCountPlayer -1) / 2;
        UpdateField(playerFieldList,middlePosition,-1);
        UpdateField(enemyFieldList,middlePosition,1);
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
    private void UpdateField(List<GameObject> cellList,int middlePos,float sine)
    {
        //calc front width and middle index
        int frontCount = cellList
            .Where(x => x.GetComponent<OnFieldCompanyManager>().Company.Type == FormationType.Frontline)
            .ToList().Count;

        foreach (GameObject comp in cellList)
        {
            var compMan = comp.GetComponent<OnFieldCompanyManager>();
            var truePos = GetTruePosition(compMan,frontCount, middlePos);
            //x adjustment for flanks and reserve
            float stepY = 0;
            if (compMan.Company.Type == FormationType.Flank1 || compMan.Company.Type == FormationType.Flank2)
                stepY = companyHeight / 2;
            if (compMan.Company.Type == FormationType.Support) stepY = companyHeight *1.5f;
            else if (compMan.Company.Type == FormationType.Reserve) stepY = companyHeight *3.5f;
            comp.transform.localPosition = new Vector3(-truePos * (companySpacing+companyHeight),stepY*sine,0);
            UpdateCompanySprite(compMan);
        }
    }

    private int GetTruePosition(OnFieldCompanyManager comp,int frontCount, int middlePos)
    {
        //Func to make middle of Frontline index = 0, and adjust flanks
        
        int truePos = comp.Company.Position - middlePos;
        if (comp.Company.Type == FormationType.Flank1) truePos = -middlePos-comp.Company.Position-1;
        else if (comp.Company.Type == FormationType.Flank2) truePos = truePos + frontCount;
        return truePos;
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
            comp.ChangeSprite(UnitSprites.GetCardSpriteByName(comp.Company.Unit.GetComponent<ArmyUnitClass>().UnitName));
            comp.UpdateCellText();
        }
    }
    
    public bool AddUnitToFormationLogic(GameObject newUnit, Company comp, OnFieldCompanyManager compMan,GameObject unitOwner)
    {
        if (unitOwner.GetComponent<Hero>()!=comp.Field.FieldOwner) return false;
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
        FormationField opposingFormation = PlayerFormation;
        bool activateAbilityButtons = false;
        if (comp.Field == PlayerFormation) {opposingFormation = EnemyFormation; activateAbilityButtons=true;}
        bool answer = false;
        if (comp.Field.AddUnitToFormation(comp, newUnit, opposingFormation))
        {
            answer=true;
        }
        else return answer;
        compMan.ResetAbilityButtons();
        int index = 0;
        foreach (var ability in comp.Unit.GetComponent<ArmyUnitClass>().Abilities)
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
        comp.Field.RemoveUnitFromField(comp.Unit);
        compMan.ResetAbilityButtons();
        compMan.DeHighlightField();
    }

    public void OnBattleEnd()
    {
        DestroyField();
        logic.BattlefieldOrder.Clear();
        PlayerFormation.ClearField();
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
