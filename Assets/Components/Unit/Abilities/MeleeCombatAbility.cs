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
        RetaliationTags.Add(AbilityTags.MeleeRetaliation);
    }
    public override bool SelectTargets()
    {
        var onFieldTargetsList = GetPossibleTargets();
        if (onFieldTargetsList.Count > 0)
        {
            if (onFieldTargetsList.Where(comp => comp.Type == UnitCompany.Type && comp.Position == UnitCompany.Position)
                    .Count() == 1)
            {
                targets.Add(onFieldTargetsList.Where(comp => comp.Type == UnitCompany.Type && comp.Position == UnitCompany.Position).First());
                return true;
            }
            else
            {
                Random rand = new Random();
                int index = rand.Next(onFieldTargetsList.Count);
                targets.Add(onFieldTargetsList[index]);
                return true;
            }
        }
        return false;
    }
}
