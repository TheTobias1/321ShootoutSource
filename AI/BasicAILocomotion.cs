using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BasicAILocomotion : MonoBehaviour
{
    public NavMeshAgent navigation;

    public bool turn;
    public bool move;
    public bool lookWhereMoving;

    public Vector3 moveDirection;
    public Vector3 lookDirection;

    public float speed;
    public float turnSpeed;
    public float angleThreshold = 4;

    // Start is called before the first frame update
    void Start()
    {
        navigation = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        navigation.nextPosition = transform.position;
        if (move)
        {
            Move();
        }
        if(turn)
        {
            Turn();
        }
    }

    private void Move()
    {
        float appliedSpeed = speed;
        if(lookWhereMoving)
        {
            lookDirection = moveDirection;
            Turn();

            if(Vector3.Angle(transform.forward, moveDirection) > 45)
            {
                appliedSpeed = 0;
            }
        }
        transform.Translate(moveDirection.normalized * appliedSpeed * Time.deltaTime, Space.World);
    }

    private void Turn()
    {
        float angleToTarget = Vector3.SignedAngle(transform.forward, lookDirection, Vector3.up);

        if(Mathf.Abs(angleToTarget) > angleThreshold)
        {
            if(angleToTarget > 0)//turn right
            {
                transform.Rotate(0, turnSpeed * Time.deltaTime, 0);
            }
            else
            {
                transform.Rotate(0, -turnSpeed * Time.deltaTime, 0);
            }
        }
    }
}
