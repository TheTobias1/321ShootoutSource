using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Forcefield : MonoBehaviour
{

    public bool riseOnStart;
    public bool destroyOnFall;
    private int state;
    private float regularHeight;

    Health hp;

    // Start is called before the first frame update
    void Start()
    {
        if(riseOnStart)
        {
            InitiateRise();
        }

        hp = GetComponentInChildren<Health>();
        hp.OnDie += OnDie;
    }

    public void InitiateRise()
    {
        regularHeight = transform.localScale.y;
        transform.localScale = new Vector3(transform.localScale.x, 0, transform.localScale.z);
        state = 1;
    }

    public void InitiateFall()
    {
        state = -2;
    }

    // Update is called once per frame
    void Update()
    {
        if(state != 0)
        {
            Rise();
        }
    }

    void Rise()
    {
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y + Time.deltaTime * 2 * state, transform.localScale.z);

        if(transform.localScale.y >= regularHeight && state > 0)
        {
            transform.localScale = new Vector3(transform.localScale.x, regularHeight, transform.localScale.z);
            state = 0;
        }

        else if (transform.localScale.y <= 0 && state < 0)
        {
            transform.localScale = new Vector3(transform.localScale.x, 0, transform.localScale.z);
            if (destroyOnFall)
            {
                Destroy(gameObject);
            }
            state = 0;
        }
    }

    void OnDie(Damage d)
    {
        hp.OnDie += OnDie;
        InitiateFall();
    }
}
