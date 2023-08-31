using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmyCellScript : MonoBehaviour
{
    public SpriteRenderer spriteComponent;
    [SerializeField] private List<Sprite> spritetList;
    public void ChangeSprite(Sprite newSprite)
    {
        spriteComponent.sprite = newSprite;
    }
    public void SetSpriteByName(string _spriteName)
    {
        spriteComponent.sprite = GetSpriteByName(_spriteName);
    }
    public Sprite GetSpriteByName(string _name)
    {
        int i = -1;
        foreach (Sprite _sprite in spritetList)
        {
            i++;
            if (_sprite.name == _name) { return _sprite; }
        }
        return spritetList[i];
    }
    private void OnMouseDown()
    {
        EventManager.StartCellChosen(this.gameObject);
    }
}
