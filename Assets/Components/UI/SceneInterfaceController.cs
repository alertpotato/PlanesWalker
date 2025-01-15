using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SceneInterfaceController : MonoBehaviour
{
    [Header("Supply")]
    [SerializeField]private Image SupplyIcons;
    [SerializeField]private TextMeshProUGUI SupplyFoodText;
    [SerializeField]private TextMeshProUGUI SupplyWeaponsText;
    [SerializeField]private TextMeshProUGUI SupplyGoldText;
    [SerializeField]private TextMeshProUGUI SupplyKnowledgeText;
    [Header("HelpText")]
    [SerializeField]private TextMeshProUGUI HeadText;
    [SerializeField]private TextMeshProUGUI BottomText;
    [Header("TechHint")]
    [SerializeField]private TextMeshProUGUI HintText;
    
    // Start is called before the first frame update
    public void UpdateSupply(int[] supply)
    {
        SupplyFoodText.text = supply[0].ToString();
        SupplyWeaponsText.text = supply[1].ToString();
        SupplyGoldText.text = supply[2].ToString();
        SupplyKnowledgeText.text = supply[3].ToString();
    }

    public void UpdateHelpText(string newHeadText,string newBottomText)
    {
        HeadText.text = newHeadText;
        BottomText.text = newBottomText;
    }

    public void UpdateHintText(int days, int[] supply, float speed)
    {
        string newHintText = $"Day:{days}\nEnemy supply: {supply[0]} {supply[1]} {supply[2]}\nSpeed: {speed}\nW S to change speed\nR to restart game";
        HintText.text = newHintText;
    }
}
