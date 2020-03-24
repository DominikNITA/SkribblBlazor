using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Skribbl_Website.Server.Models;
using Skribbl_Website.Server.Services;
using Skribbl_Website.Shared;
using Skribbl_Website.Shared.Dtos;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Skribbl_Website.Server.Controllers
{
    [ApiController]
    [Route("lobbies")]
    public class LobbiesController : Controller
    {
        public LobbiesManager _lobbiesManager { get; set; }

        public LobbiesController(LobbiesManager lobbiesManager)
        {
            _lobbiesManager = lobbiesManager;
        }

        // GET: api/<controller>
        [HttpGet("create/{name}")]
        async public Task<ActionResult<LobbyRedirectDto>> Get(string name)
        {
            var host = new UserDto(name);
            var lobbyUrl = _lobbiesManager.CreateLobby(host);
            return new LobbyRedirectDto(host, lobbyUrl);
        }

        // GET api/<controller>/5
        [HttpGet("join/{inviteLink}/{name}")]
        async public Task<ActionResult<LobbyRedirectDto>> Get(string inviteLink, string name)
        {
            var player = new UserDto(name);
            try
            {
                var lobbyUrl = _lobbiesManager.AddPlayerToLobby(inviteLink, player);
                return new LobbyRedirectDto(player, lobbyUrl);
            }
            catch (Exception)
            {
                throw;
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
                    if(lobby.Users.Where(user => user.Id == userId).Count() == 1)
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

        //// POST api/<controller>
        //[HttpPost]
        //public void Post([FromBody]string value)
        //{
        //}

        //// PUT api/<controller>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE api/<controller>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
