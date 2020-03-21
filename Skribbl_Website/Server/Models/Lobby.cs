using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Skribbl_Website.Server.Models
{
    public class Lobby
    {
        public enum LobbyState { Preparing, Playing, Finished, Deleted }
        public int MaxPlayers { get; private set; }
        public List<string> Players { get; private set; } = new List<string>();
        public LobbyState State { get; private set; } = LobbyState.Preparing;
        public Guid Id { get; private set; }
        public string InviteLink { get; private set; }
        public int RoundsLimit { get; set; }
        public int TimeLimit { get; set; }
        public Lobby()
        {
            MaxPlayers = 10;
            Id = new Guid();
            InviteLink = new Guid("dddddd").ToString();
        }
    }
}
