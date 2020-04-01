using Skribbl_Website.Shared.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;

namespace Skribbl_Website.Shared.Dtos
{
    public class LobbyClient : LobbyBase<PlayerClient>
    {
        //public List<PlayerClient> Players { get; private set; }
        //public int MaxPlayers { get; set; }
        //public string Id { get; private set; }
        //public string InviteLink { get; private set; }
        //[Required]
        //[Range(1, 10)]
        //public int RoundsLimit { get; private set; }
        //[Required]
        //[Range(30, 120)]
        //public int TimeLimit { get; private set; }

        public LobbyClient()
        {
            Id = Guid.NewGuid().ToString();
            InviteLink = Guid.NewGuid().ToString().Substring(0, 5);
            MaxPlayers = 10;
            RoundsLimit = 6;
            TimeLimit = 60;
            Players = new List<PlayerClient>();
        }

        public LobbyClient(LobbyClientDto lobbyClientDto)
        {
            Id = lobbyClientDto.Id;
            InviteLink = lobbyClientDto.InviteLink;
            MaxPlayers = lobbyClientDto.MaxPlayers;
            RoundsLimit = lobbyClientDto.RoundsLimit;
            TimeLimit = lobbyClientDto.TimeLimit;
            Players = lobbyClientDto.Players;
        }

        public LobbyClientDto ConvertToLobbyClientDto()
        {
            return new LobbyClientDto(Players, MaxPlayers, Id, InviteLink, RoundsLimit, TimeLimit);
        }
    }
}
