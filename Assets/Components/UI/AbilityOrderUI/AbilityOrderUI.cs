using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityOrderUI : MonoBehaviour
{
    public Image AbilityIcon;
    public Image FromIcon;
    public Image ToIcon;
    public void SetIcons(Sprite abilityIcon,Sprite fromIcon,Sprite toIcon)
    {
        AbilityIcon.sprite = abilityIcon;
        FromIcon.sprite = fromIcon;
        ToIcon.sprite = toIcon;
    }
}
