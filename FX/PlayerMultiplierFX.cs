using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMultiplierFX : MonoBehaviour
{
    public ParticleSystem effect;
    // Start is called before the first frame update
    void Start()
    {
        CashManager.OnMultiplierChange += MultiplierChange;
    }

    public void MultiplierChange(int m)
    {
        if(m == 3)
        {
            effect.Play();
        }
        else
        {
            effect.Stop();
        }
    }

    private void OnDestroy()
    {
        CashManager.OnMultiplierChange -= MultiplierChange;
    }
}
