using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rewired;
using SquadAI;

public class HudManager : MonoBehaviour
{
    public static HudManager HUD;
    private Canvas canvas;
    private GraphicRaycaster raycaster;
    private GameObject eventSystem;

    public PlayerHealth playerHealth;
    public GameObject player;
    public Slider healthBar;
    public Text healthPackCount;
    public Image bloodEffect;
    int bloodStatus;
    public Image healEffect;
    public Image healthIcon;
    public Image[] ammoIcons;

    public Color genericUIColour;
    public Text ammoCounter;
    public Color hasHealthColour;
    public Color noAmmoColour;

    public GameObject grenadeIcon;
    public RectTransform grenadeIconRotation;
    private float grenadeSwitchOff = 0;

    public Text cashText;
    public Text secondaryCashText;
    public Text moneyFX;

    public GameObject pauseMenu;
    public GameObject hudPanel;
    public Button play;

    private bool currentlyPaused = false;
    private bool lockCursorAfterPause;
    private float nextPause = 0;
    private Player playerInput;

    private bool levelClear = false;

    private int curSecondaryCash = 0;
    [SerializeField]
    private bool continuing;

    private void Awake()
    {
        HudManager.HUD = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        canvas = GetComponent<Canvas>();
        raycaster = GetComponent<GraphicRaycaster>();
        raycaster.enabled = false;
        eventSystem = GameObject.Find("EventSystem");

        player = GameObject.FindGameObjectWithTag("Player");

        playerHealth = player.GetComponent<PlayerHealth>();
        playerInput = ReInput.players.GetPlayer(0);
        playerHealth.OnHeal += OnHeal;

        healEffect.CrossFadeAlpha(0, 0, true);

        EnemySpawner.OnLevelClear += OnLevelClear;
        ContinueMenu.Continue += OnContinue;

        bloodEffect.CrossFadeAlpha(0, 0, true);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateHealthBar();

        if(Time.time > grenadeSwitchOff && grenadeIcon.activeSelf)
        {
            grenadeIcon.SetActive(false);
        }

        if(levelClear)
        {
            int continueValue = PowerupManager.GetPowerupStatus(Powerups.Investment)? CashManager.Cash : 0;
            Debug.Log("Player has $" + continueValue);
            curSecondaryCash = Mathf.RoundToInt(Mathf.Lerp(curSecondaryCash, continuing? continueValue : CashManager.Cash, 12 * Time.deltaTime));

            if(continuing)
            {
                if(Mathf.Abs(curSecondaryCash - continueValue) < 4)
                    curSecondaryCash = continueValue;
            }
            else
            {
                if(Mathf.Abs(curSecondaryCash - CashManager.Cash) < 4)
                {
                    curSecondaryCash = CashManager.Cash;
                }
            }

            secondaryCashText.text = "$" + curSecondaryCash.ToString();
        }
        else
        {
            cashText.text = "$" + CashManager.Cash.ToString();
        }


    }

    public void OnLevelClear()
    {
        hudPanel.SetActive(false);
        levelClear = true;
        secondaryCashText.gameObject.SetActive(true);
    }

    public void OnContinue()
    {
        continuing = true;
    }

    private void OnGUI()
    {
        if (playerInput.GetButtonUp("Pause") && Time.unscaledTime > nextPause)
        {
            nextPause = Time.unscaledTime + 1;
            SetPause(!currentlyPaused);
        }
    }

    void UpdateHealthBar()
    {
        if (healthBar == null || playerHealth == null)
            return;

        float hpFraction = playerHealth.currentHealth / playerHealth.maxHealth;

        healthBar.value = Mathf.Lerp(healthBar.value, hpFraction, 10 * Time.deltaTime);

        if(hpFraction < 0.4f)
        {
            if(hpFraction < 0.2f)
            {
                if(bloodStatus != 2)
                {
                    bloodStatus = 2;
                    bloodEffect.CrossFadeAlpha(0.5f, 0.4f, true);
                }
            }
            else
            {
                if (bloodStatus != 1)
                {
                    bloodStatus = 1;
                    bloodEffect.CrossFadeAlpha(1, 0.4f, true);
                }
            }
        }
        else
        {
            if (bloodStatus != 0)
            {
                bloodStatus = 0;
                bloodEffect.CrossFadeAlpha(0, 0.8f, true);
            }
        }
    }

    public void SetAmmoCount(int count)
    {
        ammoCounter.text = "" + count;

        if(count == 0)
        {
            foreach(Image ammoIcon in ammoIcons)
            {
                ammoIcon.color = noAmmoColour;
            }
        }
        else
        {
            foreach(Image ammoIcon in ammoIcons)
            {
                ammoIcon.color = genericUIColour;
            }
        }
    }

    public void DisableAmmoCount()
    {
        ammoCounter.text = "";

        foreach(Image ammoIcon in ammoIcons)
        {
            ammoIcon.color = genericUIColour;
        }
    }

    public void SetHealthPackCount(int count)
    {
        healthPackCount.text = count.ToString();

        if(count == 0)
        {
            healthIcon.color = genericUIColour;
        }
        else
        {
            healthIcon.color = hasHealthColour;
        }
    }

    public void ActivateGrenade(Vector3 grenadePosition)
    {
        float a = Vector3.SignedAngle(player.transform.forward, grenadePosition - player.transform.position, Vector3.up);
        Quaternion targetRot = Quaternion.Euler(new Vector3(-50, 0, -a));

        grenadeIconRotation.localRotation = Quaternion.Lerp(grenadeIconRotation.localRotation, targetRot, 8 * Time.deltaTime);

        if (!grenadeIcon.activeSelf)
        {
            grenadeIcon.SetActive(true);
        }

        grenadeSwitchOff = Time.time + 0.1f;
    }

    public void OnHeal()
    {
        healEffect.CrossFadeAlpha(1, 0.12f, true);
        Invoke("FinishHeal", 0.12f);
    }

    public void FinishHeal()
    {
        healEffect.CrossFadeAlpha(0, 0.4f, true);
    }

    public void SetPause(bool p)
    {
        raycaster.enabled = p;

        if(eventSystem != null)
            eventSystem.SetActive(!p);

        #region Money Text
        moneyFX.CrossFadeAlpha(0, 0, true);
        #endregion

        if (!currentlyPaused && p)
        {
            if(Cursor.lockState == CursorLockMode.Locked)
            {
                lockCursorAfterPause = true;
            }
        }

        if(p)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            if(lockCursorAfterPause)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        currentlyPaused = p;
        pauseMenu.SetActive(p);
        hudPanel.SetActive(!p);

        canvas.sortingOrder = p ? 99 : 0;

        if(p)
        {
            play.Select();
        }

        Time.timeScale = p ? 0 : 1;
        MapCanvasController.ResetWarningStatus();
    }

    public void QuitGame()
    {
        SessionManager.SessionInstance.GameOver();
        SessionManager.SessionInstance.LoadMenu();
        Time.timeScale = 1;
    }

    private void OnDestroy()
    {
        playerHealth.OnHeal -= OnHeal;
        EnemySpawner.OnLevelClear -= OnLevelClear;
        ContinueMenu.Continue -= OnContinue;
    }
}
