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
        RetaliationTags.Add(AbilityTags.MeleeRetaliation);
    }
    public override bool SelectTargets()
    {
        targets.Clear();
        var onFieldTargetsList = GetPossibleTargets();
        if (onFieldTargetsList.Count() > 0)
        {
            var sortedUnits = from comp in onFieldTargetsList
                orderby comp.Unit.GetComponent<ArmyUnitClass>().currentSquadHealth ascending
                select comp;
            targets.Add(sortedUnits.First());
            return true;
        }
        return false;
    }
}
