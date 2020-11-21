using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SquadAI
{
    public class GotoAndShoot : AIStatemachine
    {
        public override void Init(AIStatemachineManager m)
        {
            base.Init(m);

            AITransition goto_stand = new AITransition();
            goto_stand.condition = AIConditions.CanSeePlayer;
            goto_stand.inverse = false;
            goto_stand.currentState = 0;
            goto_stand.jumpIndex = 1;

            AITransition stand_goto = new AITransition();
            stand_goto.condition = AIConditions.LostPlayer;
            stand_goto.inverse = false;
            stand_goto.currentState = 1;
            stand_goto.jumpIndex = 0;

            transitions = new AITransition[2];
            transitions[0] = goto_stand;
            transitions[1] = stand_goto;
        }

        protected override void ConfigureStates()
        {
            states = new IEnumerator[2];
            states[0] = AIStates.GotoPlayer(blackboardReference, 8, manager.components);
            states[1] = AIStates.StandardAttack(blackboardReference, manager.components);
        }
    }
}
