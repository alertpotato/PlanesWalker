using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GameLoopSharedData))]
public class GameLoopRewardState : StateBehaviour
{
    private GameLoopSharedData Config;
    public int NumberOfRewards = 5;
    public int NumberOfChoices = 5;
    public int leftNumberOfChoices;
    [SerializeField]private bool isChosing;
    public override void OnEnter()
    {
        leftNumberOfChoices = NumberOfChoices;
        isChosing = false;
    }

    public override void OnUpdate()
    {
        if (!isChosing & leftNumberOfChoices == 0)
        {
            CreateCardChoice(NumberOfRewards);
            isChosing = true;
            leftNumberOfChoices--;
        }

        if (!isChosing & leftNumberOfChoices == 0)
        {
            ChangeState<GameLoopPreBattleState>();
        }
    }
    
    private void CreateCardChoice(int cardsNum)
    {
        for (int i = 0; i < cardsNum; i++)
        {
            //float r = Random.value;
            var a = Camera.main.ScreenToWorldPoint(new Vector3((Screen.width / (cardsNum + 2)) * (i + 1), Screen.height/2, 8));
        }
    }
}

