using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthpackActivator : MonoBehaviour
{
    public List<GameObject> healthPacks;
    public int numHealthPacks;

    private void Start()
    {
        List<GameObject> packs = new List<GameObject>(healthPacks);
        for(int i = 0; i < numHealthPacks; ++i)
        {
            int chosenPack = Random.Range(0, packs.Count);
            packs[chosenPack].SetActive(true);
            packs.RemoveAt(chosenPack);
        }
    }
}
