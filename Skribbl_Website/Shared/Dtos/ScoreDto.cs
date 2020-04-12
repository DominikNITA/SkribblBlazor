using System;
using System.Collections.Generic;
using System.Text;

namespace Skribbl_Website.Shared.Dtos
{
    public class ScoreDto
    {
        public string PlayerName { get; set; }
        public int ScoreToAdd { get; set; }

        public ScoreDto(string playerName, int scoreToAdd)
        {
            PlayerName = playerName;
            ScoreToAdd = scoreToAdd;
        }

        public ScoreDto()
        {

        }
    }
}
