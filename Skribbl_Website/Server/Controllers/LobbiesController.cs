using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Skribbl_Website.Server.Models;
using Skribbl_Website.Server.Services;
using Skribbl_Website.Shared;
using Skribbl_Website.Shared.Dtos;
using Skribbl_Website.Shared.Exceptions;

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
            var host = new UserDto(name);
            var lobbyUrl = _lobbiesManager.CreateLobby(host);
            return new LobbyRedirectDto(host, lobbyUrl);
        }

        [HttpGet("join/{inviteLink}/{name}")]
        async public Task<ActionResult<LobbyRedirectDto>> Get(string inviteLink, string name)
        {            
            try
            {
                var player = new UserDto(name);
                var lobbyUrl = _lobbiesManager.AddPlayerToLobby(inviteLink, player);
                return new LobbyRedirectDto(player, lobbyUrl);
            }
            //Catch custom exception
            catch(Exception exception)
            {
                return new LobbyRedirectDto(exception.Message);
            }
        }

        [HttpGet("{lobbyId}/{userId}")]
        async public Task<ActionResult<LobbyDto>> GetLobby(string lobbyId, string userId)
        {
            //TODO: Move logic outside
            foreach (var lobby in _lobbiesManager.Lobbies)
            {
                if (lobby.Id == lobbyId)
                {
                    if (lobby.Users.Where(user => user.Id == userId).Count() == 1)
                    {
                        return lobby;
                    }
                    else
                    {
                        return Unauthorized();
                    }

                }
            }
            return NotFound();
        }
    }
}
