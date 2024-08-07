using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameLoopSharedData : MonoBehaviour
{
    [Header("Components")]
    public Camera MainCamera;
    public SelectManager SelectedUnits;
    public GameObject Battlefield;
    public GameObject ArmyDeck;
    public ListOfCommonUnits listOfCommonUnits;
    public GameObject PlayerHero;
    public GameObject EnemyHero;
    public StateMachine StateManager;
    public GameObject RewardParent;
    public FormationField PlayerFormation;
    public FormationField EnemyFormation;
    public PlayerData WorldData;
    public Scene MainGame;
    [Header("UI")]
    public TextMeshProUGUI HeadText;
    public TextMeshProUGUI BottomText;
    [Header("Variables")]
    public LayerMask UnitLayer;
    public LayerMask BattlefieldLayer;
    public int BattlesWon;
    [Header("Prefabs")]
    public GameObject Unit;
    public GameObject UnitCard;
    [Header("States")]
    public GameLoopPreBattleState PreBattleState;
    public GameLoopRewardState RewardState;
    private void Awake()
    {
        Battlefield.SetActive(false);
    }
    private void Start()
    {
        PlayerHero.GetComponent<Hero>().modifyHero("Planeswalker", 1, 1);
        EnemyHero.GetComponent<Hero>().modifyHero("Antagonist", 1, 1);
        
        //Init of Formation scriptable objects
        PlayerFormation.InitializeField(PlayerHero.GetComponent<Hero>());
        EnemyFormation.InitializeField(EnemyHero.GetComponent<Hero>());
        
        //Battlefield.GetComponent<Battlefield>().InicializeField(PlayerHero.GetComponent<Hero>(), EnemyHero.GetComponent<Hero>());
        ArmyDeck.GetComponent<ArmyDeck>().GetArmyHero(PlayerHero.GetComponent<Hero>());
        
        //Supply
        WorldData.Reset();
        WorldData.AddSupply(0,1);
        WorldData.AddSupply(1,1);
        WorldData.AddSupply(2,1);

        BattlesWon = 0;
    }
    //TODO .....
    public void TempRewards()
    {
        int newNumberOfRewards = 1;
        RewardState.NumberOfRewards = newNumberOfRewards;
    }

    void OnClick(InputValue value)
    {
        Ray ray = MainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, UnitLayer))
        {
            SelectedUnits.SelectEntity(hit.collider.gameObject);
        }
        if (StateManager.CurrentState.Equals(PreBattleState))
        {
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, BattlefieldLayer) & SelectedUnits.IsEntitySelected())
            {
                if (PlayerFormation.AddUnitToFormation(hit.collider.gameObject.GetComponent<ArmyCellScript>().GetCompanyBanner(),
                        SelectedUnits.SelectedEntity.GetComponent<UnitCardMain>().RelatedUnit,EnemyFormation))
                {
                    Battlefield.GetComponent<BattlefieldLogic>().Order();
                }
            }
        }
    }
    private void OnAlternateClick(InputValue value)
    {
        if (StateManager.CurrentState.Equals(PreBattleState))
        {
            Ray ray = MainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, BattlefieldLayer))
            {
                PlayerFormation.ClearField();
                Battlefield.GetComponent<BattlefieldLogic>().Order();
            }
        }
        else SelectedUnits.DeSelectEntity();
    }

    private void OnRestartGame(InputValue value)
    {
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentIndex);
    }

    public GameObject InstantiateRandomUnit(Race unitRace)
    {
        var newUnitCharacteristics = listOfCommonUnits.GetRandomUnit(unitRace);
        GameObject newUnit = Instantiate(Unit,RewardParent.transform);
        ArmyUnitClass unitClass = newUnit.GetComponent<ArmyUnitClass>();
        unitClass.InitializeUnit(newUnitCharacteristics.Item1,newUnitCharacteristics.Item2);
        newUnit.name = $"{unitClass.UnitName}_{newUnit.GetInstanceID()}";
        return newUnit;
    }
    public void CreateRandomUnits(Hero unitOwner,int number, Race race)
    {
        for (int i = 0; i < number; i++)
        {
            GameObject newEnemyUnit = InstantiateRandomUnit(race);
            unitOwner.AddBannerList(newEnemyUnit);
        }
    }

    public void UpdateHelpText(string headText, string bottomText)
    {
        HeadText.text = headText;
        BottomText.text = bottomText;
    }
}
