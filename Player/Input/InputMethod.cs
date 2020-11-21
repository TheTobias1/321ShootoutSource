using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InputSystem
{
    //An abstract definition of an input method
    public class InputMethod : MonoBehaviour
    {
        public bool frozen;
        protected PlayerManager manager;
        private PlayerInputClass inputObject;

        public PlayerInputClass PlayerInputState
        {
            get
            {
                return inputObject;
            }
            set
            {
                inputObject = value;
            }
        }

        protected virtual void Start()
        {
            manager = GetComponent<PlayerManager>();
            inputObject = manager.PlayerInputState;
        }

        //Writes movement values to an input state if one exists
        protected void WriteMovement(Vector2 moveIn, float rot)
        {
            if (inputObject == null)
                return;

            inputObject.WriteMovement((frozen)? Vector2.zero : moveIn, rot);
        }

        protected void WriteWeapons(bool isShooting, bool isReloading, bool isSwitching, bool isSwapping)
        {
            if (inputObject == null || frozen)
            {
                if(inputObject != null)
                {
                    inputObject.Shoot = false;
                }
                return;
            }
                

            inputObject.Shoot = isShooting;
            inputObject.Reload = isReloading;
            inputObject.Switch = isSwitching;
            inputObject.Swap = isSwapping;
        }

        protected void WriteInteraction(bool heal, bool interact)
        {
            if (inputObject == null || frozen)
                return;

            inputObject.Heal = heal;
        }

        public void SetFreezeInput(bool f)
        {
            frozen = f;
        }

    }
}

