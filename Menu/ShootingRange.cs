using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingRange : MonoBehaviour
{
    public GameObject[] targets;
    Vector3[] positions;
    public GameObject original;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("Tick", 5, 1);

        positions = new Vector3[targets.Length];
        for (int i = 0; i < targets.Length; ++i)
        {
            positions[i] = targets[i].transform.position;
        }
    }

    // Update is called once per frame
    void Tick()
    {
        for(int i = 0; i < targets.Length; ++i)
        {
            if(targets[i] == null)
            {
                targets[i] = Instantiate(original, positions[i], Quaternion.Euler(new Vector3(0, -90, 0)));
                targets[i].SetActive(true);
                break;
            }
        }
    }
}
