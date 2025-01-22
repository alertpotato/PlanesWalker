using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HintManager : MonoBehaviour
{
    public GameObject HintParent;
    public TextMeshProUGUI HintText;
    public RectTransform HintBox;

    void Start()
    {
        HideHint();
    }

    public void ShowHint(string newHintText,Vector3 mousePosition)
    {
        HintText.text=newHintText;
        HintBox.sizeDelta = new Vector2(HintText.preferredWidth>400 ? 400 : HintText.preferredWidth,HintText.preferredHeight);
        HintParent.transform.position = mousePosition;
        HintParent.SetActive(true);
    }

    public void HideHint()
    {
        HintText.text = "";
        HintParent.SetActive(false);
    }

}
