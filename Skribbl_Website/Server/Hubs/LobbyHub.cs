using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Skribbl_Website.Server.Services;
using Skribbl_Website.Client.Pages;
using Skribbl_Website.Shared.Dtos;
using Skribbl_Website.Shared;
using System.Threading;

namespace Skribbl_Website.Server.Hubs
{
    public class LobbyHub : Hub
    {
        private LobbiesManager _lobbiesManager;
        //private string _lobbyId;
        //private PlayerDto _player;

        public LobbyHub(LobbiesManager lobbiesManager)
        {
            _lobbiesManager = lobbiesManager;
        }

        public async Task AddToGroup(string userId, string lobbyId)
        {
            if (IsUser(userId, lobbyId))
            {
                var player = _lobbiesManager.GetUserByIdFromLobby(userId, lobbyId);
                await Groups.AddToGroupAsync(Context.ConnectionId, lobbyId);
                await Clients.Client(Context.ConnectionId).SendAsync("ReceiveLobbyState", _lobbiesManager.GetLobbyById(lobbyId));
                await Clients.Group(lobbyId).SendAsync("AddPlayer", player);
                await Clients.Group(lobbyId).SendAsync("ReceiveMessage",
                    new Message(player.Name + " joined.", Message.MessageType.Join));
                if (_lobbiesManager.TrySetHost(lobbyId, userId, Context.ConnectionId))
                {
                    await Clients.Group(lobbyId).SendAsync("ReceiveMessage",
                        new Message(player.Name + " is the new host.", Message.MessageType.Host));
                }
            }
            //TODO: add wrong user
        }

        private bool IsUser(string userId, string lobbyId)
        {
            if (_lobbiesManager.IsUserIdInSpecificLobby(userId, lobbyId))
            {
                return true;
            }
            return false;
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            return base.OnDisconnectedAsync(exception);
        }
    }
}
