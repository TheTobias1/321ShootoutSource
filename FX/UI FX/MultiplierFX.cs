using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultiplierFX : MonoBehaviour
{
    public Sprite sprite2x;
    public Sprite sprite3x;
    public Image multiplierText;
    private Vector2 pulseSize = new Vector2(1.17f,1.35f);

    public Color regularColour;
    public Color betterColour;

    private void Start()
    {
        multiplierText.CrossFadeAlpha(0, 0, true);

        CashManager.OnMultiplierChange += OnMultiplier;
    }

    public void OnMultiplier(int amount)
    {
        if(amount > 1)
        {
            int textAmount = amount;

            multiplierText.CrossFadeAlpha(1, 0.2f, true);

            multiplierText.sprite = (amount == 2) ? sprite2x : sprite3x;

            LeanTween.cancel(multiplierText.gameObject);

            multiplierText.rectTransform.localScale = new Vector3(1, 1, 1);
            float desiredSize = (amount == 2) ? pulseSize.x : pulseSize.y;

            LeanTween.scale(multiplierText.rectTransform, new Vector3(desiredSize, desiredSize, 1), (amount == 2)? 0.4f : 0.2f).setEaseInOutCirc().setLoopPingPong();
            LeanTween.textAlpha(multiplierText.rectTransform, 0, 0.3f).setEaseInOutCirc().setLoopPingPong(1);
        }
        else
        {
            LeanTween.cancel(multiplierText.gameObject);
            multiplierText.CrossFadeAlpha(0, 0.2f, true);
        }
    }

    private void OnDestroy()
    {
        CashManager.OnMultiplierChange -= OnMultiplier;
    }
}
