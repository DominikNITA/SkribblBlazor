﻿using System;
using System.Collections.Generic;
using System.Linq;
using Skribbl_Website.Shared.Exceptions;

namespace Skribbl_Website.Shared.Dtos
{
    public enum LobbyState
    {
        Preparing,
        Started,
        Choosing,
        Drawing,
        Completed,
        Ended
    }

    public class LobbyBase<T> : LobbyParameters<T> where T : PlayerClient
    {
        public LobbyBase()
        {
            Id = Guid.NewGuid().ToString();
            InviteLink = Guid.NewGuid().ToString().Substring(0, 5);
        }

        public List<string> WordsToChoose { get; set; } = new List<string>();

        public SelectionTemplate SelectionTemplate { get; set; } = new SelectionTemplate();

        public string Selection { get; set; } = string.Empty;

        protected virtual void OnTimeChanged()
        {
            TimeChanged?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler TimeChanged;

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
            Players.ForEach(player => player.IsDrawing = false);
        }

        public void SetDrawingPlayer(string username)
        {
            Console.WriteLine("in set drawing player");
            SetAllPlayersToNotDrawing();
            State = LobbyState.Choosing;
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
            Players.ForEach(player => player.IsHost = false);
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

        public int GetConnectedPlayersCount()
        {
            return Players.Where(player => player.IsConnected).Count();
        }

        public void StartGame()
        {
            if (GetConnectedPlayersCount() >= MinPlayers)
            {
                State = LobbyState.Started;
            }
        }

        public void SelectSelection(string selection)
        {
            if (WordsToChoose.Contains(selection))
            {
                Selection = selection;
            }
            else
            {
                throw new ArgumentException();
            }
        }

        public void PrepareForNextDrawer()
        {
            Selection = string.Empty;
            WordsToChoose = new List<string>();
            SelectionTemplate = new SelectionTemplate();
            Players.ForEach(player => player.HasGuessedCorrectly = false);
        }

        public void UpdateScores(List<ScoreDto> newScores)
        {
            try
            {
                newScores.ForEach(scoreDto => GetPlayerByName(scoreDto.PlayerName).AddScore(scoreDto.ScoreToAdd));
            }
            catch
            {
                Console.WriteLine("error with updating the scores");
            }
        }
    }
}