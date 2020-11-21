using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

namespace SquadAI
{
    public class FlankCalculator
    {
        private MonoBehaviour master;

        private struct RaycastResult
        {
            public RaycastHit[] result;
        }

        private struct RoomBuffer
        {
            public Vector3[] waypoints;
        }

        private struct Solution
        {
            public Vector3[] waypoints;
            public float rating;
            public int complete;
        }

        public delegate void CalculateFlank(Vector3[] flank);
        public CalculateFlank OnCalculateFlank;

        private Vector3 startPosBuffer;
        private Vector3 targetPosBuffer;

        private RoomBuffer[] boundaryBufferA;
        private RoomBuffer[] boundaryBufferB;

        private Solution solutionBufferA;
        private Solution solutionBufferB;

        private bool raycasting;
        private bool[] calculatingSolution = new bool[2];
        private bool working;

        private enum FlankType {Vertical = 0, Horizontal = 1}

        private const int starFlankThreshold = 35;
        private float raycastRange;
        private float idealDistance;
        private float idealAngle;

        private LayerMask mask;

        public bool Working
        {
            get { return working; }
        }

        public FlankCalculator(Vector3 start, Vector3 goal, float range, float targetDist, float targetAngle, LayerMask m, MonoBehaviour callingObject)
        {
            working = true;
            startPosBuffer = new Vector3(start.x, start.y, start.z);
            targetPosBuffer = new Vector3(goal.x, start.y, goal.z);
            raycastRange = range;
            idealDistance = targetDist;
            idealAngle = targetAngle;
            master = callingObject;
            mask = m;

            callingObject.StartCoroutine(FindFlankRoute());
        }

        IEnumerator FindFlankRoute()
        {
            FlankType typeOfFlank = CalculateFlankType(CondensePosition(startPosBuffer), CondensePosition(targetPosBuffer));

            bool horizontal = typeOfFlank == FlankType.Horizontal;
           
            raycasting = true;

            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();

            master.StartCoroutine(BoxFlankRaycast(targetPosBuffer, startPosBuffer, horizontal));

            while(raycasting)
            {
                yield return null;
            }

            Thread solAThread = new Thread(() => CalculateSolution(0, boundaryBufferA));
            Thread solBThread = new Thread(() => CalculateSolution(1, boundaryBufferB));
            calculatingSolution[0] = true;
            calculatingSolution[1] = true;

            solAThread.Start();
            solBThread.Start();

            while(calculatingSolution[0] && calculatingSolution[1])
            {
                yield return null;
            }

            if(OnCalculateFlank != null)
            {
                if ((solutionBufferA.rating < solutionBufferB.rating && solutionBufferA.complete == 2) || solutionBufferB.complete != 2)
                {
                    OnCalculateFlank(solutionBufferA.waypoints);
                }
                else
                {
                    OnCalculateFlank(solutionBufferB.waypoints);
                }

                OnCalculateFlank = null;
            }
            working = false;

        }

        private FlankType CalculateFlankType(Vector2 playerPosition, Vector2 startingPosition)
        {
            int t = 0;

            Vector2 attackDirection = playerPosition - startingPosition;
            float attackAngle = Vector2.Angle(Vector2.up, attackDirection);

            if(!Between(attackAngle, 45, 135))
            {
                t = 1;
            }

            return (FlankType)t;
        }


