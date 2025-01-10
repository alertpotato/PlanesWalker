using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class BattlefieldLogic : MonoBehaviour
{
    public Battlefield Battlefield;
    public List<UnitAbility> AbilitiesOrder = new List<UnitAbility>();
    public RectTransform AbilityUIBack;
    public GameObject AbilityUI;
    public GameObject AbilityAnimation;
    public GameObject AbilityUIParent;
    public List<GameObject> AbilitiesUI;
    public OtherGraphic IconsSprites;
    public UnitGraphic UnitSprites;
    public GameObject DamageIndicator;
    public float pauseBetweenAbilities = 1f;
    IEnumerator PlayAnimations(GameLoopRoundState parent)
    {
        float YY = -60;
        int index = 0;
        var arrowAnim = Instantiate(AbilityAnimation,AbilityUIParent.transform);
        
        foreach (var ability in AbilitiesOrder)
        {
            
            
            //Arrow anim
            float YYY = YY * index;
            arrowAnim.transform.localPosition = new Vector3(-120,YYY,0);
            index++;
            
            //Debug target line
            Color lineColor;
            float xAdj = 0.1f;
            if (ability.UnitField == Battlefield.PlayerFormation)
            {
                lineColor = Color.red;
                xAdj = -xAdj;
            }
            else
            {
                lineColor = Color.blue;
            }
            Vector3 startPos = Battlefield.GetFieldByUnit(ability.UnitCompany.Unit).transform.position;
            Vector3 endPos = Battlefield.GetFieldByUnit(ability.targets[0].Unit).transform.position;
            
            //Logic
            bool answer = ApplyAbility(ability);
            
            if (answer)
            {
                Debug.DrawLine(new Vector3(startPos.x+xAdj,startPos.y,startPos.z),new Vector3(endPos.x+xAdj,endPos.y,endPos.z) , lineColor, pauseBetweenAbilities*2);
            }
            //Logic
            Battlefield.UpdateField();
            if (answer) yield return new WaitForSeconds(pauseBetweenAbilities); // wait for x sec if ability was applied
        }
        Destroy(arrowAnim);
        parent.RoundEnd(); // this must be here
    }
    public void ApplyAbilities(GameLoopRoundState parent)
    {
        StartCoroutine(PlayAnimations(parent));
    }
    private bool ApplyAbility(UnitAbility ability)
    {
        if (ability.UnitCompany.Unit==null) return false;
        var cycleAbility = ability;
        // check target again -> calculate damage -> calculate retaliate damage -> apply -> check for defeted companies
        // check target again 
        if (!cycleAbility.SelectTargets())
        {
            var newPossibleAbility = cycleAbility.UnitCompany.Unit.GetComponent<ArmyUnitClass>().GetPossibleAbility();
            if (newPossibleAbility != null) cycleAbility = newPossibleAbility;
            if (!cycleAbility.SelectTargets())
            {
                Debug.Log($"{ability.UnitCompany.Unit.name} does not have targets");
                string dmgText = "skip";
                Color dmgColor = new Color(0.8f, 0.7f, 0.3f, 1);
                CreateDamageText(dmgText,dmgColor,ability.UnitCompany);
                return false;
            }
        }
        List<(GameObject, int, int, int,Company)> results = new List<(GameObject, int, int, int,Company)>();
        // calculate damage
        var abilityOwnerResult = cycleAbility.GetAbilityImpact();
        var abilityOpposingResult = (-1, -1, -1);
        
        // calculate retaliate damage
        var opposingCompany = cycleAbility.targets[0];
        var opposingCompanyAbility = opposingCompany.Unit.GetComponent<ArmyUnitClass>()
            .GetRetaliationAbility(cycleAbility.RetaliationTags);
        if (opposingCompanyAbility != null)
        {
            opposingCompanyAbility.AssignTargetForRetaliation(cycleAbility.UnitCompany);
            abilityOpposingResult = opposingCompanyAbility.GetAbilityImpact();
        }
        // create list of combat results
        results.Add((cycleAbility.UnitCompany.Unit,abilityOpposingResult.Item1,abilityOpposingResult.Item2,abilityOwnerResult.Item3,cycleAbility.UnitCompany));
        results.Add((opposingCompany.Unit,abilityOwnerResult.Item1,abilityOwnerResult.Item2,abilityOpposingResult.Item3,opposingCompany));
        // apply -> check for defeted companies
        ApplyResults(results);
        return true;
    }

    public void ApplyResults(List<(GameObject, int, int, int,Company)> results)
    {
        foreach (var res in results)
        {
            if (res.Item2 != -1)
            {
                string dmgText = (res.Item2-res.Item5.Unit.GetComponent<ArmyUnitClass>().currentSquadHealth).ToString();
                Color dmgColor = new Color(0.55f, 0, 0, 1);   
                CreateDamageText(dmgText,dmgColor,res.Item5);
                if (!res.Item1.GetComponent<ArmyUnitClass>().TakeDamage((res.Item2, res.Item3)))
                {
                    res.Item5.Field.RemoveUnitFromField(res.Item1);
                    continue;
                }
            }
            if (res.Item4 != -1) 
                res.Item1.GetComponent<ArmyUnitClass>().UpdateEffectiveness(res.Item4);
        }
    }
    private void CreateDamageText(string text, Color color,Company comp)
    {
        var allcells = new List<GameObject>();
        allcells.AddRange(Battlefield.playerFieldList);
        allcells.AddRange(Battlefield.enemyFieldList);
        var targetCell = allcells.Where(cell => cell.GetComponent<OnFieldCompanyManager>().Company == comp)
            .First();
        Vector3 newPos = new Vector3(targetCell.transform.position.x, targetCell.transform.position.y, targetCell.transform.position.z*1.1f);
        DamageIndicator indicator =
            Instantiate(DamageIndicator, targetCell.transform.position, Quaternion.identity)
                .GetComponent<DamageIndicator>();
        indicator.transform.SetParent(targetCell.transform);
        indicator.InitializeIndicator(color,text);
    }

    public void Order()
    {
        DestroyUI();
        AbilitiesUI = new List<GameObject>();
        AbilitiesOrder.Clear();
        List<Company> onFieldUnits = new List<Company>();

        var onFieldPlayerCompanies = Battlefield.PlayerFormation.GetOnFieldcompanies();
        var onFieldEnemyCompanies = Battlefield.EnemyFormation.GetOnFieldcompanies();

        onFieldUnits.AddRange(onFieldPlayerCompanies);
        onFieldUnits.AddRange(onFieldEnemyCompanies);
        var sortedUnits = from comp in onFieldUnits
            orderby comp.Unit.GetComponent<ArmyUnitClass>().CurrentUnitCharacteristics.Initiative descending
            select comp;
        string answer = "";
        foreach (var comp in sortedUnits)
        {
            answer = $"{answer} -> {comp.Unit.gameObject.name}[{comp.Unit.GetComponent<ArmyUnitClass>().CurrentUnitCharacteristics.Initiative}]";
            var ab = comp.Unit.GetComponent<ArmyUnitClass>().GetPossibleAbility();
            if (ab!=null) AbilitiesOrder.Add(ab);
        }
        //Debug.Log(answer);
        // -------------GRAPHIC
        float YY = -60;
        int index = 0;
        foreach (var ability in AbilitiesOrder)
        {
            //Debug.Log($"{ability.UnitSquad.Unit.name} {ability.IsActive.ToString()}");
            float YYY = YY * index;
            var newUI = Instantiate(AbilityUI,AbilityUIParent.transform);
            newUI.transform.localPosition = new Vector3(0,YYY,0);
            AbilitiesUI.Add(newUI);
            newUI.GetComponent<AbilityOrderUI>().SetIcons(
                IconsSprites.GetSpriteByName(ability.AbilityName),
                UnitSprites.GetIconSpriteByName(ability.UnitCompany.Unit.GetComponent<ArmyUnitClass>().UnitName),
                UnitSprites.GetIconSpriteByName(ability.targets[0].Unit.GetComponent<ArmyUnitClass>().UnitName)
                );
            index++;
        }
        AbilityUIBack.sizeDelta = new Vector2(AbilityUIBack.rect.width,114*index);
        Battlefield.UpdateField();
    }
    private void DestroyUI()
    {
        foreach (var ui in AbilitiesUI)
        {
            Destroy(ui);
        }
    }
}
