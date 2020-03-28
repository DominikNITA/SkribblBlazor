using Skribbl_Website.Server.Models;
using Skribbl_Website.Shared.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Skribbl_Website.Shared;
using Skribbl_Website.Shared.Dtos;

namespace Skribbl_Website.Server.Services
{
    public class LobbiesManager
    {
        //TODO: change to private
        public List<Lobby> Lobbies { get; private set; } = new List<Lobby>();

        public string CreateLobby(UserDto host)
        {
            var lobby = new Lobby(host);
            host.IsHost = true;
            Lobbies.Add(lobby);
            return lobby.Id.ToString();
        }

        public bool TrySetHost(string lobbyId, string userId, string connection)
        {
            if (!LobbyHasHost(lobbyId))
            {
                GetLobbyById(lobbyId).HostConnection = connection;
                GetUserByIdFromLobby(userId, lobbyId).IsHost = true;
                return true;
            }
            return false;
        }

        private bool LobbyHasHost(string lobbyId)
        {
            return GetLobbyById(lobbyId).HostConnection != string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inviteLink"></param>
        /// <param name="player"></param>
        /// <exception cref="InviteLinkNotMatchingException"></exception>
        /// <exception cref="MaxPlayersReachedException"></exception>
        /// <returns></returns>
        public string AddPlayerToLobby(string inviteLink, UserDto player)
        {
            foreach (var lobby in Lobbies)
            {
                //Search for lobby with corresponding invite link
                if (lobby.InviteLink.Equals(inviteLink))
                {
                    if(lobby.Users.Any(user => user.Name == player.Name))
                    {
                        throw new UserNameAlreadyExistsException();
                    }
                    //Try to add a new player
                    if (lobby.AddUser(player))
                    {
                        return lobby.Id.ToString();
                    }
                    else
                    {
                        throw new MaxPlayersReachedException();
                    }
                }
            }
            throw new InviteLinkNotMatchingException();
        }

        public PlayerDto GetUserByIdFromLobby(string playerId, string lobbyId)
        {
            var foundLobby = Lobbies.Where(lobby => lobby.Id == lobbyId).First();
            return foundLobby.Users.Where(player => player.Id == playerId).First();
        }

        public Lobby GetLobbyById(string lobbyId)
        {
            return Lobbies.Where(lobby => lobby.Id == lobbyId).First();
        }

        public bool IsUserIdInSpecificLobby(string userId, string lobbyId)
        {
            foreach (var lobby in Lobbies)
            {
                if(lobby.Id == lobbyId)
                {
                    if(lobby.Users.Where(player => player.Id == userId).Count() == 1)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return false;
        }

        public void SetConnectionIdForUserInLobby(string lobbyId, string username, string connectionId)
        {
            GetLobbyById(lobbyId).SetConnectionIdForUser(username, connectionId);
        }
    }
}
