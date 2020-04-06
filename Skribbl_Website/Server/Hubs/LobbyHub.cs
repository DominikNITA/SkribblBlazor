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
        private LobbiesManager _lobbiesManager;

        public LobbyHub(LobbiesManager lobbiesManager)
        {
            _lobbiesManager = lobbiesManager;
        }

        public async Task AddToGroup(string userId, string lobbyId)
        {
            var lobby = _lobbiesManager.GetLobbyById(lobbyId);
            var player = lobby.GetPlayerById(userId);
            player.Connection = Context.ConnectionId;

            await Clients.Group(lobbyId).SendAsync("AddPlayer", player);
            await Groups.AddToGroupAsync(Context.ConnectionId, lobbyId);
            var lobbyState = new LobbyClientDto(lobby);
            string lobbyJson = JsonSerializer.Serialize(lobbyState);
            //await Clients.Client(Context.ConnectionId).SendAsync("SetLobby", lobbyState);
            //await Clients.All.SendAsync("SetLobbyString", lobbyJson);
            await Clients.All.SendAsync("SetLobby", lobbyState);
            await Clients.Group(lobbyId).SendAsync("ReceiveMessage",
                new Message(player.Name + " joined.", Message.MessageType.Join));

            if (lobby.GetHostPlayer() == null)
            {
                await lobby.SetHostPlayer(player.Name);
            }
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            if (exception == null)
            {
                var lobby = _lobbiesManager.GetLobbyByPlayerConnectionId(Context.ConnectionId);
                var player = lobby.GetPlayerByConnectionId(Context.ConnectionId);
                lobby.RemovePlayerByName(player.Name);
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, lobby.Id);
                await Clients.Group(lobby.Id).SendAsync("ReceiveMessage",
    new Message(player.Name + " left.", Message.MessageType.Leave));
                await Clients.Group(lobby.Id).SendAsync("RemovePlayer", player.Name);
            }
            else
            {
                var lobby = _lobbiesManager.GetLobbyByPlayerConnectionId(Context.ConnectionId);
                var username = lobby.GetPlayerByConnectionId(Context.ConnectionId);
                //lobby.RemoveUserByName(username);
                await Clients.Group(lobby.Id).SendAsync("ReceiveMessage",
new Message(username + " lost connection.", Message.MessageType.Leave));
            }
            await base.OnDisconnectedAsync(exception);
        }
    }
}
