using UnityEngine;
using UnityEngine.Events;

public class SelectAdapter : MonoBehaviour
{
    public UnityEvent Select;
    public UnityEvent Deselect;
    public UnityEvent<GameObject,GameObject> SelectField;
    public UnityEvent<GameObject> DeselectField;
}
