using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattlefieldLogic : MonoBehaviour
{
    public Battlefield Battlefield;
    public List<UnitAbility> AbilitiesOrder = new List<UnitAbility>();
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
            ability.MainFunc(true);
        }
    }

    public void Order()
    {
        DestroyUI();
        AbilitiesUI = new List<GameObject>();
        AbilitiesOrder.Clear();
        List<Company> onFieldUnits = new List<Company>();

        var onFieldPlayerCompanies = Battlefield.PlayerFormation.GetOnFieldcompanies();
        var onFieldEnemyCompanies = Battlefield.EnemyFormation.GetOnFieldcompanies();

        onFieldUnits.AddRange(onFieldPlayerCompanies);
        onFieldUnits.AddRange(onFieldEnemyCompanies);
        var sortedUnits = from comp in onFieldUnits
            orderby comp.Unit.GetComponent<ArmyUnitClass>().CurrentUnitCharacteristics.Initiative descending
            select comp;
        string answer = "";
        foreach (var comp in sortedUnits)
        {
            answer = $"{answer} -> {comp.Unit.gameObject.name}[{comp.Unit.GetComponent<ArmyUnitClass>().CurrentUnitCharacteristics.Initiative}]";
            AbilitiesOrder.Add(comp.Unit.GetComponent<ArmyUnitClass>().Abilities[0]);
        }
        //Debug.Log(answer);
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
                UnitSprites.GetIconSpriteByName(ability.UnitCompany.Unit.GetComponent<ArmyUnitClass>().UnitName),
                UnitSprites.GetIconSpriteByName(ability.OpposingField.Formation[ability.targets[0].Item1].Line[ability.targets[0].Item2].Unit.GetComponent<ArmyUnitClass>().UnitName)
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
}
