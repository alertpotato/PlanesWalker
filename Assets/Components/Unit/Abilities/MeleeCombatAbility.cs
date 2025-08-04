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
        MoveSpeed = 1;
        RetaliationTags.Add(AbilityTags.MeleeRetaliation);
        AbilityDescription = $"Melee combat [melee][melee retaliation]\n  Damage: {Mathf.Round(AbilityDamageModifier*100)}%\n  Target priority:\nopposite unit first";
    }
    public override bool SelectTargets()
    {
        targets.Clear();

        List<Hero> EnemyHero = new List<Hero>();
        if (UnitCompany.unitOwner==formation.EnemyHero) EnemyHero.Add(formation.PlayerHero);
        else EnemyHero.Add(formation.EnemyHero);
        
        var onFieldTargetsList = formation.FormationGrid.GetNeighbors(UnitCompany, EnemyHero);
        if (onFieldTargetsList.Count > 0)
        {
            //TODO add multiple targets logic
            targets.Add(onFieldTargetsList[0].Position);
            Debug.Log($"AB:{AbilityName} U:{UnitCompany.Unit.name} ATTACKTARGET:{targets[0]}");
            return true;
        }

        var moveTarget = GetMoveTarget();
        if (moveTarget != UnitCompany.Position)
        {
            targets.Add(moveTarget);
            Debug.Log($"AB:{AbilityName} U:{UnitCompany.Unit.name} MOVETARGET:{targets[0]}");
            return true;
        }
        return false;
    }
    public override bool Edvance()
    {
        Debug.Log($"AB:{AbilityName} U:{UnitCompany.Unit.name} TRYMOVETO:{targets[0]}");
        if (Move()) return true;
        else return false;
    }
}
