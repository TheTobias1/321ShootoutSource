using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ZoneRating : MonoBehaviour
{
    public GameObject[] stars;
    public Text highestRoundText;

    public string worldName;

    // Start is called before the first frame update
    void Start()
    {
        Rate();
    }

    void Rate()
    {
        int amount = PlayerPrefs.GetInt(worldName);
        highestRoundText.text = amount.ToString();

        int starRating = (amount > 0)? SessionManager.CalculateNumberOfStars(amount) - 1 : -1;

        for(int i = 0; i < stars.Length; ++i)
        {
            stars[i].SetActive(i <= starRating);
        }
    }
}
