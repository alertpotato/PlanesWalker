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
        Sprite card;
        if (UnitGraphics.Exists(x => x.UnitType == name))
        {
            card = UnitGraphics.Find(x => x.UnitType == name).Card;
        }
        else
        {
            card = UnitGraphics.Find(x => x.UnitType == "notavailable").Card;
        }
        return card;
    }
    public Sprite GetIconSpriteByName(string name)
    {
        Sprite icon ;
        if (UnitGraphics.Exists(x => x.UnitType == name))
        {
            icon = UnitGraphics.Find(x => x.UnitType == name).BattleIcon;
        }
        else
        {
            icon = UnitGraphics.Find(x => x.UnitType == "notavailable").BattleIcon;
        }
        return icon;
    }
}