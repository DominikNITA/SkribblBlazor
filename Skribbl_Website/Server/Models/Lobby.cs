using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.AspNetCore.SignalR;
using Skribbl_Website.Server.Hubs;
using Skribbl_Website.Server.Interfaces;
using Skribbl_Website.Server.Services;
using Skribbl_Website.Shared;
using Skribbl_Website.Shared.Dtos;

namespace Skribbl_Website.Server.Models
{
    public class Lobby : LobbyBase<Player>
    {
        private readonly IHubContext<LobbyHub> _lobbyHub;
        private readonly IScoreCalculator _scoreCalculator;
        private readonly IWordDistanceCalculator _wordDistanceCalculator;
        private readonly IWordsProviderService _wordsProviderService;
        private Timer _gameTimer;
        private List<HintTimer> _hintTimers = new List<HintTimer>();
        private Timer _selectionTimer;

        public Lobby(IHubContext<LobbyHub> lobbyHub, IWordsProviderService wordsProviderService,
            IScoreCalculator scoreCalculator, IWordDistanceCalculator wordDistanceCalculator)
        {
            _lobbyHub = lobbyHub ?? throw new ArgumentNullException(nameof(lobbyHub));
            _wordsProviderService =
                wordsProviderService ?? throw new ArgumentNullException(nameof(wordsProviderService));
            _scoreCalculator = scoreCalculator ?? throw new ArgumentNullException(nameof(scoreCalculator));
            _wordDistanceCalculator =
                wordDistanceCalculator ?? throw new ArgumentNullException(nameof(wordDistanceCalculator));
        }

        public Lobby()
        {
        }

        public new async Task<int> RemovePlayerByName(string username)
        {
            await SetUserStateToDisconnected(username);
            return base.RemovePlayerByName(username);
        }

        public async Task<int> RemovePlayerByConnectionId(string connectionId)
        {
            return await RemovePlayerByName(GetPlayerByConnectionId(connectionId).Name);
        }

        public void SetConnectionIdForPlayer(string connection, string username)
        {
            if (connection == null || connection == string.Empty) throw new ArgumentException();
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
            await SelectNextDrawingPlayer();
        }

        private async Task CheckNeedFor_Base(Func<Player> ifStatement, Func<string, Task> setFunction)
        {
            if (Players.Count > 1 && ifStatement() == null)
                foreach (var player in Players)
                    if (player.IsConnected)
                    {
                        await setFunction(player.Name);
                        break;
                    }
        }

        public new async Task StartGame()
        {
            base.StartGame();
            var delay = 3000;
            await _lobbyHub.Clients.Group(Id).SendAsync("StartGame", delay);
            await Task.Delay(delay);
            await SelectNextDrawingPlayer();
        }

        private async Task SelectNextDrawingPlayer()
        {
            if (State != LobbyState.Preparing && State != LobbyState.Ended)
            {
                var actualDrawingPlayer = GetDrawingPlayer();
                var newDrawingPlayer = new Player();
                PrepareForNextDrawer();
                if (actualDrawingPlayer == null)
                {
                    newDrawingPlayer = Players.Where(player => player.IsConnected).First();
                }
                else
                {
                    var currentDrawingIndex = Players.IndexOf(actualDrawingPlayer);
                    while (!newDrawingPlayer.IsConnected)
                    {
                        currentDrawingIndex++;
                        if (currentDrawingIndex >= Players.Count)
                        {
                            currentDrawingIndex = 0;
                            RoundCount++;
                            if (RoundCount > LobbySettings.RoundsLimit)
                            {
                                await EndGame();
                                return;
                            }

                            await _lobbyHub.Clients.Group(Id).SendAsync("UpdateRound", RoundCount);
                        }

                        newDrawingPlayer = Players[currentDrawingIndex];
                    }
                }

                await SetDrawingPlayer(newDrawingPlayer.Name);
                await Task.Delay(50);
                await SendWordsToChoose();

                _selectionTimer = new Timer();
                _selectionTimer.Interval = 10000;
                _selectionTimer.Elapsed += async (sender, e) => await CheckForSelection();
                _selectionTimer.Enabled = true;
            }
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
                var selectionTemplate = new SelectionTemplate(word);

                _selectionTimer.Stop();
                _selectionTimer = null;

                _scoreCalculator.StartCounting();

                await _lobbyHub.Clients.GroupExcept(Id, new List<string> {player.Connection})
                    .SendAsync("ReceiveWordTemplate", selectionTemplate);
                await _lobbyHub.Clients.Client(player.Connection).SendAsync("ReceiveSelection", word);

                await StartTimers();
            }
        }

