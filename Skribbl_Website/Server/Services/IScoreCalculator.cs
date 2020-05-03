using System.Collections.Generic;
using Skribbl_Website.Shared.Dtos;

namespace Skribbl_Website.Server.Services
{
    public interface IScoreCalculator
    {
        void StartCounting();

        void AddPlayer(string playerName);

        List<ScoreDto> GetScores(int maxPlayers);
    }
}