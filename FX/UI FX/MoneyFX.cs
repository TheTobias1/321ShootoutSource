using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoneyFX : MonoBehaviour
{
    public Text moneyText;
    private float additiveThreshold;
    public int additiveBuffer;

    private void Start()
    {
        moneyText.CrossFadeAlpha(0, 0, true);

        CashManager.OnGainScore += OnScoreCash;
    }

    public void OnScoreCash(int amount)
    {
        int textAmount = amount;

        if(Time.time < additiveThreshold)
        {
            textAmount += additiveBuffer;
            additiveBuffer = textAmount;
            Debug.Log("ADD");
        }
        else
        {
            additiveBuffer = amount;
            Debug.Log("SET: " + amount.ToString());
        }

        moneyText.text = "+" + textAmount.ToString();
        LeanTween.cancel(moneyText.gameObject);

        moneyText.rectTransform.localScale = new Vector3(0, 0, 1);
        moneyText.CrossFadeAlpha(1, 0, true);

        LeanTween.scale(moneyText.rectTransform, new Vector3(1,1,1), 1f).setEaseOutElastic();
        moneyText.CrossFadeAlpha(0, 1, true);

        additiveThreshold = Time.time + 0.5f;
    }

    private void OnDestroy()
    {
        CashManager.OnGainScore -= OnScoreCash;
    }
}
