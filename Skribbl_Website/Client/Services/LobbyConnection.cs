using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Skribbl_Website.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Skribbl_Website.Client.Services
{
    public class LobbyConnection
    {
        public Player User { get; set; }
        public LobbyClient Lobby { get; set; }
        public List<Message> Messages { get; set; } = new List<Message>();

        private HubConnection _hubConnection;

        private void InvokeOnReceive()
        {
            StateChanged?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler StateChanged;

        protected virtual void OnError()
        {
            ErrorOccured?.Invoke(this, EventArgs.Empty);
        }
        public event EventHandler ErrorOccured;

        public LobbyConnection()
        {

        }

        public async Task StartConnection(Player user, Uri hubUrl, string lobbyId)
        {
            _hubConnection = new HubConnectionBuilder().WithUrl(hubUrl).AddJsonProtocol(options => {
                options.PayloadSerializerOptions.PropertyNamingPolicy = null;
            }).WithAutomaticReconnect().Build();
            User = user;

            _hubConnection.On<Message>("ReceiveNewHost", (message) =>
            {
                Messages.Add(new Message(message.MessageContent,message.Type));
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

            _hubConnection.On<string>("RemovePlayer", (playerNameToRemove) =>
            {
                Lobby.RemovePlayerByName(playerNameToRemove);
                InvokeOnReceive();
            });

            _hubConnection.On<LobbySettings>("ReceiveLobbySettings", (lobbySettings) => 
            {
                Console.WriteLine("In ReceiveLobbySettings!");
                Lobby.LobbySettings = lobbySettings;
                Console.WriteLine("Rounds: " + Lobby.LobbySettings.RoundsLimit);
                InvokeOnReceive();
            });

            await _hubConnection.StartAsync();
            try
            {
                await Join(lobbyId);
            }
            catch
            {
                // User does not hace access to demanded lobby
                Console.WriteLine("Catched error from hub");
                OnError();
            }
        }
        public void CloseConnection()
        {
            _hubConnection.StopAsync();
            //TODO: Clear UserState
        }

        public Task Send(string messageContent) =>
_hubConnection.SendAsync("SendMessage", new Message(messageContent,Message.MessageType.Guess,User.Name));

        Task Join(string lobbyId) =>
        _hubConnection.InvokeAsync("AddToGroup", User.Id, lobbyId);

        public Task UpdateLobbySettings()
        {
            if(Lobby.GetHostPlayer().Name == User.Name)
            {
                Console.WriteLine(Lobby.LobbySettings.RoundsLimit);
                return _hubConnection.SendAsync("SendLobbySettings", Lobby.LobbySettings);
            }
            else
            {
                return Task.CompletedTask;
            }
        }
    }
}
