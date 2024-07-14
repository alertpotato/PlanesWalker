using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Lumin;

[CreateAssetMenu]
public class PlayerData : ScriptableObject
{
    public int[] UnitSupplyReq = new int[4]; // food, weapons, money, wisdom

    public int[] AddSupply(int index,int value)
    {
        UnitSupplyReq[index] = value;
        return UnitSupplyReq;
    }
}