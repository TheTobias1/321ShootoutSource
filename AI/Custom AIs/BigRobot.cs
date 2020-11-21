using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SquadAI;
using RootMotion.FinalIK;

public class BigRobot : CustomAIController
{
    public Animator anim;
    public CharacterController controller;
    public Weapon secondWeapon;
    public Weapon eyeWeapon;
    public AimIK[] aimControllers;

    private Transform targeter;

    bool canSeePlayer;
    bool lostPlayer = true;
    float loseTime;
    bool inRange = false;

    private float desiredWeight;
    private float curWeight;
    private bool linedUp;

    public AudioSource stepSource;

    protected override void Awake()
    {
        Player = GameObject.FindGameObjectWithTag("Player");


        components.navigation.updatePosition = false;
        components.navigation.updateRotation = false;

        GameObject t = new GameObject("Targeting");
        t.transform.parent = transform;
        targeter = t.transform;
        foreach (AimIK ik in aimControllers)
        {
            ik.solver.target = targeter;
            ik.solver.IKPositionWeight = 1;
        }

        graceStamp = Time.time + 3;
    }

    protected override void Update()
    {
        components.navigation.nextPosition = transform.position;
        canSeePlayer = LineOfSight(1, 1, 1) && Time.time > graceStamp;

        bool shoot = false;
        bool tooClose = false;

        if(!lostPlayer)
        {
            if(canSeePlayer)
            {
                loseTime = Time.time + 6f;
                lostPlayer = false;
            }
            else
            {
                if(loseTime > Time.time)
                {
                    lostPlayer = true;
                }
            }

            Vector3 center = transform.position + new Vector3(0, 1, 0);
            Vector3 playerDir = Player.transform.position - center;
            float angle = Vector3.Angle(transform.forward, playerDir);

            if(!linedUp)
            {
                RotateToPlayer();

                if(angle < 15)
                {
                    linedUp = true;
                }
            }
            else
            {
                float d = Vector3.Distance(transform.position, Player.transform.position);
                if (d < 12f)
                {
                    inRange = true;
                }

                if(d < 2.5f && Time.time > graceStamp + 2)
                {
                    tooClose = true;
                }

                if(!inRange)
                {
                    MoveToPlayer();
                }
                else
                {
                    Stand();
                }

                shoot = true;


                if (angle > 50)
                {
                    linedUp = false;
                }
            }
        }
        else
        {
            linedUp = false;
            inRange = false;

            if (canSeePlayer)
            {
                lostPlayer = false;
            }

            MoveToPlayer();
        }

        if(shoot)
        {
            Vector3 pos = Player.transform.position;
            pos.y -= 0.5f;
            targeter.transform.position = Vector3.Lerp(targeter.transform.position, pos, Time.deltaTime * 14);
        }
        else
        {
            Vector3 pos = transform.position + transform.forward * 30;
            pos.y = Player.transform.position.y-0.5f;
            targeter.transform.position = Vector3.Lerp(targeter.transform.position, pos, Time.deltaTime * 14);
        }

        components.weapon.Shooting = shoot;
        if (secondWeapon != null)
            secondWeapon.Shooting = shoot;

        if(eyeWeapon != null)
        {
            eyeWeapon.transform.LookAt(Player.transform.position);
            eyeWeapon.Shooting = tooClose;
        }
    }

    void MoveToPlayer()
    {
        components.navigation.SetDestination(Player.transform.position);

        anim.SetBool("Walk", true);
        components.navigation.updateRotation = true;
        components.navigation.isStopped = false;
        anim.SetBool("Backup", false);
    }

    void RotateToPlayer()
    {
        components.navigation.isStopped = true;

        Vector3 playerDir = Player.transform.position - transform.position;
        float angle = Vector3.SignedAngle(transform.forward, playerDir, Vector3.up);

        anim.SetFloat("Turn", angle);
        anim.SetBool("Walk", false);
        anim.SetBool("false", true);
    }

    void Stand()
    {
        components.navigation.isStopped = true;
        anim.SetBool("Walk", false);
        anim.SetFloat("Turn", 0);
        anim.SetBool("false", true);
    }

    void Backup()
    {
        components.navigation.isStopped = true;

        anim.SetBool("Walk", false);
        anim.SetBool("Backup", true);


    }

    private void OnAnimatorMove()
    {
        Vector3 vel = anim.deltaPosition;
        vel.y = -9;
        controller.Move(vel);
        transform.Rotate(anim.angularVelocity, Space.World);
    }

    public void Step()
    {
        stepSource.PlayOneShot(stepSource.clip);
    }

    /*protected bool SightCheck()
    {
        Vector3 center = transform.position + new Vector3(0, 1, 0);
        Vector3 playerDir = Player.transform.position - center;
        float angle = Vector3.Angle(transform.forward, playerDir);
        if (angle > 130)
            return false;

        return LineOfSight();
    }*/
}
