using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;
using Skribbl_Website.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Skribbl_Website.Client.Services
{
    public class LobbyConnection
    {
        public UserDto User { get; set; }
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

        public async Task StartConnection(UserDto user, Uri hubUrl, string lobbyId)
        {
            _hubConnection = new HubConnectionBuilder().WithUrl(hubUrl).Build();
            User = user;
            _hubConnection.On<Message>("ReceiveMessage", (message) =>
            {
                Messages.Add(message);
                InvokeOnReceive();
            });

            _hubConnection.On<LobbyClient>("ReceiveLobbyState", (lobby) =>
            {
                Lobby = lobby;
                InvokeOnReceive();
            });

            _hubConnection.On<PlayerClient>("AddPlayer", (playerToAdd) =>
            {
                if(!Lobby.Players.Any(p => p.Name == playerToAdd.Name))
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

            await _hubConnection.StartAsync();
            try
            {
                await Join(lobbyId);
            }
            catch
            {
                // User does not hace access to demanded lobby
                OnError();
            }
        }
        public void CloseConnection()
        {
            _hubConnection.StopAsync();
            //TODO: Clear UserState
        }

        Task Send() =>
_hubConnection.SendAsync("SendMessage", User.Name, "Hello from SEND");

        Task Join(string lobbyId) =>
        _hubConnection.InvokeAsync("AddToGroup", User.Id, lobbyId);
    }
}
