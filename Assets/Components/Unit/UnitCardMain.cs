using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UnitCardMain : MonoBehaviour
{
    //public float _sizeMult;
    public GameObject RelatedUnit;
    public BoxCollider _cardCollider;
    public SpriteRenderer _cardSprite;
    public GameObject _cardText;
    public UnitGraphic UnitSprites;
    public Camera MainCamera;
    [SerializeField] private Vector2 _cardSpriteSize;
    [SerializeField] private float _cardMoveMultiplier;
    [SerializeField] private float _cardRotateMultiplier;
    float _approach;
    [SerializeField] private Vector3 _startCardPos;
    [SerializeField] private Vector2 _startCardSize;
    [SerializeField] private Color _startCardColor = new Color(0.529f, 0.529f, 0.529f, 255.000f);
    [SerializeField] private Color _newCardColor = new Color(1, 1, 1, 1);
    [SerializeField] private Vector3 _cardMoveDirection = new Vector3(0, 0, -0.1f);
    [SerializeField] private bool isMouseOff = true;
    public bool IsSelected = false;
    public SelectManager SelectingManager;
    public GameObject UIElements;
    public UnitCardStatPanel StatText;

    private bool isUnitInArmy;

    private void Awake()
    {
        _cardText = Instantiate(_cardText);
        _cardText.transform.SetParent(transform);
        _cardText.transform.position = this.transform.position;

        _cardSprite.color = _startCardColor;
    }
    private void Start()
    {
        _startCardSize = _cardSprite.size*2;
        _startCardPos = transform.position;
        _cardMoveMultiplier = 3;
        _cardRotateMultiplier = 10;
        _cardText.transform.localPosition = new Vector3(_cardSpriteSize.x / 2,0,-0.05f);
        _approach = 0.1f * (MainCamera.transform.position.z - _startCardPos.z);
    }
    private void Update()
    {
        float _detlaTime = Time.deltaTime * 30;
        if (!isMouseOff)
        {
            float _approach = 0.1f * (MainCamera.transform.position.z - _startCardPos.z);
            Vector3 startMousePos = GetRaycastHit().point;
            float relativeXToMouse = (transform.position.x - startMousePos.x) / (_cardSpriteSize.x / 2);
            float relativeYToMouse = (transform.position.y - startMousePos.y) / (_cardSpriteSize.y / 2);
            float relativeToMouse = (Mathf.Abs(relativeXToMouse) + Mathf.Abs(relativeYToMouse)) / 2; 
            Vector3 asd = new Vector3(15 * relativeXToMouse, 15 * relativeYToMouse, 0);
            transform.SetPositionAndRotation(transform.position, Quaternion.FromToRotation(transform.position,new Vector3(startMousePos.x, startMousePos.y, transform.position.z)));
            //if (transform.position.z > transform.position.z+_approach)
            //{
            //    transform.position = transform.position + _cardMoveDirection * _detlaTime * _cardMoveMultiplier;
            //    if (transform.position.z < transform.position.z + _approach) { transform.position = new Vector3(transform.position.x, transform.position.y,-3);
            //        if (_cardCollider.GetComponent<CardCollider2D>().GetTrueMousePosition()) { isMouseOff = true; }
            //    };
            //    //_sizeMult = Mathf.Pow(1.1f, Mathf.Abs(1 - transform.position.z));
            //    //_cardCollider.GetComponent<BoxCollider>().size = _spriteRenderer.size * _sizeMult;
            //}
        }
        else
        {
            if (transform.localRotation.x != 0 && transform.localRotation.y != 0 && transform.localRotation.z != 0)
            {
                transform.Rotate(new Vector3(-transform.localRotation.x * _detlaTime * _cardRotateMultiplier, -transform.localRotation.y * _detlaTime * _cardRotateMultiplier, -transform.localRotation.z * _detlaTime * _cardRotateMultiplier * 2));
                //if (transform.position.z < -2)
                //{
                //    transform.position = transform.position - _cardMoveDirection * _detlaTime * _cardMoveMultiplier;
                //    //_sizeMult = Mathf.Pow(1.1f, Mathf.Abs(1 - transform.position.z));
                //    //_cardCollider.GetComponent<BoxCollider>().size = _spriteRenderer.size * _sizeMult;
                //    if (transform.position.z > -2) { transform.position = new Vector3(transform.position.x, transform.position.y, -2);};
                //}
            }
        }
    }
    public void SetUnitParameters(GameObject unit,Vector3 _pos, bool isInArmy)
    {
        RelatedUnit = unit;
        ArmyUnitClass RelatedUnitClass = RelatedUnit.GetComponent<ArmyUnitClass>();
        SetSpriteByName(RelatedUnitClass.UnitName);
        _cardSpriteSize = _cardSprite.size;
        _cardCollider.size = new Vector3(_cardSprite.size.x, _cardSprite.size.y, 0.1f);
        //_cardText.GetComponent<UnitCardText>().ChangeText(RelatedUnitClass.DefaultUnitCharacteristics.Characteristics,RelatedUnitClass.CurrentUnitCharacteristics, RelatedUnitClass.unitUpgrades);
        transform.position = _pos;
        transform.localPosition = _pos;
        isUnitInArmy = isInArmy;
        StatText.SetStatText(RelatedUnitClass.DefaultUnitCharacteristics.Characteristics,RelatedUnitClass.CurrentUnitCharacteristics, RelatedUnitClass.unitUpgrades);
    }
    public void OnMouseEnterCollider()
    {
        isMouseOff = false;
        _cardSprite.color = _newCardColor;
        _cardSprite.size *= 1.5f;
        _cardSprite.transform.position = new Vector3(_startCardPos.x, _startCardPos.y, _startCardPos.z + _approach);
        _cardCollider.size = new Vector3(_cardSprite.size.x, _cardSprite.size.y, 0.1f);
        //_cardText.GetComponent<UnitCardText>().UnHideText();
    }
    public void OnMouseExitCollider()
    {
        isMouseOff = true;
        _cardSprite.color = _startCardColor;
        _cardSprite.size = _startCardSize;
        transform.position = _startCardPos;
        _cardCollider.size = new Vector3(_cardSprite.size.x, _cardSprite.size.y, 0.1f);
        //_cardText.GetComponent<UnitCardText>().HideText();
    }
    public GameObject GetCardUnit()
    { return RelatedUnit; }
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
        _cardSprite.sprite = UnitSprites.GetCardSpriteByName(spriteName);
    }
    public void selectCard()
    {
        IsSelected = true;
        UIElements.SetActive(true);
    }
    public void deSelectCard()
    {
        IsSelected = false;
        //UIElements.SetActive(false);
    }
}