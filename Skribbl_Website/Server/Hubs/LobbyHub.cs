using Microsoft.AspNetCore.SignalR;
using Skribbl_Website.Server.Services;
using Skribbl_Website.Shared.Dtos;
using System;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Skribbl_Website.Server.Hubs
{
    public class LobbyHub : Hub
    {
        private readonly LobbiesManager _lobbiesManager;

        public LobbyHub(LobbiesManager lobbiesManager)
        {
            _lobbiesManager = lobbiesManager;
        }

        public async Task SendMessage(Message message)
        {
            if(message.MessageContent != null && message.MessageContent != string.Empty)
            {
                var lobby = _lobbiesManager.GetLobbyByPlayerConnectionId(Context.ConnectionId);
                await Clients.Group(lobby.Id).SendAsync("ReceiveMessage", message);
            }
        }

        public async Task StartGame()
        {
            var lobby = _lobbiesManager.GetLobbyByPlayerConnectionId(Context.ConnectionId);
            if (lobby.GetHostPlayer()?.Connection == Context.ConnectionId)
            {
                //lobby.
            }
        }

        public async Task AddToGroup(string userId, string lobbyId)
        {
            var lobby = _lobbiesManager.GetLobbyById(lobbyId);
            var player = lobby.GetPlayerById(userId);
            player.Connection = Context.ConnectionId;

            await Clients.Group(lobbyId).SendAsync("AddPlayer", player);
            await Groups.AddToGroupAsync(Context.ConnectionId, lobbyId);
            var lobbyState = new LobbyClientDto(lobby);
            await Clients.Caller.SendAsync("SetLobby", lobbyState);
            await Clients.Group(lobbyId).SendAsync("ReceiveMessage",
                new Message(player.Name + " joined.", Message.MessageType.Join));

            if (lobby.GetHostPlayer() == null)
            {
                await lobby.SetHostPlayer(player.Name);
            }
        }

        public async Task SendLobbySettings(LobbySettings lobbySettings)
        {
            var lobby = _lobbiesManager.GetLobbyByPlayerConnectionId(Context.ConnectionId);
            if(lobby.GetHostPlayer().Connection == Context.ConnectionId)
            {
                lobby.LobbySettings = lobbySettings;
                await Clients.Group(lobby.Id).SendAsync("ReceiveLobbySettings", lobbySettings);
            }
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            if (exception != null)
            {
                var lobby = _lobbiesManager.GetLobbyByPlayerConnectionId(Context.ConnectionId);
                var player = lobby.GetPlayerByConnectionId(Context.ConnectionId);
                await lobby.SetUserStateToDisconnected(Context.ConnectionId);
                await Clients.Group(lobby.Id).SendAsync("ReceiveMessage",
new Message(player.Name + " lost connection.", Message.MessageType.Leave));
            }
            else
            {
                var lobby = _lobbiesManager.GetLobbyByPlayerConnectionId(Context.ConnectionId);
                var player = lobby.GetPlayerByConnectionId(Context.ConnectionId);
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, lobby.Id);
                await Clients.Group(lobby.Id).SendAsync("ReceiveMessage",
    new Message(player.Name + " left.", Message.MessageType.Leave));
                await Clients.Group(lobby.Id).SendAsync("RemovePlayer", player.Name);
                await lobby.RemovePlayerByName(player.Name);
            }
            await base.OnDisconnectedAsync(exception);
        }
    }
}
