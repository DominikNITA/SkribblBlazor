using Skribbl_Website.Server.Models;
using Skribbl_Website.Shared.Dtos;
using Skribbl_Website.Shared.Exceptions;
using System;
using Xunit;

namespace LobbyTests
{
    public class LobbyTests
    {
        [Fact]
        public void Lobby_ShouldWork()
        {
            var player = new Player("player");

            var lobby = new Lobby();
            lobby.AddPlayer(player);

            Assert.Contains(player, lobby.Players);
        }

        [Fact]
        public void AddPlayer_FullLobbyShouldThrowException()
        {
            var lobby = new Lobby();
            lobby.AddPlayer(new Player("player1"));
            lobby.MaxPlayers = 2;
            lobby.AddPlayer(new Player("player2"));

            Assert.Throws<MaxPlayersReachedException>(() => lobby.AddPlayer(new Player("player3")));
        }

        [Fact]
        public void AddPlayer_UsernameAlreadyInLobbyShouldThrowException()
        {
            var player = new Player("player");
            var lobby = new Lobby();
            lobby.AddPlayer(player);
            var player2 = new Player("player");

            Assert.Throws<UserNameAlreadyExistsException>(() => lobby.AddPlayer(player2));
        }

        [Fact]
        public void AddPlayer_NullPlayerShouldThrowException()
        {
            var lobby = new Lobby();

            Assert.Throws<ArgumentNullException>(() => lobby.AddPlayer(null));
        }

        [Fact]
        public void GetPlayerById_ShouldWork()
        {
            var player1 = new Player("player1");
            var player2 = new Player("player2");
            var lobby = new Lobby();
            lobby.AddPlayer(player1);
            lobby.AddPlayer(player2);

            var actualUser = lobby.GetPlayerById(player1.Id);

            Assert.Equal(player1, actualUser);
        }

        [Fact]
        public void GetPlayerById_NonexistentIdShouldThrowException()
        {
            var player = new Player("player");
            var lobby = new Lobby();
            lobby.AddPlayer(player);

            Assert.Throws<ArgumentException>(() => lobby.GetPlayerById("nonexistentId"));
        }

        [Fact]
        public void SetConnectionIdForPlayer_ShouldWork()
        {
            var player = new Player("player");
            var lobby = new Lobby();
            lobby.AddPlayer(player);

            lobby.SetConnectionIdForPlayer("testConnection", player.Name);

            Assert.Equal("testConnection", player.Connection);
        }

        [Fact]
        public void SetConnectionIdForPlayer_NonexistentPlayerShouldThrowException()
        {
            var player = new Player("player");
            var lobby = new Lobby();
            lobby.AddPlayer(player);

            Assert.Throws<ArgumentException>(() => lobby.SetConnectionIdForPlayer("testConnection", "nonexistentPlayer"));
        }

        [Fact]
        public void SetConnectionIdForPlayer_NullOrEmptyConnectionShouldThrowException()
        {
            var player = new Player("player");
            var lobby = new Lobby();
            lobby.AddPlayer(player);

            Assert.Throws<ArgumentException>(() => lobby.SetConnectionIdForPlayer(null, player.Name));
            Assert.Throws<ArgumentException>(() => lobby.SetConnectionIdForPlayer("", player.Name));
        }

    }
}
