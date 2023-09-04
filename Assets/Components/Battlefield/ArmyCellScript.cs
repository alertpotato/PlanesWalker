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

    public void InicializeCell(int line, int column)
    {
        formationLine = line;
        formationColumn = column;
    }
    public (int,int) GetSquad()
    {
        return (formationLine,formationColumn);
    }
    public void ChangeSprite(Sprite newSprite)
    {
        spriteComponent.sprite = newSprite;
        CellCollider.size = new Vector3(spriteComponent.size.x, spriteComponent.size.y, colliderThickness);
    }
}