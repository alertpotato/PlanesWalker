using UnityEngine;

namespace Planeswalker.GameObjects.ArmyField
{
    public class ArmyFieldCellColliderScript : MonoBehaviour
    {
        private GameObject cellParent;
        private int cellID;
        void Awake()
        {
            cellID = -1;
        }

        // Update is called once per frame
        void Update()
        {

        }
        public void GetParent(GameObject _parent)
        {
            if (cellParent == null) { cellParent = _parent; };
        }

        public void SetId(int id)
        {
            if (cellID == -1)
            { cellID = id; }
        }
        private void OnMouseUp()
        {
            Debug.Log("Mouse up on cell");
            EventManager.StartCellChosen(cellParent);
        }
    }
}
