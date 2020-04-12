using Skribbl_Website.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Skribbl_Website.Server.Services
{
    public class SimpleScoreCalculator : IScoreCalculator
    {
        private List<string> _order;
        public void AddPlayer(string playerName)
        {
            _order.Add(playerName);
        }

        public List<ScoreDto> GetScores(int maxPlayers)
        {
            List<ScoreDto> newScores = _order.Select((player) => new ScoreDto(player, maxPlayers - _order.IndexOf(player))).ToList();
            return newScores;
        }

        public void StartCounting()
        {
            
        }
    }
}
