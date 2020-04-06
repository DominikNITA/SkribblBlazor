using System.Collections.Generic;
using System.Linq;

namespace Skribbl_Website.Shared.Dtos
{
    public class LobbyClientDto : LobbyParameters<PlayerClient>
    {
        public LobbyClientDto(LobbyParameters<Player> lobby)
        {
            var playersClient = lobby.Players.Select(player => (PlayerClient)player).ToList();
            Players = playersClient;
            MaxPlayers = lobby.MaxPlayers;
            MinPlayers = lobby.MinPlayers;
            Id = lobby.Id;
            InviteLink = lobby.InviteLink;
            RoundsLimit = lobby.RoundsLimit;
            RoundCount = lobby.RoundCount;
            TimeLimit = lobby.TimeLimit;
            TimeCount = lobby.TimeCount;
            State = lobby.State;
        }
        public LobbyClientDto()
        {

        }
    }
}
