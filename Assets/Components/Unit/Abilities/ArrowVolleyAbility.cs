using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

[System.Serializable]
public class ArrowVolleyAbility : UnitAbility
{
    public ArrowVolleyAbility()
    {
        AbilityName = "Arrow Volley";
        Tags.Add(AbilityTags.Ranged);
        AbilityDamageModifier = 0.8f;
        RetaliationTags.Add(AbilityTags.RangedRetaliation);
        InitiativeModifier = 0;
        AbilityDescription = $"Arrow volley [ranged]\n  Damage: {Mathf.Round(AbilityDamageModifier*100)}%\n  Hits any unit on the frontline or flanks\n  Target priority:\nMounted->Ranged->Melee";
    }
    public override bool SelectTargets()
    {
        targets.Clear();
        var onFieldTargetsList = GetPossibleTargets();
        //making list of priorities - first will check for mounted units, then for ranged, then others
        List<AbilityTags> cycleOrder = new List<AbilityTags>()
            { AbilityTags.Mounted, AbilityTags.Ranged, AbilityTags.Melee }; 
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
                targets.Add(possibleUnits[index].Position);
                return true;
            }
        }
        return false;
    }
    public override bool Edvance()
    {
        if (Move()) return true;
        else return false;
    }
}
