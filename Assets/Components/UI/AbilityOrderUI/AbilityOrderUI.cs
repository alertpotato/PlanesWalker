using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilityOrderUI : MonoBehaviour
{
    public Image AbilityIcon;
    public Image FromIcon;
    public Image ToIcon;
    public Image Back;
    public UnitInOrder Unit;
    public UnitGraphic UnitSprites;
    public OtherGraphic IconsSprites;
    public TextMeshProUGUI OrderIndex;
    public TextMeshProUGUI FromResults;
    public TextMeshProUGUI ToResults;
    public GameObject Highlight;
    public void InitializeUI(UnitInOrder unit,Color backColor,int index)
    {
        Unit = unit;
        Back.color = backColor;
        OrderIndex.text = index.ToString();
        FromIcon.sprite = UnitSprites.GetIconSpriteByName(Unit.UnitCompany.Unit.GetComponent<ArmyUnitClass>().squadName);
        List<Company> abTargets = new List<Company>();
        if (Unit.UnitAbility != null) abTargets.AddRange(Unit.UnitAbility.GetAbilityTargets());
        if (abTargets.Count > 0)
        {
            AbilityIcon.sprite = IconsSprites.GetSpriteByName(Unit.UnitAbility.AbilityName);
            ToIcon.sprite =
                UnitSprites.GetIconSpriteByName(
                    (Unit.UnitAbility.targets[0].Unit.GetComponent<ArmyUnitClass>().squadName));
        }
    }

    public void UpdateRoundResults(string resultsFrom, string resultsTo)
    {
        FromResults.text = resultsFrom;
        ToResults.text = resultsTo;
    }

    public void HighlightElement()
    {
        Highlight.SetActive(true);
    }
    public void DeHighlightElement()
    {
        Highlight.SetActive(false);
    }
}
