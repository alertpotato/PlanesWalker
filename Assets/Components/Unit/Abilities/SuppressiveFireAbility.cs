using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

[System.Serializable]
public class SuppressiveFireAbility : UnitAbility
{
    public SuppressiveFireAbility()
    {
        AbilityName = "Suppressive Fire";
        Tags.Add(AbilityTags.Ranged);
        AbilityDamageModifier = 0.6f;
        InitiativeModifier = 1;
        RetaliationTags.Add(AbilityTags.RangedRetaliation);
        AbilityDescription = $"Suppressive fire [ranged]\n  Damage: {Mathf.Round(AbilityDamageModifier*100)}%\n  Hits any unit on the frontline or flanks.\n  Target priority:\nMelee->Mounted->Ranged\n  Additional modifiers:\n+{InitiativeModifier} initiative";
    }
    public override bool SelectTargets()
    {
        targets.Clear();
        var onFieldTargetsList = GetPossibleTargets();
        //making list of priorities - first will check for Melee units, then for Mounted, then others
        List<AbilityTags> cycleOrder = new List<AbilityTags>()
            { AbilityTags.Melee,AbilityTags.Mounted, AbilityTags.Ranged }; 
        bool answer = CycleOrder(onFieldTargetsList, cycleOrder);
        return answer;
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
