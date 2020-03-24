using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Skribbl_Website.Shared;
using Skribbl_Website.Shared.Dtos;

namespace Skribbl_Website.Server.Models
{
    public class Lobby : LobbyDto
    {
        public List<UserDto> Users { get; set; }
        public Dictionary<string, string> Connections { get; set; }
        public string HostConnection { get; set; }

        public Lobby(UserDto host)
        {      
            Id = Guid.NewGuid().ToString();
            InviteLink = Guid.NewGuid().ToString();
            MaxPlayers = 10;
            RoundsLimit = 6;
            TimeLimit = 60;
            Users = new List<UserDto>();
            Players = new List<PlayerDto>();
            _ = AddUser(host);
            Connections = new Dictionary<string, string>();
            HostConnection = string.Empty;
        }

        /// <summary>
        /// Add new player if the limit of players in the lobby is not reached
        /// </summary>
        /// <param name="player">User to add to the lobby</param>
        /// <returns>Is succesful?</returns>
        public bool AddUser(UserDto user)
        {
            if (Users.Count < MaxPlayers)
            {
                Players.Add(user);
                Users.Add(user);
                return true;
            }
            else
            {
                return false;
            }
        }

        
    }
}
