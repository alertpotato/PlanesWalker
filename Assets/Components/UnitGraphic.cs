using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class UnitGraphicClass
{
    public string UnitType;
    public Sprite Card;
    public Sprite BattleIcon;

    public UnitGraphicClass(string unitType, Sprite card, Sprite battleIcon)
    {
        UnitType = unitType;
        Card = card;
        BattleIcon = battleIcon;
    }
}
[CreateAssetMenu]
public class UnitGraphic : ScriptableObject
{
    public List<UnitGraphicClass> UnitGraphics;
    
    public Sprite GetCardSpriteByName(string name)
    {
        return UnitGraphics.Find(x => x.UnitType == name).Card;
    }
    public Sprite GetIconSpriteByName(string name)
    {
        return UnitGraphics.Find(x => x.UnitType == name).BattleIcon;
    }
}
