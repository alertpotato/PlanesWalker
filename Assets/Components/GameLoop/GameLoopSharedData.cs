using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class GameLoopSharedData : MonoBehaviour
{
    [Header("Components")]
    public Camera MainCamera;
    public SelectManager SelectedUnits;
    public GameObject Battlefield;
    public GameObject ArmyDeck;
    public ListOfCommonUnits listOfCommonUnits;
    public GameObject YourHero;
    public GameObject EvilHero;
    public GameObject Unit;
    public GameObject UnitCard;

    public LayerMask UnitLayer;
    public LayerMask BattlefieldLayer;
    public int CurrentRound = 0;
    private void Awake()
    {
        YourHero = Instantiate(YourHero);
        EvilHero = Instantiate(EvilHero);
        Battlefield.SetActive(false);
    }
    private void Start()
    {
        YourHero.GetComponent<Hero>().modifyHero("Chosen one", 1, 1);
        EvilHero.GetComponent<Hero>().modifyHero("Dark Lord", 1, 1);
        YourHero.name = YourHero.GetComponent<Hero>().heroName;
        EvilHero.name = EvilHero.GetComponent<Hero>().heroName;
        ArmyDeck.GetComponent<ArmyDeck>().GetArmyHero(YourHero.GetComponent<Hero>());
        AddEvilArmy(EvilHero.GetComponent<Hero>());
    }
    void OnClick(InputValue value)
    {
        Ray ray = MainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, UnitLayer))
        {
            SelectedUnits.SelectEntity(hit.collider.gameObject);
        }
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, BattlefieldLayer) & SelectedUnits.IsEntitySelected())
        {
            if (Battlefield.GetComponent<Battlefield>()
                .AddUnitToField(hit.collider.gameObject.GetComponent<ArmyCellScript>().GetSquad(),
                    SelectedUnits.SelectedEntity.GetComponent<UnitCardMain>().RelatedUnit))
            {
                //TODO EvilHero.GetComponent<Hero>().RandomUnitAllocating(YourHero.GetComponent<Hero>());
                Battlefield.GetComponent<BattlefieldLogic>().Order();
            }
        }
    }
    private void OnAlternateClick(InputValue value)
    {
        Ray ray = MainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, BattlefieldLayer))
        {
            //YourHero.GetComponent<Hero>().RemoveUnitFromFormation(hit.collider.gameObject.GetComponent<ArmyCellScript>().GetSquad());
            Battlefield.GetComponent<Battlefield>().RemoveAllFormations();
            Battlefield.GetComponent<BattlefieldLogic>().Order();
        }
        else SelectedUnits.DeSelectEntity();
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
