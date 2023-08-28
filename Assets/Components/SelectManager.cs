using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu]
public class SelectManager : ScriptableObject
{
    public GameObject SelectedEntity;
    // Start is called before the first frame update
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
        SelectedEntity.GetComponent<SelectAdapter>()?.Deselect.Invoke();
        SelectedEntity = null;
    }
    public GameObject GetSelectedEntity()
    {
        return SelectedEntity;
    }
}