using Skribbl_Website.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace LobbyTests
{
    public class UserDtoTests
    {
        [Fact]
        public void UserDto_TwoUsersShouldHaveDifferentIds()
        {
            var user1 = new UserDto("player1");
            var user2 = new UserDto("player2");

            Assert.NotEqual(user1.Id, user2.Id);
        }
    }
}
