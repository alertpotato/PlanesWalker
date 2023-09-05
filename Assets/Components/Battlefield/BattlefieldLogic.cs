using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattlefieldLogic : MonoBehaviour
{
    public Battlefield Battlefield;
    public List<UnitAbility> AbilitiesOrder;
    public  RectTransform AbilityUIBack;
    public GameObject AbilityUI;
    public GameObject AbilityUIParent;
    public List<GameObject> AbilitiesUI;
    public Sprite AbilityIcon;
    public UnitGraphic UnitSprites;
        

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
        // -------------GRAPHIC
        float YY = -60;
        int index = 0;
        foreach (var ability in AbilitiesOrder)
        {
            float YYY = YY * index;
            var newUI = Instantiate(AbilityUI,AbilityUIParent.transform);
            newUI.transform.localPosition = new Vector3(0,YYY,0);
            AbilitiesUI.Add(newUI);
            ability.MainFunc();
            newUI.GetComponent<AbilityOrderUI>().SetIcons(AbilityIcon,UnitSprites.GetIconSpriteByName(ability.Unit.GetComponent<ArmyUnitClass>().UnitName),
                UnitSprites.GetIconSpriteByName(ability.OpposingHero.ArmyFormation[ability.targets[0][0]].ArmyLine[ability.targets[0][1]].Unit.GetComponent<ArmyUnitClass>().UnitName));
            index++;
        }
        //AbilityUIBack.rect.Set(AbilityUIBack.rect.x,AbilityUIBack.rect.y,1,114+114*AbilitiesOrder.Count);
        AbilityUIBack.sizeDelta = new Vector2(AbilityUIBack.rect.width,114*AbilitiesOrder.Count);
        Debug.Log(answer);
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
                    onFieldUnits.Add(unitSquad);
                    unitSquad.Unit.GetComponent<ArmyUnitClass>().Abilities[0].InitAbility(
                        new int[2] { unitSquad.Line, unitSquad.Column }, unitSquad.Unit, unitHero, opposingHero);
                }
            }
        }
        return onFieldUnits;
    }
}
