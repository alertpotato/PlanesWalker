using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CowardlyAttackAbility : UnitAbility
{
    public static Abilities UniqueType = Abilities.CowardlyAttack;
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
        // Check if we are on the fronline
        (int,int) unitPosition = UnitCompany.Banner;
        if (unitPosition.Item1 != 0) return false;
        // Get 3 units in front of us
        List<Company> unitsInfront = new List<Company>()
        {
            OpposingField.GetCompany(unitPosition),
            OpposingField.GetCompany((unitPosition.Item1, unitPosition.Item2 - 1)),
            OpposingField.GetCompany((unitPosition.Item1, unitPosition.Item2 + 1))
        };

        Predicate<Company> isOccupied = comp => comp.Type != CompanyType.Occupied;
        unitsInfront.RemoveAll(isOccupied);

        if(unitsInfront.Count<=0) return false;
        var sortedUnits = from comp in unitsInfront
            orderby comp.Unit.GetComponent<ArmyUnitClass>().currentSquadHealth ascending 
            where comp.Type==CompanyType.Occupied
            select comp;
        if (sortedUnits.Count() > 0)
        {
            targets.Add(sortedUnits.First().Banner);
            return true;
        }
        return false;
    }
}
