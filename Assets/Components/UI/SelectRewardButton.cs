using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class SelectRewardButton : MonoBehaviour
{
    public Button SelectButton;
    public SelectManager SelectedEntity;
    public TextMeshProUGUI buttonText;

    private void Start()
    {
        SelectButton.interactable = false;
        buttonText.text = "";
    }

    void Update()
    {
        if (SelectedEntity.IsEntitySelected())
        {
            SelectButton.interactable = true;
            buttonText.text = "Select card";
        }
        else
        {
            SelectButton.interactable = false;
            buttonText.text = "";
        }
        
    }
}