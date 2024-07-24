using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

[System.Serializable]
public class MountedChargeAbility : UnitAbility
{
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
        targets.Clear();
        var onFieldTargetsList = GetPossibleTargets();
        if (onFieldTargetsList.Count > 0)
        {
            List<AbilityTags> cycleOrder = new List<AbilityTags>()
                { AbilityTags.Ranged };
            bool answer = CycleOrder(onFieldTargetsList, cycleOrder);
            if (answer) return true;
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
    private bool CycleOrder(List<Company> onFieldTargetsList,List<AbilityTags> cycleOrder)
    {
        Random rand = new Random();
        foreach (var tag in cycleOrder)
        {
            var possibleUnits = onFieldTargetsList
                .Where(tar => tar.Unit.GetComponent<ArmyUnitClass>().UnitAbilityTags.Contains(tag))
                .ToList();
            if (possibleUnits.Count > 0)
            {
                int index = rand.Next(possibleUnits.Count);
                targets.Add(possibleUnits[index]);
                return true;
            }
        }
        return false;
    }
}
