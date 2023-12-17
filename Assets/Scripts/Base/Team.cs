using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Base
{
    public abstract class Team : MonoBehaviour
    {
        [field:SerializeField] public string Name { get; set; }
        [SerializeField] protected short numberOfPlayersInTeam;
        [SerializeField] protected List<Player> players;
        
        public void AddPlayer(Player player)
        {
            if (players.Count >= numberOfPlayersInTeam)
            {
                Debug.LogError("You tried to add player to the team that is already full. Team Name: " + Name);
                return;
            }

            players.Add(player);
        }
        
        public void RemovePlayer(Player player)
        {
            if (!players.Contains(player))
            {
                Debug.LogError("You tried to remove player from the team that is not in the team. Team Name: " + Name);
                return;
            }

            players.Remove(player);
        }
    }
}