using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressionActivator : MonoBehaviour
{
    [System.Serializable]
    public struct ProgressionItem
    {
        public RectTransform item;
        public RectTransform lockedPlaceholder;
    }

    public ProgressionItem[] progression;
    public XpTypes progressionType;
    public int boundary;

    // Start is called before the first frame update
    void Start()
    {
        switch(progressionType)
        {
            case XpTypes.Maps:
                boundary = XPManager.MAP_XP_BOUNDARIES;
                break;
            case XpTypes.Skins:
                boundary = XPManager.SKIN_XP_BOUNDARIES;
                break;
        }

        int level = XPManager.CalculateLevel(XPManager.GetStoredXP(), boundary);

        for(int i = 0; i < progression.Length; ++i)
        {
            progression[i].item.gameObject.SetActive(level >= i);
            if(progression[i].lockedPlaceholder != null)
            {
                progression[i].lockedPlaceholder.gameObject.SetActive(level < i);
            }
        }
    }
}
