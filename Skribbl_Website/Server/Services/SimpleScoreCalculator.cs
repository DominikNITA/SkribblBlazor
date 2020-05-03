using System.Collections.Generic;
using System.Linq;
using Skribbl_Website.Shared.Dtos;

namespace Skribbl_Website.Server.Services
{
    public class SimpleScoreCalculator : IScoreCalculator
    {
        private List<string> _order = new List<string>();

        public void AddPlayer(string playerName)
        {
            _order.Add(playerName);
        }

        public List<ScoreDto> GetScores(int maxPlayers)
        {
            var newScores = _order.Select(player => new ScoreDto(player, maxPlayers - _order.IndexOf(player))).ToList();
            return newScores;
        }

        public void StartCounting()
        {
            _order = new List<string>();
        }
    }
}