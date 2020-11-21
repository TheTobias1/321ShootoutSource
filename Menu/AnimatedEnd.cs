using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimatedEnd : MonoBehaviour
{
    public static event ActionDelegate OnEndFinished;

    public GameObject[] stars;
    public GameObject starRoot;

    public Text roundNumber;
    public GameObject roundText;

    public Image rankImage;
    public Text rankText;

    public void Initiate(int round)
    {
        foreach (GameObject s in stars)
        {
            s.transform.localScale = Vector3.zero;
        }

        roundNumber.text = round.ToString();
        roundText.transform.localScale = Vector3.zero;
        roundNumber.transform.localScale = Vector3.zero;
        rankImage.transform.localScale = Vector3.zero;
        rankText.transform.localScale = Vector3.zero;

        StartCoroutine(AnimateStars(round));
    }

    IEnumerator AnimateStars(int round)
    {
        yield return new WaitForSeconds(0.4f);

        LeanTween.scale(roundText, Vector3.one, 0.5f).setEaseOutElastic();
        yield return new WaitForSeconds(0.5f);
        LeanTween.scale(roundNumber.gameObject, Vector3.one, 0.6f).setEaseOutElastic();


        int rating = SessionManager.CalculateNumberOfStars(round);
        yield return new WaitForSeconds(1f);

        for(int i = 0; i < rating; ++i)
        {
            stars[i].SetActive(true);
            LeanTween.scale(stars[i], Vector3.one, 0.5f).setEaseOutElastic();
            yield return new WaitForSeconds(0.4f);
        }

        yield return new WaitForSeconds(0.4f);


        if(PostGameRankCalculator.gameRank != PlayerRanks.None)
        {
            rankImage.gameObject.SetActive(true);
            rankImage.sprite = SessionManager.currentRank;
            LeanTween.scale(rankImage.gameObject, Vector3.one, 0.5f).setEaseOutElastic();
            yield return new WaitForSeconds(0.5f);

            switch (PostGameRankCalculator.gameRank)
            {
                case PlayerRanks.Top100:
                    rankText.text = "You are Top 100 Globally";
                    break;
                case PlayerRanks.Top50:
                    rankText.text = "You are Top 50 Globally";
                    break;
                case PlayerRanks.Top10:
                    rankText.text = "You are Top 10 Globally";
                    break;
                case PlayerRanks.Top3:
                    rankText.text = "You are Top 3 Globally";
                    break;
            }

            rankText.gameObject.SetActive(true);
            LeanTween.scale(rankText.gameObject, Vector3.one, 0.6f).setEaseOutElastic();

            yield return new WaitForSeconds(3f);
        }

        LeanTween.moveLocalY(starRoot, -500, 0.5f).setEaseInBack();

        yield return new WaitForSeconds(0.5f);

        if (OnEndFinished != null)
        {
            OnEndFinished();
        }

        Destroy(gameObject);
    }
}
