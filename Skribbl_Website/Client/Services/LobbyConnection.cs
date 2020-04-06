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
                Messages.Add(message);
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

                //Console.WriteLine(lobbyClientDto);
                Lobby = new LobbyClient(lobbyClientDto);
                Console.WriteLine("In LOBBY receive");
                Console.WriteLine(Lobby.Players.Count);
                Console.WriteLine(Lobby.InviteLink);
                Console.WriteLine(lobbyClientDto.Players.Count);
                Console.WriteLine(lobbyClientDto.InviteLink);
                InvokeOnReceive();
            });

            _hubConnection.On<string>("SetLobbyString", (lobbyJson) =>
            {
                //Lobby = new LobbyClient(lobbyClientDto);
                Console.WriteLine("In lobby STRING receive");
                Console.WriteLine(lobbyJson);
                try
                {
                    var lobbyDto = JsonSerializer.Deserialize<LobbyClientDto>(lobbyJson);
                    Console.WriteLine(lobbyDto == null);
                    Console.WriteLine("DTO player1: " + lobbyDto.Players[0].Name);
                    Lobby = new LobbyClient(lobbyDto);
                }
                catch(Exception error)
                {
                    Console.WriteLine(error.Message);
                }

                //Console.WriteLine(Lobby.Players.Count);
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
