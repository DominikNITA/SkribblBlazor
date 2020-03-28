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
        /// <summary>
        /// Key: ConnectionId
        /// Value: Username
        /// </summary>
        public Dictionary<string, string> Connections { get; set; }
        public string HostConnection { get; set; }

        public Lobby(UserDto host)
        {      
            Id = Guid.NewGuid().ToString();
            InviteLink = Guid.NewGuid().ToString().Substring(0, 5);
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

        public void SetConnectionIdForUser(string connection, string username)
        {
            if(!Connections.TryGetValue(connection, out string user))
            {
                Connections[connection] = username;
            }
        }

        public string GetUserNameByConnectionId(string connectionId)
        {
            if(Connections.TryGetValue(connectionId, out var username))
            {
                return username;
            }
            else
            {
                //TODO: Rework or new exception
                throw new Exception();
            }
        }

        public override void RemoveUserByName(string username)
        {
            base.RemoveUserByName(username);
            Users.RemoveAll(user => user.Name == username);
            //TODO: Delete from Connections
        }
    }
}
