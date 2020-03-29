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

        public LobbyHub(LobbiesManager lobbiesManager)
        {
            _lobbiesManager = lobbiesManager;
        }

        public async Task AddToGroup(string userId, string lobbyId)
        {
            if (IsUser(userId, lobbyId))
            {
                var player = _lobbiesManager.GetUserByIdFromLobby(userId, lobbyId);
                _lobbiesManager.SetConnectionIdForUserInLobby(lobbyId, player.Name, Context.ConnectionId);
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
            else
            {
                throw new HubException("You don't have access to this lobby.");
            }
        }

        private bool IsUser(string userId, string lobbyId)
        {
            if (_lobbiesManager.IsUserIdInSpecificLobby(userId, lobbyId))
            {
                return true;
            }
            return false;
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            if(exception == null)
            {
                var lobby = _lobbiesManager.GetLobbyByUserConnectionId(Context.ConnectionId);
                var username = lobby.GetUserNameByConnectionId(Context.ConnectionId);
                lobby.RemoveUserByName(username);
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, lobby.Id);
                await Clients.Group(lobby.Id).SendAsync("ReceiveMessage",
    new Message(username + " left.", Message.MessageType.Leave));
                await Clients.Group(lobby.Id).SendAsync("RemovePlayer", username);
            }
            else
            {
                var lobby = _lobbiesManager.GetLobbyByUserConnectionId(Context.ConnectionId);
                var username = lobby.GetUserNameByConnectionId(Context.ConnectionId);
                //lobby.RemoveUserByName(username);
                await Clients.Group(lobby.Id).SendAsync("ReceiveMessage",
new Message(username + " lost connection.", Message.MessageType.Leave));
            }
            await base.OnDisconnectedAsync(exception);
        }
    }
}
