using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SquadAI
{
    public class CustomAIController : AIController
    {

        protected override void Awake()
        {
            Debug.Log("Custom AI");
        }

        protected override void Update()
        {
            return;
        }
    }
}

