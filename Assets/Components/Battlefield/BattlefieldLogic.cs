using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions.Must;
[System.Serializable]
public class UnitInOrder
{
    public GameObject FieldManager;
    public Company UnitCompany;
    public UnitAbility UnitAbility;
    public int UnitInitiative;
    public int OrderIndex;
    [SerializeField] private string AbilityName; 

    public UnitInOrder(GameObject fieldManager,Company unitCompany)
    {
        FieldManager = fieldManager;
        UnitCompany = unitCompany;
        CalculateUnitInitiative();
    }

    public void AssignUnitAbility(UnitAbility unitAbility=null)
    {
        UnitAbility = unitAbility;
        CalculateUnitInitiative();
        if (unitAbility!=null) AbilityName = UnitAbility.AbilityName;
        else AbilityName = "";
    }

    private void CalculateUnitInitiative()
    {
        //TODO Remove init??
        UnitInitiative = 0;
    }
}

public class BattlefieldLogic : MonoBehaviour
{
    public Battlefield Battlefield;
    public GameObject AbilityUI;
    public GameObject AbilityAnimation;
    public GameObject AbilityUIParent;
    public List<GameObject> AbilitiesUI = new List<GameObject>();
    public GameObject DamageIndicator;
    public float pauseBetweenAbilities = 1f;
    public List<UnitInOrder> BattlefieldOrder = new List<UnitInOrder>();
    public List<GameObject> TargetPointers = new List<GameObject>();
    public GameObject TargetPointerPrefab;
    public bool isAction=false;
    public bool ManualAnimations=false;
    IEnumerator PlayAnimations(GameLoopRoundState parent)
    {
        float YY = -50;
        int index = 0;
        AbilityAnimation.SetActive(true);
        AbilityAnimation.transform.localPosition = new Vector3(-150, 0, 0);
        Battlefield.GetComponent<Battlefield>().DeHighlightUnitUI();
        foreach (var unit in BattlefieldOrder)
        {
            float timeSaved = Time.realtimeSinceStartup;
            yield return new WaitUntil(
                delegate
                {
                    return ManualAnimations ? isAction : Time.realtimeSinceStartup >= timeSaved + pauseBetweenAbilities || (isAction);
                }
            );
            Battlefield.GetComponent<Battlefield>().HighlightUnitUI(unit.UnitCompany.Unit);
            //Arrow anim
            float YYY = YY * index;
            AbilityAnimation.transform.localPosition = new Vector3(-150,YYY,0);
            index++;
            
            if (unit.UnitAbility == null || unit.UnitCompany.Unit.GetComponent<ArmyUnitClass>().currentSquadHealth <= 0)
            {
                string dmgText = "skip";
                Color dmgColor = new Color(0.8f, 0.7f, 0.3f, 0.66f);
                CreateDamageText(dmgText,dmgColor,unit.UnitCompany);
            }
            else ApplyAbility(unit.UnitAbility,unit.OrderIndex);

            Battlefield.UpdateField();
            isAction=false;
        }

        parent.ButtonEndRound();
        AbilityAnimation.SetActive(false);
    }
    public void ApplyAbilities(GameLoopRoundState parent)
    {
        StartCoroutine(PlayAnimations(parent));
    }
    private bool ApplyAbility(UnitAbility ability,int orderIndex)
    {
        if (ability.UnitCompany.Unit==null)
        {Debug.LogWarning($"ability.UnitCompany.Unit=null {ability.UnitCompany}");return false;}
        var cycleAbility = ability;
        // check target again -> calculate damage -> calculate retaliate damage -> apply -> check for defeted companies
        // check target again 
        /*if (!cycleAbility.SelectTargets())
        {
            var newPossibleAbility = cycleAbility.UnitCompany.Unit.GetComponent<ArmyUnitClass>().GetPossibleAbility();
            if (newPossibleAbility != null) cycleAbility = newPossibleAbility;
            if (!cycleAbility.SelectTargets())
            {
                Debug.Log($"{ability.UnitCompany.Unit.name} does not have targets");
                string dmgText = "skip";
                Color dmgColor = new Color(0.8f, 0.7f, 0.3f, 1);
                CreateDamageText(dmgText,dmgColor,ability.UnitCompany);
                return false;
            }
        }*/
        if (cycleAbility.targets.Count == 0 || cycleAbility.targets[0]?.Unit.GetComponent<ArmyUnitClass>().currentSquadHealth <= 0 )
        {
            Debug.Log($"{ability.UnitCompany.Unit.name} does not have targets");
            string dmgText = "skip";
            Color dmgColor = new Color(0.8f, 0.7f, 0.3f, 1);
            CreateDamageText(dmgText,dmgColor,ability.UnitCompany);
            return false;
        }
        List<(GameObject, int, int, int,Company)> results = new List<(GameObject, int, int, int,Company)>();
        // calculate damage
        var abilityOwnerResult = cycleAbility.GetAbilityImpact();
        var abilityOpposingResult = (-1, -1, -1);
        
        // calculate retaliate damage
        var opposingCompany = cycleAbility.targets[0];
        var opposingCompanyAbility = opposingCompany.Unit.GetComponent<ArmyUnitClass>()
            .GetRetaliationAbility(cycleAbility.RetaliationTags);
        if (opposingCompanyAbility != null)
        {
            opposingCompanyAbility.AssignTargetForRetaliation(cycleAbility.UnitCompany);
            abilityOpposingResult = opposingCompanyAbility.GetAbilityImpact();
        }
        // create list of combat results
        results.Add((cycleAbility.UnitCompany.Unit,abilityOpposingResult.Item1,abilityOpposingResult.Item2,abilityOwnerResult.Item3,cycleAbility.UnitCompany));
        results.Add((opposingCompany.Unit,abilityOwnerResult.Item1,abilityOwnerResult.Item2,abilityOpposingResult.Item3,opposingCompany));
        //CreateDamageText
        var newTextFrom = GenerateDamageText(results[0].Item2, results[0].Item3, results[0].Item5);
        if (results[0].Item2>=0) CreateDamageText(newTextFrom.Item1, newTextFrom.Item2, results[0].Item5);
        var newTextTo = GenerateDamageText(results[1].Item2, results[1].Item3, results[1].Item5);
        if (results[1].Item2>=0) CreateDamageText(newTextTo.Item1, newTextTo.Item2, results[1].Item5);
        AbilitiesUI[orderIndex].GetComponent<AbilityOrderUI>().UpdateRoundResults(results[0].Item2>=0 ? newTextFrom.Item1 : "",results[1].Item2>=0 ? newTextTo.Item1 : "");
        // apply -> check for defeted companies
        ApplyResults(results);
        return true;
    }

