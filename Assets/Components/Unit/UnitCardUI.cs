using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UnitCardUI : MonoBehaviour
{
    [Header("Components")]
    public ArmyUnitClass Unit;
    [Header("Card")]
    public TextMeshProUGUI name;
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

    public void InitializeUI(ArmyUnitClass unit)
    {
        Unit = unit;
    }

    public void UpdateAllUI()
    {
        UpdateCard();
        UpdateSupply();
        UpdateStats();
    }

    private void UpdateCard()
    {
        name.text = Unit.UnitName;
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
        var currentChars = Unit.CurrentUnitCharacteristics;
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