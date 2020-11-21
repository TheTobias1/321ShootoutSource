using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace SquadAI
{

    //[System.Serializable]
    public class AIBlackboard
    {
        public GameObject player;

        private static float playerSpotTime;
        public static bool PlayerSpotted
        {
            get
            {
                return AIBlackboard.playerSpotTime > Time.time;
            }
            set
            {
                if(!PlayerSpotted)
                {
                    //AIManager.SquadManager.OnPlayerSpotted();
                }

                if (value)
                {
                    playerSpotTime = Time.time + 15;
                }
            }
        }

        public static Vector3 playerSuspectedPosition;


        public float closeAttackRange = 30;
        private bool canSeePlayer;
        private bool lineOfSight;
        private float loseTime;
        public float timeBeforeLost;
        public bool hasSeenPlayer;
        public float distanceToPlayer;
        public bool canBackUp;
        private float dangerTime;
        private bool waiting;
        public bool redundant;
        private float critSpotTime;
        public bool InCriticalSpot
        {
            set
            {
                critSpotTime = Time.time + 0.2f;
            }
            get
            {
                return Time.time < critSpotTime;
            }
        }

        public Transform squadLeader;

        private CoverPoint cover;
        private CoverPoint antiCover;

        private Vector3 lastSeenPosition;
        private Vector3 lastSuspectedPosition;

        [HideInInspector]
        private List<AIBlackboard> companions = new List<AIBlackboard>();

        public List<AIBlackboard> Companions
        {
            get
            {
                return companions;
            }
        }

        public bool Waiting
        {
            get
            {
                return waiting;
            }
            set
            {
                waiting = value;
            }
        }

        public float AttackRange
        {
            get
            {
                return closeAttackRange;
            }
            set
            {
                closeAttackRange = value;
            }
        }

        public CoverPoint BestCover
        {
            get
            {
                return cover;
            }
            set
            {
                if(cover != null)
                    cover.Occupied = false;
                cover = value;
            }
        }

        public CoverPoint BestAntiCover
        {
            get
            {
                return antiCover;
            }
            set
            {
                if(antiCover != null)
                    antiCover.Occupied = false;
                antiCover = value;
            }
        }

        public bool CanSeePlayer
        {
            get
            {
                return canSeePlayer;
            }
            set
            {
                if(value && !canSeePlayer)//on see, write to 
                {
                    if(companions.Count > 0)
                    {
                        foreach(AIBlackboard a in Companions)
                        {
                            a.Danger = true;
                        }
                    }
                }
                if(value)
                {
                    AIBlackboard.PlayerSpotted = true;
                    lastSeenPosition = player.transform.position;
                    lastSuspectedPosition = player.transform.position;
                }
                else if(canSeePlayer)
                {
                    loseTime = Time.time + timeBeforeLost;
                }
                else if(!LostPlayer)
                {
                    lastSuspectedPosition = player.transform.position;
                    playerSuspectedPosition = player.transform.position;
                }
                canSeePlayer = value;

            }
        }

        public bool Danger
        {
            get
            {
                return dangerTime > Time.time;
            }
            set
            {
                if (value)
                    dangerTime = Time.time + 3;
                else
                    dangerTime = 0;
            }
        }

        public bool LostPlayer
        {
            get
            {
                return !canSeePlayer && loseTime < Time.time;
            }
        }

        public bool LineOfSight
        {
            get
            {
                return lineOfSight;
            }
            set
            {

                lineOfSight = value;

            }
        }

        public bool GoodToShoot
        {
            get
            {
                return CanSeePlayer && !InCriticalSpot && distanceToPlayer < closeAttackRange && !redundant;
            }
        }

        public Transform[] targetBuffer = new Transform[5];
        public Vector3[] positionBuffer = new Vector3[5];

        //companionize 2 blackboards
        public static void Link(AIBlackboard a, AIBlackboard b)
        {
            a.Companions.Add(b);
            b.Companions.Add(a);
        }

        public void ResetCompanions()
        {
            companions.Clear();
        }

    }

    [System.Serializable]
    public class ComponentBlackboard
    {
        public NavMeshAgent navigation;
        public GameObject aiObject;
        public MecanimLocomotion locomotion;
        public Weapon weapon;
        public AIController controller;
        public Health aiHealth;
        public AIStatemachineManager stateMachineManager;
        private Transform weaponSpawner;

        public ActionDelegate OnAdvance;

        public void AimAtPlayer(Transform player)
        {
            if(weaponSpawner != null)
            {
                locomotion.AimAt(player);   
            }
            else
            {
                weaponSpawner = weapon.spawner;
            }
            //Quaternion targetRotation = Quaternion.LookRotation(player.position - weapon.transform.position);
            //weapon.transform.rotation = Quaternion.Slerp(weapon.transform.rotation, targetRotation, 8 * Time.deltaTime);
        }

        public bool  Navigate()
        {
            navigation.avoidancePriority = 70;

            if(navigation.isOnNavMesh)
            {
                if (navigation.isStopped)
                    navigation.isStopped = false;

                locomotion.moveDirection = navigation.desiredVelocity.normalized;
                locomotion.move = true;

                return true;
            }
            return false;
        }

        public void AdvanceState()
        {
            //Debug.Log("NEXT STATE");
            if (OnAdvance != null)
                OnAdvance();
        }

        public void SetWaypoints(Vector3[] ways)
        {
            for(int i = 0; i < ways.Length; ++i)
            {
                controller.Blackboard.positionBuffer[i] = ways[i];
            }
        }

        public void StopNavigating()
        {
            navigation.isStopped = false;
            navigation.avoidancePriority = 30;
            
        }

        public void ShootPlayer()
        {
            locomotion.shootPlayer = true;
        }

        public void StopShootingPlayer()
        {
            locomotion.shootPlayer = false;
        }
    }
}

