using UnityEngine;

public class UnitCardMain : MonoBehaviour
{
    private GameObject _cardCollider;
    private UnitCardCollider _cardColliderScript;
    private GameObject _cardSprite;
    private GameObject _cardText;
    private SpriteRenderer _spriteRenderer;
    private Camera _mainCamera;
    private ArmyUnitClass thisUnit;
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

    private bool isUnitInArmy;
    //public float _sizeMult;

    private void Awake()
    {
        _cardCollider = Instantiate(Resources.Load<GameObject>("Prefab/UnitCardCollider"));
        _cardCollider.transform.SetParent(this.transform);
        _cardColliderScript = _cardCollider.GetComponent<UnitCardCollider>();
        _cardSprite = Instantiate(Resources.Load<GameObject>("Prefab/UnitCardSprite"));
        _cardSprite.transform.SetParent(this.transform);
        _cardText = Instantiate(Resources.Load<GameObject>("Prefab/UnitCardText"));
        _cardText.transform.SetParent(_cardSprite.transform);
        _spriteRenderer = _cardSprite.GetComponent<SpriteRenderer>();
        _mainCamera = Camera.main;
        _cardCollider.transform.position = this.transform.position;
        _spriteRenderer.transform.position = this.transform.position;
        _cardText.transform.position = this.transform.position;
    }
    private void Start()
    {
        _cardCollider.GetComponent<UnitCardCollider>().GetParent(this);
        _spriteRenderer.color = _startCardColor;
        _startCardSize = _spriteRenderer.size;

        _startCardPos = _spriteRenderer.transform.position;
        _cardMoveMultiplier = 3;
        _cardRotateMultiplier = 10;
        _cardText.transform.localPosition = new Vector3(_cardSpriteSize.x / 2,0,-0.05f);

        _approach = 0.1f * (_mainCamera.transform.position.z - _startCardPos.z);
    }
    private void Update()
    {
        float _detlaTime = Time.deltaTime * 30;
        if (!isMouseOff)
        {
            float _approach = 0.1f * (_mainCamera.transform.position.z - _startCardPos.z);
            Vector3 startMousePos = _cardColliderScript.GetRaycastHit().point;
            float relativeXToMouse = (transform.position.x - startMousePos.x) / (_cardSpriteSize.x / 2);
            float relativeYToMouse = (transform.position.y - startMousePos.y) / (_cardSpriteSize.y / 2);
            float relativeToMouse = (Mathf.Abs(relativeXToMouse) + Mathf.Abs(relativeYToMouse)) / 2;
            Vector3 asd = new Vector3(15 * relativeXToMouse, 15 * relativeYToMouse, 0);
            _spriteRenderer.transform.SetPositionAndRotation(_spriteRenderer.transform.position, Quaternion.FromToRotation(_spriteRenderer.transform.position,new Vector3(startMousePos.x, startMousePos.y, _spriteRenderer.transform.position.z)));
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
            if (_spriteRenderer.transform.localRotation.x != 0 && _spriteRenderer.transform.localRotation.y != 0 && _spriteRenderer.transform.localRotation.z != 0)
            {
                _spriteRenderer.transform.Rotate(new Vector3(-_spriteRenderer.transform.localRotation.x * _detlaTime * _cardRotateMultiplier, -_spriteRenderer.transform.localRotation.y * _detlaTime * _cardRotateMultiplier, -_spriteRenderer.transform.localRotation.z * _detlaTime * _cardRotateMultiplier * 2));
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
    public void SetUnitParameters(string unitname, ArmyUnitClass unit, Vector3 _pos, bool isInArmy)
    {
        _cardSprite.GetComponent<UnitCardSprite>().SetSpriteByName(unitname);
        _cardSpriteSize = _spriteRenderer.size;
        _cardCollider.GetComponent<BoxCollider>().size = new Vector3(_spriteRenderer.size.x, _spriteRenderer.size.y, 0.1f);
        _cardText.GetComponent<UnitCardText>().ChangeText(unit.armyunithealth.startunithealth, unit.armyunithealth.currentsquadhealth, unit.armyunithealth.startsquadhealth, unit.numberofunits.currentnumberofunits, unit.numberofunits.startnumberofunits, unit.unitdamage.startunitdamage, unit.unitstats.currentinitiative, unit.unitstats.startinitiative, unit.unitstats.currentcohesion, unit.unitstats.startcohesion, unit.unitstats.currentarmour, unit.unitUpgrades);
        transform.position = _pos;
        transform.localPosition = _pos;
        thisUnit = unit;
        isUnitInArmy = isInArmy;
    }
    public void OnMouseEnterCollider()
    {
        isMouseOff = false;
        _spriteRenderer.color = _newCardColor;
        _spriteRenderer.size *= 1.5f;
        _cardSprite.transform.position = new Vector3(_startCardPos.x, _startCardPos.y, _startCardPos.z + _approach);
        _cardCollider.GetComponent<BoxCollider>().size = new Vector3(_spriteRenderer.size.x, _spriteRenderer.size.y, 0.1f);
        _cardText.GetComponent<UnitCardText>().UnHideText();
    }
    public void OnMouseExitCollider()
    {
        isMouseOff = true;
        _spriteRenderer.color = _startCardColor;
        _spriteRenderer.size = _startCardSize;
        _cardSprite.transform.position = _startCardPos;
        _cardCollider.GetComponent<BoxCollider>().size = new Vector3(_spriteRenderer.size.x, _spriteRenderer.size.y, 0.1f);
        _cardText.GetComponent<UnitCardText>().HideText();
    }
    public void OnMouseUpCollider()
    {
        EventManager.StartCardChosen(this.gameObject,isUnitInArmy);
    }
    public ArmyUnitClass GetCardUnit()
    { return thisUnit; }
}