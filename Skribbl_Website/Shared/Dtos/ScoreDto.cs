namespace Skribbl_Website.Shared.Dtos
{
    public class ScoreDto
    {
        public ScoreDto(string playerName, int scoreToAdd)
        {
            PlayerName = playerName;
            ScoreToAdd = scoreToAdd;
        }

        public ScoreDto()
        {
        }

        public string PlayerName { get; set; }
        public int ScoreToAdd { get; set; }
    }
}