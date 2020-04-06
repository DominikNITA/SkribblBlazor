using Microsoft.AspNetCore.SignalR;
using Skribbl_Website.Server.Hubs;
using Skribbl_Website.Shared.Dtos;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Skribbl_Website.Server.Models
{
    public class Lobby : LobbyBase<Player>
    {
        IHubContext<LobbyHub> _lobbyHub;
        public Lobby(IHubContext<LobbyHub> lobbyHub) : base()
        {
            _lobbyHub = lobbyHub;
        }

        public Lobby()
        {

        }

        public void SetConnectionIdForPlayer(string connection, string username)
        {
            if (connection == null || connection == string.Empty)
            {
                throw new ArgumentException();
            }
            var player = GetPlayerByName(username);
            player.Connection = connection;
        }

        public Player GetPlayerByConnectionId(string connectionId)
        {
            return Players.Where(player => player.Connection == connectionId).First();
        }

        public override int RemovePlayerByName(string username)
        {
            return base.RemovePlayerByName(username);
            //TODO: Change RemoveALl below
            //UsersIds.RemoveAll(user => user.Name == username);
            //TODO: Delete from Connections
        }

        public Player GetPlayerById(string id)
        {
            try
            {
                return Players.Where(player => player.Id == id).First();
            }
            catch
            {
                throw new ArgumentException();
            }
        }

        public void RemoveUserByConnectionId(string connectionId)
        {
            RemovePlayerByName(GetPlayerByConnectionId(connectionId).Name);
            if (Players.Count == 0)
            {
                //TODO: invoke empty event;
                return;
            }
            //TODO: Add host check
        }

        public void SetUserStateToDisconnected(string connectionId)
        {
            GetPlayerByConnectionId(connectionId).IsConnected = false;
        }

        public void SetNewHost()
        {
            //TODO: Check connection
            Players[0].IsHost = true;
        }

        public new async Task SetHostPlayer(string username)
        {
            base.SetHostPlayer(username);
            await _lobbyHub.Clients.Group(Id).SendAsync("ReceiveNewHost",
    new Message(username + " is the new host.", Message.MessageType.Host, username));
        }
    }
}
