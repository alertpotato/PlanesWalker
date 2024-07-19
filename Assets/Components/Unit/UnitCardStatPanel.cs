using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UnitCardStatPanel : MonoBehaviour
{
    public UnitCardMain MainCardScript;
    public GameObject StatPanel;
    public TextMeshProUGUI StatText;
    public float maxX=52;
    public float minX=19;
    public float animationSpeed = 300f;
    void Update()
    {
        float multi;
        /*if (MainCardScript.IsSelected) multi = animationSpeed;
        else multi = -animationSpeed;
        var localPos = StatPanel.transform.localPosition;
        StatPanel.transform.localPosition = new Vector3(Mathf.Clamp(localPos.x + multi * Time.deltaTime,minX,maxX), localPos.y, localPos.z);
        if (!MainCardScript.IsSelected && localPos.x == minX) MainCardScript.UIElements.SetActive(false);
        */
    }
    public void SetStatText(UnitCharacteristics defaultChars,UnitCharacteristics currentChars, UnitUpgrades upgrd)
    {
        string unitDescription;
        string nupgrd = ""; string hupgrd = ""; string dupgrd = ""; string iupgrd = ""; string cupgrd = ""; string aupgrd = "";
        nupgrd = AddStars(nupgrd, upgrd.NumberOfUnits);
        hupgrd = AddStars(hupgrd, upgrd.Health);
        dupgrd = AddStars(dupgrd, upgrd.Damage);
        iupgrd = AddStars(iupgrd, upgrd.Initiative);
        cupgrd = AddStars(cupgrd, upgrd.Cohesion);
        aupgrd = AddStars(aupgrd, upgrd.Armour);
        unitDescription = $"{currentChars.NumberOfUnits*currentChars.Health}({defaultChars.NumberOfUnits*defaultChars.Health})\n{currentChars.NumberOfUnits}({defaultChars.NumberOfUnits}){nupgrd}\n{currentChars.Health}({defaultChars.Health}){hupgrd}\n{currentChars.Damage}({defaultChars.Damage}){dupgrd}\n{currentChars.Initiative}({defaultChars.Initiative}){iupgrd}\n{currentChars.Cohesion}({defaultChars.Cohesion}){cupgrd}\n{currentChars.Armour}({defaultChars.Armour}){aupgrd}";
        StatText.text = unitDescription;
    }
    private string AddStars(string starsToAdd, int numOfTimes)
    {
        for (int i = 0; i < numOfTimes; i++)
        {
            starsToAdd += "★";
        }
        return $"<color=#c8a106>{starsToAdd}</color>" ;
    }
}
