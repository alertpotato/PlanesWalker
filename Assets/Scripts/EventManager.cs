using UnityEngine;
using System;

namespace Planeswalker
{
    public class EventManager : MonoBehaviour
    {
        public static event Action<GameObject> CardChosen;
        public static event Action<GameObject> CellChosen;
        //public static event Action<int> CardChosenToCell;
        //public static void StartCellChosen(GameObject cardChosen)
        public static void StartCardChosen(GameObject cardChosen)
        {
            CardChosen?.Invoke(cardChosen);
        }
        public static void StartCellChosen(GameObject cellChosen)
        {
            CellChosen?.Invoke(cellChosen);
        }
    }
}
