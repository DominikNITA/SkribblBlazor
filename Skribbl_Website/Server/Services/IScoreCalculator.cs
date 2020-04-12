using Skribbl_Website.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Skribbl_Website.Server.Services
{
    public interface IScoreCalculator
    {
        void StartCounting();

        void AddPlayer(string playerName);

        List<ScoreDto> GetScores(int maxPlayers);
    }
}
