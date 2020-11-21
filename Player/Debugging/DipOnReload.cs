using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DipOnReload : MonoBehaviour
{
    public int reloadTime;

    Vector3 localPos;
    // Start is called before the first frame update
    void Start()
    {
        localPos = transform.localPosition;
        GetComponentInParent<Weapon>().OnReload += OnReload;
    }

    public void OnReload()
    {
        StartCoroutine(DipSequence());
    }

    IEnumerator DipSequence()
    {
        transform.localPosition = new Vector3(localPos.x, localPos.y - 1, localPos.z);
        yield return new WaitForSeconds(reloadTime);
        transform.localPosition = localPos;
    }
}
