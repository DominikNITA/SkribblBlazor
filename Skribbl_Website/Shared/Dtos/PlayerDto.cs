using System;
using System.Collections.Generic;
using System.Text;

namespace Skribbl_Website.Shared.Dtos
{
    public class PlayerDto
    {
        public string Name { get; set; }
        public bool IsHost { get; set; }
        public bool IsConnected { get; set; }
        public bool IsDrawing { get; set; }
        public int Score { get; set; }

        public PlayerDto(string name)
        {
            Name = name;
            IsHost = false;
            IsConnected = false;
            IsDrawing = false;
            Score = 0;
        }

        public PlayerDto()
        {

        }
    }
}
