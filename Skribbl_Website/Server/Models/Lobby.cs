using Microsoft.AspNetCore.SignalR;
using Skribbl_Website.Server.Hubs;
using Skribbl_Website.Server.Services;
using Skribbl_Website.Shared;
using Skribbl_Website.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace Skribbl_Website.Server.Models
{
    public class Lobby : LobbyBase<Player>
    {
        private IHubContext<LobbyHub> _lobbyHub;
        private IWordsProviderService _wordsProviderService;
        private List<HintTimer> _hintTimers = new List<HintTimer>();
        private Timer _selectionTimer = null;
        private Timer _gameTimer = null;

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
            await SelectNextDrawingPlayer();
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
            if (actualDrawingPlayer == null)
            {
                newDrawingPlayer = Players.Where(player => player.IsConnected).First();
            }
            else
            {
                int currentDrawingIndex = Players.IndexOf(actualDrawingPlayer);
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
            await this.SetDrawingPlayer(newDrawingPlayer.Name);
            await SendWordsToChoose();

            _selectionTimer = new Timer();
            _selectionTimer.Interval = 10000;
            _selectionTimer.Elapsed += async (sender,e) => await CheckForSelection();
            _selectionTimer.Enabled = true;
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

                _scoreCalculator = new SimpleScoreCalculator();
                _scoreCalculator.StartCounting();

                await _lobbyHub.Clients.GroupExcept(Id, new List<string> { player.Connection }).SendAsync("ReceiveWordTemplate", selectionTemplate);
                await _lobbyHub.Clients.Client(player.Connection).SendAsync("ReceiveSelection", word);

                await StartTimer();
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
            _gameTimer = new Timer();
            _gameTimer.Interval = LobbySettings.TimeLimit * 1000;
            _gameTimer.Elapsed += async (sender, e) => await GoToNextStep();
            _gameTimer.Enabled = true;


            _hintTimers = HintsCreator.CreateHintTimersForSelection(Selection,LobbySettings.TimeLimit);
            _hintTimers.ForEach( timer => timer.Elapsed += async (sender, e) => await SendHint(timer.Hint, sender));
            _hintTimers.ForEach(timer => timer.Enabled = true);
        }


        public async Task<bool> CheckGuess(Player player, string guess)
        {
            if (guess.Equals(Selection, StringComparison.OrdinalIgnoreCase))
            {
                player.HasGuessedCorrectly = true;
                _scoreCalculator.AddPlayer(player.Name);
                await _lobbyHub.Clients.Group(Id).SendAsync("GuessedCorrectly", new Message(player.Name + " guessed correctly.", Message.MessageType.Guessed, player.Name));
                await CheckForCompletedGuessing();
                return true;
            }
            else
            {
                //TODO: rework with interface
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
            _hintTimers = new List<HintTimer>();
            base.PrepareForNextDrawer();
        }

        private async Task GoToNextStep()
        {
            //Clear all timers
            _gameTimer.Stop();
            _gameTimer = null;
            _hintTimers.ForEach(timer => DisposeTimer(timer));
            _hintTimers.Clear();

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

        private async Task SendHint(HintDto hint, object sender)
        {
            ((HintTimer)sender).Stop();
            await _lobbyHub.Clients.Group(Id).SendAsync("ReceiveHint", hint);
        }

        private void DisposeTimer(object sender)
        {
            var timer = (Timer)sender;
            timer.Stop();
            sender = null;
        }

        private async Task EndGame()
        {
            await _lobbyHub.Clients.Group(Id).SendAsync("EndGame");
        }
    }
}