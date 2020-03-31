using System;
using System.Collections.Generic;
using System.Text;

namespace Skribbl_Website.Shared.Dtos
{
    public class PlayerClient
    {
        private bool isHost;
        private bool isDrawing;
        private bool isConnected;

        public string Name { get; set; }
        public bool IsConnected
        {
            get => isConnected;
            set
            {
                if (value)
                {
                    isConnected = true;
                }
                else
                {
                    isConnected = false;
                    isHost = false;
                    isDrawing = false;
                }
            }

        }
        public int Score { get; set; }

        public bool IsHost
        {
            get => isHost;
            set
            {
                if (IsConnected || !value)
                {
                    isHost = value;
                }
                else
                {
                    throw new Exception();
                }
            }
        }
        public bool IsDrawing
        {
            get => isDrawing;
            set
            {
                if (IsConnected || !value)
                {
                    isDrawing = value;
                }
                else
                {
                    throw new Exception();
                }
            }
        }

        public PlayerClient(string name)
        {
            Name = name;
            IsHost = false;
            IsConnected = false;
            IsDrawing = false;
            Score = 0;
        }
        public PlayerClient()
        {

        }

        public void AddScore(int scoreToAdd)
        {
            if (scoreToAdd < 0)
            {
                throw new ArgumentOutOfRangeException();
            }
            else
            {
                Score += scoreToAdd;
            }
        }

    }
}
