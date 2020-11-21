using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CriticalSpot : MonoBehaviour
{
    public Dictionary<int, MecanimLocomotion> ais = new Dictionary<int, MecanimLocomotion>();
    public List<MecanimLocomotion> mirror = new List<MecanimLocomotion>();

    public Vector2 extents;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Enemy")
        {
            MecanimLocomotion l = other.GetComponent<MecanimLocomotion>();

            if (l != null)
            {
                int i = other.gameObject.GetInstanceID();
                if (!ais.ContainsKey(i))
                {
                    ais.Add(i, l);
                }
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        int id = other.gameObject.GetInstanceID();
        if (ais.ContainsKey(id))
        {
            MecanimLocomotion loc = ais[id];
            if(!loc.move && Within(loc.transform.position))
            {
                Vector3 dir = loc.moveDirection;
                if (dir.magnitude < 0.1f)
                {
                    dir = loc.transform.forward;
                }

                float angle = Vector3.Angle(dir, transform.forward);
                dir = transform.forward;

                if(angle > 90)
                {
                    dir = -transform.forward;
                }

                dir = 2 * dir + (transform.position - loc.transform.position);
                dir.y = 0;
                dir = dir.normalized;

                Vector4 moveVec = new Vector4(dir.x, dir.y, dir.z, 4);
                loc.moveOverride = true;
                loc.Move(moveVec);
            }
        }
    }

    private bool Within(Vector3 pos)
    {
        if(pos.x > transform.position.x - extents.x)
        {
            if(pos.x < transform.position.x + extents.x)
            {
                if(pos.z > transform.position.z - extents.y)
                {
                    if(pos.z < transform.position.z + extents.y)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Enemy")
        {
            MecanimLocomotion l = other.GetComponent<MecanimLocomotion>();
            l.moveOverride = false;

            if (l != null)
            {
                int i = other.gameObject.GetInstanceID();
                if (ais.ContainsKey(i))
                {
                    ais.Remove(i);
                }
            }
        }
    }
}
