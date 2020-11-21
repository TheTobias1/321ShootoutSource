using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeHUDTracker : MonoBehaviour
{
    GameObject player;
    HudManager HUD;
    public float warningRange = 9;
    // Start is called before the first frame update
    void Start()
    {
        HUD = HudManager.HUD;
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
            return;

        if(Vector3.Distance(transform.position, player.transform.position) < warningRange)
        {
            HUD.ActivateGrenade(transform.position);
        }
    }
}
