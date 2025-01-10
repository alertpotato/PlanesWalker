using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OnFieldCompanyManager : MonoBehaviour
{
    [Header("Components")]
    public Company Company;
    public SpriteRenderer CompanySprite;
    public SpriteRenderer AbilitySprite;
    [Header("UI")]
    public GameObject UI;
    public TextMeshProUGUI NumberText;
    public TextMeshProUGUI HealthText;
    public TextMeshProUGUI DamageText;
    public TextMeshProUGUI InitiativeText;
    public TextMeshProUGUI CohesionText;
    public TextMeshProUGUI ArmourText;
    
    public void InitializeCell(Company company)
    {
        Company = company;
        UI.SetActive(false);
    }
    public void ChangeSprite(Sprite newSprite)
    {
        CompanySprite.sprite = newSprite;
    }

    public void DisableCellText()
    {
        UI.SetActive(false);
    }

    public void UpdateCellText()
    {
        UI.SetActive(true);
        Color green = new Color(0.062f, 0.729f, 0, 1);
        Color yelow = new Color(0.835f, 0.729f, 0, 1);
        var unit = Company.Unit.GetComponent<ArmyUnitClass>();
        float baseH = unit.BaseCharacteristics.Health * unit.BaseCharacteristics.NumberOfUnits;
        float curH = unit.currentSquadHealth;
        var newHColor = ColorUtility.ToHtmlStringRGBA(Color.Lerp(yelow,green, curH / baseH));
        if (curH/baseH < 0.5f) newHColor =  ColorUtility.ToHtmlStringRGBA(Color.Lerp(Color.red, yelow, curH / baseH));
        HealthText.text = $"<color=#{newHColor}>{curH}</color>";

        float baseN = unit.BaseCharacteristics.NumberOfUnits;
        float curN = unit.CurrentUnitCharacteristics.NumberOfUnits;
        var newNColor = ColorUtility.ToHtmlStringRGBA(Color.Lerp(Color.red,green, curN / baseN));
        NumberText.text = $"<color=#{newNColor}>{curN}</color>";
        
        float baseD = unit.BaseCharacteristics.Damage;
        float curD = unit.CurrentUnitCharacteristics.Damage;
        DamageText.text = $"{curD}/{baseD}";
        
        float baseI = unit.BaseCharacteristics.Initiative;
        float curI = unit.CurrentUnitCharacteristics.Initiative;
        InitiativeText.text = $"{curI}/{baseI}";
        
        float baseC = unit.BaseCharacteristics.Cohesion;
        float curC = unit.CurrentUnitCharacteristics.Cohesion;
        string newCColor;
        if (curC>0) newCColor = ColorUtility.ToHtmlStringRGBA(Color.Lerp(Color.yellow,green, curC / Mathf.Clamp(baseC,5,10)));
        else if (curC==0) newCColor = ColorUtility.ToHtmlStringRGBA(Color.yellow);
        else newCColor = ColorUtility.ToHtmlStringRGBA(Color.Lerp(Color.yellow,Color.red, Mathf.Abs(curC) / 10));
        CohesionText.text = $"<color=#{newCColor}>{curC}</color>";
        
        float baseA = unit.BaseCharacteristics.Armour;
        float curA = unit.CurrentUnitCharacteristics.Armour;
        ArmourText.text = $"{curA}/{baseA}";
    }

    public void MouseOverAnswer()
    {
        Debug.Log("MouseOverAnswer");
    }
}