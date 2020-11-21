using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SquadAI
{
    public class SidekickFollower : MonoBehaviour
    {
        private Transform leader;
        private Transform waypoint;
        private const float offset = 2;
        private const float rayLength = 4;
        public LayerMask mask;

        private float lastUsed;
        public Transform Waypoint
        {
            get
            {
                return waypoint;
            }
        }

        public static SidekickFollower SpawnSidekick(Transform lead, LayerMask m)
        {
            GameObject g = new GameObject("sidekickFollower");
            SidekickFollower sk = g.AddComponent<SidekickFollower>();
            
            GameObject way = new GameObject("waypoint");
            way.transform.parent = g.transform;
            sk.mask = m;
            sk.Init(lead, way.transform);
            return sk;
        }


        private void Start()
        {
            InvokeRepeating("CheckRelevance", 10, 10);    
        }

        private void CheckRelevance()
        {
            if(lastUsed + 5 < Time.time)
            {
                Kill();
            }
        }

        public void Init(Transform lead, Transform way)
        {
            leader = lead;
            waypoint = way;
        }

        public void PositionWaypoint()
        {
            lastUsed = Time.time;

            if (leader == null)
                return;

            Vector3 dir = leader.transform.right;

            Vector3 origin = leader.transform.position - leader.transform.forward * 1.5f;

            RaycastHit hit0;
            if (Physics.Raycast(origin, -leader.transform.forward , out hit0, 2f, mask))
            {
                origin = hit0.point + leader.transform.forward * 0.5f;
            }

            RaycastHit hit1;
            if(Physics.Raycast(origin, dir, out hit1, rayLength, mask))
            {
                RaycastHit hit2;
                if (!Physics.Raycast(origin, -dir, out hit2, rayLength, mask))
                {
                    waypoint.position = origin - dir * offset;
                }
                else
                {
                    if(hit1.distance > hit2.distance)
                    {
                        waypoint.position = origin + dir * (hit1.distance - 2);
                    }
                    else
                    {
                        waypoint.position = origin - dir * (hit2.distance - 2);
                    }
                }
            }
            else
            {
                waypoint.position = origin + dir * offset;
            }
        }

        public void Kill()
        {
            Destroy(gameObject);
        }
    }
}

