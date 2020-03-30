using Skribbl_Website.Client.Pages;
using Skribbl_Website.Shared.Dtos;
using Skribbl_Website.Shared.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Moq;

namespace LobbyTests
{
    public class LobbyDtoTests
    {
        [Fact]
        public void EachLobbyHasDifferentIdByDefault()
        {
            //arrange&act
            var lobbyA = new LobbyDto();
            var lobbyB = new LobbyDto();
            //assert
            Assert.NotEqual(lobbyA.Id, lobbyB.Id);
        }

        [Fact]
        public void EachLobbyHasDifferentInviteLinkByDefault()
        {
            //arrange&act
            var lobbyA = new LobbyDto();
            var lobbyB = new LobbyDto();
            //assert
            Assert.NotEqual(lobbyA.InviteLink, lobbyB.InviteLink);
        }

        [Fact]
        public void NewLobbyHasNoPlayers()
        {
            var lobby = new LobbyDto();
            Assert.Empty(lobby.Players);
        }

        [Fact]
        public void MaxPlayersIsGreaterThanOne()
        {
            var lobby = new LobbyDto();
            Assert.NotInRange(lobby.MaxPlayers, 0, 1);
        }

        [Fact]
        public void AddingPlayerToLobbyWorks()
        {
            var lobby = new LobbyDto();
            lobby.AddPlayer(new PlayerDto("Some Player"));
            Assert.NotEmpty(lobby.Players);
        }

        [Fact]
        public void AddingPlayerToFullLobbyThrowsException()
        {
            var lobby = new LobbyDto();
            lobby.MaxPlayers = 2;
            lobby.AddPlayer(new PlayerDto("player1"));
            lobby.AddPlayer(new PlayerDto("player2"));

            Assert.Throws<MaxPlayersReachedException>(() => lobby.AddPlayer(new PlayerDto("player3")));
        }

        [Fact]
        public void AddPlayer_UsernameAlreadyInLobby_ThrowError()
        {
            var lobby = new LobbyDto();
            var player = new PlayerDto("player");
            var player2 = new PlayerDto("player");
            lobby.AddPlayer(player);

            Assert.Throws<UserNameAlreadyExistsException>(() => lobby.AddPlayer(player2));
        }

        [Fact]
        public void AddPlayer_NullPlayer_ThrowError()
        {
            var lobby = new LobbyDto();

            Assert.Throws<ArgumentNullException>(() => lobby.AddPlayer(null));
        }

        [Fact]
        public void RemovingPlayerByNameWorks()
        {
            var lobby = new LobbyDto();
            lobby.Players.Add(new PlayerDto("Some Player"));

            var result = lobby.RemovePlayerByName("Some Player");

            Assert.Empty(lobby.Players);
            Assert.Equal(1, result);
        }

        [Fact]
        public void RemovingPlayerByNameFromEmptyLobbyReturnsZero()
        {
            var lobby = new LobbyDto();

            var result = lobby.RemovePlayerByName("Nonexistent Player");

            Assert.Equal(0, result);
        }

        [Fact]
        public void GetPlayerByName_SinglePlayer()
        {
            var lobby = new LobbyDto();
            var player = new PlayerDto("player");
            lobby.AddPlayer(player);

            var actualPlayer = lobby.GetPlayerByName("player");

            Assert.Equal(player, actualPlayer);
        }

        [Fact]
        public void GetPlayerByName_MultiplePlayer()
        {
            var lobby = new LobbyDto();
            var player = new PlayerDto("player");
            var player2 = new PlayerDto("player22");
            lobby.AddPlayer(player);
            lobby.AddPlayer(player2);

            var actualPlayer = lobby.GetPlayerByName("player");

            Assert.Equal(player, actualPlayer);
        }

        [Fact]
        public void GetPlayerByName_NonexistentPlayer()
        {
            var lobby = new LobbyDto();
            var player = new PlayerDto("player");
            lobby.AddPlayer(player);

            Assert.Throws<ArgumentException>(() => lobby.GetPlayerByName("nonexisting"));
        }

