using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

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

        public LobbyClient()
        {
        }

        public List<ScoreDto> ScoresToUpdate { get; set; }

        public async Task StartCounting()
        {
            TimeCount = LobbySettings.TimeLimit;
            await StartTimer();
        }

        private async Task StartTimer()
        {
            var timer = new Timer(async e => { await SubstractOneSecond(); }, null, 1000, Timeout.Infinite);
        }

        private async Task SubstractOneSecond()
        {
            TimeCount--;
            OnTimeChanged();
            if (TimeCount > 0 && State == LobbyState.Drawing)
            {
                await StartTimer();
            }
        }
    }
}