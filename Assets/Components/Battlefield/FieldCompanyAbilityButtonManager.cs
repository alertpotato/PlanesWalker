using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FieldCompanyAbilityButtonManager : MonoBehaviour
{
    public Button AbilityButton;
    public Image ButtonBackground;
    public Image ButtonIcon;
    public Color DefaultBackgroundColor = new Color(0.1792453f,0,0.04004889f);
    public Color SelectedBackgroundColor = new Color(0.009901334f,0.5754717f,0);
    public GameObject FieldCompany;
    public bool IsButtonsActive = false;
    public int CurrentIndex = -1;

    public void InitializeButton(int index, BattlefieldLogic funcObect, Sprite icon,GameObject fieldCompany,bool isButtonsActive)
    {
        IsButtonsActive = isButtonsActive;
        DeselectAbility();
        if (IsButtonsActive) AbilityButton.onClick.AddListener(() => funcObect.ManualAbilityPick(index,FieldCompany));
        ButtonIcon.sprite = icon;
        FieldCompany = fieldCompany;
        CurrentIndex = index;
    }
    public void SelectAbility()
    {
        ButtonBackground.color = SelectedBackgroundColor;
    }
    public void DeselectAbility()
    {
        ButtonBackground.color = DefaultBackgroundColor;
    }
}
