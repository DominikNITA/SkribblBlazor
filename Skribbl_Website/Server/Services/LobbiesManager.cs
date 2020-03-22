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
        public List<Lobby> Lobbies { get; private set; } = new List<Lobby>();

        //async public Task<string> CreateLobby()
        //{
        //    var newLobby = new Lobby(null);
        //    Lobbies.Add(newLobby);
        //    return newLobby.InviteLink;
        //}

        public string CreateLobby(User host)
        {
            var lobby = new Lobby(host);
            host.IsHost = true;
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
        public string AddPlayerToLobby(string inviteLink, User player)
        {
            foreach (var lobby in Lobbies)
            {
                //Search for lobby with corresponding invite link
                if (lobby.InviteLink.Equals(inviteLink))
                {
                    //Try to add a new player
                    if (lobby.AddPlayer(player))
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
    }
}
