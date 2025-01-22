using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CreateAssetMenu]
public class SelectManager : ScriptableObject
{
    public GameObject SelectedEntity = null;
    public void OnValidate()
    {
        SelectedEntity = null;
    }
    public bool SelectEntity(GameObject entity)
    {
        if (SelectedEntity == entity)
        {
            SelectedEntity.GetComponent<SelectAdapter>()?.Deselect.Invoke();
            SelectedEntity = null;
            return false;
        }
        else
        {
            if (SelectedEntity!=null) SelectedEntity.GetComponent<SelectAdapter>()?.Deselect.Invoke();
            SelectedEntity = entity;
            SelectedEntity.GetComponent<SelectAdapter>()?.Select.Invoke();
            return true;
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