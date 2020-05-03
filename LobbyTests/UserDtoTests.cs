using Skribbl_Website.Shared.Dtos;
using Xunit;

namespace LobbyTests
{
    public class UserDtoTests
    {
        [Fact]
        public void UserDto_TwoUsersShouldHaveDifferentIds()
        {
            var user1 = new Player("player1");
            var user2 = new Player("player2");

            Assert.NotEqual(user1.Id, user2.Id);
        }
    }
}