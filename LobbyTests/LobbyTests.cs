﻿using Skribbl_Website.Server.Models;
using Skribbl_Website.Shared.Dtos;
using Skribbl_Website.Shared.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace LobbyTests
{
    public class LobbyTests
    {
        [Fact]
        public void Lobby_ShouldWork()
        {
            var user = new UserDto("player");

            var lobby = new Lobby(user);

            Assert.Contains(user, lobby.Players);
            Assert.Empty(lobby.Connections);
            Assert.Equal(string.Empty, lobby.HostConnection);
        }

        [Fact]
        public void Lobby_NullUserShouldThrowException()
        {
            Assert.Throws<ArgumentNullException>(() => new Lobby(null));
        }

        [Fact]
        public void AddPlayer_FullLobbyShouldThrowException()
        {
            var lobby = new Lobby(new UserDto("player1"));
            lobby.MaxPlayers = 2;
            lobby.AddPlayer((PlayerClient)new UserDto("player2"));

            Assert.Throws<MaxPlayersReachedException>(() => lobby.AddPlayer(new PlayerClient("player3")));
        }

        [Fact]
        public void AddPlayer_UsernameAlreadyInLobbyShouldThrowException()
        {
            var lobby = new Lobby(new UserDto("player"));
            var player2 = new UserDto("player");

            Assert.Throws<UserNameAlreadyExistsException>(() => lobby.AddPlayer(player2));
        }

        [Fact]
        public void AddPlayer_NullPlayerShouldThrowException()
        {
            Assert.Throws<ArgumentNullException>(() => new Lobby(null));
        }

        [Fact]
        public void GetUserById_ShouldWork()
        {
            var user1 = new UserDto("user1");
            var user2 = new UserDto("user2");
            var lobby = new Lobby(user1);
            lobby.AddPlayer(user2);

            var actualUser = lobby.GetUserById(user1.Id);

            Assert.Equal(user1, actualUser);
        }
    }
}