        [Fact]
        public void GetPlayerByName_NullPlayer()
        {
            var lobby = new LobbyDto();
            var player = new PlayerDto("player");
            lobby.AddPlayer(player);

            Assert.Throws<ArgumentException>(() => lobby.GetPlayerByName(null));
        }

        //TODO: Change to theory for both below
        [Fact]
        public void ConnectingToLobby()
        {
            var lobby = new LobbyDto();
            var player = new PlayerDto("player");
            lobby.AddPlayer(player);

            lobby.ChangeConnectionToLobby("player", true);

            Assert.True(player.IsConnected);
        }

        [Fact]
        public void DisconnectingFromLobby()
        {
            var lobby = new LobbyDto();
            var player = new PlayerDto("player");
            lobby.AddPlayer(player);

            lobby.ChangeConnectionToLobby("player", false);

            Assert.False(player.IsConnected);
        }

        [Fact]
        public void ChangeConnectionToLobby_NonexistentPlayerPassed_ThrowError()
        {
            var lobby = new LobbyDto();
            var player = new PlayerDto("player");
            lobby.AddPlayer(player);

            Assert.Throws<ArgumentException>(() => lobby.ChangeConnectionToLobby("nonexistent", true));
        }

        [Fact]
        public void SettingDrawingForTheFirstTime()
        {
            var lobby = new LobbyDto();
            var player = new PlayerDto("player");
            lobby.AddPlayer(player);
            player.IsConnected = true;

            lobby.SetDrawingPlayer("player");

            Assert.True(player.IsDrawing);
        }

        [Fact]
        public void ChangingDrawingBetweenPlayers()
        {
            var lobby = new LobbyDto();
            var player1 = new PlayerDto("player1");
            var player2 = new PlayerDto("player2");
            lobby.AddPlayer(player1);
            lobby.AddPlayer(player2);
            player1.IsConnected = true;
            player2.IsConnected = true;

            lobby.SetDrawingPlayer("player1");
            lobby.SetDrawingPlayer("player2");

            Assert.False(player1.IsDrawing);
            Assert.True(player2.IsDrawing);
        }

        [Fact]
        public void SetDrawingPlayer_NonexistentPlayerPassed_ThrowError()
        {
            var lobby = new LobbyDto();
            var player = new PlayerDto("player");
            lobby.AddPlayer(player);

            Assert.Throws<ArgumentException>(() => lobby.SetDrawingPlayer("nonexistent"));
        }

        [Fact]
        public void SetHostPlayer_InitialSetup()
        {
            var lobby = new LobbyDto();
            var player = new PlayerDto("player");
            lobby.AddPlayer(player);
            player.IsConnected = true;

            lobby.SetHostPlayer("player");

            Assert.True(player.IsHost);
        }

        [Fact]
        public void SetHostPlayer_NonexistentPlayer_ThrowError()
        {
            var lobby = new LobbyDto();
            var player = new PlayerDto("player");
            lobby.AddPlayer(player);
            player.IsConnected = true;

            Assert.Throws<ArgumentException>(() => lobby.SetHostPlayer("nonexistent"));
        }

        [Fact]
        public void SetHostPlayer_DisconnectedPlayer_ThrowError()
        {
            var lobby = new LobbyDto();
            var player = new PlayerDto("player");
            lobby.AddPlayer(player);
            player.IsConnected = false;

            Assert.Throws<DisconnectedPlayerException>(() => lobby.SetHostPlayer("player"));
        }

        [Fact]
        public void SetHostPlayer_SwitchHost()
        {
            var lobby = new LobbyDto();
            var player1 = new PlayerDto("player1");
            var player2 = new PlayerDto("player2");
            lobby.AddPlayer(player1);
            lobby.AddPlayer(player2);
            player1.IsConnected = true;
            player2.IsConnected = true;

            lobby.SetHostPlayer("player1");
            lobby.SetHostPlayer("player2");

            Assert.False(player1.IsHost);
            Assert.True(player2.IsHost);
        }
    }
}
