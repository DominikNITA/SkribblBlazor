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
            RoundsLimit = lobbyClientDto.RoundsLimit;
            RoundCount = lobbyClientDto.RoundCount;
            TimeLimit = lobbyClientDto.TimeLimit;
            TimeCount = lobbyClientDto.TimeCount;
            State = lobbyClientDto.State;

        }

        public LobbyClient() : base()
        {

        }
    }
}
