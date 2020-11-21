using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ProgressionCreator : MonoBehaviour
{
    public XpTypes progressionType;

    public RectTransform[] items;

    public bool takeChildren;
    public bool place;

    // Update is called once per frame
    void Update()
    {
        if (place)
            Place();
    }

    void Place()
    {
        if(takeChildren)
        {
            items = new RectTransform[transform.childCount];

            for(int i = 0; i < transform.childCount; ++i)
            {
                items[i] = transform.GetChild(i).gameObject.GetComponent<RectTransform>();
            }
        }

        ProgressionActivator activator = gameObject.AddComponent<ProgressionActivator>();

        activator.progression = new ProgressionActivator.ProgressionItem[items.Length];
        activator.progressionType = progressionType;

        for(int i = 0; i < items.Length; ++i)
        {
            activator.progression[i].item = items[i];
        }

        place = false;
    }
}
