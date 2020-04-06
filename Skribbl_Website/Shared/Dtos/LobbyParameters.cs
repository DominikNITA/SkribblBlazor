using System;
using System.Collections.Generic;
using System.Text;

namespace Skribbl_Website.Shared.Dtos
{
    public class LobbyParameters<T>
    {
        public List<T> Players { get;  set; } = new List<T>();
        public int MaxPlayers { get; set; } = 10;
        public int MinPlayers { get;  set; } = 2;
        public string Id { get;  set; }
        public string InviteLink { get;  set; }
        public int RoundsLimit { get;  set; } = 6;
        public int RoundCount { get;  set; }
        public int TimeLimit { get;  set; } = 60;
        public int TimeCount { get;  set; }
        public LobbyState State { get;  set; } = LobbyState.Preparing;

        public LobbyParameters(List<T> players, int maxPlayers, int minPlayers, string id, string inviteLink, int roundsLimit, int roundCount, int timeLimit, int timeCount, LobbyState state)
        {
            Players = players;
            MaxPlayers = maxPlayers;
            MinPlayers = minPlayers;
            Id = id;
            InviteLink = inviteLink;
            RoundsLimit = roundsLimit;
            RoundCount = roundCount;
            TimeLimit = timeLimit;
            TimeCount = timeCount;
            State = state;
        }

        public LobbyParameters()
        {

        }
    }
}
