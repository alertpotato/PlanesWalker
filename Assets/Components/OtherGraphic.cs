using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class OtherGraphicClass
{
    public string SpriteName;
    public Sprite Icon;

    public OtherGraphicClass(string spriteName, Sprite icon)
    {
        SpriteName = spriteName;
        Icon = icon;
    }
}
[CreateAssetMenu]
public class OtherGraphic : ScriptableObject
{
    public List<OtherGraphicClass> IconGraphics;
    
    public Sprite GetSpriteByName(string name)
    {
        Sprite icon;
        if (IconGraphics.Exists(x => x.SpriteName == name))
        {
            icon = IconGraphics.Find(x => x.SpriteName == name).Icon;
        }
        else
        {
            icon = null;
        }
        return icon;
    }
}