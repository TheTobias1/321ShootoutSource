using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SquadAI
{
    public abstract class AIStatemachine
    {
        protected AIStatemachineManager manager;
        protected AIBlackboard blackboardReference;

        protected AITransition[] transitions;
        protected IEnumerator[] states;
        public int currentState;

        public virtual void Init(AIStatemachineManager m)
        {
            manager = m;
            blackboardReference = manager.Blackboard;
            ConfigureStates();
        }

        public void StartStatemachine()
        {
            currentState = 0;
            manager.StartCoroutine(states[0]);
        }

        public void AdvanceState()
        {
            if (states.Length - 1 <= currentState || states[currentState + 1] == null)
            {
                currentState = 0;
            }
            else
            {
                ++currentState;
            }
            manager.StopAllCoroutines();
            ConfigureStates();
            manager.StartCoroutine(states[currentState]);
        }

        protected void TransitionState(int newState)
        {
            if(newState == -1)
            {
                //throw to manager
                manager.StopAllCoroutines();
                Debug.Log("PANIC");
            }
            manager.StopAllCoroutines();
            ConfigureStates();
            currentState = newState;
            manager.StartCoroutine(states[Mathf.Clamp(newState, 0, states.Length - 1)]);
            //Debug.Log("TRANSITION TO " + newState);
        }

        public void EvaluateTransitions()
        {
            
            for(int i = 0; i < transitions.Length; ++i)
            {
                AITransition transition = transitions[i];
               
                if (transition.currentState != currentState)
                    continue;

                bool evaluation = AIStates.CheckBoleanCondition(transition, blackboardReference, manager.components);
                
                if (evaluation)
                {
                    
                    TransitionState(transition.jumpIndex);
                    break;
                }
            }
        }

        protected abstract void ConfigureStates();
    }
}

