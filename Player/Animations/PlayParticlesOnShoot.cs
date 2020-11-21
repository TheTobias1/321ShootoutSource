using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayParticlesOnShoot : MonoBehaviour
{
    public ParticleSystem fx;
    Weapon weapon;
    // Start is called before the first frame update
    void Start()
    {
        weapon = GetComponentInParent<Weapon>();
        weapon.OnShoot += OnShoot;
    }

    // Update is called once per frame
    void OnShoot()
    {
        fx.Play();
    }

    private void OnDestroy()
    {
        weapon.OnShoot -= OnShoot;
    }
}
