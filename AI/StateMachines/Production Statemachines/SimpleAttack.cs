using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SquadAI
{
    public class SimpleAttack : AIStatemachine
    {
        public override void Init(AIStatemachineManager m)
        {
            base.Init(m);

            transitions = new AITransition[4];

            transitions[0] = new AITransition();
            transitions[0].condition = AIConditions.GoodShoot;
            transitions[0].inverse = false;
            transitions[0].currentState = 0;
            transitions[0].jumpIndex = 2;

            transitions[1] = new AITransition();
            transitions[1].condition = AIConditions.IsRedundant;
            transitions[1].inverse = false;
            transitions[1].currentState = 0;
            transitions[1].jumpIndex = 1;

            transitions[2] = new AITransition();
            transitions[2].condition = AIConditions.IsRedundant;
            transitions[2].inverse = false;
            transitions[2].currentState = 2;
            transitions[2].jumpIndex = 0;

            transitions[3] = new AITransition();
            transitions[3].condition = AIConditions.LostPlayer;
            transitions[3].inverse = false;
            transitions[3].currentState = 2;
            transitions[3].jumpIndex = 0;

        }

        protected override void ConfigureStates()
        {
            states = new IEnumerator[3];
            states[0] = AIStates.GotoPlayer(blackboardReference, 8, manager.components);
            states[1] = AIStates.GotoCover(blackboardReference, manager.components, 4, true, 0, true);
            states[2] = AIStates.StandAndShootPlayer(blackboardReference, manager.components);
 
        }
    }
}

