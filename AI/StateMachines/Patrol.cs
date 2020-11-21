using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SquadAI
{
    public class Patrol : AIStatemachine
    {
        public override void Init(AIStatemachineManager m)
        {
            Debug.Log("PATROL");
            base.Init(m);

            transitions = new AITransition[5];

            transitions[0] = new AITransition();
            transitions[0].condition = AIConditions.CanSeePlayer;
            transitions[0].inverse = false;
            transitions[0].currentState = 0;
            transitions[0].jumpIndex = 5;

            transitions[1] = new AITransition();
            transitions[1].condition = AIConditions.CanSeePlayer;
            transitions[1].inverse = false;
            transitions[1].currentState = 1;
            transitions[1].jumpIndex = 5;

            transitions[2] = new AITransition();
            transitions[2].condition = AIConditions.CanSeePlayer;
            transitions[2].inverse = false;
            transitions[2].currentState = 2;
            transitions[2].jumpIndex = 5;

            transitions[3] = new AITransition();
            transitions[3].condition = AIConditions.CanSeePlayer;
            transitions[3].inverse = false;
            transitions[3].currentState = 3;
            transitions[3].jumpIndex = 5;


            transitions[4] = new AITransition();
            transitions[4].condition = AIConditions.LostPlayer;
            transitions[4].inverse = false;
            transitions[4].currentState = 4;
            transitions[4].jumpIndex = 1;
        }

        protected override void ConfigureStates()
        {
            Debug.Log("Configure PATROL");
            states = new IEnumerator[6];

            states[0] = AIStates.CalculatePatrolPath(blackboardReference, manager.components, 80);
            states[1] = AIStates.GotoPosition(blackboardReference, 0, 3, manager.components, 3, 3, true);
            states[2] = AIStates.GotoPosition(blackboardReference, 1, 3, manager.components, 3, 3, true);
            states[3] = AIStates.GotoPosition(blackboardReference, 2, 3, manager.components, 3, 3, true);
            states[4] = AIStates.GotoPosition(blackboardReference, 3, 3, manager.components, 3, 3, true);

            states[5] = AIStates.StandAndShootPlayer(blackboardReference, manager.components);
        }
    }
}
