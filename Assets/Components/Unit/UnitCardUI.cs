using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class UnitCardUI : MonoBehaviour
{
    [Header("Components")]
    public ArmyUnitClass unitClass;

    public SpriteRenderer Back;
    public SpriteRenderer Image;
    public Canvas UI;
    [Header("Variables")] 
    public int startingOrder;
    [Header("Card")]
    public TextMeshProUGUI cardName;
    public TextMeshProUGUI add;
    public TextMeshProUGUI multi;
    [Header("Stats")]
    public TextMeshProUGUI stat_number;
    public TextMeshProUGUI stat_health;
    public TextMeshProUGUI stat_damage;
    public TextMeshProUGUI stat_init;
    public TextMeshProUGUI stat_coh;
    public TextMeshProUGUI stat_armour;
    [Header("Supply")]
    public TextMeshProUGUI food;
    public TextMeshProUGUI weapon;
    public TextMeshProUGUI coin;
    public TextMeshProUGUI knowledge;
    [Header("Abilities")] 
    public List<GameObject> abilities;
    public GameObject AbilityPrefab;
    public GameObject VerticalGroup;
    public OtherGraphic Icons;

    public void InitializeUI(ArmyUnitClass unit)
    {
        unitClass = unit;
        abilities = new List<GameObject>();
        Back.sortingOrder = startingOrder;
        Image.sortingOrder = startingOrder+10;
        UI.sortingOrder = startingOrder+20;
    }

    public void UpdateAllUI()
    {
        UpdateCard();
        //UpdateSupply();
        UpdateStats();
    }
    public void MoveToFront(bool toMove)
    {
        int backOrder = startingOrder;
        int imageOrder = startingOrder+10;
        int uiOrder = startingOrder+20;
        if (toMove)
        {
            backOrder = startingOrder+21;
            imageOrder = startingOrder+31;
            uiOrder = startingOrder+41;
        }
        Back.sortingOrder = backOrder;
        Image.sortingOrder = imageOrder;
        UI.sortingOrder = uiOrder;
    }

    public void ShowAbilityUI()
    {
        if (abilities.Count == 0)
        {
            CreateAbilityUI();
        }
    }

    public void CreateAbilityUI()
    {
        foreach (var ability in unitClass.unit.CurrentUnitAttributes.SquadAbilities)
        {
            var newPanel = Instantiate(AbilityPrefab, VerticalGroup.transform);
            newPanel.GetComponent<AbilityPanelManager>().Initialize(Icons.GetSpriteByName(ability.AbilityName),ability.AbilityName);
            abilities.Add(newPanel);
        }
    }

    public void HideAbilityUI()
    {
        foreach (var obj in abilities)
        {
            Destroy(obj);
        }
        abilities.Clear();
    }

    private void UpdateCard()
    {
        cardName.text = unitClass.squadName;
        add.text = $"{unitClass.unit.UnitRace.ToString()}\n{unitClass.UnitAbilityTags[0].ToString()}";
        multi.text = $"x1";
    }
    private void UpdateStats()
    {
        var currentChars = unitClass.unit.CurrentUnitAttributes;
        var defaultChars = unitClass.unit.SavedUnitAttributes;

        stat_number.text = $"{currentChars.SquadSize}({defaultChars.SquadSize})";
        stat_health.text = $"{currentChars.Health}({defaultChars.Health})";
        stat_damage.text = $"0";
        stat_init.text = $"0";
        stat_coh.text = $"{currentChars.Cohesion}({defaultChars.Cohesion})";
        stat_armour.text = $"0";
    }

    private string AddStars(string starsToAdd, int numOfTimes)
    {
        for (int i = 0; i < numOfTimes; i++)
        {
            starsToAdd += "+";
        }
        return $"<color=\"yellow\">{starsToAdd}</color>";
    }
}