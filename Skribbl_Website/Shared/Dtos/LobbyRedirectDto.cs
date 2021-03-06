﻿namespace Skribbl_Website.Shared.Dtos
{
    public class LobbyRedirectDto
    {
        public LobbyRedirectDto(Player confirmedUser, string lobbyUrl)
        {
            //TODO: Change to only send playerID
            ConfirmedUser = confirmedUser;
            LobbyUrl = lobbyUrl;
            ExceptionMessage = string.Empty;
            HasError = false;
        }

        public LobbyRedirectDto(string exceptionMessage)
        {
            ExceptionMessage = exceptionMessage;
            HasError = true;
            ConfirmedUser = new Player();
            LobbyUrl = string.Empty;
        }

        public LobbyRedirectDto()
        {
        }

        public Player ConfirmedUser { get; set; }
        public string LobbyUrl { get; set; }
        public string ExceptionMessage { get; set; }
        public bool HasError { get; set; }
    }
}