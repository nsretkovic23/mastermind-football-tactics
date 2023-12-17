using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Base
{
    public class Field : MonoBehaviour
    {
        [field:SerializeField] public Vector2Int Position { get; set; }
        [field:SerializeField] public int X => Position.x; 
        [field:SerializeField] public int Y => Position.y;
        [field:SerializeField] public bool IsForbidden { get; set; }
        [field:SerializeField] public bool IsGoalkeeper { get; set; }
        [field: SerializeField] public bool IsEmpty => Players.Count == 0;
        [field: SerializeField] public List<Player> Players { get; set; }
        [field:SerializeField] public Ball Ball { get; set; }
        [field:SerializeField] public SpriteRenderer Sprite { get; set; }
        [field:SerializeField] public Transform LeftBallHolder { get; private set; }
        [field:SerializeField] public Transform RightBallHolder { get; private set; }
        public UnityAction<Field> OnFieldClicked { get; set; }
        public UnityAction<Field, Player> OnPlayerMovedToTheField { get; set; }
        public UnityAction<Field, Player> OnPlayerLeftTheField { get; set; }

        [SerializeField] protected int maximumPlayersOnField;

        private void OnValidate()
        {
            Sprite = GetComponent<SpriteRenderer>();
            //Players = new List<Player>();
        }

        private void Awake()
        {
           //Players = new List<Player>(maximumPlayersOnField);
        }

        private void OnMouseDown()
        {
            OnFieldClicked?.Invoke(this);
        }

        public virtual bool CanMoveToTheField(Base.Player player)
        {
            return Players.Count < maximumPlayersOnField && !IsForbidden;
        }
        
        public virtual bool CanPassTheBallToTheField(Base.Player player)
        {
            return !IsForbidden;
        }

        public bool IsPlayerOnTheField(Player player)
        {
            return Players.Contains(player);
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

            // TODO: Temporary just to make sure that player player is on the field, it should be animated as a player running or something
            player.transform.position = transform.position;
            Players.Add(player);
            OnPlayerMovedToTheField?.Invoke(this, player);
        }

        public void LeaveTheField(Player player)
        {
            if (!Players.Contains(player))
            {
                Debug.LogError(
                    "You tried to leave the field that you are not on, make sure you check if player is on this field: " +
                    gameObject.name);
                return;
            }

            Players.Remove(player);
            OnPlayerLeftTheField?.Invoke(this, player);
        }
    }
}