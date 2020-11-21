using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InputSystem;
using Rewired;

public class PlayerMovement : MonoBehaviour
{
    private PlayerManager manager;
    private CharacterController controller;

    private Vector3 currentVelocity;
    private float currentTurn;

    public float speed;
    private float snapThreshold;
    public float acceleration;
    public float decceleration;
    private float stoppingThreshold = 0.2f;

    private float mouseSensitivity;
    private float mouseAcceleration;

    Vector3 inputDirection;
    public Vector3 InputDirection
    {
        get { return inputDirection; }
    }

    public float aimSlowDownAngle;
    public LayerMask aimAssistMask;
    public float aimAssistAmount;
    public float movingAimAssistAmount;
    //target value of aim assist
    public float assistWell;
    public Vector2 aimAssistLerp;
    private float curAimAssist;
    private float maxAimAssistAmount = 0;
    public float hitAngle = 5;
    public float aimAssistSlowSpeed;

    // Start is called before the first frame update
    void Start()
    {
        manager = GetComponent<PlayerManager>();
        controller = GetComponent<CharacterController>();
        snapThreshold = speed - 0.6f;

        if(PlayerPrefs.HasKey("Sensitivity"))
        {
            mouseSensitivity = PlayerPrefs.GetFloat("Sensitivity");
        }
        else
        {
            mouseSensitivity = 4;
            mouseAcceleration = 8;
            PlayerPrefs.SetFloat("Sensitivity", 4);
            PlayerPrefs.Save();
        }

        if (PlayerPrefs.HasKey("Acceleration"))
        {
            mouseAcceleration = PlayerPrefs.GetFloat("Acceleration");
        }
        else
        {
            mouseAcceleration = 35;
            PlayerPrefs.SetFloat("Acceleration", 35);
            PlayerPrefs.Save();
        }

#if UNITY_EDITOR     
#elif UNITY_ANDROID
        mouseSensitivity = mouseSensitivity * 100;
#endif

        PauseSlider.OnSensitivityChange += SensitivityChange;
        PauseSlider.OnAccelerationChange += AccelerationChange;

    }

    public void SensitivityChange(float a)
    {
        mouseSensitivity = a;

#if UNITY_EDITOR
#elif UNITY_ANDROID
        mouseSensitivity = mouseSensitivity * 100;
#endif

        PlayerPrefs.SetFloat("Sensitivity", a);
        PlayerPrefs.Save();
    }

    public void AccelerationChange(float a)
    {
        mouseAcceleration = a;

        PlayerPrefs.SetFloat("Acceleration", a);
        PlayerPrefs.Save();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (manager == null)
            return;

        MovePlayer(Time.fixedDeltaTime);
    }

    void MovePlayer(float updateDeltaTime)
    {
        //get the input
        PlayerInputClass inputState = manager.PlayerInputState;

        Vector2 moveInput = new Vector2();
        float camXinput = 0;
        float moveMag = 0;

        if (inputState != null)
        {
            moveInput = inputState.MoveInput;
            inputDirection.x = moveInput.x;
            inputDirection.y = moveInput.y;
            
            camXinput = inputState.CameraX;

            if (camXinput > maxAimAssistAmount)
                maxAimAssistAmount = camXinput;

            inputDirection.z = camXinput;

            moveMag = moveInput.magnitude;
        }

        //translate input to 3d vector
        Vector3 forwardVector = transform.forward * moveInput.y;
        Vector3 horizontalVector = transform.right * moveInput.x;

        Vector3 moveVector = Vector3.Normalize(forwardVector + horizontalVector);

        //rotate
        float x = camXinput * mouseSensitivity;
        bool dirtyAim = false;
        int multiTarget = 0;
        float utilisedAssist = (moveMag > 0.5f)? movingAimAssistAmount : aimAssistAmount;

#if !UNITY_STANDALONE_WIN
        if (manager.slowDownAim)
        {
            foreach (AimAssistEnemy enemy in manager.enemiesInSights)
            {
                if(multiTarget == 0)
                {
                    multiTarget++;
                    //normalised value of angle to target (bigger when closer)
                    float m = manager.aimSlowDownAngle - Mathf.Abs(enemy.angle);
                    m = m / manager.aimSlowDownAngle;
                    //apply linear slow down
                    float absNewX = Mathf.Min(Mathf.Lerp(Mathf.Abs(x), aimAssistSlowSpeed, m), Mathf.Abs(x));
                    x = (x > 0) ? absNewX : -absNewX;

                    if (Mathf.Abs(x) < maxAimAssistAmount * 0.1f || (x > 0 && enemy.angle > 0) || (x < 0 && enemy.angle < 0))
                    {
                        ++multiTarget;
                        float assist = Mathf.Max(0, Mathf.Lerp(aimAssistAmount, assistWell, m));
                        curAimAssist = Mathf.Lerp(curAimAssist, (enemy.angle > 0) ? assist : -assist, aimAssistLerp.x * updateDeltaTime);
                    }


                }

            }

            /*if(!dirtyAim)
            {
                curAimAssist = Mathf.Lerp(curAimAssist, 0, aimAssistLerp * updateDeltaTime);
            }
            else if(multiTarget == 1)
                x += curAimAssist;*/
        }
        else
        {
            if (Mathf.Abs(curAimAssist) > 0.01f && Mathf.Abs(x) > 0.01f)
                curAimAssist = Mathf.Lerp(curAimAssist, 0, aimAssistLerp.y * updateDeltaTime);
            else
                curAimAssist = 0;
        }

        x += curAimAssist;
#endif

        if (mouseAcceleration > 0)
        {
            if (Mathf.Abs(x) > 0.1f)
            {
                currentTurn = Mathf.Lerp(currentTurn, x, mouseAcceleration * updateDeltaTime);
            }  
            else
                currentTurn = Mathf.Lerp(currentTurn, x, mouseAcceleration * updateDeltaTime/* * 4*/);
        }
        else
            currentTurn = x;

        transform.Rotate(0, currentTurn, 0);
        
        //move
        Vector3 resultingVelocity = currentVelocity;
        resultingVelocity.y = 0;

        float currentSpeed = resultingVelocity.magnitude;

        if(moveMag > 0.15f)
        {
            if(currentSpeed > snapThreshold && (Vector3.Angle(moveVector, currentVelocity) < 90 || moveInput.y > 0.7f))
            {
                resultingVelocity += moveVector * acceleration * 6 * updateDeltaTime;
            }
            else
            {
                resultingVelocity += moveVector * acceleration * updateDeltaTime;
            }
            
        }
        else
        {
            if(currentSpeed < stoppingThreshold)
            {
                resultingVelocity = Vector3.zero;
            }
            else
            {
                resultingVelocity = Vector3.Lerp(resultingVelocity, Vector3.zero, decceleration * updateDeltaTime);
            }
        }

        resultingVelocity = Vector3.ClampMagnitude(resultingVelocity, speed);

        resultingVelocity.y = -10;

        currentVelocity = resultingVelocity;
        MoveController(updateDeltaTime);
    }

    void MoveController(float d)
    {
        controller.Move(currentVelocity * d);
    }

    private void OnDestroy()
    {
        PauseSlider.OnSensitivityChange -= SensitivityChange;
        PauseSlider.OnAccelerationChange -= AccelerationChange;
    }
}
