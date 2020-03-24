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

        public async Task SendMessage(string user, string message)
        {
            var a = Context.UserIdentifier;
            await Clients.All.SendAsync("ReceiveMessage", new NameModel(user), message);
        }

        public async Task AddToGroup(string userId, string lobbyId)
        {
            if (IsUser(userId, lobbyId))
            {
                var player = _lobbiesManager.GetUserByIdFromLobby(userId, lobbyId);
                await Groups.AddToGroupAsync(Context.ConnectionId, lobbyId);
                await Clients.Group(lobbyId).SendAsync("ReceiveJoinMessage",player);
                if (_lobbiesManager.TrySetHost(lobbyId, userId, Context.ConnectionId))
                {
                    await Clients.Group(lobbyId).SendAsync("ReceiveJoinMessage", player);
                }
                await SendMessage("AddToGroup", "after");
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
