using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.SignalR;
using Skribbl_Website.Server.Hubs;
using Skribbl_Website.Server.Interfaces;
using Skribbl_Website.Server.Models;
using Skribbl_Website.Shared.Dtos;
using Skribbl_Website.Shared.Exceptions;

namespace Skribbl_Website.Server.Services
{
    public class LobbiesManager
    {
        private readonly IHubContext<LobbyHub> _lobbyHub;
        private readonly List<Lobby> _lobbies = new List<Lobby>();
        private readonly IServiceProvider _serviceProvider;
        private readonly IWordDistanceCalculator _wordDistanceCalculator;
        private readonly IWordsProviderService _wordsProviderService;

        public LobbiesManager(IHubContext<LobbyHub> lobbyHub, IWordsProviderService wordsProviderService,
            IWordDistanceCalculator wordDistanceCalculator, IServiceProvider serviceProvider)
        {
            _lobbyHub = lobbyHub;
            _wordsProviderService = wordsProviderService;
            _wordDistanceCalculator = wordDistanceCalculator;
            _serviceProvider = serviceProvider;
        }

        public string CreateLobby(Player host)
        {
            var scoreCalculator = (IScoreCalculator) _serviceProvider.GetService(typeof(IScoreCalculator));
            var lobby = new Lobby(_lobbyHub, _wordsProviderService, scoreCalculator, _wordDistanceCalculator);
            lobby.AddPlayer(host);
            _lobbies.Add(lobby);
            return lobby.Id;
        }

        /// <summary>
        /// </summary>
        /// <param name="inviteLink"></param>
        /// <param name="player"></param>
        /// <exception cref="InviteLinkNotMatchingException"></exception>
        /// <exception cref="MaxPlayersReachedException"></exception>
        /// <returns></returns>
        public string AddPlayerToLobby(string inviteLink, Player player)
        {
            foreach (var lobby in _lobbies)
                //Search for lobby with corresponding invite link
                if (lobby.InviteLink.Equals(inviteLink))
                {
                    if (lobby.Players.Any(user => user.Name == player.Name))
                        throw new Exception("Username already exists in this lobby! Try another one.");
                    //Try to add a new player
                    try
                    {
                        lobby.AddPlayer(player);
                        return lobby.Id;
                    }
                    catch
                    {
                        throw new Exception("Lobby is full. Cannot join.");
                    }
                }

            throw new Exception("This invite link doesn't match to any lobby.");
        }

        public Lobby GetLobbyById(string lobbyId)
        {
            return _lobbies.Where(lobby => lobby.Id == lobbyId).First();
        }

        public Lobby GetLobbyByInviteLink(string invitelink)
        {
            return _lobbies.Where(lobby => lobby.InviteLink == invitelink).First();
        }

        public Lobby GetLobbyByPlayerConnectionId(string connectionId)
        {
            return _lobbies.Where(lobby => lobby.Players.Any(player => player.Connection == connectionId)).First();
        }
    }
}