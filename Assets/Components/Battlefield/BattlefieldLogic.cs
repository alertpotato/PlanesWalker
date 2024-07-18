using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattlefieldLogic : MonoBehaviour
{
    public Battlefield Battlefield;
    public List<UnitAbility> AbilitiesOrder = new List<UnitAbility>();
    public RectTransform AbilityUIBack;
    public GameObject AbilityUI;
    public GameObject AbilityUIParent;
    public List<GameObject> AbilitiesUI;
    public Sprite AbilityIcon;
    public UnitGraphic UnitSprites;
    
    public void ApplyAbilities()
    {
        foreach (var ability in AbilitiesOrder)
        {
            var cycleAbility = ability;
            // check target again -> calculate damage -> calculate retaliate damage -> apply -> check for defeted companies
            // check target again 
            if (!cycleAbility.SelectTargets())
            {
                cycleAbility = cycleAbility.UnitCompany.Unit.GetComponent<ArmyUnitClass>().GetPossibleAbility();
                if (!cycleAbility.SelectTargets()) continue;
            }

            // calculate damage
            var abilityOwnerResult = cycleAbility.GetAbilityImpact();
            
            // calculate retaliate damage
            var opposingCompany = cycleAbility.OpposingField.GetCompany(cycleAbility.targets[0]);
            var opposingCompanyAbility = opposingCompany.Unit.GetComponent<ArmyUnitClass>()
                .GetRetaliationAbility(cycleAbility.RetaliationTags);
            if (opposingCompanyAbility != null)
            {
                opposingCompanyAbility.AssignTargetForRetaliation(cycleAbility.UnitCompany.Banner);
                var abilityOpposingResult = opposingCompanyAbility.GetAbilityImpact();
                // apply -> check for defeted companies
                if (!cycleAbility.UnitCompany.Unit.GetComponent<ArmyUnitClass>().TakeDamage(abilityOpposingResult))
                    cycleAbility.UnitCompany.Field.RemoveUnitFromField(cycleAbility.UnitCompany.Unit);
            }
            // apply -> check for defeted companies
            if (!opposingCompany.Unit.GetComponent<ArmyUnitClass>().TakeDamage(abilityOwnerResult))
                opposingCompany.Field.RemoveUnitFromField(opposingCompany.Unit);
        }
    }

    public void Order()
    {
        DestroyUI();
        AbilitiesUI = new List<GameObject>();
        AbilitiesOrder.Clear();
        List<Company> onFieldUnits = new List<Company>();

        var onFieldPlayerCompanies = Battlefield.PlayerFormation.GetOnFieldcompanies();
        var onFieldEnemyCompanies = Battlefield.EnemyFormation.GetOnFieldcompanies();

        onFieldUnits.AddRange(onFieldPlayerCompanies);
        onFieldUnits.AddRange(onFieldEnemyCompanies);
        var sortedUnits = from comp in onFieldUnits
            orderby comp.Unit.GetComponent<ArmyUnitClass>().CurrentUnitCharacteristics.Initiative descending
            select comp;
        string answer = "";
        foreach (var comp in sortedUnits)
        {
            answer = $"{answer} -> {comp.Unit.gameObject.name}[{comp.Unit.GetComponent<ArmyUnitClass>().CurrentUnitCharacteristics.Initiative}]";
            var ab = comp.Unit.GetComponent<ArmyUnitClass>().GetPossibleAbility();
            if (ab!=null) AbilitiesOrder.Add(ab);
        }
        //Debug.Log(answer);
        // -------------GRAPHIC
        float YY = -60;
        int index = 0;
        foreach (var ability in AbilitiesOrder)
        {
            //Debug.Log($"{ability.UnitSquad.Unit.name} {ability.IsActive.ToString()}");
            float YYY = YY * index;
            var newUI = Instantiate(AbilityUI,AbilityUIParent.transform);
            newUI.transform.localPosition = new Vector3(0,YYY,0);
            AbilitiesUI.Add(newUI);
            newUI.GetComponent<AbilityOrderUI>().SetIcons(
                AbilityIcon,
                UnitSprites.GetIconSpriteByName(ability.UnitCompany.Unit.GetComponent<ArmyUnitClass>().UnitName),
                UnitSprites.GetIconSpriteByName(ability.OpposingField.Formation[ability.targets[0].Item1].Line[ability.targets[0].Item2].Unit.GetComponent<ArmyUnitClass>().UnitName)
                );
            index++;
        }
        AbilityUIBack.sizeDelta = new Vector2(AbilityUIBack.rect.width,114*index);
        Battlefield.UpdateField();
    }
    private void DestroyUI()
    {
        foreach (var ui in AbilitiesUI)
        {
            Destroy(ui);
        }
    }
}
