using System;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

public class AbilityPanelManager : MonoBehaviour
{
    public Image PanelIcon;
    public TextMeshProUGUI PanelText;

    public void Initialize(Sprite abilityIcon,string panelText)
    {
        PanelIcon.sprite = abilityIcon;
        PanelText.text = panelText;
    }
}
