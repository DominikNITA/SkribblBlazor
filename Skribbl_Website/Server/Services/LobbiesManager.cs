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

        private readonly IHubContext<LobbyHub> _lobbyHub;
        private IWordsProviderService _wordsProviderService;

        public LobbiesManager(IHubContext<LobbyHub> hubContext, IWordsProviderService wordsProviderService)
        {
            _lobbyHub = hubContext;
            _wordsProviderService = wordsProviderService;
        }

        public string CreateLobby(Player host)
        {
            var lobby = new Lobby(_lobbyHub, _wordsProviderService);
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
                    try
                    {
                        lobby.AddPlayer(player);
                        return lobby.Id.ToString();
                    }
                    catch
                    {
                        throw new Exception("Lobby is full. Cannot join.");
                    }
                }
            }
            throw new Exception("This invite link doesn't match to any lobby.");
        }

        public Lobby GetLobbyById(string lobbyId)
        {
            return Lobbies.Where(lobby => lobby.Id == lobbyId).First();
        }

        public Lobby GetLobbyByInviteLink(string invitelink)
        {
            return Lobbies.Where(lobby => lobby.InviteLink == invitelink).First();
        }

        public Lobby GetLobbyByPlayerConnectionId(string connectionId)
        {
            return Lobbies.Where(lobby => lobby.Players.Any(player => player.Connection == connectionId)).First();
        }
    }
}
