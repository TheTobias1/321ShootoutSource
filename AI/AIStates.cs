using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace SquadAI
{
    public class AIStates
    {
        public static IEnumerator CalculatePatrolPath(AIBlackboard blackboard, ComponentBlackboard components, float radius = 80)
        {
            NavMeshAgent navigation = components.navigation;
            MecanimLocomotion locomotion = components.locomotion;
            Transform aiTransform = components.aiObject.transform;

            Collider[] NearbyRooms = Physics.OverlapSphere(aiTransform.position, radius, components.stateMachineManager.flankMask);
            yield return null;
            int[] chosenRooms = new int[4];

            int amount = NearbyRooms.Length;
            chosenRooms[0] = Random.Range(0, amount);

            int i;

            for (i = 1; i < chosenRooms.Length && i < amount; ++i)
            {
                int b = (chosenRooms[i - 1] + Random.Range(0, amount - 1)) % amount - 1;
                b = Mathf.Clamp(b, 0, NearbyRooms.Length - 1);
                chosenRooms[i] = b;
            }


            blackboard.positionBuffer[0] = NearbyRooms[chosenRooms[0]].transform.position;
            blackboard.positionBuffer[1] = NearbyRooms[chosenRooms[1]].transform.position;
            if(amount > 2)
                blackboard.positionBuffer[2] = NearbyRooms[chosenRooms[2]].transform.position;
            if(amount > 3)
                blackboard.positionBuffer[3] = NearbyRooms[chosenRooms[3]].transform.position;

            components.AdvanceState();
        }
        public static IEnumerator CalculateFlank(AIBlackboard blackboard, ComponentBlackboard components)
        {
            Debug.Log("CalculatingFlank");
            components.stateMachineManager.state = "Calculating Flank";
            components.StopNavigating();
            components.locomotion.move = false;
            blackboard.Waiting = false;
            

            yield return null;
            yield return new WaitForEndOfFrame();

            blackboard.positionBuffer[0] = Vector3.zero;

            FlankCalculator flanker = new FlankCalculator(components.controller.currentPosition, blackboard.player.transform.position, 200, 50, 80, components.stateMachineManager.flankMask, components.stateMachineManager);
            flanker.OnCalculateFlank += components.SetWaypoints;

            while(flanker.Working)
            {
                yield return null;
            }

            flanker.OnCalculateFlank -= components.SetWaypoints;

            components.AdvanceState();
        }

        public static IEnumerator FollowLeader(AIBlackboard blackboard, ComponentBlackboard components, float speed, int bufferIndex)
        {
            blackboard.Waiting = false;
            components.StopShootingPlayer();
            SidekickFollower target = SidekickFollower.SpawnSidekick(blackboard.targetBuffer[bufferIndex], components.controller.visionMask);
            NavMeshAgent navigation = components.navigation;
            MecanimLocomotion locomotion = components.locomotion;
            Transform aiTransform = components.aiObject.transform;

            navigation.SetDestination(target.Waypoint.position);

            locomotion.speed = 8;
            locomotion.lookWhereMoving = true;
            locomotion.turn = false;
            locomotion.move = true;

            yield return null;

            while (Vector3.Distance(target.Waypoint.position, aiTransform.position) > 5)
            {
                navigation.SetDestination(target.Waypoint.position);
                components.Navigate();
                target.PositionWaypoint();
                yield return new WaitForSeconds(0.1f);
            }

            locomotion.speed = speed;
            locomotion.lookWhereMoving = true;
            locomotion.turn = false;
            locomotion.move = true;

            yield return null;

            while (true)
            {
                if (Vector3.Distance(target.Waypoint.position, aiTransform.position) > 1)
                {
                    navigation.SetDestination(target.Waypoint.position);
                    target.PositionWaypoint();
                    components.Navigate();

                }
                else
                {
                    components.StopNavigating();
                    locomotion.turn = false;
                    locomotion.move = false;
                }
                yield return null;
            }
        }

        public static IEnumerator TrailLeader(AIBlackboard blackboard, ComponentBlackboard components, float speed, int bufferIndex)
        {
            blackboard.Waiting = false;
            components.StopShootingPlayer();
            TrailingFollower target = TrailingFollower.SpawnTrailer(blackboard.targetBuffer[bufferIndex]);
            NavMeshAgent navigation = components.navigation;
            MecanimLocomotion locomotion = components.locomotion;
            Transform aiTransform = components.aiObject.transform;

            navigation.SetDestination(target.Waypoint.position);

            locomotion.speed = 8;
            locomotion.lookWhereMoving = true;
            locomotion.turn = false;
            locomotion.move = true;

            yield return null;

            while (Vector3.Distance(target.Waypoint.position, aiTransform.position) > 5)
            {
                navigation.SetDestination(target.Waypoint.position);
                components.Navigate();
                target.PositionWaypoint();
                yield return new WaitForSeconds(0.1f);
            }

            locomotion.speed = 3;
            locomotion.lookWhereMoving = false;
            locomotion.turn = true;
            locomotion.move = true;

            yield return null;

            while (true)
            {
                if(Vector3.Distance(target.Waypoint.position, aiTransform.position) > 1)
                {
                    locomotion.lookDirection = -locomotion.moveDirection;
                    navigation.SetDestination(target.Waypoint.position);
                    target.PositionWaypoint();
                    components.Navigate();
                    
                }
                else
                {
                    components.StopNavigating();
                    locomotion.turn = false;
                    locomotion.move = false;
                }
                yield return null;
            }
        }

        public static IEnumerator LookAround(AIBlackboard blackboard, ComponentBlackboard components)
        {
            blackboard.Waiting = true;
            components.StopNavigating();
            Transform aiTransform = components.aiObject.transform;
            Transform playerTransform = blackboard.player.transform;
            MecanimLocomotion locomotion = components.locomotion;

            locomotion.move = false;

            while(true)
            {
                Vector3 direction = new Vector3(Random.Range(-1, 1), 0, Random.Range(-1, 1));

                locomotion.lookDirection = direction;
                locomotion.turn = true;

                int wait = Random.Range(2, 4);
                yield return new WaitForSeconds(wait);
            }
        }

        public static IEnumerator GotoCover(AIBlackboard blackboard, ComponentBlackboard components, float speed, bool strafeShoot, float waitTime, bool antiCover = false)
        {
            components.stateMachineManager.state = "COVER";
            //don't want a new instruction
            blackboard.Waiting = false;
            Debug.Log("Cover");
            components.StopShootingPlayer();

            NavMeshAgent navigation = components.navigation;
            MecanimLocomotion locomotion = components.locomotion;
            Transform aiTransform = components.aiObject.transform;
            Transform playerTransform = blackboard.player.transform;

            Vector3 pos = aiTransform.position;
            CoverPoint cover = antiCover ? blackboard.BestAntiCover : blackboard.BestCover;
            if (cover != null)
            {
                pos = cover.transform.position;
            }
            else
            {
                Debug.Log("NO COVER!!");
            }


            bool validCover = true;

            //don't turn back on player
            float angle = Vector3.Angle(new Vector3(pos.x, 0, pos.z), new Vector3(aiTransform.position.x, aiTransform.position.z));
            if(angle > 100)
            {
                speed = 3;
            }

            navigation.SetDestination(pos);
            locomotion.lookWhereMoving = true;
            locomotion.turn = false;
            locomotion.move = true;

            while (Vector3.Distance(pos, aiTransform.position) > 3.5f && validCover)
            {
                /*if(antiCover)
                {
                    validCover = blackboard.BestAntiCover.GetInstanceID() == cover.GetInstanceID();
                }
                else
                {
                    validCover = blackboard.BestCover.GetInstanceID() == cover.GetInstanceID();
                }*/


                if(strafeShoot)
                {
                    if (!blackboard.LostPlayer && speed < 6)
                    {
                        locomotion.speed = speed;
                        locomotion.turn = true;

                        if(playerTransform != null)
                            locomotion.lookDirection = playerTransform.position - aiTransform.position;

                        locomotion.lookWhereMoving = false;

                        if(blackboard.LineOfSight)
                            components.ShootPlayer();
                        else
                            components.StopShootingPlayer();

                        components.AimAtPlayer(playerTransform);
                    }
                    else
                    {
                        locomotion.speed = speed;
                        //locomotion.turn = true;
                        //locomotion.lookWhereMoving = true;
                        components.StopShootingPlayer();
                    }
                }
                navigation.SetDestination(pos);
                components.Navigate();

                yield return null;

                cover = antiCover ? blackboard.BestAntiCover : blackboard.BestCover;
                if (cover != null)
                    pos = cover.transform.position;
            }

            float leaveTime = Time.time + waitTime;

            locomotion.move = false;
            components.StopNavigating();

            components.stateMachineManager.state = "AT COVER";

            while (leaveTime > Time.time)
            {
                if (blackboard.LineOfSight)
                {
                    locomotion.speed = 0;
                    locomotion.turn = true;

                    if(playerTransform != null)
                        locomotion.lookDirection = playerTransform.position - aiTransform.position;

                    components.ShootPlayer();
                    components.AimAtPlayer(playerTransform);
                }
                else
                {
                    locomotion.lookDirection = aiTransform.forward;
                    locomotion.turn = false;
                    components.StopShootingPlayer();
                }

                yield return null;
            }

            components.AdvanceState();
        }

        public static IEnumerator GotoPosition(AIBlackboard blackboard, int bufferIndex, float speed, ComponentBlackboard components, float radius = 2, float waitTime = 0, bool waiting = false)
        {
            components.stateMachineManager.state = "Going to a place";
            blackboard.Waiting = waiting;
            components.StopShootingPlayer();
            Vector3 pos = blackboard.positionBuffer[bufferIndex];
            NavMeshAgent navigation = components.navigation;
            MecanimLocomotion locomotion = components.locomotion;
            Transform aiTransform = components.aiObject.transform;

            navigation.SetDestination(pos);

            locomotion.speed = speed;
            locomotion.lookWhereMoving = true;
            locomotion.turn = false;
            locomotion.move = true;

            bool legitLocation = pos.magnitude > 0.001f;

            while (Vector3.Distance(pos, aiTransform.position) > radius && legitLocation)
            {
                navigation.SetDestination(pos);
                components.Navigate();
                yield return null;
            }

            locomotion.move = false;
            components.StopNavigating();

            if (waitTime > 0)
                yield return new WaitForSeconds(waitTime);

            components.AdvanceState();
        }

        public static IEnumerator GotoPerson(AIBlackboard blackboard, int bufferIndex, float speed, ComponentBlackboard components, float radius = 2, bool waiting = false)
        {
            blackboard.Waiting = waiting;
            components.StopShootingPlayer();
            Transform target = blackboard.targetBuffer[bufferIndex];
            NavMeshAgent navigation = components.navigation;
            MecanimLocomotion locomotion = components.locomotion;
            Transform aiTransform = components.aiObject.transform;

            navigation.SetDestination(target.position);

            locomotion.speed = speed;
            locomotion.lookWhereMoving = true;
            locomotion.turn = false;
            locomotion.move = true;

            yield return null;

            while (Vector3.Distance(target.position, aiTransform.position) > radius)
            {
                navigation.SetDestination(target.position);
                components.Navigate();
                yield return new WaitForSeconds(0.1f);
            }

            locomotion.move = false;
            components.AdvanceState();
        }

        public static IEnumerator GotoPlayer(AIBlackboard blackboard, float speed, ComponentBlackboard components, bool waiting = false)
        {
            components.stateMachineManager.state = "GOTO";
            blackboard.Waiting = waiting;
            components.StopShootingPlayer();
            NavMeshAgent navigation = components.navigation;
            Transform target = blackboard.player.transform;
            MecanimLocomotion locomotion = components.locomotion;
            Transform aiTransform = components.aiObject.transform;

            navigation.SetDestination(target.position);
            

            yield return null;

            while (true)
            {

                if(blackboard.CanSeePlayer)
                {
                    locomotion.speed = 4;
                    components.ShootPlayer();
                    components.AimAtPlayer(target);
                }
                else
                {
                    locomotion.speed = speed;
                    components.StopShootingPlayer();
                }
                
                locomotion.lookWhereMoving = true;
                locomotion.turn = false;
                locomotion.move = true;

                /*if (AIBlackboard.PlayerSpotted)
                    navigation.SetDestination(target.position);
                else
                    navigation.SetDestination(AIBlackboard.playerSuspectedPosition);*/

                navigation.SetDestination(target.position);
                components.Navigate();

                yield return new WaitForSeconds(0.1f);
            }
        }

        public static IEnumerator StandAndShootPlayer(AIBlackboard blackboard, ComponentBlackboard components)
        {
            components.stateMachineManager.state = "STAND AND SHOOT";
            blackboard.Waiting = true;
            components.StopNavigating();
            Transform aiTransform = components.aiObject.transform;
            Transform playerTransform = blackboard.player.transform;
            MecanimLocomotion locomotion = components.locomotion;

            locomotion.ResetWalkAngle();

            locomotion.move = false;
            locomotion.speed = 0;

            while (true)
            {

                //if (blackboard.distanceToPlayer > 6 || !blackboard.LineOfSight || !blackboard.canBackUp)
                //{
                    if (blackboard.LineOfSight)
                    {
                        locomotion.move = false;
                        locomotion.moveDirection = Vector3.zero;
                        locomotion.turn = true;
                        locomotion.lookDirection = playerTransform.position - aiTransform.position;
                        components.ShootPlayer();
                        components.AimAtPlayer(playerTransform);
                    }
                    else
                    {
                        locomotion.turn = false;
                        components.StopShootingPlayer();
                    }
                    components.StopNavigating();
                //}
                //else if(blackboard.canBackUp)
                /*{
                    Vector3 backUp = locomotion.transform.position - playerTransform.position * 5;
                    backUp.y = locomotion.transform.position.y;

                    locomotion.move = true;
                    locomotion.turn = true;
                    locomotion.lookWhereMoving = false;

                    locomotion.lookDirection = playerTransform.position - aiTransform.position;
                    components.ShootPlayer();
                    components.AimAtPlayer(playerTransform);
                    components.Navigate();

                    components.navigation.SetDestination(backUp);
                }*/
                yield return null;
            }
        }

        public static IEnumerator StandardAttack(AIBlackboard blackboard, ComponentBlackboard components)
        {
            blackboard.Waiting = true;
            Transform aiTransform = components.aiObject.transform;
            Transform playerTransform = blackboard.player.transform;
            MecanimLocomotion locomotion = components.locomotion;
            NavMeshAgent navigation = components.navigation;

            locomotion.move = false;
            locomotion.speed = 3;
            components.StopNavigating();

            while(true)
            {
                float moveTime = Time.time + Mathf.Max(2f, Random.Range(1, 8));
                locomotion.move = false;

                while (moveTime > Time.time)//stand time
                {
                    if(blackboard.distanceToPlayer > 6 || !blackboard.CanSeePlayer)
                    {
                        if (blackboard.CanSeePlayer)
                        {
                            locomotion.move = false;
                            locomotion.moveDirection = Vector3.zero;
                            locomotion.turn = true;
                            locomotion.lookDirection = playerTransform.position - aiTransform.position;
                            components.ShootPlayer();
                            components.AimAtPlayer(playerTransform);
                        }
                        else
                        {
                            locomotion.turn = false;
                            components.StopShootingPlayer();
                        }
                        components.StopNavigating();
                    }
                    else
                    {
                        Vector3 backUp = locomotion.transform.position - playerTransform.position * 5;
                        backUp.y = locomotion.transform.position.y;

                        locomotion.move = true;
                        locomotion.turn = true;
                        locomotion.lookWhereMoving = false;

                        locomotion.lookDirection = playerTransform.position - aiTransform.position;
                        components.ShootPlayer();
                        components.AimAtPlayer(playerTransform);
                        components.Navigate();

                        navigation.SetDestination(backUp);
                    }


                    yield return null;
                }

                CoverPoint originalCover = (Random.Range(0,10) > 3)? blackboard.BestAntiCover : blackboard.BestCover;

                if (originalCover != null)
                {
                    
                    locomotion.move = true;
                    navigation.SetDestination(originalCover.transform.position);

                    moveTime = Time.time + Mathf.Max(1, Random.Range(2, 6));

                    
                    bool coverValid = (originalCover.GetInstanceID() == blackboard.BestAntiCover.GetInstanceID() || originalCover.GetInstanceID() == blackboard.BestCover.GetInstanceID() || Time.time < moveTime);

                    locomotion.turn = true;
                    locomotion.lookWhereMoving = false;

                    while (coverValid && Vector3.Distance(originalCover.transform.position, aiTransform.position) > 2)
                    {
                        components.Navigate();
                        if(blackboard.LineOfSight)
                        {
                            
                            locomotion.lookDirection = playerTransform.position - aiTransform.position;
                            components.ShootPlayer();
                            components.AimAtPlayer(playerTransform);
                        }
                        else
                        {
                            components.StopShootingPlayer();
                        }
                        yield return null;
                    }
                }
                else
                {
                    locomotion.turn = false;
                    locomotion.lookWhereMoving = true;
                    locomotion.move = true;
                    components.StopShootingPlayer();
                    while (Vector3.Distance(playerTransform.position, aiTransform.position) > blackboard.AttackRange || !blackboard.CanSeePlayer)
                    {
                        if(blackboard.CanSeePlayer)
                        {
                            components.AimAtPlayer(playerTransform);
                            components.ShootPlayer();
                        }
                        else
                        {
                            components.StopShootingPlayer();
                        }

                        navigation.SetDestination(playerTransform.position);
                        components.Navigate();
                        yield return null;
                    }
                }
            }
        }

        public static IEnumerator ApproachAttack(AIBlackboard blackboard, ComponentBlackboard components, float speed)
        {
            blackboard.Waiting = true;
            Debug.Log("Attack");
            Transform aiTransform = components.aiObject.transform;
            Transform playerTransform = blackboard.player.transform;
            MecanimLocomotion locomotion = components.locomotion;
            NavMeshAgent navigation = components.navigation;

            locomotion.speed = speed;
            locomotion.turn = false;
            locomotion.move = true;
            components.StopShootingPlayer();
            while (true)
            {
                while (Vector3.Distance(playerTransform.position, aiTransform.position) > blackboard.AttackRange || !blackboard.CanSeePlayer)
                {
                    if (blackboard.CanSeePlayer)
                    {
                        locomotion.lookWhereMoving = false;
                        locomotion.turn = true;
                        locomotion.lookDirection = playerTransform.position - aiTransform.position;
                        components.ShootPlayer();
                        components.AimAtPlayer(playerTransform);
                    }
                    else
                    {
                        locomotion.lookWhereMoving = true;
                        locomotion.turn = false;
                        components.StopShootingPlayer();
                    }

                    navigation.SetDestination(playerTransform.position);
                    components.Navigate();
                    yield return null;
                }

                float r = blackboard.AttackRange + 7;
                float breakTime = 0;
                bool canSee = blackboard.CanSeePlayer;

                while (Vector3.Distance(playerTransform.position, aiTransform.position) < r)
                {
                    if (blackboard.CanSeePlayer)
                    {
                        locomotion.turn = true;
                        locomotion.lookDirection = playerTransform.position - aiTransform.position;
                        components.ShootPlayer();
                        components.AimAtPlayer(playerTransform);
                    }
                    else
                    {
                        if(canSee)
                        {
                            breakTime = Time.time + 2.5f;
                        }
                        else if(Time.time > breakTime)
                        {
                            break;
                        }
                        locomotion.turn = false;
                    }

                    canSee = blackboard.CanSeePlayer;

                    yield return null;
                }
            }
            
        }

        public static bool CheckBoleanCondition(AITransition transitionData, AIBlackboard blackboard, ComponentBlackboard components)
        {
            if(components.locomotion.interrupt)
            {
                return false;
            }

            if (transitionData.condition == AIConditions.CanSeePlayer)
            {
                bool canSee = blackboard.LineOfSight;
                if (!transitionData.inverse)
                    return canSee;
                else
                    return !canSee;
            }
            else if (transitionData.condition == AIConditions.IsCloseToPlayer)
            {
                if (!transitionData.inverse)
                    return blackboard.distanceToPlayer < 13.5f;
                else
                    return blackboard.distanceToPlayer > 13.5f;
            }
            else if (transitionData.condition == AIConditions.InRange)
            {
                if (!transitionData.inverse)
                    return blackboard.distanceToPlayer < blackboard.closeAttackRange;
                else
                    return blackboard.distanceToPlayer > blackboard.closeAttackRange;
            }
            else if (transitionData.condition == AIConditions.GoodShoot)
            {
                if (!transitionData.inverse)
                    return blackboard.GoodToShoot;
                else
                    return !blackboard.GoodToShoot;
            }
            else if(transitionData.condition == AIConditions.IsRedundant)
            {
                if (!transitionData.inverse)
                    return blackboard.redundant;
                else
                    return !blackboard.redundant;
            }
            else if (transitionData.condition == AIConditions.HasPowerSpot)
            {
                if (!transitionData.inverse)
                    return blackboard.BestAntiCover != null;
                else
                    return blackboard.BestAntiCover == null;
            }
            else if (transitionData.condition == AIConditions.HasCover)
            {
                if (!transitionData.inverse)
                    return blackboard.BestCover != null;
                else
                    return blackboard.BestCover == null;
            }
            else if (transitionData.condition == AIConditions.LostPlayer)
            {
                if (!transitionData.inverse)
                    return blackboard.LostPlayer;
                else
                    return !blackboard.LostPlayer;
            }
            else if (transitionData.condition == AIConditions.InDanger)
            {
                bool d = blackboard.CanSeePlayer && !blackboard.GoodToShoot;
                if (!transitionData.inverse)
                    return d;
                else
                    return !d;
            }
            else if (transitionData.condition == AIConditions.PlayerSpotted)
            {
                bool s = AIBlackboard.PlayerSpotted;
                if (!transitionData.inverse)
                    return s;
                else
                    return !s;
            }
            else if(transitionData.condition == AIConditions.Healthy)
            {
                if (components.aiHealth == null)
                    return false;

                bool healthy = components.aiHealth.currentHealth > components.aiHealth.maxHealth / 2;

                if (!transitionData.inverse)
                    return healthy;
                else
                    return !healthy;
            }
            else if (transitionData.condition == AIConditions.HasLeader)
            {
                bool s = blackboard.squadLeader != null;
                if (!transitionData.inverse)
                    return s;
                else
                    return !s;
            }

            return false;
        }

    }

    [System.Serializable]
    public struct AITransition
    {
        public AIConditions condition;
        public bool inverse;
        public int currentState;
        public int jumpIndex;
    }

    public enum AIEmotionalStates { Relaxed, Cautious, Hurry};
    public enum AIConditions { CanSeePlayer, HasPowerSpot, HasCover, LostPlayer, InDanger, PlayerSpotted, Healthy, HasLeader, IsRedundant, GoodShoot, IsCloseToPlayer, InRange };
}

