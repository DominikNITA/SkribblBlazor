using System;
using System.Collections.Generic;
using System.Text;

namespace Skribbl_Website.Shared.Dtos
{
    public class LobbyParameters<T>
    {
        public List<T> Players { get; set; } = new List<T>();
        public int MaxPlayers { get; set; } = 10;
        public int MinPlayers { get; set; } = 2;
        public string Id { get; set; }
        public string InviteLink { get; set; }
        public int RoundCount { get; set; }
        public int TimeCount { get; set; }
        public LobbySettings LobbySettings { get; set; } = new LobbySettings();
        public LobbyState State { get; set; } = LobbyState.Preparing;

        public LobbyParameters(List<T> players, int maxPlayers, int minPlayers, string id, string inviteLink, int roundCount, int timeCount, LobbyState state, LobbySettings lobbySettings)
        {
            Players = players;
            MaxPlayers = maxPlayers;
            MinPlayers = minPlayers;
            Id = id;
            InviteLink = inviteLink;
            RoundCount = roundCount;
            TimeCount = timeCount;
            State = state;
            LobbySettings = lobbySettings;
        }

        public LobbyParameters()
        {

        }
    }
}
