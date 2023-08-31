using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class ArmyDeck : MonoBehaviour
{
    public Hero armyHero;
    public List<GameObject> _cards;
    public GameObject yourDeckSpace;
    public GameObject enemyDeckSpace;
    [SerializeField]private GameObject UnitCard;
    public void GetArmyHero(Hero _hero)
    {
        if (armyHero == null) { armyHero = _hero; }
    }
    private void Start()
    {
        _cards = new List<GameObject>();
        yourDeckSpace.transform.localPosition = Camera.main.ScreenToWorldPoint(new Vector3((Screen.width / 10), Screen.height/2, 10));
        yourDeckSpace.SetActive(false);
        enemyDeckSpace.transform.localPosition = Camera.main.ScreenToWorldPoint(new Vector3((Screen.width / 10)*9, Screen.height/2, 10));
        enemyDeckSpace.SetActive(false);
    }
    private List<GameObject> CreateSortList()
    {
        var armyList = armyHero.bannersList;
        armyList = armyList.OrderBy(x => x.GetComponent<ArmyUnitClass>().UnitName).ToList();
        return armyList;
    }
    private void ShowCards()
    {
        var _armyListSorted = CreateSortList();
        float stepX = 0; float stepY = 0; float stepZ = 0;
        string prevCardName = "";

        for (int i = 0; i < armyHero.bannersList.Count; i++)
        {
            if (_armyListSorted[i].GetComponent<ArmyUnitClass>().UnitName != prevCardName) { stepY += 150; stepX = 0; stepZ = 0; }
            else { stepX += 70; stepZ = 0.1f; }
            var a = Camera.main.ScreenToWorldPoint(new Vector3( (Screen.width / 20) + stepX, Screen.height - stepY, 8 + stepZ));
            _cards.Add(Instantiate(UnitCard));
            _cards[i].GetComponent<UnitCardMain>().SetUnitParameters(_armyListSorted[i], a,true);
            prevCardName = _armyListSorted[i].GetComponent<ArmyUnitClass>().UnitName;
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
            if (yourDeckSpace.activeSelf == true) { WipeCards(); yourDeckSpace.SetActive(false); enemyDeckSpace.SetActive(false); }
            else { yourDeckSpace.SetActive(true);enemyDeckSpace.SetActive(true); ShowCards(); }
        }
    }
}