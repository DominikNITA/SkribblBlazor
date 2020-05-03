using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Skribbl_Website.Server.Services;
using Skribbl_Website.Shared;
using Skribbl_Website.Shared.Dtos;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Skribbl_Website.Server.Controllers
{
    [ApiController]
    [Route("lobbies")]
    public class LobbiesController : ControllerBase
    {
        private readonly LobbiesManager _lobbiesManager;

        public LobbiesController(LobbiesManager lobbiesManager)
        {
            _lobbiesManager = lobbiesManager;
        }

        [HttpGet("create/{name}")]
        public async Task<ActionResult<LobbyRedirectDto>> Get(string name)
        {
            var host = new Player(name);
            var lobbyUrl = _lobbiesManager.CreateLobby(host);
            return new LobbyRedirectDto(host, lobbyUrl);
        }

        [HttpGet("join/{inviteLink}/{name}")]
        public async Task<ActionResult<LobbyRedirectDto>> Get(string inviteLink, string name)
        {
            try
            {
                var player = new Player(name);
                var lobbyUrl = _lobbiesManager.AddPlayerToLobby(inviteLink, player);
                return new LobbyRedirectDto(player, lobbyUrl);
            }
            //Catch custom exception
            catch (Exception exception)
            {
                return new LobbyRedirectDto(exception.Message);
            }
        }

        [HttpGet("join/{inviteLink}")]
        public async Task<ActionResult<NameModel>> GetHostName(string inviteLink)
        {
            try
            {
                var lobby = _lobbiesManager.GetLobbyByInviteLink(inviteLink);
                return new NameModel(lobby.GetHostPlayer().Name);
            }
            //Catch custom exception
            catch (Exception exception)
            {
                return new NameModel(string.Empty);
            }
        }
    }
}