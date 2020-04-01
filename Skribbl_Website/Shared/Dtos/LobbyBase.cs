using Skribbl_Website.Shared.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Skribbl_Website.Shared.Dtos
{
    public class LobbyBase<T> where T : PlayerClient
    {
        public List<T> Players { get; protected set; }
        public int MaxPlayers { get; set; }
        public string Id { get; protected set; }
        public string InviteLink { get; protected set; }
        public int RoundsLimit { get; protected set; }
        public int TimeLimit { get; protected set; }

        public PlayerClient GetPlayerByName(string username)
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

        public void AddPlayer(T playerDto)
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

        public void ChangeConnectionForPlayer(string username, bool newIsConnected)
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
