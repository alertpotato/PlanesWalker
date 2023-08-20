using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class ArmyDeck : MonoBehaviour
{
    private Hero armyHero;
    private List<GameObject> _cards;
    private GameObject deckSpace;
    public void GetArmyHero(Hero _hero)
    {
        if (armyHero == null) { armyHero = _hero; }
    }
    private void Awake()
    {
        deckSpace = Instantiate(Resources.Load<GameObject>("Prefab/DeckSpace"));
        deckSpace.transform.SetParent(this.transform);
    }
    private void Start()
    {
        _cards = new List<GameObject>();
        deckSpace.transform.localPosition = Camera.main.ScreenToWorldPoint(new Vector3((Screen.width / 10), Screen.height/2, 10));
        deckSpace.SetActive(false);
    }
    private List<ArmyUnitClass> CreateSortList()
    {
        var armyList = armyHero.bannersList;
        armyList = armyList.OrderBy(x => x.unitname).ToList();
        return armyList;
    }
    private void ShowCards()
    {
        var _armyListSorted = CreateSortList();
        float stepX = 0; float stepY = 0; float stepZ = 0;
        string prevCardName = "";

        for (int i = 0; i < armyHero.bannersList.Count; i++)
        {
            if (_armyListSorted[i].unitname != prevCardName) { stepY += 150; stepX = 0; stepZ = 0; }
            else { stepX += 70; stepZ = 0.1f; }
            var a = Camera.main.ScreenToWorldPoint(new Vector3( (Screen.width / 20) + stepX, Screen.height - stepY, 8 + stepZ));
            _cards.Add(Instantiate(Resources.Load<GameObject>("Prefab/UnitCardMain")));
            _cards[i].GetComponent<UnitCardMain>().SetUnitParameters(_armyListSorted[i].unitname, _armyListSorted[i], a,true);
            prevCardName = _armyListSorted[i].unitname;
        }
    }
    private void WipeCards()
    {
        if (_cards.Count > 0)
        {
            foreach (GameObject _card in _cards)
            {
                Destroy(_card);
            }
            _cards.Clear();
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown("q") )
        {
            if (deckSpace.activeSelf == true) { WipeCards(); deckSpace.SetActive(false); }
            else { deckSpace.SetActive(true); ShowCards(); }
        }
    }
}