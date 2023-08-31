using UnityEngine;

public class UnitCardText : MonoBehaviour
{
    private string unitDescription;
    public void ChangeText(UnitCharacteristics defaultChars,UnitCharacteristics currentChars, UnitUpgrades upgrd)
    {
        string nupgrd = ""; string hupgrd = ""; string dupgrd = ""; string iupgrd = ""; string cupgrd = ""; string aupgrd = "";
        nupgrd = AddStars(nupgrd, upgrd.NumberOfUnits);
        hupgrd = AddStars(hupgrd, upgrd.Health);
        dupgrd = AddStars(dupgrd, upgrd.Damage);
        iupgrd = AddStars(iupgrd, upgrd.Initiative);
        cupgrd = AddStars(cupgrd, upgrd.Cohesion);
        aupgrd = AddStars(aupgrd, upgrd.Armour);
        unitDescription = $"Army health:{currentChars.NumberOfUnits*currentChars.Health}({defaultChars.NumberOfUnits*defaultChars.Health})\n{nupgrd}Number of units:{currentChars.NumberOfUnits}({defaultChars.NumberOfUnits})\n{hupgrd}Unit health:{currentChars.Health}({defaultChars.Health})\n{dupgrd}Unit damage:{currentChars.Damage}({defaultChars.Damage})\n{iupgrd}Unit initiative{currentChars.Initiative}({defaultChars.Initiative})\n{cupgrd}Unit cohesion:{currentChars.Cohesion}({defaultChars.Cohesion})\n{aupgrd}Unit armour:{currentChars.Armour}({defaultChars.Armour})";
        //GetComponent<TextMesh>().text = $"{hupgrd}Unit health:{health}\n{dupgrd}Unit damage:{damage}\n{nupgrd}Number of units:{currnumofunits}({startnumofunits})\nArmy health:{currheslth}({starthealth})\n{iupgrd}Unit initiative{currinit}({startinit})\n{cupgrd}Unit cohesion:{currcoh}({startcoh})\n{aupgrd}Unit armour:{armour}";
    }
    private string AddStars(string starsToAdd, int numOfTimes)
    {
        for (int i = 0; i < numOfTimes; i++)
        {
            starsToAdd += "★";
        }
        return starsToAdd;
    }
    public void HideText()
    {
        GetComponent<TextMesh>().text = "";
    }
    public void UnHideText()
    {
        GetComponent<TextMesh>().text = unitDescription;
    }
}