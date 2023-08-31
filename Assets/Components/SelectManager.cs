using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu]
public class SelectManager : ScriptableObject
{
    public GameObject SelectedEntity;

    public void Start()
    {
        SelectedEntity = null;
    }
    public void SelectEntity(GameObject entity)
    {
        if (SelectedEntity == entity)
        {
            SelectedEntity.GetComponent<SelectAdapter>()?.Deselect.Invoke();
            SelectedEntity = null;
        }
        else
        {
            if (SelectedEntity!=null) SelectedEntity.GetComponent<SelectAdapter>()?.Deselect.Invoke();
            SelectedEntity = entity;
            SelectedEntity.GetComponent<SelectAdapter>()?.Select.Invoke();
        }
    }
    public void DeSelectEntity()
    {
        if (SelectedEntity!=null) SelectedEntity.GetComponent<SelectAdapter>()?.Deselect.Invoke();
        SelectedEntity = null;
    }
    public bool IsEntitySelected()
    {
        if (ReferenceEquals(SelectedEntity, null)) return false;
        else return true;
    }
}