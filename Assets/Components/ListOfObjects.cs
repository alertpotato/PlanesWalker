using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ListOfObjects : ScriptableObject
{
    public Sprite GetSpriteByName(string name, string list)
    {
        List<Sprite> spriteList = null ;
        if (list == "units") { spriteList = spriteUnitList; }
        if (list == "ui") { spriteList = spriteUiList; }
        int _index = 0;
        //int i = -1;
        for (int i = 0; i < spriteList.Count; i++)
        //foreach (Sprite _sprite in spritetList)
        {
            Debug.Log(_index);
            if (spriteList[i].name == name) { _index = i; }
        }
        return spriteList[_index];
    }
    public List<Sprite> spriteUiList;
    public List<Sprite> spriteUnitList;
    void Awake()
    {
        spriteUiList = new List<Sprite>(Resources.LoadAll<Sprite>("Sprites/ui"));
        spriteUnitList = new List<Sprite>(Resources.LoadAll<Sprite>("Sprites/spites"));
    }
}
