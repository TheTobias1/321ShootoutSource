using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashOnHit : MonoBehaviour
{
    public Health healthComponent;
    private EnemyHealth enemyHp;
    private bool doubleFlash = false;

    public SkinnedMeshRenderer meshRenderer;
    public MeshRenderer staticMeshRenderer;
    [HideInInspector]
    public Material[] normalMaterials;
    public Material flashMaterial;
    public Material secondaryFlash;

    [HideInInspector]
    public Vector3 normalScale;

    public float hitScale = 1.2f;
    // Start is called before the first frame update
    void Start()
    {
        if(meshRenderer != null)
        {
            normalMaterials = meshRenderer.materials;
            normalScale = meshRenderer.transform.localScale;
        }
        else
        {
            normalMaterials = staticMeshRenderer.materials;
            normalScale = staticMeshRenderer.transform.localScale;
        }

        if (healthComponent is EnemyHealth)
        {
            enemyHp = (EnemyHealth)healthComponent;
            doubleFlash = true;
            enemyHp.OnBreakShield += BreakShield;
            enemyHp.OnHitShield += HitShield;
        }

        if(healthComponent != null)
            healthComponent.OnDamage += OnHit;
    }

    public void OnHit(Damage dmg)
    {
        Flash(secondaryFlash, 0.1f);
        if(meshRenderer != null)
            meshRenderer.transform.localScale = normalScale * hitScale;
        else
            staticMeshRenderer.transform.localScale = normalScale * hitScale;
    }

    public void BreakShield()
    {
        Flash(secondaryFlash, 0.1f);
        if (meshRenderer != null)
            meshRenderer.transform.localScale = normalScale * 1.3f;
        else
            staticMeshRenderer.transform.localScale = normalScale * 1.3f;
    }

    public void HitShield()
    {
        Flash(flashMaterial);
        if (meshRenderer != null)
            meshRenderer.transform.localScale = normalScale * 1.1f;
        else
            staticMeshRenderer.transform.localScale = normalScale * 1.1f;
    }

    public void Flash(Material mat, float flashTime = 0.06f)
    {
        CancelInvoke("UndoFlash");
        Material[] newMats = new Material[normalMaterials.Length];
        for (int i = 0; i < normalMaterials.Length; ++i)
        {
            newMats[i] = mat;
        }
        if (meshRenderer != null)
            meshRenderer.materials = newMats;
        else
            staticMeshRenderer.materials = newMats;

        Invoke("UndoFlash", flashTime);
    }

    public void UndoFlash()
    {
        if (meshRenderer != null)
            meshRenderer.materials = normalMaterials;
        else
            staticMeshRenderer.materials = normalMaterials;
        CancelInvoke("UndoFlash");
        if (meshRenderer != null)
            meshRenderer.transform.localScale = normalScale;
        else
            staticMeshRenderer.transform.localScale = normalScale;
    }

    public void OnDestroy()
    {
        if(healthComponent != null)
            healthComponent.OnDamage -= OnHit;
    }
}
