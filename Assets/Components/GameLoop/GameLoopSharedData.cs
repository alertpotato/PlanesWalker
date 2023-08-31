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
    public GameObject YourHero;
    public GameObject EvilHero;
    public GameObject Unit;
    public GameObject UnitCard;
    private int _countUnitsSelected;
    public ListOfCommonUnits listOfCommonUnits;
    public GameObject Battlefield;
    private GameObject cardChosenToField;
    public SelectManager SelectedUnits;
    public Camera MainCamera;
    public LayerMask UnitLayer;
    private void Awake()
    {
        YourHero = Instantiate(YourHero);
        EvilHero = Instantiate(EvilHero);
        _newArmyList = new List<GameObject>();
        _cards = new List<GameObject>();
        _cardsEnemy = new List<GameObject>();
        Battlefield.SetActive(false);
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
    }
    private void PresentArmies(Hero _yourHero, Hero _enemyHero)
    {
        for (int i = 0; i < _yourHero.bannersList.Count; i++)
        {
            var a = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 4, (Screen.height / (_yourHero.bannersList.Count + 1)) * (i + 1), 8));
            _cards.Add(Instantiate(UnitCard));
            _cards[i].GetComponent<UnitCardMain>().SetUnitParameters(_yourHero.bannersList[i], a,true);
        }
        for (int i = 0; i < _enemyHero.bannersList.Count; i++)
        {
            var a = Camera.main.ScreenToWorldPoint(new Vector3((Screen.width / 4) * 3, (Screen.height / (_enemyHero.bannersList.Count + 1)) * (i + 1), 8));
            _cardsEnemy.Add(Instantiate(UnitCard));
            _cardsEnemy[i].GetComponent<UnitCardMain>().SetUnitParameters(_enemyHero.bannersList[i], a, true);
        }
    }
    public GameObject InstantiateRandomUnit(Race unitRace)
    {
        var newUnitCharacteristics = listOfCommonUnits.GetRandomUnit(unitRace);
        GameObject newUnit = Instantiate(Unit);
        ArmyUnitClass unitClass = newUnit.GetComponent<ArmyUnitClass>();
        unitClass.InitializeUnit(newUnitCharacteristics.Item1,newUnitCharacteristics.Item2,newUnitCharacteristics.Item3);
        newUnit.name = $"{unitClass.UnitName}_{newUnit.GetInstanceID()}";
        return newUnit;
    }
    private void AddEvilArmy(Hero _hero)
    {
        for (int i = 0; i < 3; i++)
        {
            GameObject newEvilUnit = InstantiateRandomUnit(Race.Goblin);
            _hero.AddBannerList(newEvilUnit);
        }
    }

}
