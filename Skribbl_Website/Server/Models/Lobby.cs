using Microsoft.AspNetCore.SignalR;
using Skribbl_Website.Server.Hubs;
using Skribbl_Website.Server.Services;
using Skribbl_Website.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Skribbl_Website.Server.Models
{
    public class Lobby : LobbyBase<Player>
    {
        private readonly IHubContext<LobbyHub> _lobbyHub;
        private IWordsProviderService _wordsProviderService;

        private IScoreCalculator _scoreCalculator;

        public Lobby(IHubContext<LobbyHub> lobbyHub, IWordsProviderService wordsProviderService) : base()
        {
            _lobbyHub = lobbyHub;
            _wordsProviderService = wordsProviderService;
        }

        public Lobby()
        {
        }

        public async new Task<int> RemovePlayerByName(string username)
        {
            GetPlayerByName(username).IsConnected = false;
            await CheckNeedForNewHost();
            await CheckNeedForDrawingPlayer();
            return base.RemovePlayerByName(username);
        }

        public void SetConnectionIdForPlayer(string connection, string username)
        {
            if (connection == null || connection == string.Empty)
            {
                throw new ArgumentException();
            }
            var player = GetPlayerByName(username);
            player.Connection = connection;
        }

        public Player GetPlayerByConnectionId(string connectionId)
        {
            return Players.Where(player => player.Connection == connectionId).First();
        }

        public bool IsConnectionAHost(string connectionId)
        {
            return GetHostPlayer().Connection == connectionId;
        }

        public Player GetPlayerById(string id)
        {
            try
            {
                return Players.Where(player => player.Id == id).First();
            }
            catch
            {
                throw new ArgumentException();
            }
        }

        public async Task SetUserStateToDisconnected(string connectionId)
        {
            GetPlayerByConnectionId(connectionId).IsConnected = false;
            await CheckNeedForNewHost();
            await CheckNeedForDrawingPlayer();
        }

        public new async Task SetHostPlayer(string username)
        {
            base.SetHostPlayer(username);
            await _lobbyHub.Clients.Group(Id).SendAsync("ReceiveNewHost",
    new Message(username + " is the new host.", Message.MessageType.Host, username));
        }

        private async Task CheckNeedForNewHost()
        {
            await CheckNeedFor_Base(GetHostPlayer, SetHostPlayer);
        }

        public new async Task SetDrawingPlayer(string username)
        {
            base.SetDrawingPlayer(username);
            State = LobbyState.Drawing;
            await _lobbyHub.Clients.Group(Id).SendAsync("ReceiveNewDrawingPlayer",
    new Message(username + " is drawing now.", Message.MessageType.Join, username));
        }

        private async Task CheckNeedForDrawingPlayer()
        {
            //TODO: change to a method keeping order
            await CheckNeedFor_Base(GetDrawingPlayer, SetDrawingPlayer);
        }

        private async Task CheckNeedFor_Base(Func<Player> ifStatement, Func<string, Task> setFunction)
        {
            if (Players.Count > 1 && ifStatement() == null)
            {
                foreach (var player in Players)
                {
                    if (player.IsConnected)
                    {
                        await setFunction(player.Name);
                        break;
                    }
                }
            }
        }

        public new async Task StartGame()
        {
            base.StartGame();
            int delay = 3000;
            await _lobbyHub.Clients.Group(Id).SendAsync("StartGame", delay);
            await Task.Delay(delay);
            await SelectNextDrawingPlayer();
        }

        private async Task SelectNextDrawingPlayer()
        {
            var actualDrawingPlayer = GetDrawingPlayer();
            var newDrawingPlayer = new Player();
            PrepareForNextDrawer();
            //TODO: add listener
            //await _lobbyHub.Clients.Group(Id).SendAsync("PrepareForNextDrawer");
            if (actualDrawingPlayer == null)
            {
                newDrawingPlayer = Players.Where(player => player.IsConnected).First();
            }
            else
            {
                int actualDrawingIndex = Players.IndexOf(actualDrawingPlayer);
                while (!newDrawingPlayer.IsConnected)
                {
                    actualDrawingIndex++;
                    if (actualDrawingIndex >= Players.Count)
                    {
                        actualDrawingIndex = 0;
                        RoundCount++;
                        if (RoundCount > LobbySettings.RoundsLimit)
                        {
                            await EndGame();
                            return;
                        }
                        await _lobbyHub.Clients.Group(Id).SendAsync("UpdateRound", RoundCount);
                    }
                    newDrawingPlayer = Players[actualDrawingIndex];
                }
            }
            await this.SetDrawingPlayer(newDrawingPlayer.Name);
            await SendWordsToChoose();
            _scoreCalculator = new SimpleScoreCalculator();
            _scoreCalculator.StartCounting();
            var timer = new Timer(async (e) => { await CheckForSelection(); }, null, 10000, Timeout.Infinite);
        }

        private async Task SendWordsToChoose()
        {
            WordsToChoose = await _wordsProviderService.GetWords();
            await _lobbyHub.Clients.Client(GetDrawingPlayer().Connection).SendAsync("ReceiveWords", WordsToChoose);
        }

        public async Task SelectSelection(Player player, string word)
        {
            if (GetDrawingPlayer() == player && Selection == string.Empty)
            {
                SelectSelection(word);
                await _lobbyHub.Clients.GroupExcept(Id, new List<string> { player.Connection }).SendAsync("ReceiveWordTemplate", new List<int> { word.Length });
                await _lobbyHub.Clients.Client(player.Connection).SendAsync("ReceiveSelection", word);
                //await StartTimer();
            }
        }

        private async Task CheckForSelection()
        {
            if (Selection == null || Selection == string.Empty)
            {
                var random = new Random();
                int index = random.Next(WordsToChoose.Count);
                await SelectSelection(GetDrawingPlayer(), WordsToChoose[index]);
            }
        }

        private async Task StartTimer()
        {
            var timer = new Timer(async (e) => { await SubstractOneSecond(); }, null, 1000, Timeout.Infinite);
        }

        private async Task SubstractOneSecond()
        {
            TimeCount--;
            if (TimeCount > 0 && State== LobbyState.Drawing)
            {
                await StartTimer();
            }
            else
            {
                await GoToNextStep();
            }
        }

        public async Task<bool> CheckGuess(Player player, string guess)
        {
            if (guess == Selection)
            {
                player.HasGuessedCorrectly = true;
                _scoreCalculator.AddPlayer(player.Name);
                await _lobbyHub.Clients.Group(Id).SendAsync("GuessedCorrectly", new Message(player.Name + " guessed correctly.", Message.MessageType.Guessed, player.Name));
                await CheckForCompletedGuessing();
                return true;
            }
            else
            {
                if (LevenshteinDistance.Calculate(guess, Selection) <= 2)
                {
                    await _lobbyHub.Clients.Client(player.Connection).SendAsync("ReceiveMessage", new Message(guess + " is a close one!", Message.MessageType.CloseGuess));
                }
                return false;
            }
        }

        private async Task CheckForCompletedGuessing()
        {
            if (Players.Where(player => player.IsConnected && !player.IsDrawing).All((player) => player.HasGuessedCorrectly))
            {
                await GoToNextStep();
            }
        }
        public new void PrepareForNextDrawer()
        {
            base.PrepareForNextDrawer();
        }

        private async Task GoToNextStep()
        {
            await SendScores();
            await Task.Delay(4000);
            PrepareForNextDrawer();
            await SelectNextDrawingPlayer();
        }

        private async Task SendScores()
        {
            var newScores = _scoreCalculator.GetScores(Players.Count);
            await _lobbyHub.Clients.Group(Id).SendAsync("ReceiveScores", newScores);
        }

        private async Task EndGame()
        {
            //TODO: add listener
            await _lobbyHub.Clients.Group(Id).SendAsync("EndGame");
        }
    }
}