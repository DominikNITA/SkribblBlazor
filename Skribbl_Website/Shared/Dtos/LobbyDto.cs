using Skribbl_Website.Shared.Exceptions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
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
        [Range(30, 120)]
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

        public PlayerDto GetPlayerByName(string username)
        {
            try
            {
                return Players.Where(player => player.Name == username).First();
            }
            catch
            {
                throw new ArgumentException();
            }
        }

        public bool ContainsPlayerWithName(string username)
        {
            return Players.Where(player => player.Name == username).Count() == 1;
        }

        public virtual int RemovePlayerByName(string username)
        {
            return Players.RemoveAll(player => player.Name == username);
        }

        public void AddPlayer(PlayerDto playerDto)
        {
            if (playerDto == null)
            {
                throw new ArgumentNullException();
            }
            if (Players.Count >= MaxPlayers)
            {
                throw new MaxPlayersReachedException();
            }
            if (ContainsPlayerWithName(playerDto.Name))
            {
                throw new UserNameAlreadyExistsException();
            }

            Players.Add(playerDto);
        }
        private void SetAllPlayersToNotDrawing()
        {
            Players.ForEach((player) => player.IsDrawing = false);
        }
        public void SetDrawingPlayer(string username)
        {
            SetAllPlayersToNotDrawing();
            GetPlayerByName(username).IsDrawing = true;
        }

        public void ChangeConnectionToLobby(string username, bool newIsConnected)
        {
            GetPlayerByName(username).IsConnected = newIsConnected;
        }

        private void SetAllPlayersToNotHosts()
        {
            Players.ForEach((player) => player.IsHost = false);
        }

        public void SetHostPlayer(string username)
        {
            var player = GetPlayerByName(username);
            if (!player.IsConnected)
            {
                throw new DisconnectedPlayerException();
            }
            SetAllPlayersToNotHosts();
            player.IsHost = true;
        }
    }
}
