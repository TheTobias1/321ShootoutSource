using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SquadAI;

public class PlacePlayer : MonoBehaviour
{
    public Transform playerSpawn;
    public GameObject player;
    int moved = 0;

    private void FixedUpdate()
    {
        if(moved < 10)
        { 
            MovePlayer();
        }
    }


    void MovePlayer()
    {
        if(player == null)
            player = GameObject.FindGameObjectWithTag("Player");
        

        player.transform.position = playerSpawn.position;
        player.transform.rotation = playerSpawn.rotation;

        //Debug.Log("SPAWNING: d - " + Vector3.Distance(player.transform.position, playerSpawn.transform.position));
       //Debug.Log("a - " + Vector3.Angle(player.transform.forward, playerSpawn.forward));

        if (Vector3.Distance(player.transform.position, playerSpawn.transform.position) < 0.03f && Vector3.Angle(player.transform.forward, playerSpawn.forward) < 1)
        {
            ++moved;
        }
    }


}