        private async Task CheckForSelection()
        {
            if (string.IsNullOrEmpty(Selection))
            {
                var random = new Random();
                var index = random.Next(WordsToChoose.Count);
                await SelectSelection(GetDrawingPlayer(), WordsToChoose[index]);
            }
        }

        private Task StartTimers()
        {
            _gameTimer = new Timer
            {
                Interval = LobbySettings.TimeLimit * 1000
            };
            _gameTimer.Elapsed += async (sender, e) => await GoToNextStep();
            _gameTimer.Enabled = true;


            _hintTimers = HintsCreator.CreateHintTimersForSelection(Selection, LobbySettings.TimeLimit);
            _hintTimers.ForEach(timer => timer.Elapsed += async (sender, e) => await SendHint(timer.Hint, sender));
            _hintTimers.ForEach(timer => timer.Enabled = true);
            return Task.CompletedTask;
        }


        public async Task<bool> CheckGuess(Player player, string guess)
        {
            if (State != LobbyState.Drawing) return true;
            if (guess.Equals(Selection, StringComparison.OrdinalIgnoreCase))
            {
                player.HasGuessedCorrectly = true;
                _scoreCalculator.AddPlayer(player.Name);
                await _lobbyHub.Clients.Group(Id).SendAsync("GuessedCorrectly",
                    new Message(player.Name + " guessed correctly.", Message.MessageType.Guessed, player.Name));
                await CheckForCompletedGuessing();
                return true;
            }

            //TODO: rework with interface
            if (_wordDistanceCalculator.Calculate(guess, Selection) <= 2)
                await _lobbyHub.Clients.Client(player.Connection).SendAsync("ReceiveMessage",
                    new Message(guess + " is a close one!", Message.MessageType.CloseGuess));
            return false;
        }

        private async Task CheckForCompletedGuessing()
        {
            if (Players.Where(player => player.IsConnected && !player.IsDrawing)
                .All(player => player.HasGuessedCorrectly)) await GoToNextStep();
        }

        public new void PrepareForNextDrawer()
        {
            _hintTimers = new List<HintTimer>();
            base.PrepareForNextDrawer();
        }

        private async Task GoToNextStep()
        {
            DisposeAllTimers();

            await SendScores();

            await Task.Delay(4000);
            PrepareForNextDrawer();
            await SelectNextDrawingPlayer();
        }

        private async Task SendScores()
        {
            var newScores = _scoreCalculator.GetScores(Players.Count);
            await _lobbyHub.Clients.Group(Id).SendAsync("ReceiveScores", newScores);
            await _lobbyHub.Clients.Group(Id).SendAsync("ReceiveCorrectAnswer", Selection);
        }

        private async Task SendHint(HintDto hint, object sender)
        {
            ((HintTimer) sender).Stop();
            await _lobbyHub.Clients.Group(Id).SendAsync("ReceiveHint", hint);
        }

        private void DisposeAllTimers()
        {
            _gameTimer.Stop();
            _gameTimer = null;
            _hintTimers.ForEach(DisposeTimer);
            _hintTimers.Clear();
        }

        private static void DisposeTimer(object sender)
        {
            var timer = (Timer) sender;
            timer.Stop();
            sender = null;
        }

        private async Task EndGame()
        {
            await _lobbyHub.Clients.Group(Id).SendAsync("EndGame");
        }
    }
}