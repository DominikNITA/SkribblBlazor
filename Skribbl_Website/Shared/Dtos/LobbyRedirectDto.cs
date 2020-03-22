using System;
using System.Collections.Generic;
using System.Text;

namespace Skribbl_Website.Shared.Dtos
{
    public class LobbyRedirectDto
    {
        public User ConfirmedUser { get; set; }
        public string LobbyUrl { get; set; }

        public LobbyRedirectDto(User confirmedUser, string lobbyUrl)
        {
            ConfirmedUser = confirmedUser;
            LobbyUrl = lobbyUrl;
        }
        public LobbyRedirectDto()
        {

        }
    }
}
