using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class CowardlyAttackAbility : UnitAbility
{
    public CowardlyAttackAbility()
    {
        AbilityName = "Cowardly Attack";
        Tags.Add(AbilityTags.Melee);
        AbilityDamageModifier = 1;
        InitiativeModifier = -1;
        RetaliationTags.Add(AbilityTags.MeleeRetaliation);
        AbilityDescription = $"Cowardly attack [melee]\n  Damage: {Mathf.Round(AbilityDamageModifier*100)}%\n  Target priority:\nunit with least total damage\n  Additional modifiers:\n{InitiativeModifier} initiative";
    }
    public override bool SelectTargets()
    {
        targets.Clear();
        var onFieldTargetsList = GetPossibleTargets();
        if (onFieldTargetsList.Count() > 0)
        {
            var sortedUnits = from comp in onFieldTargetsList
                orderby comp.Unit.GetComponent<ArmyUnitClass>().CurrentUnitCharacteristics.NumberOfUnits*comp.Unit.GetComponent<ArmyUnitClass>().CurrentUnitCharacteristics.Damage ascending
                select comp;
            targets.Add(sortedUnits.First());
            return true;
        }
        return false;
    }
}
