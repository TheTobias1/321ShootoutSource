using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InputSystem;
using SquadAI;

//Manages the player object
public class PlayerManager : MonoBehaviour
{
    private PlayerInputClass inputState;
    private Camera playerCamera;

    private WeaponPickup currentPickup;
    public delegate void PickupEvent(WeaponPickup pickup);
    public static event PickupEvent OnNearPickup;

    public bool lockCursor;

    [SerializeField]
    private int[] playerInventory = new int[4];

    public List<AimAssistEnemy> enemiesInSights;
    private bool checkingEnemiesInSights;

    public float aimSlowDownAngle;
    public LayerMask aimAssistMask;
    public bool slowDownAim;

    public PlayerInputClass PlayerInputState
    {
        get
        {
            return inputState;
        }
        set
        {
            inputState = value;
        }
    }

    public Camera PlayerCamera
    {
        get
        {
            return playerCamera;
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        inputState = new PlayerInputClass();
        playerCamera = GetComponentInChildren<Camera>();

#if UNITY_EDITOR || UNITY_STANDALONE
        if(lockCursor)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
#endif

    }

    private void Update()
    {
        if (!checkingEnemiesInSights)
        {
            checkingEnemiesInSights = true;
            StartCoroutine(UpdateEnemiesInSight());
        }
    }

    public int TakeAmmo(AmmoType ammoType, int amount)
    {
        int t = (int)ammoType;
        amount = Mathf.Min(amount, playerInventory[t]);

        playerInventory[t] = Mathf.Max(0, playerInventory[t] - amount);
        return amount;
    }

    public void AddAmmo(AmmoType ammoType, int amount)
    {
        int t = (int)ammoType;
        playerInventory[t] += amount;
    }

    public void SetAmmo(AmmoType ammoType, int amount)
    {
        int t = (int)ammoType;
        playerInventory[t] = amount;
    }

    public int GetAmmoRemaining(AmmoType ammoType)
    {
        int t = (int)ammoType;
        return playerInventory[t];
    }

    public WeaponPickup GetPickup()
    {
        return currentPickup;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "WeaponDrop")
        {
            currentPickup = other.gameObject.GetComponent<WeaponPickup>();
            /*if(OnNearPickup != null)
            {
                OnNearPickup(currentPickup);
            }*/
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "WeaponDrop")
        {
            if (OnNearPickup != null)
            {
                if(currentPickup != null)
                    OnNearPickup(currentPickup);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "WeaponDrop")
        {
            currentPickup = null;
        }
    }

    IEnumerator UpdateEnemiesInSight()
    {
        List<AimAssistEnemy> newInSightsList = new List<AimAssistEnemy>();
        bool slowAim = false;
        List<AIController> enemies = AIManager.SquadManager != null? new List<AIController>(AIManager.SquadManager.AISquad) : new List<AIController>();

        foreach (AIController enemy in enemies)
        {
            if (enemy == null)
                continue;

            Vector3 enemyPos = enemy.transform.position + new Vector3(0, 1, 0);
            float a = Vector3.SignedAngle(transform.forward, enemyPos - transform.position, Vector3.up);

            if (Mathf.Abs(a) < aimSlowDownAngle)
            {
                RaycastHit hit;

                if (Physics.Linecast(transform.position, enemyPos, out hit, aimAssistMask))
                {
                    if (hit.collider.tag == "Enemy")
                    {
                        AimAssistEnemy e = new AimAssistEnemy();
                        e.enemy = enemy.gameObject;
                        e.angle = a;

                        newInSightsList.Add(e);
                        slowAim = true;
                        Debug.DrawLine(transform.position, enemyPos, Color.yellow);
                    }

                }
                yield return null;
            }
        }

        slowDownAim = slowAim;
        enemiesInSights = newInSightsList;
        checkingEnemiesInSights = false;
    }

    private void OnDestroy()
    {
        PlayerManager.OnNearPickup = null;
    }
}

public enum AmmoType { Light = 0, Heavy = 1, Special = 2, HealthPacks = 3};

[System.Serializable]
public struct AimAssistEnemy
{
    public GameObject enemy;
    public float angle;
}
