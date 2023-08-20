using System.Collections.Generic;
using UnityEngine;

public class ArmyFieldCellBackSpriteScript : MonoBehaviour
{
    private SpriteRenderer spriteComponent;
    [SerializeField] private List<Sprite> spritetList;
    private void Awake()
    {
        spritetList = new List<Sprite>(Resources.LoadAll<Sprite>("Sprites/spites"));
        spriteComponent = GetComponent<SpriteRenderer>();
    }
    private void Start()
    {
    }
    public void SetSpriteByName(string _spriteName)
    {
        spriteComponent.sprite = GetSpriteByName(_spriteName);
        //Debug.Log(GetComponent<SpriteRenderer>().bounds.size.x);
        //Debug.Log(GetComponent<SpriteRenderer>().bounds);
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
}