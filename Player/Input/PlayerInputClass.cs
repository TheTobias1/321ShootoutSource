using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InputSystem
{
    //Stores the current input state of the player
    //Can be read by player controlled objects and written to be input devices
    public class PlayerInputClass
    {
        //variables
        private Vector2 move;
        private float cameraX;

        private bool shooting;
        private bool reload;
        private bool switchWeapon;
        private bool swapWeapon;

        private bool heal;

        //properties
        public Vector2 MoveInput
        {
            get
            {
                return move;
            }
            set
            {
                move = value;
            }
        }

        public float CameraX
        {
            get
            {
#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL
                float x = Mathf.Pow(cameraX, 2f);
                if (x > 0 && cameraX < 0)
                    x = -x;

                return x;
#else
                return cameraX;
#endif
            }
            set
            {
                cameraX = value;
            }
        }

        public bool Shoot
        {
            get
            {
                return shooting;
            }
            set
            {
                shooting = value;
            }
        }

        public bool Reload
        {
            get
            {
                return reload;
            }
            set
            {
                reload = value;
            }
        }

        public bool Switch
        {
            get
            {
                return switchWeapon;
            }
            set
            {
                switchWeapon = value;
            }
        }

        public bool Swap
        {
            get { return swapWeapon; }
            set { swapWeapon = value; }
        }

        public bool Heal
        {
            get
            {
                return heal;
            }
            set
            {
                heal = value;
            }
        }

        //Methods
        public void WriteMovement(Vector2 moveIn, float camXIn)
        {
            cameraX = camXIn;
            move = moveIn;
        }
    }
}

