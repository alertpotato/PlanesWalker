using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmyCellScript : MonoBehaviour
{
    private GameObject backSprite; 
    private GameObject cellCollider;
    public ArmyFieldCellBackSpriteScript backSpriteRendererScript;
    [SerializeField] private int cellID;
    private SpriteRenderer backSpriteRenderer;

    private GameObject unitSprite;
    private SpriteRenderer unitSpriteRenderer;
    private void Awake()
    {
        cellID = -1;
        backSprite = Instantiate(Resources.Load<GameObject>("Prefab/ArmyFieldCellBackSprite"));
        backSprite.transform.SetParent(this.transform);
        backSpriteRenderer = backSprite.GetComponent<SpriteRenderer>();
        backSpriteRendererScript = backSprite.GetComponent<ArmyFieldCellBackSpriteScript>();
        cellCollider = Instantiate(Resources.Load<GameObject>("Prefab/ArmyFieldCellCollider"));
        cellCollider.GetComponent<ArmyFieldCellColliderScript>().GetParent(this.gameObject);
        cellCollider.transform.SetParent(this.transform);
    }
    private void Start()
    {

    }
    private void Update()
    {

    }
    public void SetId(int id)
    {
        if (cellID == -1)
        { cellID = id; cellCollider.GetComponent<ArmyFieldCellColliderScript>().SetId(cellID); }
        
    }
    public void ChangeSprite(Sprite newSprite)
    {
        backSpriteRenderer.sprite = newSprite;
    }
}
