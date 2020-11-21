using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using SquadAI;

public class TouchControlsManager : MonoBehaviour
{
    public static TouchControlsManager touchControls;
    [HideInInspector]
    public GameObject player;
    private Player rewiredPlayer;
    private CustomController controller;

    public RectTransform lookRectTransform;
    public Rect lookRect;
    public GameObject swapButton;
    [HideInInspector]
    public PlayerManager playerManager;

    public bool swipeToCycle = true;
    public Dictionary<int, Vector3> touches = new Dictionary<int, Vector3>();
    private bool swipedDown;
    public float swipeDownThreshold = 0.25f;

    private float lookInput;
    public float LookInput
    {
        set { lookInput = value; }
    }
    private float moveX;
    private float moveY;

    private bool levelDone = false;

    public Vector2 MoveInput
    {
        set { moveX = value.x; moveY = value.y; }
    }

    // Start is called before the first frame update
    void Awake()
    {
        TouchControlsManager.touchControls = this;

        player = GameObject.FindGameObjectWithTag("Player");
        playerManager = player.GetComponent<PlayerManager>();
    }

    private void Start()
    {
        rewiredPlayer = ReInput.players.GetPlayer(0);
        lookRect = RectTransformToScreenSpace(lookRectTransform);

        ReInput.InputSourceUpdateEvent += OnRewiredInputUpdate; // subscribe to input update event
        controller = (CustomController)rewiredPlayer.controllers.GetControllerWithTag(ControllerType.Custom, "Touch");

        EnemySpawner.OnLevelClear += OnLevelClear;
    }

    // Update is called once per frame
    void Update()
    {
        //Weapon swapping
        if(playerManager.GetPickup() != null && !swapButton.activeSelf)
        {
            swapButton.SetActive(true);
        }
        else if(playerManager.GetPickup() == null && swapButton.activeSelf)
        {
            swapButton.SetActive(false);
        }

        //swipe down to cycle weapons
        /*if(swipeToCycle)
        {

            for (int i = 0; i < Input.touchCount; ++i)
            {
                Touch t = Input.touches[i];

                //add new touches
                if(t.phase == TouchPhase.Began)
                {
                    if (lookRect.Contains(t.position))
                        touches.Add(t.fingerId, Vector3.zero);
                }

                //track movement over time
                Vector3 touchNormalisedDelta = new Vector3(t.deltaPosition.x / Screen.width, t.deltaPosition.y / Screen.height, t.deltaTime);
                touches[t.fingerId] += touchNormalisedDelta;

                //asses the current touch
                Vector3 touchRecord = touches[t.fingerId];
                //is it primarily going down
                if (touchRecord.y < -swipeDownThreshold && Mathf.Abs(touchRecord.y) > Mathf.Abs(touchRecord.x * 2))
                {
                    //did it do all this reasonably fast
                    if(touchRecord.z < 1f)
                    {
                        //swipe down
                        swipedDown = true;
                    }
                }

                if(t.phase == TouchPhase.Ended)
                {
                    touches.Remove(t.fingerId);
                }
            }
        }*/
    }

    int swaps = 0;

    public void OnRewiredInputUpdate()
    {
        //if(swipeToCycle)
        //controller.SetButtonValueById(6, swipedDown || Input.GetKey(KeyCode.L));

        if (levelDone)
        {
            controller.SetButtonValueById(3, false);
            controller.SetAxisValueById(2, 0);
            controller.SetAxisValueById(0, 0);
            controller.SetAxisValueById(1, 0);
        }
        else
        {
            controller.SetAxisValueById(2, lookInput);
            controller.SetAxisValueById(0, moveX);
            controller.SetAxisValueById(1, moveY);
        }
    }

    void OnLevelClear()
    {
        try
        {
            GetComponentInChildren<TouchInputDisabler>().gameObject.SetActive(false);
        }
        catch
        {
            //No Touch inpu
        }
        lookInput = 0;
        moveX = 0;
        moveY = 0;

        levelDone = true;
    }

    public static Rect RectTransformToScreenSpace(RectTransform rt)
    {
        Vector3[] worldCorners = new Vector3[4];
        rt.GetWorldCorners(worldCorners);
        Bounds bounds = new Bounds(worldCorners[0], Vector3.zero);
        for (int i = 1; i < 4; ++i)
        {
            bounds.Encapsulate(worldCorners[i]);
        }
        return new Rect(bounds.min, bounds.size);
    }

    private void OnDestroy()
    {
        EnemySpawner.OnLevelClear -= OnLevelClear;
    }
}
