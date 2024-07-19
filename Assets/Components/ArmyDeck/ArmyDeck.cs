using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class ArmyDeck : MonoBehaviour
{
    public Hero armyHero;
    public List<GameObject> _cards;
    public GameObject yourDeckSpace;
    [SerializeField]private GameObject UnitCard;
    public void GetArmyHero(Hero hero)
    {
        if (armyHero == null) { armyHero = hero; }
    }

    private void Awake()
    {
        _cards = new List<GameObject>();
    }

    private void Start()
    {
        yourDeckSpace.transform.localPosition = Camera.main.ScreenToWorldPoint(new Vector3((Screen.width / 10), Screen.height/2, 10));
        yourDeckSpace.SetActive(false);
    }
    private List<GameObject> CreateSortList()
    {
        var armyList = armyHero.bannersList;
        armyList = armyList.OrderBy(x => x.GetComponent<ArmyUnitClass>().UnitName).ToList();
        return armyList;
    }
    public void ShowCards()
    {
        var armyListSorted = CreateSortList();
        float stepX = 0; float stepY = 0; float stepZ = 0;
        string prevCardName = "";

        for (int i = 0; i < armyHero.bannersList.Count; i++)
        {
            if (armyListSorted[i].GetComponent<ArmyUnitClass>().UnitName != prevCardName) { stepY += 170; stepX = 0; stepZ -= 0.025f; }
            else { stepX += 100; stepZ += 0f; }
            var a = Camera.main.ScreenToWorldPoint(new Vector3( (Screen.width / 20) + stepX, Screen.height - stepY, 8 + stepZ));
            _cards.Add(Instantiate(UnitCard));
            _cards[i].GetComponent<UnitCardMain>().SetUnitParameters(armyListSorted[i], a,Vector3.one*0.7f,false);
            prevCardName = armyListSorted[i].GetComponent<ArmyUnitClass>().UnitName;
        }
    }
    public void WipeCards()
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
    public void UpdateDeck()
    {
        if (yourDeckSpace.activeSelf == true) { WipeCards();}
        else { ShowCards(); }
    }
}