
using UnityEngine;

namespace Planeswalker.Scripts.Battle
{
    public abstract class BattleBaseState
    {
        public abstract void EnterState(BattleStateManager battle);
        public abstract void UpdateState(BattleStateManager battle);
        public abstract void OnCollisionEnter(BattleStateManager battle);


    }
}
