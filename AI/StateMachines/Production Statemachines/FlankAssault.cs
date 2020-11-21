using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SquadAI
{
    public class FlankAssault : AIStatemachine
    {
        public override void Init(AIStatemachineManager m)
        {
            base.Init(m);

            transitions = new AITransition[9];

            //Flank to shoot
            transitions[0] = new AITransition();
            transitions[0].condition = AIConditions.CanSeePlayer;
            transitions[0].inverse = false;
            transitions[0].currentState = 1;
            transitions[0].jumpIndex = 3;

            transitions[1] = new AITransition();
            transitions[1].condition = AIConditions.CanSeePlayer;
            transitions[1].inverse = false;
            transitions[1].currentState = 2;
            transitions[1].jumpIndex = 3;

            transitions[2] = new AITransition();
            transitions[2].condition = AIConditions.GoodShoot;
            transitions[2].inverse = false;
            transitions[2].currentState = 3;
            transitions[2].jumpIndex = 4;

            //Attack
            transitions[3] = new AITransition();
            transitions[3].condition = AIConditions.IsRedundant;
            transitions[3].inverse = false;
            transitions[3].currentState = 4;
            transitions[3].jumpIndex = 5;

            transitions[8] = new AITransition();
            transitions[8].condition = AIConditions.CanSeePlayer;
            transitions[8].inverse = true;
            transitions[8].currentState = 4;
            transitions[8].jumpIndex = 5;

            transitions[4] = new AITransition();
            transitions[4].condition = AIConditions.LostPlayer;
            transitions[4].inverse = true;
            transitions[4].currentState = 5;
            transitions[4].jumpIndex = 0;

            transitions[5] = new AITransition();
            transitions[5].condition = AIConditions.LostPlayer;
            transitions[5].inverse = false;
            transitions[5].currentState = 6;
            transitions[5].jumpIndex = 0;

            //Interrupt flank on close
            transitions[6] = new AITransition();
            transitions[6].condition = AIConditions.IsCloseToPlayer;
            transitions[6].inverse = false;
            transitions[6].currentState = 1;
            transitions[6].jumpIndex = 3;

            transitions[7] = new AITransition();
            transitions[7].condition = AIConditions.IsCloseToPlayer;
            transitions[7].inverse = false;
            transitions[7].currentState = 2;
            transitions[7].jumpIndex = 3;
        }

        protected override void ConfigureStates()
        {
            states = new IEnumerator[7];

            //flank
            states[0] = AIStates.CalculateFlank(blackboardReference, manager.components);
            states[1] = AIStates.GotoPosition(blackboardReference, 0, 8, manager.components, 6);
            states[2] = AIStates.GotoPosition(blackboardReference, 1, 8, manager.components, 6);

            //attack
            states[3] = AIStates.GotoPlayer(blackboardReference, blackboardReference.distanceToPlayer > blackboardReference.closeAttackRange ? 8 : 4, manager.components);
            states[4] = AIStates.StandAndShootPlayer(blackboardReference, manager.components);

            //Tactically place
            states[5] = AIStates.GotoCover(blackboardReference, manager.components, 4, true, 0, true);

            //re attack
            states[6] = AIStates.StandAndShootPlayer(blackboardReference, manager.components);
        }
    }
}
