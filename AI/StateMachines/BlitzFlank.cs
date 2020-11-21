using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SquadAI
{

    public class BlitzFlank : AIStatemachine
    {
        public override void Init(AIStatemachineManager m)
        {
            base.Init(m);
            transitions = new AITransition[4];

            //attack1
            transitions[0] = new AITransition();
            transitions[0].condition = AIConditions.CanSeePlayer;
            transitions[0].inverse = false;
            transitions[0].currentState = 1;
            transitions[0].jumpIndex = 4;

            transitions[1] = new AITransition();
            transitions[1].condition = AIConditions.CanSeePlayer;
            transitions[1].inverse = false;
            transitions[1].currentState = 2;
            transitions[1].jumpIndex = 4;

            transitions[2] = new AITransition();
            transitions[2].condition = AIConditions.CanSeePlayer;
            transitions[2].inverse = false;
            transitions[2].currentState = 3;
            transitions[2].jumpIndex = 4;

            transitions[3] = new AITransition();
            transitions[3].condition = AIConditions.LostPlayer;
            transitions[3].inverse = false;
            transitions[3].currentState = 4;
            transitions[3].jumpIndex = -1;
        }

        protected override void ConfigureStates()
        {
            states = new IEnumerator[5];
            states[0] = AIStates.CalculateFlank(blackboardReference, manager.components);
            states[1] = AIStates.GotoPosition(blackboardReference, 0, 8, manager.components, 6);
            states[2] = AIStates.GotoPosition(blackboardReference, 1, 8, manager.components, 6);
            states[3] = AIStates.GotoPlayer(blackboardReference, 8, manager.components);
            states[4] = AIStates.StandardAttack(blackboardReference, manager.components);
        }
    }
}
