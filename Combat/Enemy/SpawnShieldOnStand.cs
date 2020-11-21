using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SquadAI;

public class SpawnShieldOnStand : MonoBehaviour
{
    public AIController controller;
    public AIBlackboard blackboard;
    public AIStatemachineManager manager;
    private GameObject player;

    public Forcefield forcefieldPrefab;
    private Forcefield currentForcefield;
    public ParticleSystem genertatorEffect;

    public string standState;
    private bool spawned;
    private float spawnTime;
    public float spawnOffset;

    Vector3 forcefieldDirection = new Vector3(0, 1, 0);

    private void Start()
    {
        player = AIManager.SquadManager.player;
        blackboard = controller.Blackboard;
    }


    // Update is called once per frame
    void Update()
    {
        if (player == null)
            return;

        if(!spawned && manager.state == standState && spawnTime < Time.time && !blackboard.redundant)
        {
            OnStand();
            spawned = true;
            spawnTime = Time.time + 10;
        }

        bool validForcefield = Vector3.Angle(forcefieldDirection, player.transform.position - transform.position) < 105 || Time.time < spawnTime;
        if (spawned && manager.state != standState || !validForcefield)
        {
            OnMove();
            spawned = false;
        }
    }

    void OnStand()
    {
        if (player == null)
            return;

        Vector3 pos = transform.position + Vector3.Normalize(player.transform.position - transform.position) * spawnOffset;

        currentForcefield = Instantiate(forcefieldPrefab, pos, Quaternion.identity);
        currentForcefield.transform.LookAt(new Vector3(player.transform.position.x, currentForcefield.transform.position.y, player.transform.position.z));

        forcefieldDirection = currentForcefield.transform.forward;

        if (genertatorEffect != null)
            genertatorEffect.Play();
    }

    void OnMove()
    {
        spawnTime = Time.time + 3;
        if (currentForcefield != null)
        {
            currentForcefield.InitiateFall();
        }

        if (genertatorEffect != null)
            genertatorEffect.Stop();
    }

    private void OnDestroy()
    {
        if(currentForcefield != null)
        {
            currentForcefield.InitiateFall();
        }
    }
}
