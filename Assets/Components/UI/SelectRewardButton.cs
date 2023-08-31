using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SelectRewardButton : MonoBehaviour
{
    public Button SelectButton;
    public SelectManager SelectedEntity;
    private void Start()
    {
        SelectButton.interactable = false;
    }

    void Update()
    {
        if (SelectedEntity.IsEntitySelected())
        {
            SelectButton.interactable = true;
        }
        else SelectButton.interactable = false;
    }
}