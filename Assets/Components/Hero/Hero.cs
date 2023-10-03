using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    [Header("Characteristics")]
    public string heroName;
    public List<GameObject> bannersList = new List<GameObject> { };

    [Header("HeroModifiers")]
    [SerializeField] private int modinit = 0;
    [SerializeField] private int modcoh = 0;

//------------------------------------
    public void modifyHero(string n, int init, int coh) { heroName = n; modinit = init; modcoh = coh; }
    public void AddBannerList(GameObject unit)
    {
        bannersList.Add(unit);
        unit.GetComponent<ArmyUnitClass>().ApplyHeroModifyers(modinit, modcoh);
        unit.transform.SetParent(transform);
    }
}
