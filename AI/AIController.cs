using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace SquadAI
{
    public class AIController : MonoBehaviour
    {
        private AIBlackboard blackboard;
        public ComponentBlackboard components;
        public AIStatemachineManager stateMachine;

        public GameObject Player;
        public Vector3 currentPosition;
        public float intendedRange = 20;

        public LayerMask visionMask;

        public bool graceEnded = false;
        public float graceStamp;

        public AIBlackboard Blackboard
        {
            get
            {
                return blackboard;
            }
        }

        int missedLines = 6;
        public bool HasLineOfSight
        {
            get
            {
                return missedLines < 4;
            }
        }

        private bool checkingRedundancy;

        public bool canThrowGrenade;
        public float timeBeforeLost = 2.5f;

        public bool customAI = false;

        // Start is called before the first frame update
        protected virtual void Awake()
        {
            GameObject p = GetPlayer();

            if(p == null)
            {
                Destroy(gameObject);
                return;
            }

            blackboard = new AIBlackboard();
            blackboard.player = p;
            blackboard.closeAttackRange = intendedRange;
            blackboard.timeBeforeLost = timeBeforeLost;

            components.navigation.updatePosition = false;
            components.navigation.updateRotation = false;
            components.controller = this;
            components.stateMachineManager = stateMachine;

            EnemySpawner.SPAWNING_ENEMIES = false;
            graceStamp = Time.time + 1.3f;
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            if(!graceEnded)
            {
                graceEnded = Time.time > graceStamp;
            }

            int mod = Time.frameCount % 10;
            if(mod == 0)
            {
                bool lineOfSight = LineOfSight();
                if(lineOfSight)
                {
                    missedLines = 0;
                }
                else
                {
                    ++missedLines;
                }

                blackboard.LineOfSight = LineOfSight();

                if (!graceEnded && !blackboard.LineOfSight)
                    graceStamp = Mathf.Max(Time.time + 0.1f, graceStamp);

                blackboard.CanSeePlayer = CanSeePlayer() && graceEnded;
            }
            else if(mod == 1)
            {
                if(blackboard.BestCover != null)
                {
                    Debug.DrawLine(blackboard.BestCover.transform.position, currentPosition, Color.red);
                    if (Vector3.Distance(currentPosition, blackboard.BestCover.transform.position) > 20 || blackboard.BestCover.CanSeePlayer)
                    {
                        blackboard.BestCover.Occupied = false;
                        blackboard.BestCover = null;
                    }
                }

                if(blackboard.BestAntiCover != null)
                {
                    Debug.DrawLine(blackboard.BestAntiCover.transform.position, currentPosition, Color.green);
                    if (Vector3.Distance(currentPosition, blackboard.BestAntiCover.transform.position) > 80 || !blackboard.BestAntiCover.CanSeePlayer)
                    {
                        blackboard.BestAntiCover.Occupied = false;
                        blackboard.BestAntiCover = null;
                    }
                }


            }
            else if(mod == 2)
            {
                blackboard.distanceToPlayer = Vector3.Distance(transform.position, Player.transform.position);
                blackboard.canBackUp = CanBackUp();
            }
            
            currentPosition = transform.position;

            if(!checkingRedundancy)
            {
                checkingRedundancy = true;
                StartCoroutine(CheckRedundancySequence());
            }

            if(Mathf.Abs(Player.transform.position.y - transform.position.y) > 100)
            {
                Destroy(gameObject);
            }
        }

        private GameObject GetPlayer()
        {
            if(Player == null)
            {
                Player = GameObject.FindGameObjectWithTag("Player");
            }

            return Player;
        }


        private bool CanSeePlayer()
        {
            Vector3 center = transform.position + new Vector3(0, 1, 0);
            Vector3 playerDir = Player.transform.position - center;
            float angle = Vector3.Angle(transform.forward, playerDir);
            if (angle > 130 && blackboard.LostPlayer)
                return false;

            return blackboard.LineOfSight;
        }

        protected bool LineOfSight(float o = 0.25f, float backwardsOffset = 0, int rayCount = 2)
        {
            Vector3 center = transform.position + new Vector3(0, 1.5f, 0) - transform.forward * backwardsOffset;
            Vector3 playerPos = Player.transform.position;
            Vector3 offset = Vector3.Cross(playerPos - center, Vector3.up).normalized * o;
            Vector3 origin = center + offset;
            int count = 0;

            RaycastHit hit;

            if (Physics.Raycast(origin, playerPos - origin, out hit, 200, visionMask))
            {
                Debug.DrawLine(origin, hit.point);
                if (hit.collider.tag == "Player")
                {
                    ++count;
                }
            }

            origin = center - offset;
            if (Physics.Raycast(origin, playerPos - origin, out hit, 200, visionMask))
            {
                Debug.DrawLine(origin, hit.point);
                if (hit.collider.tag == "Player")
                {
                    ++count;
                }
            }


            return count >= rayCount;
        }

        private bool CanBackUp()
        {
            Vector3 center = transform.position + new Vector3(0, 1, 0);
            Vector3 playerPos = Player.transform.position;
            Vector3 offset = Vector3.Cross(playerPos - center, Vector3.up).normalized * 0.2f;
            Vector3 origin = center + offset;

            RaycastHit hit;
            if (Physics.Raycast(center, center - playerPos, out hit, 5, visionMask))
            {
                return false;
            }

            return true;
        }

        IEnumerator CheckRedundancySequence()
        {
            List<AIController> squadMates = new List<AIController>(AIManager.SquadManager.AISquad);
            bool redundant = false;

            foreach(AIController ai in squadMates)
            {
                if (ai == null)
                    continue;

                if(ai.gameObject.GetInstanceID() != gameObject.GetInstanceID() && Vector3.Distance(transform.position, ai.transform.position) < 4.5f)
                {
                    if(ai.blackboard.distanceToPlayer > blackboard.distanceToPlayer)
                    {
                        redundant = Time.time > graceStamp + 3.5f;
                        break;
                    }
                }

                yield return null;
            }

            blackboard.redundant = redundant;
            checkingRedundancy = false;
        }

        private void OnTriggerStay(Collider other)
        {
            if(other.tag == "CriticalSpot")
            {
                if(blackboard != null)
                    blackboard.InCriticalSpot = true;
            }
        }

    }
}

