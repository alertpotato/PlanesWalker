using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

[System.Serializable]
public class MeleeCombatAbility : UnitAbility
{
    public static Abilities UniqueType = Abilities.MeleeCombat;
    public MeleeCombatAbility()
    {
        AbilityName = "Melee Combat";
        Tags.Add(AbilityTags.Melee);
        Tags.Add(AbilityTags.MeleeRetaliation);
        AbilityDamageModifier = 1;
        RetaliationTags.Add(AbilityTags.MeleeRetaliation);
    }
    public override bool SelectTargets()
    {
        // Check if we are on the fronline
        (int,int) unitPosition = UnitCompany.Banner;
        if (unitPosition.Item1 != 0) return false;
        targets.Clear();
        // First try to find target infront
        var opposingUnit = OpposingField.GetCompany(unitPosition);
        if (opposingUnit?.Type == CompanyType.Occupied)
        {
            targets.Add(unitPosition);
            return true;
        }
        // If noones infront try to find neigbours
        List<Company> neigbours = new List<Company>();
        neigbours.Add(OpposingField.GetCompany((unitPosition.Item1,unitPosition.Item2-1)));
        neigbours.Add(OpposingField.GetCompany((unitPosition.Item1,unitPosition.Item2+1)));
        Random rand = new Random();
        int index = rand.Next(neigbours.Count);
        if (neigbours[index]?.Type == CompanyType.Occupied)
        {
            targets.Add(neigbours[index].Banner);
            return true;
        }
        return false;
    }
}
