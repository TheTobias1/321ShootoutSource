using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SquadAI
{
    public class CoverPoint : MonoBehaviour
    {
        public LayerMask rayMask;

        public GameObject player;
        public AIController occupier;
        private bool canSeePlayer;

        public bool Occupied
        {
            get
            {
                return occupier != null;
            }
            set
            {
                if (value == false)
                {
                    occupier = null;
                }
            }
        }

        public bool CanSeePlayer
        {
            get
            {
                return canSeePlayer;
            }
        }

        public void SetPlayer(GameObject p)
        {
            player = p;
        }

        public bool TraceToPlayer()
        {

            if (player == null)
            {
                canSeePlayer = false;
                return false;
            }
            

            Vector3 dir = player.transform.position - transform.position;
            RaycastHit hit;

            if(Physics.Raycast(transform.position, dir, out hit, 70, rayMask))
            {
                if(hit.collider.tag == "Player")
                {
                    canSeePlayer = true;
                    return true;
                }
            }

            canSeePlayer = false;
            return false;
        }
    }
}

