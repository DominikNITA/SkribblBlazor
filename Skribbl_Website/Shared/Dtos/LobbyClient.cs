namespace Skribbl_Website.Shared.Dtos
{
    public class LobbyClient : LobbyBase<PlayerClient>
    {
        public LobbyClient(LobbyClientDto lobbyClientDto)
        {
            Players = lobbyClientDto.Players;
            MaxPlayers = lobbyClientDto.MaxPlayers;
            MinPlayers = lobbyClientDto.MinPlayers;
            Id = lobbyClientDto.Id;
            InviteLink = lobbyClientDto.InviteLink;
            RoundCount = lobbyClientDto.RoundCount;
            LobbySettings = lobbyClientDto.LobbySettings;
            TimeCount = lobbyClientDto.TimeCount;
            State = lobbyClientDto.State;
        }

        public LobbyClient() : base()
        {

        }
    }
}
