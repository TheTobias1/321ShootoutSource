using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SquadAI
{
    public class CoverFollow : AIStatemachine
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
            stand_goto.condition = AIConditions.InDanger;
            stand_goto.inverse = false;
            stand_goto.currentState = 0;
            stand_goto.jumpIndex = 2;

            AITransition lostLeader = new AITransition();
            lostLeader.condition = AIConditions.HasLeader;
            lostLeader.inverse = true;
            lostLeader.currentState = 0;
            lostLeader.jumpIndex = 2;

            transitions = new AITransition[3];
            transitions[0] = goto_stand;
            transitions[1] = stand_goto;
            transitions[2] = lostLeader;


        }

        protected override void ConfigureStates()
        {
            states = new IEnumerator[3];
            states[0] = AIStates.FollowLeader(blackboardReference, manager.components, 6, 0);
            states[1] = AIStates.GotoCover(blackboardReference, manager.components, 8, true, Random.Range(1, 5));
            states[2] = AIStates.StandardAttack(blackboardReference, manager.components);
        }
    }
}
