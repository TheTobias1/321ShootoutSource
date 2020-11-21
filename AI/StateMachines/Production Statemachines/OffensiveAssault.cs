using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SquadAI
{
    public class OffensiveAssault : AIStatemachine
    {
        public override void Init(AIStatemachineManager m)
        {
            base.Init(m);

            AITransition goto_stand = new AITransition();
            goto_stand.condition = AIConditions.GoodShoot;
            goto_stand.inverse = false;
            goto_stand.currentState = 0;
            goto_stand.jumpIndex = 3;

            AITransition stand_goto = new AITransition();
            stand_goto.condition = AIConditions.LostPlayer;
            stand_goto.inverse = false;
            stand_goto.currentState = 3;
            stand_goto.jumpIndex = 0;

            AITransition cover_goto = new AITransition();
            cover_goto.condition = AIConditions.LostPlayer;
            cover_goto.inverse = false;
            cover_goto.currentState = 1;
            cover_goto.jumpIndex = 0;

            AITransition goto_cover = new AITransition();
            goto_cover.condition = AIConditions.IsRedundant;
            goto_cover.inverse = false;
            goto_cover.currentState = 0;
            goto_cover.jumpIndex = 2;

            AITransition stand_cover = new AITransition();
            stand_cover.condition = AIConditions.IsRedundant;
            stand_cover.inverse = false;
            stand_cover.currentState = 3;
            stand_cover.jumpIndex = 0;

            transitions = new AITransition[4];
            transitions[0] = goto_stand;
            transitions[1] = stand_goto;
            //transitions[2] = cover_goto;
            transitions[2] = goto_cover;
            transitions[3] = stand_cover;
        }

        protected override void ConfigureStates()
        {
            states = new IEnumerator[4];
            states[0] = AIStates.GotoPlayer(blackboardReference, blackboardReference.distanceToPlayer > blackboardReference.closeAttackRange? 8 : 4, manager.components);
            states[1] = AIStates.GotoCover(blackboardReference, manager.components, 4, true, 2, false);
            states[2] = AIStates.GotoCover(blackboardReference, manager.components, 4, true, 0, true);
            states[3] = AIStates.StandAndShootPlayer(blackboardReference, manager.components);
        }
    }
}
