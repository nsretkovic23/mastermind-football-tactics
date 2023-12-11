using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Base
{
    public enum FieldState
    {
        Free,
        OnePlayer,
        TwoPlayers,
        Ball
    }

    public class Field : MonoBehaviour
    {
        public bool IsForbidden { get; set; }
        public UnityAction<Field, Player> OnPlayerMovedToTheField { get; set; }
        public UnityAction<Field, Player> OnPlayerLeftTheField { get; set; }

        [SerializeField] protected int maximumPlayersOnField;
        [SerializeField] protected List<Player> players;

        private void Awake()
        {
            players = new List<Player>(maximumPlayersOnField);
        }

        public bool CanMoveToTheField()
        {
            return players.Count < maximumPlayersOnField && !IsForbidden;
        }

        public bool IsPlayerOnTheField(Player player)
        {
            return players.Contains(player);
        }

        public void MoveToTheField(Player player)
        {
            if (IsForbidden)
            {
                Debug.LogError(
                    "You tried to move to the forbidden field, make sure to call CanMoveToTheField() to check if you can move to the specific field. Field Name: " +
                    gameObject.name);
                return;
            }

            players.Add(player);
            OnPlayerMovedToTheField?.Invoke(this, player);
        }

        public void LeaveTheField(Player player)
        {
            if (!players.Contains(player))
            {
                Debug.LogError(
                    "You tried to leave the field that you are not on, make sure you check if player is on this field: " +
                    gameObject.name);
                return;
            }

            players.Remove(player);
            OnPlayerLeftTheField?.Invoke(this, player);
        }
    }
}