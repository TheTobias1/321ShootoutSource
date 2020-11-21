using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using InputSystem;

public class PlayerWeaponManager : MonoBehaviour
{
    protected PlayerHealth playerHealth;
    public PlayerManager manager;
    public Transform spawner;
    public Weapon[] weapons = new Weapon[2];
    public Weapon HealingGun;

    private int currentWeapon = 0;
    [SerializeField]
    private bool switchingWeapons;
    private bool locked;

    public static event Action OnPickedUpWeapon;

    // Start is called before the first frame update
    void Start()
    {
        if(weapons[1] != null)
        {
            weapons[1].gameObject.SetActive(false);
        }
        weapons[0].gameObject.SetActive(true);
        weapons[0].TakeOutWeapon();

        playerHealth = GetComponentInParent<PlayerHealth>();

        PlayerManager.OnNearPickup += OnNearPickup;

        if (PowerupManager.GetPowerupStatus(Powerups.MaxHealthPacks))
        {
            HealingGun.ammunition.SetAmmo(3);
        }
        else
        {
            HealingGun.ammunition.SetAmmo(0);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        PlayerInputClass inputState = manager.PlayerInputState;
        HealingGun.ammunition.UpdateHealthPackAmmo();

        if (inputState != null)
        {
            if(inputState.Switch)
            {
                if (HealingGun.gameObject.activeSelf)
                    HealingGun.Shoot();
                else if(!switchingWeapons)
                    SwitchWeapon();
            }

            if(inputState.Swap)
            {
                WeaponPickup p = manager.GetPickup();
                if (!HealingGun.gameObject.activeSelf && p != null)
                {
                    int c = p.AmmoInClip;
                    int a = p.ammo;
                    Debug.Log("Swapping: (" + c.ToString() + "," + a.ToString() + ")");

                    if(p.nonreloadable && p.worldSpawned && PowerupManager.GetPowerupStatus(Powerups.ExtraAmmo))
                    {
                        c += Mathf.CeilToInt((float)c / 4);
                    }

                    bool taken = SwapWeapon(p.weaponType, p.transform, a, c);
                    if (taken)
                    {
                        p.TakePickup();
                    }

                }
            }

            if(inputState.Heal && HealingGun.ammunition.remainingAmmo > 0 && playerHealth.currentHealth < playerHealth.maxHealth - 1)
            {
                if(!switchingWeapons)
                    SwitchWeapon(HealingGun, GetCurrentWeapon());
            }
        }
    }

    public void SwitchWeapon()
    {
        int newWeaponIndex = (currentWeapon == 0) ? 1 : 0;
        Weapon newWeapon = weapons[newWeaponIndex];
        Weapon oldWeapon = weapons[currentWeapon];
        if (newWeapon != null && !switchingWeapons)
        {
            switchingWeapons = true;
            StartCoroutine(SwitchWeaponSequence(newWeapon, oldWeapon));
            currentWeapon = newWeaponIndex;
        }
    }

    public void SwitchWeapon(Weapon newWeapon, Weapon oldWeapon)
    {
        if(newWeapon != null && oldWeapon != null && (!switchingWeapons || HealingGun.gameObject.activeSelf))
        {
            switchingWeapons = true;
            StartCoroutine(SwitchWeaponSequence(newWeapon, oldWeapon));
        }
            
    }

    IEnumerator SwitchWeaponSequence(Weapon newWeapon, Weapon oldWeapon)
    {
        float putAwayTime = oldWeapon.PutAwayWeapon();
        yield return new WaitForSeconds(putAwayTime);

        newWeapon.gameObject.SetActive(true);
        float takeOutTime = newWeapon.TakeOutWeapon();
        yield return new WaitForSeconds(takeOutTime);

        switchingWeapons = false;
    }

    public bool SwapWeapon(WeaponNames newWeapon, Transform pickup, int totalAmmo, int clipAmmo = -1)
    {
        WeaponPrefab newPrefab = PlayerWeaponLoader.GetWeapon(newWeapon);
        Weapon old = weapons[currentWeapon];
        int slot = (currentWeapon == 0) ? 1 : 0;

        if (!old.Ready || !old.gameObject.activeSelf || switchingWeapons || newPrefab.weaponPrefab == null || old.gunLocked)
            return false;

        if (weapons[1] != null)
        {
            if (weapons[slot] != null)
                slot = currentWeapon;

            old = weapons[slot];
            switchingWeapons = true;

            Vector2Int dropAmmo = new Vector2Int();
            dropAmmo.x = old.GetAmmo();
            dropAmmo.y = old.ammunition.AmmoLeft();

            if(dropAmmo.magnitude > 0)
                SpawnDrop(old.thisWeapon, pickup, dropAmmo);

            StartCoroutine(SwapWeaponSequence(newPrefab, old, (slot == currentWeapon), slot, clipAmmo, totalAmmo));
        }
        else
        {
            Weapon instance = Instantiate(newPrefab.weaponPrefab, transform.position, transform.rotation, transform);
            weapons[1] = instance;
            instance.SetAmmo(clipAmmo);
            instance.ammunition.SetAmmo(totalAmmo);
            instance.gameObject.SetActive(false);
            SwitchWeapon();
        }

        if(OnPickedUpWeapon != null)
            OnPickedUpWeapon();
        return true;
    }

    void SpawnDrop(WeaponNames weaponDrop, Transform spawn, Vector2Int ammo)
    {
        GameObject drop = PlayerWeaponLoader.GetWeapon(weaponDrop).dropPrefab;
        drop = Instantiate(drop, spawn.position, spawn.rotation);
        drop.GetComponent<WeaponPickup>().SetAmmo(ammo);
    }

    IEnumerator SwapWeaponSequence(WeaponPrefab newPrefab, Weapon oldWeapon, bool destroyOld, int newIndex, int clipAmmo, int ammo)
    {
        //make old disappear
        float putAwayTime = oldWeapon.PutAwayWeapon();
        yield return new WaitForSeconds(putAwayTime);

        if (destroyOld)
            Destroy(oldWeapon);
        else
            oldWeapon.gameObject.SetActive(false);

        //spawn new
        Weapon instance = Instantiate(newPrefab.weaponPrefab, transform.position, transform.rotation, transform);
        float takeOutTime = instance.TakeOutWeapon();
        weapons[newIndex] = instance;
        instance.SetAmmo(clipAmmo);
        instance.ammunition.SetAmmo(ammo);
        
        yield return new WaitForSeconds(takeOutTime);

        switchingWeapons = false;
    }

    public Weapon GetCurrentWeapon()
    {
        return weapons[currentWeapon];
    }

    public void OnNearPickup(WeaponPickup pickup)
    {
        if (switchingWeapons)
            return;

        if(weapons[0] != null && weapons[0].thisWeapon == pickup.weaponType)
        {
            if(weapons[0].ammunition != null)
            {
                if (!pickup.nonreloadable)
                    weapons[0].ammunition.TakeAmmo(-pickup.TotalAmmo);
                else
                    weapons[0].AddAmmoToClip(pickup.AmmoInClip);

                pickup.ammo = 0;
                Destroy(pickup.gameObject);
            }
        }
        else if (weapons[1] != null && weapons[1].thisWeapon == pickup.weaponType)
        {
            if (weapons[1].ammunition != null)
            {
                if (!pickup.nonreloadable)
                    weapons[1].ammunition.TakeAmmo(-pickup.TotalAmmo);
                else        
                    weapons[1].AddAmmoToClip(pickup.AmmoInClip);

                pickup.ammo = 0;
                Destroy(pickup.gameObject);
            }
        }
    }
}
