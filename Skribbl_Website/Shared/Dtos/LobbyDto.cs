using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Skribbl_Website.Shared.Dtos
{
    public class LobbyDto
    {
        public List<PlayerDto> Players { get; set; }
        public int MaxPlayers { get; set; }
        public string Id { get; set; }
        public string InviteLink { get; set; }
        [Required]
        [Range(1, 10)]
        public int RoundsLimit { get; set; }
        [Required]
        [Range(30,120)]
        public int TimeLimit { get; set; }

        public LobbyDto()
        {
            Id = Guid.NewGuid().ToString();
            InviteLink = Guid.NewGuid().ToString().Substring(0, 5);
            MaxPlayers = 10;
            RoundsLimit = 6;
            TimeLimit = 60;
            Players = new List<PlayerDto>();
        }

        public virtual void RemoveUserByName(string username)
        {
            Players.RemoveAll(player => player.Name == username);
        }
    }
}
