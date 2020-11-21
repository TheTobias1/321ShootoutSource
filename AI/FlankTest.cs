using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using SquadAI;

public class FlankTest : MonoBehaviour
{
    public NavMeshAgent agent;
    public GameObject player;
    public LayerMask mask;

    public float TargetDistance;
    public float TargetAngle;

    public int curWaypoint = -1;
    public Vector3[] waypoints;
    public Vector3 startPos;
    bool started;
    // Start is called before the first frame update
    void Start()
    {
        FlankCalculator flanker = new FlankCalculator(transform.position, player.transform.position, 200, TargetDistance, TargetAngle, mask, this);
        flanker.OnCalculateFlank += OnCalculate;
    }

    private void Update()
    {
        if(started)
        {
            Debug.DrawLine(startPos, waypoints[0]);
            Debug.DrawLine(waypoints[0], waypoints[1]);
            if (Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(waypoints[curWaypoint].x, 0 ,waypoints[curWaypoint].z)) < 5)
            {
                curWaypoint++;
                if (curWaypoint >= waypoints.Length)
                    agent.SetDestination(player.transform.position);
                else
                    agent.SetDestination(waypoints[curWaypoint]);
            }
        }
        else if (curWaypoint == 0 && !started)
        {
            agent.SetDestination(waypoints[0]);
            started = true;
        }

    }

    void OnCalculate(Vector3[] ways)
    {
        startPos = transform.position;
        waypoints = ways;
        curWaypoint = 0;
    }
}
