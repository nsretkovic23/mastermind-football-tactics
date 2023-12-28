using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Base
{
    public class Manager : MonoBehaviour
    {
        [field:SerializeField] public string Name { get; set; }

        [field:SerializeField] public List<Player> Players { get; set; }

        private void OnValidate()
        {
            Players.ForEach(player => player.Manager = this);
        }
        
        public List<Field> GetFieldsWithPlayers()
        {
            List<Field> fields = new List<Field>();
            Players.ForEach(player =>
            {
                if (!fields.Contains(player.CurrentField))
                {
                    fields.Add(player.CurrentField);
                }
            });

            return fields;
        } 
    }
}