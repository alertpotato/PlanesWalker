using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

[RequireComponent(typeof(StateBehaviour))]
[RequireComponent(typeof(GameLoopRewardState))]
[RequireComponent(typeof(GameLoopPreBattleState))]
[RequireComponent(typeof(GameLoopRoundState))]
[RequireComponent(typeof(GameLoopDecisionState))]
public class GameLoopSharedData : MonoBehaviour
{
    [Header("Components")]
    public Camera MainCamera;
    public SelectManager SelectedUnits;
    public GameObject Battlefield;
    public GameObject DeckManager;
    public StateMachine StateManager;
    public GameObject RewardParent;
    public RewardFactory RewardFactory;
    public EventFactory EventFactory;
    [FormerlySerializedAs("Difficulty")] public DifficultyManager difficultyManager;
    public StateMachine GameLoopState;
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
    public int Day = 1;
    [Header("Prefabs")]
    public GameObject Unit;
    public GameObject UnitCard;
    [Header("States")]
    public GameLoopPreBattleState PreBattleState;
    public GameLoopRewardState RewardState;
    public GameLoopRoundState RoundState;
    public GameLoopDecisionState DecisionState;
    [Header("StartingParams")]
    public Dictionary<FormationType, int> startingField = new Dictionary<FormationType, int> { {FormationType.Frontline,3}, {FormationType.Support,1}, {FormationType.Flank1,1}};
    
    private void OnValidate()
    {
        PreBattleState = transform.GetComponent<GameLoopPreBattleState>();
        RewardState = transform.GetComponent<GameLoopRewardState>();
        RoundState = transform.GetComponent<GameLoopRoundState>();
        DecisionState = transform.GetComponent<GameLoopDecisionState>();
        PreBattleState.Config = this;
        RewardState.Config = this;
        RoundState.Config = this;
        DecisionState.Config = this;
        Battlefield.GetComponent<Battlefield>().Initialize(MainCamera,PlayerFormation,EnemyFormation);
    }

    private void Start()
    {
        //INIT FACTORIES 
        listOfCommonUnits.InizializeUnitFactory();
        EventFactory.InizializeEventFactory();
        RewardFactory.InizializeRewardFactory();
        //Init hero
        PlayerHero.GetComponent<Hero>().modifyHero("Planeswalker", 0, 0);
        EnemyHero.GetComponent<Hero>().modifyHero("Antagonist", 0, 0);
        
        //Init of Formation scriptable objects
        PlayerFormation.InitializeField(PlayerHero.GetComponent<Hero>());
        EnemyFormation.InitializeField(EnemyHero.GetComponent<Hero>());
        // Init field
        PlayerFormation.RebuildField(startingField);
        EnemyFormation.RebuildField(startingField);
        //-----------????
        DeckManager.GetComponent<Deck>().InitializeDeck(PlayerHero.GetComponent<Hero>(),MainCamera,UnitCard);
        Battlefield.GetComponent<Battlefield>().Initialize(MainCamera,PlayerFormation,EnemyFormation);
        
        //Supply
        WorldData.Reset();
        WorldData.AddSupply(0,1);
        WorldData.AddSupply(1,1);
        WorldData.AddSupply(2,1);
        WorldData.AddEnemySupply(0,1);
        WorldData.AddEnemySupply(1,1);
        WorldData.AddEnemySupply(2,1);
        InterfaceUI.UpdateSupply(WorldData.PlayerSupply);
        Day = 0;
        //Getting 3 new card at start
        List<(int, int)> rewards = new List<(int, int)>{(1,0),(1,0),(1,0),(99,0)};
        var StartingDeck = (rewards,RewardFactory.rewardsWeightsAndAttributes);
        RewardState.Rewards=StartingDeck;
        //Init Difficulty
        difficultyManager.InitializeDifficulty(0,WorldData,RewardFactory,EventFactory,listOfCommonUnits);
        InterfaceUI.UpdateHintText(Day,WorldData.EnemySupply,Battlefield.GetComponent<BattlefieldLogic>().pauseBetweenAbilities);
    }
    //TODO .....

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
                    unitPreview = CreateCard(mousedOverUnit,0.7f,0.65f,3);
                }
            }
        else if (mousedOverUnit) {mousedOverUnit = null; Destroy(unitPreview);}
    }

    public GameObject CreateCard(GameObject unit,float x, float y,float z)
    {
        var pos = MainCamera.ScreenToWorldPoint(new Vector3((Screen.width * x), Screen.height*y, z)); //z = 5 bc its distance between cards and camera
        GameObject newCard = Instantiate(UnitCard,this.gameObject.transform);
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
        if (StateManager.CurrentState.Equals(PreBattleState))
        {
            Ray ray = MainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit,Mathf.Infinity))
            {
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

    private void OnGameSpeedPlus(InputValue value)
    {
        Battlefield.GetComponent<BattlefieldLogic>().pauseBetweenAbilities =
            Mathf.Clamp(Battlefield.GetComponent<BattlefieldLogic>().pauseBetweenAbilities + 0.25f, 0.25f, 10f);
        InterfaceUI.UpdateHintText(Day,WorldData.EnemySupply,Battlefield.GetComponent<BattlefieldLogic>().pauseBetweenAbilities);
    }
    private void OnGameSpeedMinus(InputValue value)
    {
        Battlefield.GetComponent<BattlefieldLogic>().pauseBetweenAbilities =
            Mathf.Clamp(Battlefield.GetComponent<BattlefieldLogic>().pauseBetweenAbilities - 0.25f, 0.25f, 10f);
        InterfaceUI.UpdateHintText(Day,WorldData.EnemySupply,Battlefield.GetComponent<BattlefieldLogic>().pauseBetweenAbilities);
    }

    public void EndOfLoopEvents()
    {
        Day += 1;
        //Apply changed rewards weights
        if (RewardState.Rewards.Item1 != null) RewardFactory.rewardsWeightsAndAttributes= RewardState.Rewards.Item2;
        //End of day reward weights regen
        RewardFactory.RegenerateRewards(RewardFactory.rewardsWeightsAndAttributes);
        //Difficulty script
        difficultyManager.UpdateDifficulty(Day);
        //Start new loop from new Decision
        GameLoopState.ChangeState<GameLoopDecisionState>();
        //Temp hint update
        InterfaceUI.UpdateHintText(Day,WorldData.EnemySupply,Battlefield.GetComponent<BattlefieldLogic>().pauseBetweenAbilities);
    }

    public GameObject InstantiateRandomUnit(List<Race> unitRace,GameObject parent)
    {
        var newUnitCharacteristics = listOfCommonUnits.GetRandomUnit(unitRace);
        GameObject newUnit = Instantiate(Unit,parent.transform);
        ArmyUnitClass unitClass = newUnit.GetComponent<ArmyUnitClass>();
        Debug.Log($"---------------UNITS: {unitClass} {newUnitCharacteristics.Item1} {newUnitCharacteristics.Item2}");
        unitClass.InitializeUnit(newUnitCharacteristics.Item1,newUnitCharacteristics.Item2);
        Debug.Log($"-----------sssssss----UNITS: {unitClass} {newUnitCharacteristics.Item1} {newUnitCharacteristics.Item2}");
        newUnit.name = $"{unitClass.UnitName}_{newUnit.GetInstanceID()}";
        newUnit.transform.position = Vector3.zero;
        return newUnit;
    }
}
