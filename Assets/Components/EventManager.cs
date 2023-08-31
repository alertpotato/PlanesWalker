using UnityEngine;
using System;

public class EventManager : MonoBehaviour
{
    public static event Action<GameObject,bool> CardChosen;
    public static event Action<GameObject> CellChosen;
    public static event Action OpenDeck;
    //public static event Action<int> CardChosenToCell;
    //public static void StartCellChosen(GameObject cardChosen)
    public static void StartCardChosen(GameObject cardChosen, bool isInArmy)
    {
        CardChosen?.Invoke(cardChosen, isInArmy);
    }
    public static void StartCellChosen(GameObject cellChosen)
    {
        CellChosen?.Invoke(cellChosen);
    }
    public static void StartOpenDeck()
    {
        OpenDeck?.Invoke();
    }
}
