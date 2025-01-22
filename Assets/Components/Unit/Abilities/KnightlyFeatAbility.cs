using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class KnightlyFeatAbility : UnitAbility
{
    public KnightlyFeatAbility()
    {
        AbilityName = "Knightly Feat";
        Tags.Add(AbilityTags.Melee);
        AbilityDamageModifier = 1.2f;
        InitiativeModifier = 0;
        RetaliationTags.Add(AbilityTags.MeleeRetaliation);
        AbilityDescription = $"Knightly feat [melee]\n  Damage: {Mathf.Round(AbilityDamageModifier*100)}%\n  Target priority:\nunit with most total health";
    }
    public override bool SelectTargets()
    {
        targets.Clear();
        var onFieldTargetsList = GetPossibleTargets();
        if (onFieldTargetsList.Count() > 0)
        {
            var sortedUnits = from comp in onFieldTargetsList
                orderby comp.Unit.GetComponent<ArmyUnitClass>().currentSquadHealth descending
                select comp;
            targets.Add(sortedUnits.First());
            return true;
        }
        return false;
    }
}
