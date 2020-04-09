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
        readonly IHubContext<LobbyHub> _lobbyHub;
        public Lobby(IHubContext<LobbyHub> lobbyHub) : base()
        {
            _lobbyHub = lobbyHub;
        }

        public Lobby()
        {

        }

        public async new Task<int> RemovePlayerByName(string username)
        {
            GetPlayerByName(username).IsConnected = false;
            await CheckNeedForNewHost();
            await CheckNeedForDrawingPlayer();
            return base.RemovePlayerByName(username);
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

        public bool IsConnectionAHost(string connectionId)
        {
            return GetHostPlayer().Connection == connectionId;
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

        public async Task SetUserStateToDisconnected(string connectionId)
        {
            GetPlayerByConnectionId(connectionId).IsConnected = false;
            await CheckNeedForNewHost();
            await CheckNeedForDrawingPlayer();
        }

        public new async Task SetHostPlayer(string username)
        {
            base.SetHostPlayer(username);
            await _lobbyHub.Clients.Group(Id).SendAsync("ReceiveNewHost",
    new Message(username + " is the new host.", Message.MessageType.Host, username));
        }

        private async Task CheckNeedForNewHost()
        {
            await CheckNeedFor_Base(GetHostPlayer, SetHostPlayer);
        }

        public new async Task SetDrawingPlayer(string username)
        {
            base.SetDrawingPlayer(username);
            await _lobbyHub.Clients.Group(Id).SendAsync("ReceiveNewDrawingPlayer",
    new Message(username + " is drawing now.", Message.MessageType.Join, username));
        }

        private async Task CheckNeedForDrawingPlayer()
        {
            //TODO: change to a method keeping order
            await CheckNeedFor_Base(GetDrawingPlayer, SetDrawingPlayer);
        }

        private async Task CheckNeedFor_Base(Func<Player> ifStatement, Func<string,Task> setFunction)
        {
            if (Players.Count > 1 && ifStatement() == null)
            {
                foreach (var player in Players)
                {
                    if (player.IsConnected)
                    {
                        await setFunction(player.Name);
                        break;
                    }
                }
            }
        }

        public new async Task StartGame()
        {
            base.StartGame();
            int delay = 3000;
            await _lobbyHub.Clients.Group(Id).SendAsync("StartGame",delay);
            await Task.Delay(delay);
            await SelectNextDrawingPlayer();
        }

        private async Task SelectNextDrawingPlayer()
        {
            var actualDrawingPlayer = GetDrawingPlayer();
            var newDrawingPlayer = new Player();
            if (actualDrawingPlayer == null)
            {
                newDrawingPlayer = Players.Where(player => player.IsConnected).First();
            }
            else
            {
                int actualDrawingIndex = Players.IndexOf(actualDrawingPlayer);
                while (!newDrawingPlayer.IsConnected)
                {
                    actualDrawingIndex = actualDrawingIndex >= Players.Count ? 0 : actualDrawingIndex+1;
                    newDrawingPlayer = Players[actualDrawingIndex];
                }
            }
            await SetDrawingPlayer(newDrawingPlayer.Name);
        }
    }
}