        public IEnumerator BoxFlankRaycast(Vector3 playerPosition, Vector3 startingPosition, bool isHorizontal)
        {
            RaycastResult[] bufferA = new RaycastResult[4];

            Vector3 rayDirection;
            Vector3 attackDirection = Vector3.Normalize(playerPosition - startingPosition);

            if(isHorizontal)
            {
                rayDirection = Vector3.right;
            }
            else
            {
                rayDirection = Vector3.forward;
            }

            //waypoint 1
            bufferA[0].result = SearchBoundaries(startingPosition, rayDirection - attackDirection, rayDirection);
            bufferA[2].result = SearchBoundaries(startingPosition, -rayDirection - attackDirection, -rayDirection);

            yield return null;

            //waypoint 2
            bufferA[1].result = SearchBoundaries(playerPosition, rayDirection, -rayDirection);
            bufferA[3].result = SearchBoundaries(playerPosition, -rayDirection, rayDirection);

            //Debug.Log("FLANK RESULTS");

            foreach(RaycastResult _r in bufferA)
            {
                foreach(RaycastHit _h in _r.result)
                {
                    //Debug.Log("Hit: " + _h.collider.gameObject.name);
                }
            }

            yield return null;

            boundaryBufferA = new RoomBuffer[2];
            boundaryBufferB = new RoomBuffer[2];

            boundaryBufferA[0].waypoints = new Vector3[bufferA[0].result.Length];
            boundaryBufferA[1].waypoints = new Vector3[bufferA[1].result.Length + 1];
            boundaryBufferA[1].waypoints[bufferA[1].result.Length] = targetPosBuffer;

            boundaryBufferB[0].waypoints = new Vector3[bufferA[2].result.Length];
            boundaryBufferB[1].waypoints = new Vector3[bufferA[3].result.Length + 1];
            boundaryBufferB[1].waypoints[bufferA[3].result.Length] = targetPosBuffer;

            yield return null;

            for(int i = 0; i < 4; ++i)
            {
                if(i < 2)
                {
                    for(int j = 0; j < bufferA[i].result.Length; ++j)
                    {
                        boundaryBufferA[i].waypoints[j] = bufferA[i].result[j].collider.transform.position;
                        boundaryBufferA[i].waypoints[j].y = startingPosition.y;
                    }
                }
                else
                {
                    for (int j = 0; j < bufferA[i].result.Length; ++j)
                    {
                        boundaryBufferB[i-2].waypoints[j] = bufferA[i].result[j].collider.transform.position;
                        boundaryBufferB[i-2].waypoints[j].y = startingPosition.y;
                    }
                }
                yield return null;
            }

            startPosBuffer = startingPosition;
            targetPosBuffer = playerPosition;

            raycasting = false;
        }

        private void CalculateSolution(int bufferIndex, RoomBuffer[] boundaryBuffer)
        {

            //pick waypoint 1 from bufferA
            Solution solA = new Solution();
            solA.complete = 0;

            float closestToSpot = raycastRange;
            Vector3 waypoint1A = new Vector3();

            foreach(Vector3 way in boundaryBuffer[0].waypoints)
            {
                float dist = Vector3.Distance(startPosBuffer, way);
                dist = Mathf.Abs(idealDistance - dist);
                if (dist < closestToSpot)
                {
                    waypoint1A = way;
                    closestToSpot = dist;
                    solA.complete = 1;
                }
             
            }

            solA.rating = closestToSpot;
            //pick waypoint 2 from buffer A
            Vector3 direction = startPosBuffer - waypoint1A;

            closestToSpot = 360;
            Vector3 waypoint2A = new Vector3();

            foreach (Vector3 way in boundaryBuffer[1].waypoints)
            {
                float a = Vector3.Angle(direction, way - waypoint1A);
                a = Mathf.Abs(a - idealAngle);
                if (a < closestToSpot)
                {
                    waypoint2A = way;
                    closestToSpot = a;
                }
                if(solA.complete == 1)
                    solA.complete = 2;
            }

            solA.rating += closestToSpot;
            solA.waypoints = new Vector3[] { waypoint1A, waypoint2A };

            if (bufferIndex == 0)
                solutionBufferA = solA;
            else
                solutionBufferB = solA;
            calculatingSolution[bufferIndex] = false;
        }

        RaycastHit[] SearchBoundaries(Vector3 position, Vector3 direction)
        {
            RaycastHit[] hits = Physics.RaycastAll(position, direction, raycastRange, mask);
            //Debug.Log("SEARCHING:" + hits.Length);
            //Debug.Log("pos: " + position);
            //Debug.Log(hits);
            return hits;
        }
        RaycastHit[] SearchBoundaries(Vector3 position, Vector3 direction, Vector3 failDirection)
        {
            RaycastHit[] b1 = Physics.RaycastAll(position, direction, raycastRange, mask);
            //Debug.Log("SEARCHING:" + b1.Length);
           // Debug.Log("pos: " + position);
            //Debug.Log(b1);
            if(b1.Length <= 1)
            {
                return Physics.RaycastAll(position, failDirection, raycastRange, mask);
            }

            return b1;
        }



        private bool Between(float x, float min, float max)
        {
            return x > min && x < max;
        }

        private Vector2 CondensePosition(Vector3 x)
        {
            return new Vector2(x.x, x.z);
        }
    }
}