    public void ApplyResults(List<(GameObject, int, int, int,Company)> results)
    {
        foreach (var res in results)
        {
            if (res.Item2 != -1)
            {
                res.Item1.GetComponent<ArmyUnitClass>().TakeDamage((res.Item2, res.Item3));
            }
        }
    }
    public void RemoveDeadUnits()
    {
        List<Company> deadCompanies = new List<Company>();
        foreach (var unit in BattlefieldOrder)
        {
            if (unit.UnitCompany.Unit.GetComponent<ArmyUnitClass>().currentSquadHealth <= 0) deadCompanies.Add(unit.UnitCompany);
        }

        foreach (var comp in deadCompanies)
        {
            Battlefield.RemoveUnitFromFormationLogic(comp,Battlefield.GetFieldByCompany(comp).GetComponent<OnFieldCompanyManager>());
        }
    }

    private (string,Color) GenerateDamageText(int newSquadHealth, int newSquadNumber, Company comp)
    {
        Color dmgColor;
        string dmgText =
            $"{newSquadHealth - comp.Unit.GetComponent<ArmyUnitClass>().currentSquadHealth}";
        if (comp.unitOwner==Battlefield.Formation.PlayerHero) dmgColor = new Color(0.7f, 0, 1, 1);
        else dmgColor = new Color(0.55f, 0, 0, 1);
        int deadCount = comp.Unit.GetComponent<ArmyUnitClass>().unit.CurrentUnitAttributes.SquadSize -
                        newSquadNumber;
        if (deadCount > 0) dmgText += $"<sprite=\"gameSprites\" index=0 color=#000000>{deadCount}";
        return (dmgText,dmgColor);
    }

    private void CreateDamageText(string text, Color color,Company comp,float correction=1)
    {
        var allcells = new List<GameObject>();
        allcells.AddRange(Battlefield.playerFieldList);
        allcells.AddRange(Battlefield.enemyFieldList);
        var targetCell = allcells.Where(cell => cell.GetComponent<OnFieldCompanyManager>().Company == comp)
            .First();
        Vector3 newPos = new Vector3(targetCell.transform.position.x*correction, targetCell.transform.position.y*correction, targetCell.transform.position.z);
        DamageIndicator indicator =
            Instantiate(DamageIndicator, newPos, Quaternion.identity)
                .GetComponent<DamageIndicator>();
        indicator.transform.SetParent(targetCell.transform);
        indicator.InitializeIndicator(color,text);
    }

