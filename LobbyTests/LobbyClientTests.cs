using Skribbl_Website.Shared.Dtos;
using Skribbl_Website.Shared.Exceptions;
using System;
using System.Security.Cryptography;
using Xunit;

namespace LobbyTests
{
    public class LobbyClientTests
    {
        [Fact]
        public void EachLobbyHasDifferentIdByDefault()
        {
            //arrange&act
            var lobbyA = new LobbyClient();
            var lobbyB = new LobbyClient();
            //assert
            Assert.NotEqual(lobbyA.Id, lobbyB.Id);
        }

        [Fact]
        public void EachLobbyHasDifferentInviteLinkByDefault()
        {
            //arrange&act
            var lobbyA = new LobbyClient();
            var lobbyB = new LobbyClient();
            //assert
            Assert.NotEqual(lobbyA.InviteLink, lobbyB.InviteLink);
        }

        [Fact]
        public void NewLobbyHasNoPlayers()
        {
            var lobby = new LobbyClient();
            Assert.Empty(lobby.Players);
        }

        [Fact]
        public void MaxPlayersIsGreaterThanOne()
        {
            var lobby = new LobbyClient();
            Assert.NotInRange(lobby.MaxPlayers, 0, 1);
        }

        [Fact]
        public void AddPlayer_ShouldWork()
        {
            var lobby = new LobbyClient();
            lobby.AddPlayer(new PlayerClient("Some Player"));
            Assert.NotEmpty(lobby.Players);
        }

        [Fact]
        public void AddPlayer_FullLobbyShouldThrowException()
        {
            var lobby = new LobbyClient();
            lobby.MaxPlayers = 2;
            lobby.AddPlayer(new PlayerClient("player1"));
            lobby.AddPlayer(new PlayerClient("player2"));

            Assert.Throws<MaxPlayersReachedException>(() => lobby.AddPlayer(new PlayerClient("player3")));
        }

        [Fact]
        public void AddPlayer_UsernameAlreadyInLobbyShouldThrowException()
        {
            var lobby = new LobbyClient();
            var player = new PlayerClient("player");
            var player2 = new PlayerClient("player");
            lobby.AddPlayer(player);

            Assert.Throws<UserNameAlreadyExistsException>(() => lobby.AddPlayer(player2));
        }

        [Fact]
        public void AddPlayer_NullPlayerShouldThrowException()
        {
            var lobby = new LobbyClient();

            Assert.Throws<ArgumentNullException>(() => lobby.AddPlayer(null));
        }

        [Fact]
        public void RemovePlayerByName_ShoudlWork()
        {
            var lobby = new LobbyClient();
            lobby.Players.Add(new PlayerClient("Some Player"));

            var result = lobby.RemovePlayerByName("Some Player");

            Assert.Empty(lobby.Players);
            Assert.Equal(1, result);
        }

        [Fact]
        public void RemovePlayerByName_FromEmptyLobbyShouldReturnZero()
        {
            var lobby = new LobbyClient();

            var result = lobby.RemovePlayerByName("Nonexistent Player");

            Assert.Equal(0, result);
        }

        [Fact]
        public void GetPlayerByName_SinglePlayerShouldWork()
        {
            var lobby = new LobbyClient();
            var player = new PlayerClient("player");
            lobby.AddPlayer(player);

            var actualPlayer = lobby.GetPlayerByName("player");

            Assert.Equal(player, actualPlayer);
        }

        [Fact]
        public void GetPlayerByName_MultiplePlayersShouldWork()
        {
            var lobby = new LobbyClient();
            var player = new PlayerClient("player");
            var player2 = new PlayerClient("player22");
            lobby.AddPlayer(player);
            lobby.AddPlayer(player2);

            var actualPlayer = lobby.GetPlayerByName("player");

            Assert.Equal(player, actualPlayer);
        }

        [Fact]
        public void GetPlayerByName_NonexistentPlayerShouldThrowException()
        {
            var lobby = new LobbyClient();
            var player = new PlayerClient("player");
            lobby.AddPlayer(player);

            Assert.Throws<ArgumentException>(() => lobby.GetPlayerByName("nonexisting"));
        }

