using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

[System.Serializable]
public class MountedChargeAbility : UnitAbility
{
    public static Abilities UniqueType = Abilities.MountedCharge;
    public MountedChargeAbility()
    {
        AbilityName = "Mounted Charge";
        Tags.Add(AbilityTags.Melee);
        Tags.Add(AbilityTags.Mounted);
        AbilityDamageModifier = 1.2f;
        RetaliationTags.Add(AbilityTags.MeleeRetaliation);
    }
    public override bool SelectTargets()
    {
        var answer = false;
        targets.Clear();
        Random rand = new Random();
        var onFieldTargetsList = OpposingField.GetOnFieldcompanies();
        //making list of priorities - first will check for mounted units, then for ranged, then others
        if (onFieldTargetsList.Count > 0)
        {
            int index = rand.Next(onFieldTargetsList.Count);
            targets.Add(onFieldTargetsList[index].Banner);
            return true;
        }
        return answer;
    }
}
