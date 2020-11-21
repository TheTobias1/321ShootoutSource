using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using RootMotion.FinalIK;

public class MecanimLocomotion : MonoBehaviour
{
    public CharacterController characterController;
    public NavMeshAgent navigation;
    public Animator anim;
    public AimController aim;
    public Transform weaponMuzzle;
    public Transform raycaster;
    public Transform aimTarget;
    public float aimAssistSpeed;

    private bool t;
    public bool turn
    {
        get { return t; }
        set
        {
            if (!value && t)
                anim.SetFloat("Turn", 0);
            t = value;
        }
    }
    public bool move;
    public bool lookWhereMoving;

    public Vector3 moveDirection;
    public Vector3 lookDirection;

    public float speed;
    public float turnSpeed;
    public float angleThreshold = 4;
    private float desiredSpeed;
    private float currentSpeed;

    private float desiredAngle;
    private float currentAngle;
    public float animatorLerpSpeed;

    private float aimSwitchOff;
    private float aimSwitchOn;
    public bool shootPlayer;
    private bool linedUp;
    public Weapon weapon;

    public bool interrupt;
    public bool moveOverride;
    private bool turningOnSpot;

    public Vector3 grenadeOffset;
    public Vector2 grenadeForce;
    public float grenadeDelay;
    public float grenadeCooldown;
    public Rigidbody grenadePrefab;

    // Start is called before the first frame update
    void Start()
    {
        navigation = GetComponent<NavMeshAgent>();
        if(weapon == null)
        {
            weapon = GetComponentInChildren<Weapon>();
        }
        GameObject a = GameObject.FindGameObjectWithTag("AimTarget");
        if (a != null)
            aimTarget = a.transform;
    }
    public bool avoiding;
    // Update is called once per frame
    void Update()
    {
        
        currentAngle = Mathf.Lerp(currentAngle, desiredAngle, animatorLerpSpeed * Time.deltaTime);
        navigation.nextPosition = transform.position;
        avoiding = false;

        if(!move && navigation.desiredVelocity.magnitude > 0.1f)
        {
            avoiding = true;
            MoveWithNav();
        }

        if (move && moveDirection.magnitude > 0)
        {
            if(!interrupt)
                Move();
        }
        else
        {
            if (!moveOverride && !avoiding)
                desiredSpeed = 0;
        }

        if (turn)
        {
            if (!interrupt)
            {
                Turn();
            } 
        }
        else
        {
            if(!turningOnSpot)
                anim.SetFloat("Turn", 0);
        }

        if(Time.time > aimSwitchOff || interrupt)
        {
            aim.target = null;
        }

        weapon.Shooting = shootPlayer && linedUp && !interrupt && currentSpeed < 7.5f;

        if(linedUp)
        {
            if (aimAssistSpeed > 0)
            {
                Quaternion dir = Quaternion.LookRotation(aimTarget.position - raycaster.position);
                raycaster.rotation = Quaternion.Lerp(raycaster.rotation, dir, aimAssistSpeed * Time.deltaTime);
            }
        }

        if(aimAssistSpeed <= 0)
            raycaster.transform.LookAt(aimTarget.position);

        currentSpeed = Mathf.Lerp(currentSpeed, desiredSpeed, 20 * Time.deltaTime);
        anim.SetFloat("Speed", currentSpeed);
    }
    public float aimAngle;
    public void AimAt(Transform player)
    {
        if (aimTarget == null || player == null)
            return;

        bool close = Vector3.Distance(player.transform.position, transform.position) < 6.4f;

        if (Time.time > aimSwitchOff)
            aimSwitchOn = Time.time + 0.3f;

        aimSwitchOff = Time.time + 0.2f;
        if(aim.target == null && !interrupt)
        {
            aim.target = aimTarget;
        }

        Vector3 dir = aimTarget.position - weaponMuzzle.position;
        aimAngle = Vector3.SignedAngle(dir.normalized, weaponMuzzle.forward, Vector3.up);



        if (Mathf.Abs(aimAngle) < 36 || close)
        {
            linedUp = true && aimSwitchOn < Time.time;
        }
        else
        {
            linedUp = false;
        }
        
    }

    private void Move()
    {
        float angle = Vector3.SignedAngle(transform.forward, moveDirection.normalized, Vector3.up);
        desiredAngle = angle;
        bool tos = false;

        float appliedSpeed = speed;
        if (lookWhereMoving)
        {
            lookDirection = moveDirection;
            float a = Vector3.Angle(transform.forward, moveDirection);

            if (a > 45)
            {
                Turn(true);
                appliedSpeed = 0;
                tos = true;
            }
            else if(a > 20 && speed > 4)
            {
                Turn(false);
                anim.SetFloat("Turn", 0);
                appliedSpeed = 3;
            }
            else
            {
                Turn(false);
            }
        }
        else
        {
            anim.SetFloat("WalkAngle", angle);
        }

        desiredSpeed = appliedSpeed;
        turningOnSpot = tos;
    }

