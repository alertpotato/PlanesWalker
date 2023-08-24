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
    
}