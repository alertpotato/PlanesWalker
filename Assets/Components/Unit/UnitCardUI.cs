using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UnitCardUI : MonoBehaviour
{
    [Header("Components")]
    public ArmyUnitClass Unit;

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
        Unit = unit;
        abilities = new List<GameObject>();
        Back.sortingOrder = startingOrder;
        Image.sortingOrder = startingOrder+10;
        UI.sortingOrder = startingOrder+20;
    }

    public void UpdateAllUI()
    {
        UpdateCard();
        UpdateSupply();
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
        foreach (var ability in Unit.Abilities)
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
        cardName.text = Unit.UnitName;
        add.text = $"{Unit.FactoryCharacteristics.UnitRace.ToString()}\n{Unit.UnitAbilityTags[0].ToString()}";
        multi.text = $"x{Unit.SupplyMultiplier.ToString()}";
    }
    private void UpdateSupply()
    {
        List<TextMeshProUGUI> supplys = new List<TextMeshProUGUI>(){food,weapon,coin,knowledge};
        int i = 0;
        foreach (var sup in supplys)
        {
            var req = Unit.FactoryCharacteristics.UnitSupplyReq[i];
            if (req == 0) sup.text = $"<color=#939393>{req}</color>";
            else sup.text = req.ToString();
            i++;
        }
    }
    private void UpdateStats()
    {
        var upgrd = Unit.unitUpgrades;
        var currentChars = Unit.BaseCharacteristics;
        var defaultChars = Unit.FactoryCharacteristics.Characteristics;
        string nupgrd = ""; string hupgrd = ""; string dupgrd = ""; string iupgrd = ""; string cupgrd = ""; string aupgrd = "";
        nupgrd = AddStars(nupgrd, upgrd.NumberOfUnits);
        hupgrd = AddStars(hupgrd, upgrd.Health);
        dupgrd = AddStars(dupgrd, upgrd.Damage);
        iupgrd = AddStars(iupgrd, upgrd.Initiative);
        cupgrd = AddStars(cupgrd, upgrd.Cohesion);
        aupgrd = AddStars(aupgrd, upgrd.Armour);

        stat_number.text = $"{currentChars.NumberOfUnits}({defaultChars.NumberOfUnits}){nupgrd}";
        stat_health.text = $"{currentChars.Health}({defaultChars.Health}){hupgrd}";
        stat_damage.text = $"{currentChars.Damage}({defaultChars.Damage}){dupgrd}";
        stat_init.text = $"{currentChars.Initiative}({defaultChars.Initiative}){iupgrd}";
        stat_coh.text = $"{currentChars.Cohesion}({defaultChars.Cohesion}){cupgrd}";
        stat_armour.text = $"{currentChars.Armour}({defaultChars.Armour}){aupgrd}";
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