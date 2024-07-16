using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ArmyCellScript : MonoBehaviour
{
    public SpriteRenderer spriteComponent;
    public BoxCollider CellCollider;
    public float colliderThickness=0.1f;
    public GameObject UI;
    public GameObject ui_attack_right;
    public GameObject ui_attack_left;
    public Company Company;
    public TextMeshProUGUI UnitHealthText;
    public TextMeshProUGUI UnitNumberText;
    public TextMeshProUGUI UnitPowerText;
    

    public void InitializeCell(Company company)
    {
        Company = company;
        UI.SetActive(false);
    }
    public (int,int) GetCompanyBanner()
    {
        return (Company.Banner);
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
        UnitHealthText.text = unit.currentSquadHealth + "/" + unit.BaseCharacteristics.Health * unit.BaseCharacteristics.NumberOfUnits;
        UnitNumberText.text =
            unit.CurrentUnitCharacteristics.NumberOfUnits + "/" + unit.BaseCharacteristics.NumberOfUnits;

    }
    public void GetAttackedFromRight()
    {
        ui_attack_right.SetActive(true);
    }
    public void GetAttackedFromLeft()
    {
        ui_attack_left.SetActive(true);
    }
    public void ClearAttack()
    {
        ui_attack_right.SetActive(false);
        ui_attack_left.SetActive(false);
    }
}