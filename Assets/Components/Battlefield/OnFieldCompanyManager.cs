using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OnFieldCompanyManager : MonoBehaviour
{
    public SpriteRenderer spriteComponent;
    public BoxCollider CellCollider;
    public float colliderThickness=0.1f;
    public GameObject UI;
    public Company Company;
    public TextMeshProUGUI UnitHealthText;
    public TextMeshProUGUI UnitNumberText;
    public TextMeshProUGUI UnitPowerText;
    
    public void InitializeCell(Company company)
    {
        Company = company;
        UI.SetActive(false);
    }
    public void ChangeSprite(Sprite newSprite)
    {
        spriteComponent.sprite = newSprite;
        CellCollider.size = new Vector3(spriteComponent.size.x, spriteComponent.size.y, colliderThickness);
    }

    public void DisableCellText()
    {
        UI.SetActive(false);
    }

    public void UpdateCellText()
    {
        UI.SetActive(true);
        
        var unit = Company.Unit.GetComponent<ArmyUnitClass>();
        float baseH = unit.BaseCharacteristics.Health * unit.BaseCharacteristics.NumberOfUnits;
        float curH = unit.currentSquadHealth;
        if (curH / baseH > 0.75f) UnitHealthText.text = $"<color=\"green\">{curH}</color>/{baseH}";
        else if (curH / baseH > 0.33f) UnitHealthText.text = $"<color=\"yellow\">{curH}</color>/{baseH}";
        else UnitHealthText.text = $"<color=\"red\">{curH}</color>/{baseH}";

        float baseN = unit.BaseCharacteristics.NumberOfUnits;
        float curN = unit.CurrentUnitCharacteristics.NumberOfUnits;
        if (curN / baseN > 0.75f) UnitNumberText.text = $"<color=\"green\">{curN}</color>/{baseN}";
        else if (curN / baseN > 0.33f) UnitNumberText.text = $"<color=\"yellow\">{curN}</color>/{baseN}";
        else UnitNumberText.text = $"<color=\"red\">{curN}</color>/{baseN}";
        
        var coh = unit.CurrentUnitCharacteristics.Cohesion;
        if (coh < 0) UnitPowerText.text = $"<color=\"red\">{coh}</color>";
        else if (coh < unit.BaseCharacteristics.Cohesion) UnitPowerText.text = $"<color=\"yellow\">{coh}</color>";
        else UnitPowerText.text = $"<color=\"green\">{coh}</color>";
    }
}