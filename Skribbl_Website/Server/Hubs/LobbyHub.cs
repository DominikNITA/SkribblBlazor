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

        //public async Task SendMessage(string user, string message)
        //{
        //    await Clients.All.SendAsync("ReceiveMessage", new Message(message,));
        //}

        public async Task AddToGroup(string userId, string lobbyId)
        {
            if (IsUser(userId, lobbyId))
            {
                var player = _lobbiesManager.GetUserByIdFromLobby(userId, lobbyId);
                await Groups.AddToGroupAsync(Context.ConnectionId, lobbyId);
                await Clients.Client(Context.ConnectionId).SendAsync("ReceiveLobbyState", _lobbiesManager.GetLobbyId(lobbyId));
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

        //private void SetLocals(string lobbyId, string userId)
        //{
        //    _lobbyId = lobbyId;
        //    _player = _lobbiesManager.GetUserByIdFromLobby(userId, lobbyId);
        //}



        //private bool IsUser()
        //{
        //    var headers = Context.GetHttpContext().Request.Headers;
        //    if (!(headers.ContainsKey("User") && headers.ContainsKey("Lobby")))
        //    {
        //        headers.TryGetValue("User", out var userId);
        //        headers.TryGetValue("Lobby", out var lobbyId);
        //        if(_lobbiesManager.IsUserIdInSpecificLobby(userId, lobbyId))
        //        {
        //            _lobbyId = lobbyId;
        //            Context.Items.Add("User", userId);
        //            Context.Items.Add("Lobby", lobbyId);
        //        }
        //    }

        //    return false;
        //}


    }
}
