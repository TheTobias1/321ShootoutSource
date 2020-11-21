using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SquadAI
{
    public class LookAroundAttack : AIStatemachine
    {
        public override void Init(AIStatemachineManager m)
        {
            base.Init(m);

            AITransition attackOnSee = new AITransition();
            attackOnSee.condition = AIConditions.CanSeePlayer;
            attackOnSee.inverse = false;
            attackOnSee.currentState = 0;
            attackOnSee.jumpIndex = 1;

            AITransition stand_cover = new AITransition();
            stand_cover.condition = AIConditions.Healthy;
            stand_cover.inverse = true;
            stand_cover.currentState = 1;
            stand_cover.jumpIndex = 2;

            AITransition stand_approach = new AITransition();
            stand_approach.condition = AIConditions.LostPlayer;
            stand_approach.inverse = false;
            stand_approach.currentState = 1;
            stand_approach.jumpIndex = 3;

            transitions = new AITransition[3];
            transitions[0] = attackOnSee;
            transitions[1] = stand_cover;
            transitions[2] = stand_approach;
        }

        protected override void ConfigureStates()
        {
            states = new IEnumerator[4];
            Debug.Log("Current: " + currentState);
            states[0] = AIStates.LookAround(blackboardReference, manager.components);
            states[1] = AIStates.StandAndShootPlayer(blackboardReference, manager.components);
            states[2] = AIStates.GotoCover(blackboardReference, manager.components, 6, true, 4);
            states[3] = AIStates.ApproachAttack(blackboardReference, manager.components, 3);
        }
    }
}
