using Skribbl_Website.Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Skribbl_Website.Server.Services
{
    public class LobbiesManager
    {
        public List<Lobby> Lobbies { get; private set; } = new List<Lobby>();

        async public Task<string> CreateLobby()
        {
            var newLobby = new Lobby();
            Lobbies.Add(newLobby);
            return newLobby.InviteLink;
        }
    }
}
