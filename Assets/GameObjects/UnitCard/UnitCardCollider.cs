using UnityEngine;

public class UnitCardCollider : MonoBehaviour
{
    [SerializeField] private bool isMouseOff;
    [SerializeField] private UnitCardMain parentCard;
    [SerializeField] private Camera _mainCamera;
    private void Awake()
    {
        _mainCamera = Camera.main;
    }
    private void Start()
    {
    }
    public void GetParent(UnitCardMain _parent)
    {
        if (parentCard == null) { parentCard = _parent; };
    }
    public RaycastHit GetRaycastHit()
    {
        Physics.Raycast(_mainCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit raycastHit);
        return raycastHit;
    }
    public bool GetTrueMousePosition()
    { return isMouseOff; }
    private void OnMouseExit()
    {
        isMouseOff = true;
        parentCard.OnMouseExitCollider();
    }
    private void OnMouseEnter()
    {
        isMouseOff = false;
        parentCard.OnMouseEnterCollider();
    }
    private void OnMouseUp()
    {
        parentCard.OnMouseUpCollider();
    }
}