using System;
using System.Collections.Generic;
using System.Text;

namespace Skribbl_Website.Shared.Dtos
{
    public class LobbyClientDto
    {
        public List<PlayerClient> Players { get; set; }
        public int MaxPlayers { get; set; }
        public string Id { get; set; }
        public string InviteLink { get; set; }
        public int RoundsLimit { get; set; }
        public int TimeLimit { get; set; }

        public LobbyClientDto(List<PlayerClient> players, int maxPlayers, string id, string inviteLink, int roundsLimit, int timeLimit)
        {
            Players = players;
            MaxPlayers = maxPlayers;
            Id = id;
            InviteLink = inviteLink;
            RoundsLimit = roundsLimit;
            TimeLimit = timeLimit;
        }
    }
}
