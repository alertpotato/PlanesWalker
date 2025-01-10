using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(StateBehaviour))]
[RequireComponent(typeof(GameLoopRewardState))]
[RequireComponent(typeof(GameLoopPreBattleState))]
[RequireComponent(typeof(GameLoopRoundState))]
public class GameLoopSharedData : MonoBehaviour
{
    [Header("Components")]
    public Camera MainCamera;
    public SelectManager SelectedUnits;
    public GameObject Battlefield;
    public GameObject DeckManager;
    public StateMachine StateManager;
    public GameObject RewardParent;
    [Header("Data")]
    public PlayerData WorldData;
    public ListOfCommonUnits listOfCommonUnits;
    [Header("Entities")]
    public GameObject PlayerHero;
    public GameObject EnemyHero;
    public FormationField PlayerFormation;
    public FormationField EnemyFormation;
    private GameObject mousedOverUnit;
    private GameObject unitPreview;
    [Header("UI")] 
    public SceneInterfaceController InterfaceUI;
    [Header("Variables")]
    public int BattlesWon;
    [Header("Prefabs")]
    public GameObject Unit;
    public GameObject UnitCard;
    [Header("States")]
    public GameLoopPreBattleState PreBattleState;
    public GameLoopRewardState RewardState;
    public GameLoopRoundState RoundState;


    private void OnValidate()
    {
        PreBattleState = transform.GetComponent<GameLoopPreBattleState>();
        RewardState = transform.GetComponent<GameLoopRewardState>();
        RoundState = transform.GetComponent<GameLoopRoundState>();
        PreBattleState.Config = this;
        RewardState.Config = this;
        RoundState.Config = this;
        Battlefield.GetComponent<Battlefield>().Initialize(MainCamera,PlayerFormation,EnemyFormation);
    }

    private void Start()
    {
        //Init hero
        PlayerHero.GetComponent<Hero>().modifyHero("Planeswalker", 1, 1);
        EnemyHero.GetComponent<Hero>().modifyHero("Antagonist", 1, 1);
        
        //Init of Formation scriptable objects
        PlayerFormation.InitializeField(PlayerHero.GetComponent<Hero>());
        EnemyFormation.InitializeField(EnemyHero.GetComponent<Hero>());
        //-----------????
        DeckManager.GetComponent<Deck>().InitializeDeck(PlayerHero.GetComponent<Hero>(),MainCamera,UnitCard);
        Battlefield.GetComponent<Battlefield>().Initialize(MainCamera,PlayerFormation,EnemyFormation);
        
        //Supply
        WorldData.Reset();
        WorldData.AddSupply(0,1);
        WorldData.AddSupply(1,1);
        WorldData.AddSupply(2,1);
        InterfaceUI.UpdateSupply(WorldData.PlayerSupply);
        BattlesWon = 0;
    }
    //TODO .....
    public void TempRewards()
    {
        int newNumberOfRewards = 1;
        RewardState.NumberOfRewards = newNumberOfRewards;
    }

    private void Update()
    {
        Ray ray = MainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
        }
        else
        {
            if (mousedOverUnit) {mousedOverUnit = null; Destroy(unitPreview);}
            return;
        }
        
