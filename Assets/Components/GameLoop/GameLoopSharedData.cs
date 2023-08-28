using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum eventState { chosingCards, presentArmies };
public class GameLoopSharedData : MonoBehaviour
{
    public eventState state { get; private set; }
    public List<GameObject> _cards;
    public List<GameObject> _cardsEnemy;
    public List<GameObject> _newArmyList;
    [SerializeField]private GameObject YourHero;
    [SerializeField]private GameObject EvilHero;
    //[SerializeField]private GameObject YourArmy;
    [SerializeField]private GameObject Unit;
    [SerializeField]private GameObject UnitCardMain;
    private int _countUnitsSelected;
    public ListOfCommonUnits listOfCommonUnits;
    public ListOfObjects listOfCommonObjects;
    public GameObject Battlefield;
    private List<Sprite> spriteUiList;
    private GameObject cardChosenToField;
    private void Awake()
    {
        YourHero = Instantiate(YourHero);
        EvilHero = Instantiate(EvilHero);
        _newArmyList = new List<GameObject>();
        _cards = new List<GameObject>();
        _cardsEnemy = new List<GameObject>();
    }
    
    private void Start()
    {
        YourHero.GetComponent<Hero>().modifyHero("Chosen one", 1, 1);
        EvilHero.GetComponent<Hero>().modifyHero("Dark Lord", 1, 1);
        YourHero.name = YourHero.GetComponent<Hero>().heroName;
        EvilHero.name = EvilHero.GetComponent<Hero>().heroName;
        Battlefield.GetComponent<ArmyField>().InicializeField(YourHero.GetComponent<Hero>(), EvilHero.GetComponent<Hero>());
        GameObject.Find("ArmyDeck").GetComponent<ArmyDeck>().GetArmyHero(YourHero.GetComponent<Hero>());

        AddEvilArmy(EvilHero.GetComponent<Hero>());
        _countUnitsSelected = 0;
        spriteUiList = new List<Sprite>(Resources.LoadAll<Sprite>("Sprites/ui"));
        //YourArmy = Instantiate(YourArmy);
    }
    private void Update()
    {
        if (Input.GetKeyDown("s") && state == eventState.chosingCards)
        {
            ChosingArmyEvent();
        }
        if (Input.GetKeyDown("t") && state == eventState.chosingCards)
        {
            _countUnitsSelected++;
            StaticChosingArmyEvent();
        }
        if (Input.GetKeyDown("r") && state == eventState.chosingCards)
        {
            Battlefield.SetActive(!Battlefield.gameObject.activeSelf); 
        }
    }
    private void CardChosen(GameObject _chosenCard,bool isInArmy)
    {
        if (state == eventState.presentArmies && isInArmy == true)
        {
            CardChosenToCell(_chosenCard);
        }
        else if (state == eventState.chosingCards && isInArmy==false)
        {
            TakeCardAndClear(_chosenCard);
        }
    }
    private void CellChosen(GameObject _chosenCell)
    {
        if (state == eventState.presentArmies && cardChosenToField!=null)
        {
            cardChosenToField.transform.position = _chosenCell.transform.position;

            //Sprite _sprite = listOfCommonObjects.GetSpriteByName(cardChosenToField.GetComponent<UnitCardMain>().GetCardUnit().unitname, "units") ;

            _chosenCell.GetComponent<ArmyCellScript>().SetSpriteByName(cardChosenToField.GetComponent<UnitCardMain>().GetCardUnit().GetComponent<ArmyUnitClass>().UnitName);
            //_chosenCell.GetComponent<ArmyCellScript>().backSpriteRendererScript.SetSpriteByName(cardChosenToField.GetComponent<UnitCardMain>().GetCardUnit().unitname);
            //_chosenCell.GetComponent<ArmyCellScript>().ChangeSprite(_sprite);

            cardChosenToField = null;
        }
        else
        {

        }
    }
    private void PresentArmies(Hero _yourHero, Hero _enemyHero)
    {
        for (int i = 0; i < _yourHero.bannersList.Count; i++)
        {
            var a = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 4, (Screen.height / (_yourHero.bannersList.Count + 1)) * (i + 1), 8));
            _cards.Add(Instantiate(UnitCardMain));
            _cards[i].GetComponent<UnitCardMain>().SetUnitParameters(_yourHero.bannersList[i], a,true);
        }
        for (int i = 0; i < _enemyHero.bannersList.Count; i++)
        {
            var a = Camera.main.ScreenToWorldPoint(new Vector3((Screen.width / 4) * 3, (Screen.height / (_enemyHero.bannersList.Count + 1)) * (i + 1), 8));
            _cardsEnemy.Add(Instantiate(UnitCardMain));
            _cardsEnemy[i].GetComponent<UnitCardMain>().SetUnitParameters(_enemyHero.bannersList[i], a, true);
        }
    }
    private void ChosingArmyEvent() { CreateCardChoice(5); }
    private void StaticChosingArmyEvent() { CreateCardChoice(5); }

    private GameObject InstantiateRandomUnit(Race unitRace)
    {
        var newUnitCharacteristics = listOfCommonUnits.GetRandomUnit(unitRace);
        GameObject newUnit = Instantiate(Unit);
        ArmyUnitClass unitClass = newUnit.GetComponent<ArmyUnitClass>();
        unitClass.InitializeUnit(newUnitCharacteristics.Item1,newUnitCharacteristics.Item2,newUnitCharacteristics.Item3);
        newUnit.name = $"{unitClass.UnitName}_{newUnit.GetInstanceID()}";
        return newUnit;
    }

    private void CreateCardChoice(int cardsNum)
    {
        for (int i = 0; i < cardsNum; i++)
        {
            GameObject newUnit = InstantiateRandomUnit(Race.Human);
            var a = Camera.main.ScreenToWorldPoint(new Vector3((Screen.width / (cardsNum + 2)) * (i + 1), Screen.height/2, 8));
            _newArmyList.Add(newUnit);
            GameObject newCard = Instantiate(UnitCardMain);
            newCard.name = $"{newUnit.name}_Card";
            newCard.transform.SetParent(newUnit.transform);
            _cards.Add(newCard);
            _cards[i].GetComponent<UnitCardMain>().SetUnitParameters(newUnit, a,false);
        }
    }

    private void TakeCardAndClear(GameObject _chosenCard)
    {
        int _cardIndex = _cards.FindIndex(x => x.Equals(_chosenCard));
        YourHero.GetComponent<Hero>().AddBannerList(_newArmyList[_cardIndex]); _newArmyList.Remove(_newArmyList[_cardIndex]);
        WipeCard(_cardIndex);
        foreach (GameObject __card in _cards)
        {
            Destroy(__card);
        }
        foreach (GameObject _army in _newArmyList)
        {
            Destroy(_army);
        }
        _cards.Clear();
        _newArmyList.Clear();
        if (_countUnitsSelected == 3) { state = eventState.presentArmies; PresentArmies(YourHero.GetComponent<Hero>(), EvilHero.GetComponent<Hero>()); _countUnitsSelected = 0; }
    }
    private void WipeCard(int cardIndex)
    {
        Destroy(_cards[cardIndex]);
        _cards.Remove(_cards[cardIndex]);
        
    }
    private void WipeArmy(int cardIndex)
    {
        Destroy(_newArmyList[cardIndex]);
        _newArmyList.Remove(_newArmyList[cardIndex]);
    }
    public void CardChosenToCell(GameObject _chosenCard)
        {
            cardChosenToField = _chosenCard;
        }

    private void AddEvilArmy(Hero _hero)
    {
        for (int i = 0; i < 3; i++)
        {
            GameObject newEvilUnit = InstantiateRandomUnit(Race.Goblin);
            _hero.AddBannerList(newEvilUnit);
        }
    }
    //private void StaticCardChise()
    //{
    //    var a1 = Camera.main.ScreenToWorldPoint(new Vector3((Screen.width / (4)) * (1), Screen.height / 2, 8));
    //    ArmyUnitClass newSquad = new ArmyUnitClass("local_militia", Random.Range(10, 35), Random.Range(0, 3) + 4, 2 + Random.Range(0, 3), Random.Range(0, 3) - 1, Random.Range(0, 3) - 1, 5,0);
    //    _newArmyList.Add(newSquad);
    //    _cards.Add(Instantiate(Resources.Load<GameObject>("Prefab/UnitCardMain")));
    //    _cards[0].GetComponent<UnitCardMain>().SetUnitParameters("local_militia", newSquad, a1);

    //    var a2 = Camera.main.ScreenToWorldPoint(new Vector3((Screen.width / (4)) * (2), Screen.height / 2, 8));
    //    ArmyUnitClass secondSquad = new ArmyUnitClass("dirty_thieves", Random.Range(10, 25), Random.Range(0, 3) + 4, 5 + Random.Range(0, 3), Random.Range(1, 5) - 1, Random.Range(0, 3) - 1, 5,0);
    //    _newArmyList.Add(secondSquad);
    //    _cards.Add(Instantiate(Resources.Load<GameObject>("Prefab/UnitCardMain")));
    //    _cards[1].GetComponent<UnitCardMain>().SetUnitParameters("dirty_thieves", secondSquad, a2);

    //    var a3 = Camera.main.ScreenToWorldPoint(new Vector3((Screen.width / (4)) * (3), Screen.height / 2, 8));
    //    ArmyUnitClass thirdSquad = new ArmyUnitClass("trained_warriors", Random.Range(7, 16), Random.Range(2, 6) + 8, 3 + Random.Range(1, 4), Random.Range(1, 5) - 1, Random.Range(2, 6) - 1, 5,0);
    //    _newArmyList.Add(thirdSquad);
    //    _cards.Add(Instantiate(Resources.Load<GameObject>("Prefab/UnitCardMain")));
    //    _cards[2].GetComponent<UnitCardMain>().SetUnitParameters("trained_warriors", thirdSquad, a3);
    //}
    private void OnEnable()
    {
        EventManager.CardChosen += CardChosen;
        EventManager.CellChosen += CellChosen;
    }
    private void OnDisable()
    {
        EventManager.CardChosen -= CardChosen;
        EventManager.CellChosen -= CellChosen;
    }
}