    public void Move(Vector4 mov)
    {
        Vector3 moveDir = new Vector3(mov.x, mov.y, mov.z);
        float s = mov.w;
        float angle = Vector3.SignedAngle(transform.forward, moveDir.normalized, Vector3.up);
        desiredAngle = angle;

        float appliedSpeed = s;

        anim.SetFloat("WalkAngle", angle);
        desiredSpeed = appliedSpeed;
    }

    public void ResetWalkAngle()
    {
        anim.SetFloat("WalkAngle", 0);
    }

    private void MoveWithNav()
    {
        float angle = Vector3.SignedAngle(transform.forward, navigation.desiredVelocity.normalized, Vector3.up);
        desiredAngle = angle;
        bool tos = false;
        anim.SetFloat("WalkAngle", angle);

        desiredSpeed = 4;
        turningOnSpot = tos;
    }

    private void Turn(bool mecanim = false)
    {
        float angleToTarget = Vector3.SignedAngle(transform.forward, lookDirection, Vector3.up);
        if (move && !mecanim)//if we're walking, rotate using our transform
        {
            if(lookWhereMoving)
            {
                anim.SetFloat("Turn", 0);
            }
            
            if (Mathf.Abs(angleToTarget) > 3)
            {
                if (angleToTarget > 0)//turn right
                {
                    transform.Rotate(0, turnSpeed * Time.deltaTime, 0);
                }
                else
                {
                    transform.Rotate(0, -turnSpeed * Time.deltaTime, 0);
                }
            }
        }
        else //otherwise rotate using mecanim
        {
            desiredSpeed = 0;
            if(Mathf.Abs(angleToTarget) > angleThreshold)
            {
                float a = Mathf.Lerp(anim.GetFloat("Turn"), Mathf.Clamp(angleToTarget * 2, -90, 90), animatorLerpSpeed * Time.deltaTime);
                anim.SetFloat("Turn", a);
            }  
            else
            {
                float a = Mathf.Lerp(anim.GetFloat("Turn"), 0, animatorLerpSpeed * Time.deltaTime);
                anim.SetFloat("Turn", a);
                //float a = Mathf.Lerp(anim.GetFloat("WalkAngle"), 0, 2 * animatorLerpSpeed * Time.deltaTime);
                //anim.SetFloat("WalkAngle", a);
                /*if (angleToTarget > 0)//turn right
                {
                    transform.Rotate(0, turnSpeed * Time.deltaTime, 0);
                }
                else
                {
                    transform.Rotate(0, -turnSpeed * Time.deltaTime, 0);
                }*/
            }

        }

    }

    private void OnAnimatorMove()
    {
        Vector3 vel = anim.deltaPosition;
        vel.y = -9;
        characterController.Move(vel);
        transform.Rotate(anim.angularVelocity, Space.World);
    }

    public void ThrowGrenade(Transform player)
    {
        interrupt = true;
        StartCoroutine(ThrowGrenadeSequence(player));
    }

    IEnumerator ThrowGrenadeSequence(Transform player)
    {
        Vector3 dir = player.position - transform.position;
        float angleToTarget = Vector3.SignedAngle(transform.forward, dir, Vector3.up) +10;

        while (Mathf.Abs(angleToTarget) > 25)
        {
            float a = Mathf.Lerp(anim.GetFloat("Turn"), Mathf.Clamp(angleToTarget * 2, -90, 90), animatorLerpSpeed * Time.deltaTime);
            anim.SetFloat("Turn", a);
            yield return null;
            angleToTarget = Vector3.SignedAngle(transform.forward, dir, Vector3.up) + 10;
        }

        anim.SetFloat("Turn", 0);

        while (Mathf.Abs(angleToTarget) > 10)
        {
            if (angleToTarget > 0)//turn right
            {
                transform.Rotate(0, turnSpeed * Time.deltaTime, 0);
            }
            else
            {
                transform.Rotate(0, -turnSpeed * Time.deltaTime, 0);
            }

            yield return null;
            angleToTarget = Vector3.SignedAngle(transform.forward, dir, Vector3.up) + 10;
        }

        yield return new WaitForSeconds(0.1f);

        anim.SetTrigger("Grenade");

        yield return new WaitForSeconds(grenadeDelay);

        Vector3 offset = transform.forward * grenadeOffset.z;
        offset += transform.up * grenadeOffset.y;
        offset += transform.right * grenadeOffset.x;

        Vector3 force = player.position - (transform.position + offset);
        force = force.normalized * grenadeForce.x;
        force += Vector3.up * grenadeForce.y;

        yield return null;

        Rigidbody grenadeInstance = Instantiate(grenadePrefab, transform.position + offset, Quaternion.identity);

        grenadeInstance.velocity = force;
        grenadeInstance.angularVelocity = Random.insideUnitSphere.normalized * Random.Range(5, 50);

        if (Vector2.Distance(new Vector2(aimTarget.position.x, aimTarget.position.z), new Vector2(transform.position.x, transform.position.z)) > 12)
        {
            Debug.Log("Extra FORCE");
            grenadeInstance.velocity = new Vector3(force.x, 4, force.z);
        }

        yield return new WaitForSeconds(grenadeCooldown);

        interrupt = false;
    }
}
