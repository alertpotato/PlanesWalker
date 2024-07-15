using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmyCellScript : MonoBehaviour
{
    public SpriteRenderer spriteComponent;
    public BoxCollider CellCollider;
    public float colliderThickness=0.1f;
    public int formationLine;
    public int formationColumn;
    public GameObject ui_attack_right;
    public GameObject ui_attack_left;
    public Company Company;
    
    public void InicializeCell(int line, int column)
    {
        formationLine = line;
        formationColumn = column;
    }
    public void InitializeCell(Company company)
    {
        Company = company;
    }
    public (int,int) GetSquad()
    {
        return (formationLine,formationColumn);
        //return (Company.Banner);
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