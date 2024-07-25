using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
[RequireComponent(typeof(UnitCardUI))]
public class UnitCardMain : MonoBehaviour
{
    [Header("Components")]
    public Camera MainCamera;
    public UnitGraphic UnitSprites;
    public UnitCardUI CardUI;
    public BoxCollider cardCollider;
    public SpriteRenderer cardSprite;
    public GameObject RelatedUnit;

    [Header("Private variables")]
    public bool IsSelected = false;
    [SerializeField] private Vector2 cardSpriteSize;
    [SerializeField] private float _cardMoveMultiplier = 3;
    [SerializeField] private float cardRotateMultiplier = 10;
    [SerializeField] private Vector3 startCardPos;
    [SerializeField] private Vector3 StartCardScale;
    [SerializeField] private Color _newCardColor = new Color(1, 1, 1, 1);
    [SerializeField] private Vector3 _cardMoveDirection = new Vector3(0, 0, -0.1f);
    [SerializeField] private bool isMouseOff = true;

    private void OnValidate()
    {
        CardUI = transform.GetComponent<UnitCardUI>();
    }

    public void SetNewPosition(Vector3 newPos, Vector3 newScale)
    {
        transform.localPosition = newPos;
        transform.localScale = newScale;
        startCardPos = newPos;
        StartCardScale = newScale;
    }

    public void SetUnitParameters(Camera camera,GameObject unit,Vector3 pos,Vector3 startCardScale, bool showMore)
    {
        MainCamera = camera;
        StartCardScale = startCardScale;
        RelatedUnit = unit;
        ArmyUnitClass RelatedUnitClass = RelatedUnit.GetComponent<ArmyUnitClass>();
        CardUI.InitializeUI(RelatedUnitClass);
        SetSpriteByName(RelatedUnitClass.UnitName);
        cardSpriteSize = cardSprite.size;
        cardCollider.size = new Vector3(cardSprite.size.x, cardSprite.size.y, 0.1f);
        transform.position = pos;
        startCardPos = pos;
        
        CardUI.UpdateAllUI();
        if (showMore) CardUI.CreateAbilityUI();
    }

    private void Update()
    {
        float detlaTime = Time.deltaTime * 30;
        if (!isMouseOff)
        {
            //float _approach = 0.1f * (MainCamera.transform.position.z - startCardPos.z);
            Ray ray = MainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Vector3 startMousePos;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            { startMousePos = hit.point;}
            else return;
            float relativeXToMouse = -(transform.position.x - startMousePos.x) / (cardSpriteSize.x / 2);
            float relativeYToMouse = -(transform.position.y - startMousePos.y) / (cardSpriteSize.y / 2);
            float relativeToMouse = (Mathf.Abs(relativeXToMouse) + Mathf.Abs(relativeYToMouse)) / 2; 
            Vector3 asd = new Vector3(15 * relativeXToMouse, 15 * relativeYToMouse, 0);
            Vector3 newPosition = transform.position;

            //      if (IsSelected) newPosition = new Vector3(startCardPos.x, startCardPos.y, startCardPos.z - 1);
            
            transform.SetPositionAndRotation(newPosition, Quaternion.FromToRotation(newPosition,new Vector3(startMousePos.x, startMousePos.y, transform.position.z)));
            //if (transform.position.z > transform.position.z+_approach)
            //{
            //    transform.position = transform.position + _cardMoveDirection * _detlaTime * _cardMoveMultiplier;
            //    if (transform.position.z < transform.position.z + _approach) { transform.position = new Vector3(transform.position.x, transform.position.y,-3);
            //        if (cardCollider.GetComponent<CardCollider2D>().GetTrueMousePosition()) { isMouseOff = true; }
            //    };
            //    //_sizeMult = Mathf.Pow(1.1f, Mathf.Abs(1 - transform.position.z));
            //    //cardCollider.GetComponent<BoxCollider>().size = _spriteRenderer.size * _sizeMult;
            //}
        }
        else
        {
            if (transform.rotation.eulerAngles.x != 0 || transform.rotation.eulerAngles.y != 0 || transform.rotation.eulerAngles.z != 0)
            {
                transform.Rotate(new Vector3(-transform.localRotation.x * detlaTime * cardRotateMultiplier, -transform.localRotation.y * detlaTime * cardRotateMultiplier, -transform.localRotation.z * detlaTime * cardRotateMultiplier * 2));
                //if (transform.position.z < -2)
                //{
                //    transform.position = transform.position - _cardMoveDirection * _detlaTime * _cardMoveMultiplier;
                //    //_sizeMult = Mathf.Pow(1.1f, Mathf.Abs(1 - transform.position.z));
                //    //cardCollider.GetComponent<BoxCollider>().size = _spriteRenderer.size * _sizeMult;
                //    if (transform.position.z > -2) { transform.position = new Vector3(transform.position.x, transform.position.y, -2);};
                //}
            }
        }
    }
    public void OnMouseEnterCollider()
    {
        isMouseOff = false;
        cardSprite.color = _newCardColor;
        //cardSprite.size *= 1.5f;
        //cardSprite.transform.position = new Vector3(_startCardPos.x, _startCardPos.y, _startCardPos.z ); //+ _approach
        //cardCollider.size = new Vector3(cardSprite.size.x, cardSprite.size.y, 0.1f);
        //_cardText.GetComponent<UnitCardText>().UnHideText();
    }
    public void OnMouseExitCollider()
    {
        isMouseOff = true;
        //cardSprite.size = _startCardSize;
        //transform.position = _startCardPos;
        //cardCollider.size = new Vector3(cardSprite.size.x, cardSprite.size.y, 0.1f);
        //_cardText.GetComponent<UnitCardText>().HideText();
    }
    //--------------Collider
    public RaycastHit GetRaycastHit()
    {
        Physics.Raycast(MainCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit raycastHit);
        return raycastHit;
    }
    private void OnMouseExit()
    {
        isMouseOff = true;
        OnMouseExitCollider();
    }
    private void OnMouseEnter()
    {
        isMouseOff = false;
        OnMouseEnterCollider();
    }
    //--------------SPRITE
    public void SetSpriteByName(string spriteName)
    {
        cardSprite.sprite = UnitSprites.GetCardSpriteByName(spriteName);
    }
    //=--= Select adapter ivents
    public void selectCard()
    {
        IsSelected = true;
        transform.localPosition = new Vector3(startCardPos.x, startCardPos.y, startCardPos.z * 0.96f);
        transform.localScale = StartCardScale*1.2f;
        //UIElements.SetActive(true);
        //UIOutline.SetActive(true);
    }
    public void deSelectCard()
    {
        IsSelected = false;
        transform.localPosition = startCardPos;
        transform.localScale = StartCardScale;
        //UIElements.SetActive(false);
        //UIOutline.SetActive(false);
    }
}