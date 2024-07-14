using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UnitCardSupplyPanel : MonoBehaviour
{
    public UnitCardMain MainCardScript;
    public GameObject SupplyPanel;
    public TextMeshProUGUI SupplyText;
    public float maxY=-28;
    public float minY=-55;
    public float animationSpeed = 300f;
    void Update()
    {
        float multi;
        if (MainCardScript.IsSelected) multi = animationSpeed;
        else multi = -animationSpeed;
        var localPos = SupplyPanel.transform.localPosition;
        SupplyPanel.transform.localPosition = new Vector3(localPos.x,Mathf.Clamp( localPos.y - multi * Time.deltaTime,minY,maxY), localPos.z);
        //if (!MainCardScript.IsSelected && localPos.y == minY) MainCardScript.UIElements.SetActive(false);
    }
    public void SetSupplyText(int[] unitSupplyReq)
    {
        string unitDescription = "";
        foreach (var req in unitSupplyReq)
        {
            unitDescription = unitDescription + req;
        }
        SupplyText.text = unitDescription;
    }
}
