using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SquadAI
{
    public class TrailingFollower : MonoBehaviour
    {
        private Transform leader;
        private Transform waypoint;
        private const float lerp = 40;

        private float lastUsed;

        private Vector3[] positions;

        public Transform Waypoint
        {
            get
            {
                return waypoint;
            }
        }

        public static TrailingFollower SpawnTrailer(Transform lead)
        {
            GameObject g = new GameObject("trailingFollower");
            TrailingFollower tk = g.AddComponent<TrailingFollower>();
            
            GameObject way = new GameObject("waypoint");
            way.transform.parent = g.transform;

            Vector3[] positions = new Vector3[]
            {
                lead.transform.position - lead.transform.forward,
                lead.transform.position - lead.transform.forward,
            };

            tk.Init(lead, way.transform, positions);
            
            return tk;
        }


        private void Start()
        {
            InvokeRepeating("CheckRelevance", 10, 10);
        }

        private void CheckRelevance()
        {
            if (lastUsed + 5 < Time.time)
            {
                Kill();
            }
        }

        public void Init(Transform lead, Transform way, Vector3[] pos)
        {
            leader = lead;
            waypoint = way;
            positions = pos;
        }

        public void PositionWaypoint()
        {
            lastUsed = Time.time;

            if (leader == null)
                return;

            positions[0] = leader.transform.position - leader.transform.forward ;
            positions[1] = Vector3.Lerp(positions[1], positions[0], lerp * Time.deltaTime);

            waypoint.transform.position = Vector3.Lerp(waypoint.transform.position, positions[1], lerp * Time.deltaTime);
        }

        public void Kill()
        {
            Destroy(gameObject);
        }
    }
}

