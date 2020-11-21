using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SquadAI
{
    public class CautiousApproach : AIStatemachine
    {
        public override void Init(AIStatemachineManager m)
        {
            base.Init(m);

            AITransition goto_stand = new AITransition();
            goto_stand.condition = AIConditions.CanSeePlayer;
            goto_stand.inverse = false;
            goto_stand.currentState = 0;
            goto_stand.jumpIndex = 2;

            AITransition stand_goto = new AITransition();
            stand_goto.condition = AIConditions.Healthy;
            stand_goto.inverse = false;
            stand_goto.currentState = 2;
            stand_goto.jumpIndex = 1;

            transitions = new AITransition[2];
            transitions[0] = goto_stand;
            transitions[1] = stand_goto;
        }

        protected override void ConfigureStates()
        {
            Debug.Log("SQUAD APPROACHING");
            states = new IEnumerator[3];
            states[0] = AIStates.GotoPlayer(blackboardReference, 3, manager.components);
            states[1] = AIStates.GotoCover(blackboardReference, manager.components, 8, true, Random.Range(1, 5));
            states[2] = AIStates.StandardAttack(blackboardReference, manager.components);
        }
    }
}