    public void Order()
    {
        DestroyUI();
        List<Company> onFieldUnits = new List<Company>();
        //Get all active units
        var onFieldPlayerCompanies = Battlefield.Formation.GetDeployedCompanies(Battlefield.Formation.PlayerHero);
        var onFieldEnemyCompanies = Battlefield.Formation.GetDeployedCompanies(Battlefield.Formation.EnemyHero);
        onFieldUnits.AddRange(onFieldPlayerCompanies);
        onFieldUnits.AddRange(onFieldEnemyCompanies);
        //Check if any units was removed from the field that are still in the BattlefieldOrder
        var removedUnits = new List<UnitInOrder>();
        removedUnits.AddRange(BattlefieldOrder.Where(x => x.UnitCompany.Unit == null));
        foreach (var unit in removedUnits) BattlefieldOrder.Remove(unit);

        //Add new units to the BattlefieldOrder
        foreach (var comp in onFieldUnits)
        {
            if (!BattlefieldOrder.Any(i => i.UnitCompany.Equals(comp)))
            {
                var newUnitInOrder = new UnitInOrder(Battlefield.GetFieldByCompany(comp),comp);
                BattlefieldOrder.Add(newUnitInOrder);
                var ab = comp.Unit.GetComponent<ArmyUnitClass>().GetPossibleAbility();
                newUnitInOrder.AssignUnitAbility(ab);
                if (ab!=null) newUnitInOrder.FieldManager.GetComponent<OnFieldCompanyManager>().SelectAbility(comp.Unit.GetComponent<ArmyUnitClass>().unit.CurrentUnitAttributes.SquadAbilities.FindIndex(abl=>abl==ab));
            }
        }
        //Check if targets of previous abilities was removed and assign new ability
        foreach (var unit in BattlefieldOrder.Where(x => x.UnitAbility != null))
        {
            if (unit.UnitAbility.targets[0].Unit == null)
            {
                var ab = unit.UnitCompany.Unit.GetComponent<ArmyUnitClass>().GetPossibleAbility();
                unit.AssignUnitAbility(ab);
                Battlefield.GetFieldByCompany(unit.UnitCompany).GetComponent<OnFieldCompanyManager>().SelectAbility(-1);
                if (ab!=null) Battlefield.GetFieldByCompany(unit.UnitCompany).GetComponent<OnFieldCompanyManager>().SelectAbility(unit.UnitCompany.Unit.GetComponent<ArmyUnitClass>().unit.CurrentUnitAttributes.SquadAbilities.FindIndex(abl=>abl==ab));
            }
        }
        //Check if unit had no ability before and try add new one
        foreach (var unit in BattlefieldOrder.Where(x => x.UnitAbility == null))
        {
            var ab = unit.UnitCompany.Unit.GetComponent<ArmyUnitClass>().GetPossibleAbility();
            unit.AssignUnitAbility(ab);
            if (ab!=null) Battlefield.GetFieldByCompany(unit.UnitCompany).GetComponent<OnFieldCompanyManager>().SelectAbility(unit.UnitCompany.Unit.GetComponent<ArmyUnitClass>().unit.CurrentUnitAttributes.SquadAbilities.FindIndex(abl=>abl==ab));
        }
        //Enemy units only logic
        foreach (var unit in BattlefieldOrder.Where(x => x.UnitCompany.unitOwner ==Battlefield.Formation.EnemyHero))
        {
            var ab = unit.UnitCompany.Unit.GetComponent<ArmyUnitClass>().GetPossibleAbility();
            unit.AssignUnitAbility(ab);
            if (ab!=null) unit.FieldManager.GetComponent<OnFieldCompanyManager>().SelectAbility(unit.UnitCompany.Unit.GetComponent<ArmyUnitClass>().unit.CurrentUnitAttributes.SquadAbilities.FindIndex(abl=>abl==ab));
        }
        BattlefieldOrder.Sort((a, b) => b.UnitInitiative.CompareTo(a.UnitInitiative));
        int orderIndex = 0;
        foreach (var unit in BattlefieldOrder)
        {
            unit.OrderIndex = orderIndex;
            orderIndex += 1;
        }

        //Debug.Log(answer);
        // -------------GRAPHIC
        foreach (var unit in BattlefieldOrder)
        {
            var newUI = Instantiate(AbilityUI, AbilityUIParent.transform);
            AbilitiesUI.Add(newUI);
            //Make background of AbilityOrderUI based on unit owner
            Color heroColor = new Color(0, 0.125f, 0.55f, 0.78f);
            if (unit.UnitCompany.unitOwner == Battlefield.Formation.PlayerHero) heroColor=new Color(0.5f, 0, 0.1f,0.78f);
            newUI.GetComponent<AbilityOrderUI>().InitializeUI(unit,heroColor,unit.OrderIndex+1);
        }
        //Create list of units with only active Abilities
        List<UnitInOrder> activeUnitList = new List<UnitInOrder>();
        activeUnitList.AddRange(BattlefieldOrder.Where(x => x.UnitAbility != null));
        if (activeUnitList.Count > 0)
        {
            //Rank all units by their TARGET X coord
            var rankedList = activeUnitList
                .OrderBy(x => Battlefield.GetFieldByCompany(x.UnitAbility.targets[0]).transform.position.x).Select(
                    (x, i) => new
                    {
                        Item = x, Rank = Battlefield.GetFieldByCompany(x.UnitAbility.targets[0]).transform.position.x
                    });
            //For each distinct field target for each unit create target pointer
            var distinctRankes = rankedList.Select(u => u.Rank).Distinct().ToList();
            foreach (var rank in distinctRankes)
            {
                var listXX = rankedList.Where(x => x.Rank == rank)
                    .OrderBy(x => x.Item.FieldManager.transform.position.x).ToList();
                for (int i = 0; i < listXX.Count; i++)
                {
                    float shooterAdjustment = -1;
                    float targetAdjustment = 1;
                    Color startColor = new Color(0.1f, 0.5f, 0.85f,0.8f);
                    Color endColor = new Color(0,0.125f, 0.55f,1f);
                    if (listXX[i].Item.UnitCompany.unitOwner == Battlefield.Formation.PlayerHero)
                    {
                        shooterAdjustment = 1;
                        targetAdjustment = -1;
                        startColor = new Color(0.8f, 0, 0,0.8f);
                        endColor = new Color(0.557f, 0, 0,1f);
                    }

                    Vector3 initShooterPos = listXX[i].Item.FieldManager.transform.position;
                    Vector3 initTargetPos = Battlefield.GetFieldByCompany(listXX[i].Item.UnitAbility.targets[0])
                        .GetComponent<OnFieldCompanyManager>().transform.position;
                    float xAdjustment = -1.05f + 2.1f / listXX.Count * (i+0.5f);
                    Vector3 shooterPos = new Vector3(initShooterPos.x + xAdjustment, initShooterPos.y + shooterAdjustment * 0.75f,
                        initShooterPos.z);
                    Vector3 targetPos = new Vector3(initTargetPos.x + xAdjustment,
                        initTargetPos.y + targetAdjustment * 0.8f, initTargetPos.z);

                    var newTargetPointer = Instantiate(TargetPointerPrefab, listXX[i].Item.FieldManager.transform);
                    newTargetPointer.GetComponent<TargetPointerManager>().SetPositinsAndColors(shooterPos, targetPos,startColor, endColor);
                    TargetPointers.Add(newTargetPointer);
                }
            }
        }
        Battlefield.UpdateField();
    }
    public void SelectAbility(int index,GameObject onFieldManager)
    {
        if (onFieldManager.GetComponent<OnFieldCompanyManager>().Company.Unit.GetComponent<ArmyUnitClass>()
            .unit.CurrentUnitAttributes.SquadAbilities[index]
            .SelectTargets())
        {
            UnitInOrder unitInOrder = BattlefieldOrder.Where(x => x.UnitCompany == onFieldManager.GetComponent<OnFieldCompanyManager>().Company).First();
            unitInOrder.AssignUnitAbility(onFieldManager.GetComponent<OnFieldCompanyManager>().Company.Unit
                .GetComponent<ArmyUnitClass>()
                .unit.CurrentUnitAttributes.SquadAbilities[index]);
            onFieldManager.GetComponent<OnFieldCompanyManager>().SelectAbility(index);
            Order();
        }
        else Debug.LogWarning("No target found for this ability");
    }

    public void DestroyUI()
    {
        foreach (var ui in AbilitiesUI)
        {
            Destroy(ui);
        }
        AbilityAnimation.transform.localPosition = new Vector3(0, 0, 0);
        AbilitiesUI.Clear();
        foreach (var targetPointer in TargetPointers)
        {
            Destroy(targetPointer);
        }
        TargetPointers.Clear();
    }
}
