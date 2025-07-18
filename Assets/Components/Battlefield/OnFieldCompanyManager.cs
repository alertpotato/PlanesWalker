using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using ColorUtility = UnityEngine.ColorUtility;

public class OnFieldCompanyManager : MonoBehaviour
{
    [Header("Components")] public Battlefield Battlefield;
    public SpriteRenderer CompanySprite;
    public SpriteRenderer BackSprite;
    public GameObject DeadIcon;
    public GameObject AbilityButtonsParent;
    public GameObject Highlight;
    [Header("UI")] public GameObject StatsUI;
    public TextMeshProUGUI NumberText;
    public TextMeshProUGUI HealthText;
    public TextMeshProUGUI DamageText;
    public TextMeshProUGUI InitiativeText;
    public TextMeshProUGUI CohesionText;
    public TextMeshProUGUI ArmourText;
    public TextMeshProUGUI EffectivenessText;
    public TextMeshProUGUI HintText;
    [Header("Prefabs")] public GameObject AbilityButton;
    [Header("Variables")] public Company Company;
    List<GameObject> AbilityButtons = new List<GameObject>();

    public void InitializeCell(Company company, Battlefield battlefield, float AbilitiesPosMod = -75)
    {
        Battlefield = battlefield;
        Company = company;
        HideUI(false);
        ////Mirror abilities interface position
        AbilityButtonsParent.transform.localPosition = new Vector3(0, AbilitiesPosMod, 0);
    }

    public void ChangeSprite(Sprite newSprite)
    {
        CompanySprite.sprite = newSprite;
    }

    public void DisableCellText()
    {
        HideUI(false);
        CompanyHint(Company.Type.ToString() + "#" + Company.occupiedPositions[0]);
    }

    private void HideUI(bool hide)
    {
        StatsUI.SetActive(hide);
        DeadIcon.SetActive(hide);
    }

    //If field is empty add Hint text of company Type
    public void CompanyHint(string newHint = "")
    {
        HintText.text = newHint;
    }

    public void CreateAbilityButton(int index, BattlefieldLogic obj, Sprite icon, bool isButtonsActive)
    {
        var newButton = Instantiate(AbilityButton, AbilityButtonsParent.transform);
        newButton.GetComponent<FieldCompanyAbilityButtonManager>()
            .InitializeButton(index, obj, icon, gameObject, isButtonsActive);
        AbilityButtons.Add(newButton);
    }

    public void ResetAbilityButtons()
    {
        foreach (var button in AbilityButtons)
        {
            Destroy(button);
        }

        AbilityButtons.Clear();
    }

    public void SelectAbility(int index)
    {
        foreach (var button in AbilityButtons)
        {
            button.GetComponent<FieldCompanyAbilityButtonManager>().DeselectAbility();
        }

        if (index != -1) AbilityButtons[index].GetComponent<FieldCompanyAbilityButtonManager>().SelectAbility();
    }

    public void UpdateCellText()
    {
        CompanyHint();
        HideUI(true);
        Color green = new Color(0.062f, 0.729f, 0, 1);
        Color yelow = new Color(0.835f, 0.729f, 0, 1);
        var unitClass = Company.Unit.GetComponent<ArmyUnitClass>();
        float baseH = unitClass.unit.BaseUnitAttributes.Health * unitClass.unit.BaseUnitAttributes.SquadSize;
        float curH = unitClass.currentSquadHealth;
        var newHColor = ColorUtility.ToHtmlStringRGBA(Color.Lerp(yelow, green, curH / baseH));
        if (curH / baseH < 0.5f) newHColor = ColorUtility.ToHtmlStringRGBA(Color.Lerp(Color.red, yelow, curH / baseH));
        HealthText.text = $"<color=#{newHColor}>{curH}</color>";

        float baseN = unitClass.unit.BaseUnitAttributes.SquadSize;
        float curN = unitClass.unit.CurrentUnitAttributes.SquadSize;
        var newNColor = ColorUtility.ToHtmlStringRGBA(Color.Lerp(Color.red, green, curN / baseN));
        NumberText.text = $"<color=#{newNColor}>{curN}</color>";
        
        DamageText.text = $"0/0";
        InitiativeText.text = $"0/0";

        float baseC = unitClass.unit.BaseUnitAttributes.Cohesion;
        float curC = unitClass.unit.CurrentUnitAttributes.Cohesion;
        string newCColor;
        if (curC > 0)
            newCColor = ColorUtility.ToHtmlStringRGBA(Color.Lerp(Color.yellow, green,
                curC / Mathf.Clamp(baseC, 5, 10)));
        else if (curC == 0) newCColor = ColorUtility.ToHtmlStringRGBA(Color.yellow);
        else newCColor = ColorUtility.ToHtmlStringRGBA(Color.Lerp(Color.yellow, Color.red, Mathf.Abs(curC) / 10));
        CohesionText.text = $"<color=#{newCColor}>{curC}</color>";
        
        ArmourText.text = $"0/0";
        
        EffectivenessText.text = $"0%";
        //Indicator that unit is dead
        if (unitClass.currentSquadHealth <= 0)
        {
            DeadIcon.SetActive(true);
            ChangeSprite(null);
        }
        else DeadIcon.SetActive(false);
    }

    public void HighlightField()
    {
        Highlight.SetActive(true);
    }
    public void DeHighlightField()
    {
        Highlight.SetActive(false);
    }

    public void SelectField(GameObject newUnit,GameObject unitOwner)
    {
        Battlefield.AddUnitToFormationLogic(newUnit,Company,this,unitOwner);
    }

    public void DeselectField(GameObject unitOwner)
    {
        if (Company.unitOwner != unitOwner.GetComponent<Hero>()) return;
        Battlefield.RemoveUnitFromFormationLogic(Company,this);
    }
}