        [Fact]
        public void GetPlayerByName_NullPlayerShouldThrowException()
        {
            var lobby = new LobbyClient();
            var player = new PlayerClient("player");
            lobby.AddPlayer(player);

            Assert.Throws<ArgumentException>(() => lobby.GetPlayerByName(null));
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ChangeConnectionForPlayer_ShouldWork(bool newState)
        {
            var lobby = new LobbyClient();
            var player = new PlayerClient("player");
            lobby.AddPlayer(player);

            lobby.ChangeConnectionForPlayer("player", newState);

            Assert.Equal(newState, player.IsConnected);
        }

        [Fact]
        public void ChangeConnectionForPlayer_NonexistentPlayerPassed_ThrowError()
        {
            var lobby = new LobbyClient();
            var player = new PlayerClient("player");
            lobby.AddPlayer(player);

            Assert.Throws<ArgumentException>(() => lobby.ChangeConnectionForPlayer("nonexistent", true));
        }

        [Fact]
        public void SetDrawingPlayer_ShouldWork()
        {
            var lobby = new LobbyClient();
            var player = new PlayerClient("player");
            lobby.AddPlayer(player);
            player.IsConnected = true;

            lobby.SetDrawingPlayer("player");

            Assert.True(player.IsDrawing);
        }

        [Fact]
        public void SetDrawingPlayer_ChangingBetweenPlayersShouldWork()
        {
            var lobby = new LobbyClient();
            var player1 = new PlayerClient("player1");
            var player2 = new PlayerClient("player2");
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
            var lobby = new LobbyClient();
            var player = new PlayerClient("player");
            lobby.AddPlayer(player);

            Assert.Throws<ArgumentException>(() => lobby.SetDrawingPlayer("nonexistent"));
        }

        [Fact]
        public void GetDrawingPlayer_ShouldWork()
        {
            var lobby = new LobbyClient();
            var player = new PlayerClient("player");
            lobby.AddPlayer(player);
            player.IsConnected = true;
            lobby.SetDrawingPlayer(player.Name);

            var actualDrawingPlayer = lobby.GetDrawingPlayer();

            Assert.Equal(player, actualDrawingPlayer);
        }

        [Fact]
        public void GetDrawingPlayer_NoDrawingPlayerShouldReturnNull()
        {
            var lobby = new LobbyClient();
            var player1 = new PlayerClient("player1");
            lobby.AddPlayer(player1);

            var actualDrawingPlayer = lobby.GetDrawingPlayer();

            Assert.Null(actualDrawingPlayer);
        }

        [Fact]
        public void SetHostPlayer_ShouldWork()
        {
            var lobby = new LobbyClient();
            var player = new PlayerClient("player");
            lobby.AddPlayer(player);
            player.IsConnected = true;

            lobby.SetHostPlayer("player");

            Assert.True(player.IsHost);
        }

        [Fact]
        public void SetHostPlayer_NonexistentPlayer_ThrowError()
        {
            var lobby = new LobbyClient();
            var player = new PlayerClient("player");
            lobby.AddPlayer(player);
            player.IsConnected = true;

            Assert.Throws<ArgumentException>(() => lobby.SetHostPlayer("nonexistent"));
        }

        [Fact]
        public void SetHostPlayer_DisconnectedPlayer_ThrowError()
        {
            var lobby = new LobbyClient();
            var player = new PlayerClient("player");
            lobby.AddPlayer(player);
            player.IsConnected = false;

            Assert.Throws<DisconnectedPlayerException>(() => lobby.SetHostPlayer("player"));
        }

        [Fact]
        public void SetHostPlayer_SwitchHost()
        {
            var lobby = new LobbyClient();
            var player1 = new PlayerClient("player1");
            var player2 = new PlayerClient("player2");
            lobby.AddPlayer(player1);
            lobby.AddPlayer(player2);
            player1.IsConnected = true;
            player2.IsConnected = true;

            lobby.SetHostPlayer("player1");
            lobby.SetHostPlayer("player2");

            Assert.False(player1.IsHost);
            Assert.True(player2.IsHost);
        }

        [Fact]
        public void GetHostPlayer_ShouldWork()
        {
            var lobby = new LobbyClient();
            var player1 = new PlayerClient("player1");
            var player2 = new PlayerClient("player2");
            lobby.AddPlayer(player1);
            lobby.AddPlayer(player2);
            player1.IsConnected = true;
            player2.IsConnected = true;

            lobby.SetHostPlayer("player1");

            var actualHost = lobby.GetHostPlayer();

            Assert.Equal(player1, actualHost);
        }

        [Fact]
        public void GetHostPlayer_NoHostShouldReturnNull()
        {
            var lobby = new LobbyClient();
            var player1 = new PlayerClient("player1");
            lobby.AddPlayer(player1);

            var actualHost = lobby.GetHostPlayer();

            Assert.Null(actualHost);
        }
    }
}
