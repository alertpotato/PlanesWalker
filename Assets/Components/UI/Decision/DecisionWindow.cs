using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DecisionWindow : MonoBehaviour
{
    public TextMeshProUGUI Text;

    public void UpdateText(string text)
    {
        Text.text = text;
    }
}
