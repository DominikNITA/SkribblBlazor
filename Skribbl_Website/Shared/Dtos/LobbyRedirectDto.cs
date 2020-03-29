﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Skribbl_Website.Shared.Dtos
{
    public class LobbyRedirectDto
    {
        public UserDto ConfirmedUser { get; set; }
        public string LobbyUrl { get; set; }
        public string ExceptionMessage { get; set; }
        public bool HasError { get; set; }

        public LobbyRedirectDto(UserDto confirmedUser, string lobbyUrl)
        {
            ConfirmedUser = confirmedUser;
            LobbyUrl = lobbyUrl;
            ExceptionMessage = string.Empty;
            HasError = false;
        }
        public LobbyRedirectDto(string exceptionMessage)
        {
            ExceptionMessage = exceptionMessage;
            HasError = true;
            ConfirmedUser = new UserDto();
            LobbyUrl = string.Empty;
        }
        public LobbyRedirectDto()
        {

        }
    }
}
