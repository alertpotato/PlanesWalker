using UnityEngine;

public class UnitCardText : MonoBehaviour
{
    private string unitDescription;
    public void ChangeText(int health, int currheslth, int starthealth, int currnumofunits, int startnumofunits, int damage, int currinit, int startinit, int currcoh, int startcoh, int armour, CountUnitUpgrades upgrd)
    {
        string hupgrd = ""; string nupgrd = ""; string dupgrd = ""; string iupgrd = ""; string cupgrd = ""; string aupgrd = "";
        hupgrd = AddStars(hupgrd, upgrd.chealth);
        nupgrd = AddStars(nupgrd, upgrd.cnumber);
        dupgrd = AddStars(dupgrd, upgrd.cdamage);
        iupgrd = AddStars(iupgrd, upgrd.cinitiative);
        cupgrd = AddStars(cupgrd, upgrd.ccohesion);
        aupgrd = AddStars(aupgrd, upgrd.carmour);
        unitDescription = $"{hupgrd}Unit health:{health}\n{dupgrd}Unit damage:{damage}\n{nupgrd}Number of units:{currnumofunits}({startnumofunits})\nArmy health:{currheslth}({starthealth})\n{iupgrd}Unit initiative{currinit}({startinit})\n{cupgrd}Unit cohesion:{currcoh}({startcoh})\n{aupgrd}Unit armour:{armour}";
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