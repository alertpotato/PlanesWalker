using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

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
    private GameObject mousedOverObject;
    private GameObject unitPreview;
    [Header("UI")] 
    public SceneInterfaceController InterfaceUI;
    public HintManager HintManager;
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
    public EventSystem eventSystem;
    public GraphicRaycaster graphicRaycaster;
    
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
        //Test UI
        eventSystem = FindObjectOfType<EventSystem>();
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
        //TEST TEST TEST
        // Check if the left mouse button is pressed
        if (Input.GetMouseButtonDown(0))
        {
            // Create PointerEventData
            PointerEventData eventData = new PointerEventData(eventSystem);
            
            // Set the position to the mouse position
            eventData.position = Input.mousePosition;

            // Create a list to store the results
            List<RaycastResult> results = new List<RaycastResult>();

            // Perform the raycast
            graphicRaycaster.Raycast(eventData, results);

            // Check if anything was hit
            if (results.Count > 0)
            {
                // Print the name of the hit UI element
                Debug.Log($"Hit UI element: {results[0].gameObject.name}");
                
                // You can also get the RectTransform of the hit element
                // RectTransform hitRectTransform = results[0].GetComponent<RectTransform>();
            }
        }
        //----------------------------------
        
        var mousePos = Input.mousePosition;
        Ray ray = MainCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
        }
        else
        {
            if (mousedOverObject) {mousedOverObject = null; Destroy(unitPreview);HintManager.HideHint();Battlefield.GetComponent<Battlefield>().DeHighlightUnitUI();}
            return;
        }
        //Unit preview logic
        if (hit.collider.gameObject.GetComponent<OnFieldCompanyManager>())
            {
                GameObject newMousedOverUnit = hit.collider.gameObject.GetComponent<OnFieldCompanyManager>().Company.Unit;
                if (newMousedOverUnit == null) return;
                mousePos.z = hit.collider.transform.position.z;
                Vector3 worldPos = MainCamera.ScreenToWorldPoint(mousePos);
                float xAdjust = -5f;
                if (mousePos.x < Screen.width / 2) xAdjust = 5;
                Vector3 cardPos = new Vector3(worldPos.x+xAdjust, worldPos.y/3, hit.collider.transform.position.z);
                if (mousedOverObject != newMousedOverUnit & newMousedOverUnit)
                {
                    Destroy(unitPreview);
                    mousedOverObject = newMousedOverUnit;
                    unitPreview = CreateCard(mousedOverObject,Vector3.zero);
                    Battlefield.GetComponent<Battlefield>().HighlightUnitUI(newMousedOverUnit);
                }
                unitPreview.GetComponent<UnitCardMain>().SetNewPosition(cardPos,new Vector3(3,3,3));
            }
        //Hint logic
        if (hit.collider.gameObject.GetComponent<FieldCompanyAbilityButtonManager>())
        {
            GameObject newMousedOverAbilityButton = hit.collider.gameObject;
            if (newMousedOverAbilityButton == null) return;
            mousePos.x += 50;
            if (mousedOverObject != newMousedOverAbilityButton & newMousedOverAbilityButton)
            {
                mousedOverObject = newMousedOverAbilityButton;
                int index = newMousedOverAbilityButton.GetComponent<FieldCompanyAbilityButtonManager>().CurrentIndex;
                string hintText = newMousedOverAbilityButton.GetComponent<FieldCompanyAbilityButtonManager>()
                    .FieldCompany.GetComponent<OnFieldCompanyManager>().Company.Unit.GetComponent<ArmyUnitClass>()
                    .Abilities[index].AbilityDescription;
                HintManager.ShowHint(hintText,mousePos);
            }
        }
    }

    public GameObject CreateCard(GameObject unit,Vector3 newPos)
    {
        GameObject newCard = Instantiate(UnitCard,this.gameObject.transform);
        newCard.name = $"{unit.name}_CardInfo";
        newCard.transform.SetParent(unit.transform);
        newCard.GetComponent<UnitCardMain>().SetUnitParameters(MainCamera,unit, newPos,Vector3.one,true,61);
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
            if (SelectedUnits.SelectEntity(hit.collider.gameObject))
            {
                Battlefield.GetComponent<Battlefield>().HighlightUnitUI(hit.collider.gameObject.GetComponent<UnitCardMain>().RelatedUnit);   
            }
            else Battlefield.GetComponent<Battlefield>().DeHighlightUnitUI();
        }
        if (StateManager.CurrentState.Equals(PreBattleState))
        {
            if (SelectedUnits.IsEntitySelected())
            {
                GameObject selectedUnit = SelectedUnits.SelectedEntity.GetComponent<UnitCardMain>().RelatedUnit;
                hit.collider.gameObject.GetComponent<SelectAdapter>()?.SelectField.Invoke(selectedUnit,PlayerHero);
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
                hit.collider.gameObject.GetComponent<SelectAdapter>()?.DeselectField.Invoke(PlayerHero);
            }
        }
        else
        {
            GameObject selectedUnit = SelectedUnits.SelectedEntity.GetComponent<UnitCardMain>().RelatedUnit;
            Battlefield.GetComponent<Battlefield>().DeHighlightUnitUI();
            SelectedUnits.DeSelectEntity();
        }

        
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

    private void OnAction(InputValue value)
    {
        if (StateManager.CurrentState.Equals(RoundState))
        {
            RoundState.RoundButtonAction();
        }
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
        unitClass.InitializeUnit(newUnitCharacteristics.Item1,newUnitCharacteristics.Item2);
        newUnit.name = $"{unitClass.UnitName}_{newUnit.GetInstanceID()}";
        newUnit.transform.position = Vector3.zero;
        return newUnit;
    }
}
