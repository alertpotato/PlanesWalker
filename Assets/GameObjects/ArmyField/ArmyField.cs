using System.Collections.Generic;
using UnityEngine;
using Planeswalker.Scripts;

namespace Planeswalker.GameObjects.ArmyField
{
    public class ArmyField : MonoBehaviour
    {
        private ListOfObjects listOfCommonObjects;
        [SerializeField] private List<Sprite> spriteUiList;
        [SerializeField] public List<List<GameObject>> cellList;

        private void Awake()
        {
            listOfCommonObjects = ScriptableObject.CreateInstance<ListOfObjects>();
            cellList = new List<List<GameObject>>();
        }

        private void Start()
        {
            spriteUiList = new List<Sprite>(Resources.LoadAll<Sprite>("Sprites/ui"));
            transform.position = new Vector3(0, 0, -2);
            Debug.Log("start ArmyField");
            InicializeField();
        }

        private void Update()
        {

        }

        private void InicializeField()
        {
            int cellId = 0;
            Debug.Log("init ArmyField");
            for (int i = 0; i < 5; i++)
            {
                var _tempList = new List<GameObject>();
                for (int _i = 0; _i < 5; _i++)
                {
                    GameObject _cell = Instantiate(Resources.Load<GameObject>("Prefab/ArmyFieldCell"));
                    _cell.transform.SetParent(this.transform);
                    _cell.GetComponent<ArmyCellScript>().SetId(cellId);
                    _tempList.Add(_cell);
                    cellId++;
                }
                cellList.Add(_tempList);
            }
        }

        public void PositionField(Hero _hero)
        {
            Debug.Log("pos ArmyField");
            //float step = listOfCommonObjects.GetSpriteByName("armyCellE", spriteUiList).rect.width * 3f;
            float step = spriteUiList[0].rect.width * 3.5f;
            float stepFromLeft = Screen.width / 2;
            float stepFromTop = Screen.height * 0.9f;
            int xcell = -1; int ycell = -1;
            foreach (List<ArmyCell> _armyCelllist in _hero.ArmyFormation)
            {
                xcell++; ycell = -1;
                foreach (ArmyCell _armyCell in _armyCelllist)
                {
                    ycell++;
                    cellList[xcell][ycell].transform.position = Camera.main.ScreenToWorldPoint(new Vector3(stepFromLeft - xcell * step, stepFromTop - ycell * step, 8));
                    if (_armyCell.type == cellType.Forbidden)
                    {
                        //cellList[xcell][ycell].GetComponent<ArmyCellScript>().ChangeSprite(listOfCommonObjects.GetSpriteByName("armyCellF", spriteUiList));
                        cellList[xcell][ycell].GetComponent<ArmyCellScript>().ChangeSprite(spriteUiList[1]);
                    }
                    else if (_armyCell.type == cellType.Empty)
                    {
                        //cellList[xcell][ycell].GetComponent<ArmyCellScript>().ChangeSprite(listOfCommonObjects.GetSpriteByName("armyCellE", spriteUiList));
                        cellList[xcell][ycell].GetComponent<ArmyCellScript>().ChangeSprite(spriteUiList[0]);
                    }
                    Debug.Log($"{xcell} {ycell} {_armyCell.type}");
                }
            }
        }
    }
}
