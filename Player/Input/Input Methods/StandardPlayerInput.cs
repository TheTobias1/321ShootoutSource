using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

namespace InputSystem
{
    //An input method that uses rewired input
    public class StandardPlayerInput : InputMethod
    {
        private Player playerInput;

        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
            playerInput = ReInput.players.GetPlayer(0);
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            Controller controller = playerInput.controllers.GetLastActiveController();
            Vector2 move;
            float rot;

            bool nativeController = false;
            /*if(controller != null)
            {
                if(controller.type != ControllerType.Keyboard && controller.type != ControllerType.Mouse)
                {
                    native = true;
                }
            }*/

            if (!nativeController)
            {
                move = new Vector2(playerInput.GetAxisRaw("MoveX"), playerInput.GetAxisRaw("MoveY"));
                rot = playerInput.GetAxis("CamX");
            }
            else
            {
                move = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
                rot = Input.GetAxis("Mouse X");
            }

            WriteMovement(move, rot);
            WriteWeapons(playerInput.GetButton("Fire"), playerInput.GetButtonDown("Reload"), playerInput.GetButtonShortPressUp("Switch"), playerInput.GetButtonDown("Swap") || playerInput.GetButtonLongPressDown("Switch"));
            WriteInteraction(playerInput.GetButtonDown("Heal"), false);
        }
    }
}

