using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerupItem : MonoBehaviour
{
    public GameObject buyButton;
    public Text costText;

    public Powerups powerup;
    public int cost;

    public int inflation = 1500;
    public int randomDeviation = 50;

    // Start is called before the first frame update
    void Start()
    {
        cost *= PowerupManager.GetRoundMultiplier();
        float multiplier = (float)SessionManager.currentRound / 5f;
        cost = Mathf.RoundToInt((float)cost + inflation * multiplier);
        cost += Random.Range(-randomDeviation, randomDeviation);

        if(powerup == Powerups.Investment)
        {
            cost = CashManager.Cash;
        }

        costText.text = "$" + cost.ToString();
    }

    public void Buy()
    {
        if(CashManager.TakeCash(cost))
        {
            buyButton.SetActive(false);
            PowerupManager.ActivatePowerup(powerup);

            if (powerup == Powerups.Investment)
            {
                PowerupManager.PowerupSystem.investmentBuffer = cost;
            }

            SendMessageUpwards("OnBuy");
        }
    }
}
