using System;

namespace Skribbl_Website.Shared.Dtos
{
    public class PlayerClient
    {
        private bool _isConnected;
        private bool _isDrawing;
        private bool _isHost;

        public PlayerClient(string name)
        {
            Name = name;
            IsHost = false;
            IsConnected = false;
            IsDrawing = false;
            HasGuessedCorrectly = false;
            Score = 0;
        }

        public PlayerClient()
        {
        }

        public string Name { get; set; }

        public bool IsConnected
        {
            get => _isConnected;
            set
            {
                if (value)
                {
                    _isConnected = true;
                }
                else
                {
                    _isConnected = false;
                    _isHost = false;
                    _isDrawing = false;
                }
            }
        }

        public int Score { get; set; }

        public bool IsHost
        {
            get => _isHost;
            set
            {
                if (IsConnected || !value)
                {
                    _isHost = value;
                }
                else
                {
                    throw new Exception();
                }
            }
        }

        public bool IsDrawing
        {
            get => _isDrawing;
            set
            {
                if (IsConnected || !value)
                {
                    _isDrawing = value;
                }
                else
                {
                    throw new Exception();
                }
            }
        }

        public bool HasGuessedCorrectly { get; set; }

        public void AddScore(int scoreToAdd)
        {
            if (scoreToAdd < 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            Score += scoreToAdd;
        }
    }
}