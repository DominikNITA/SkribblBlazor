using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Skribbl_Website.Shared;
using Skribbl_Website.Shared.Dtos;

namespace Skribbl_Website.Server.Models
{
    public class Lobby : LobbyClient
    {
        public Dictionary<string, string> UsersIds { get; set; }
        /// <summary>
        /// Key: ConnectionId
        /// Value: Username
        /// </summary>
        public Dictionary<string, string> Connections { get; set; }
        public string HostConnection { get; set; }

        public Lobby(UserDto host) : base()
        {
            UsersIds = new Dictionary<string, string>();
            Connections = new Dictionary<string, string>();
            HostConnection = string.Empty;
            AddPlayer(host);
        }

        /// <summary>
        /// Add new player if the limit of players in the lobby is not reached
        /// </summary>
        /// <param name="player">User to add to the lobby</param>
        /// <returns>Is succesful?</returns>
        public void AddPlayer(UserDto user)
        {
            base.AddPlayer(user);
            UsersIds[user.Id] = user.Name;
        }


        public void SetConnectionIdForUser(string connection, string username)
        {
            if (!Connections.TryGetValue(connection, out _))
            {
                Connections[connection] = username;
            }
        }

        public string GetUserNameByConnectionId(string connectionId)
        {
            if (Connections.TryGetValue(connectionId, out var username))
            {
                return username;
            }
            else
            {
                //TODO: Rework or new exception
                throw new Exception();
            }
        }

        public override int RemovePlayerByName(string username)
        {
            return base.RemovePlayerByName(username);
            //TODO: Change RemoveALl below
            //UsersIds.RemoveAll(user => user.Name == username);
            //TODO: Delete from Connections
        }

        public object GetUserById(string id)
        {
            throw new NotImplementedException();
        }

        public void RemoveUserByConnectionId(string connectionId)
        {
            RemovePlayerByName(GetUserNameByConnectionId(connectionId));
            if (Players.Count == 0)
            {
                //TODO: invoke empty event;
                return;
            }
            if (connectionId == HostConnection)
            {

            }
        }

        public void SetUserStateToDisconnected(string connectionId)
        {
            var username = GetUserNameByConnectionId(connectionId);
        }

        public void SetNewHost()
        {
            Players[0].IsHost = true;
        }
    }
}
