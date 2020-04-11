using Microsoft.AspNetCore.SignalR;
using Skribbl_Website.Server.Services;
using Skribbl_Website.Shared.Dtos;
using System;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;
using Skribbl_Website.Shared;

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
                if(lobby.State == LobbyState.Drawing)
                {
                    var player = lobby.GetPlayerByConnectionId(Context.ConnectionId);
                    if (player.HasGuessedCorrectly || lobby.GetDrawingPlayer() == player)
                    {
                        return;
                    }
                    if(await lobby.CheckGuess(player, message.MessageContent))
                    {
                        return;
                    }
                }
                await Clients.Group(lobby.Id).SendAsync("ReceiveMessage", message);
            }
        }

        public async Task StartGame()
        {
            var lobby = _lobbiesManager.GetLobbyByPlayerConnectionId(Context.ConnectionId);
            if (lobby.IsConnectionAHost(Context.ConnectionId))
            {
                await lobby.StartGame();
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

        public async Task BanPlayer(string playerName)
        {
            var lobby = _lobbiesManager.GetLobbyByPlayerConnectionId(Context.ConnectionId);
            if (lobby.IsConnectionAHost(Context.ConnectionId))
            {
                var playerToBan = lobby.GetPlayerByName(playerName);
                await Clients.Client(playerToBan.Connection).SendAsync("RedirectToKickedPage", lobby.GetHostPlayer().Name);
                await Groups.RemoveFromGroupAsync(playerToBan.Connection, lobby.Id);
                await lobby.RemovePlayerByName(playerName);
                await Clients.Group(lobby.Id).SendAsync("RemovePlayer", new Message(playerName + " banned.", Message.MessageType.Leave, playerName));
            }
        }

        public async Task SetSelection(string word)
        {
            var lobby = _lobbiesManager.GetLobbyByPlayerConnectionId(Context.ConnectionId);
            var player = lobby.GetPlayerByConnectionId(Context.ConnectionId);
            await lobby.SelectSelection(player, word);
        }

        public async Task SendDraw(DrawDetails drawDetails)
        {
            var lobby = _lobbiesManager.GetLobbyByPlayerConnectionId(Context.ConnectionId);
            var player = lobby.GetPlayerByConnectionId(Context.ConnectionId);
            if(lobby.GetDrawingPlayer() == player && lobby.State == LobbyState.Drawing)
            {
                await Clients.OthersInGroup(lobby.Id).SendAsync("GetDraw", drawDetails);
            }
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            if (exception != null)
            {
                var lobby = _lobbiesManager.GetLobbyByPlayerConnectionId(Context.ConnectionId);
                var player = lobby.GetPlayerByConnectionId(Context.ConnectionId);
                await lobby.SetUserStateToDisconnected(Context.ConnectionId);
                //TODO send request to set is connected to false.
                await Clients.Group(lobby.Id).SendAsync("ReceiveMessage",
new Message(player.Name + " lost connection.", Message.MessageType.Leave));
            }
            else
            {
                var lobby = _lobbiesManager.GetLobbyByPlayerConnectionId(Context.ConnectionId);
                var player = lobby.GetPlayerByConnectionId(Context.ConnectionId);
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, lobby.Id);
                await Clients.Group(lobby.Id).SendAsync("RemovePlayer", new Message(player.Name + " left.", Message.MessageType.Leave, player.Name));
                await lobby.RemovePlayerByName(player.Name);
            }
            await base.OnDisconnectedAsync(exception);
        }
    }
}
