using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattlefieldLogic : MonoBehaviour
{
    public Battlefield Battlefield;
    public List<UnitAbility> AbilitiesOrder;
    public RectTransform AbilityUIBack;
    public GameObject AbilityUI;
    public GameObject AbilityUIParent;
    public List<GameObject> AbilitiesUI;
    public Sprite AbilityIcon;
    public UnitGraphic UnitSprites;

    public void ApplyAbilities()
    {
        foreach (var ability in AbilitiesOrder)
        {
            ability.MainFunc(ability.OpposingHero, true);
        }
    }

    public void Order()
    {
        DestroyUI();
        AbilitiesUI = new List<GameObject>();
        AbilitiesOrder = new List<UnitAbility>();
        List<Squad> onFieldUnits = new List<Squad>();
        onFieldUnits.AddRange(InitAbilities(Battlefield.YourHero, Battlefield.EnemyHero));
        onFieldUnits.AddRange(InitAbilities(Battlefield.EnemyHero,Battlefield.YourHero));
        var sortedUnits = from squad in onFieldUnits
            orderby squad.Unit.GetComponent<ArmyUnitClass>().CurrentUnitCharacteristics.Initiative descending
            select squad;
        string answer = "";
        foreach (var squad in sortedUnits)
        {
            answer = $"{answer} -> {squad.Unit.gameObject.name}[{squad.Unit.GetComponent<ArmyUnitClass>().CurrentUnitCharacteristics.Initiative}]";
            AbilitiesOrder.Add(squad.Unit.GetComponent<ArmyUnitClass>().Abilities[0]);
        }
        Debug.Log(answer);
        // -------------GRAPHIC
        float YY = -60;
        int index = 0;
        foreach (var ability in AbilitiesOrder)
        {
            //Debug.Log($"{ability.UnitSquad.Unit.name} {ability.IsActive.ToString()}");
            if (!ability.IsActive) continue;
            float YYY = YY * index;
            var newUI = Instantiate(AbilityUI,AbilityUIParent.transform);
            newUI.transform.localPosition = new Vector3(0,YYY,0);
            AbilitiesUI.Add(newUI);
            newUI.GetComponent<AbilityOrderUI>().SetIcons(
                AbilityIcon,
                UnitSprites.GetIconSpriteByName(ability.UnitSquad.Unit.GetComponent<ArmyUnitClass>().UnitName),
                UnitSprites.GetIconSpriteByName(ability.OpposingHero.ArmyFormation[ability.targets[0][0]].ArmyLine[ability.targets[0][1]].Unit.GetComponent<ArmyUnitClass>().UnitName)
                );
            index++;
        }
        AbilityUIBack.sizeDelta = new Vector2(AbilityUIBack.rect.width,114*index);
    }
    private void DestroyUI()
    {
        foreach (var ui in AbilitiesUI)
        {
            Destroy(ui);
        }
    }
    public List<Squad> InitAbilities(Hero unitHero,Hero opposingHero)
    {
        List<Squad> onFieldUnits = new List<Squad>();
        for (int line = 0;  line < unitHero.ArmyFormation.Count;  line++)
        {
            for (int column = 0; column < unitHero.ArmyFormation[line].ArmyLine.Count; column++)
            {
                Squad unitSquad = unitHero.ArmyFormation[line].ArmyLine[column];
                if (unitSquad.Type == CellType.Occupied)
                {
                    //Debug.Log($"Added unit {line}_{column} {unitSquad.Unit.name}");
                    onFieldUnits.Add(unitSquad);
                    unitSquad.Unit.GetComponent<ArmyUnitClass>().Abilities[0].MainFunc(opposingHero,false);
                }
            }
        }
        return onFieldUnits;
    }
}
