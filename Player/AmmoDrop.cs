using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoDrop : MonoBehaviour
{
    public AmmoType ammoType;
    public int ammoAmount;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            other.gameObject.GetComponent<PlayerManager>().AddAmmo(ammoType, ammoAmount);
            Destroy(gameObject);
        }
    }
}
