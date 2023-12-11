using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Modes.Standard
{
    public class GameplayManager : Base.GameplayManager
    {
        protected override void Awake()
        {
            base.Awake();
            Debug.Log("Hello from Standard Gameplay Manager");
        }
    }
}