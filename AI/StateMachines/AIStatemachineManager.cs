using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SquadAI
{
    public class AIStatemachineManager : MonoBehaviour
    {
        public AIController controller;
        public ComponentBlackboard components;
        AIStatemachine currentStatemachine;

        bool hasLeader = false;

        public int currentState;

        public string state;
        public string statemachine;

        private AIBlackboard blackboard;
        public AIBlackboard Blackboard
        {
            get
            {
                return blackboard;
            }
        }

        public LayerMask flankMask;
        public enum EnemyType { Flanker, Defender, Pusher, Simple};
        public EnemyType AIType;

        private void Start()
        {
            blackboard = controller.Blackboard;
            components = controller.components;
            components.OnAdvance += AdvanceState;

            ConfigureAttack();
        }

        public void AdvanceState()
        {
            if (currentStatemachine != null)
            {
                currentStatemachine.AdvanceState();
            }
        }

        public void LoadNewSM(StateMachines selectedMachine)
        {
            StopAllCoroutines();

            switch(selectedMachine)
            {
                case StateMachines.BlitzFlank:
                    ConfigureBlitzFlank();
                    break;
                case StateMachines.Patrol:
                    ConfigurePatrol();
                    break;
                case StateMachines.FrontCover:
                    ConfigureFrontCover();
                    break;
                case StateMachines.BackCover:
                    ConfigureBackCover();
                    break;

                case StateMachines.CautiousApproach:
                    ConfigureCautiousAttack();
                    break;

                case StateMachines.SquadApproach:
                    ConfigureSquadLeader();
                    break;
                default:
                    ConfigureAttack();
                    break;
            }
        }

        public void ReEvaluateAttack(EnemyType newType)
        {
            AIType = newType;
            StopAllCoroutines();
            ConfigureAttack();
        }

        //---------PATROL------
        void ConfigurePatrol()
        {
            if(Random.Range(0, 10) > 5)
            {
                currentStatemachine = new Patrol();
            }
            else
            {
                currentStatemachine = new LookAroundAttack();
            }
            
            currentStatemachine.Init(this);
            currentStatemachine.StartStatemachine();
            statemachine = "patrol";
        }

        //--------Flank---------

        void ConfigureBlitzFlank()
        {
            currentStatemachine = new BlitzFlank();
            currentStatemachine.Init(this);
            currentStatemachine.StartStatemachine();
            statemachine = "Flank ";
        }

        //--------------attacks--------------

        private void ConfigureAttack()
        {
            if(AIType == EnemyType.Defender)
            {
                currentStatemachine = new DeffensiveAssault();
                currentStatemachine.Init(this);
                currentStatemachine.StartStatemachine();
                statemachine = "passive ";
            }
            else if(AIType == EnemyType.Pusher)
            {
                currentStatemachine = new OffensiveAssault();
                currentStatemachine.Init(this);
                currentStatemachine.StartStatemachine();
                statemachine = "pusher ";
            }
            else if(AIType == EnemyType.Simple)
            {
                currentStatemachine = new SimpleAttack();
                currentStatemachine.Init(this);
                currentStatemachine.StartStatemachine();
                statemachine = "simple ";
            }
            else
            {
                currentStatemachine = new FlankAssault();
                currentStatemachine.Init(this);
                currentStatemachine.StartStatemachine();
                statemachine = "flank ";
            }
        }

        private void ConfigureCautiousAttack()
        {
            currentStatemachine = new DefensiveAttack();
            currentStatemachine.Init(this);
            currentStatemachine.StartStatemachine();

            statemachine = "catious attack ";
        }
        
        //----------Squad---------
        private void ConfigureSquadLeader()
        {
            currentStatemachine = new SquadApproach();
            currentStatemachine.Init(this);
            currentStatemachine.StartStatemachine();
            statemachine = "lead ";
        }

        private void ConfigureFrontCover()
        {
            Blackboard.targetBuffer[0] = Blackboard.squadLeader;
            hasLeader = true;
            currentStatemachine = new CoverFollow();
            currentStatemachine.Init(this);
            currentStatemachine.StartStatemachine();

            statemachine = "cover front";
        }

        private void ConfigureBackCover()
        {
            Blackboard.targetBuffer[0] = Blackboard.squadLeader;
            hasLeader = true;
            currentStatemachine = new CoverTrail();
            currentStatemachine.Init(this);
            currentStatemachine.StartStatemachine();

            statemachine = "cover back";
        }

        public void FallOutOfState()
        {
            ConfigurePatrol();
        }

        private void Update()
        {
            if (currentStatemachine != null)
            {
                currentStatemachine.EvaluateTransitions();
                currentState = currentStatemachine.currentState;
            }

            if(Blackboard.squadLeader == null && hasLeader)
            {
                if (Random.Range(0, 10) > 5 || Vector3.Distance(transform.position, Blackboard.player.transform.position) < 17)
                    ConfigureAttack();
                else
                    ConfigureBlitzFlank();

                hasLeader = false;
            }
        }

        

    }

    public enum StateMachines {BlitzFlank, DefensiveAttack, GotoAndAttack, Patrol, SquadApproach, FrontCover, BackCover, CautiousApproach};
}

