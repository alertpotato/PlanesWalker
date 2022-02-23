using UnityEngine;

namespace Planeswalker.Scripts.Battle
{
    public class BattlePrepareState : BattleBaseState
    {
        public override void EnterState(BattleStateManager battle)
        {
            Debug.Log("Hello from Prepare State");
        }
        public override void UpdateState(BattleStateManager battle)
        { }
        public override void OnCollisionEnter(BattleStateManager battle)
        { }
    }
}
