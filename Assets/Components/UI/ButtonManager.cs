using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    public Button myButton;
    public TextMeshProUGUI ButtonText;

    public void ChangeButtonText(string buttonText)
    {
        ButtonText.text = buttonText;
    }
}
