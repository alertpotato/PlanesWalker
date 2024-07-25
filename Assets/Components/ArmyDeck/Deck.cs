using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    public Camera MainCamera;
    public Hero Hero;
    public List<GameObject> cards;
    public GameObject yourDeckSpace;
    public GameObject UnitCard;
    
    public float scaleMulti;
    public int MaxCardsInRow;
    
    public void InitializeDeck(Hero hero,Camera mainCamera,GameObject CardPrefab)
    {
        Hero = hero;
        MainCamera = mainCamera;
        UnitCard = CardPrefab;
        //Position deck space in upper left corner 5 units infront of camera
        yourDeckSpace.transform.localPosition = MainCamera.ScreenToWorldPoint(new Vector3((Screen.width *0.05f), Screen.height*0.8f, 5));
    }
    private void Awake()
    {
        cards = new List<GameObject>();
        MaxCardsInRow = 4;
        scaleMulti = 0.7f;
        yourDeckSpace.SetActive(false);
    }
    public void RebuildDeck()
    {
        WipeCards();
        CreateCards();
        if (cards.Count>0) SortCards();
    }
    public void SortCards()
    {
        var cardListSorted = CreateSortList();
        if (cardListSorted.Count==0)
        {
            Debug.LogWarning("No cards in list!");return;
        }
        float stepX = -1.5f*scaleMulti; float stepY = 0;
        string prevCardName = cardListSorted[0].GetComponent<UnitCardMain>().RelatedUnit.GetComponent<ArmyUnitClass>().UnitName;
        int maxCardsCount = 0;
        foreach (var card in cardListSorted)
        {
            var unit = card.GetComponent<UnitCardMain>().RelatedUnit.GetComponent<ArmyUnitClass>();
            if (unit.UnitName != prevCardName) { stepY -= 2.1f*scaleMulti; stepX = 0; maxCardsCount = 0;}
            else { stepX += 1.5f*scaleMulti;}
            if (maxCardsCount==MaxCardsInRow) { stepY -= 2.1f*scaleMulti; stepX = 0;
                maxCardsCount = 0; 
            }
            var newPos = new Vector3( stepX, stepY, 0 );
            card.GetComponent<UnitCardMain>().SetNewPosition(newPos,Vector3.one*scaleMulti);
            prevCardName = unit.UnitName;
            maxCardsCount++;
        }
    }
    private List<GameObject> CreateSortList()
    {
        var sortList = cards;
        sortList = sortList.OrderBy(x => x.GetComponent<UnitCardMain>().RelatedUnit.GetComponent<ArmyUnitClass>().UnitName).ToList();
        return sortList;
    }
    private void CreateCards()
    {
        foreach (var unit in Hero.bannersList)
        {
            var card = Instantiate(UnitCard,yourDeckSpace.transform);
            card.GetComponent<UnitCardMain>().SetUnitParameters(MainCamera, unit, Vector3.zero, Vector3.one,false);
            cards.Add(card);
        }
        yourDeckSpace.SetActive(true);
    }
    public void WipeCards()
    {
        foreach (GameObject card in cards)
        {
            Destroy(card);
        }
        cards.Clear();
        yourDeckSpace.SetActive(false);
    }
}