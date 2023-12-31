using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Modes.Standard
{
    public class Field : Base.Field
    {
        // TODO: Remove ball spaghetti dependency between Field and Ball
        public override bool CanMoveToTheField(Base.Player player)
        {
            bool result =  Players.Count < 2
                   && !IsForbidden
                   && !IsPlayerOnTheField(player)
                   && Players.TrueForAll(p => p.Manager != player.Manager)
                   && ((Ball == null && Players.Count > 0) || IsEmpty);
            
            return result;
        }
        
        /// <summary>
        /// Checks if player can pass the ball to this field.
        /// </summary>
        public override bool CanPassTheBallToTheField(Base.Player player)
        {
            bool result =
                !IsForbidden
                && !IsGoal
                && Players.Count == 1 // TODO: If decided that ball can be passed to the empty field modify to (Players.Count == 1 || IsEmpty)
                && Players[0].Manager == player.Manager
                && Players[0] != player;
            
            return result;
        }
    }
}