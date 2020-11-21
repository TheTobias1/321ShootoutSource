using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPack : MonoBehaviour
{
    public AudioClip takeSound;
    HealingGun healingGun;

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            if(healingGun == null)
                healingGun = (HealingGun)other.gameObject.GetComponentInChildren<PlayerWeaponManager>().HealingGun;

            if(healingGun.ammunition.AmmoLeft() < 2)
            {
                if(healingGun.ammunition.remainingAmmo == 1)
                {
                    healingGun.ammunition.SetAmmo(3, false);
                }
                else
                {
                    healingGun.ammunition.SetAmmo(2, false);
                }

                if(takeSound != null)
                {
                    SoundFXPlayer.SFX.PlaySound(takeSound);
                }
                Destroy(gameObject);
            }
            
        }
    }
}
