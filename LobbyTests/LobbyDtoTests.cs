using Skribbl_Website.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

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
            lobby.Players.Add(new PlayerDto("Some Player"));
            Assert.NotEmpty(lobby.Players);
        }

        [Fact]
        public void RemovingPlayerByNameWorks()
        {
            var lobby = new LobbyDto();
            lobby.Players.Add(new PlayerDto("Some Player"));
            lobby.RemoveUserByName("Some Player");
            Assert.Empty(lobby.Players);
        }
    }
}
