using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

[System.Serializable]
public class MeleeCombatAbility : UnitAbility
{
    public MeleeCombatAbility()
    {
        AbilityName = "Melee Combat";
        Tags.Add(AbilityTags.Melee);
        Tags.Add(AbilityTags.MeleeRetaliation);
        AbilityDamageModifier = 1;
        InitiativeModifier = 0;
        RetaliationTags.Add(AbilityTags.MeleeRetaliation);
        AbilityDescription = $"Melee combat [melee][melee retaliation]\n  Damage: {Mathf.Round(AbilityDamageModifier*100)}%\n  Target priority:\nopposite unit first";
    }
    public override bool SelectTargets()
    {
        targets.Clear();
        var onFieldTargetsList = GetPossibleTargets();
        if (onFieldTargetsList.Count > 0)
        {
            targets.Add(onFieldTargetsList[0]);
            return true;
        }
        else return false;
    }
}
