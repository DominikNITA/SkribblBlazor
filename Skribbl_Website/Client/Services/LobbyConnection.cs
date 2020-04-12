using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Skribbl_Website.Shared;
using Skribbl_Website.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Skribbl_Website.Client.Services
{
    public class LobbyConnection
    {
        public Player User { get; set; }
        public LobbyClient Lobby { get; set; }
        public List<Message> Messages { get; set; } = new List<Message>();

        private IJSRuntime _jsRuntime;
        private HubConnection _hubConnection;

        private void InvokeOnReceive(object sender = null, EventArgs e = null)
        {
            StateChanged?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler StateChanged;

        protected virtual void OnError()
        {
            ErrorOccured?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler ErrorOccured;

        protected virtual void OnBan(string username)
        {
            UserBanned?.Invoke(this, username);
        }

        public event EventHandler<string> UserBanned;

        protected virtual void OnDraw(DrawDetails drawDetails)
        {
            DrawReceived?.Invoke(this, drawDetails);
        }

        public event EventHandler<DrawDetails> DrawReceived;

        public LobbyConnection(IJSRuntime jSRuntime)
        {
            _jsRuntime = jSRuntime;
        }

        public async Task StartConnection(Player user, Uri hubUrl, string lobbyId)
        {
            _hubConnection = new HubConnectionBuilder().WithUrl(hubUrl).AddJsonProtocol(options =>
            {
                options.PayloadSerializerOptions.PropertyNamingPolicy = null;
            }).WithAutomaticReconnect().Build();
            User = user;

            _hubConnection.On<Message>("ReceiveNewHost", (message) =>
            {
                Messages.Add(new Message(message.MessageContent, message.Type));
                Lobby.SetHostPlayer(message.Sender);
                InvokeOnReceive();
            });

            _hubConnection.On<Message>("ReceiveMessage", (message) =>
            {
                Messages.Add(message);
                InvokeOnReceive();
            });

            _hubConnection.On<LobbyClientDto>("SetLobby", (lobbyClientDto) =>
            {
                Lobby = new LobbyClient(lobbyClientDto);
                Lobby.TimeChanged += InvokeOnReceive;
                InvokeOnReceive();
            });

            _hubConnection.On<PlayerClient>("AddPlayer", (playerToAdd) =>
            {
                if (!Lobby.Players.Any(p => p.Name == playerToAdd.Name))
                {
                    Lobby.Players.Add(playerToAdd);
                }
                InvokeOnReceive();
            });

            _hubConnection.On<Message>("RemovePlayer", (message) =>
            {
                Lobby.RemovePlayerByName(message.Sender);
                Messages.Add(message);
                InvokeOnReceive();
            });

            _hubConnection.On<LobbySettings>("ReceiveLobbySettings", (lobbySettings) =>
            {
                Console.WriteLine("in receive");
                Lobby.LobbySettings = lobbySettings;
                InvokeOnReceive();
            });

            _hubConnection.On<string>("RedirectToKickedPage", (hostName) =>
            {
                OnBan(hostName);
            });

            _hubConnection.On<int>("StartGame", (delay) =>
            {
                Lobby.StartGame();
                InvokeOnReceive();
            });

            _hubConnection.On<Message>("ReceiveNewDrawingPlayer", async (message) =>
            {
                await _jsRuntime.InvokeVoidAsync("prepareBoard");
                Lobby.PrepareForNextDrawer();
                Messages.Add(message);
                Lobby.SetDrawingPlayer(message.Sender);
                if (UserIsDrawing)
                {
                    await _jsRuntime.InvokeVoidAsync("setIsDrawing", true);
                }
                InvokeOnReceive();
            });

            _hubConnection.On<List<string>>("ReceiveWords", (words) =>
            {
                Lobby.WordsToChoose = words;
                Lobby.State = LobbyState.Choosing;
                InvokeOnReceive();
            });

            _hubConnection.On<List<int>>("ReceiveWordTemplate", async (wordTemplate) =>
            {
                Lobby.SelectionTemplate = wordTemplate;
                Lobby.State = LobbyState.Drawing;
                await Lobby.StartCounting();
                InvokeOnReceive();
            });

            _hubConnection.On<string>("ReceiveSelection", async (selection) =>
            {
                Lobby.Selection = selection;
                Lobby.State = LobbyState.Drawing;
                await Lobby.StartCounting();
                InvokeOnReceive();
            });

            _hubConnection.On<List<ScoreDto>>("ReceiveScores", (newScores) =>
            {
                Lobby.UpdateScores(newScores);
                Lobby.State = LobbyState.Completed;
                InvokeOnReceive();
            });

            _hubConnection.On<char, int>("GetHint", (character, position) =>
            {
                throw new NotImplementedException();
                InvokeOnReceive();
            });

            _hubConnection.On<DrawDetails>("GetDraw", (drawDetails) =>
            {
                OnDraw(drawDetails);
                InvokeOnReceive();
            });

            _hubConnection.On<int>("UpdateRound", (newRoundCount) =>
            {
                Console.WriteLine("new round " + newRoundCount);
                Lobby.RoundCount = newRoundCount;
                InvokeOnReceive();
            });

            _hubConnection.On("EndGame", () =>
            {
                OnBan("Game ended");
                InvokeOnReceive();
            });

            _hubConnection.On<Message>("GuessedCorrectly", (message) =>
            {
                Lobby.GetPlayerByName(message.Sender).HasGuessedCorrectly = true;
                Messages.Add(message);
                InvokeOnReceive();
            });

            await _hubConnection.StartAsync();
            try
            {
                await Join(lobbyId);
            }
            catch
            {
                OnError();
            }
        }

        public void CloseConnection()
        {
            _hubConnection.StopAsync();
            //TODO: Clear UserState
        }

        public Task Send(string messageContent) =>
        _hubConnection.SendAsync("SendMessage", new Message(messageContent, Message.MessageType.Guess, User.Name));

        private Task Join(string lobbyId) =>
        _hubConnection.InvokeAsync("AddToGroup", User.Id, lobbyId);

        public Task UpdateLobbySettings()
        {
            if (UserIsHost)
            {
                return _hubConnection.SendAsync("SendLobbySettings", Lobby.LobbySettings);
            }
            else
            {
                return Task.CompletedTask;
            }
        }

        public async Task BanPlayer(string playerName)
        {
            if (UserIsHost)
            {
                await _hubConnection.SendAsync("BanPlayer", playerName);
            }
        }

        public async Task StartGame()
        {
            if (UserIsHost && Lobby.GetConnectedPlayersCount() >= Lobby.MinPlayers && Lobby.State == LobbyState.Preparing)
            {
                await _hubConnection.SendAsync("StartGame");
            }
        }

        public async Task SelectSelection(string selection)
        {
            Lobby.SelectSelection(selection);
            await _hubConnection.SendAsync("SetSelection", selection);
        }

        public async Task SendDraw(DrawDetails drawDetails)
        {
            if (UserIsDrawing && Lobby.State == LobbyState.Drawing)
            {
                await _hubConnection.SendAsync("SendDraw", drawDetails);
            }
        }

        public bool UserIsHost
        {
            get
            {
                return User?.Name == Lobby.GetHostPlayer()?.Name;
            }
        }

        public bool UserIsDrawing
        {
            get
            {
                return User?.Name == Lobby.GetDrawingPlayer()?.Name;
            }
        }
    }
}