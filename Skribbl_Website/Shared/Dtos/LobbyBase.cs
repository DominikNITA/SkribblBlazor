using Skribbl_Website.Shared.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Skribbl_Website.Shared.Dtos
{
    public enum LobbyState { Preparing, Started, Choosing, Drawing, Ended }

    public class LobbyBase<T> : LobbyParameters<T> where T : PlayerClient
    {
        //public List<T> Players { get; protected set; } = new List<T>();
        //public int MaxPlayers { get; set; } = 10;
        //public int MinPlayers { get; protected set; } = 2;
        //public string Id { get; protected set; }
        //public string InviteLink { get; protected set; }
        //public int RoundsLimit { get; protected set; } = 6;
        //public int RoundCount { get; protected set; }
        //public int TimeLimit { get; protected set; } = 60;
        //public int TimeCount { get; protected set; }
        //public LobbyState State { get; protected set; } = LobbyState.Preparing;

        public LobbyBase()
        {
            Id = Guid.NewGuid().ToString();
            InviteLink = Guid.NewGuid().ToString().Substring(0, 5);
        }

        public T GetPlayerByName(string username)
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
            var deletedPlayers = Players.RemoveAll(player => player.Name == username);
            return deletedPlayers;
        }

        public virtual void AddPlayer(T playerDto)
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

        public T GetDrawingPlayer()
        {
            return Players.Where(player => player.IsDrawing).FirstOrDefault();
        }

        public virtual void ChangeConnectionForPlayer(string username, bool newIsConnected)
        {
            GetPlayerByName(username).IsConnected = newIsConnected;
        }

        private void SetAllPlayersToNotHosts()
        {
            Players.ForEach((player) => player.IsHost = false);
        }

        public virtual void SetHostPlayer(string username)
        {
            var player = GetPlayerByName(username);
            if (!player.IsConnected)
            {
                throw new DisconnectedPlayerException();
            }
            SetAllPlayersToNotHosts();
            player.IsHost = true;
        }

        public T GetHostPlayer()
        {
            return Players.Where(player => player.IsHost).FirstOrDefault();
        }
    }
}
