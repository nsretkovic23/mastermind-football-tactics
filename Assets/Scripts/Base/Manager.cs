using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Base
{
    public class Manager : MonoBehaviour
    {
        public string Name { get; set; }

        [field:SerializeField] public List<Player> Players { get; set; }

        private void OnValidate()
        {
            Players.ForEach(player => player.Manager = this);
        }
    }
}