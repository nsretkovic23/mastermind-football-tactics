using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Base
{
    public enum AttackingDirection
    {
        Left,
        Right
    }
    public class Manager : MonoBehaviour
    {
        [field:SerializeField] public AttackingDirection AttDirection { get; private set; }
        [field:SerializeField] public string Name { get; private set; }
        [field:SerializeField] public List<Player> Players { get; private set; }

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