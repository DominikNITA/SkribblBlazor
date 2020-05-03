using System.Linq;

namespace Skribbl_Website.Shared.Dtos
{
    public class LobbyClientDto : LobbyParameters<PlayerClient>
    {
        public LobbyClientDto(LobbyParameters<Player> lobby)
        {
            var playersClient = lobby.Players.Select(player => (PlayerClient) player).ToList();
            Players = playersClient;
            MaxPlayers = lobby.MaxPlayers;
            MinPlayers = lobby.MinPlayers;
            Id = lobby.Id;
            InviteLink = lobby.InviteLink;
            RoundCount = lobby.RoundCount;
            LobbySettings = lobby.LobbySettings;
            TimeCount = lobby.TimeCount;
            State = lobby.State;
        }

        public LobbyClientDto()
        {
        }
    }
}