        if (hit.collider.gameObject.GetComponent<OnFieldCompanyManager>())
            {
                GameObject newMousedOverUnit = hit.collider.gameObject.GetComponent<OnFieldCompanyManager>().Company.Unit;
                if (mousedOverUnit != newMousedOverUnit & newMousedOverUnit)
                {
                    Destroy(unitPreview);
                    mousedOverUnit = newMousedOverUnit;
                    unitPreview = CreateCard(mousedOverUnit,0.8f,0.5f,3);
                }
            }
        else if (mousedOverUnit) {mousedOverUnit = null; Destroy(unitPreview);}
    }

    public GameObject CreateCard(GameObject unit,float x, float y,float z)
    {
        var pos = MainCamera.ScreenToWorldPoint(new Vector3((Screen.width * x), Screen.height*y, z)); //z = 5 bc its distance between cards and camera
        GameObject newCard = Instantiate(UnitCard);
        newCard.name = $"{unit.name}_CardInfo";
        newCard.transform.SetParent(unit.transform);
        newCard.GetComponent<UnitCardMain>().SetUnitParameters(MainCamera,unit, pos,Vector3.one,true,61);
        return newCard;
    }

    void OnClick(InputValue value)
    {
        //TODO Need to redone all of this
        Ray ray = MainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
        }
        else return;
        if (hit.collider.gameObject.GetComponent<UnitCardMain>())
        {
            SelectedUnits.SelectEntity(hit.collider.gameObject);
        }
        if (StateManager.CurrentState.Equals(PreBattleState))
        {
            if (hit.collider.gameObject.GetComponent<OnFieldCompanyManager>().Company.Field.FieldOwner==PlayerHero.GetComponent<Hero>() & SelectedUnits.IsEntitySelected())
            {
                if (PlayerFormation.AddUnitToFormation(hit.collider.gameObject.GetComponent<OnFieldCompanyManager>().Company,
                        SelectedUnits.SelectedEntity.GetComponent<UnitCardMain>().RelatedUnit,EnemyFormation))
                {
                    Battlefield.GetComponent<BattlefieldLogic>().Order();
                }
            }
        }
    }
    private void OnAlternateClick(InputValue value)
    {
        Debug.Log("OnAlternateClick");
        if (StateManager.CurrentState.Equals(PreBattleState))
        {
            Debug.Log("PreBattleState");
            Ray ray = MainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit,Mathf.Infinity))
            {
                Debug.Log("Hit!");
                if (hit.collider.gameObject.GetComponent<OnFieldCompanyManager>().Company.Field.FieldOwner ==
                    PlayerHero.GetComponent<Hero>() &&
                    hit.collider.gameObject.GetComponent<OnFieldCompanyManager>().Company.Unit != null)
                {
                    PlayerFormation.RemoveUnitFromField(hit.collider.gameObject.GetComponent<OnFieldCompanyManager>()
                        .Company.Unit);
                    Battlefield.GetComponent<Battlefield>().UpdateField();
                    Battlefield.GetComponent<BattlefieldLogic>().Order();
                }
            else
            {
                Debug.Log("Else!");
                PlayerFormation.ClearField();
                Battlefield.GetComponent<Battlefield>().UpdateField();
                Battlefield.GetComponent<BattlefieldLogic>().Order();
            }
            }
        }
        else SelectedUnits.DeSelectEntity();
    }
    private void OnRestartGame(InputValue value)
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentIndex);
    }

    public GameObject InstantiateRandomUnit(Race unitRace,GameObject parent)
    {
        var newUnitCharacteristics = listOfCommonUnits.GetRandomUnit(unitRace);
        GameObject newUnit = Instantiate(Unit,parent.transform);
        ArmyUnitClass unitClass = newUnit.GetComponent<ArmyUnitClass>();
        unitClass.InitializeUnit(newUnitCharacteristics.Item1,newUnitCharacteristics.Item2);
        newUnit.name = $"{unitClass.UnitName}_{newUnit.GetInstanceID()}";
        newUnit.transform.position = Vector3.zero;
        return newUnit;
    }
    public void CreateRandomUnits(Hero unitOwner,int number, Race race)
    {
        //Checks
        if (number <= 0)
        {
            Debug.LogError("Number of new units must be greater than 0");
            return;
        }
        else if (number >= 100)
        {
            Debug.LogWarning("Are you sure you want THIS much units?");
        }
        
        for (int i = 0; i < number; i++)
        {
            GameObject newEnemyUnit = InstantiateRandomUnit(race,unitOwner.GameObject());
            unitOwner.AddBannerList(newEnemyUnit);
        }
    }
}
