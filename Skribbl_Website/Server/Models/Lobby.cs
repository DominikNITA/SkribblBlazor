using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Skribbl_Website.Shared;
using Skribbl_Website.Shared.Dtos;

namespace Skribbl_Website.Server.Models
{
    public class Lobby : LobbyDto
    {
        public Lobby(User host)
        {      
            Id = Guid.NewGuid().ToString();
            InviteLink = Guid.NewGuid().ToString();
            Players = new List<User>();
            Players.Add(host);
            MaxPlayers = 10;
            RoundsLimit = 6;
            TimeLimit = 60;
        }

        /// <summary>
        /// Add new player if the limit of players in the lobby is not reached
        /// </summary>
        /// <param name="player">User to add to the lobby</param>
        /// <returns>Is succesful?</returns>
        public bool AddPlayer(User player)
        {
            if (Players.Count < MaxPlayers)
            {
                Players.Add(player);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
