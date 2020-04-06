using Microsoft.AspNetCore.SignalR;
using Skribbl_Website.Server.Hubs;
using Skribbl_Website.Server.Models;
using Skribbl_Website.Shared.Dtos;
using Skribbl_Website.Shared.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Skribbl_Website.Server.Services
{
    public class LobbiesManager
    {
        //TODO: change to private
        public List<Lobby> Lobbies { get; private set; } = new List<Lobby>();

        private IHubContext<LobbyHub> _lobbyHub;

        public LobbiesManager(IHubContext<LobbyHub> hubContext)
        {
            _lobbyHub = hubContext;
        }

        public string CreateLobby(Player host)
        {
            var lobby = new Lobby(_lobbyHub);
            lobby.AddPlayer(host);
            Lobbies.Add(lobby);
            return lobby.Id.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inviteLink"></param>
        /// <param name="player"></param>
        /// <exception cref="InviteLinkNotMatchingException"></exception>
        /// <exception cref="MaxPlayersReachedException"></exception>
        /// <returns></returns>
        public string AddPlayerToLobby(string inviteLink, Player player)
        {
            foreach (var lobby in Lobbies)
            {
                //Search for lobby with corresponding invite link
                if (lobby.InviteLink.Equals(inviteLink))
                {
                    if (lobby.Players.Any(user => user.Name == player.Name))
                    {
                        throw new Exception("Username already exists in this lobby! Try another one.");
                    }
                    //Try to add a new player
                    lobby.AddPlayer(player);
                    return lobby.Id.ToString();
                    //throw new Exception("Lobby is full. Cannot join.");
                }
            }
            throw new Exception("This invite link doesn't match to any lobby.");
        }

        public Lobby GetLobbyById(string lobbyId)
        {
            return Lobbies.Where(lobby => lobby.Id == lobbyId).First();
        }

        public bool IsUserIdInSpecificLobby(string userId, string lobbyId)
        {
            foreach (var lobby in Lobbies)
            {
                //TODO: below
                //if (lobby.Id == lobbyId)
                //{
                //    if (lobby.Users.Where(player => player.Id == userId).Count() == 1)
                //    {
                //        return true;
                //    }
                //    else
                //    {
                //        return false;
                //    }
                //}
            }
            return false;
        }

        public void SetConnectionIdForUserInLobby(string lobbyId, string username, string connectionId)
        {
            GetLobbyById(lobbyId).SetConnectionIdForPlayer(connectionId, username);
        }

        public Lobby GetLobbyByPlayerConnectionId(string connectionId)
        {
            return Lobbies.Where(lobby => lobby.Players.Any(player => player.Connection == connectionId)).First();
        }
        //public void RemoveUserByConnectionId(string connectionId)
        //{

        //}
    }
}
