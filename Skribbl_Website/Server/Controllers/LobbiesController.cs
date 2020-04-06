using Microsoft.AspNetCore.Mvc;
using Skribbl_Website.Server.Services;
using Skribbl_Website.Shared.Dtos;
using System;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Skribbl_Website.Server.Controllers
{
    [ApiController]
    [Route("lobbies")]
    public class LobbiesController : ControllerBase
    {
        private LobbiesManager _lobbiesManager;

        public LobbiesController(LobbiesManager lobbiesManager)
        {
            _lobbiesManager = lobbiesManager;
        }

        [HttpGet("create/{name}")]
        async public Task<ActionResult<LobbyRedirectDto>> Get(string name)
        {
            var host = new Player(name);
            var lobbyUrl = _lobbiesManager.CreateLobby(host);
            return new LobbyRedirectDto(host, lobbyUrl);
        }

        [HttpGet("join/{inviteLink}/{name}")]
        async public Task<ActionResult<LobbyRedirectDto>> Get(string inviteLink, string name)
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
    }
}
