using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedActivator : MonoBehaviour
{
    public bool activeState;
    public float waitTime;
    public GameObject target;
    public bool startAutomatically;

    public GameObject next;
    public bool broadcast;

    // Start is called before the first frame update
    void Start()
    {
        if (startAutomatically)
            TriggerActivation();
    }

    public void TriggerActivation()
    {
        Invoke("Activate", waitTime);
    }

    // Update is called once per frame
    void Activate()
    {
        if(target != null)
            target.SetActive(activeState);

        if(next != null)
        {
            if(broadcast)
            {
                next.BroadcastMessage("TriggerActivation");
            }
            else
            {
                next.SendMessage("TriggerActivation");
            }
        }
    }
}
