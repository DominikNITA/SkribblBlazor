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
        public LobbyDto Lobby { get; set; }
        public List<Message> Messages { get; set; } = new List<Message>(); 

        private HubConnection _hubConnection;

        protected virtual void OnReceive(EventArgs e)
        {
            StateChanged?.Invoke(this, e);
        }

        private void InvokeOnReceive()
        {
            OnReceive(EventArgs.Empty);
        }

        public event EventHandler StateChanged;

        public LobbyConnection()
        {

        }

        public async Task StartConnection(UserDto user, Uri hubUrl, string lobbyId)
        {
            Console.WriteLine("hello");
            _hubConnection = new HubConnectionBuilder().WithUrl(hubUrl).Build();
            User = user;
            _hubConnection.On<Message>("ReceiveMessage", (message) =>
            {
                Messages.Add(message);
                InvokeOnReceive();
            });

            _hubConnection.On<LobbyDto>("ReceiveLobbyState", (lobby) =>
            {
                Lobby = lobby;
                InvokeOnReceive();
            });

            _hubConnection.On<PlayerDto>("AddPlayer", (player) =>
            {
                //TODO: Check for same player already
                Lobby.Players.Add(player);
                InvokeOnReceive();
            });

            await _hubConnection.StartAsync();

            await Join(lobbyId);
        }
        Task Send() =>
_hubConnection.SendAsync("SendMessage", User.Name, "Hello from SEND");

        Task Join(string lobbyId) =>
        _hubConnection.SendAsync("AddToGroup", User.Id, lobbyId);
    }
}
