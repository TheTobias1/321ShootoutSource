using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SquadAI
{
    public class DefensiveAttack : AIStatemachine
    {
        public override void Init(AIStatemachineManager m)
        {
            base.Init(m);

            AITransition stand_cover = new AITransition();
            stand_cover.condition = AIConditions.Healthy;
            stand_cover.inverse = true;
            stand_cover.currentState = 0;
            stand_cover.jumpIndex = 1;

            AITransition stand_approach = new AITransition();
            stand_approach.condition = AIConditions.LostPlayer;
            stand_approach.inverse = false;
            stand_approach.currentState = 0;
            stand_approach.jumpIndex = 2;

            transitions = new AITransition[2];
            transitions[0] = stand_cover;
            transitions[1] = stand_approach;
        }

        protected override void ConfigureStates()
        {
            states = new IEnumerator[3];
            states[0] = AIStates.StandAndShootPlayer(blackboardReference, manager.components);
            states[1] = AIStates.GotoCover(blackboardReference, manager.components, 8, true, 4);
            states[2] = AIStates.ApproachAttack(blackboardReference, manager.components, 3);
        }
    }
}

