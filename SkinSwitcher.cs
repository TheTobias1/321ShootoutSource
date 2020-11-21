using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SkinSwitcher : MonoBehaviour
{
    public Material[] skins;
    public int curSkin = 0;
    public MeshRenderer rend;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            rend.material = skins[curSkin];
            curSkin++;

            if(curSkin >= skins.Length)
            {
                curSkin = 0;
            }
        }
    }
}
