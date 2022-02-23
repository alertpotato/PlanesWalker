using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Planeswalker.Scripts.Battle
{
    public class BattleStateManager : MonoBehaviour
    {
        BattleBaseState currentState;
        public BattlePrepareState prepareState = new BattlePrepareState();
        public BattleTacticsState tacticsState = new BattleTacticsState();
        public BattleEngagingState engagingState = new BattleEngagingState();
        public BattleEndingRoundState endingRoundState = new BattleEndingRoundState();
        void Start()
        {
            currentState = prepareState;
            currentState.EnterState(this);
        }

        // Update is called once per frame
        void Update()
        {
            currentState.UpdateState(this);
        }
        public void SwitchState(BattleBaseState state)
        {
            currentState = state;
            state.EnterState(this);
        }
    }
}
